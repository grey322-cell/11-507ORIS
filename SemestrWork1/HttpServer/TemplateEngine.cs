using System.Reflection;

namespace HttpServer;

public class TemplateEngine
{
    public string Render<T>(string html, T model)
    {
        if (model == null) return html;

        // Получаем все публичные свойства переданного объекта (например, Card)
        PropertyInfo[] properties = typeof(T).GetProperties();

        foreach (PropertyInfo prop in properties)
        {
            string placeholder = $"{{{{{prop.Name}}}}}"; // Формирует строку вида "{{Name}}"
            string value = prop.GetValue(model)?.ToString() ?? string.Empty;

            html = html.Replace(placeholder, value);
        }

        return html;
    }
    
    public string RenderList<T>(string html, string listName, List<T> items)
    {
        string startTag = $"{{{{#each {listName}}}}}"; // Формирует "{{#each Cards}}"
        string endTag = "{{/each}}";
    
        int startIndex = html.IndexOf(startTag);
        int endIndex = html.IndexOf(endTag);
    
        // Если тегов цикла в HTML нет — возвращаем шаблон без изменений
        if (startIndex == -1 || endIndex == -1) return html;
    
        // 1. Вырезаем внутренний кусок HTML между {{#each ...}} и {{/each}}
        int innerStart = startIndex + startTag.Length;
        string itemTemplate = html.Substring(innerStart, endIndex - innerStart);
    
        // 2. Проходим по списку и рендерим каждую карточку нашим первым методом Render
        var resultBuilder = new System.Text.StringBuilder();
        foreach (T item in items)
        {
            resultBuilder.Append(Render(itemTemplate, item));
        }
    
        // 3. Заменяем весь исходный блок (от {{#each}} до {{/each}}) на сгенерированный список
        string fullBlock = html.Substring(startIndex, (endIndex + endTag.Length) - startIndex);
        return html.Replace(fullBlock, resultBuilder.ToString());
    }
}