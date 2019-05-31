using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lab10.Data;
using Lab10.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Lab10.Controllers
{
    [AllowAnonymous]
    public class StoryBlockController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration _configuration;

        public StoryBlockController(ApplicationDbContext context, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _context = context;
           _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        /*---Fetching the story block for a given storyID
         *---Before returning the model the action sorts the storyblocks
         *---in descending order based on the time when it was added */
        public IActionResult StoryBlock(int id)
        {
            
            if (!(_context.Stories.Any(o => o.StoriesID == id)))
            {
                return RedirectToAction("Error", "Home");
            }
            var storyBlock = _context.StoryBlocks.Include(l => l.Stories)
                .Where(m => m.StoriesID == id)
                .OrderBy(sort=>sort.StoryBlockID)
                .Select(l => l);

            ViewData["StoryID"] = id;
            return View(storyBlock);
        }

        [HttpGet]
        public IActionResult AddComment(int id)
        {
            var comment = new Comments();
            return View(comment);
        }

        [HttpPost]
        public IActionResult AddComment(int id,Comments comment)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            comment.StoriesID = id;
            comment.Commenter = User.Identity.Name;
            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("StoryBlock", "StoryBlock", new { id = comment.StoriesID });
        }
    }
}