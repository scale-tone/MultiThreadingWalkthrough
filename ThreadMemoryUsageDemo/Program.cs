using System;
using System.Diagnostics;
using System.Threading;

namespace ThreadMemoryUsageDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // This is just a basic sample to show how much memory a single thread takes.
            // Default max stack size is 1MB, let's change it to 4MB.
            int stackSize = 1 << 22;

            int i = 1;
            while (true)
            {
                Console.WriteLine("Press any key to start a thread #" + i++);
                Console.ReadKey();

                new Thread(() =>
                    {
                        while (true) Thread.Sleep(1000);
                    }, 
                    // explicitly setting stack size
                    stackSize
                ).Start();

                var proc = Process.GetCurrentProcess();
                Console.WriteLine("Working set: {0}MB, Private: {1}MB, Virtual: {2}MB", 
                    proc.WorkingSet64 >> 20, 
                    proc.PrivateMemorySize64 >> 20, 
                    proc.VirtualMemorySize64 >> 20
                );
            }
        }
    }
}
