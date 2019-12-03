using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer
{
    public interface IShelfDbContext
    {
        List<Shelf> GetShelfList();
        Shelf GetShelfById(int id);
        List<Shelf> AddShelf(Shelf shelf);
        Shelf GetShelfByName(Shelf shelf);
        bool GetShelfByCode(string shelfCode);
        List<Shelf> DeleteShelf(string shelfCode);
    }
}
