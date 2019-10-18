using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xrabbit.consumer.Publisher
{
    public class MessagePublisher
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        public MessagePublisher()
        {
           
            this.connection= GetConnectionFactory().CreateConnection();
            this.channel = this.connection.CreateModel();
            this.Register();
        }

        private ConnectionFactory GetConnectionFactory()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                Port = 5672,
                VirtualHost = "/"
            };
            return factory;

        }
        private ConnectionFactory GetConnectionFactoryURI()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@locahost:5672/"); //amqp://guest:guest@locahost:5672/<vhostname>

            return factory;
        }


        private void Register()
        {
            this.channel.ExchangeDeclare(exchange: "xrabbit.exchange.direct.xchange1", type: ExchangeType.Direct, durable: true);

            //Since, this has already been declared in Consumer, Lock Exception is occuring
            //this.channel.QueueDeclare(queue: "xrabbit.queue.q1", durable: false);
            //this.channel.QueueBind(queue: "xrabbit.queue.q1", exchange: "xrabbit.exchange.direct.xchange1", routingKey: "bind.key.1");
        }
        public bool PublishMessage(string message)
        {
            bool isSent = false;
            this.channel.ConfirmSelect();
            var messageBytes = Encoding.UTF8.GetBytes(message);
            
            this.channel.BasicPublish(exchange: "xrabbit.exchange.direct.xchange1", routingKey: "bind.key.1", body: messageBytes);
            this.channel.WaitForConfirmsOrDie();
            this.channel.BasicAcks += (ch, e) => {
                Console.WriteLine("Publisher ACK, Sender Type: " + ch.GetType());
                isSent = true;
            };

            return isSent;
        }


        private IBasicProperties GetBasicProperties()
        {
            IBasicProperties basicProperties = this.channel.CreateBasicProperties();
            basicProperties.ContentType = "text/plain";
            basicProperties.DeliveryMode = 2;
            basicProperties.Persistent = true;
            return basicProperties;
        }
    }
}
