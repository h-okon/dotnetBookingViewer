using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using bookingSystem.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
namespace bookingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string url = "https://pl-vacc.org.pl/newvacc/API/";
            string bookings = "booking.php";
            string bookings_today = "today_booking.php";
            string bookings_tomorrow = "tomorrow_booking.php";
            HttpClient httpclient = new HttpClient();
            // get all bookings
            httpclient.BaseAddress = new Uri(url);
            httpclient.DefaultRequestHeaders.Accept.Clear();
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage httpresponse = httpclient.GetAsync(bookings).Result;
            ViewBag.status = httpresponse.StatusCode;
            var bookingList = new List<Booking>();
            var todayBookingList = new List<Booking>();
            var tomorrowBookingList = new List<Booking>();
            if (httpresponse.IsSuccessStatusCode)
            {
                // Parse the response body.
                try
                {
                    var rawData = httpresponse.Content.ReadAsStringAsync().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                    var jsonData = (JObject)JsonConvert.DeserializeObject(rawData);
                    var payload = jsonData["payload"];
                    int i = 0;
                    foreach (var booking in payload)
                    {
                        i++;
                        Booking bkng = booking.First.ToObject<Booking>();
                        bookingList.Add(bkng);
                        if (i == 5)
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(" Exception occured.");

                }

            }
            else
            {
                Console.WriteLine(" ERR: {0} ({1})", (int)httpresponse.StatusCode, httpresponse.ReasonPhrase);
            }

            /*
             * Today bookings
             */
            httpclient.Dispose();
            httpclient = new HttpClient();
            httpclient.BaseAddress = new Uri(url);
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage httpresponseTodayBookings = httpclient.GetAsync(bookings_today).Result;
            if (httpresponseTodayBookings.IsSuccessStatusCode)
            {
                // Parse the response body.
                try
                {
                    var rawData = httpresponseTodayBookings.Content.ReadAsStringAsync().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                    var jsonData = (JObject)JsonConvert.DeserializeObject(rawData);
                    foreach (var booking in jsonData)
                    {

                        Console.WriteLine(booking.Value);
                        Booking bkng = booking.Value.ToObject<Booking>();
                        todayBookingList.Add(bkng);
                    }
                }
                
                catch (Exception e)
                {
                    Console.WriteLine(" Exception occured.");
                }
            }
            else
            {
                Console.WriteLine(" ERR: {0} ({1})", (int)httpresponse.StatusCode, httpresponse.ReasonPhrase);
            }

            /*
             * Tomorrow bookings
             */
            httpclient.Dispose();
            httpclient = new HttpClient();
            httpclient.BaseAddress = new Uri(url);
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage httpresponseTomorrowBookings = httpclient.GetAsync(bookings_tomorrow).Result;
            if (httpresponseTomorrowBookings.IsSuccessStatusCode )
            {
                // Parse the response body.
                try
                {
                    var rawData = httpresponseTomorrowBookings.Content.ReadAsStringAsync().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                    var jsonData = (JObject)JsonConvert.DeserializeObject(rawData);
                    foreach (var booking in jsonData)
                    {
                        Booking bkng = booking.Value.ToObject<Booking>();
                        tomorrowBookingList.Add(bkng);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(" Exception occured.");
                }
                
            }
            else
            {
                Console.WriteLine(" ERR: {0} ({1})", (int)httpresponse.StatusCode, httpresponse.ReasonPhrase);
            }





            ViewBag.allBookings = bookingList;
            ViewBag.tomorrowBookings = tomorrowBookingList;
            ViewBag.todayBookings = todayBookingList;
            httpclient.Dispose();
            return View();
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
