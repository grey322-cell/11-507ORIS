using HttpServer.Models;
namespace HttpServer.Contollers;

public class HomeController
{
    public string Index()
    {
        string filePath = Path.Combine("Views", "index.html");
        return File.ReadAllText(filePath);
    }
    
    public string Card()
    {
        // Симуляция работы бд
        var testCard = new Card
        {
            Id = 1,
            Name = "Дарт Вейдер",
            Rarity = "Легендарная",
            Description = "Люк, он твой отец",
            ImageUrl = "/images/google.png" // заглушка
        };
        
        string html = File.ReadAllText(Path.Combine("Views", "cardTest.html"));
        var templateEngine = new TemplateEngine();
        return templateEngine.Render(html, testCard);
    }

    public string Catalog()
    {
        var testCard = new Card
        {
            Id = 1,
            Name = "Дарт Вейдер",
            Rarity = "Легендарная",
            Description = "Люк, он твой отец",
            ImageUrl = "/images/google.png" // заглушка
        };
        var testCard1 = new Card
                {
                    Id = 2,
                    Name = "Дарт Мол",
                    Rarity = "Мифическа",
                    Description = "Лысик",
                    ImageUrl = "/images/reddit.png" // заглушка
                };
        var testCard2 = new Card
        {
            Id = 3,
            Name = "Штурмовик",
            Rarity = "Обычная",
            Description = "My lord!",
            ImageUrl = "/images/google.png" // заглушка
        };

        List<Card> cards = new List<Card>() { testCard, testCard1, testCard2 };
        string html = File.ReadAllText(Path.Combine("Views", "catalog.html"));
        var templateEngine = new TemplateEngine();
        return templateEngine.RenderList(html, "Cards", cards);
    }
}