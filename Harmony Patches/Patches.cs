﻿
using System;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

using static ATLYSS_UiTweaks.CommonFunctions;

namespace ATLYSS_UiTweaks.Harmony_Patches
{
    [HarmonyPatch(typeof(Player),"Start")]
    public static class HandleNewPlayerConnectionPatch
    {
        [HarmonyPostfix]
        public static async void showNewPlayerConnection(Player __instance)
        {
            if(ModSettings.toggleCharMemory)
            {
                await Task.Delay(5000); //Wait 5 seconds so the game has time to send all the player data to our client
                if(!__instance.isLocalPlayer)
                {
                    string steamID = __instance._steamID;
                    if(steamID.Length > 0 && UsernamePersistence.PersistentUsernames.ContainsKey(steamID))
                    {
                        if(UsernamePersistence.PersistentUsernames.TryGetValue(steamID, out string previousName)
                           && __instance._nickname != previousName)
                        {
                            {
                                ChatBehaviour._current.New_ChatMessage(__instance._nickname + " connected (Previous character was " + previousName + ")"); 
                            }
                        }
                    }
                    else
                    {
                        UsernamePersistence.PersistentUsernames.Add(steamID,__instance._nickname);
                    }
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(QuickItemSlot),"Iterate_ItemHandleInfo")]
    public static class DisplayConsumableCoolDownInSecondsPatch
    {
        [HarmonyPostfix]
        public static void patchShowSecondsCooldown(ref QuickItemSlot __instance, ref PlayerInventory ____pInventory)
        {
                GameObject itemObject = __instance.gameObject;
            
                GameObject cooldownText = GetGameObjectChild(itemObject,"CooldownText");
                if(cooldownText != null)
                {
                    for (int i = 0; i < ____pInventory._consumableBuffers.Count; i++)
                    {
                        if(____pInventory._consumableBuffers[i]._scriptableConsumable._itemName == __instance._setItemName)
                        {
                            float currentCooldown = ____pInventory._consumableBuffers[i]._bufferTimer;
                            if(currentCooldown > 0f && ModSettings.toggleConsumableCooldown)
                            {
                                cooldownText.GetComponent<Text>().text = currentCooldown.ToString("F1");
                                cooldownText.SetActive(true);
                            }
                            else
                            {
                                cooldownText.SetActive(false);
                            }
                        }
                    }
                }
                else
                {
                    GameObject cooldownObj = GameObject.Instantiate(GetGameObjectChild(itemObject,"_quickItemSlot_quantityCounter"),itemObject.transform);
                    cooldownObj.transform.localPosition = new Vector3(10f,0f,0f);
                    cooldownObj.GetComponent<Text>().color = Color.white;
                    cooldownObj.name = "CooldownText";
                    cooldownObj.SetActive(false);
                }
        }
    }
    
    [HarmonyPatch(typeof(StatusEntityGUI),"Update")]
    public static class DisplayEnemyHPPatch
    {
        [HarmonyPostfix]
        public static void displayEnemyHPInBar(ref StatusEntityGUI __instance, ref Creep ____isCreep)
        {
            if(ModSettings.toggleEnemyHP)
            {
                if(____isCreep != null)
                {
                    int maxHealth = ____isCreep._statStruct._maxHealth;
                    int currentHealth = ____isCreep._statusEntity._currentHealth;
                
                    bool canBeExecuted = currentHealth < maxHealth*0.30f;
                    string newText = "";
                    if(ModSettings.toggleEnemyHPCritical)
                    {
                        newText = 
                            (canBeExecuted ? "<color=orange>" : "")
                            + ____isCreep._scriptCreep._creepName
                            + (canBeExecuted ? "</color>" : "")
                            + " (" + 
                            (canBeExecuted ? "<color=orange>" : "")
                            + currentHealth
                            + (canBeExecuted ? "</color>" : "")
                            + "/" + maxHealth + ")";
                    }
                    else
                    {
                        newText = 
                            ____isCreep._scriptCreep._creepName
                            + " (" + 
                            + currentHealth
                            + "/" + maxHealth + ")";
                    }

                    ____isCreep._creepDisplayName = newText;
                } 
            }
            
        }
    }
    
    [HarmonyPatch(typeof(InGameUI),"Update")]
    public static class DisplayXPToNextLevelPatch
    {
        [HarmonyPostfix]
        public static void patchExpBar(ref InGameUI __instance, PlayerStats ____pStats, ref Text ____text_experienceCounter)
        {
            if(ModSettings.toggleXPToLevelDisplay)
            {
                StatsMenuCell smc = Object.FindObjectOfType<StatsMenuCell>();
                if(____pStats != null && smc != null && ____text_experienceCounter.text != "MAX")
                {
                    int currentExp = ____pStats._currentExp;
                    float requiredForNextLevel = smc._mainPlayer._pStats._statStruct._experience;
                
                    float percentage = currentExp / requiredForNextLevel*100f;
                    string str = string.Format("{0} ({1}%) | {2} to next level", currentExp, percentage.ToString("F2"),((requiredForNextLevel-currentExp).ToString()));
                    
                    ____text_experienceCounter.text = str;
                    ____text_experienceCounter.horizontalOverflow = HorizontalWrapMode.Overflow;
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(ActionBarManager),"LateUpdate")]
    public static class DisplaySkillCooldownInSecondsPatch
    {
        [HarmonyPostfix]
        public static void patchShowSecondsCooldown(ref ActionBarManager __instance, ref ActionSlot[] ____actionSlots, PlayerCasting ____pCast)
        {
                if(____actionSlots != null && ____pCast != null && __instance != null)
                {
                    for(int x = 0; x < 6; x++)
                    {
                        if(____actionSlots[x] != null)
                        {
                            GameObject actionObject = ____actionSlots[x].gameObject;
                            if(actionObject != null)
                            {
                                GameObject CooldownText = GetGameObjectChild(actionObject,"CooldownText");
                                if(CooldownText != null)
                                {
                                    if(____pCast._skillCoolDowns[x] != null && ____pCast._skillCoolDowns[x] > 0f && ModSettings.toggleActionCooldown)
                                    {
                                        CooldownText.SetActive(true);
                                        CooldownText.GetComponent<Text>().text = ____pCast._skillCoolDowns[x].ToString("F1");
                                        if (____pCast._skillCoolDowns[x] < 10f && CooldownText.transform.localPosition.x == 40f)
                                        {
                                            CooldownText.transform.localPosition = new Vector3(45f, 0f, 0f);
                                        }
                                    }
                                    else
                                    {
                                        CooldownText.transform.localPosition = new Vector3(40f,0f,0f);
                                        CooldownText.SetActive(false);
                                    }
                                }
                                else
                                {
                                    GameObject cooldownObject = GameObject.Instantiate(GetGameObjectChild(actionObject,"_actionSlot_hotKey"),actionObject.transform);
                                    cooldownObject.name = "CooldownText";
                                    cooldownObject.transform.localPosition = new Vector3(40f,0f,0f);
                                }
                            }
                        }
                    }
            }
        }
    }
}