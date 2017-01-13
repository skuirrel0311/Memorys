using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


public class CreateScriptableObject : MonoBehaviour
{
    [MenuItem("Assets/Create/ClearRankData")]
    public static void CreateAsset()
    {
        CreateAsset<ClearRankData>();
    }

    public static void CreateAsset<Type>() where Type : ScriptableObject
    {
        Type item = ScriptableObject.CreateInstance<Type>();

        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Programmer/Data/" + typeof(Type) + ".asset");

        AssetDatabase.CreateAsset(item, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = item;
    }
}
