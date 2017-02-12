using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebSite.Controllers
{
    public class SampleApiController : ApiController
    {
        // GET: api/SampleApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SampleApi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SampleApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/SampleApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/SampleApi/5
        public void Delete(int id)
        {
        }
    }
}
