using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altered.Shared.Extensions
{
    public static class TaskExtensions
    {
        // https://youtu.be/-cJjnVTJ5og?t=3385
        public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
        {
            using (var cts = new CancellationTokenSource())
            {
                var delayTask = Task.Delay(timeout, cts.Token);

                var resultTask = await Task.WhenAny(task, delayTask);

                if (resultTask == delayTask)
                {
                    throw new OperationCanceledException();
                }
                else
                {
                    cts.Cancel();
                }

                return await task;
            }
        }
    }
}
