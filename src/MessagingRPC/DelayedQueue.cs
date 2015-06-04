using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MessagingRPC
{
    public class DelayedQueue<T>
    {
        private readonly ConcurrentQueue<T> _internalQueue = new ConcurrentQueue<T>();
        private readonly TimeSpan _delay = TimeSpan.FromMilliseconds(500);

        public DelayedQueue(TimeSpan delay)
        {
            _delay = delay;
        }

        public void Enqueue(T message)
        {
            _internalQueue.Enqueue(message);
        }

        public async Task<T> Dequeue(CancellationToken cancellationToken)
        {
            while (true) {
                cancellationToken.ThrowIfCancellationRequested();
                T result;
                if (_internalQueue.TryDequeue(out result))
                    return result;

                await Task.Delay(_delay, cancellationToken);
            }
        }
    }
}
