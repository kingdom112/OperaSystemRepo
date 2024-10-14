using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 显示重心点的组件
/// </summary>
public class AnimInfoWindow_WeighCenterShower : MonoBehaviour
{

    public AnimInfoWindowControl control;
    public GameObject weightCenterGO; 


    void Start()
    {
        weightCenterGO.transform.SetParent(null);
        weightCenterGO.transform.localScale = Vector3.one * 0.1f;
    }

    void Update()
    {
        if (control == null) return;
        if (control.vfPlayer == null) return;
        if(control.isMaster)
        { 
            weightCenterGO.transform.position = GetWeightCenter(control.vfPlayer.ch_master.bm_p1); 
        }
        else
        { 
            weightCenterGO.transform.position = GetWeightCenter(control.vfPlayer.ch_student.bm_p1);
        }
    }

    private void OnDestroy()
    {
        Destroy(weightCenterGO);
    } 

    private Vector3 GetWeightCenter(JAnimationSystem.BoneMatch_Player _bonePlayer)
    {
        Vector3 posAll = Vector3.zero;
        int boneCount = 0;

        foreach(var p1 in _bonePlayer.bonePlayers)
        {
            if(p1.boneOneMatch.matchedT != null)
            {
                posAll += p1.boneOneMatch.matchedT.position;
                boneCount++;
            }
        }
        //GetAllBonePosAdd(_boneRoot, ref posAll, ref boneCount);
        posAll = posAll / boneCount;
        posAll.y = 0;
        return posAll;
    }

    /*
    /// <summary>
    /// 获取所有骨骼的位置加和
    /// </summary>
    void GetAllBonePosAdd(Transform bone, ref Vector3 posAll, ref int boneCount)
    {
        if(bone.GetComponent<SphereCollider>() != null)
        {
            posAll += bone.position;
            boneCount++;
        } 
        for (int i=0; i<bone.childCount; i++)
        {
            GetAllBonePosAdd(bone.GetChild(i), ref posAll, ref boneCount);
        }
    }
    */
}
