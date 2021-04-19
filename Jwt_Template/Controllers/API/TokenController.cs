using Jwt_Template.Filters;
using System.Net.Http;
using System.Web.Http;

namespace Jwt_Template.Controllers.API
{
    public class TokenController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GenerateToken(string user)
        {
            string token = JwtManager.GenerateToken(user);
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, token);
        }


        //public IHttpActionResult ValidateUser(string token)
        //{
        //    if (JwtAuthenticationAttribute.ValidateUser(token) != null)
        //        return Ok();
        //    else return NotFound();
        //}
        
        [HttpGet]
        public HttpResponseMessage ValidateToken(string token, string user)
        {
            string username = null;
            JwtAuthenticationAttribute.ValidateToken(token, out username);

            if(user.Equals(username))
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest);         
        }
    }
}
