using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Battlehub.RTEditor.RuntimeAnimation))]
public class RuntimeAnimation_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Battlehub.RTEditor.RuntimeAnimation sctipt = target as Battlehub.RTEditor.RuntimeAnimation;

        for (int i=0; i< sctipt.Clips.Count; i++)
        {
            List<Battlehub.RTEditor.RuntimeAnimationProperty> pros = (List<Battlehub.RTEditor.RuntimeAnimationProperty>)sctipt.Clips[i].Properties;
            for (int j=0; j < pros.Count; j++)
            {
                Battlehub.RTEditor.RuntimeAnimationProperty pro1 = pros[j];
                showPro(pro1); 
                if(pro1.Children.Count >0)
                {
                    foreach (Battlehub.RTEditor.RuntimeAnimationProperty p1 in pro1.Children)
                    {
                        showPro(p1);
                    }
                }
            }
            

        
        }
    }

    void showPro(Battlehub.RTEditor.RuntimeAnimationProperty pro1)
    { 
        EditorGUILayout.LabelField("AnimationPropertyName: " + pro1.AnimationPropertyName);
        EditorGUILayout.LabelField("AnimationPropertyPath: " + pro1.AnimationPropertyPath);
        EditorGUILayout.LabelField("ComponentTypeName: " + pro1.ComponentTypeName);
        EditorGUILayout.LabelField("Component: " +(pro1.Component != null? pro1.Component.ToString():"[NULL]"));
        EditorGUILayout.LabelField("ComponentDisplayName: " + pro1.ComponentDisplayName);
        EditorGUILayout.LabelField("ComponentTypeName: " + pro1.ComponentTypeName);
        EditorGUILayout.CurveField("curve: ", pro1.Curve);
        EditorGUILayout.LabelField("PropertyDisplayName: " + pro1.PropertyDisplayName);
        EditorGUILayout.LabelField("PropertyName: " + pro1.PropertyName);
        GUILayout.Space(20);
    }
}
