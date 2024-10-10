using AutoMapper;
using BusinessCardWebAPI.Configrations;
using BusinessCardWebAPI.Controllers;
using BusinessCardWebAPI.Core.Data;
using BusinessCardWebAPI.Core.DTO;
using BusinessCardWebAPI.Core.IServieces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text;



namespace BusinessCardWebAPI.Tests
{
    public class BusinessCardsControllerTests
    {
        private readonly BusinessCardsController _controller;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBusinessCardsServieces> _businessCardsServiceMock;
        private readonly BusinessCardDbContext _contextMock; // Use the in-memory DbContext

        private BusinessCardDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<BusinessCardDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            return new BusinessCardDbContext(options);
        }

        public BusinessCardsControllerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _businessCardsServiceMock = new Mock<IBusinessCardsServieces>();
            _contextMock = GetInMemoryDbContext(); // Initialize the in-memory database context

            // Correct the order of parameters
            _controller = new BusinessCardsController(_mapperMock.Object, _businessCardsServiceMock.Object, _contextMock);
        }


        [Fact]
        public async Task GetBusinessCards_ReturnsListOfBusinessCards_WhenCalled()
        {
            // Arrange
            var context = GetInMemoryDbContext(); // Use in-memory context for testing
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<MapperConfig>()); // Replace AutoMapperProfile with your profile class
            var mapper = mapperConfiguration.CreateMapper();

            // Create mock business cards data
            context.BusinessCards.AddRange(
                new BusinessCards
                {
                    Id = 1,
                    Name = "John Doe",
                    Gender = "Male",
                    DateOfBirth = new DateTime(1990, 5, 15),  // Date of Birth set to May 15, 1990
                    Email = "john@example.com",
                    Phone = "123-456-7890",
                    Photo = "john_photo.jpg",  // Path or file name for the photo
                    Address = "123 Elm Street, Springfield",
                    Notes = "This is a sample note for John Doe.",
                    CreatedAt = DateTime.Now,  // Use the current timestamp for creation
                    UpdatedAt = DateTime.Now,  // Use the current timestamp for update
                    UserId = 10
                },
                new BusinessCards
                {
                    Id = 2,
                    Name = "Jane Doe",
                    Gender = "Female",
                    DateOfBirth = new DateTime(1992, 3, 10),  // Date of Birth set to March 10, 1992
                    Email = "jane@example.com",
                    Phone = "987-654-3210",
                    Photo = "jane_photo.jpg",  // Path or file name for the photo
                    Address = "456 Maple Street, Springfield",
                    Notes = "This is a sample note for Jane Doe.",
                    CreatedAt = DateTime.Now,  // Use the current timestamp for creation
                    UpdatedAt = DateTime.Now,  // Use the current timestamp for update
                    UserId = 11
                }
            );
            await context.SaveChangesAsync();

            // Create mock data to be returned by the service
            var businessCardDtos = new List<GetBusinessCardsDto>
    {
        new GetBusinessCardsDto { Id = 1, Name = "John Doe", Gender = "Male", Email = "john@example.com", Address = "123 Elm Street, Springfield" },
        new GetBusinessCardsDto { Id = 2, Name = "Jane Doe", Gender = "Female", Email = "jane@example.com", Address = "456 Maple Street, Springfield" }
    };

            var businessCardsServiceMock = new Mock<IBusinessCardsServieces>();
            businessCardsServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(context.BusinessCards.ToList());

            // Configure AutoMapper to map business card entities to DTOs
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<List<GetBusinessCardsDto>>(It.IsAny<List<BusinessCards>>()))
                      .Returns(businessCardDtos);

            // Create the controller with the mocked dependencies
            var controller = new BusinessCardsController(mapperMock.Object, businessCardsServiceMock.Object, context);

            // Act
            var result = await controller.GetBusinessCards();

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<GetBusinessCardsDto>>>(result);
            var returnValue = Assert.IsType<List<GetBusinessCardsDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);  // Expecting 2 items since we added 2 business cards
            Assert.Equal("John Doe", returnValue[0].Name); // Check if first entry is "John Doe"
            Assert.Equal("Jane Doe", returnValue[1].Name); // Check if second entry is "Jane Doe"
        }




        [Fact]
        public async Task GetBusinessCardById_ReturnsBusinessCard_WhenIdIsValid()
        {
            // Arrange
            int testId = 1;
            var businessCard = new BusinessCards { Id = testId, Name = "John Doe", Email = "john@example.com" };
            _businessCardsServiceMock.Setup(service => service.GetAsync(testId)).ReturnsAsync(businessCard);

            var businessCardDto = new GetBusinessCardsDto { Id = testId, Name = "John Doe", Email = "john@example.com" };
            _mapperMock.Setup(m => m.Map<GetBusinessCardsDto>(businessCard)).Returns(businessCardDto);

            // Act
            var result = await _controller.GetBusinessCards(testId);

            // Assert
            var okResult = Assert.IsType<ActionResult<GetBusinessCardsDto>>(result);
            var returnValue = Assert.IsType<GetBusinessCardsDto>(okResult.Value);
            Assert.Equal(testId, returnValue.Id);
        }

        //[Fact]
        //public async Task PostBusinessCard_CreatesBusinessCard_WhenCalled()
        //{
        //    // Arrange
        //    var createBusinessCardsDto = new CreateBusinessCardsDto { Name = "John Doe", Email = "john@example.com" };
        //    var businessCard = new BusinessCards { Id = 1, Name = "John Doe", Email = "john@example.com" };

        //    _mapperMock.Setup(m => m.Map<BusinessCards>(createBusinessCardsDto)).Returns(businessCard);
        //    _businessCardsServiceMock.Setup(service => service.AddAsync(businessCard)).Returns(Task.FromResult(businessCard));

        //    // Act
        //    var result = await _controller.PostBusinessCards(createBusinessCardsDto);

        //    // Assert
        //    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        //    Assert.Equal("GetBusinessCards", createdAtActionResult.ActionName);
        //    Assert.Equal(1, ((BusinessCards)createdAtActionResult.Value).Id);
        //}

        [Fact]
        public async Task PostBusinessCard_CreatesBusinessCard_WhenCalled()
        {
            // Arrange
            var createBusinessCardsDto = new CreateBusinessCardsDto { Name = "John Doe", Email = "john@example.com" };
            var businessCard = new BusinessCards { Id = 1, Name = "John Doe", Email = "john@example.com" };

            _mapperMock.Setup(m => m.Map<BusinessCards>(createBusinessCardsDto)).Returns(businessCard);
            _businessCardsServiceMock.Setup(service => service.AddAsync(It.IsAny<BusinessCards>())).ReturnsAsync(businessCard);

            // Act
            var result = await _controller.PostBusinessCards(createBusinessCardsDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetBusinessCards", createdAtActionResult.ActionName);
            Assert.NotNull(createdAtActionResult.Value);  // Ensure Value is not null
            Assert.Equal(1, ((BusinessCards)createdAtActionResult.Value).Id);
        }


        [Fact]
        public async Task PutBusinessCards_UpdatesBusinessCard_WhenIdIsValid()
        {
            // Arrange
            int testId = 1;
            var updateBusinessCardsDto = new UpdateBusinessCardsDto { Id = testId, Name = "John Doe", Email = "john@example.com" };
            var businessCard = new BusinessCards { Id = testId, Name = "John Doe", Email = "john@example.com" };

            _businessCardsServiceMock.Setup(service => service.GetAsync(testId)).ReturnsAsync(businessCard);
            _mapperMock.Setup(m => m.Map(updateBusinessCardsDto, businessCard));
            _businessCardsServiceMock.Setup(service => service.UpdateAsync(businessCard)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PutBusinessCards(testId, updateBusinessCardsDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteBusinessCards_DeletesBusinessCard_WhenIdIsValid()
        {
            // Arrange
            int testId = 1;
            var businessCard = new BusinessCards { Id = testId, Name = "John Doe", Email = "john@example.com" };

            _businessCardsServiceMock.Setup(service => service.GetAsync(testId)).ReturnsAsync(businessCard);
            _businessCardsServiceMock.Setup(service => service.DeleteAsync(testId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteBusinessCards(testId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ImportBusinessCards_ReturnsOk_WhenFileIsValidCsv()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var fileName = "test.csv";
            var content = "Name,Email\nJohn Doe,john@example.com";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);

            var businessCardDtos = new List<CreateBusinessCardsDto> {
                new CreateBusinessCardsDto { Name = "John Doe", Email = "john@example.com" }
            };
            _businessCardsServiceMock.Setup(service => service.ImportFromCsvAsync(It.IsAny<StreamReader>())).ReturnsAsync(businessCardDtos);
            _businessCardsServiceMock.Setup(service => service.AddRangeAsync(It.IsAny<IEnumerable<BusinessCards>>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ImportBusinessCards(fileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("1 business cards imported successfully.", okResult.Value);
        }
    }
}