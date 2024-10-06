using BusinessCardWebAPI.Core.Data;
using BusinessCardWebAPI.Core.IServieces;
using BusinessCardWebAPI.Infra.Reposetory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardWebAPI.Infra.Servieces
{
    public class BusinessCardsServieces:BusinessCardsReposetory,IBusinessCardsServieces
    {
        public BusinessCardsServieces(BusinessCardDbContext context):base(context)
        {

        }
    }
}
