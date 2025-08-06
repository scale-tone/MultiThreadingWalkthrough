using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveObjectDemo
{
    class Program
    {
        static Task _taskQueue = Task.CompletedTask;

        static void EnqueueTask(Func<Task> todo)
        {
            _taskQueue = _taskQueue
                .ContinueWith((_) => todo())
                .Unwrap();
        }

        static void EnqueueAction(Action todo)
        {
            _taskQueue = _taskQueue
                .ContinueWith((_) => todo());
        }

        static void NonSequentialEnqueueTask(Task todo)
        {
            _taskQueue = _taskQueue
                .ContinueWith((_) => todo)
                .Unwrap();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Press ESC to quit, any other key to enqueue a job");

            int i = 0;
            while (true)
            {
                var k = Console.ReadKey();

                if (k.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("Waiting for all jobs to finish and quitting");
                    break;
                }

                int j = i++;
                Console.WriteLine($"Enqueueing job {j}");

                //                EnqueueAction(() => DoSomethingLengthy(j));
                //                EnqueueTask(() => DoSomethingLengthyAsync(j));

                NonSequentialEnqueueTask(DoSomethingLengthyAsync(j));
            }

            _taskQueue.Wait();
        }

        static void DoSomethingLengthy(int j)
        {
            Console.WriteLine($"Job {j} started");

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Console.WriteLine($"Job {j} finished");
        }

        static async Task DoSomethingLengthyAsync(int j)
        {
            Console.WriteLine($"Job {j} started");

            await Task.Delay(TimeSpan.FromSeconds(1));

            Console.WriteLine($"Job {j} finished");
        }


        static Task<string> _myLazyInitTask = Task.Run(() => {

            Thread.Sleep(TimeSpan.FromSeconds(5));

            return "Some long-awaited data";
        });

    }
}
