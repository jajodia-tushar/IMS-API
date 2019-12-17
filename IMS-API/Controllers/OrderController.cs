using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderService _orderService;
        private ILogManager _logger;

        public OrderController(IOrderService orderService, ILogManager logManager)
        {
            _orderService = orderService;
            _logger = logManager;
        }

        /// <summary>
        /// Deletes the Vendor Order By OrderId
        /// </summary>
        /// <param name="orderId">'Order Id of that Particular Order</param>
        /// <response code="200">Returns status success if order is successfully deleted otherwise returns failure if order is not deleted</response>
        [Route("VendorOrder/{orderId}")]
        [HttpDelete]
        public async Task<Response> Delete(int orderId)
        {
            Response deleteVendorOrderResponse = null;
            try
            {
                IMS.Entities.Response entityDeleteVendorOrderResponse = await _orderService.Delete(orderId);
                deleteVendorOrderResponse = Translator.ToDataContractsObject(entityDeleteVendorOrderResponse);
            }
            catch(Exception exception)
            {
                deleteVendorOrderResponse = new IMS.Contracts.Response()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "Delete", IMS.Entities.Severity.High, orderId, deleteVendorOrderResponse); }).Start();
            }
            return deleteVendorOrderResponse;
        }
    }
}