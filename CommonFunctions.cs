using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ATLYSS_UiTweaks;

public static class CommonFunctions
{

    public static bool checkRoomInInventorySilent(ScriptableItem item, PlayerInventory inventory, int quantity)
    {
        switch (item._itemType)
        {
            case ItemType.GEAR:
            {
                return (inventory._usedEquipmentSlots <= inventory._maxInventoryStruct._maxEquipmentSlots);
            }
            case ItemType.CONSUMABLE:
            {
                if (inventory._usedConsumableItemSlots <= inventory._maxInventoryStruct._maxConsumableItemSlots)
                {
                    for (int i = 0; i < inventory._heldItems.Count; i++)
                    {
                        if (inventory._heldItems[i]._itemName == item._itemName && inventory._heldItems[i]._quantity + quantity <= inventory._heldItems[i]._maxQuantity)
                        {
                            return true;
                        }
                    }
                    return true;
                }

                return false;
            }
            case ItemType.TRADE:
            {
                if (inventory._usedTradeItemSlots <= inventory._maxInventoryStruct._maxTradeItemSlots)
                {
                    for (int j = 0; j < inventory._heldItems.Count; j++)
                    {
                        if (inventory._heldItems[j]._itemName == item._itemName
                            && inventory._heldItems[j]._quantity + quantity <= inventory._heldItems[j]._maxQuantity)
                        {
                            return true;
                        }
                    }
                    return true;
                }
                return false;   
            }
        }
        return false;
    }
    
    public static string getSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
    
    public static GameObject GetInactiveRootObject(string objectName)
    {
        List<GameObject> rootList = new List<GameObject>();
        SceneManager.GetActiveScene().GetRootGameObjects(rootList);
        foreach (GameObject child in rootList)
        {
            if (child.name == objectName)
            {
                return child;
            }
        }
        return null;
    }
    
    public static IEnumerator WaitforSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
    
    public static GameObject GetGameObjectChild(GameObject parentObject, string childToFind)
    {
        if(parentObject.transform.Find(childToFind) != null)
        {
            return parentObject.transform.Find(childToFind).gameObject;
        }
        else return null;
    }
    
    public static Text GetTextfromGameObject(GameObject objectToUse)
    {
        return objectToUse.GetComponent<Text>();
    }
}