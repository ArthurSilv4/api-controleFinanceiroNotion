using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using api_controleFinanceiroNotion.Models;


namespace api_controleFinanceiroNotion.Notion
{
    public class NotionService
    {
        private readonly string _token;
        private readonly string _databaseId;
        private readonly string _version;
        private readonly HttpClient _httpClient;

        public NotionService(string notionToken, string databaseId, string version)
        {
            _token = notionToken;
            _databaseId = databaseId;
            _version = version;
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            _httpClient.DefaultRequestHeaders.Add("Notion-Version", _version);
        }

        public async Task<(bool Success, string Message)> SendTransactionAsync(Transaction transaction)
        {
            var payload = new
            {
                parent = new { database_id = _databaseId },
                properties = new
                {
                    Titulo = new
                    {
                        title = new[] {
                        new {
                            text = new { content = transaction.Title }
                        }
                    }
                    },
                    Valor = new { number = transaction.Amount },
                    Categoria = new
                    {
                        multi_select = new[] {
                            new { name = transaction.Category }
                        }
                    },
                    Tipo = new { select = new { name = transaction.Type } }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.notion.com/v1/pages", content);

            var message = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode
                ? (true, "Transaction sent successfully!")
                : (false, $"Failed to send to Notion: {message}");
        }
    }
}
