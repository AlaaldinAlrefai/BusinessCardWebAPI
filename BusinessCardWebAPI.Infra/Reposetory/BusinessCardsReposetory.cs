using BusinessCardWebAPI.Core.Data;
using BusinessCardWebAPI.Core.DTO;
using BusinessCardWebAPI.Core.IReposetory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BusinessCardWebAPI.Infra.Reposetory
{
    public class BusinessCardsReposetory:GenericRepository<BusinessCards>,IBusinessCardsReposetory
    {
        private readonly BusinessCardDbContext _context;
        private int userId;

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


        // Import method for CSV
        public async Task<List<CreateBusinessCardsDto>> ImportFromCsvAsync(StreamReader stream)
        {
            var businessCards = new List<CreateBusinessCardsDto>();

            // Read each line from the CSV
            while (!stream.EndOfStream)
            {
                var line = await stream.ReadLineAsync();
                var values = line.Split(',');

                if (values.Length < 8) // Ensure there are enough fields
                {
                    // Handle error: log or throw an exception
                    continue; // Skip this line or handle it as needed
                }

                var card = new CreateBusinessCardsDto
                {
                    Name = values[0],
                    Gender = values[1],
                    DateOfBirth = DateTime.TryParse(values[2], out var dob) ? dob : (DateTime?)null,
                    Email = values[3],
                    Phone = values[4],
                    Photo = values[5],
                    Address = values[6],
                    Notes = values[7],
                    UserId = int.TryParse(values[8], out var userId) ? userId : 0 // Ensure UserId is parsed correctly
                };

                businessCards.Add(card);
            }

            return businessCards;
        }


        // Import method for XML
        public async Task<List<CreateBusinessCardsDto>> ImportFromXmlAsync(StreamReader stream)
        {
            var serializer = new XmlSerializer(typeof(BusinessCardsWrapper));

            BusinessCardsWrapper wrapper;
            try
            {
                wrapper = (BusinessCardsWrapper)serializer.Deserialize(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during XML deserialization: {ex.Message}");
                return new List<CreateBusinessCardsDto>();
            }

            var businessCards = wrapper.BusinessCards;

            // Debug: Log the imported cards
            foreach (var card in businessCards)
            {
                Console.WriteLine($"Importing: {card.Name}, UserId: {card.UserId}");
            }

            // Optional: Validate UserId
            var existingUserIds = await _context.Users.Select(u => u.Id).ToListAsync();
            businessCards = businessCards.Where(dto => existingUserIds.Contains(dto.UserId)).ToList();

            if (businessCards.Count == 0)
            {
                Console.WriteLine("No valid business cards found for import.");
            }

            return businessCards;
        }



    }

}

