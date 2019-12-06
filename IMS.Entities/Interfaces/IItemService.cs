using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Interfaces
{
    public interface IItemService
    {
        ItemResponse GetAllItems();
        ItemResponse GetItemById(int id);
        ItemResponse AddItem(ItemRequest itemRequest);
        ItemResponse Delete(int id);
        ItemResponse UpdateItem(ItemRequest itemRequest);
    }
}