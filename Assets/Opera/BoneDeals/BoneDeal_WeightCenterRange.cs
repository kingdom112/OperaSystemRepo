using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JAnimationSystem;
using Dreamteck.Splines;
using JAnimCurves;

public class BoneDeal_WeightCenterRange : BoneDealBase
{
    public override bool needSelectBone
    {
        get
        {
            return false;
        }
    }

    public override int NeedCalculateCount()
    {
        int _needCalculateCount = 0;

        if (vfPlayer.ch_master.hasInit)
        {
            JAnimCurves.AnimOneCurve_InGame savedCurve =
                GetSavedCurve(true);
            if (savedCurve == null)
            {
                _needCalculateCount++;
            }
        }
        if (vfPlayer.ch_student.hasInit)
        {
            JAnimCurves.AnimOneCurve_InGame savedCurve =
                GetSavedCurve(false);
            if (savedCurve == null)
            {
                _needCalculateCount++;
            }
        }


        return _needCalculateCount;
    }

    private int AllCurveCount = 0;
    private int nowCurveCount = 0;
    public override IEnumerator DealCalculate()
    {
        AllCurveCount = 1;
        nowCurveCount = 1;
        Color mainColor1 = Color.white;
        List<Color> colors = new List<Color>();
        AnimationCurve curve1 = null;
        List<AnimationCurve> curves = new List<AnimationCurve>();
        float max = 0f;
        float min = 0f;

        if (vfPlayer.ch_master.hasInit)
        {
            AnimationCurve weightCenterRangeCurve1 = null;
            JAnimCurves.AnimOneCurve_InGame savedCurve =
                GetSavedCurve(true);
            if (savedCurve == null)
            {
                yield return GetWeightCenterRangeCurve(true);
                weightCenterRangeCurve1 = tempCurve;
                max = GetCurveMax(weightCenterRangeCurve1);
                min = GetCurveMin(weightCenterRangeCurve1);
            }
            else
            {
                weightCenterRangeCurve1 = savedCurve.curve;
                max = savedCurve.max;
                min = savedCurve.min;
            }
            if (weightCenterRangeCurve1 != null)
            {
                curve1 = weightCenterRangeCurve1;
                mainColor1 = mainCurveColor;
            }
            else
            {
                Debug.LogError("计算根骨高度曲线失败！！");
            }
        }
        if (vfPlayer.ch_student.hasInit)
        {
            AnimationCurve weightCenterRangeCurve1 = null;
            JAnimCurves.AnimOneCurve_InGame savedCurve =
                GetSavedCurve(false);
            if (savedCurve == null)
            {
                yield return GetWeightCenterRangeCurve(false);
                weightCenterRangeCurve1 = tempCurve;
                max = GetCurveMax(weightCenterRangeCurve1);
                min = GetCurveMin(weightCenterRangeCurve1);
            }
            else
            {
                weightCenterRangeCurve1 = savedCurve.curve;
                max = savedCurve.max;
                min = savedCurve.min;
            }
            if (weightCenterRangeCurve1 != null)
            {
                curves.Add(weightCenterRangeCurve1);
                colors.Add(addedCurvesColor);
            }
            else
            {
                Debug.LogError("计算根骨高度曲线失败！！");
            }
        }
        vfPlayer.m_progressWindow.Close();
        if (vfPlayer.curveShowWindowControl == null)
        {
            vfPlayer.CreateRTECurveWindow();//create
        }
        vfPlayer.curveShowWindowControl.SetCurve(curve1, max, min, mainColor1);
        vfPlayer.curveShowWindowControl.SetCurves(curves, colors);
        vfPlayer.curveShowWindowControl.sliderY.value = 0.5f;
    }

