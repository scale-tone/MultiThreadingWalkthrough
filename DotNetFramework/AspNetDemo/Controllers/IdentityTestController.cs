using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AspNetDemo.Controllers
{
    public class IdentityTestController : ApiController
    {
        private void TestIdentity(string prefix = "")
        {
            Trace.TraceInformation($"{prefix} > Testing identity on a thread {Thread.CurrentThread.ManagedThreadId}");

            var threadIdentity = Thread.CurrentPrincipal.Identity;
            Trace.TraceInformation($"{prefix} >> Thread Identity: {threadIdentity.Name}");

            var claimsPrincipalIdentity = ClaimsPrincipal.Current.Identity;
            Trace.TraceInformation($"{prefix} >> ClaimsPrincipal Identity: {claimsPrincipalIdentity.Name}");

            var windowsIdentity = WindowsIdentity.GetCurrent(false);
            Trace.TraceInformation($"{prefix} >> Windows Identity: {windowsIdentity.Name}");

            var httpContextIdentity = HttpContext.Current?.User?.Identity;
            Trace.TraceInformation($"{prefix} >> HttpContext Identity: {httpContextIdentity?.Name}");
        }

        private void TestSqlConnection(string prefix = "")
        {
            Trace.TraceInformation($"{prefix} > Testing SQL connection on a thread {Thread.CurrentThread.ManagedThreadId}");

            var sqlConnectionString = @"Data Source=.\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            using (var conn = new SqlConnection(sqlConnectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand("SELECT CURRENT_USER;", conn))
                {
                    string userName = cmd.ExecuteScalar().ToString();

                    Trace.TraceInformation($"{prefix} >> SQL user name: {userName}");
                }
            }
        }

        // GET: api/IdentityTest
        public async Task<string> Get()
        {
            var thread = new Thread(() =>
            {
                this.TestIdentity("New thread");
                this.TestSqlConnection("New thread");
            });

            thread.Start();
            thread.Join();

/*
            this.TestIdentity();
            this.TestSqlConnection();
*/

/*
            var t = new Task(() => {

                this.TestIdentity("New task");
                this.TestSqlConnection("New task");
            });

            t.Start();

            t.Wait();
*/

/*
            Parallel.For(0, 2, (i) =>
            {
                this.TestIdentity("Parallel.For");
                this.TestSqlConnection("Parallel.For");
            });
*/
/*
            ThreadPool.QueueUserWorkItem((state) =>
            {
                Thread.Sleep(2000);

                this.TestIdentity("QueueUserWorkItem");
                this.TestSqlConnection("QueueUserWorkItem");
            });
*/

/*
            Task.Run(() => {

                this.TestIdentity();
                this.TestSqlConnection();
            
            });
*/

/*
            this.TestIdentity();
            this.TestSqlConnection();

            await Task.Yield();

            this.TestIdentity();
            this.TestSqlConnection();
*/
            return "OK";
        }
    }
}
