#region Includes

using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

#endregion

namespace Daishi.AMQP {
    public class RabbitMQAdapter : AMQPAdapter {
        private static readonly RabbitMQAdapter _instance = new RabbitMQAdapter();
        private IConnection _connection;

        static RabbitMQAdapter() {}

        public static RabbitMQAdapter Instance
        {
            get { return _instance; }
        }

        public override bool IsConnected
        {
            get { return _connection != null && _connection.IsOpen; }
        }

        public override AMQPAdapter Init(string hostName, int port, string userName, string password, ushort heartbeat) {
            this.hostName = hostName;
            this.port = port;
            this.userName = userName;
            this.password = password;
            this.heartbeat = heartbeat;
            return this;
        }

        public override AMQPAdapter Init(string hostName, string virtualHost, int port, string userName, string password, ushort heartbeat) {
            this.hostName = hostName;
            this.virtualHost = virtualHost;
            this.port = port;
            this.userName = userName;
            this.password = password;
            this.heartbeat = heartbeat;
            return this;
        }

        public override void Connect() {
            var connectionFactory = new ConnectionFactory {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password,
                RequestedHeartbeat = heartbeat
            };

            if (!string.IsNullOrEmpty(virtualHost)) connectionFactory.VirtualHost = virtualHost;
            _connection = connectionFactory.CreateConnection();
        }

        public override void Disconnect() {
            if (_connection != null) _connection.Dispose();
        }

        public override object GetConnection() {
            return _connection;
        }

        public override void Publish(string message, string queueName, bool createQueue = true,
            IBasicProperties messageProperties = null, IDictionary<string, object> queueArgs = null) {
            if (!IsConnected) Connect();
            using (var channel = _connection.CreateModel()) {
                if (createQueue) channel.QueueDeclare(queueName, true, false, false, queueArgs);
                var payload = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(string.Empty, queueName,
                    messageProperties ?? RabbitMQProperties.CreateDefaultProperties(channel), payload);
            }
        }

        public override void Publish(string message, string exchangeName, string routingKey,
            IBasicProperties messageProperties = null) {
            if (!IsConnected) Connect();
            using (var channel = _connection.CreateModel()) {
                var payload = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchangeName, routingKey,
                    messageProperties ?? RabbitMQProperties.CreateDefaultProperties(channel), payload);
            }
        }

        public override bool TryGetNextMessage(string queueName, out string message, out BasicDeliverEventArgs eventArgs, int timeout,
            bool createQueue = true, bool noAck = false, bool implicitAck = true, IDictionary<string, object> queueArgs = null) {
            if (!IsConnected) Connect();
            using (var channel = _connection.CreateModel()) {
                if (createQueue) channel.QueueDeclare(queueName, true, false, false, queueArgs);
                channel.BasicQos(0, 1, false);

                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queueName, noAck, consumer);

                var messageIsAvailable = consumer.Queue.Dequeue(timeout * 1000, out eventArgs);

                if (messageIsAvailable) {
                    message = Encoding.UTF8.GetString(eventArgs.Body);
                    if (implicitAck && !noAck) channel.BasicAck(eventArgs.DeliveryTag, false);
                    return true;
                }

                message = null;
                return false;
            }
        }

        public override void AcknowledgeMessage(ulong deliveryTag) {
            if (!IsConnected) Connect();
            using (var channel = _connection.CreateModel())
                channel.BasicAck(deliveryTag, false);
        }

        public override T GetApi<T>(string path)
        {
            if (!IsConnected) Connect();

            var authorizationParameter = String.Join(":", this.userName, this.password);
            var base64EncodedString = Base64Encoder.GetBase64Encoded(authorizationParameter);

            using (var wc = new WebClient())
            {
                wc.Headers.Add("Authorization", String.Format("Basic {0}", base64EncodedString));
                var uri = String.Format("{1}://{0}:15672/api/{2}", this.hostName, this.hostName.StartsWith("https") ? "https" : "http", path);
                var str = wc.DownloadString(uri);
                if (!String.IsNullOrEmpty(str))
                    return JsonConvert.DeserializeObject<T>(str);
            }
            return (T)Convert.ChangeType(null, typeof(T));
        }

        public async override Task<T> GetApiAsync<T>(string path)
        {
            if (!IsConnected) Connect();

            var authorizationParameter = String.Join(":", this.userName, this.password);
            var base64EncodedString = Base64Encoder.GetBase64Encoded(authorizationParameter);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedString);
                var uri = String.Format("{1}://{0}:15672/api/{2}", this.hostName, this.hostName.StartsWith("https") ? "https" : "http", path);
                var str = await client.GetStringAsync(uri);
                if (!String.IsNullOrEmpty(str))
                    return JsonConvert.DeserializeObject<T>(str);
            }
            return (T)Convert.ChangeType(null, typeof(T));
        }
    }
}