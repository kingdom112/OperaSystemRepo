using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JAnimationSystem;
using JVehicleFrameSystem;



[CustomEditor(typeof(BoneTPoseMarker))]
public class BoneTPoseMarkerEditor : Editor
{
    Transform target1;
    JAnimation_AutoSetBoneMatchData autoMatchData;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BoneTPoseMarker _script = target as BoneTPoseMarker;

        GUILayout.Space(20);


        target1 = EditorGUILayout.ObjectField(target1, typeof(Transform), true) as Transform;
        autoMatchData = EditorGUILayout.ObjectField(autoMatchData, typeof(JAnimation_AutoSetBoneMatchData), false) as JAnimation_AutoSetBoneMatchData;
        if (GUILayout.Button("GetTPoseData"))
        {
            if (target1 == null) Debug.LogWarning("target1 == null !!");
            if (autoMatchData == null) Debug.LogWarning("autoMatchData == null !!");
            if (target1 != null && autoMatchData != null)
            {
                GetTPoseData(target1, _script.tPoseData, autoMatchData);
            }

        }
        GUILayout.Label("请尽量把角色正面朝向Z轴正向");
        GUILayout.Space(20);
        if (GUILayout.Button("SetToTPoseData"))
        {
            if (target1 == null) Debug.LogWarning("target1 == null !!");
            if (autoMatchData == null) Debug.LogWarning("autoMatchData == null !!");
            if (target1 != null && autoMatchData != null)
            {
                SetToTPoseData(target1, _script.tPoseData, autoMatchData);
            }

        }
        GUILayout.Space(20);
        if (GUILayout.Button("GetBoneFramework"))
        {
            if (target1 != null && autoMatchData != null)
            {
                BoneFramework boneFramework1 = new BoneFramework();
                BoneFramework.ReadBoneFrame(target1.gameObject, autoMatchData, out boneFramework1, 1f);
                _script.boneFramework = boneFramework1; 
                Debug.Log("GetBoneFramework.");
            }
        }
    }

    private void GetTPoseData(Transform target1, JAnimationData.TPoseData data1, JAnimation_AutoSetBoneMatchData autoMatchData)
    {
        Debug.Log("请尽量把角色正面朝向Z轴正向！");
        BoneMatch boneMatch1 = new BoneMatch();
        boneMatch1.StartAutoMatch(target1, autoMatchData);
        for (int i = 0; i < boneMatch1.boneMatchList.Count; i++)
        {
            if (boneMatch1.boneMatchList[i].matchedT != null)
            {
                if (data1.GetDataByType(boneMatch1.boneMatchList[i].boneType) == null)
                {
                    data1.dataList.Add(new JAnimationData.TPoseData.TPoseOneData());
                    int number = data1.dataList.Count - 1;
                    data1.dataList[number].boneType = boneMatch1.boneMatchList[i].boneType;
                    data1.dataList[number].roa_Local = boneMatch1.boneMatchList[i].matchedT.localRotation;
                    data1.dataList[number].roa_World = boneMatch1.boneMatchList[i].matchedT.rotation;
                    data1.dataList[number].pos_World = boneMatch1.boneMatchList[i].matchedT.position;
                }
                else
                {
                    data1.GetDataByType(boneMatch1.boneMatchList[i].boneType).roa_Local = boneMatch1.boneMatchList[i].matchedT.localRotation;
                    data1.GetDataByType(boneMatch1.boneMatchList[i].boneType).roa_World = boneMatch1.boneMatchList[i].matchedT.rotation;
                }
            }
            else
            {
                Debug.Log("boneMatch1.boneMatchList[" + i + "].matchedT(" + boneMatch1.boneMatchList[i].boneType.ToString() + ") == null !!!");
                continue;
            }
        } 
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
                        data1.GetDataByType(boneMatch1.boneMatchList[i].boneType).roa_Local ; 
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

}
