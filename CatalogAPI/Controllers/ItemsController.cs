﻿using CatalogAPI.Entities;
using CatalogAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogAPI.Controllers
{


    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {

        private readonly IItemsRepository _repository;
        private readonly ILogger<ItemsController> _logger;


        public ItemsController(IItemsRepository repository, ILogger<ItemsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }



        // GET /items
        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync() 
        {
            var items = (await _repository.GetItemsAsync()).Select(item => item.AsDto());
            _logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Retrieved {items.Count()} items");

            return items;
        }



        // GET /items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id) 
        {
            var item = await _repository.GetItemAsync(id);

            if (item is null)
            {
                return NotFound();
            }

            _logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Retrieved Id: {item.Id}");


            return item.AsDto();
        }



        // POST /items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto createItemDto) 
        {
            Item item = new() 
            {
                Id = Guid.NewGuid(),
                Name = createItemDto.Name,
                Price = createItemDto.Price,
                Description = createItemDto.Description,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _repository.CreateItemAsync(item);

            _logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Created Item: {item.Id} {item.Name} {item.Price}");


            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
        }



        // PUT /items/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto updateItemDto) 
        {

            var existingItem = await _repository.GetItemAsync(id);

            if (existingItem is null)
            {

                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            //Creating copy - with-expressions - Item is record
            //Item updatedItem = existingItem with
            //{
            //    Name = updateItemDto.Name,
            //    Price = updateItemDto.Price
            //};

            await _repository.UpdateItemAsync(existingItem);

            _logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Updated Item: {existingItem.Id}");


            return NoContent();
        }



        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id)
        {

            var existingItem = await _repository.GetItemAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            await _repository.DeleItemAsync(id);

            _logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Deleted Item: {existingItem.Id}");


            return NoContent();
        }
    }
}
