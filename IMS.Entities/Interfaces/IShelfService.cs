using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Interfaces
{
     public interface IShelfService
    {
       ShelfResponse GetShelfList();
        ShelfResponse GetShelfById(int id);
    }
}
