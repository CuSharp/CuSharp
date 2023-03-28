using System.Reflection;

namespace CuSharp.Kernel;

public class KernelDiscovery
{
    private readonly Dictionary<string, MethodInfo> MarkedMethods = new ();

    public void ScanAll()
    {
        var markedMethods = Assembly.GetCallingAssembly()
            .GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(m => Attribute.IsDefined(m, typeof(KernelAttribute)));

        foreach (var m in markedMethods)
        {
            var key = $"{m.DeclaringType.Namespace}.{m.DeclaringType.Name}.{m.Name}";
            MarkedMethods[key] = m;
        }
    }

    public bool IsMarked(string name)
    {
        return MarkedMethods.ContainsKey(name);
    }

    public MethodInfo GetMethod(string name)
    {
        return MarkedMethods[name];
    }

    public IEnumerable<MethodInfo> GetAllMethods()
    {
        return MarkedMethods.Values;
    }
}