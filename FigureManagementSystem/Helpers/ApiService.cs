using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FigureManagementSystem.Models;

namespace FigureManagementSystem.Helpers
{
    public static class ApiService
    {
        public static async Task<string> SendChatAsync(ObservableCollection<ChatMessage> history, string userInput) 
        {
            using var client = new HttpClient();
            var request = new
            {
                message = userInput,
                history = history
            };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:8000/chat", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("reply").GetString();
        }
    }
}
