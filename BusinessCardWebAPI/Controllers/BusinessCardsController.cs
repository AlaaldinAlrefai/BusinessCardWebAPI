using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessCardWebAPI.Core.Data;
using BusinessCardWebAPI.Core.IServieces;
using AutoMapper;
using BusinessCardWebAPI.Core.DTO;
using BusinessCardWebAPI.Infra.Servieces;

namespace BusinessCardWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessCardsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBusinessCardsServieces _businessCardsServieces;
        private readonly BusinessCardDbContext _context;

        public BusinessCardsController(IMapper mapper, IBusinessCardsServieces businessCardsServieces, BusinessCardDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _businessCardsServieces = businessCardsServieces ?? throw new ArgumentNullException(nameof(businessCardsServieces));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/BusinessCards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetBusinessCardsDto>>> GetBusinessCards()
        {
          if (_context.BusinessCards == null)
          {
              return NotFound();
          }
            var businessCards = await _businessCardsServieces.GetAllAsync();
            var record = _mapper.Map<List<GetBusinessCardsDto>>(businessCards);
            return record ;
        }

        // GET: api/BusinessCards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetBusinessCardsDto>> GetBusinessCards(int id)
        {
          if (id <= 0)
          {
                return BadRequest("Invalid Id");
            }
            var businessCards = await _businessCardsServieces.GetAsync(id);
            
            if (businessCards == null)
            {
                return NotFound();
            }
            var record = _mapper.Map<GetBusinessCardsDto>(businessCards);

            return record;
        }

        // PUT: api/BusinessCards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBusinessCards(int id, UpdateBusinessCardsDto updateBusinessCardsDto)
        {
            if (id != updateBusinessCardsDto.Id)
            {
                return BadRequest("Recored Not Exist");
            }

            var businessCards = await _businessCardsServieces.GetAsync(id);
            if (businessCards == null)
            {
                return NotFound("Does Not Exist");
            }
            _mapper.Map(updateBusinessCardsDto, businessCards);

            try
            {
                await _businessCardsServieces.UpdateAsync(businessCards);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (! await BusinessCardsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(businessCards);
        }

        // POST: api/BusinessCards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BusinessCards>> PostBusinessCards(CreateBusinessCardsDto createBusinessCardsDto)
        {
            if (createBusinessCardsDto == null)
            {
                return Problem("CreateBusinessCardsDto cannot be null.");
            }
            var businessCards= _mapper.Map<BusinessCards>(createBusinessCardsDto);
            var createdBusinessCard = await _businessCardsServieces.AddAsync(businessCards);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBusinessCards", new { id = businessCards.Id }, createdBusinessCard);
        }

        // DELETE: api/BusinessCards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusinessCards(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Id");
            }
            var businessCards = await _businessCardsServieces.GetAsync(id);
            if (businessCards == null)
            {
                return NotFound();
            }

            await _businessCardsServieces.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> BusinessCardsExists(int id)
        {
            
            return await _businessCardsServieces.Exists(id) ;
            
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<GetBusinessCardsDto>>> GetFilteredBusinessCards(
            [FromQuery] string? name,
            [FromQuery] string? email,
            [FromQuery] string? phone,
            [FromQuery] string? gender,
            [FromQuery] DateTime? dob)
        {
            var filteredCards = await _businessCardsServieces.FilterBusinessCards(
                name, email, phone, gender, dob);

            if (filteredCards == null || !filteredCards.Any())
            {
                return NotFound("No business cards match the specified criteria.");
            }
            var result = _mapper.Map<List<GetBusinessCardsDto>>(filteredCards);
            return Ok(result);
        }



        [HttpPost("import")]
        public async Task<IActionResult> ImportBusinessCards(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded or the file is empty.");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            List<CreateBusinessCardsDto> importedBusinessCardsDtos = new();

            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                switch (fileExtension)
                {
                    case ".csv":
                        importedBusinessCardsDtos = await _businessCardsServieces.ImportFromCsvAsync(stream);
                        break;
                    case ".xml":
                        importedBusinessCardsDtos = await _businessCardsServieces.ImportFromXmlAsync(stream);
                        break;
                    default:
                        return BadRequest("Unsupported file format. Please upload a CSV or XML file.");
                }
            }

            // Convert DTOs to Entities
            var businessCards = importedBusinessCardsDtos
                .Select(dto => new BusinessCards
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
                })
                .ToList();

            if (businessCards.Count > 0)
            {
                // Ensure this method accepts IEnumerable<BusinessCards>
                await _businessCardsServieces.AddRangeAsync(businessCards);
                await _context.SaveChangesAsync(); // Save all changes to the database
            }

            return Ok($"{importedBusinessCardsDtos.Count} business cards imported successfully.");
        }





    }
}
