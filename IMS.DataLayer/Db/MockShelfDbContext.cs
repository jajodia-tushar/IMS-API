using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IMS.DataLayer.Interfaces;

namespace IMS.DataLayer.Db
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
        public async Task<List<Shelf>> GetAllShelves()
        {
            return _shelf;
        }
        public async Task< Shelf> GetShelfByShelfCode(string shelfCode)
        {
            return _shelf.Find
                   (
                        s =>
                        {
                            return s.Code.Equals(shelfCode);
                        }
                   );
        }

        public async Task< List<Shelf>> AddShelf(Shelf shelf)
        {
            shelf.Id = _shelf.Count + 1;
            shelf.IsActive = true;
            _shelf.Add(shelf);
            return _shelf;
        }

        public async Task< bool> IsShelfPresentByCode(string shelfCode)
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

        public async Task< List<Shelf>> DeleteShelfByCode(string shelfCode)
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

  
        public async Task <bool> GetShelfStatusByCode(string shelfCode)
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
