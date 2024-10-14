using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JAnimationSystem;
using JVehicleFrameSystem;

[CustomEditor(typeof(JAnimationData))]
public class JAnimationDataEditor : Editor
{
    Transform target1; 
    JAnimation_AutoSetBoneMatchData autoMatchData;
    bool LookCurves = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        JAnimationData _script = target as JAnimationData;

        GUILayout.Space(20);

        GUILayout.Label("Auxiliary functions");
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(17);
        LookCurves = EditorGUILayout.Toggle("LookCurves", LookCurves);
        GUILayout.EndHorizontal();
        if (LookCurves)
        {
            for (int i = 0; i < _script.dataList.Count; i++)
            {
                GUILayout.Label("No." + (i + 1).ToString() + "  [" + _script.dataList[i].boneType.ToString() + "]");

                if (_script.dataList[i].V4List_roa4.Count >= 2)
                {
                    GUILayout.Label("RoaData:");
                    AnimationCurve roa4_X = new AnimationCurve();
                    AnimationCurve roa4_Y = new AnimationCurve();
                    AnimationCurve roa4_Z = new AnimationCurve();
                    AnimationCurve roa4_W = new AnimationCurve();
                    SetCurves_4(_script.dataList[i].V4List_roa4, _script.dataList[i].timeList_roa,
                        roa4_X, roa4_Y, roa4_Z, roa4_W);
                    EditorGUILayout.CurveField(roa4_X);
                    EditorGUILayout.CurveField(roa4_Y);
                    EditorGUILayout.CurveField(roa4_Z);
                    EditorGUILayout.CurveField(roa4_W);
                } 
                else
                {
                    GUILayout.Label("No Roa Data !");
                }
                GUILayout.Space(10);
                if (_script.dataList[i].V3List_pos.Count >= 2)
                {
                    GUILayout.Label("PosData:");
                    AnimationCurve roa4_X = new AnimationCurve();
                    AnimationCurve roa4_Y = new AnimationCurve();
                    AnimationCurve roa4_Z = new AnimationCurve();
                    SetCurves_3(_script.dataList[i].V3List_pos, _script.dataList[i].timeList_pos,
                        roa4_X, roa4_Y, roa4_Z);
                    EditorGUILayout.CurveField(roa4_X);
                    EditorGUILayout.CurveField(roa4_Y);
                    EditorGUILayout.CurveField(roa4_Z);
                }
                else
                {
                    GUILayout.Label("No Pos Data !");
                }
                GUILayout.Space(25);
            }


        }
        
        GUILayout.Space(10);

        EditorGUI.indentLevel++;
        target1 = EditorGUILayout.ObjectField(target1, typeof(Transform), true) as Transform;
        autoMatchData = EditorGUILayout.ObjectField(autoMatchData, typeof(JAnimation_AutoSetBoneMatchData), false) as JAnimation_AutoSetBoneMatchData;
        EditorGUI.indentLevel--;
        GUILayout.BeginHorizontal();
        GUILayout.Space(17);
        if (GUILayout.Button("GetTPoseData"))
        {
            if (target1 == null) Debug.LogWarning("target1 == null !!");
            if (autoMatchData == null) Debug.LogWarning("autoMatchData == null !!");
            if (target1 != null && autoMatchData != null)
            {
                GetTPoseData(target1, _script, autoMatchData);
            }
           
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(17);
        GUILayout.Label("请尽量把角色正面朝向Z轴正向");
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(17);
        if (GUILayout.Button("SetToTPoseData"))
        {
            if (target1 == null) Debug.LogWarning("target1 == null !!");
            if (autoMatchData == null) Debug.LogWarning("autoMatchData == null !!");
            if (target1 != null && autoMatchData != null)
            {
                SetToTPoseData(target1, _script.tPoseData, autoMatchData);
            }

        }
        GUILayout.EndHorizontal();


        if (GUILayout.Button("GetBoneFramework"))
        {
            if (target1 != null && autoMatchData != null )
            {
                BoneFramework boneFramework1 = new BoneFramework();
                BoneFramework.ReadBoneFrame(target1.gameObject, autoMatchData, out boneFramework1, 1f);
                _script.boneFramework = boneFramework1;
                Save(target as JAnimationData);
                Debug.Log("GetBoneFramework And Saved.");
            }
        }
        if (GUILayout.Button("交换位置数据的y和z，并使交换后的z相反"))
        {
            for (int i = 0; i < _script.dataList.Count; i++)
            {
                if (_script.dataList[i].V3List_pos.Count > 0)
                {
                    for (int j = 0; j < _script.dataList[i].V3List_pos.Count; j++)
                    {
                        float y1 = _script.dataList[i].V3List_pos[j].y;
                        float z1 = _script.dataList[i].V3List_pos[j].z;
                        _script.dataList[i].V3List_pos[j] = new Vector3(_script.dataList[i].V3List_pos[j].x, z1, -y1);
                    }
                }
            }
            Debug.Log("complete.");
        }
        GUILayout.Space(60);
        if(GUILayout.Button("Save"))
        {
            Save(target as JAnimationData);
        }

   
    }

