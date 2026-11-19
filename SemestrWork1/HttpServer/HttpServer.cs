using System.Net;
using System.Text;

namespace HttpServer;

public class HttpServer
{
    private readonly HttpListener Listener; 
    private readonly string Url; 
    private readonly string StaticDirectory; 
    
    public HttpServer(string url, string staticDirectory)
    {
        Url = url;
        Listener = new HttpListener();
        Listener.Prefixes.Add(Url); 
        StaticDirectory = staticDirectory;
    }
    
    public async Task StartAsync()
    {
        Listener.Start(); 
        Console.WriteLine($"Сервер запущен и ждёт гостей по адресу: {Url}");
        
        while (Listener.IsListening)
        {
            
            var context = await Listener.GetContextAsync();
            var request = context.Request;
            string rawPath = request.Url?.AbsolutePath ?? "/";
            if (rawPath == "/")
            {
                rawPath = "/index.html";
            }
            
            string relativePath = rawPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            string filePath = Path.Combine(StaticDirectory, relativePath);
            var response = context.Response;
            
            
            if (!File.Exists(filePath))
            {
                string notFound = "<h1>404 - Файл не найден</h1>";
                byte[] bufferq = Encoding.UTF8.GetBytes(notFound);
                response.StatusCode = 404;
                response.ContentType = "text/html; charset=utf-8";
                response.ContentLength64 = bufferq.Length;
                await response.OutputStream.WriteAsync(bufferq);
            }
            byte[] buffer = await File.ReadAllBytesAsync(filePath);
            response.StatusCode = 200;
            response.ContentType = GetContentType(filePath);
            
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer);
            
            await response.OutputStream.WriteAsync(buffer);
            response.Close();
            
            Console.WriteLine($"Обработан запрос: {context.Request.Url}");
        }
    }
    
    private static string GetContentType(string filePath)
    {
        string ext = Path.GetExtension(filePath).ToLower();
        return ext switch
        {
            ".html" => "text/html; charset=utf-8",
            ".css" => "text/css; charset=utf-8",
            ".js" => "application/javascript; charset=utf-8",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };
    }
}