using System.Reflection;

namespace CuSharp.Kernel;

public class KernelDiscovery
{
    private readonly Dictionary<string, MethodInfo> _markedMethods = new();

    public void ScanAll()
    {
        var markedMethods = Assembly.GetCallingAssembly()
            .GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(m => Attribute.IsDefined(m, typeof(KernelAttribute)));

        foreach (var m in markedMethods)
        {
            var key = $"{m.DeclaringType.Namespace}.{m.DeclaringType.Name}.{m.Name}";
            _markedMethods[key] = m;
        }
    }

    public void ScanAssembly(Assembly assembly)
    {
        var markedMethods = assembly
            .GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(m => Attribute.IsDefined(m, typeof(KernelAttribute)));
        foreach (var m in markedMethods)
        {
            var key = $"{m.DeclaringType.Namespace}.{m.DeclaringType.Name}.{m.Name}";
            _markedMethods[key] = m;
        }

    }

    public bool IsMarked(string name)
    {
        return _markedMethods.ContainsKey(name);
    }

    public MethodInfo GetMethod(string name)
    {
        return _markedMethods[name];
    }

    public IEnumerable<MethodInfo> GetAllMethods()
    {
        return _markedMethods.Values;
    }
}