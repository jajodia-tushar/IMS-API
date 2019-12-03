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
        private IShelfDbContext _shelfDbContext;

        public ShelfService(IShelfDbContext shelfDbContext)
        {
            _shelfDbContext = shelfDbContext;
        }
        public ShelfResponse GetShelfList()
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            List<Shelf> Shelf = _shelfDbContext.GetShelfList();
            try
            {
                if (Shelf == null)
                {
                    shelfResponse.Status = Status.Failure;
                    shelfResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.NotFound,
                        ErrorMessage = Constants.ErrorMessages.EmptyShelfList
                    };
                   
                }
                if(Shelf != null)
                {
                    shelfResponse.Status = Status.Success;
                    shelfResponse.Shelves = Shelf;
                }
            }
            catch(Exception exception)
            {

                throw exception;

            }
          
            return shelfResponse;
        }

        public ShelfResponse GetShelfById(string id)
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            Shelf Shelf = _shelfDbContext.GetShelfByShelfCode(id);
            try
            {
                if (Shelf == null)
                {
                    shelfResponse.Status = Status.Failure;
                    shelfResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.NotFound,
                        ErrorMessage = Constants.ErrorMessages.InvalidShelfId
                    };
                    

                }
                if (Shelf != null)
                {
                    shelfResponse.Status = Status.Success;
                    shelfResponse.Shelves = new List<Shelf> { Shelf };
                }
            }
            catch (Exception exception)
            {

                throw exception;

            }

            return shelfResponse;
        }

        public ShelfResponse AddShelf(Shelf shelf)
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            Shelf shelfPresent = _shelfDbContext.GetShelfByName(shelf);
            if (shelfPresent != null)
            {
                List<Shelf> shelfList = _shelfDbContext.AddShelf(shelf);
                shelfResponse.Status = Status.Success;
                shelfResponse.Shelves = shelfList;
            }
            else
            {
                shelfResponse.Status = Status.Failure;
                shelfResponse.Error.ErrorMessage = Constants.ErrorMessages.ShelfIsAlreadyPresent;

            }
            
            return shelfResponse;


        }

        public ShelfResponse Delete(string shelfCode)
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            bool shelfPresent = _shelfDbContext.GetShelfByCode(shelfCode);
            if (shelfPresent == true)
            {
                List<Shelf> shelfList = _shelfDbContext.DeleteShelf(shelfCode);
                shelfResponse.Status = Status.Success;
                shelfResponse.Shelves = shelfList;
            }
            else
            {
                shelfResponse.Status = Status.Failure;
                shelfResponse.Error.ErrorMessage = Constants.ErrorMessages.InvalidShelfId;

            }

            return shelfResponse;

        }
    }
}
