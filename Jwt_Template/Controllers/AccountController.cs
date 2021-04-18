using Jwt_Template.Filters;
using Jwt_Template.Models;
using Jwt_Template.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Jwt_Template.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {
            using (var httpClient = new HttpClient())
            {
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                        Request.ApplicationPath.TrimEnd('/') + "/";
                httpClient.BaseAddress = new Uri(baseUrl);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");

                var parameters = new Dictionary<string, string> { { "username", username }, { "password", password } };
                var encodedContent = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = await httpClient.PostAsync("api/AccountAPI/Login", encodedContent);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Session["Username"] = username;
                    //string jwtToken = JwtManager.GenerateToken(username);

                    return RedirectToAction("Dashboard");
                }
            }
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll(); // it will clear the session at the end of request
            return RedirectToAction("Login", "Account");
        }

        public async Task<ActionResult> Dashboard()
        {
            string token = null;
            try
            {
                token = JwtManager.GenerateToken(Session["UserName"].ToString());
                Session["Token"] = token;
            }
            catch (Exception)
            {
                RedirectToAction("Login");
            }

            using (var httpClient = new HttpClient())
            {
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                        Request.ApplicationPath.TrimEnd('/') + "/";
                httpClient.BaseAddress = new Uri(baseUrl);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.GetAsync($"api/AccountAPI/Dashboard");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return View();
            }

            return RedirectToAction("Login");
        }

        public async Task<ActionResult> FileUpload(FileUploadRepo model)
        {
            string token = null;
            try
            {
                token = Session["Token"].ToString();

                using (var httpClient = new HttpClient())
                {
                    string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                            Request.ApplicationPath.TrimEnd('/') + "/";
                    httpClient.BaseAddress = new Uri(baseUrl);
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await httpClient.GetAsync($"api/Files/FileUpload");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var list = new FileUploadRepo();
                        if (list != null)
                        {
                            model.FileList = list.FileList;
                            return View(model);
                        }
                        else return View();
                        //List<tblFileDetails> list = new List<tblFileDetails>();
                        //using (DB_Entities context = new DB_Entities())
                        //{
                        //    var allFiles = (from r in context.tblFileDetails select r).ToList();
                        //    if (allFiles != null)
                        //        list = allFiles;
                        //}

                        //if (list != null)
                        //{
                        //    return View(list);
                        //}
                        //else return View();
                    }
                }
            }
            catch (Exception)
            {
                Session.RemoveAll();
                return RedirectToAction("Login", "Account");
            }

            Session.RemoveAll();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase files)
        {
            string ext = Path.GetExtension(files.FileName);

            if (ext == ".doc" || ext == ".docx")
            {
                try
                {
                    //var timeStamp = InfoVuln.GetInstance().TimeStamp;
                    //if (!Directory.Exists(Server.MapPath($"~/UploadedFiles/{timeStamp}")))
                    //    Directory.CreateDirectory(Server.MapPath($"~/UploadedFiles/{timeStamp}"));

                    //string path = Path.Combine(Server.MapPath($"~/UploadedFiles/{timeStamp}"), files.FileName);
                    //var fileUrl = Url.Content(Path.Combine($"~/UploadedFiles/{timeStamp}/", files.FileName));

                    //var timeStamp = InfoVuln.GetInstance().TimeStamp;

                    if (!Directory.Exists(Server.MapPath($"~/Template")))
                        Directory.CreateDirectory(Server.MapPath($"~/Template"));

                    string newFileName = files.FileName;

                    using (DB_Entities db = new DB_Entities())
                    {
                        var isExist = db.tblFileDetails.Where(a => a.FileName.Equals(files.FileName)).FirstOrDefault();
                        if (isExist != null)
                            newFileName = InfoVuln.GetInstance().TimeStamp + newFileName;
                    }

                    string path = Path.Combine(Server.MapPath($"~/Template"), newFileName);
                    var fileUrl = Url.Content(Path.Combine($"~/Template/", newFileName));

                    files.SaveAs(path);

                    using (var context = new DB_Entities())
                    {
                        var t = new tblFileDetails
                        {
                            FileName = files.FileName,
                            FileUrl = fileUrl,
                        };

                        context.tblFileDetails.Add(t);
                        context.SaveChanges();
                    }
                    return RedirectToAction("FileUpload");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Error In Add File. Please Try Again !!!");
                }
            }
            
            return RedirectToAction("FileUpload", "Account");
        }

        private byte[] GetFile(string s)
        {
            FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new IOException(s);
            return data;
        }
    }
}