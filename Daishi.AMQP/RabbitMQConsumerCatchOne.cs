#region Includes

using System.Collections.Generic;

#endregion

namespace Daishi.AMQP {
    internal class RabbitMQConsumerCatchOne : RabbitMQConsumer {
        public RabbitMQConsumerCatchOne(string queueName, int timeout,  ushort prefetchCount = 1, bool noAck = false,
            bool connectBinding = false, string exchangeName = "", string routingKeyName = "",
            bool createQueue = true, bool durable = true, bool exclusive = false, bool autoDelete = false, 
            bool implicitAck = true, IDictionary<string, object> queueArgs = null) :
                base(queueName, timeout, prefetchCount, noAck, connectBinding, exchangeName, routingKeyName, createQueue, durable, exclusive, autoDelete, implicitAck, queueArgs) { }

        public override void Start(AMQPAdapter amqpAdapter) {
            base.Start(amqpAdapter);
            Start(amqpAdapter, false);
        }
    }
}