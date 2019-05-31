using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab10.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Lab10.Models;

namespace Lab10.Components
{ 
    public class CommentViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration _configuration;
        public CommentViewComponent(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IViewComponentResult> InvokeAsync(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                var commentListAdmin = _context.Comments
                    .Where(s => s.StoriesID == id);
                return View(commentListAdmin);
            }

            var commentList = _context.Comments
                .Where(s => s.StoriesID == id)
                .Where(k => k.Commenter == User.Identity.Name);
            return await Task.FromResult((IViewComponentResult)View("Default", commentList));
        }
    }
}
