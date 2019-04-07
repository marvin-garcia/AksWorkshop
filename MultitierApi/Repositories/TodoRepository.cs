using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using FrontendApi.Models;

namespace FrontendApi.Repositories
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoItem>> GetAll();
        Task<TodoItem> Get(string id);
        Task<TodoItem> Create(TodoItem item);
        Task Update(string id, TodoItem itemIn);
        Task Remove(string id);
    }

    public class TodoRepository : ITodoRepository
    {
        private string _backendUrl { get; set; }
        private IHttpClient _httpClient { get; set; }

        public TodoRepository(IConfiguration configuration, IHttpClient httpClient)
        {
            _httpClient = httpClient;
            _backendUrl = $"{configuration["backendurl"]}/api/todo";
        }

        public async Task<IEnumerable<TodoItem>> GetAll()
        {
            var response = await _httpClient.GetStringAsync($"{_backendUrl}");
            var items = JsonConvert.DeserializeObject<IEnumerable<TodoItem>>(response);
            return items;
        }

        public async Task<TodoItem> Get(string id)
        {
            var response = await _httpClient.GetStringAsync($"{_backendUrl}/{id}");
            var item = JsonConvert.DeserializeObject<TodoItem>(response);
            return item;
        }

        public async Task<TodoItem> Create(TodoItem item)
        {
            var response = await _httpClient.PostAsync($"{_backendUrl}/", item);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            var itemOut = JsonConvert.DeserializeObject<TodoItem>(await response.Content.ReadAsStringAsync());
            return itemOut;
        }

        public async Task Update(string id, TodoItem itemIn)
        {
            var response = await _httpClient.PutAsync($"{_backendUrl}/{id}", itemIn);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            return;
        }

        public async Task Remove(string id)
        {
            var response = await _httpClient.DeleteAsync($"{_backendUrl}/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            return;
        }
    }
}
