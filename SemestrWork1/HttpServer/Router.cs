using System.Reflection;

namespace HttpServer;

public class Router
{
    public string? Route(string urlPath)
    {
        string[] parts = urlPath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length > 2)
        {
            return null;
        }
        string controllerName = parts.Length > 0 ? parts[0] : "home";
        string methodName = parts.Length > 1 ? parts[1] : "index";
        
        string fullControllerName = char.ToUpper(controllerName[0]) + controllerName.Substring(1).ToLower() + "Controller";
        string formattedMethodName = char.ToUpper(methodName[0]) + methodName.Substring(1).ToLower();

        Type? controllerType = Assembly.GetExecutingAssembly()
            .GetTypes()
            .FirstOrDefault(t => t.Name == fullControllerName);
        if (controllerType == null)
        {
            return null;
        }
        
        MethodInfo? method = controllerType.GetMethod(formattedMethodName);
        if (method == null)
        {
            return null;
        }
        
        object? controllerInstance = Activator.CreateInstance(controllerType);
        object? result = method.Invoke(controllerInstance, null);

        return result?.ToString();
    }
}