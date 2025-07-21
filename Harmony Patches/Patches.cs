using System.Collections.Generic;
using System.Linq;
using ATLYSS_UiTweaks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

using static ATLYSS_UiTweaks.CommonFunctions;

namespace ATLYSS_UiTweaks.Harmony_Patches
{
    [HarmonyPatch(typeof(ItemObject), "Update")]
    public static class AutoLootPatch
    {
        //Auto-loot any items that's locally tied to the player (instanced loot).
        [HarmonyPostfix]
        public static void autoLoot(ref ItemObject __instance, Net_ItemObject ____netItemObj)
        {
            if (__instance._canPickUp && __instance._local_taggedID != null && __instance._currencyDropAmount == 0 &&
                !____netItemObj && ModSettings.toggleAutoLoot)
            {
                //Make sure the item is close enough to the player
                float distance = Vector3.Distance(__instance.gameObject.transform.position,
                    __instance._local_taggedID.gameObject.transform.position);
                
                if (distance < 20f)
                {
                    //Check if player has room in inv
                    if(checkRoomInInventorySilent(__instance._foundItem, Player._mainPlayer._pInventory,__instance._local_itemData._quantity))
                    {
                        __instance.Init_PickupItem(Player._mainPlayer.netIdentity);
                    }
                }
            }
        }
    }
    
    
    [HarmonyPatch(typeof(Player),"Player_OnDeath")]
    public static class resetCooldownOnDeathPatch
    {
        [HarmonyPostfix]
        //Reset all timers if the player dies.
        public static void resetSkillCooldownOnDeath(ref Player __instance)
        {
            if (__instance.isLocalPlayer)
            {
                for (int j = 0; j < ActionBarManager._current._actionSlots.Length; j++)
                {
                    GameObject actionObject = ActionBarManager._current._actionSlots[j].gameObject;
                    GameObject CooldownText = GetGameObjectChild(actionObject, "CooldownText");
                    if (CooldownText != null)
                    {
                        CooldownText.GetComponent<Text>().text = "0.0";
                        CooldownText.SetActive(false);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(QuickItemSlot),"Update")]
    public static class DisplayConsumableCoolDownInSecondsPatch
    {
        [HarmonyPostfix]
        public static void patchShowSecondsCooldown(ref QuickItemSlot __instance)
        { 
            if (Player._mainPlayer != null)
            {
                GameObject itemObject = __instance.gameObject;
                PlayerInventory pInv = Player._mainPlayer._pInventory;
                GameObject cooldownText = GetGameObjectChild(itemObject,"CooldownText");
                if(itemObject != null && pInv != null && cooldownText != null)
                {
                    for (int i = 0; i < pInv._consumableBuffers.Count; i++)
                    {
                        if(pInv._consumableBuffers[i]._scriptableConsumable._itemName == __instance._setItemName)
                        {
                            float currentCooldown = pInv._consumableBuffers[i]._bufferTimer;
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
                        else
                        {
                            cooldownText.SetActive(false);
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
                    bool canBeExecuted = currentHealth < maxHealth*0.25f && !____isCreep._scriptCreep._isElite;

                    string prefix = (____isCreep._scriptStatModifier != null
                        ? ____isCreep._scriptStatModifier._modifierTag + " "
                        : "");

                    string newText = "";
                    if(ModSettings.toggleEnemyHPCritical)
                    {
                        newText = 
                            (canBeExecuted ? "<color=orange>" : "")
                            + prefix + ____isCreep._scriptCreep._creepName
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
                            prefix + ____isCreep._scriptCreep._creepName
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
            if(ModSettings.toggleXPToLevelDisplay && __instance != null && ____pStats != null && ____text_experienceCounter != null)
            {
                StatsMenuCell smc = Object.FindObjectOfType<StatsMenuCell>();
                if(smc != null && ____text_experienceCounter.text != "MAX")
                {
                    int currentExp = ____pStats._currentExp;
                    float requiredForNextLevel = smc._mainPlayer._pStats._statStruct._experience;

                    float percentage = currentExp / requiredForNextLevel*100f;
                    string str = string.Format("{0} ({1}%) | {2}xp to next level", currentExp, percentage.ToString("F2"),((requiredForNextLevel-currentExp).ToString()));

                    ____text_experienceCounter.text = str;
                    ____text_experienceCounter.horizontalOverflow = HorizontalWrapMode.Overflow;
                }
            }
        }
    }

    [HarmonyPatch(typeof(ActionBarManager), "Update")]
    public static class DisplaySkillCooldownInSecondsPatch
    {
        [HarmonyPostfix]
        public static void patchShowSecondsCooldown(ref ActionBarManager __instance, ref ActionSlot[] ____actionSlots,
            PlayerCasting ____pCast)
        {
            if (____actionSlots != null && ____pCast != null && __instance != null && ModSettings.toggleActionCooldown)
            {
                for (int i = 0; i < ____actionSlots.Length; i++)
                {
                    for (int j = 0; j < ____pCast._skillCoolDownSlots.Count; j++)
                    {
                        GameObject actionObject = ____actionSlots[i].gameObject;
                        GameObject CooldownText = GetGameObjectChild(actionObject, "CooldownText");

                        if (CooldownText != null)
                        {
                            List<ScriptableSkill> skillsOnCooldown =
                                ____pCast._skillCoolDownSlots.Select(slot => slot._scriptSkill).ToList();

                            if (skillsOnCooldown.Contains(____actionSlots[i]._scriptSkill))
                            {
                                if (____actionSlots[i]._scriptSkill == ____pCast._skillCoolDownSlots[j]._scriptSkill)
                                {
                                    Text cooldownText = CooldownText.GetComponent<Text>();
                                    cooldownText.text = ____pCast._skillCoolDownSlots[j]._currentCooldown.ToString("F1");
                                    
                                    CooldownText.transform.position = actionObject.transform.position;
                                    CooldownText.transform.localPosition = new Vector3(40f, 0f, 0f);

                                    CooldownText.SetActive(____pCast._skillCoolDownSlots[j]._currentCooldown > 0.1f);

                                    if (____pCast._skillCoolDownSlots[j]._currentCooldown < 10f && CooldownText.transform.localPosition.x == 40f)
                                    {
                                        CooldownText.transform.localPosition = new Vector3(45f, 0f, 0f);
                                    }
                                }  
                            }
                            else
                            {
                                CooldownText.SetActive(false);
                            }
                        }
                        else
                        {
                            GameObject cooldownObject = GameObject.Instantiate(
                                GetGameObjectChild(actionObject, "_actionSlot_hotKey"), actionObject.transform);
                            cooldownObject.name = "CooldownText";
                            cooldownObject.transform.position = actionObject.transform.position;
                            cooldownObject.transform.localPosition = new Vector3(40f, 0f, 0f);
                            cooldownObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
    
}