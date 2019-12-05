using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
     public interface IShelfService
    {
        Task<ShelfResponse> GetShelfList();
        ShelfResponse GetShelfByShelfCode(string id);
        ShelfResponse AddShelf(IMS.Entities.Shelf shelf);
        ShelfResponse Delete(string shelfCode);
    }
}
