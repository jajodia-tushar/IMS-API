﻿using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer
{
    public interface IShelfDbContext
    {
        List<Shelf> GetAllShelves();
        Shelf GetShelfByShelfCode(string id);
        List<Shelf> AddShelf(Shelf shelf);
        bool IsShelfPresent(Shelf shelf);
        bool IsShelfPresentByCode(string shelfCode);
        List<Shelf> DeleteShelfByCode(string shelfCode);
        bool GetShelfStatusByCode(string shelfCode);
    }
}
