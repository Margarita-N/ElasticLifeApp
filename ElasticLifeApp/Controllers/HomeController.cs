using ElasticLifeApp.Models;
using ElasticLifeApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ElasticLifeApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ElasticService _elasticService;

        public HomeController(
            ILogger<HomeController> logger,
            ElasticService elasticService)
        {

            _logger = logger;
            _elasticService = elasticService;

            _elasticService.CheckForData();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[Action]")]
        public IActionResult Search(SearchRequestModel request)
        {
            var results = _elasticService.Search(request);
            return View(results);
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