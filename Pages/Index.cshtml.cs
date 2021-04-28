using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SL.Models;
using SL.Models.Departure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SL.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public HttpClient client = new HttpClient();

        public Stops hpl { get; set; }

        [BindProperty(SupportsGet = true)]
        public string siteId { get; set; }

        public Departures departure { get; set; }

        [BindProperty(SupportsGet = true)]
        public string location { get; set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (location != null)
            {
                HttpContext.Session.SetString("location", location);
            }
            else
            {
                if(HttpContext.Session.GetString("location") !=null)
                {
                    location = HttpContext.Session.GetString("location");
                }
                else
                { 
                    location = "";
                }
            }
            hpl = await GetStopsAsync();
            departure = await GetDeparturesAsync();
            return Page();
        }

        public async Task<Stops> GetStopsAsync()
        {
            string link = $"https://api.sl.se/api2/typeahead.json?key=eb5a71e54f8f4727a309b248bfd0be68&searchstring={location}n&stationsonly=true&maxresults=10";
            Task<string> getBusstopStringTask = client.GetStringAsync(link);
            string busstopString = await getBusstopStringTask;
            hpl = JsonSerializer.Deserialize<Stops>(busstopString);
            return hpl;
        }
        public async Task<Departures> GetDeparturesAsync()
        {
            string link = $"https://api.sl.se/api2/realtimedeparturesV4.json?key=3a7f3f2cc36548e79e819cb7f8068049&siteid={siteId}&timewindow=15";
            Task<string> getBusDepartureStringTask = client.GetStringAsync(link);
            string busDepartureString = await getBusDepartureStringTask;
            departure = JsonSerializer.Deserialize<Departures>(busDepartureString);
            return departure;
        }
    }
}
