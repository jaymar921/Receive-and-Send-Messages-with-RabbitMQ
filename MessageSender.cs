using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receive_and_Send_Messages_with_RabbitMQ
{
    public class MessageSender
    {
        private readonly ConnectionFactory connectionFactory;
        private readonly string exchangeName = "message_exchange";
        private readonly string responseQueueName = "message_response_queue";

        public MessageSender()
        {
            Console.WriteLine("[Sender] Sender Intitializing");
            connectionFactory = new ConnectionFactory() { HostName = "localhost"};
            Console.WriteLine("[Sender] Sender Intitialized");
        }

        public string CreateMessage(string message)
        {
            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();

            // declare an exchange and a queue
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

            var body = Encoding.UTF8.GetBytes(message);

            // Publish the message to the exchange
            channel.BasicPublish(exchangeName, "", null, body);

            Thread.Sleep(100);
            // declare a response queue for receiving responses
            channel.QueueDeclare(responseQueueName, false, false, false, null);
            EventingBasicConsumer responseConsumer = new(channel);
            channel.BasicConsume(responseQueueName, true, responseConsumer);

            // print the response
            string responseMessage = string.Empty;
            responseConsumer.Received += (model, ea) =>
            {
                byte[] responseBody = ea.Body.ToArray();
                responseMessage = Encoding.UTF8.GetString(responseBody);
            };

            channel.Close();
            connection.Close();

            return responseMessage;
        }
    }
}
