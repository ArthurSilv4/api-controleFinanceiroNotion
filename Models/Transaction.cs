namespace api_controleFinanceiroNotion.Models
{
    public class Transaction
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }

        public Transaction(string title, decimal amount, string category, string type)
        {
            Title = title;
            Amount = amount;
            Category = category;
            Type = type;
        }
    }
}
