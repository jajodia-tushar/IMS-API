using System;
using System.Collections.Generic;
using System.Text;
using IMS.Entities;

namespace IMS.DataLayer
{
    public class MockShelfDbContext : IShelfDb
    {
        private static List<Shelf> _shelf = new List<Shelf>()
        {
            new Shelf()
               {
                   ShelfId = 1,
                   ShelfName = "First Floor"
               },
            new Shelf()
               {
                    ShelfId = 2,
                   ShelfName = "Sixth Floor"
               },

        };
        public ShelfResponse GetShelfList()
        {
            ShelfResponse shelfResponse = new ShelfResponse();
              shelfResponse.GetShelves = _shelf;
            return shelfResponse;
        }
    }
}
