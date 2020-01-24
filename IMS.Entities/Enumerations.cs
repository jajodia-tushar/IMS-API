using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public enum Colour
    {
        Red,
        Amber,
        Green
    }

    public enum BulkOrderRequestStatus
    {
        Pending,
        Rejected,
        Approved,
        Cancelled
    }
    public enum RequestStatus
    {
        Pending,
        Rejected,
        Approved,
        Edited
    }

    public enum RequestType
    {
        VendorOrder,
        BulkOrder,
        UserModification
    }
}
