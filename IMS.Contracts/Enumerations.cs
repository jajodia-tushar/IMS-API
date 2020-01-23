using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
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
        Approved
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
