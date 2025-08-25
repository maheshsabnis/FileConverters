using System;
using System.IO;
using System.Text;
using System.Text.Json;
using CS_JSON_TO_HTML.Services;


class Program
{
    static async Task Main(string[] args)
    {
        //JsonToHtmlConverter converter = new JsonToHtmlConverter();
        //// Read the filepath for json file from the Project
        //var curDir = Directory.GetCurrentDirectory();

        //var projectDirectory = Directory.GetParent(curDir)?.Parent?.Parent?.ToString();


        //string jsonFilePath = Path.Combine(projectDirectory, "jsonfiles" , "customers_data_large.json");



        //// Load JSON (you may load from file instead)
        //string json = File.ReadAllText(jsonFilePath);
        //using JsonDocument jsonDocument = JsonDocument.Parse(json);

        //// Convert JSON to HTML
        //string html = converter.ConvertJsonToHtml(jsonDocument.RootElement);

        //string outputFilePath = Path.Combine(projectDirectory, "output.html");
        //// Save HTML
        //File.WriteAllText(outputFilePath, html);
        //Console.WriteLine("HTML file generated: output.html");
        //HtmlDocumentProcessor processor = new HtmlDocumentProcessor();

        //string result = await processor.ProcessHtmlDocumentAsync(outputFilePath);

        //Console.WriteLine($"Document processing result: {result}");

        var canContinue = "y";
        do
        {
            GptWithJson gpt = new GptWithJson();
            Console.WriteLine("Enter the Prompt");
            //string prompt = "Show list of Product Names ordered by the customerId as C0001";
            string prompt = Console.ReadLine();
            string gptResult = await gpt.Run(prompt);
            Console.WriteLine($"GPT Result: {gptResult}");
            Console.WriteLine("Do you want to continue? (y/n)");
            canContinue = Console.ReadLine();

        } while (canContinue == "y");
        Console.ReadLine();
    }
}
