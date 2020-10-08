using System;
using System.Threading.Tasks;
using Jint.Native;

namespace RestNexus.JintInterop
{
    public static class NativeFunctions
    {
        public static void RunDeferred(long waitTimeMS, Func<JsValue, JsValue[], JsValue> function)
        {
            Task.Delay(TimeSpan.FromMilliseconds(waitTimeMS)).ContinueWith(t =>
            {
                // the first parameter of the function is the object on which we want to invoke it on,
                // which is "undefined" for a free-standing function.
                // the second parameter is an array of parameter values, which we don't support at
                // this point (and isn't necessary, since we can just capture context).
                // TODO: we might want to do something with the return value; like log it or so?
                _ = function(JsValue.Undefined, Array.Empty<JsValue>());
            }).Forget();
        }

        // https://www.meziantou.net/fire-and-forget-a-task-in-dotnet.htm
        internal static void Forget(this Task task)
        {
            // Only care about tasks that may fault or are faulted,
            // so fast-path for SuccessfullyCompleted and Canceled tasks
            if (!task.IsCompleted || task.IsFaulted)
            {
                _ = ForgetAwaited(task);
            }

            static async Task ForgetAwaited(Task task)
            {
                try
                {
                    // No need to resume on the original SynchronizationContext
                    await task.ConfigureAwait(false);
                }
                catch
                {
                    // Nothing to do here
                }
            }
        }
    }
}
