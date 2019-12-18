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
        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        [Route("GetRAGStatus")]
        [HttpGet]
        public async Task<RAGStatusResponse> GetRAGStatus()
        {

            RAGStatusResponse rAGStatusResponse = new RAGStatusResponse();
            List<ColourCountMapping> c = new List<ColourCountMapping>();
            rAGStatusResponse.Error = null;
            rAGStatusResponse.Status = Status.Success;
            c.Add(new ColourCountMapping() { Colour = Colour.Red, Count = 9 });
            c.Add(new ColourCountMapping() { Colour = Colour.Amber, Count = 10 });
            c.Add(new ColourCountMapping() { Colour = Colour.Green, Count = 13 });
            rAGStatusResponse.RAGStatusList = new List<RAGStatus>();
            rAGStatusResponse.RAGStatusList.Add(
                new RAGStatus()
                {
                    Name = "floor-1",
                    Code = "A",
                    ColourCountMappings = c
                });
            rAGStatusResponse.RAGStatusList.Add(
               new RAGStatus()
               {
                   Name = "floor-2",
                   Code = "B",
                   ColourCountMappings = c
               });
            return rAGStatusResponse;
        }

        [Route("GetMostConsumedItems/{StartDate}/{EndDate}/{ItemsCount}")]
        [HttpGet]
        public MostConsumedItemsResponse Get(string StartDate, string EndDate, int ItemsCount)
        {
            MostConsumedItemsResponse mostConsumedItemsResponse = new MostConsumedItemsResponse();
            List<ItemQuantityMapping> itemQuantityMappings = new List<ItemQuantityMapping>();
            mostConsumedItemsResponse.Error = null;
            mostConsumedItemsResponse.Status = Status.Success;
            itemQuantityMappings.Add(new ItemQuantityMapping()
            {
                Item = new Item()
                {
                    Id = 1,
                    Name = "Pen",
                    MaxLimit = 5,
                    IsActive = true,
                    ImageUrl = "abcdef",
                    Rate = 5
                },
                Quantity = 5
            });
            itemQuantityMappings.Add(new ItemQuantityMapping()
            {
                Item = new Item()
                {
                    Id = 2,
                    Name = "Pencil",
                    MaxLimit = 5,
                    IsActive = true,
                    ImageUrl = "abcdefghij",
                    Rate = 2
                },
                Quantity = 15
            });
            itemQuantityMappings.Add(new ItemQuantityMapping()
            {
                Item = new Item()
                {
                    Id = 3,
                    Name = "Marker",
                    MaxLimit = 5,
                    IsActive = true,
                    ImageUrl = "abcdefghijklmn",
                    Rate = 15
                },
                Quantity = 10
            });
            mostConsumedItemsResponse.ItemQuantityMapping = itemQuantityMappings;
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
