using BepInEx.Logging;

namespace ATLYSS_UiTweaks
{
    public static class Logging 
    {
        public static ManualLogSource PluginLogger = Logger.CreateLogSource(Main.pluginName);

        public static void Debug(string text)
        {
            PluginLogger.LogDebug(text);
        }
        
        public static void Message(string text)
        {
            PluginLogger.LogMessage(text);
        }
        
        public static void Warn(string text)
        {
            PluginLogger.LogWarning(text);
        }
        
        public static void Error(string text)
        {
            PluginLogger.LogError(text);
        }
        
        public static void Fatal(string text)
        {
            PluginLogger.LogFatal(text);
        }
        
        public static void Info(string text)
        {
            PluginLogger.LogInfo(text);
        }
    }
}