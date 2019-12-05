using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
     public interface IShelfService
    {
        Task<ShelfResponse> GetShelfList();
        Task<ShelfResponse> GetShelfByShelfCode(string id);
        Task<ShelfResponse> AddShelf(IMS.Entities.Shelf shelf);
        Task<ShelfResponse> Delete(string shelfCode);
    }
}
