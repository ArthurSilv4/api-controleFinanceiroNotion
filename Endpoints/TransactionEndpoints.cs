using api_controleFinanceiroNotion.Notion;
using System.Globalization;
using System.Text.RegularExpressions;

namespace api_controleFinanceiroNotion.Endpoints
{
    public static class TransactionEndpoints
    {
        public static void MapTransactionEndpoints(this WebApplication app)
        {
            var configuration = app.Services.GetRequiredService<IConfiguration>();

            var notionSection = configuration.GetSection("Notion");
            var notionService = new NotionService(
                notionToken: notionSection["Token"] ?? "",
                databaseId: notionSection["Database"] ?? "",
                version: notionSection["Version"] ?? ""
            );

            app.MapPost("/transaction", async (string messagem) =>
            {
                var transaction = new Models.Transaction("Saida", 0, "Sem descrição", "");

                var match = Regex.Match(
                    messagem,
                    @"\/(\+\+|--)\s*(.*?)\s*-\s*([\d.,]+)\s*-\s*(.+)"
                );

                if (match.Success)
                {
                    transaction.Type = match.Groups[1].Value == "++" ? "Entrada" : "Saida";

                    transaction.Title = match.Groups[2].Success && !string.IsNullOrWhiteSpace(match.Groups[2].Value)
                        ? match.Groups[2].Value.Trim()
                        : "Sem descrição";

                    transaction.Amount = match.Groups[3].Success && !string.IsNullOrWhiteSpace(match.Groups[3].Value)
                        ? decimal.Parse(
                            (match.Groups[1].Value == "--" ? "-" : "") + match.Groups[3].Value.Trim(),
                            CultureInfo.InvariantCulture
                          )
                        : 0;

                    transaction.Category = match.Groups[4].Success && !string.IsNullOrWhiteSpace(match.Groups[4].Value)
                        ? match.Groups[4].Value.Trim()
                        : "";
                }

                var result = await notionService.SendTransactionAsync(transaction);

                return result.Success ? Results.Ok(result.Message) : Results.BadRequest(result.Message);
            });
        }
    }
}
