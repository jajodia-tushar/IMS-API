using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Interfaces
{
     public interface IShelfService
    {
       ShelfResponse GetShelfList();
        ShelfResponse GetShelfById(int id);
        ShelfResponse AddShelf(IMS.Entities.Shelf shelf);
        ShelfResponse Delete(string shelfCode);
    }
}
