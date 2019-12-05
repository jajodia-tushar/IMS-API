using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<ShelfResponse> GetShelfList()
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            try
            {
                List<Shelf> shelves = await _shelfDbContext.GetAllShelves();

                if (shelves == null || shelves.Count == 0)
                {
                    shelfResponse.Status = Status.Failure;
                    shelfResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.NotFound,
                        ErrorMessage = Constants.ErrorMessages.EmptyShelfList
                    };
                   
                }
                else
                {
                    shelfResponse.Status = Status.Success;
                    shelfResponse.Shelves = shelves;
                }
            }
            catch(Exception exception)
            {

                throw exception;

            }
          
            return shelfResponse;
        }

        public async Task<ShelfResponse> GetShelfByShelfCode(string shelfCode)
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            Shelf Shelf =await _shelfDbContext.GetShelfByShelfCode(shelfCode);
            try
            {
                if (Shelf == null)
                {
                    shelfResponse.Status = Status.Failure;
                    shelfResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.NotFound,
                        ErrorMessage = Constants.ErrorMessages.InvalidShelfCode
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

        public async Task< ShelfResponse> AddShelf(Shelf shelf)
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            bool shelfPresent = await _shelfDbContext.IsShelfPresentByCode(shelf.Code);
            try
            {

                if (shelfPresent == false)
                {
                    List<Shelf> shelfList =await _shelfDbContext.AddShelf(shelf);
                    shelfResponse.Status = Status.Success;
                    shelfResponse.Shelves = shelfList;
                }
                else
                {
                    shelfResponse.Status = Status.Failure;
                    shelfResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.ShelfIsAlreadyPresent
                    };
                }

            }
            catch (Exception exception)
            {

                throw exception;

            }
            return shelfResponse;


        }

        public async Task<ShelfResponse> Delete(string shelfCode)
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            bool shelfPresent = await _shelfDbContext.IsShelfPresentByCode(shelfCode);
            try
            {

                if (shelfPresent == true && await _shelfDbContext.GetShelfStatusByCode(shelfCode) == true)
                {
                   
                        List<Shelf> shelfList =await _shelfDbContext.DeleteShelfByCode(shelfCode);
                        shelfResponse.Status = Status.Success;
                        shelfResponse.Shelves = shelfList;
                   
                }
                else
                {
                    shelfResponse.Status = Status.Failure;
                    shelfResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.ShelfNotPresent
                    };
                }
            }
            catch (Exception exception)
            {

                throw exception;

            }

            return shelfResponse;

        }
    }
}
