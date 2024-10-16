﻿using System;
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
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

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
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<GetBusinessCardsDto>>> GetBusinessCards()
        //{

        //    try
        //    {
        //        var businessCards = await _businessCardsServieces.GetAllAsync();

        //        // Log the business cards retrieved for debugging
        //        Console.WriteLine("Business Cards Retrieved: " + JsonConvert.SerializeObject(businessCards));

        //        if (businessCards == null || !businessCards.Any())
        //        {
        //            Console.WriteLine("No business cards found.");
        //            return Ok(new List<GetBusinessCardsDto>()); // Avoid returning null
        //        }

        //        var dto = _mapper.Map<List<GetBusinessCardsDto>>(businessCards);
        //        return Ok(dto);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception if necessary
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }

        //}






        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<GetBusinessCardsDto>>> GetBusinessCards()
        //{
        //    try
        //    {
        //        var businessCards = await _businessCardsServieces.GetAllAsync();
        //        Console.WriteLine("Business Cards Retrieved: " + JsonConvert.SerializeObject(businessCards));

        //        if (businessCards == null || !businessCards.Any())
        //        {
        //            Console.WriteLine("No business cards found.");
        //            return Ok(new List<GetBusinessCardsDto>());
        //        }

        //        var dto = _mapper.Map<List<GetBusinessCardsDto>>(businessCards);
        //        return Ok(dto);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Exception in GetBusinessCards: " + ex.Message);
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}


        //public async Task<ActionResult<IEnumerable<GetBusinessCardsDto>>> GetBusinessCards()
        //{
        //    try
        //    {
        //        var businessCards = await _businessCardsServieces.GetAllAsync();

        //        if (businessCards == null || !businessCards.Any())
        //        {
        //            return Ok(new List<GetBusinessCardsDto>());
        //        }

        //        var dto = _mapper.Map<List<GetBusinessCardsDto>>(businessCards);
        //        return Ok(dto);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetBusinessCardsDto>>> GetBusinessCards()
        {
            try
            {
                var businessCards = await _businessCardsServieces.GetAllAsync();
                if (businessCards == null || !businessCards.Any())
                {
                    return Ok(new List<GetBusinessCardsDto>()); // Return an empty list if no cards found
                }

                var dto = _mapper.Map<List<GetBusinessCardsDto>>(businessCards);
                return Ok(dto);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
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

            try
            {
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

                // Check if any business cards were imported
                if (importedBusinessCardsDtos.Count == 0)
                {
                    return BadRequest("No business cards found in the file."); // Return bad request if no records
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

                // Save only if there are valid business cards to save
                if (businessCards.Count > 0)
                {
                    await _businessCardsServieces.AddRangeAsync(businessCards);
                    await _context.SaveChangesAsync(); // Save all changes to the database
                }

                return Ok(new
                {
                    message = $"{importedBusinessCardsDtos.Count} business cards imported successfully.",
                    businessCards = importedBusinessCardsDtos // Return the imported DTOs for preview on the frontend
                });
            }
            catch (Exception ex)
            {
                // Log the exception if necessary (e.g., to a logging framework)
                return StatusCode(500, "An error occurred while processing the file. Please try again later.");
            }
        }




        // Export Business Cards to CSV
        //[HttpGet("export/csv")]
        //public async Task<IActionResult> ExportToCsv()
        //{
        //    var csvBytes = await _businessCardsServieces.ExportToCsvAsync();
        //    return File(csvBytes, "text/csv", "BusinessCards.csv");
        //}


        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportToCsv()
        {
            try
            {
                var csvBytes = await _businessCardsServieces.ExportToCsvAsync();
                return File(csvBytes, "text/csv", "BusinessCards.csv");
            }
            catch
            {
                // Return a status code without an object
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        // Export Business Cards to XML
        [HttpGet("export/xml")]
        public async Task<IActionResult> ExportToXml()
        {
            var xmlBytes = await _businessCardsServieces.ExportToXmlAsync();
            return File(xmlBytes, "application/xml", "BusinessCards.xml");
        }

    }
}
