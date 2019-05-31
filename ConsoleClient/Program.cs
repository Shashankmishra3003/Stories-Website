using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Collections;

namespace ConsoleClient
{
    class Program
    {

        public HttpClient client { get; set; }

        private string baseUrl_;

        Program(string url)
        {
            baseUrl_ = url;
            client = new HttpClient();
        }
        //----< upload file >--------------------------------------

        public async Task<HttpResponseMessage> SendFile(string storyName, string storyImage, string storydesc,
            string blockFileName, string blockImageName, string intputCategory)
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            //Story Block File
            byte[] data = File.ReadAllBytes(blockFileName);
            ByteArrayContent bytes = new ByteArrayContent(data);

            //Story Block Image
            byte[] image = File.ReadAllBytes(blockImageName);
            ByteArrayContent imageContent = new ByteArrayContent(image);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            //Story Name
            byte[] sName = Encoding.ASCII.GetBytes(storyName);
            ByteArrayContent stry = new ByteArrayContent(sName);

            //Story Description
            byte[] description = Encoding.ASCII.GetBytes(storydesc);
            ByteArrayContent desc = new ByteArrayContent(description);

            //Story Image
            byte[] sImage = File.ReadAllBytes(storyImage);
            ByteArrayContent sImgContent = new ByteArrayContent(sImage);
            sImgContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            //Story Category
            byte[] category = Encoding.ASCII.GetBytes(intputCategory);
            ByteArrayContent catg = new ByteArrayContent(category);

            //For indetifying the type
            string indentifier = "Story";
            byte[] indentity = Encoding.ASCII.GetBytes(indentifier);
            ByteArrayContent idnt = new ByteArrayContent(indentity);


            string fileName = Path.GetFileName(blockFileName);
            string imageName = Path.GetFileName(blockImageName);
            string storyImgName = Path.GetFileName(storyImage);

            multiContent.Add(bytes, "files", fileName);
            multiContent.Add(imageContent, "image", imageName);
            multiContent.Add(sImgContent, "image", storyImgName);
            multiContent.Add(desc, "Descp");
            multiContent.Add(stry, "StoryName");
            multiContent.Add(catg, "Category");
            multiContent.Add(idnt, "OperationType");

            return await client.PostAsync(baseUrl_, multiContent);
        }
        //----< get list of files in server FileStorage >----------

        public async Task<HttpResponseMessage> SendStoryBlock(string blockFileName, string blockImageName,
            string StoryCategory)
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            //Story Block File
            byte[] data = File.ReadAllBytes(blockFileName);
            ByteArrayContent bytes = new ByteArrayContent(data);

            //Story Block Image
            byte[] image = File.ReadAllBytes(blockImageName);
            ByteArrayContent imageContent = new ByteArrayContent(image);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            //Dummy story Image (Not used)
            string storyImage = "city.jpg";
            byte[] sImage = File.ReadAllBytes(storyImage);
            ByteArrayContent sImgContent = new ByteArrayContent(sImage);
            sImgContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            byte[] story = Encoding.ASCII.GetBytes(StoryCategory);
            ByteArrayContent stry = new ByteArrayContent(story);

            string fileName = Path.GetFileName(blockFileName);
            string imageName = Path.GetFileName(blockImageName);
            string storyImgName = Path.GetFileName(storyImage);

            multiContent.Add(bytes, "files", fileName);
            multiContent.Add(imageContent, "image", imageName);
            multiContent.Add(sImgContent, "image", storyImgName);
            multiContent.Add(stry, "Story");

