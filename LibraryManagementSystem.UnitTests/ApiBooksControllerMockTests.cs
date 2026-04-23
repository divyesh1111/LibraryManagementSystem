using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Controllers.Api.V1;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LibraryManagementSystem.UnitTests
{
    public class ApiBooksControllerMockTests
    {
        private static Mock<IBookService> MakeMock(PagedResult<Book> result)
        {
            var mock = new Mock<IBookService>();
            mock.Setup(s => s.GetPagedAsync(
                    It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                    It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<string?>(),
                    It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(result);
            return mock;
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WithPagedResult()
        {
            // Arrange
            var mockResult = new PagedResult<Book>
            {
                Items = new List<Book> { new Book { Id = 1, Title = "Test", ISBN = "T001", AuthorId = 1 } },
                TotalCount = 1,
                Page = 1,
                PageSize = 12
            };
            var mock = MakeMock(mockResult);
            var controller = new ApiBooksController(mock.Object);

            // Act
            var result = await controller.Get(null, null, null, null, null, null, 1, 12);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var payload = Assert.IsType<PagedResult<Book>>(ok.Value);
            Assert.Equal(1, payload.TotalCount);
            Assert.Single(payload.Items);

            // Verify service was called exactly once
            mock.Verify(s => s.GetPagedAsync(null, null, null, null, null, null, 1, 12), Times.Once);
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WithEmptyList_WhenNoBooksExist()
        {
            // Arrange
            var mockResult = new PagedResult<Book>
            {
                Items = new List<Book>(),
                TotalCount = 0,
                Page = 1,
                PageSize = 12
            };
            var mock = MakeMock(mockResult);
            var controller = new ApiBooksController(mock.Object);

            // Act
            var result = await controller.Get(null, null, null, null, null, null, 1, 12);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var payload = Assert.IsType<PagedResult<Book>>(ok.Value);
            Assert.Equal(0, payload.TotalCount);
            Assert.Empty(payload.Items);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            var mock = new Mock<IBookService>();
            mock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Book?)null);

            var controller = new ApiBooksController(mock.Object);

            // Act
            var result = await controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            mock.Verify(s => s.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenBookExists()
        {
            // Arrange
            var book = new Book { Id = 5, Title = "Found Book", ISBN = "F001", AuthorId = 1 };
            var mock = new Mock<IBookService>();
            mock.Setup(s => s.GetByIdAsync(5))
                .ReturnsAsync(book);

            var controller = new ApiBooksController(mock.Object);

            // Act
            var result = await controller.GetById(5);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var payload = Assert.IsType<Book>(ok.Value);
            Assert.Equal(5, payload.Id);
            Assert.Equal("Found Book", payload.Title);
        }
    }
}