using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.DTOs;
using Catalog.Entities;
using Catalog.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;

        public ItemsController(IItemsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ItemDTO>> GetItems()
        {
            var items = repository.GetItems().Select(item => item.AsDTO());
            return Ok(items);
        }

        [HttpGet("{id}")]
        public ActionResult<ItemDTO> GetItem(Guid id)
        {
            var item = repository.GetItem(id);

            if (item is null)
            {
                return NotFound();
            }

            return Ok(item.AsDTO());
        }
    }
}