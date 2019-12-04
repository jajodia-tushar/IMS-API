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
                   IsActive=true
               },
            new Shelf()
               {
                    Id = 2,
                   Name = "Sixth Floor",
                   Code="B",
                   IsActive = true
               },

        };
        public List<Shelf> GetAllShelves()
        {
            return _shelf;
        }
        public Shelf GetShelfByShelf(string shelfCode)
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
            shelf.IsActive = true;
            _shelf.Add(shelf);
            return _shelf;
        }

        public bool IsShelfPresent(Shelf shelf)
        {
            foreach(var list in _shelf)
            {
                if(list.Name == shelf.Name && list.Code == shelf.Code)
                {
                    return true;
                    
                }
            }

            return false;
        }

        public bool IsShelfPresentByCode(string shelfCode)
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

        public List<Shelf> DeleteShelfByCode(string shelfCode)
        {
           Shelf shelf = _shelf.Find
                  (
                       s =>
                       {
                           return s.Code.Equals(shelfCode);
                       }
                  );
            shelf.IsActive = false;
            return _shelf;
        }

        public Shelf GetShelfByShelfCode(string id)
        {
            throw new NotImplementedException();
        }

        public bool GetShelfStatusByCode(string shelfCode)
        {
            Shelf shelf = _shelf.Find
                  (
                       s =>
                       {
                           return s.Code.Equals(shelfCode);
                       }
                  );
           
            
            return shelf.IsActive ;
        }
    }
}
