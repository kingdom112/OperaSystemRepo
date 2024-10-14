using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JAnimationSystem;
using Dreamteck.Splines;
using JAnimCurves;

public class BoneDeal_RootHeightCurve : BoneDealBase
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
                GetSavedRootHeightCurve(true);
            if (savedCurve == null)
            {
                _needCalculateCount++;
            }
        }
        if (vfPlayer.ch_student.hasInit)
        {
            JAnimCurves.AnimOneCurve_InGame savedCurve =
                GetSavedRootHeightCurve(false);
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
            AnimationCurve rootHeightCurve1 = null;
            JAnimCurves.AnimOneCurve_InGame savedCurve =
                GetSavedRootHeightCurve(true);
            if (savedCurve == null)
            {
                yield return GetRootHeightCurve(true);
                rootHeightCurve1 = tempCurve;
                max = GetCurveMax(rootHeightCurve1);
                min = GetCurveMin(rootHeightCurve1);
            }
            else
            {
                rootHeightCurve1 = savedCurve.curve;
                max = savedCurve.max;
                min = savedCurve.min;
            }
            if (rootHeightCurve1 != null)
            {
                curve1 = rootHeightCurve1;
                mainColor1 = mainCurveColor;
            }
            else
            {
                Debug.LogError("计算根骨高度曲线失败！！");
            }
        }
        if (vfPlayer.ch_student.hasInit)
        {
            AnimationCurve rootHeightCurve1 = null;
            JAnimCurves.AnimOneCurve_InGame savedCurve =
                GetSavedRootHeightCurve(false);
            if (savedCurve == null)
            {
                yield return GetRootHeightCurve(false);
                rootHeightCurve1 = tempCurve;
                max = GetCurveMax(rootHeightCurve1);
                min = GetCurveMin(rootHeightCurve1);
            }
            else
            {
                rootHeightCurve1 = savedCurve.curve;
                max = savedCurve.max;
                min = savedCurve.min;
            }
            if (rootHeightCurve1 != null)
            {
                curves.Add(rootHeightCurve1);
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
        vfPlayer.curveShowWindowControl.SetCurve(curve1, max, min , mainColor1);
        vfPlayer.curveShowWindowControl.SetCurves(curves, colors);
        vfPlayer.curveShowWindowControl.sliderY.value = 0.5f;
    }

    private IEnumerator GetRootHeightCurve(bool ismaster)
    {
        Debug.Log("get RootHeight");
        tempCurve = new AnimationCurve();
        AnimationCurve rootHeightcurve1 = new AnimationCurve();
        AnimOneCurve_ForSave saveData1 = new AnimOneCurve_ForSave(animCurveType.rootHeight, JAnimationData.BoneType.hips);
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
            float height1 = bonePlayer1.boneOneMatch.matchedT.position.y;
            rootHeightcurve1.AddKey(timer, height1);
            saveData1.AddData(height1, timer);
            timer += timejiange;
            yield_k++;
            if (yield_k >= yieldWaitTick)
            {
                yield_k = 0;
                yield return 0;
            }
            //Debug.Log(((float)i/(float)count));
            vfPlayer.m_progressWindow.Show("当前", ((float)i / (float)count).ToString(), (float)i / (float)count, "全部", nowCurveCount + "/" + AllCurveCount, (float)nowCurveCount / (float)AllCurveCount, "计算胯部高度曲线中");
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

        tempCurve = rootHeightcurve1;
    }


    private JAnimCurves.AnimOneCurve_InGame GetSavedRootHeightCurve(bool ismaster)
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
                animName, JAnimationData.BoneType.hips, JAnimCurves.animCurveType.rootHeight);
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
              animName, JAnimationData.BoneType.hips, JAnimCurves.animCurveType.rootHeight);
            if (savedData1 != null)
            {
                JAnimCurves.AnimOneCurve_InGame data1 = new JAnimCurves.AnimOneCurve_InGame(savedData1);
                return data1;
            }
        }
        return null;
    }

}
