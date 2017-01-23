#region Includes

using System;
using System.Collections.Generic;

#endregion

namespace Daishi.AMQP {
    public abstract class AMQPConsumer {
        protected readonly string queueName;
        
        protected readonly ushort prefetchCount;
        protected readonly bool noAck;

        protected readonly bool connectBinding;
        protected readonly string exchangeName;
        protected readonly string routingKeyName;

        protected readonly bool createQueue;
        protected readonly bool durable;
        protected readonly bool exclusive;
        protected readonly bool autoDelete;

        protected readonly int timeout;
        protected readonly bool implicitAck;
        protected readonly IDictionary<string, object> queueArgs;
        protected volatile bool stopConsuming;

        protected AMQPConsumer(string queueName, int timeout, 
            ushort prefetchCount = 1, bool noAck = false,
            bool connectBinding = false, string exchangeName = "", string routingKeyName = "",
            bool createQueue = true, bool durable = true, bool exclusive = false, bool autoDelete = false, 
            bool implicitAck = true, IDictionary<string, object> queueArgs = null)
        {
            this.queueName = queueName;
            this.prefetchCount = prefetchCount;
            this.noAck = noAck;

            this.connectBinding = connectBinding;
            this.exchangeName = exchangeName;
            this.routingKeyName = routingKeyName;

            this.createQueue = createQueue;
            this.durable = durable;
            this.exclusive = exclusive;
            this.autoDelete = autoDelete;

            this.timeout = timeout;
            this.implicitAck = implicitAck;
            this.queueArgs = queueArgs;
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public virtual void Start(AMQPAdapter amqpAdapter) {
            stopConsuming = false;
        }

        public void Stop() {
            stopConsuming = true;
        }

        protected void OnMessageReceived(MessageReceivedEventArgs e) {
            var handler = MessageReceived;
            if (handler != null) handler(this, e);
        }
    }
}