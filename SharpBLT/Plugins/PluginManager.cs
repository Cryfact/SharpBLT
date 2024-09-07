namespace SharpBLT.Plugins;

using System;
using System.Collections.Generic;

public static class PluginManager
{
    private static readonly List<Plugin> PluginsList = [];

    public static PluginLoadResult LoadPlugin(string file, out Plugin? plugin)
    {
        plugin = null;

        foreach (var existingPlugin in PluginsList)
        {
            if (file == existingPlugin.File)
            {
                plugin = existingPlugin;
                return PluginLoadResult.AlreadyLoaded;
            }
        }

        Logger.Instance().Log(LogType.Log, $"Loading binary extension {file}");

        try
        {
            plugin = CreateNativePlugin(file);
            PluginsList.Add(plugin);
            Lua.RegisterPluginForActiveStates(plugin);
            return PluginLoadResult.Success;
        }
        catch (Exception ex)
        {
            Logger.Instance().Log(LogType.Error, $"Plugin loading failed: {ex.Message}");
            throw;
        }
    }

    public static IReadOnlyList<Plugin> GetPlugins() => PluginsList.AsReadOnly();

    private static WindowsPlugin CreateNativePlugin(string file)
    {
        return new WindowsPlugin(file);
    }
}
