using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ScriptableObjectUtility
{
    public static void CreateAsset<T>(T scObj = null ,string path = "") where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        if(scObj != null)
        {
            asset = scObj;
        }
        string completePath = "Assets/" + typeof(T).ToString() + ".asset";
        if (path != "")
        {
            completePath = "Assets/" + path + ".asset";
        }
        
        AssetDatabase.DeleteAsset(completePath);
        
        AssetDatabase.CreateAsset(asset, completePath);

        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    public static T LoadAsset<T>(string path ="") where T : ScriptableObject
    {
        string completePath = "Assets/" + typeof(T).ToString() + ".asset";
        if (path != "")
        {
            completePath = "Assets/" + path + ".asset";
        }

        T asset = AssetDatabase.LoadAssetAtPath<T>("Assets/" + path + ".asset");
        return asset;

    }
}
