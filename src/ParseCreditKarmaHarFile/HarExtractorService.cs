using AutoMapper;
using HarSharp;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace ParseCreditKarmaHarFile
{
    public static class HarExtractorService
    {
        public static (List<CsvEntry>, List<JsonNode>) ExtractHarFile(Har harFile)
        {
            var config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<Transaction, CsvEntry>()
                    .ForMember(dest => dest.Amount, act => act.MapFrom(src => src.Amount.Value))
                    .ForMember(dest => dest.OriginalDescription, act => act.MapFrom(src => src.Description))
                    .ForMember(dest => dest.TransactionType, act => act.MapFrom(src => src.Amount.Value < 0 ? "credit" : "debit"))
                    .ForMember(dest => dest.Category, act => act.MapFrom(src => src.Category.Name))
                    .ForMember(dest => dest.AccountName, act => act.MapFrom(src => src.Account.Name))
            );
            var mapper = config.CreateMapper();

            var errorEntries = new List<JsonNode>();
            var result = harFile.Log.Entries.Where(m => m.Request.Url.Segments.LastOrDefault()?.StartsWith("graphql") ?? false)
                .SelectMany(entry =>
                {
                    var obj = JsonNode.Parse(entry.Response.Content.Text);

                    if (obj != null &&
                        obj["data"] != null &&
                        obj["data"]?["prime"] != null &&
                        obj["data"]?["prime"]?["transactionList"] != null &&
                        obj["data"]?["prime"]?["transactionList"]?["transactions"] != null)
                    {
                        var currTrans = obj["data"]?["prime"]?["transactionList"]?["transactions"].Deserialize<List<Transaction>>(
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (currTrans != null && currTrans.Count > 0)
                            return mapper.Map<List<CsvEntry>>(currTrans);
                    }
                    else if (obj != null &&
                        obj["data"] != null &&
                        obj["data"]?["prime"] != null &&
                        obj["data"]?["prime"]?["transactionsHub"] != null &&
                        obj["data"]?["prime"]?["transactionsHub"]?["transactionPage"] != null)
                    {
                        var currTrans = obj["data"]?["prime"]?["transactionsHub"]?["transactionPage"]?["transactions"].Deserialize<List<Transaction>>(
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (currTrans != null && currTrans.Count > 0)
                            return mapper.Map<List<CsvEntry>>(currTrans);
                    }
                    else if (obj != null)
                    {
                        errorEntries.Add(obj);
                    }

                    return new List<CsvEntry>();
                }).ToList();

            return (result, errorEntries);
        }
    }
}
