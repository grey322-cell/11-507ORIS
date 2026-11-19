using System.Text.Json;
using HttpServer;

string jsonText = File.ReadAllText("settings.json");

var config = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonText);
string url = config!["Address"];
string staticDir = config["StaticDirectory"]; 

var server = new HttpServer.HttpServer(url, staticDir);
await server.StartAsync();