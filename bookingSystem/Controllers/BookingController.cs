using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using bookingSystem.Models;
using System.Net;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bookingSystem.Controllers
{
    public class BookingController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            Console.WriteLine("Controller start");
            string url = "https://pl-vacc.org.pl/newvacc/API/";
            string bookings = "booking.php";
            HttpClient httpclient = new HttpClient();

            httpclient.BaseAddress = new Uri(url);
            httpclient.DefaultRequestHeaders.Accept.Clear();
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage httpresponse = httpclient.GetAsync(bookings).Result;
            Console.WriteLine("dl data finish");
            System.Diagnostics.Trace.TraceInformation("downloading");

            var bookingList = new List<Booking>();
            if (httpresponse.IsSuccessStatusCode)
            {
                // Parse the response body.
                var rawData = httpresponse.Content.ReadAsStringAsync().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                var jsonData = (JObject)JsonConvert.DeserializeObject(rawData);
                var payload = jsonData["payload"];
                foreach (var booking in payload)
                {
                    Booking bkng = booking.First.ToObject<Booking>();
                    System.Diagnostics.Trace.TraceInformation(bkng.position);
                    bookingList.Add(bkng);
                }
            }
            else
            {
                System.Diagnostics.Trace.TraceInformation(" ERR: {0} ({1})", (int)httpresponse.StatusCode, httpresponse.ReasonPhrase);

                Console.WriteLine(" ERR: {0} ({1})", (int)httpresponse.StatusCode, httpresponse.ReasonPhrase);
            }
            ViewBag.bookings = bookingList;
            httpclient.Dispose();
            foreach (Booking book in ViewBag.bookings)
            {
                Console.WriteLine(book.position);
            }
            return View();
        }
    }
}
