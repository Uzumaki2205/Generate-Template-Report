using Jwt_Template.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Jwt_Template.Controllers.API
{
    public class FillDocxController : ApiController
    {
        private string CurrentDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}";

        [HttpGet]
        public HttpResponseMessage IsExistPath(string path)
        {
            if (File.Exists(path))
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        [HttpGet]
        public HttpResponseMessage IsExistFile(string fileName)
        {
            var filepath = $"{CurrentDirectory}Renders/{fileName}";

            if (File.Exists(filepath))
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [HttpGet]
        public string Fill(string nameTemplate, string jsonPath)
        {
            try
            {
                InfoVuln.GetInstance().ProcessDocx(nameTemplate, jsonPath);
                if (IsExistFile($"{InfoVuln.GetInstance().TimeStamp}.Report.docx").StatusCode == HttpStatusCode.OK)
                    return $"{InfoVuln.GetInstance().TimeStamp}.Report.docx";
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }
    }
}
