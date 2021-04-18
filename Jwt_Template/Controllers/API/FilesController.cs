using Jwt_Template.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Jwt_Template.Controllers.API
{
    public class FilesController : ApiController
    {
        [JwtAuthentication]
        [HttpGet] 
        public IHttpActionResult FileUpload()
        {
            return Ok();
        }
    }
}
