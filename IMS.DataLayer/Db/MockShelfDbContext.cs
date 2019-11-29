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
                   Id = 1,
                   Name = "First Floor",
                   Code ="A"
               },
            new Shelf()
               {
                    Id = 2,
                   Name = "Sixth Floor",
                   Code="B"
                   
               },

        };
        public List<Shelf> GetShelfList()
        {
            return _shelf;
        }
    }
}
