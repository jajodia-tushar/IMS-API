using System;
using System.Collections.Generic;
using System.Text;
using IMS.DataLayer;
using IMS.Entities;
using IMS.Entities.Interfaces;

namespace IMS.Core.services
{
    public class ShelfService : IShelfService
    {
        private IShelfDb _shelfDbContext;

        public ShelfService(IShelfDb shelfDbContext)
        {
            _shelfDbContext = shelfDbContext;
        }
        public ShelfResponse GetShelfList()
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            //List<Shelf> shelf = _shelfDbContext.GetShelfList();
            try
            {
                if (_shelfDbContext.GetShelfList() == null)
                {
                    shelfResponse.Status = Status.Failure;
                    shelfResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.NotFound,
                        ErrorMessage = Constants.ErrorMessages.EmptyShelfList
                    };
                    return shelfResponse;

                }
                if(_shelfDbContext.GetShelfList() != null)
                {
                    shelfResponse.Status = Status.Success;
                    shelfResponse.shelfList = _shelfDbContext.GetShelfList();
                    return shelfResponse;
                }
            }
            catch(Exception exception)
            {

                throw exception;

            }
          
            return shelfResponse;
        }
    }
}
