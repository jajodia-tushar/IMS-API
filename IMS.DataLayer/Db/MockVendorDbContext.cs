using IMS.DataLayer.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class MockVendorDbContext : IVendorDbContext
    {
        private static List<Vendor> vendors = new List<Vendor>()
            {
               new Vendor()
               {
                   Id =1,
                   Name="Vendor1",
                   Title="Title1",
                   ContactNumber="Mobile1",
                   PAN="Pan1",
                   GST="Gst1",
                   CompanyIdentificationNumber="Cin1"
               },
               new Vendor()
               {
                   Id =2,
                   Name="Vendor2",
                   Title="Title2",
                   ContactNumber="Mobile2",
                   PAN="Pan2",
                   GST="Gst2",
                   CompanyIdentificationNumber="Cin2"
               },
               new Vendor()
               {
                   Id =3,
                   Name="Vendor3",
                   Title="Title3",
                   ContactNumber="Mobile3",
                   PAN="Pan3",
                   GST="Gst3",
                   CompanyIdentificationNumber="Cin3"
               },
            };

        public async Task<Vendor> GetVendorById(int vendorId)
        {
            return vendors.Find
                   (
                        u =>
                        {
                            return u.Id.Equals(vendorId);
                        }
                   );
        }

        public Task<VendorsResponse> GetVendors(string name, int limit, int offset)
        {
            throw new NotImplementedException();
        }
        public Task<Vendor> UpdateVendor(Vendor vendor)
        {
            throw new NotImplementedException();
        }
    }
}

