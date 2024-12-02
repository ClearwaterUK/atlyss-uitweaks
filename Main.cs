using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using PluginInfo = BepInEx.PluginInfo;

namespace ATLYSS_UiTweaks
{
    [BepInPlugin(pluginId, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string pluginId = "clearwater.atlyss.uitweaks";
        public const string pluginName = "Atlyss UI Tweaks";
        public const string pluginVersion = "1.0.0";
        
        //This method is called when your mod is first loaded. Use this to handle any startup & initialisation logic.
        private void Awake()
        {
            Logging.Warn("-- LOADING " + pluginName + "--");
            
            Harmony harmony = new Harmony(pluginId);
            harmony.PatchAll();

        }
        
        //This method is called whenever the game switches scenes (levels). Use this to handle any mod logic that should be updated when a scene is switched.
        public void onSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            
        }
    }

}