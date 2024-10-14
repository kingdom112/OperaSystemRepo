using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JAnimationSystem;

[CustomEditor(typeof(JAnimation_AutoSetBoneMatchData))]
public class JAnimation_AutoSetBoneMatchDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(60);
        if (GUILayout.Button("Save"))
        {
            List<string> paths = new List<string>();
            paths.Add(AssetDatabase.GetAssetPath(target as JAnimation_AutoSetBoneMatchData));
            AssetDatabase.ForceReserializeAssets(paths);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("Sort"))
        {
            JAnimation_AutoSetBoneMatchData _script = target as JAnimation_AutoSetBoneMatchData;
            _script.SortDataList();
        }
        if (GUILayout.Button("CompleteTheLowerAndUpperKeyWords"))
        {
            JAnimation_AutoSetBoneMatchData _script = target as JAnimation_AutoSetBoneMatchData;
            _script.CompleteTheLowerAndUpperKeyWords();
        }
        if (GUILayout.Button("CompleteTheLowerAndUpperKeyWords_TypeName"))
        {
            JAnimation_AutoSetBoneMatchData _script = target as JAnimation_AutoSetBoneMatchData;
            _script.CompleteTheLowerAndUpperKeyWords_TypeName();
        }
        GUILayout.Space(50);
        if (GUILayout.Button("添加手指项目"))
        {
            JAnimation_AutoSetBoneMatchData _script = target as JAnimation_AutoSetBoneMatchData;
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lThumb1));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lThumb2));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lThumb3));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lIndex1));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lIndex2));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lIndex3));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lMid1));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lMid2));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lMid3));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lRing1));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lRing2));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lRing3)); 
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lPinky1));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lPinky2));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lPinky3));

            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rThumb1));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rThumb2));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rThumb3));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rIndex1));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rIndex2));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rIndex3));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rMid1));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rMid2));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rMid3));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rRing1));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rRing2));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rRing3));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rPinky1));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rPinky2));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rPinky3));

            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lCarpal1));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lCarpal2));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lCarpal3));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.lCarpal4));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rCarpal1));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rCarpal2));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rCarpal3));
            _script.DataList.Add(new JAnimation_AutoSetBoneMatchData.OneData(JAnimationData.BoneType.rCarpal4));
        }
    }

}