    public static void GetTPoseData(Transform target1, JAnimationData data1, JAnimation_AutoSetBoneMatchData autoMatchData)
    {
        Debug.Log("请尽量把角色正面朝向Z轴正向！");
        BoneMatch boneMatch1 = new BoneMatch();
        boneMatch1.StartAutoMatch(target1, autoMatchData);
        for(int i=0; i< boneMatch1.boneMatchList.Count; i++)
        {
            if(boneMatch1.boneMatchList[i].matchedT != null)
            {
                if(data1.tPoseData.GetDataByType(boneMatch1.boneMatchList[i].boneType) == null)
                {
                    data1.tPoseData.dataList.Add(new JAnimationData.TPoseData.TPoseOneData());
                    int number = data1.tPoseData.dataList.Count - 1;
                    data1.tPoseData.dataList[number].boneType = boneMatch1.boneMatchList[i].boneType;
                    data1.tPoseData.dataList[number].roa_Local = boneMatch1.boneMatchList[i].matchedT.localRotation;
                    data1.tPoseData.dataList[number].roa_World = boneMatch1.boneMatchList[i].matchedT.rotation;
                    data1.tPoseData.dataList[number].pos_World = boneMatch1.boneMatchList[i].matchedT.position;
                }
                else
                {
                    data1.tPoseData.GetDataByType(boneMatch1.boneMatchList[i].boneType).roa_Local = boneMatch1.boneMatchList[i].matchedT.localRotation;
                    data1.tPoseData.GetDataByType(boneMatch1.boneMatchList[i].boneType).roa_World = boneMatch1.boneMatchList[i].matchedT.rotation; 
                }
            }
            else
            {
                Debug.Log("boneMatch1.boneMatchList[" + i + "].matchedT("+ boneMatch1.boneMatchList[i].boneType .ToString()+ ") == null !!!");
                continue;
            }
        }
        Save(data1);
        Debug.Log("Done and saved");

    }
    private void SetToTPoseData(Transform target1, JAnimationData.TPoseData data1, JAnimation_AutoSetBoneMatchData autoMatchData)
    {
        BoneMatch boneMatch1 = new BoneMatch();
        boneMatch1.StartAutoMatch(target1, autoMatchData);
        if (data1.dataList.Count == 0)
        {
            return;
        }

        for (int i = 0; i < boneMatch1.boneMatchList.Count; i++)
        {
            if (boneMatch1.boneMatchList[i].matchedT != null)
            {
                if (data1.GetDataByType(boneMatch1.boneMatchList[i].boneType) == null)
                {

                }
                else
                {
                    boneMatch1.boneMatchList[i].matchedT.localRotation =
                        data1.GetDataByType(boneMatch1.boneMatchList[i].boneType).roa_Local;
                }
            }
            else
            {
                Debug.Log("boneMatch1.boneMatchList[" + i + "].matchedT(" + boneMatch1.boneMatchList[i].boneType.ToString() + ") == null !!!");
                continue;
            }
        }
        Debug.Log("Done");

    }

    public static void Save(JAnimationData data)
    {
        List<string> paths = new List<string>();
        paths.Add(AssetDatabase.GetAssetPath(data));
        AssetDatabase.ForceReserializeAssets(paths);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void SetCurves_4(List<Vector4>dataList,List<float> timeList,
        AnimationCurve Curve_X, 
        AnimationCurve Curve_Y, 
        AnimationCurve Curve_Z, 
        AnimationCurve Curve_W)
    {
        for(int i=0; i< dataList.Count; i++)
        {
            Curve_X.AddKey(timeList[i], dataList[i].x);
            Curve_Y.AddKey(timeList[i], dataList[i].y);
            Curve_Z.AddKey(timeList[i], dataList[i].z);
            Curve_W.AddKey(timeList[i], dataList[i].w);
        }
    }

    private void SetCurves_3(List<Vector3> dataList, List<float> timeList,
         AnimationCurve Curve_X,
         AnimationCurve Curve_Y,
         AnimationCurve Curve_Z)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            Curve_X.AddKey(timeList[i], dataList[i].x);
            Curve_Y.AddKey(timeList[i], dataList[i].y);
            Curve_Z.AddKey(timeList[i], dataList[i].z);
        }
    }
}
