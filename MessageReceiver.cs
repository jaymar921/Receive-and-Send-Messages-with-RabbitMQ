using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receive_and_Send_Messages_with_RabbitMQ
{
    public class MessageReceiver
    {
        private readonly ConnectionFactory connectionFactory;
        private readonly string exchangeName = "message_exchange";
        private readonly string requestQueueName = "message_request_queue";
        private readonly string responseQueueName = "message_response_queue";

        public MessageReceiver()
        {
            Console.WriteLine("[Reciever] Receiver Intitializing");
            connectionFactory = new ConnectionFactory() { HostName = "localhost"};
            Console.WriteLine("[Reciever] Receiver Intitialized");
        }

        public void RecieverInstance()
        {
            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declare an exchange
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

                // Declare a request queue for receiving messages
                channel.QueueDeclare(requestQueueName, false, false, false, null);
                channel.QueueBind(requestQueueName, exchangeName, "");

                // Declare a response queue for sending responses
                channel.QueueDeclare(responseQueueName, false, false, false, null);

                // Set up a consumer to listen for incoming messages
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"[Reciever] Received message: {message}");

                    // Simulate some processing
                    // You can replace this with your actual processing logic
                    System.Threading.Thread.Sleep(100);

                    // Return a response
                    string responseMessage = $"Processed: {message}";
                    var responseBody = Encoding.UTF8.GetBytes(responseMessage);

                    // Publish the response to the response queue
                    channel.BasicPublish("", responseQueueName, null, responseBody);
                    Console.WriteLine($"[Reciever] Sending response to Sender: {responseMessage}");
                };

                // Consume messages from the request queue
                channel.BasicConsume(requestQueueName, true, consumer);

                Console.WriteLine("[Reciever] Receiver is Running");
                Console.ReadLine();
            }
        }
    }
}
