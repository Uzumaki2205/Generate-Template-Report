using Jwt_Template.Models;
using Jwt_Template.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Jwt_Template.Controllers
{
    public class FillDocxController : Controller
    {
        private string CurrentDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}";
        // GET: FillDocx
        public ActionResult Index(tblFileDetails model)
        {
            using (DB_Entities context = new DB_Entities())
            {
                var allFiles = (from r in context.tblFileDetails select r).ToList();
                if (allFiles != null)
                {
                    model.FileList = allFiles;
                    return View(model);
                }  
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Generate(string nameTemplate, HttpPostedFileBase jsonPath)
        {
            string ext = Path.GetExtension(jsonPath.FileName);
            if (ext == ".json")
            {
                try
                {
                    var timeStamp = InfoVuln.GetInstance().TimeStamp;
                    if (!Directory.Exists(Server.MapPath($"~/UploadedFiles/Json/{timeStamp}")))
                        Directory.CreateDirectory(Server.MapPath($"~/UploadedFiles/Json/{timeStamp}"));

                    string path = Path.Combine(Server.MapPath($"~/UploadedFiles/Json/{timeStamp}"), jsonPath.FileName);
                    var fileUrl = Url.Content(Path.Combine($"~/UploadedFiles/Json/{timeStamp}/", jsonPath.FileName));

                    jsonPath.SaveAs(path);

                    ////------------------------------------Process Docx
                    //InfoVuln.GetInstance().ProcessDocx(nameTemplate, path);

                    using (var httpClient = new HttpClient())
                    {
                        string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                                Request.ApplicationPath.TrimEnd('/') + "/";
                        httpClient.BaseAddress = new Uri(baseUrl);
                        httpClient.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");

                        //var parameters = new Dictionary<string, string> { { "nameTemplate", nameTemplate }, { "jsonPath", path } };
                        //var encodedContent = new FormUrlEncodedContent(parameters);


                        ////////////////////RESQUEST NOT POST TO API --------------FAIL
                        HttpResponseMessage response = 
                            await httpClient.GetAsync($"api/FillDocx/Fill?nameTemplate={nameTemplate}&jsonPath={path}");

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return RedirectToAction("Download", new { fileName = InfoVuln.GetInstance().TimeStamp + ".Report.docx" });
                        }
                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "Template not exist or file error";
                }
            }

            TempData["msg"] = "<script>alert('Upload file error or Not exist template!!!');</script>";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Download(string fileName)
        {
            var filepath = $"{CurrentDirectory}Renders/{fileName}";

            var memory = new MemoryStream();
            using (var stream = new FileStream(filepath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            var ext = Path.GetExtension(filepath).ToLowerInvariant();
            memory.Position = 0;

            return File(memory, GetMinetype()[ext], Path.GetFileName(filepath));
        }
        private Dictionary<string, string> GetMinetype()
        {
            return new Dictionary<string, string>
            {
                {".docx", "application/vnd.ms-word" },
                {".doc", "application/vnd.ms-word" },
            };
        }
    }
}