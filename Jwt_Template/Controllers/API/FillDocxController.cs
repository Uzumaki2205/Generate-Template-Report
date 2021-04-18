using Jwt_Template.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Jwt_Template.Controllers.API
{
    public class FillDocxController : ApiController
    {
        private string CurrentDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}";

        [HttpGet]
        public string IsExistPath(string path)
        {
            if (File.Exists(path))
                return "Exist";
            else return "not Exist";
        }

        [HttpGet]
        public HttpResponseMessage IsExistFile(string fileName)
        {
            var filepath = $"{CurrentDirectory}Renders/{fileName}";

            if (File.Exists(filepath))
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [HttpGet]
        public HttpResponseMessage Fill(string nameTemplate, string jsonPath)
        {
            try
            {
                InfoVuln.GetInstance().ProcessDocx(nameTemplate, jsonPath);
                return IsExistFile($"{InfoVuln.GetInstance().TimeStamp}.Report.docx");
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
