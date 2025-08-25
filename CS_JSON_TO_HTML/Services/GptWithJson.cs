using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CS_JSON_TO_HTML.Services
{
    internal class GptWithJson
    {

       
        string apiKey = "[THE-AZURE-OPEN-AI-SERVICE]"; // Replace with your actual key
        string jsonArray; // Load JSON from file
        int chunkSize = 100;
        int targetCustomerId = 11;

        public GptWithJson()
        {
            string curDir = Directory.GetCurrentDirectory();

            string projectDirectory = Directory.GetParent(curDir)?.Parent?.Parent?.ToString();


            string jsonFilePath = Path.Combine(projectDirectory, "jsonfiles", "complex_company_data.json");
              jsonArray = File.ReadAllText(jsonFilePath); // Load JSON from file
        }

        public async Task<string> Run(string prompt)
        {
            string result = string.Empty;
            var chunks = SplitJsonIntoChunks(jsonArray, chunkSize);
            double grandTotal = 0;

            foreach (var chunk in chunks)
            {
                //string systemPrompt = $"Given this JSON array of customer orders:\n{chunk}\nCalculate the total sum of 'amount' for customerId = {targetCustomerId}. Only return the number.";

                string systemPrompt = $"Given this JSON array as:\n{chunk}\n. Use this data to provide me result based on my question as: {prompt}.";

                  result = await QueryGptAsync(apiKey, systemPrompt);

                //if (double.TryParse(result, out double partialTotal))
                //{
                //    grandTotal += partialTotal;
                //}
                //else
                //{
                //    Console.WriteLine($" Could not parse GPT response: {result}");
                //}
            }

            //Console.WriteLine($"Grand Total for customer {targetCustomerId}: {grandTotal}");
            //return grandTotal.ToString();
            return result;
        }

        //static List<string> SplitJsonIntoChunks(string jsonArray, int chunkSize)
        //{
        //    var chunks = new List<string>();

        //    var array = JsonNode.Parse(jsonArray)?.AsArray();

        //    if (array == null)
        //        throw new Exception("Invalid JSON array");

        //    for (int i = 0; i < array.Count; i += chunkSize)
        //    {
        //        var chunk = new JsonArray();

        //        for (int j = i; j < i + chunkSize && j < array.Count; j++)
        //        {
        //            // chunk.Add(array[j]);
        //            chunk.Add(JsonNode.Parse(array[j].ToJsonString()));
        //        }

        //        chunks.Add(chunk.ToJsonString(new JsonSerializerOptions { WriteIndented = false }));
        //    }

        //    return chunks;
        //}

        static List<string> SplitJsonIntoChunks(string json, int chunkSize)
        {
            var chunks = new List<string>();
            var root = JsonNode.Parse(json);

            if (root is JsonArray array)
            {
                // Chunk array elements
                for (int i = 0; i < array.Count; i += chunkSize)
                {
                    var chunk = new JsonArray();
                    for (int j = i; j < i + chunkSize && j < array.Count; j++)
                    {
                        chunk.Add(JsonNode.Parse(array[j].ToJsonString()));
                    }
                    chunks.Add(chunk.ToJsonString(new JsonSerializerOptions { WriteIndented = false }));
                }
            }
            else if (root is JsonObject obj)
            {
                // Chunk object properties
                var properties = obj.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                var keys = properties.Keys.ToList();

                for (int i = 0; i < keys.Count; i += chunkSize)
                {
                    var chunk = new JsonObject();
                    for (int j = i; j < i + chunkSize && j < keys.Count; j++)
                    {
                        var key = keys[j];
                        chunk[key] = JsonNode.Parse(properties[key].ToJsonString());
                    }
                    chunks.Add(chunk.ToJsonString(new JsonSerializerOptions { WriteIndented = false }));
                }
            }
            else
            {
                // Treat as single value (string, number, bool, etc.)
                chunks.Add(root.ToJsonString(new JsonSerializerOptions { WriteIndented = false }));
            }

            return chunks;
        }


        static async Task<string> QueryGptAsync(string apiKey, string prompt)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-4.1",
                messages = new[]
                {
                new { role = "user", content = prompt }
            },
                temperature = 0.2
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("[THE-AZURE-OPEN-AI-ENDPOINT]", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                return doc.RootElement
                          .GetProperty("choices")[0]
                          .GetProperty("message")
                          .GetProperty("content")
                          .GetString()
                          ?.Trim() ?? "0";
            }
            catch
            {
                return "0";
            }
        }
    }
}
