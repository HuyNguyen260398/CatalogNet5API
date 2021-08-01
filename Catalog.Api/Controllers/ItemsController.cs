using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Api.Dtos;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;
        private readonly ILogger<ItemsController> logger;

        public ItemsController(IItemsRepository repository, ILogger<ItemsController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetItemsAsync()
        {
            var items = (await repository.GetItemsAsync()).Select(item => item.AsDTO());
            logger.LogInformation($"{DateTime.UtcNow.ToString()}: Retrieved {items.Count()} items");
            return Ok(items);
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync(string name = null)
        {
            var items = (await repository.GetItemsAsync()).Select(item => item.AsDTO());

            if (!string.IsNullOrWhiteSpace(name)) 
                items = items.Where(item => item.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            logger.LogInformation($"{DateTime.UtcNow.ToString()}: Retrieved {items.Count()} items");
            return items;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemAsync(Guid id)
        {
            var item = await repository.GetItemAsync(id);

            if (item is null)
                return NotFound();

            return Ok(item.AsDTO());
        }

        [HttpPost]
        public async Task<IActionResult> CreateItemAsync(CreateItemDto itemDTO)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDTO.Name,
                Description = itemDTO.Description,
                Price = itemDTO.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await repository.CreateItemAsync(item);
            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDTO());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDTO)
        {
            var existingItem = await repository.GetItemAsync(id);

            if (existingItem is null)
                return NotFound();

            existingItem.Name = itemDTO.Name;
            existingItem.Description = itemDTO.Description;
            existingItem.Price = itemDTO.Price;

            await repository.UpdateItemAsync(existingItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemAsync(Guid id)
        {
            var existingItem = await repository.GetItemAsync(id);

            if (existingItem is null)
                return NotFound();

            await repository.DeleteItemAsync(id);
            return NoContent();
        }
    }
}