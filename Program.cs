namespace Receive_and_Send_Messages_with_RabbitMQ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            

            var receiverTask = Task.Run(() =>
            {
                MessageReceiver receiver = new MessageReceiver();
                receiver.RecieverInstance();
            });

            var senderTask = Task.Run(() =>
            {
                Thread.Sleep(100);
                MessageSender sender = new MessageSender();
                Console.WriteLine($"[Sender] Sending: 'This is a message'");
                string response = sender.CreateMessage("'This is a message'");
                Console.WriteLine($"[Sender] Received Response from Receiver: {response}");
            });

            Task.WhenAll(receiverTask, senderTask).Wait();

            Console.WriteLine("App was closed");
        }
    }
}