using System.Net;
using System.Text;

namespace HttpServer;

public class HttpServer
{
    private readonly HttpListener _listener; 
    private readonly string _url; 
    private readonly string _staticDirectory; 
    private readonly Router _router = new Router();
    
    public HttpServer(string url, string staticDirectory)
    {
        _url = url;
        _listener = new HttpListener();
        _listener.Prefixes.Add(_url); 
        _staticDirectory = staticDirectory;
    }
    
    public async Task StartAsync()
    {
        _listener.Start(); 
        Console.WriteLine($"Сервер запущен и ждёт гостей по адресу: {_url}");
        
        while (_listener.IsListening)
        {
            
            var context = await _listener.GetContextAsync();
            var request = context.Request;
            string rawPath = request.Url?.AbsolutePath ?? "/";
            var response = context.Response;
            
            if (!Path.HasExtension(rawPath))
            {
                string? html = _router.Route(rawPath);

                if (html != null)
                {
                    byte[] buffer1 = Encoding.UTF8.GetBytes(html);
                    response.StatusCode = 200;
                    response.ContentType = "text/html; charset=utf-8";
                    response.ContentLength64 = buffer1.Length;
                    await response.OutputStream.WriteAsync(buffer1);
                }
                else
                {
                    string notFound = "<h1>404 - Страница не найдена</h1>";
                    byte[] buffer1 = Encoding.UTF8.GetBytes(notFound);
                    response.StatusCode = 404;
                    response.ContentType = "text/html; charset=utf-8";
                    response.ContentLength64 = buffer1.Length;
                    await response.OutputStream.WriteAsync(buffer1);
                }

                response.Close();
                continue;
            }
            
            string relativePath = rawPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            string filePath = Path.Combine(_staticDirectory, relativePath);
            
            if (!File.Exists(filePath))
            {
                string notFound = "<h1>404 - Файл не найден</h1>";
                byte[] bufferq = Encoding.UTF8.GetBytes(notFound);
                response.StatusCode = 404;
                response.ContentType = "text/html; charset=utf-8";
                response.ContentLength64 = bufferq.Length;
                await response.OutputStream.WriteAsync(bufferq);
                response.Close();
                continue;
            }
            byte[] buffer = await File.ReadAllBytesAsync(filePath);
            response.StatusCode = 200;
            response.ContentType = GetContentType(filePath);
            response.ContentLength64 = buffer.Length;
            
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