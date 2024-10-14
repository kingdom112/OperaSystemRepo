using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(JVehicleFrame))]
public class JVehicleFrameEditor : Editor
{

    public static void Save(JVehicleFrame _data)
    {
        List<string> paths = new List<string>();
        paths.Add(AssetDatabase.GetAssetPath(_data));
        AssetDatabase.ForceReserializeAssets(paths);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("save!");
    }

    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("OpenInEditor"))
        {
            JAnimationSystem.JVehicleFrameEditWindow.OpenWindow(target as JVehicleFrame);
        }
        GUILayout.Space(60);

        base.OnInspectorGUI();
        
        GUILayout.Space(60);
        if (GUILayout.Button("Save"))
        {
            Save(target as JVehicleFrame);
        }
    }
}
