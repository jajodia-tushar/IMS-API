using IMS.DataLayer.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
                   isActive = true
               },
               new Item()
               {
                   Id =2,
                   Name = "Pen",
                   MaxLimit = 4,
                   isActive = false
               },

            };
        public List<Item> GetAllItems()
        {
            return _items;
        }

        public List<Item> GetItemById(int id)
        {
            List<Item> resultantItem = new List<Item>();
            Item item = new Item();
            foreach (var i in _items)
            {
                if (i.Id.Equals(id))
                {
                    resultantItem.Add(i);
                }
            }
            return resultantItem;
        }

        public List<Item> AddItem(ItemRequest itemRequest)
        {
            Item item = new Item();
            var id = _items.Count + 1;
            item.Id = id;
            item.Name = itemRequest.Name;
            item.MaxLimit = itemRequest.MaxLimit;
            item.isActive = true;
            _items.Add(item);
            return GetAllItems();
        }

        public List<Item> Delete(int id)
        {
            foreach (var item in _items)
            {
                if (item.Id.Equals(id))
                {
                    item.isActive = false;
                }
            }
            return GetAllItems();
        }

        public List<Item> UpdateItem(ItemRequest itemRequest)
        {
            foreach (var item in _items)
            {
                if (item.Id.Equals(itemRequest.Id))
                {
                    item.Id = itemRequest.Id;
                    item.Name = itemRequest.Name;
                    item.MaxLimit = itemRequest.MaxLimit;
                    item.isActive = itemRequest.isActive;
                }
            }
            return GetAllItems();
        }
    }
}
