using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DummyClient
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var t = new Client();
            Thread.Sleep(3000);
            while (true)
            {
                t.LogPoint();
                Thread.Sleep(500);
                t.GetAll();
            }
        }
    }

    class Client
    {
        private static string Url = "http://localhost:6001/api/values/";

        private void FireRequest()
        {
            var urlWithArgs = string.Format("{0}PostGpsValue?lat={1}&longitude={2}&time={3}", 
                Url, float.Parse(string.Format("{0}.44{1}",31, DateTime.Now.Second)),
                float.Parse(string.Format("{0}.13{1}", 35, DateTime.Now.Second)),
                DateTime.Now);
            try
            {
                using (var wc = new WebClient())
                {
                    var res = wc.DownloadString(urlWithArgs);
                    Console.WriteLine(res);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        private void FireGetAllRequest()
        {
            var urlWithArgs = string.Format("{0}getallgpsdata", Url);
            try
            {
                using (var wc = new WebClient())
                {
                    var res = wc.DownloadString(urlWithArgs);
                    Console.WriteLine(res.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }
        public async void GetAll()
        {
            try
            {
                await Task.Run(() => FireGetAllRequest());
            }
            catch (Exception e)
            {

            }
        }
        public async void LogPoint()
        {
            try
            {
                await Task.Run(() => FireRequest());
            }
            catch (Exception e)
            {

            }
        }
    }
}
