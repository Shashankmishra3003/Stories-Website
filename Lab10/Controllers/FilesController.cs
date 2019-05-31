using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Lab10.Models;
using Lab10.Data;

namespace Lab10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IHostingEnvironment hostingEnvironment_;
        private string webRootPath = null;
        private string filePath = null;
        private string strImagePath = null;
        private readonly ApplicationDbContext context_;

        public FilesController(IHostingEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            hostingEnvironment_ = hostingEnvironment;
            webRootPath = hostingEnvironment_.WebRootPath;
            filePath = Path.Combine(webRootPath, "FileStorage/StoryBlockContent");
            strImagePath = Path.Combine(webRootPath, "FileStorage/Images");
            context_ = context;

            List<string> files = null;
            files = Directory.GetFiles(filePath).ToList<string>();
            for (int i = 0; i < files.Count; ++i)
            {
                files[i] = Path.GetFileName(files[i]);
            }

        }

        /*---Returning the list of Categories with ID's to the console client to display on the console---*/

        [HttpGet]
        public IEnumerable<string> Get()
        {
            List<string> categoryList = new List<string>();
            var category = context_.Categories.ToList();
            foreach (var c in category)
            {
                categoryList.Add(c.CategoriesID.ToString() + "-" + c.categoryName.ToString());
            }
            return categoryList;
        }

        /*---Returning the list of Stories with ID's to the console client to display on the console---*/

        [HttpGet("{id}")]
        public IEnumerable<string> SendBookList(int id)
        {
            List<string> bookList = new List<string>();
            var story = context_.Stories.ToList();
            foreach (var b in story)
            {
                bookList.Add(b.StoriesID.ToString() + "-" + b.storyName.ToString());
            }
            return bookList;
        }

        [HttpGet("{id}/{idd}")]
        public IEnumerable<string> SendBlockList(int id, int idd)
        {
            List<string> blockList = new List<string>();
            var story = context_.StoryBlocks
                .Where(l => l.StoriesID == idd);
            foreach (var b in story)
            {
                blockList.Add(b.StoryBlockID.ToString() + "-" + b.StoryBlockName);
            }
            return blockList;
        }



        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
      {
        {".cs", "application/C#" },
        {".txt", "text/plain"},
        {".pdf", "application/pdf"},
        {".doc", "application/vnd.ms-word"},
        {".docx", "application/vnd.ms-word"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
        {".png", "image/png"},
        {".jpg", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".gif", "image/gif"},
        {".csv", "text/csv"}
      };
        }
        //----< upload file >--------------------------------------

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            var request = HttpContext.Request;
            var blockFile = request.Form.Files[0];
            var blockImage = request.Form.Files[1];
            var storyImage = request.Form.Files[2];
            var date = DateTime.Now.ToString();
            {
                if (blockFile.Length > 0)
                {
                    var path = Path.Combine(filePath, blockFile.FileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await blockFile.CopyToAsync(fileStream);
                    }

                    path = Path.Combine(filePath, blockImage.FileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await blockImage.CopyToAsync(fileStream);
                    }
                    path = Path.Combine(strImagePath, storyImage.FileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await storyImage.CopyToAsync(fileStream);
                    }

                    /*---If a new Story is being added for the first time, saves the story into 
                     *--- the database to generate its StoryID
                      *--- The IF statement checks the type of operation being performed
                      *--- A new Story or just a new block---*/

                    if (request.Form["OperationType"].Equals("Story"))
                    {
                        var story = new Stories
                        {
                            storyName = request.Form["StoryName"],
                            storyDescription = request.Form["Descp"],
                            ImageName = storyImage.FileName,
                            CategoriesID = Int32.Parse(request.Form["Category"])
                        };
                        context_.Stories.Add(story);
                        context_.SaveChanges();

                        /*---Fetches the StoryID of the newly added story inorder to add the storyblock
                         *--- the story Block needs storyID to link the two tables---*/

                        var newId = context_.Stories.Where(s => s.storyName == story.storyName && s.CategoriesID == story.CategoriesID).First();

                        /*---Adding the story block for the newly added story into the StoryBlock Table---*/

                        var block = new StoryBlock
                        {
                            StoryBlockName = Path.GetFileNameWithoutExtension(blockFile.FileName),
                            FileName = blockFile.FileName,
                            ImageName = blockImage.FileName,
                            StoriesID = newId.StoriesID,
                            Timeline = Convert.ToDateTime(date)
                        };
                        context_.StoryBlocks.Add(block);
                        context_.SaveChanges();
                    }

                    /*---Adding Story block for an existing Story into the StoryBlock table---*/
                    else
                    {
                        var block = new StoryBlock
                        {
                            StoryBlockName = Path.GetFileNameWithoutExtension(blockFile.FileName),
                            FileName = blockFile.FileName,
                            ImageName = blockImage.FileName,
                            StoriesID = Int32.Parse(request.Form["Story"]),
                            Timeline = Convert.ToDateTime(date)
                        };
                        context_.StoryBlocks.Add(block);
                        context_.SaveChanges();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            return Ok();
        }


        [HttpPost("{x}/{xx}")]
        public async Task<IActionResult> ReplaceBlock()
        {
            var request = HttpContext.Request;
            var blockFile = request.Form.Files[0];
            var blockImage = request.Form.Files[1];
            {
                if (blockFile.Length > 0)
                {
                    var path = Path.Combine(filePath, blockFile.FileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await blockFile.CopyToAsync(fileStream);
                    }

                    path = Path.Combine(filePath, blockImage.FileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await blockImage.CopyToAsync(fileStream);
                    }
                    /*---Replace an existing Story block with a new block into StoryBlock table---*/
                    var newdate = DateTime.Now.ToString();
                    var block = context_.StoryBlocks.Find(Int32.Parse(request.Form["replaceId"]));
                    block.StoryBlockName = Path.GetFileNameWithoutExtension(blockFile.FileName);
                    block.FileName = blockFile.FileName;
                    block.ImageName = blockImage.FileName;
                    block.Timeline = Convert.ToDateTime(newdate);
                    context_.Update(block);
                    context_.SaveChanges();

                }
                else
                {
                    return BadRequest();
                }
            }
            return Ok();
        }

        [HttpPost("{id}")]
        public IActionResult UpdateTimeLine()
        {
            var request = HttpContext.Request;
            var date = DateTime.Now.ToString();
            var block = context_.StoryBlocks.Find(Int32.Parse(request.Form["BlockId"]));
            block.Timeline = Convert.ToDateTime(date);
            context_.Update(block);
            context_.SaveChanges();
            return Ok();
        }

        [HttpPost("{y}/{yy}/{yyy}")]
        public IActionResult DeleteBlock()
        {
            var request = HttpContext.Request;
            var date = DateTime.Now.ToString();
            var block = context_.StoryBlocks.Find(Int32.Parse(request.Form["DeleteId"]));
            context_.Remove(block);
            context_.SaveChanges();
            return Ok();
        }

    }
}