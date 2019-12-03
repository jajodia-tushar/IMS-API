using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer
{
    public interface IShelfDbContext
    {
        List<Shelf> GetShelfList();
        Shelf GetShelfByShelfCode(string id);
        List<Shelf> AddShelf(Shelf shelf);
        bool IsShelfPresent(Shelf shelf);
        bool GetShelfByCode(string shelfCode);
        List<Shelf> DeleteShelf(string shelfCode);
        bool GetShelfStatusByCode(string shelfCode);
    }
}
