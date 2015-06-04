using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MessagingRPC
{
    public class Server : IDisposable
    {
        private readonly DelayedQueue<Message> _inputQueue;
        private readonly DelayedQueue<Message> _outputQueue;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Random _random = new Random();

        public Server(DelayedQueue<Message> inputQueue, DelayedQueue<Message> outputQueue)
        {
            _inputQueue = inputQueue;
            _outputQueue = outputQueue;

            Process();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        private async void Process()
        {
            var cancellationToken = _cancellationTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                var recievedMessage = await _inputQueue.Dequeue(cancellationToken);
                Debug.WriteLine("SERVER RECIEVE " + recievedMessage.CorrelationId);

                var messageValue = recievedMessage.Value;
                var outgoingMessage = new Message
                {
                    CorrelationId = recievedMessage.CorrelationId,
                    Value = "Hello " + messageValue + "!"
                };

                Task.Delay(_random.Next(0, 1000))
                    .ContinueWith(task =>
                    {
                        Debug.WriteLine("SERVER SEND    " + recievedMessage.CorrelationId);
                        _outputQueue.Enqueue(outgoingMessage);
                    });

            }
        }
    }
}
