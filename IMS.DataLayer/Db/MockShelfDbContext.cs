using IMS.Entities;
using System;
using System.Collections.Generic;

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
        public Shelf GetShelfByShelfCode(string shelfCode)
        {
            return _shelf.Find
                   (
                        s =>
                        {
                            return s.Code.Equals(shelfCode);
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

        public Shelf GetShelfByName(Shelf shelf)
        {
            foreach(var list in _shelf)
            {
                if(list.Name == shelf.Name && list.Code == shelf.Code)
                {
                    return null;
                    
                }
            }

            return shelf;
        }

        public bool GetShelfByCode(string shelfCode)
        {
            foreach (var list in _shelf)
            {
                if (list.Code.Equals(shelfCode))
                {
                    return true;

                }
            }

            return false;
        }

        public List<Shelf> DeleteShelf(string shelfCode)
        {
           Shelf shelf = _shelf.Find
                  (
                       s =>
                       {
                           return s.Code.Equals(shelfCode);
                       }
                  );
            shelf.isActive = false;
            return _shelf;
        }
    }
}
