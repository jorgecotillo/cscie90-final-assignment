using System;
using System.Diagnostics;
using System.Net;
using Amazon.XRay.Recorder.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using webapp.Models;

namespace webapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Ping()
        {
            return Ok(true);
        }

        public IActionResult Index()
        {
            try
            {
                AWSXRayRecorder.Instance.AddAnnotation("Get", "HomePage");
                _ = AWSXRayRecorder.Instance.TraceMethod<string>("QueryEntity", () => "Simulates a data interaction.");

                // Trace out-going HTTP request
                AWSXRayRecorder.Instance.TraceMethod("Outgoing Http Web Request", () => MakeHttpWebRequest(1));

                CustomSubsegment();

                return View();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private void CustomSubsegment()
        {
            try
            {
                AWSXRayRecorder.Instance.BeginSubsegment("CustomSubsegment");
                // business logic
            }
            catch (Exception e)
            {
                AWSXRayRecorder.Instance.AddException(e);
            }
            finally
            {
                AWSXRayRecorder.Instance.EndSubsegment();
            }
        }


        private static void MakeHttpWebRequest(int id)
        {
            /*HttpWebRequest request = null;
            request = (HttpWebRequest) WebRequest.Create("http://www.microsoft.com");
            using (var response = request.GetResponse()) { }*/
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
