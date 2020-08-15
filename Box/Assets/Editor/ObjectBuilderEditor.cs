using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(EditorSpawn))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorSpawn myScript = (EditorSpawn)target;
        if (GUILayout.Button("创建对象"))
        {
            myScript.Respawn();
        }
    }
}