using BusinessCardWebAPI.Core.Data;
using BusinessCardWebAPI.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardWebAPI.Core.IReposetory
{
    public interface IBusinessCardsReposetory:IGenericRepository<BusinessCards>
    {
        Task<IEnumerable<BusinessCards>> FilterBusinessCards(
         string? name = null,
         string? email = null,
         string? phone = null,
         string? gender = null,
         DateTime? dob = null);

    }
}
