using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackendController : Controller
    {
        private readonly IMongoDbRepository _mongodb;

        public BackendController(IMongoDbRepository mongoDbRepository)
        {
            try
            {
                _mongodb = mongoDbRepository;
                if (_mongodb.Get().Count() == 0)
                    _mongodb.Create(new TodoItem()
                    {
                        Name = "Item1",
                        IsComplete = false,
                    });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            try
            {
                return _mongodb.Get();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet("{id:length(24)}", Name = "GetTodo")]
        public IActionResult GetById(string id)
        {
            try
            {
                var item = _mongodb.Get(id);
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
        public IActionResult Create([FromBody] TodoItem item)
        {
            try
            {
                if (item == null)
                    return BadRequest();

                _mongodb.Create(item);
                return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, [FromBody] TodoItem item)
        {
            try
            {
                if (item == null || item.Id != id)
                    return BadRequest();

                var todo = _mongodb.Get(id);
                if (todo == null)
                    return NotFound();

                todo.IsComplete = item.IsComplete;
                todo.Name = item.Name;

                _mongodb.Update(id, todo);
                return new NoContentResult();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var todo = _mongodb.Get(id);
                if (todo == null)
                    return NotFound();

                _mongodb.Remove(id);
                return new NoContentResult();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
