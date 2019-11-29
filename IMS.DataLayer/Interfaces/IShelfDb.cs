using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer
{
    public interface IShelfDb
    {
        List<Shelf> GetShelfList();
    }
}
