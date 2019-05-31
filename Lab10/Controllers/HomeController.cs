using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using Lab10.Models;
using Lab10.Data;
using Microsoft.AspNetCore.Authorization;

namespace Lab10.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context_;
        private const string session_id = "SessionId";

        public HomeController(ApplicationDbContext context)
        {
            context_ = context;
        }

        public IActionResult Index()
        {
            return View(context_.Categories.ToList<Categories>());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult readStory()
        {
            return View(context_.Categories.ToList<Categories>());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }  
}
