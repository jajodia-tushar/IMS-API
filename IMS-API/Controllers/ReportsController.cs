using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Entities.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private IReportsService _reportsService;
        ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        [Route("GetRAGStatus")]
        [HttpGet]
        public async Task<RAGStatusResponse> GetRAGStatus()
        {
            /*
            RAGStatusResponse rAGStatusResponse = new RAGStatusResponse();
            List<ColourCountMapping> c = new List<ColourCountMapping>();
            rAGStatusResponse.Error = null;
            rAGStatusResponse.Status = Status.Success;
            c.Add(new ColourCountMapping() {Colour=Colour.Red,Count=9 });
            c.Add(new ColourCountMapping() { Colour = Colour.Amber,Count = 10 });
            c.Add(new ColourCountMapping() { Colour = Colour.Green, Count = 13 });
            rAGStatusResponse.RAGStatus = new Dictionary<string, List<ColourCountMapping>>();
            rAGStatusResponse.RAGStatus.Add("floor-1;A", c);
            rAGStatusResponse.RAGStatus.Add("floor-2;B",c);
            return rAGStatusResponse;*/
            throw new NotImplementedException();
        }

        [HttpGet]
        public MostConsumedItemsResponse Get(string StartDate, string EndDate, int ItemsCount)
        {
            MostConsumedItemsResponse mostConsumedItemsResponse = new MostConsumedItemsResponse();
            return mostConsumedItemsResponse;
        }

        [Route("GetShelfWiseOrderCount")]
        [HttpGet]
        public async Task<ShelfWiseOrderCountResponse> GetShelfWiseOrderCount(string FromDate,string ToDate)
        {
           
            throw new NotImplementedException();
        }
    }
}
