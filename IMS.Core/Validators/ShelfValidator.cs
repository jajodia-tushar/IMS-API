
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Validators
{
    public class ShelfValidator
    {
        public static Response ValidateShelf(Shelf shelf)
        {
            Response validateResponse = new Response();
            if(String.IsNullOrEmpty(shelf.Name) || String.IsNullOrEmpty(shelf.Code))
            {
                validateResponse.Error.ErrorCode = Constants.ErrorCodes.BadRequest;
                validateResponse.Error.ErrorMessage = Constants.ErrorMessages.InvalidShelfDetails;
            }
            return validateResponse;
        }
    }
}
