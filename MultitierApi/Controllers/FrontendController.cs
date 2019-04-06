using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FrontendApi.Repositories;
using FrontendApi.Models;
using System;

namespace FrontendApi.Controllers
{
    [Route("api/[controller]")]
    public class FrontendController : Controller
    {
        private ITodoRepository _todo { get; set; }

        public FrontendController(ITodoRepository todoRepository)
        {
            _todo = todoRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<TodoItem>> GetAll()
        {
            try
            {
                var items = await _todo.GetAll();
                return items;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet("{id:length(24)}", Name = "GetTodo")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var item = await _todo.Get(id);

                if (item == null)
                    return NotFound();

                return new ObjectResult(item);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoItem item)
        {
            try
            {
                if (item == null)
                    return BadRequest();

                var itemOut = await _todo.Create(item);

                //return CreatedAtRoute("GetTodo", new { id = item.Id }, itemOut);
                return await GetById(itemOut.Id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, [FromBody] TodoItem item)
        {
            try
            {
                if (item == null || item.Id != id)
                    return BadRequest();

                var todo = await _todo.Get(id);
                if (todo == null)
                    return NotFound();

                todo.IsComplete = item.IsComplete;
                todo.Name = item.Name;

                await _todo.Update(id, todo);
                return new NoContentResult();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var todo = await _todo.Get(id);
                if (todo == null)
                    return NotFound();

                await _todo.Remove(id);
                return new NoContentResult();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

