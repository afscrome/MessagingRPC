using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MessagingRPC
{
    public class Client : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly DelayedQueue<Message> _inputQueue;
        private readonly DelayedQueue<Message> _outputQueue;
        private readonly IDictionary<string, TaskCompletionSource<Message>> _inProgress = new Dictionary<string, TaskCompletionSource<Message>>();

        public Client(DelayedQueue<Message> outputQueue, DelayedQueue<Message> inputQueue)
        {
            _outputQueue = outputQueue;
            _inputQueue = inputQueue;
            RecieveResponses();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        public Task<Message> SayHello(string name)
        {
            var message = new Message
            {
                CorrelationId = name,
                Value = name
            };

            var completionSource = new TaskCompletionSource<Message>();
            _inProgress.Add(message.CorrelationId, completionSource);
            _outputQueue.Enqueue(message);
            return completionSource.Task;
        }

        public async void RecieveResponses()
        {
            var cancellationToken = _cancellationTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                 var message = await _inputQueue.Dequeue(cancellationToken);

                TaskCompletionSource<Message> tcs;
                if(_inProgress.TryGetValue(message.CorrelationId, out tcs))
                {
                    Debug.WriteLine("CLIENT RECIEVE " + message.CorrelationId);
                    _inProgress.Remove(message.CorrelationId);
                    tcs.SetResult(message);
                }
                else
                {
                    Debug.WriteLine("CLIENT RECIEVE No Callback for " + message.CorrelationId);
                }
            }
        }

    }
}
