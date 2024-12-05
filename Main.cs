using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ATLYSS_UiTweaks
{
    [BepInPlugin(pluginId, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string pluginId = "clearwater.atlyss.uitweaks";
        public const string pluginName = "Atlyss Toolbar Tweaks";
        public const string pluginVersion = "1.0.0";
        
        private void Awake()
        {
            Logging.Warn("-- LOADING " + pluginName + "--");
            
            Harmony harmony = new Harmony(pluginId);
            harmony.PatchAll();
        }

        public void onSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            
        }
    }

}