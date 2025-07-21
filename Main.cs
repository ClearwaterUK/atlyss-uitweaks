using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Nessie.ATLYSS.EasySettings;

namespace ATLYSS_UiTweaks
{
    [BepInPlugin(pluginId, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string pluginId = "clearwater.atlyss.uitweaks";
        public const string pluginName = "Atlyss Toolbar Tweaks";
        public const string pluginVersion = "1.1.1";
        
        public ConfigFile ModSettingsFile  = new ConfigFile(Path.Combine("BepInEx", "config",  "clearwaterTweaks.cfg"), true);
        
        private void Awake()
        {
            Logging.Message("-- LOADING " + pluginName + "--");
            
            Harmony harmony = new Harmony(pluginId);
            harmony.PatchAll();
            
            Settings.OnInitialized.AddListener(InitModSettings);
            Settings.OnApplySettings.AddListener(() => ApplySettings());
        }
        
        public void ApplySettings()
        {
            Logging.Message("Saving mod settings");
            
            ModSettings.toggleXPToLevelDisplay = ModSettings.toggleXPToLevelDisplayToggle.Toggle.isOn;
            ModSettings.toggleEnemyHP = ModSettings.toggleEnemyHPToggle.Toggle.isOn;
            ModSettings.toggleEnemyHPCritical = ModSettings.toggleEnemyHPCriticalToggle.Toggle.isOn;
            ModSettings.toggleActionCooldown = ModSettings.toggleActionCooldownToggle.Toggle.isOn;
            ModSettings.toggleAutoLoot = ModSettings.toggleAutoLootToggle.Toggle.isOn;

            SaveToConfigFile();
        }
        
        public void SaveToConfigFile()
        {
            Config.Bind("CTT","toggleXPToLevelDisplay",true).Value = ModSettings.toggleXPToLevelDisplayToggle.Toggle.isOn;
            Config.Bind("CTT","toggleEnemyHP",true).Value = ModSettings.toggleEnemyHPToggle.Toggle.isOn;
            Config.Bind("CTT","toggleEnemyHPCritical",true).Value = ModSettings.toggleEnemyHPCriticalToggle.Toggle.isOn;
            Config.Bind("CTT","toggleActionCooldown",true).Value = ModSettings.toggleActionCooldownToggle.Toggle.isOn;
            Config.Bind("CTT","toggleAutoLoot",true).Value = ModSettings.toggleAutoLootToggle.Toggle.isOn;

            Config.Save();
        }
        
        public void InitModSettings()
        {
            ModSettings.toggleXPToLevelDisplay = Config.Bind("CTT","toggleXPToLevelDisplay",true).Value;
            ModSettings.toggleEnemyHP = Config.Bind("CTT","toggleEnemyHP",true).Value;
            ModSettings.toggleEnemyHPCritical = Config.Bind("CTT","toggleEnemyHPCritical",true).Value;
            ModSettings.toggleActionCooldown = Config.Bind("CTT","toggleActionCooldown",true).Value;
            ModSettings.toggleConsumableCooldown = Config.Bind("CTT","toggleConsumableCooldown",true).Value;
            AddTweakSettings();
        }
        
        
        public void AddTweakSettings()
        {
            SettingsTab tab = Settings.ModTab;
            
            tab.AddHeader("Clearwater's Toolbar Tweaks");
            
            ModSettings.toggleXPToLevelDisplayToggle = tab.AddToggle("Show XP to level up on toolbar",ModSettings.toggleXPToLevelDisplay);
            ModSettings.toggleEnemyHPToggle = tab.AddToggle("Enemy HP numbers",ModSettings.toggleEnemyHP);
            ModSettings.toggleEnemyHPCriticalToggle = tab.AddToggle("Enemy critical HP colors (requires Enemy HP numbers)",ModSettings.toggleEnemyHPCritical);
            ModSettings.toggleActionCooldownToggle = tab.AddToggle("Action & consumable cooldown display",ModSettings.toggleActionCooldown);
            ModSettings.toggleAutoLootToggle = tab.AddToggle("Auto-loot items (doesn't loot manually dropped items)",ModSettings.toggleAutoLoot);
        }
    }
}