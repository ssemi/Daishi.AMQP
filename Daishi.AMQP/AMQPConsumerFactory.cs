#region Includes

using System.Collections.Generic;

#endregion

namespace Daishi.AMQP {
    public abstract class AMQPConsumerFactory {
        public abstract AMQPConsumer CreateAMQPConsumer(string queueName, int timeout, ushort prefetchCount = 1, bool noAck = false,
            bool connectBinding = false, string exchangeName = "", string routingKeyName = "",
            bool createQueue = true, bool durable = true, bool exclusive = false, bool autoDelete = false, 
            bool implicitAck = true, IDictionary<string, object> queueArgs = null);
    }
}