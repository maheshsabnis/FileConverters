using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Azure.AI.DocumentIntelligence;

namespace CS_JSON_TO_HTML.Services
{
    internal class HtmlDocumentProcessor
    {
        string endpoint = "https://[COGNITIVE-SERVICE].cognitiveservices.azure.com/";
        string key = "[THE-KEY-HERE]";

        DocumentIntelligenceClient client;

        public HtmlDocumentProcessor()
        {
            client = new DocumentIntelligenceClient(new Uri(endpoint), new AzureKeyCredential(key));
        }
        
                public async Task<string> ProcessHtmlDocumentAsync(string htmlFile)
                {
                    string result = string.Empty;
                    var htmlContent = File.OpenRead(htmlFile);
                    var options = new AnalyzeDocumentOptions("prebuilt-layout", BinaryData.FromStream(htmlContent));

                    var request = client.AnalyzeDocumentAsync(WaitUntil.Completed, options);

                    var documentResult =  request.Result.Value;

                    if (documentResult != null) 
                    {
                        //foreach (var table in documentResult.Tables)
                        //{
                        //    Console.WriteLine($"\n📊 Table with {table.RowCount} rows and {table.ColumnCount} columns");

                        //    for (int row = 0; row < table.Cells.Count; row++)
                        //    {
                        //        var cell = table.Cells[row];
                        //        Console.WriteLine($"Row {cell.RowIndex}, Column {cell.ColumnIndex}: {cell.Content}");
                        //    }
                        //}
                        foreach (var table in documentResult.Tables)
                        {
                            Console.WriteLine($"\n📊 Table with {table.RowCount} rows and {table.ColumnCount} columns");

                            // Create a 2D array to hold cell contents
                            string[,] tableGrid = new string[table.RowCount, table.ColumnCount];

                            // Fill the grid with cell contents
                            foreach (var cell in table.Cells)
                            {
                                tableGrid[cell.RowIndex, cell.ColumnIndex] = cell.Content;
                            }

                            // Print the table row by row
                            for (int row = 0; row < table.RowCount; row++)
                            {
                                for (int col = 0; col < table.ColumnCount; col++)
                                {
                                    string content = tableGrid[row, col] ?? ""; // Handle empty cells
                                    Console.Write($"| {content,-20}"); // Adjust width as needed
                                }
                                Console.WriteLine("|");
                            }
                        }

                    }
                    else
                    {
                       throw new Exception("Document processing failed.");
                    }

                    return result;
                }

    }
}
