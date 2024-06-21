using HarSharp;
using ParseCreditKarmaHarFile;
using System.Text.Json;

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

var result = HarExtractorService.ExtractHarFile(harFile);

var csvStr = result.Item1.GenerateCsvString();

File.WriteAllText(OUTPUT_FILE, csvStr);
File.WriteAllText(ERROR_FILE, JsonSerializer.Serialize(result.Item2, new JsonSerializerOptions { WriteIndented = true }));