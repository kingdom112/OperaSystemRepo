using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JAnimationSystem;
using JVehicleFrameSystem;


public class JAnimRecoder
{
    public JAnimationData.JAD jAD = new JAnimationData.JAD();
    float timer = 0f;
    float time1 = 0.001f;
    float timer_total = 0f;
    bool isRecoding = false;
    public bool IsRecoding
    {
        get
        {
            return isRecoding;
        }
    }
    bool hasRecoded = false;
    public bool HasRecoded
    {
        get
        {
            return hasRecoded;
        }
    }
    public BoneMatch boneMatch;

    public JAnimRecoder()
    {
        jAD = new JAnimationData.JAD();
        timer = 0f;
        timer_total = 0f;
        isRecoding = false;
        hasRecoded = false;
    }


    public void CallFixedUpdate()
    {
        if (isRecoding)
        {
            if (timer < time1)
            {
                timer += Time.fixedDeltaTime;
            }
            else
            {
                timer = 0f;
                RecodeNow();
            }
            timer_total += Time.fixedDeltaTime;
        }
    }

    public void RecodeNow()
    {
        if (isRecoding)
        {
            for (int i = 0; i < boneMatch.boneMatchList.Count; i++)
            {
                if (boneMatch.boneMatchList[i].matchedT == null) continue;

                if (boneMatch.boneMatchList[i].boneType == JAnimationData.BoneType.hips)
                {
                    jAD.AddData_pos(boneMatch.boneMatchList[i].boneType, timer_total, boneMatch.boneMatchList[i].matchedT.localPosition);
                    // Debug.Log("Hip的Roa旋转角度数据 用世界坐标系代替相对坐标系。");
                    // 由于现在BoneFramework会在每个骨骼链起始前创建一个unknowType类型的root， 所以不需要这个操作了
                    // Vector4 roa = new Vector4(boneMatch.boneMatchList[i].matchedT.rotation.x,
                    //     boneMatch.boneMatchList[i].matchedT.rotation.y,
                    //     boneMatch.boneMatchList[i].matchedT.rotation.z,
                    //     boneMatch.boneMatchList[i].matchedT.rotation.w);
                    Vector4 roa = new Vector4(boneMatch.boneMatchList[i].matchedT.localRotation.x,
                        boneMatch.boneMatchList[i].matchedT.localRotation.y,
                        boneMatch.boneMatchList[i].matchedT.localRotation.z,
                        boneMatch.boneMatchList[i].matchedT.localRotation.w);
                    jAD.AddData_roa4(boneMatch.boneMatchList[i].boneType, timer_total, roa);
                }
                else
                {
                    Vector4 roa = new Vector4(boneMatch.boneMatchList[i].matchedT.localRotation.x,
                        boneMatch.boneMatchList[i].matchedT.localRotation.y,
                        boneMatch.boneMatchList[i].matchedT.localRotation.z,
                        boneMatch.boneMatchList[i].matchedT.localRotation.w);
                    jAD.AddData_roa4(boneMatch.boneMatchList[i].boneType, timer_total, roa);
                }
            }
        }
    }

    /// <summary>
    /// 返回修正了的文件名
    /// </summary>
    /// <param name="target"></param>
    /// <param name="autoMatcher"></param>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="modelPrefabName"></param>
    /// <param name="selectRanges"></param>
    /// <returns></returns>
    public string SaveToFile(GameObject target, JAnimation_AutoSetBoneMatchData autoMatcher, string path, string fileName, string modelPrefabName)
    {
        if (jAD.dataList.Count == 0) return "";
        BoneFramework boneFramework1 = new BoneFramework();
        if (BoneFramework.ReadBoneFrame(target, autoMatcher, out boneFramework1, 1f) == false) return "";
        return JAnimDataToFile.SaveJAD(path, fileName, jAD, boneFramework1, false, modelPrefabName);
    }

    public void StopRecode()
    {
        isRecoding = false;
        hasRecoded = true;
    }
    public void StartRecode(GameObject target, JAnimation_AutoSetBoneMatchData autoMatcher, float frequency = 60f)
    {
        if (target == null) return;
        if (autoMatcher == null) return;
        if (frequency < 0f) return;

        time1 = 1f / frequency;
        timer = time1;
        timer_total = 0f;
        boneMatch = new BoneMatch();//新建了bonematchlist
        boneMatch.StartAutoMatch(target.transform, autoMatcher);

        BoneFramework boneFramework1 = new BoneFramework();
        if (BoneFramework.ReadBoneFrame(target, autoMatcher, out boneFramework1, 1f))
        {
            jAD.boneFramework = boneFramework1;
        }
        isRecoding = true;
        hasRecoded = false;
    }

}
