using AutoMapper;
using BusinessCardWebAPI.Core.Data;
using BusinessCardWebAPI.Core.DTO;
using BusinessCardWebAPI.Core.IReposetory;
using BusinessCardWebAPI.Core.IServieces;
using BusinessCardWebAPI.Infra.Reposetory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BusinessCardWebAPI.Infra.Servieces
{
    public class BusinessCardsServieces:BusinessCardsReposetory,IBusinessCardsServieces
    {
        private readonly BusinessCardDbContext _context;
        private readonly IBusinessCardsReposetory _businessCardsReposetory;
        private readonly IMapper _mapper;
        public BusinessCardsServieces(BusinessCardDbContext context,IBusinessCardsReposetory businessCardsReposetory,IMapper mapper):base(context)
        {

            _context = context;
            _businessCardsReposetory= businessCardsReposetory;
            _mapper = mapper;
            
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




        // Implement the ImportFromCsvAsync method
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


        // Implement the ImportFromXmlAsync method
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



        // Implement the ExportToCsvAsync method
        public async Task<byte[]> ExportToCsvAsync()
        {
            var businessCards = await _businessCardsReposetory.GetAllAsync();
            var businessCardsDtos = _mapper.Map<List<CreateBusinessCardsDto>>(businessCards);

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Name,Gender,DateOfBirth,Email,Phone,Photo,Address,Notes,UserId");

            foreach (var card in businessCardsDtos)
            {
                csvBuilder.AppendLine($"{card.Name},{card.Gender},{card.DateOfBirth?.ToString("yyyy-MM-dd")},{card.Email},{card.Phone},{card.Photo},{card.Address},{card.Notes},{card.UserId}");
            }

            return Encoding.UTF8.GetBytes(csvBuilder.ToString());
        }

        // Implement the ExportToXmlAsync method
        public async Task<byte[]> ExportToXmlAsync()
        {
            var businessCards = await _businessCardsReposetory.GetAllAsync();
            var businessCardsDtos = _mapper.Map<List<CreateBusinessCardsDto>>(businessCards);

            var businessCardsWrapper = new BusinessCardsWrapper
            {
                BusinessCards = businessCardsDtos
            };

            // Serialize the BusinessCardsWrapper to XML
            var xmlSerializer = new XmlSerializer(typeof(BusinessCardsWrapper));
            using (var memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, businessCardsWrapper);
                return memoryStream.ToArray(); // Return the serialized XML as a byte array
            }
        }

    }
}
