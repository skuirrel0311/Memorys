using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

[CanEditMultipleObjects]
[CustomEditor(typeof(Transform))]
public class TransformInspector : Editor
{
    enum TargetType
    {
        Position,
        Rotation,
        Scale
    }

    [SerializeField]
    float Offset_X = 1;
    [SerializeField]
    float Offset_Z = 1;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var transform = target as Transform;

        DrawLine("P", TargetType.Position, transform);
        DrawLine("R", TargetType.Rotation, transform);
        DrawLine("S", TargetType.Scale, transform);

        DrawArrow();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawArrow()
    {
        var transform = target as Transform;

        Undo.RecordObjects(targets, transform.gameObject.name);
        //Xの定数移動
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("←", GUILayout.Width(50)))
            {
                transform.position -= new Vector3(Offset_X, 0, 0);
            }
            if (GUILayout.Button("→", GUILayout.Width(50)))
            {
                transform.position += new Vector3(Offset_X, 0, 0);
            }
            Offset_X = EditorGUILayout.FloatField("X:", Offset_X, GUILayout.Height(16));
        }


        //Zの定数移動
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("↓", GUILayout.Width(50)))
            {
                transform.position -= new Vector3(0, 0, Offset_Z);
            }
            if (GUILayout.Button("↑", GUILayout.Width(50)))
            {
                transform.position += new Vector3(0, 0, Offset_Z);
            }
            Offset_Z = EditorGUILayout.FloatField("Z:", Offset_Z, GUILayout.Height(16));
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Clone", GUILayout.Width(50)))
            {
                var list = new List<Object>();
                foreach (var n in Selection.gameObjects)
                {
                    var clone = GameObject.Instantiate(n);
                    Undo.RegisterCreatedObjectUndo(clone, clone.gameObject.name);
                    var parent = n.transform.parent;
                    clone.transform.SetParent(parent);
                    clone.transform.SetSiblingIndex(parent == null ? 0 : parent.transform.childCount - 1);
                    clone.transform.localPosition = n.transform.localPosition;
                    clone.transform.localRotation = n.transform.localRotation;
                    clone.transform.localScale = n.transform.localScale;
                    clone.name = n.name;
                    clone.name = GameObjectUtility.GetUniqueNameForSibling(parent, clone.name);
                    list.Add(clone);

                }
                Selection.objects = list.ToArray();
            }
        }

    }

    void DrawLine(string label, TargetType type, Transform transform)
    {
        Vector3 newValue = Vector3.zero;
        bool reset = false;

        EditorGUI.BeginChangeCheck();

        // Property
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button(label, GUILayout.Width(20)))
            {
                newValue = type == TargetType.Scale ? Vector3.one : Vector3.zero;
                reset = true;
            }
            if (!reset)
            {
                switch (type)
                {
                    case TargetType.Position:
                        newValue = EditorGUILayout.Vector3Field("", transform.position, GUILayout.Height(16));
                        break;
                    case TargetType.Rotation:
                        newValue = EditorGUILayout.Vector3Field("", transform.localEulerAngles, GUILayout.Height(16));
                        break;
                    case TargetType.Scale:
                        newValue = EditorGUILayout.Vector3Field("", transform.localScale, GUILayout.Height(16));
                        break;
                    default:
                        Debug.Assert(false, "should not reach here");
                        break;
                }
            }
        }

        // Register Undo if changed
        if (EditorGUI.EndChangeCheck() || reset)
        {
            Undo.RecordObjects(targets, string.Format("{0} {1} {2}", (reset ? "Reset" : "Change"), transform.gameObject.name, type.ToString()));
            targets.ToList().ForEach(x =>
            {
                var t = x as Transform;
                switch (type)
                {
                    case TargetType.Position:
                        t.position = newValue;
                        break;
                    case TargetType.Rotation:
                        t.localEulerAngles = newValue;
                        break;
                    case TargetType.Scale:
                        t.localScale = newValue;
                        break;
                    default:
                        Debug.Assert(false, "should not reach here");
                        break;
                }
                EditorUtility.SetDirty(x);
            });
        }

    }
}