    private IEnumerator GetWeightCenterRangeCurve(bool ismaster)
    {
        Debug.Log("get WeightCenterRangeCurve");
        tempCurve = new AnimationCurve();
        AnimationCurve weightCenterRangeCurve1 = new AnimationCurve();
        AnimOneCurve_ForSave saveData1 = new AnimOneCurve_ForSave(animCurveType.weightCenterRange, JAnimationData.BoneType.hips);
        float fps = 30f;
        float timejiange = 1f / fps;
        BoneMatch_Player bm_p1 = null;
        BoneOnePlayer bonePlayer1 = null;
        if (ismaster)
        {
            int hipIndex1 = -1;
            if (vfPlayer.ch_master.hasInit)
            {
                hipIndex1 = vfPlayer.ch_master.bm_p1.GetPlayerIndexByBoneType(JAnimationData.BoneType.hips);
                if (hipIndex1 == -1) yield break;
                bonePlayer1 = vfPlayer.ch_master.bm_p1.bonePlayers[hipIndex1];
                bm_p1 = vfPlayer.ch_master.bm_p1;
            }
            else
            {
                yield break;
            }
        }
        else
        {
            int hipIndex1 = -1;
            if (vfPlayer.ch_student.hasInit)
            {
                hipIndex1 = vfPlayer.ch_student.bm_p1.GetPlayerIndexByBoneType(JAnimationData.BoneType.hips);
                if (hipIndex1 == -1) yield break;
                bonePlayer1 = vfPlayer.ch_student.bm_p1.bonePlayers[hipIndex1];
                bm_p1 = vfPlayer.ch_student.bm_p1;
            }
            else
            {
                yield break;
            }
        }
        if (bonePlayer1 == null || bm_p1 == null) yield break;
        int count = (int)((bonePlayer1.animDatas_Time_roa[bonePlayer1.animDatas_Time_roa.Count - 1] - bonePlayer1.animDatas_Time_roa[0]) / timejiange);
        count++;
        float timer = 0f;
        int yield_k = 0;
        for (int i = 0; i < count; i++)
        {
            bm_p1.PlayDataInThisFrame_Curve(timer);
            Vector3 weightCenter1 = GetWeightCenter(vfPlayer.ch_master.createdGA.transform); 
            bm_p1.PlayDataInThisFrame_Curve(Mathf.Clamp(timer - 0.5f, 0f, bonePlayer1.animDatas_Time_roa[bonePlayer1.animDatas_Time_roa.Count - 1]));
            Vector3 weightCenter2 = GetWeightCenter(vfPlayer.ch_master.createdGA.transform);
            bm_p1.PlayDataInThisFrame_Curve(Mathf.Clamp(timer + 0.5f, 0f, bonePlayer1.animDatas_Time_roa[bonePlayer1.animDatas_Time_roa.Count - 1]));
            Vector3 weightCenter3 = GetWeightCenter(vfPlayer.ch_master.createdGA.transform);
            float _radius = GetCircleRadius_Min(weightCenter1, weightCenter2, weightCenter3, weightCenter2); 
            weightCenterRangeCurve1.AddKey(timer, _radius);
            saveData1.AddData(_radius, timer);
            timer += timejiange;
            yield_k++;
            if (yield_k >= yieldWaitTick)
            {
                yield_k = 0;
                yield return 0;
            }
            //Debug.Log(((float)i/(float)count));
            vfPlayer.m_progressWindow.Show("当前", ((float)i / (float)count).ToString(), (float)i / (float)count, "全部", nowCurveCount + "/" + AllCurveCount, (float)nowCurveCount / (float)AllCurveCount, "计算重心范围曲线中");
        }
        AnimCurvesSaver saver01 = new AnimCurvesSaver();
        if (ismaster)
        {
            if (vfPlayer.ch_master.createdGA_No < vfPlayer.animShowList.Count)//判断是否为内置数据
            {
                string animName = vfPlayer.animShowList[vfPlayer.ch_master.createdGA_No].name;
                saver01.SaveOne(saveData1, animName, bonePlayer1.boneOneMatch.boneType);
            }
            else if (vfPlayer.ch_master.createdGA_No < vfPlayer.animShowList.Count + vfPlayer.animRecodesShowList.Count)
            {
                string animName = vfPlayer.animRecodesShowList[vfPlayer.ch_master.createdGA_No - vfPlayer.animShowList.Count].showName;
                saver01.SaveOne(saveData1, animName, bonePlayer1.boneOneMatch.boneType);
            }
            else
            {
                Debug.Log("ERROR ! Wanted to Save at a Wrong Data!");
                yield break;
            }
        }
        else
        {
            if (vfPlayer.ch_student.createdGA_No < vfPlayer.animShowList.Count)//判断是否为内置数据
            {
                string animName = vfPlayer.animShowList[vfPlayer.ch_student.createdGA_No].name;
                saver01.SaveOne(saveData1, animName, bonePlayer1.boneOneMatch.boneType);
            }
            else if (vfPlayer.ch_master.createdGA_No < vfPlayer.animShowList.Count + vfPlayer.animRecodesShowList.Count)
            {
                string animName = vfPlayer.animRecodesShowList[vfPlayer.ch_student.createdGA_No - vfPlayer.animShowList.Count].showName;
                saver01.SaveOne(saveData1, animName, bonePlayer1.boneOneMatch.boneType);
            }
            else
            {
                Debug.Log("ERROR ! Wanted to Save at a Wrong Data!");
                yield break;
            }
        }

        tempCurve = weightCenterRangeCurve1;
    }
    private float GetCircleRadius_Min(Vector3 pt1, Vector3 pt2, Vector3 pt3, Vector3 rootPos)
    {
        Vector2 p1v, p2v, p3v, pRootv;
        p1v.x = pt1.x;
        p1v.y = pt1.z;
        p2v.x = pt2.x;
        p2v.y = pt2.z;
        p3v.x = pt3.x;
        p3v.y = pt3.z;
        pRootv.x = rootPos.x;
        pRootv.y = rootPos.z;
        float r1 = Vector2.Distance(pRootv, p1v);
        float r2 = Vector2.Distance(pRootv, p2v);
        float r3 = Vector2.Distance(pRootv, p3v);
        return Mathf.Max(r1, r2, r3);
    }
    private Vector3 GetWeightCenter(Transform _boneRoot)
    {
        Vector3 posAll = Vector3.zero;
        int boneCount = 0;
        GetAllBonePosAdd(_boneRoot, ref posAll, ref boneCount);
        posAll = posAll / boneCount;
        posAll.y = 0;
        return posAll;
    }

