using BusinessCardWebAPI.Core.Data;
using BusinessCardWebAPI.Core.DTO;
using BusinessCardWebAPI.Core.IReposetory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardWebAPI.Infra.Reposetory
{
    public class BusinessCardsReposetory:GenericRepository<BusinessCards>,IBusinessCardsReposetory
    {
        private readonly BusinessCardDbContext _context;
        public BusinessCardsReposetory(BusinessCardDbContext context) : base(context) 
        {
           _context= context;
        }

        public async Task<IEnumerable<BusinessCards>> FilterBusinessCards(
            string? name = null,
            string? email = null,
            string? phone = null,
            string? gender = null,
            DateTime? dob = null)
        {
            // Use IQueryable to build the query dynamically based on the provided filters
            IQueryable<BusinessCards> query = _context.BusinessCards;

            // Apply filters conditionally if the query parameters are provided
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(b => b.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(b => b.Email.Contains(email));
            }

            if (!string.IsNullOrEmpty(phone))
            {
                query = query.Where(b => b.Phone.Contains(phone));
            }

            if (!string.IsNullOrEmpty(gender))
            {
                query = query.Where(b => b.Gender == gender);
            }

            if (dob.HasValue)
            {
                query = query.Where(b => b.DateOfBirth == dob.Value);
            }

            // Execute the query and get the filtered list of business cards
            var filteredBusinessCards = await query.ToListAsync();

            // Return the list (could be empty if no business cards match the filter criteria)
            return filteredBusinessCards;
        }
    }
}
