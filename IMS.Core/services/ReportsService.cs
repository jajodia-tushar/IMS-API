﻿using IMS.Entities;
using IMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class ReportsService : IReportsService
    {
        public Task<RAGStatusResponse> GetRAGStatus()
        {
            throw new NotImplementedException();
        }
    }
}
