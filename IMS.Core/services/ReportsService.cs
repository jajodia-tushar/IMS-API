using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class ReportsService : IReportsService
    {
        private IReportsDbContext _reportsDbContext;
        private ILogManager _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenProvider _tokenProvider;

        public ReportsService(IReportsDbContext reportsDbContext, ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
        {
            this._reportsDbContext = reportsDbContext;
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<MostConsumedItemsResponse> GetMostConsumedItems(string startDate, string endDate,int itemsCount)
        {
            MostConsumedItemsResponse mostConsumedItemsResponse = new MostConsumedItemsResponse();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    List<ItemQuantityMapping> itemQuantityMappings;
                    try
                    {
                        mostConsumedItemsResponse = ValidateDateAndItemsCount(startDate,endDate,itemsCount);
                        if (mostConsumedItemsResponse.Error == null)
                        {
                            itemQuantityMappings = await _reportsDbContext.GetMostConsumedItemsByDate(startDate,endDate,itemsCount);
                            if (itemQuantityMappings.Count != 0)
                            {
                                mostConsumedItemsResponse.Status = Status.Success;
                                mostConsumedItemsResponse.ItemQuantityMapping = itemQuantityMappings;
                            }
                            else
                            {
                                mostConsumedItemsResponse.Status = Status.Failure;
                                mostConsumedItemsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.RecordNotFound);
                            }
                            return mostConsumedItemsResponse;
                        }
                        return mostConsumedItemsResponse;
                    }
                    catch (Exception exception)
                    {
                        mostConsumedItemsResponse.Status = Status.Failure;
                        mostConsumedItemsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                        new Task(() => { _logger.LogException(exception, "GetMostConsumedItems", Severity.High, startDate+";"+endDate+";"+itemsCount, mostConsumedItemsResponse); }).Start();
                    }
                    return mostConsumedItemsResponse;
                }
                else
                {
                    mostConsumedItemsResponse.Status = Status.Failure;
                    mostConsumedItemsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                mostConsumedItemsResponse.Status = Status.Failure;
                mostConsumedItemsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetMostConsumedItems", Severity.High, startDate + ";" + endDate + ";" + itemsCount, mostConsumedItemsResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.High;
                if (mostConsumedItemsResponse.Status == Status.Failure)
                    severity = Severity.High;
                new Task(() => { _logger.Log(startDate + ";" + endDate + ";" + itemsCount, mostConsumedItemsResponse, "GetMostConsumedItems", mostConsumedItemsResponse.Status, severity, userId); }).Start();
            }
            return mostConsumedItemsResponse;
        }

        public Task<RAGStatusResponse> GetRAGStatus()
        {
            throw new NotImplementedException();
        }

        public Task<ShelfWiseOrderCountResponse> GetShelfWiseOrderCount(string FromDate, string ToDate)
        {
            throw new NotImplementedException();
        }

        private MostConsumedItemsResponse ValidateDateAndItemsCount(string startDate,string endDate,int itemsCount)
        {
            MostConsumedItemsResponse mostConsumedItemsResponse = new MostConsumedItemsResponse();
            if(string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate) || itemsCount <= 0)
            {
                mostConsumedItemsResponse.Status = Status.Failure;
                mostConsumedItemsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidInput);
            }
            else if (String.Compare(startDate, endDate) > 0)
            {
                mostConsumedItemsResponse.Status = Status.Failure;
                mostConsumedItemsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidDates);
            }
            return mostConsumedItemsResponse;
        }
    }
}