            return await client.PostAsync(baseUrl_, multiContent);
        }

        public async Task<HttpResponseMessage> ReplaceStoryBlock(string blockFileName, string blockImageName,
            string blockId)
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            //Story Block File
            byte[] data = File.ReadAllBytes(blockFileName);
            ByteArrayContent bytes = new ByteArrayContent(data);

            //Story Block Image
            byte[] image = File.ReadAllBytes(blockImageName);
            ByteArrayContent imageContent = new ByteArrayContent(image);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            //Dummy story Image (Not used)

            byte[] blkId = Encoding.ASCII.GetBytes(blockId);
            ByteArrayContent blck = new ByteArrayContent(blkId);

            string fileName = Path.GetFileName(blockFileName);
            string imageName = Path.GetFileName(blockImageName);

            multiContent.Add(bytes, "files", fileName);
            multiContent.Add(imageContent, "image", imageName);
            multiContent.Add(blck, "replaceId");

            return await client.PostAsync(baseUrl_ + "/1" + "/1", multiContent);
        }

        public async Task<HttpResponseMessage> sendNewTimeLine(string blockId)
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            byte[] storyBlock = Encoding.ASCII.GetBytes(blockId);
            ByteArrayContent strBId = new ByteArrayContent(storyBlock);

            multiContent.Add(strBId, "BlockId");
            return await client.PostAsync(baseUrl_ + "/1", multiContent);
        }

        //delete a story block

        public async Task<HttpResponseMessage> DeleteStoryBlock(string blockId)
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            byte[] storyBlock = Encoding.ASCII.GetBytes(blockId);
            ByteArrayContent strBId = new ByteArrayContent(storyBlock);

            multiContent.Add(strBId, "DeleteId");
            return await client.PostAsync(baseUrl_ + "/1" + "/1" + "/1", multiContent);
        }
        //----< gets the list of Categories >------

        public async Task<IEnumerable<string>> GetFileList()
        {
            HttpResponseMessage resp = await client.GetAsync(baseUrl_);
            var files = new List<string>();
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                JArray jArr = (JArray)JsonConvert.DeserializeObject(json);
                foreach (var item in jArr)
                    files.Add(item.ToString());
            }
            return files;
        }

        //----< gets the list of Stories >-----
        public async Task<IEnumerable<string>> GetStoryList()
        {
            HttpResponseMessage resp = await client.GetAsync(baseUrl_ + "/1");
            var files = new List<string>();
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                JArray jArr = (JArray)JsonConvert.DeserializeObject(json);
                foreach (var item in jArr)
                    files.Add(item.ToString());
            }
            return files;
        }

        //----< gets the list of Story blocks for the given Story Id >------
        public async Task<IEnumerable<string>> GetBlockList(int id)
        {
            HttpResponseMessage resp = await client.GetAsync(baseUrl_ + "/" + id.ToString() + "/" + id.ToString());
            var files = new List<string>();
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                JArray jArr = (JArray)JsonConvert.DeserializeObject(json);
                foreach (var item in jArr)
                    files.Add(item.ToString());
            }
            return files;
        }

        public async Task<HttpResponseMessage> GetFile(int id)
        {
            return await client.GetAsync(baseUrl_ + "/" + id.ToString());
        }
        //----< delete the id-th file from FileStorage >-----------

        public async Task<HttpResponseMessage> DeleteFile(int id)
        {
            return await client.DeleteAsync(baseUrl_ + "/" + id.ToString());
        }
        //----< usage message shown if command line invalid >------

        static void showUsage()
        {
            Console.Write("\n  Command line syntax error: expected usage:\n");
            Console.Write("\n    http[s]://machine:port /option [filespec]\n\n");
        }
        //----< validate the command line >------------------------

        static bool parseCommandLine(string[] args)
        {
            if (args.Length < 2)
            {
                showUsage();
                return false;
            }
            if (args[0].Substring(0, 4) != "http")
            {
                showUsage();
                return false;
            }
            if (args[1][0] != '/')
            {
                showUsage();
                return false;
            }
            return true;
        }
        //----< display command line arguments >-----------------3--

        static void showCommandLine(string[] args)
        {
            string arg0 = args[0];
            string arg1 = args[1];
            string arg2;
            if (args.Length == 3)
                arg2 = args[2];
            else
                arg2 = "";
            Console.Write("\n  CommandLine: {0} {1} {2}", arg0, arg1, arg2);
        }

        static string checkInput(string input)
        {
            string argsVal = "";
            if (input.Equals("1"))
            {
                argsVal = "/up";
            }
            else
                argsVal = "exit";
            return argsVal;
        }

        static void Main(string[] args)
        {
            string input = "";
            while (!(input.Equals("5")))
            {
                Console.Write("\n  InStory Console Client");
                Console.Write("\n =====================================================\n");
                Console.Write("\n  Choose an option from the below list to Proceed");
                Console.Write("\n     1> Add a new Story or Story Block");
                Console.Write("\n     2> Update Timeline of Story Block");
                Console.Write("\n     3> Replace a Story Block with a new One");
                Console.Write("\n     4> Delete a Story Block");
                Console.Write("\n     5> Exit");
                Console.Write("\n =====================================================\n");


                input = Console.ReadLine();
                string url = "https://localhost:44398/api/Files";
                Program client = new Program(url);

                Console.Write("\n  sending request to {0}\n", url);

                switch (input)
                {
                    case "1":
                        Task<IEnumerable<string>> CatList = client.GetFileList();
                        var resultcat = CatList.Result;
                        Console.Write("List of available categories:");
                        foreach (var item in resultcat)
                        {
                            Console.Write("\n  {0}", item);
                        }

                        Console.Write("\n     Select a Category ID to which you want to upload the story");
                        string intputCategory = Console.ReadLine();
                        bool correctcategory = false;

                        foreach (var item in resultcat)
                        {
                            if (item.Contains(intputCategory))
                                correctcategory = true;
                        }

                        try
                        {
                            string storyOption = "";

                            if (correctcategory)
                            {
                                Console.Write("\n  Select an Option");
                                Console.Write("\n  1> Enter a new Story topic");
                                Console.Write("\n  2> Add a Story bock to an existing Story");
                                Console.Write("\n  3> Exit");
                                storyOption = Console.ReadLine();

                                switch (storyOption)
                                {
                                    case "1":
                                        Console.Write("\n  Enter the name of the Story:  ");
                                        string storyName = Console.ReadLine();
                                        Console.Write("\n  Enter a Short description of the story\n    ");
                                        string storydesc = Console.ReadLine();
                                        Console.Write("\n  Enter the name fo the Image file for the Story:  ");
                                        string storyImage = Console.ReadLine();
                                        Console.Write("\n  Enter the Filename of first Story block:  ");
                                        string blockFileName = Console.ReadLine();
                                        Console.WriteLine("\n  Enter the name of the block image file:  ");
                                        string BlockImageName = Console.ReadLine();

                                        Task<HttpResponseMessage> tup = client.SendFile(storyName, storyImage, storydesc,
                                            blockFileName, BlockImageName, intputCategory);
                                        Console.Write(tup.Result);
                                        break;

                                    case "2":
                                        Console.Write("\n     Select a Story to which you want to upload a story block:  ");
                                        Task<IEnumerable<string>> tfl = client.GetStoryList();
                                        var resultfl = tfl.Result;
                                        Console.Write("\n  List of Stories:");
                                        foreach (var item in resultfl)
                                        {
                                            Console.Write("\n  {0}", item);
                                        }

                                        string storyType = Console.ReadLine();

                                        Console.Write("\n  Enter the Filename of the Story block:  ");
                                        string NewBlockFileName = Console.ReadLine();
                                        Console.WriteLine("\n  Enter the name of the block image file:  ");
                                        string NewBlockImageName = Console.ReadLine();

                                        Task<HttpResponseMessage> newBlock = client.SendStoryBlock(NewBlockFileName, NewBlockImageName, storyType);
                                        Console.Write(newBlock.Result);
                                        break;
                                    case "3":
                                        break;
                                }
                            }
                            else
                                Console.WriteLine("\n     Enter a valid category!!");
                        }

                        catch (Exception)
                        {
                            Console.Write("Something went wrong. Please check if all the file names were correctly entered.");
                        }
                        break;
                    case "2":
                        Console.Write("List of Stories:");
                        Task<IEnumerable<string>> timelineFl = client.GetStoryList();
                        timelineFl = client.GetStoryList();
                        var storyResult = timelineFl.Result;
                        foreach (var item in storyResult)
                        {
                            Console.Write("\n  {0}", item);
                        }
                        Console.Write("\n  Select a story to change a story block's TimeLine:   ");
                        string storyId = Console.ReadLine();
                        bool correctId = false;
                        foreach (var item in storyResult)
                        {
                            if (item.Contains(storyId))
                                correctId = true;
                        }
                        if (correctId)
                        {
                            Console.Write("List of Story Blocks for the Story:");
                            timelineFl = client.GetBlockList(Int32.Parse(storyId));
                            var blockResult = timelineFl.Result;
                            foreach (var item in blockResult)
                            {
                                Console.Write("\n  {0}", item);
                            }
                            Console.Write("\n  Enter the ID of the story block to change it's timeline:   ");
                            string storyBlockId = Console.ReadLine();
                            bool correctBlockId = false;
                            foreach (var item in blockResult)
                            {
                                if (item.Contains(storyBlockId))
                                    correctBlockId = true;
                            }
                            if (correctBlockId)
                            {
                                Console.Write("\n  Using the Current Date and time as the new timeline of selected Block");
                                Task<HttpResponseMessage> newTimeLine = client.sendNewTimeLine(storyBlockId);
                                Console.Write(newTimeLine.Result);
                            }
                            else
                                Console.Write("\n  !!!!Enter a valid Story block Id!!!!");

                        }
                        else
                        {
                            Console.Write("\n  !!!!Enter a Valid Story ID!!!!!");
                        }   
                        break;
                    case "3":
                        Console.Write("List of Stories:");
                        Task<IEnumerable<string>> blockFl = client.GetStoryList();
                        blockFl = client.GetStoryList();
                        var strResult = blockFl.Result;
                        foreach (var item in strResult)
                        {
                            Console.Write("\n  {0}", item);
                        }
                        Console.Write("\n  Select a story to replace a Story Block:   ");
                        string strId = Console.ReadLine();
                        bool correctStrId = false;
                        foreach (var item in strResult)
                        {
                            if (item.Contains(strId))
                                correctStrId = true;
                        }
                        if (correctStrId)
                        {
                            Console.Write("\n  List of Story Blocks for the Story:");
                            timelineFl = client.GetBlockList(Int32.Parse(strId));
                            var blockResult = timelineFl.Result;
                            foreach (var item in blockResult)
                            {
                                Console.Write("\n  {0}", item);
                            }
                            Console.Write("\n  Enter the ID of the story block to replacce:   ");
                            string storyBlockId = Console.ReadLine();
                            bool correctBlockId = false;
                            foreach (var item in blockResult)
                            {
                                if (item.Contains(storyBlockId))
                                    correctBlockId = true;
                            }
                            if (correctBlockId)
                            {
                                Console.Write("\n  Enter the New Filename of the Story block:  ");
                                string NewBlockFileName = Console.ReadLine();
                                Console.WriteLine("\n  Enter the New name of the block image file:  ");
                                string NewBlockImageName = Console.ReadLine();

                                Task<HttpResponseMessage> newBlock = client.ReplaceStoryBlock(NewBlockFileName, NewBlockImageName, storyBlockId);
                                Console.Write(newBlock.Result);
                                break;
                            }
                            else
                                Console.Write("\n  !!!!Enter a valid Story block Id!!!!");
                            break;
                        }
                        else
                        {
                            Console.Write("\n  !!!!Enter a Valid Story ID!!!!!");
                        }
                        break;

                    case "4":
                        Console.Write("List of Stories:");
                        Task<IEnumerable<string>> DlblockFl = client.GetStoryList();
                        DlblockFl = client.GetStoryList();
                        var strRes = DlblockFl.Result;
                        foreach (var item in strRes)
                        {
                            Console.Write("\n  {0}", item);
                        }
                        Console.Write("\n  Select a story to replace a Story Block:   ");
                        string delId = Console.ReadLine();
                        bool correctDelId = false;
                        foreach (var item in strRes)
                        {
                            if (item.Contains(delId))
                                correctDelId = true;
                        }
                        if (correctDelId)
                        {
                            Console.Write("\n  List of Story Blocks for the Story:");
                            timelineFl = client.GetBlockList(Int32.Parse(delId));
                            var blockResult = timelineFl.Result;
                            foreach (var item in blockResult)
                            {
                                Console.Write("\n  {0}", item);
                            }
                            Console.Write("\n  Enter the ID of the story block to delete:   ");
                            string storyBlockId = Console.ReadLine();
                            bool correctBlockId = false;
                            foreach (var item in blockResult)
                            {
                                if (item.Contains(storyBlockId))
                                    correctBlockId = true;
                            }
                            if (correctBlockId)
                            {
                                Task<HttpResponseMessage> newBlock = client.DeleteStoryBlock(storyBlockId);
                                Console.Write(newBlock.Result);
                                break;
                            }
                            else
                                Console.Write("\n  !!!!Enter a valid Story block Id!!!!");
                            break;
                        }
                        else
                        {
                            Console.Write("\n  !!!!Enter a Valid Story ID!!!!!");
                        }
                        break;

                    case "5":
                        break;
                }
                //Console.WriteLine("\n  Press any key to proceeed: ");
                //Console.ReadKey();
            }
        }
    }
}
