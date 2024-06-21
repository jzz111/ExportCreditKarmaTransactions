namespace ParseCreditKarmaHarFile.Test
{
    public class CsvEntryExtensionsTests
    {
        [Fact]
        public void GenerateCsvString_ShouldGenerateCorrectCsvString()
        {
            // Arrange
            var transactions = new List<CsvEntry>
            {
                new CsvEntry
                {
                    Date = "2023-06-01",
                    Description = "Sample transaction",
                    OriginalDescription = "Original description",
                    Amount = "100.00",
                    TransactionType = "credit",
                    Category = "Groceries",
                    AccountName = "Savings",
                    Labels = "Label1,Label2",
                    Notes = "Sample notes"
                },
                // Add more CsvEntry objects as needed
            };

            // Act
            var csvString = CsvEntryExtensions.GenerateCsvString(transactions);

            // Assert
            var expectedCsv = "Date,Description,OriginalDescription,Amount,TransactionType,Category,AccountName,Labels,Notes\n" +
                              "\"2023-06-01\",\"Sample transaction\",\"Original description\",\"100.00\",\"credit\",\"Groceries\",\"Savings\",\"Label1,Label2\",\"Sample notes\"";
            Assert.Equal(expectedCsv, csvString);
        }
    }
}