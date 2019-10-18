using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace xrabbit.consumer.Consumer
{
    public class SingleQueueConsumer : IHostedService
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public SingleQueueConsumer()
        {
            this.connection = GetConnectionFactory().CreateConnection();
            this.channel = this.connection.CreateModel();

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Register();

            return Task.CompletedTask;
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if(this.connection.IsOpen)
            {
                this.channel.Close();
                this.connection.Close();
            }
            throw new NotImplementedException();
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
            factory.Uri = new Uri("amqp://guest:guest@locahost:5672/xrabbit");

            return factory;
        }

        private void Register()
        {
            this.channel.ExchangeDeclare(exchange: "xrabbit.exchange.direct.xchange1", type: ExchangeType.Direct, durable:true);
            this.channel.QueueDeclare(queue: "xrabbit.queue.q1", durable: true, exclusive:false, autoDelete:false);
            //setting exclusive false will delete the queue when connection closes
            //queue wont survive broker restart if durable is false
            //autodelete is set to false to avoid queue deletion when transmission completes
            this.channel.QueueBind(queue: "xrabbit.queue.q1", exchange: "xrabbit.exchange.direct.xchange1", routingKey: "bind.key.1");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Registered += Consumer_Registered;
            consumer.Received += Consumer_Received;
            consumer.ConsumerCancelled += Consumer_ConsumerCancelled;
            consumer.Shutdown += Consumer_Shutdown;

            this.channel.BasicConsume(queue: "xrabbit.queue.q1", consumer: consumer);
        }

        private void Consumer_Registered(object sender, ConsumerEventArgs e)
        {
            Console.WriteLine("Consumer Registered, Sender Type: " + sender.GetType());
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine("Consumer Received, Sender Type: "+sender.GetType());
            Console.WriteLine("Consumer ACK sending, Sender Type: " + sender.GetType());
            this.channel.BasicAck(e.DeliveryTag, false);
        }

        private void Consumer_ConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            Console.WriteLine("Consumer Cancelled");
            
        }

        private void Consumer_Shutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Consumer Shutdown");
            
        }

      
    }
}
