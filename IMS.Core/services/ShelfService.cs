using System;
using System.Collections.Generic;
using System.Text;
using IMS.DataLayer;
using IMS.Entities;
using IMS.Entities.Interfaces;

namespace IMS.Core.services
{
    public class ShelfService : IShelfService
    {
        private IShelfDb _shelfDbContext;

        public ShelfService(IShelfDb shelfDbContext)
        {
            _shelfDbContext = shelfDbContext;
        }
        public ShelfResponse GetShelfList()
        {
            ShelfResponse shelfList = new ShelfResponse();

            shelfList = _shelfDbContext.GetShelfList();
            return shelfList;
        }
    }
}
