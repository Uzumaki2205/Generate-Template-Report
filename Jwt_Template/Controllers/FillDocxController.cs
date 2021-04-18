using Jwt_Template.Models;
using Jwt_Template.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public ActionResult Generate(string nameTemplate, HttpPostedFileBase jsonPath)
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

                    ////////////////////////////////EDIT PATH TO PROCESS JSON -> UploadedFile/Json/{timestamp}/*.json
                    InfoVuln.GetInstance().ProcessDocx(nameTemplate, path);

                    TempData["msg"] = "<script>alert('Upload success');</script>";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ViewBag.Message = "Template not exist or file error";
                }
            }
            else TempData["msg"] = "<script>alert('file is not correct format');</script>";

            TempData["msg"] = "<script>alert('Upload not success');</script>";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult JsonUpload(HttpPostedFileBase files)
        {
            string ext = Path.GetExtension(files.FileName);
            if (ext == ".json")
            {
                try
                {
                    var timeStamp = InfoVuln.GetInstance().TimeStamp;
                    if (!Directory.Exists(Server.MapPath($"~/UploadedFiles/{timeStamp}")))
                        Directory.CreateDirectory(Server.MapPath($"~/UploadedFiles/{timeStamp}"));

                    string path = Path.Combine(Server.MapPath($"~/UploadedFiles/{timeStamp}"), files.FileName);
                    var fileUrl = Url.Content(Path.Combine($"~/UploadedFiles/{timeStamp}/", files.FileName));

                    files.SaveAs(path);

                    return Content("<script>alert('Success')</script>");
                }
                catch (Exception)
                {
                    return RedirectToAction("JsonUpload");
                }
            }

            return RedirectToAction("JsonUpload");
        }

        [HttpGet]
        [Route("api/Download/start")]
        public async Task<ActionResult> DownloadFile(string fileName)
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