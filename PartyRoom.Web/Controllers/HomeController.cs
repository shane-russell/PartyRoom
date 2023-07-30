using Microsoft.AspNetCore.Mvc;
using PartyRoom.Web.Models;
using System.Diagnostics;
using movement.Data;
using PartyRoom.Data;
using PartyRoom.Domain;

namespace PartyRoom.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMovementRepository _movementRepository;
        private IRoomRepository _roomRepository;
        private IMapper _mapper;
        private IMovementFactory _movementFactory;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}