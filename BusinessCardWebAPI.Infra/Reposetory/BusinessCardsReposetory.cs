﻿using BusinessCardWebAPI.Core.Data;
using BusinessCardWebAPI.Core.IReposetory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardWebAPI.Infra.Reposetory
{
    public class BusinessCardsReposetory:GenericRepository<BusinessCards>,IBusinessCardsReposetory
    {
        public BusinessCardsReposetory(BusinessCardDbContext context) : base(context) 
        {
        
        }
    }
}
