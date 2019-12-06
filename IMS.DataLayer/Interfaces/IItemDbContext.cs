using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Interfaces
{
    public interface IItemDbContext
    {
        List<Item> GetAllItems();
        Item GetItemById(int id);
        int AddItem(ItemRequest itemRequest);
        bool Delete(int id);
        Item UpdateItem(ItemRequest itemRequest);
    }
}
