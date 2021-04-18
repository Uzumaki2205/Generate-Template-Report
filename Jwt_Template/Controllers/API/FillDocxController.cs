using Jwt_Template.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Jwt_Template.Controllers.API
{
    public class FillDocxController : ApiController
    {
        private string CurrentDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}";

        [HttpGet]
        [Route("api/filldocx/isExist")]
        public string IsExistPath(string path)
        {
            if (File.Exists(path))
                return "Exist";
            else return "not Exist";
        }

        [HttpGet]
        [Route("api/Download/isExist")]
        public HttpResponseMessage IsExistFile(string fileName)
        {
            var result = new HttpResponseMessage(HttpStatusCode.Found);
            var filepath = $"{CurrentDirectory}Renders/{fileName}";

            if (File.Exists(filepath))
            {
                result.Headers.Location = new Uri($"/api/Download/start?fileName={fileName}", UriKind.Relative);
                return result;
            }
            else return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        // POST api/filldocx
        [HttpPost]
        [Route("api/FillDocx")]
        public HttpResponseMessage Fill(string nameTemplate, string jsonPath)
        {
            InfoVuln.GetInstance().ProcessDocx(nameTemplate, jsonPath);
            return IsExistFile($"{InfoVuln.GetInstance().TimeStamp}.Report.docx");
        }
    }
}
