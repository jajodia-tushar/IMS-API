using System;
using System.Collections.Generic;
using System.Text;
using IMS.Entities;

namespace IMS.DataLayer
{
    public class MockShelfDbContext : IShelfDbContext
    {
        private static List<Shelf> _shelf = new List<Shelf>()
        {
            new Shelf()
               {
                   Id = 1,
                   Name = "First Floor",
                   Code ="A",
                   isActive=true
               },
            new Shelf()
               {
                    Id = 2,
                   Name = "Sixth Floor",
                   Code="B",
                   isActive = true
               },

        };
        public List<Shelf> GetShelfList()
        {
            return _shelf;
        }
        public Shelf GetShelfById(int id)
        {
            return _shelf.Find
                   (
                        s =>
                        {
                            return s.Id.Equals(id);
                        }
                   );
        }

        public List<Shelf> AddShelf(Shelf shelf)
        {
            shelf.Id = _shelf.Count + 1;
            shelf.isActive = true;
            _shelf.Add(shelf);
            return _shelf;
        }
    }
}
