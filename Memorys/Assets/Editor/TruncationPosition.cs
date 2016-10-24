using UnityEngine;
using System.Collections;
using UnityEditor;

public static class EditorTest01
{
    //小数点を切り捨て
    [MenuItem("Tools/TruncationPosition")]
    static void TruncationPosition()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Vector3 v = obj.transform.localPosition;
            int x = (int)v.x;
            int y = (int)v.y;
            int z = (int)v.z;
            obj.transform.localPosition = new Vector3((float)x, (float)y, (float)z);
        }
    }
}
