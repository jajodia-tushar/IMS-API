using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class RAGStatusResponse : Response
    {
        public Dictionary<string,ColourCountMapping> RAGStatus = new Dictionary<string,ColourCountMapping>();
    }
}
