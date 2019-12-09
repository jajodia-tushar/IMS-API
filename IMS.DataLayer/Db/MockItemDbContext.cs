using IMS.DataLayer.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class MockItemDbContext : IItemDbContext
    {
        private static List<Item> _items = new List<Item>()
            {
               new Item()
               {
                   Id =1,
                   Name = "Pencil",
                   MaxLimit = 4,
                   IsActive = true
               },
               new Item()
               {
                   Id =2,
                   Name = "Pen",
                   MaxLimit = 4,
                   IsActive = false
               },

            };
        public async Task<List<Item>> GetAllItems()
        {
            return _items;
        }

        public async Task<Item> GetItemById(int id)
        {
            Item item = new Item();
            foreach (var i in _items)
            {
                if (i.Id.Equals(id))
                {
                    return item;
                }
            }
            return null;
        }

        public async Task<int> AddItem(Item itemRequest)
        {
            Item item = new Item();
            var latestAddedItemId = _items.Count + 1;
            item.Id = latestAddedItemId;
            item.Name = itemRequest.Name;
            item.MaxLimit = itemRequest.MaxLimit;
            item.IsActive = true;
            _items.Add(item);
            return latestAddedItemId;
        }

        public async Task<bool> Delete(int id)
        {
            foreach (var item in _items)
            {
                if (item.Id.Equals(id))
                {
                    item.IsActive = false;
                    return true;
                }
            }
            return false;
        }

        public async Task<Item> UpdateItem(Item itemRequest)
        {
            foreach (var item in _items)
            {
                if (item.Id.Equals(itemRequest.Id))
                {
                    item.Id = itemRequest.Id;
                    item.Name = itemRequest.Name;
                    item.MaxLimit = itemRequest.MaxLimit;
                    item.IsActive = itemRequest.IsActive;
                    return item;
                }
            }
            return null;
        }
    }
}
