using System;
using System.Linq;
using System.Threading.Tasks;

namespace MessagingRPC
{
    public class Program
    {
        private string[] _names = new[] { "Jack", "Jill", "James", "Bill", "Bob", "Tony", "Alice", "Ellie", "Becky", "Steve", "Sally" };

        public async Task Main(string[] args)
        {
            var requestHelloQueue = new DelayedQueue<Message>(TimeSpan.FromMilliseconds(5));
            var saidHelloQueue = new DelayedQueue<Message>(TimeSpan.FromMilliseconds(5));

            var pubSubServer = new Server(requestHelloQueue, saidHelloQueue);
            var pubSubClient = new Client(requestHelloQueue, saidHelloQueue);

            await Task.WhenAll(_names.Select(x => SayHello(pubSubClient, x)));

            Console.WriteLine("Press any key to continue");
            Console.Read();
        }

        private async Task SayHello(Client pubSubClient, string name)
        {
            Console.WriteLine("Saying hello to " + name);
            var result = await pubSubClient.SayHello(name);
            Console.WriteLine(result.Value);

        }

    }


}
