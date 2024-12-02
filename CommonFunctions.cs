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