using AutoMapper;
using HarSharp;
using ParseCreditKarmaHarFile;
using System.Text.Json;
using System.Text.Json.Nodes;

const string USAGE = "USAGE: dotnet ParseCreditKarmaHarFile.dll <input_har_filename> <output_csv_filename> <output_error_json_filename>";

if (args.Length < 3)
{
    Console.Error.WriteLine("There are not enough arguments presented.");
    Console.WriteLine(USAGE);
}

string INPUT_FILE = args[0];//"allMintTransactions.har";
string OUTPUT_FILE = args[1];// "mintTransactions.csv";
string ERROR_FILE = args[2];// "error.json";

if (!File.Exists(INPUT_FILE))
{
    Console.Error.WriteLine("The input HAR file does not exist.");
    Console.WriteLine(USAGE);
}

var harFile = HarConvert.DeserializeFromFile(INPUT_FILE);

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

var csvEntries = harFile.Log.Entries.Where(m => m.Request.Url.Segments.LastOrDefault()?.StartsWith("graphql") ?? false)
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

var csvStr = csvEntries.GenerateCsvString();

File.WriteAllText(OUTPUT_FILE, csvStr);
File.WriteAllText(ERROR_FILE, JsonSerializer.Serialize(errorEntries, new JsonSerializerOptions { WriteIndented = true }));