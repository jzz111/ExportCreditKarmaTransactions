namespace ParseCreditKarmaHarFile
{
    public class Transaction
    {
        public Account Account { get; set; } = default!;
        public Amount Amount { get; set; } = default!;
        public Category Category { get; set; } = default!;
        public DateOnly Date { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Id { get; set; } = default!;
        public Merchant? Merchant { get; set; } = default!;
        public string Status { get; set; } = default!;
    }

    public class Account
    {
        public string AccountTypeAndNumberDisplay { get; set; } = default!;
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string ProviderName { get; set; } = default!;
        public string Type { get; set; } = default!;
    }

    public class Amount
    {
        public string AsCurrencyString { get; set; } = default!;
        public decimal Value { get; set; } = default!;
    }

    public class Category
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
    }

    public class Merchant
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
