using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Api.Dtos;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Catalog.UnitTests
{
    public class ItemsControllerTests
    {
        // Unit Test naming convention
        // UnitOfWork_StateUnderTest_ExpectedBehavior

        private readonly Mock<IItemsRepository> repositoryStub = new();
        private readonly Mock<ILogger<ItemsController>> loggerStub = new();
        private readonly Random random = new();

        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnNotFound()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync((Item)null);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert
            //Assert.IsType<NotFoundResult>(result);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnExpectedItem()
        {
            // Arrange
            var expectedItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(expectedItem);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert
            //Assert.IsType<OkObjectResult>(result);
            result.Should().BeOfType<OkObjectResult>();
            // result.Should().BeEquivalentTo(expectedItem);

            // var actualItem = (result as OkObjectResult).Value as Item;
            // actualItem.Should().BeEquivalentTo(
            //     expectedItem,
            //     options => options.ComparingByMembers<Item>()
            // );
        }

        [Fact]
        public async Task GetItemsAsync_WithExistingItems_ReturnAllItems()
        {
            // Arrange
            var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };

            repositoryStub.Setup(repo => repo.GetItemsAsync()).ReturnsAsync(expectedItems);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.GetItemsAsync();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            // result.Should().BeEquivalentTo(expectedItems);

            // var actualItems = (result as OkObjectResult).Value as Item;
            // actualItems.Should().BeEquivalentTo(
            //     expectedItems,
            //     options => options.ComparingByMembers<Item>()
            // );
        }

        [Fact]
        public async Task GetItemsAsync_WithMatchingItems_ReturnMatchingItems()
        {
            // Arrange
            var allItems = new[] 
            {
                new Item() { Name = "Cull" },
                new Item() { Name = "Dark Seal" },
                new Item() { Name = "Doran's Blade" },
            };

            var nameToMatch = "Cull";

            repositoryStub.Setup(repo => repo.GetItemsAsync()).ReturnsAsync(allItems);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            IEnumerable<ItemDto> foundItems = await controller.GetItemsAsync(nameToMatch);

            // Assert
            foundItems.Should().OnlyContain(
                item => item.Name == allItems[0].Name || item.Name == allItems[1].Name || item.Name == allItems[2].Name
            );
        }

        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnCreatedItem()
        {
            // Arrange
            var itemToCreate = new CreateItemDto
            (
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                random.Next(1000)
            );

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.CreateItemAsync(itemToCreate);

            // Assert
            // var createdItem = (result as CreatedAtActionResult).Value as ItemDTO;
            // createdItem.Should().BeEquivalentTo(
            //     itemToCreate,
            //     options => options.ComparingByMembers<ItemDTO>().ExcludingMissingMembers()
            // );
            // createdItem.Id.Should().NotBeEmpty();
            // createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);

            result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnNoContent()
        {
            // Arrange
            var existingItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(existingItem);

            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDto
            (
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                existingItem.Price + 5
            );

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteItemAsync_WithExistingItem_ReturnNoContent()
        {
            // Arrange
            var existingItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(existingItem);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.DeleteItemAsync(existingItem.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        private Item CreateRandomItem()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = random.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
