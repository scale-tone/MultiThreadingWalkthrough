using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AspNetDemo.Controllers
{
    public static class AsyncClass
    {
        public static async Task<string> AsyncMethod()
        {
            await Task.Delay(10);

            // Doing something
            Thread.Sleep(100);

            return "Hello from AsyncMethod";
        }
    }
}