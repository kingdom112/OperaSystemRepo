using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JAnimationSystem;
using Dreamteck.Splines;
using JAnimCurves;

public class BoneDeal_AngleCurve : BoneDealBase
{

    public override int NeedCalculateCount()
    {
        int _needCalculateCount = 0;

        for (int i = 0; i < vfPlayer.ch_master.selectBones.Count; i++)
        {
            int index = vfPlayer.ch_master.GetIndexOfSelectBone(vfPlayer.ch_master.selectBones[i].Type);
            if (index != -1)
            {
                int i_player = vfPlayer.ch_master.bm_p1.GetPlayerIndexByBoneType(vfPlayer.ch_master.selectBones[i].Type);
                if (i_player != -1)
                { 
                    JAnimCurves.AnimOneCurve_InGame savedCurve =
                        GetSavedAngleCurve(true, vfPlayer.ch_master.bm_p1.bonePlayers[i_player]);
                    if (savedCurve == null)
                    {
                        _needCalculateCount++;
                    }
                }
            }
        }
        for (int i = 0; i < vfPlayer.ch_student.selectBones.Count; i++)
        {
            int index = vfPlayer.ch_student.GetIndexOfSelectBone(vfPlayer.ch_student.selectBones[i].Type);
            if (index != -1)
            {
                int i_player = vfPlayer.ch_student.bm_p1.GetPlayerIndexByBoneType(vfPlayer.ch_student.selectBones[i].Type);
                if (i_player != -1)
                { 
                    JAnimCurves.AnimOneCurve_InGame savedCurve =
                        GetSavedAngleCurve(false, vfPlayer.ch_student.bm_p1.bonePlayers[i_player]);
                    if (savedCurve == null)
                    {
                        _needCalculateCount++;
                    }
                }
            }
        }

        return _needCalculateCount;
    }


    private int AllCurveCount = 0;
    private int nowCurveCount = 0;
    public override IEnumerator DealCalculate()
    {
        nowCurveCount = 0;
        AllCurveCount = vfPlayer.ch_master.selectBones.Count + vfPlayer.ch_student.selectBones.Count;
        Color mainColor1 = Color.white;
        List<Color> colors = new List<Color>();
        AnimationCurve curve1 = null;
        float max = 0f;
        float min = 0f;
        List<AnimationCurve> curves = new List<AnimationCurve>();
        for (int i = 0; i < vfPlayer.ch_master.selectBones.Count; i++)
        {
            nowCurveCount++;
            int index = vfPlayer.ch_master.GetIndexOfSelectBone(vfPlayer.ch_master.selectBones[i].Type);
            if (index != -1)
            {
                int i_player = vfPlayer.ch_master.bm_p1.GetPlayerIndexByBoneType(vfPlayer.ch_master.selectBones[i].Type);
                if (i_player != -1)
                {
                    AnimationCurve angleCurve1 = null;
                    JAnimCurves.AnimOneCurve_InGame savedCurve =
                        GetSavedAngleCurve(true, vfPlayer.ch_master.bm_p1.bonePlayers[i_player]);
                    if (savedCurve == null)
                    {
                        yield return GetAngleCurve(true, vfPlayer.ch_master.bm_p1, vfPlayer.ch_master.bm_p1.bonePlayers[i_player]);
                        angleCurve1 = tempCurve;
                        max = GetCurveMax(angleCurve1);
                        min = GetCurveMin(angleCurve1);
                    }
                    else
                    {
                        angleCurve1 = savedCurve.curve;
                        max = savedCurve.max;
                        min = savedCurve.min;
                    }
                    if (angleCurve1 != null)
                    {
                        if (curve1 == null)
                        {

                            curve1 = angleCurve1;
                            mainColor1 = mainCurveColor;
                        }
                        else
                        {
                            curves.Add(angleCurve1);
                            colors.Add(mainCurveColor);
                        }
                    }
                    else
                    {
                        //curveSelectDropdown.value = 1;
                        Debug.LogError("计算角度曲线失败！！");
                    }
                }
            }
        }
        for (int i = 0; i < vfPlayer.ch_student.selectBones.Count; i++)
        {
            nowCurveCount++;
            int index = vfPlayer.ch_student.GetIndexOfSelectBone(vfPlayer.ch_student.selectBones[i].Type);
            if (index != -1)
            {
                int i_player = vfPlayer.ch_student.bm_p1.GetPlayerIndexByBoneType(vfPlayer.ch_student.selectBones[i].Type);
                if (i_player != -1)
                {
                    AnimationCurve angleCurve1 = null;
                    JAnimCurves.AnimOneCurve_InGame savedCurve =
                        GetSavedAngleCurve(false, vfPlayer.ch_student.bm_p1.bonePlayers[i_player]);
                    if (savedCurve == null)
                    {
                        yield return GetAngleCurve(false, vfPlayer.ch_student.bm_p1, vfPlayer.ch_student.bm_p1.bonePlayers[i_player]);
                        angleCurve1 = tempCurve;
                        max = GetCurveMax(angleCurve1);
                        min = GetCurveMin(angleCurve1);
                    }
                    else
                    {
                        angleCurve1 = savedCurve.curve;
                        max = savedCurve.max;
                        min = savedCurve.min;
                    }
                    if (angleCurve1 != null)
                    {
                        if (curve1 == null)
                        {

                            curve1 = angleCurve1;
                            mainColor1 = addedCurvesColor;
                        }
                        else
                        {
                            curves.Add(angleCurve1);
                            colors.Add(addedCurvesColor);
                        }
                    }
                    else
                    {
                        //curveSelectDropdown.value = 1;
                        Debug.LogError("计算角度曲线失败！！");
                    }
                }
            }
        }
        vfPlayer.m_progressWindow.Close();
        if (vfPlayer.curveShowWindowControl == null)
        {
            vfPlayer.CreateRTECurveWindow();//create
        }
        vfPlayer.curveShowWindowControl.SetCurve(curve1, max, min, mainColor1);
        vfPlayer.curveShowWindowControl.SetCurves(curves, colors);
        vfPlayer.curveShowWindowControl.sliderY.value = 1f;
        vfPlayer.curveShowWindowControl.SetY_Max(220);
        vfPlayer.curveShowWindowControl.SetY_Min(0);
    }


