#region Includes

using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

#endregion

namespace Daishi.AMQP {
    public class RabbitMQConsumers
    {
        public RabbitMQConsumerArguments arguments { get; set; }
        public int prefetch_count { get; set; }
        public bool ack_required { get; set; }
        public bool exclusive { get; set; }
        public string consumer_tag { get; set; }
        public RabbitMQConsumerQueue queue { get; set; }
        public RabbitMQConsumerChannelDetails channel_details { get; set; }
    }

    public class RabbitMQConsumerArguments
    {
        
    }
    public class RabbitMQConsumerQueue
    {
        public string name { get; set; }
        public string vhost { get; set; }
    }

    public class RabbitMQConsumerChannelDetails
    {
        public string name { get; set; }
        public int number { get; set; }
        public string user { get; set; }
        public string connection_name { get; set; }
        public int peer_port { get; set; }
        public string peer_host { get; set; }
    }
}