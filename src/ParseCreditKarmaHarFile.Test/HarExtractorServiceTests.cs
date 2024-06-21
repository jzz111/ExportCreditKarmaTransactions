using HarSharp;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ParseCreditKarmaHarFile.Test
{
    public class HarExtractorServiceTests
    {
        [Fact]
        public void ExtractHarFile_ReturnsValidResults()
        {
            // Arrange
            var harFile = HarConvert.DeserializeFromFile("SampleHar.json");
            var resp = harFile.Log.Entries.Where(m => m.Request.Url.Segments.LastOrDefault()?.StartsWith("graphql") ?? false).ToList();

            // Getting actual value
            var node = JsonNode.Parse(resp[0].Response.Content.Text); 
            var expectedTransaction = node["data"]?["prime"]?["transactionsHub"]?["transactionPage"]?["transactions"]
                .Deserialize<List<Transaction>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })[0];

            // Act
            var (csvEntries, errorEntries) = HarExtractorService.ExtractHarFile(harFile);

            // Assert
            Assert.NotNull(csvEntries);
            Assert.NotNull(errorEntries);
            Assert.Single(csvEntries);

            var actualTransaction = csvEntries[0];
            Assert.Equal(expectedTransaction.Account.Name, actualTransaction.AccountName);
        }
    }
}
