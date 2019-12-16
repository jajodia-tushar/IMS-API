using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class RAGStatusResponse : Response
    {
        public Dictionary<string, List<ColourCountMapping>> RAGStatus = new Dictionary<string, List<ColourCountMapping>>();
    }
}
