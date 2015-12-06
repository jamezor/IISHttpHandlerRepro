using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;

namespace Reproducer
{
    class Program
    {
        static void Main()
        {
            ConfirmEverythingIsSetUp();
            Console.WriteLine("Everything seems to be configured. Let's start the test...");

            var clientThreads = StartClientThreads();

            while(!Console.KeyAvailable)
            {
                var processStartInfo = new ProcessStartInfo("iisreset")
                {
                    CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden
                };

                Process.Start(processStartInfo).WaitForExit();
                Console.Write("iisreset performed... ");
            }

            runClientThreads = false;
            clientThreads.Join();
        }

        static bool runClientThreads = true;

        static Thread StartClientThreads()
        {
            var thread = new Thread(() =>
            {
                var activeThreads = new List<Thread>();

                using(HttpClient client = new HttpClient())
                {
                    while(runClientThreads)
                    {
                        activeThreads.Add(LaunchHttpClientThread(client));
                        activeThreads.RemoveAll(t => !t.IsAlive);

                        Thread.Sleep(20);
                    }

                    activeThreads.ForEach(t => t.Join());
                }
            });

            thread.Start();
            return thread;
        }

        static Thread LaunchHttpClientThread(HttpClient client)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    var result = client.GetAsync(new Uri("http://localhost:81/HttpHandlerApp/subpath")).GetAwaiter().GetResult();
                    var resultString = result.Content.ReadAsStringAsync().Result;
                    if(result.StatusCode == HttpStatusCode.OK && resultString != "This is the location scoped handler")
                        Console.WriteLine("Got the wrong result: " + resultString);
                }
                catch
                {
                }
            });
            thread.Start();

            return thread;
        }

        static void ConfirmEverythingIsSetUp()
        {
            var isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            if (!isAdmin)
            {
                throw new InvalidOperationException("You must run this script with administrative privileges.");
            }

            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(new Uri("http://localhost:81/HttpHandlerApp/")).GetAwaiter().GetResult();
                var resultString = result.Content.ReadAsStringAsync().Result;
                if(result.StatusCode != HttpStatusCode.OK || resultString != "This is the default handler")
                    throw new Exception("Something is not set up right, the default scoped handler should respond to requests against app root.");

                result = client.GetAsync(new Uri("http://localhost:81/HttpHandlerApp/subpath")).GetAwaiter().GetResult();
                resultString = result.Content.ReadAsStringAsync().Result;
                if(result.StatusCode != HttpStatusCode.OK || resultString != "This is the location scoped handler")
                    throw new Exception("Something is not set up right, the location scoped handler should respond to requests against 'subpath'");
            }
        }
    }
}
