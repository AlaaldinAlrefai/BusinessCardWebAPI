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
using Newtonsoft.Json;
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

            var context = new BusinessCardDbContext(options);
            return context;
        }


        public BusinessCardsControllerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _businessCardsServiceMock = new Mock<IBusinessCardsServieces>();
            _contextMock = GetInMemoryDbContext(); // Initialize the in-memory database context

            // Correct the order of parameters
            _controller = new BusinessCardsController(_mapperMock.Object, _businessCardsServiceMock.Object, _contextMock);
        }


        // Test Getbusinesscards method
        [Fact]
        public async Task GetBusinessCards_ReturnsListOfBusinessCards_WhenCalled()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString(); // Unique database name for each test
            var options = new DbContextOptionsBuilder<BusinessCardDbContext>()
                              .UseInMemoryDatabase(databaseName)
                              .Options;

            using (var context = new BusinessCardDbContext(options))
            {
                // Create mock business cards data
                context.BusinessCards.AddRange(
                    new BusinessCards
                    {
                        Id = 1,
                        Name = "John Doe",
                        Gender = "Male",
                        DateOfBirth = new DateTime(1990, 5, 15),
                        Email = "john@example.com",
                        Phone = "123-456-7890",
                        Photo = "john_photo.jpg",
                        Address = "123 Elm Street, Springfield",
                        Notes = "This is a sample note for John Doe.",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        UserId = 10
                    },
                    new BusinessCards
                    {
                        Id = 2,
                        Name = "Jane Doe",
                        Gender = "Female",
                        DateOfBirth = new DateTime(1992, 3, 10),
                        Email = "jane@example.com",
                        Phone = "987-654-3210",
                        Photo = "jane_photo.jpg",
                        Address = "456 Maple Street, Springfield",
                        Notes = "This is a sample note for Jane Doe.",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
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

                var mapperMock = new Mock<IMapper>();
                mapperMock.Setup(m => m.Map<List<GetBusinessCardsDto>>(It.IsAny<List<BusinessCards>>()))
                          .Returns(businessCardDtos);

                // Create the controller with the mocked dependencies
                var controller = new BusinessCardsController(mapperMock.Object, businessCardsServiceMock.Object, context);

                // Act
                var result = await controller.GetBusinessCards();

                // Assert
                var actionResult = Assert.IsType<ActionResult<IEnumerable<GetBusinessCardsDto>>>(result);
                var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); // Now check the result within the ActionResult
                var returnValue = Assert.IsType<List<GetBusinessCardsDto>>(okResult.Value);

                Assert.NotNull(okResult.Value); // Check for null
                Assert.Equal(2, returnValue.Count); // Expecting 2 items
                Assert.Equal("John Doe", returnValue[0].Name); // Check first entry
                Assert.Equal("Jane Doe", returnValue[1].Name); // Check second entry
            }
        }


        [Fact]
        public async Task GetBusinessCards_ReturnsEmptyList_WhenNoBusinessCardsExist()
        {
            // Arrange
            var context = GetInMemoryDbContext(); // Use in-memory context for testing

            // Ensure no business cards are in the database
            context.BusinessCards.RemoveRange(context.BusinessCards);
            await context.SaveChangesAsync();

            var businessCardsServiceMock = new Mock<IBusinessCardsServieces>();
            businessCardsServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(new List<BusinessCards>()); // Return empty list of BusinessCards

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<List<GetBusinessCardsDto>>(It.IsAny<List<BusinessCards>>()))
                      .Returns(new List<GetBusinessCardsDto>()); // Return empty list of DTOs

            var controller = new BusinessCardsController(mapperMock.Object, businessCardsServiceMock.Object, context);

            // Act
            var result = await controller.GetBusinessCards();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<GetBusinessCardsDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); // Access Result to check the actual value
            var returnValue = Assert.IsType<List<GetBusinessCardsDto>>(okResult.Value);
            Assert.Empty(returnValue); // Expecting an empty list since no business cards were added
        }


        [Fact]
        public async Task GetBusinessCards_ReturnsIncorrectMapping_WhenMapperConfigurationIsIncorrect()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            // Add a new BusinessCards entity
            context.BusinessCards.Add(new BusinessCards
            {
                Id = 1,
                Name = "John Doe",
                Gender = "Male",
                Email = "john@example.com",
                Address = "123 Elm Street, Springfield",
                Phone = "123-456-7890",
                Photo = "john_photo.jpg",
                Notes = "Sample note",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = 10
            });
            await context.SaveChangesAsync();

            // Incorrect DTO to simulate incorrect mapping.
            var incorrectDto = new List<GetBusinessCardsDto>
    {
        new GetBusinessCardsDto { Id = 1, Name = "Incorrect Name", Gender = "Male", Email = "incorrect@example.com", Address = "Unknown" }
    };

            // Setup mocks
            var businessCardsServiceMock = new Mock<IBusinessCardsServieces>();
            businessCardsServiceMock.Setup(service => service.GetAllAsync())
                                    .ReturnsAsync(context.BusinessCards.ToList());

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<List<GetBusinessCardsDto>>(It.IsAny<List<BusinessCards>>()))
                      .Returns(incorrectDto);

            // Create the controller with mocked dependencies
            var controller = new BusinessCardsController(mapperMock.Object, businessCardsServiceMock.Object, context);

            // Act
            var result = await controller.GetBusinessCards();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<GetBusinessCardsDto>>>(result);

            // Now check if the result is an OkObjectResult
            if (actionResult.Result is OkObjectResult okResult)
            {
                Assert.NotNull(okResult); // Ensure that okResult is not null

                var returnValue = Assert.IsType<List<GetBusinessCardsDto>>(okResult.Value);
                Assert.Single(returnValue); // Expecting a single item

                // Validate incorrect mapping
                Assert.Equal("Incorrect Name", returnValue[0].Name);  // Validate incorrect mapping
                Assert.Equal("incorrect@example.com", returnValue[0].Email);
                Assert.Equal("Unknown", returnValue[0].Address);
            }
            else
            {
                Assert.True(false, "Expected OkObjectResult, but got: " + actionResult.Result?.GetType().ToString());
            }
        }


        [Fact]
        public async Task GetBusinessCards_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var businessCardsServiceMock = new Mock<IBusinessCardsServieces>();
            businessCardsServiceMock.Setup(service => service.GetAllAsync()).ThrowsAsync(new Exception("Test Exception"));

            var mapperMock = new Mock<IMapper>();
            var controller = new BusinessCardsController(mapperMock.Object, businessCardsServiceMock.Object, context);

            // Act
            var result = await controller.GetBusinessCards();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);  // Expecting 500 Internal Server Error
        }



        // Test GetBusinessCardById method
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


        // Test PostBusinessCard method
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


        // Test PutBusinessCards method
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


        // Test DeleteBusinessCards method
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


        // Test ImportBusinessCards method
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


        [Fact]
        public async Task ImportBusinessCards_ReturnsBadRequest_WhenFileIsEmpty()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            using (var stream = new MemoryStream())
            {
                fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
                fileMock.Setup(f => f.ContentType).Returns("text/csv");
                fileMock.Setup(f => f.FileName).Returns("empty.csv");
                fileMock.Setup(f => f.Length).Returns(stream.Length);
            }

            var mapperMock = new Mock<IMapper>();
            var businessCardsServiceMock = new Mock<IBusinessCardsServieces>();

            // Set up the in-memory database context
            var options = new DbContextOptionsBuilder<BusinessCardDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new BusinessCardDbContext(options);

            var controller = new BusinessCardsController(mapperMock.Object, businessCardsServiceMock.Object, context);

            // Act
            var result = await controller.ImportBusinessCards(fileMock.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult);
        }


        [Fact]
        public async Task ImportBusinessCards_ReturnsBadRequest_WhenFileIsNotCsv()
        {
            // Arrange
            var fileContent = "This is not a CSV file.";
            var fileMock = new Mock<IFormFile>();

            // Use a new MemoryStream instance without disposing it immediately
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            // Ensure the stream position is at the beginning before reading
            stream.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.ContentType).Returns("text/plain");
            fileMock.Setup(f => f.FileName).Returns("invalid.txt");
            fileMock.Setup(f => f.Length).Returns(stream.Length);

            var options = new DbContextOptionsBuilder<BusinessCardDbContext>()
                              .UseInMemoryDatabase(databaseName: "TestDatabase")
                              .Options;

            using (var context = new BusinessCardDbContext(options))
            {
                // Mock the other dependencies
                var mapperMock = new Mock<IMapper>();
                var businessCardsServiceMock = new Mock<IBusinessCardsServieces>();

                // Create the controller with mocked dependencies
                var controller = new BusinessCardsController(mapperMock.Object, businessCardsServiceMock.Object, context);

                // Act
                var result = await controller.ImportBusinessCards(fileMock.Object);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

                Assert.NotNull(badRequestResult);
                Assert.Equal("Unsupported file format. Please upload a CSV or XML file.", badRequestResult.Value);
            }
        }


        [Fact]
        public async Task ImportBusinessCards_ReturnsOk_WhenFileIsValidXml()
        {
            // Arrange
            var xmlContent = "<BusinessCards><BusinessCard><Name>John Doe</Name><Gender>Male</Gender><Email>john@example.com</Email>" +
                             "<Address>123 Elm Street</Address><Phone>123-456-7890</Phone><Notes>Sample note</Notes><UserId>10</UserId></BusinessCard></BusinessCards>";

            var fileMock = new Mock<IFormFile>();
            var xmlBytes = Encoding.UTF8.GetBytes(xmlContent);

            // Setup fileMock to return a new stream each time OpenReadStream is called
            fileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(xmlBytes));
            fileMock.Setup(f => f.ContentType).Returns("application/xml");
            fileMock.Setup(f => f.FileName).Returns("valid.xml");
            fileMock.Setup(f => f.Length).Returns(xmlBytes.Length);

            var options = new DbContextOptionsBuilder<BusinessCardDbContext>()
                              .UseInMemoryDatabase(databaseName: "TestDatabase")
                              .Options;

            using (var context = new BusinessCardDbContext(options))
            {
                var mapperMock = new Mock<IMapper>();
                var businessCardsServiceMock = new Mock<IBusinessCardsServieces>();

                var importedDtos = new List<CreateBusinessCardsDto>
        {
            new CreateBusinessCardsDto { Name = "John Doe", Gender = "Male", Email = "john@example.com", Address = "123 Elm Street", Phone = "123-456-7890", Notes = "Sample note", UserId = 10 }
        };
                businessCardsServiceMock.Setup(s => s.ImportFromXmlAsync(It.IsAny<StreamReader>())).ReturnsAsync(importedDtos);

                var controller = new BusinessCardsController(mapperMock.Object, businessCardsServiceMock.Object, context);

                // Act
                var result = await controller.ImportBusinessCards(fileMock.Object);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.NotNull(okResult);
                Assert.Equal("1 business cards imported successfully.", okResult.Value);
            }
        }


        [Fact]
        public async Task ImportBusinessCards_ReturnsBadRequest_WhenFileIsValidButNoRecordsFound()
        {
            // Arrange
            var csvContent = "Name,Email\n"; // CSV header with no records
            var fileMock = new Mock<IFormFile>();
            var csvBytes = Encoding.UTF8.GetBytes(csvContent);

            fileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(csvBytes));
            fileMock.Setup(f => f.ContentType).Returns("text/csv");
            fileMock.Setup(f => f.FileName).Returns("empty.csv");
            fileMock.Setup(f => f.Length).Returns(csvBytes.Length);

            var options = new DbContextOptionsBuilder<BusinessCardDbContext>()
                              .UseInMemoryDatabase(databaseName: "TestDatabase")
                              .Options;

            using (var context = new BusinessCardDbContext(options))
            {
                var mapperMock = new Mock<IMapper>();
                var businessCardsServiceMock = new Mock<IBusinessCardsServieces>();

                businessCardsServiceMock.Setup(s => s.ImportFromCsvAsync(It.IsAny<StreamReader>()))
                    .ReturnsAsync(new List<CreateBusinessCardsDto>()); // No records

                var controller = new BusinessCardsController(mapperMock.Object, businessCardsServiceMock.Object, context);

                // Act
                var result = await controller.ImportBusinessCards(fileMock.Object);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.NotNull(badRequestResult);
                Assert.Equal("No business cards found in the file.", badRequestResult.Value);
            }
        }


        [Fact]
        public async Task ImportBusinessCards_ReturnsInternalServerError_WhenUnexpectedExceptionIsThrown()
        {
            // Arrange
            var csvContent = "Name,Email\nJohn Doe,john@example.com";
            var fileMock = new Mock<IFormFile>();
            var csvBytes = Encoding.UTF8.GetBytes(csvContent);

            fileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(csvBytes));
            fileMock.Setup(f => f.ContentType).Returns("text/csv");
            fileMock.Setup(f => f.FileName).Returns("valid.csv");
            fileMock.Setup(f => f.Length).Returns(csvBytes.Length);

            var options = new DbContextOptionsBuilder<BusinessCardDbContext>()
                              .UseInMemoryDatabase(databaseName: "TestDatabase")
                              .Options;

            using (var context = new BusinessCardDbContext(options))
            {
                var mapperMock = new Mock<IMapper>();
                var businessCardsServiceMock = new Mock<IBusinessCardsServieces>();

                // Simulate an unexpected exception during the import process
                businessCardsServiceMock.Setup(s => s.ImportFromCsvAsync(It.IsAny<StreamReader>()))
                    .ThrowsAsync(new Exception("Unexpected error occurred"));

                var controller = new BusinessCardsController(mapperMock.Object, businessCardsServiceMock.Object, context);

                // Act
                var result = await controller.ImportBusinessCards(fileMock.Object);

                // Assert
                var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
                Assert.NotNull(internalServerErrorResult);
                Assert.Equal(500, internalServerErrorResult.StatusCode);
                Assert.Equal("An error occurred while processing the file. Please try again later.", internalServerErrorResult.Value);
            }
        }


        // Test ImportBusinessCards method
       [Fact]
        public async Task ExportToCsv_Success_ReturnsCsvFile()
        {
        // Arrange
        var csvData = Encoding.UTF8.GetBytes("Id,Name,Email\n1,John Doe,john@example.com");

        var businessCardsServiceMock = new Mock<IBusinessCardsServieces>();
        businessCardsServiceMock.Setup(service => service.ExportToCsvAsync())
                                .ReturnsAsync(csvData);

        var mapperMock = new Mock<IMapper>(); // Create a mock for IMapper

        // Create an in-memory database context
        var options = new DbContextOptionsBuilder<BusinessCardDbContext>()
                          .UseInMemoryDatabase(databaseName: "TestDb")
                          .Options;
        var dbContext = new BusinessCardDbContext(options);

        var controller = new BusinessCardsController(mapperMock.Object, businessCardsServiceMock.Object, dbContext);

        // Act
        var result = await controller.ExportToCsv();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.Equal("BusinessCards.csv", fileResult.FileDownloadName);
        Assert.Equal(csvData, fileResult.FileContents);
        }

        [Fact]
        public async Task ExportToCsv_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var businessCardsServiceMock = new Mock<IBusinessCardsServieces>();
            businessCardsServiceMock.Setup(service => service.ExportToCsvAsync())
                                    .ThrowsAsync(new Exception("Service error"));

            var mapperMock = new Mock<IMapper>();

            var options = new DbContextOptionsBuilder<BusinessCardDbContext>()
                              .UseInMemoryDatabase(databaseName: "TestDb")
                              .Options;
            var dbContext = new BusinessCardDbContext(options);

            var controller = new BusinessCardsController(mapperMock.Object, businessCardsServiceMock.Object, dbContext);

            // Act
            var result = await controller.ExportToCsv();

            // Assert
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
        }



    }
}