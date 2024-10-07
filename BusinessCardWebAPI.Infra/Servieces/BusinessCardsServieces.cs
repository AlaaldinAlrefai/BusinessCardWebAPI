using BusinessCardWebAPI.Core.Data;
using BusinessCardWebAPI.Core.DTO;
using BusinessCardWebAPI.Core.IServieces;
using BusinessCardWebAPI.Infra.Reposetory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardWebAPI.Infra.Servieces
{
    public class BusinessCardsServieces:BusinessCardsReposetory,IBusinessCardsServieces
    {
        private readonly BusinessCardDbContext _context;
        public BusinessCardsServieces(BusinessCardDbContext context):base(context)
        {

            _context = context;
            
        }
        // Implement the AddRangeAsync method
        public async Task AddRangeAsync(IEnumerable<BusinessCards> businessCardsDtos)
        {
            // Fetch existing UserIds
            var existingUserIds = await _context.Users.Select(u => u.Id).ToListAsync();
            var businessCards = new List<BusinessCards>();

            foreach (var dto in businessCardsDtos)
            {
                if (!existingUserIds.Contains(dto.UserId))
                {
                    Console.WriteLine($"UserId {dto.UserId} does not exist. Skipping this entry.");
                    continue; // Skip this entry
                }

                var businessCard = new BusinessCards
                {
                    Name = dto.Name,
                    Gender = dto.Gender,
                    DateOfBirth = dto.DateOfBirth,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Photo = dto.Photo,
                    Address = dto.Address,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = dto.UserId
                };

                businessCards.Add(businessCard);
            }

            if (businessCards.Count > 0) // Only save if there are valid business cards
            {
                await _context.BusinessCards.AddRangeAsync(businessCards);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving to database: {ex.Message}");
                }
            }
        }



    }
}