    /// <summary>
    /// 获取所有骨骼的位置加和
    /// </summary>
    void GetAllBonePosAdd(Transform bone, ref Vector3 posAll, ref int boneCount)
    {
        if (bone.GetComponent<SphereCollider>() != null)
        {
            posAll += bone.position;
            boneCount++;
        }
        for (int i = 0; i < bone.childCount; i++)
        {
            GetAllBonePosAdd(bone.GetChild(i), ref posAll, ref boneCount);
        }
    }
    private JAnimCurves.AnimOneCurve_InGame GetSavedCurve(bool ismaster)
    {
        if (ismaster)
        { 
            string animName = "";
            if (vfPlayer.ch_master.createdGA_No < vfPlayer.animShowList.Count)//判断是否为内置数据
            {
                animName = vfPlayer.animShowList[vfPlayer.ch_master.createdGA_No].name;
            }
            else if (vfPlayer.ch_master.createdGA_No < vfPlayer.animShowList.Count + vfPlayer.animRecodesShowList.Count)
            {
                animName = vfPlayer.animRecodesShowList[vfPlayer.ch_master.createdGA_No - vfPlayer.animShowList.Count].showName;
            }
            else
            {
                Debug.Log("ERROR! Wanted to Get a Wrong Curve!");
                return null;
            }
            JAnimCurves.AnimCurvesSaver saver1 = new JAnimCurves.AnimCurvesSaver();
            JAnimCurves.AnimOneCurve_ForSave savedData1 = saver1.LoadOne(
             animName, JAnimationData.BoneType.hips, JAnimCurves.animCurveType.weightCenterRange);
            if (savedData1 != null)
            {
                JAnimCurves.AnimOneCurve_InGame data1 = new JAnimCurves.AnimOneCurve_InGame(savedData1);
                return data1;
            }
        }
        else
        { 
            string animName = "";
            if (vfPlayer.ch_student.createdGA_No < vfPlayer.animShowList.Count)//判断是否为内置数据
            {
                animName = vfPlayer.animShowList[vfPlayer.ch_student.createdGA_No].name;
            }
            else if (vfPlayer.ch_master.createdGA_No < vfPlayer.animShowList.Count + vfPlayer.animRecodesShowList.Count)
            {
                animName = vfPlayer.animRecodesShowList[vfPlayer.ch_student.createdGA_No - vfPlayer.animShowList.Count].showName;
            }
            else
            {
                Debug.Log("ERROR! Wanted to Get a Wrong Curve!");
                return null;
            }
            JAnimCurves.AnimCurvesSaver saver1 = new JAnimCurves.AnimCurvesSaver();
            JAnimCurves.AnimOneCurve_ForSave savedData1 = saver1.LoadOne(
                  animName, JAnimationData.BoneType.hips, JAnimCurves.animCurveType.weightCenterRange);
            if (savedData1 != null)
            {
                JAnimCurves.AnimOneCurve_InGame data1 = new JAnimCurves.AnimOneCurve_InGame(savedData1);
                return data1;
            }
        }
        return null;
    }

}
