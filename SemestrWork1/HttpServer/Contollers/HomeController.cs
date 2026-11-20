namespace HttpServer.Contollers;

public class HomeController
{
    public string Index()
    {
        string filePath = Path.Combine("Views", "index.html");
        return File.ReadAllText(filePath);
    }
}