    private IEnumerator GetAngleCurve(bool ismaster, BoneMatch_Player bm_p1, BoneOnePlayer bonePlayer1)
    {
        Debug.Log("get agle");
        tempCurve = new AnimationCurve();
        if (bm_p1 == null) yield break;
        if (bonePlayer1 == null) yield break;
        AnimationCurve curve1 = new AnimationCurve();
        AnimOneCurve_ForSave saveData1 = new AnimOneCurve_ForSave(animCurveType.angle, bonePlayer1.boneOneMatch.boneType);
        float fps = 30f;
        float timejiange = 1f / fps;
        Transform boneTarget1 = bonePlayer1.boneOneMatch.matchedT;
        int count = (int)((bonePlayer1.animDatas_Time_roa[bonePlayer1.animDatas_Time_roa.Count - 1] - bonePlayer1.animDatas_Time_roa[0]) / timejiange);
        count++;
        float timer = 0f;
        int yield_k = 0;
        for (int i = 0; i < count; i++)
        {
            bm_p1.PlayDataInThisFrame_Curve(timer);
            Vector3 v1 = (boneTarget1.parent.position - boneTarget1.position).normalized;
            Vector3 v2 = Vector3.zero;
            for (int j = 0; j < boneTarget1.childCount; j++)
            {
                if (bm_p1.boneMatch.GetIndexOfMatchedT(boneTarget1.GetChild(j)) != -1)
                {
                    v2 = (boneTarget1.GetChild(j).position - boneTarget1.position).normalized;
                    break;
                }
            }
            float angle1 = Vector3.Angle(v1, v2);
            curve1.AddKey(timer, angle1);
            saveData1.AddData(angle1, timer);
            timer += timejiange;
            yield_k++;
            if (yield_k >= yieldWaitTick)
            {
                yield_k = 0;
                yield return 0;
            }
            //Debug.Log(((float)i/(float)count));
            vfPlayer.m_progressWindow.Show("当前", ((float)i / (float)count).ToString(), (float)i / (float)count, "全部", nowCurveCount + "/" + AllCurveCount, (float)nowCurveCount / (float)AllCurveCount, "计算夹角曲线中");
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

        tempCurve = curve1;
    }

    private JAnimCurves.AnimOneCurve_InGame GetSavedAngleCurve(bool ismaster, BoneOnePlayer bonePlayer1)
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
                animName, bonePlayer1.boneOneMatch.boneType, JAnimCurves.animCurveType.angle);
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
                animName, bonePlayer1.boneOneMatch.boneType, JAnimCurves.animCurveType.angle);
            if (savedData1 != null)
            {
                JAnimCurves.AnimOneCurve_InGame data1 = new JAnimCurves.AnimOneCurve_InGame(savedData1);
                return data1;
            }
        }
        return null;
    }
}
