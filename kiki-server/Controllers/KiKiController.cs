using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using kiki.shared;

namespace kiki.server.Controllers {
    [ApiController]
    [Route("kiki")]
    public class KiKiController : ControllerBase {
        private readonly KiKiDBContext db;

        public KiKiController() {
            db = new KiKiDBContext();
        }

        [HttpGet]
        [Route("posts")]
        public async Task<IEnumerable<Post>> GetPosts() {
            return await db.GetAllPosts();
        }

        [HttpGet]
        [Route("communities")]
        public async Task<IEnumerable<Community>> GetCommunities() {
            return await db.GetAllCommunities();
        }
    }
}
