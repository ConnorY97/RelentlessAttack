using System.Collections;

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor((typeof(CellsDebugger)))]
public class CellDebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (CellsDebugger)target;

        if (GUILayout.Button("Generate Grid"))
        {
            script.init();
        }
    }
}
#endif