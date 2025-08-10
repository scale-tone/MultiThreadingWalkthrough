using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetDemo.Controllers
{
    public class HomeController : ApiController
    {
/*
        static void StaticTest()
        {
            Trace.TraceError(">>> From static ctor");
        }

        static HomeController()
        {
            Parallel.For(0, 3, (i) => StaticTest());
        }
*/
        // GET: api/Home
        public string Get()
        {
            return "OK";

            var t = AsyncClass.AsyncMethod(); // Task.Run(AsyncClass.AsyncMethod);

            if (t.Wait(TimeSpan.FromSeconds(10)))
            {
                return t.Result;
            }

            return "!!!DEADLOCK!!!";

            /*            
                        var t = this.AsyncWrapperMethod();

                        if (t.Wait(TimeSpan.FromSeconds(10)))
                        {
                            return t.Result;
                        }

                        return "!!!DEADLOCK!!!";
            */
        }

        private async Task<string> AsyncWrapperMethod()
        {
            var result = await AsyncClass.AsyncMethod().ConfigureAwait(false);

            return result;
        }
    }
}
