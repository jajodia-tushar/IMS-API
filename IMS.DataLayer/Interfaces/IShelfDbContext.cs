using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer
{
    public interface IShelfDbContext
    {
        Task<List<Shelf>> GetAllShelves();
        Task<Shelf> GetShelfByShelfCode(string shelfCode);
        Task<List<Shelf>> AddShelf(Shelf shelf);
      
        Task<bool> IsShelfPresentByCode(string shelfCode);
        Task<List<Shelf>> DeleteShelfByCode(string shelfCode);
        Task< bool> GetShelfStatusByCode(string shelfCode);
        Task<Shelf> UpdateShelf(Shelf shelf);
    }
}
