using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        [Route("GetRAGStatus")]
        [HttpGet]
        public async Task<RAGStatusResponse> GetRAGStatus()
        {
            throw new NotImplementedException();
        }
    }
}
