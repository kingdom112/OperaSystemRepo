using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JAnimationSystem;
using Dreamteck.Splines;
using JAnimCurves;

public class BoneDeal_SpeedCurve : BoneDealBase
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
                        GetSavedSpeedCurve(true, vfPlayer.ch_master.bm_p1.bonePlayers[i_player]);
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
                        GetSavedSpeedCurve(false, vfPlayer.ch_student.bm_p1.bonePlayers[i_player]);
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
        AnimationCurve curve1 = null;
        Color mainColor1 = Color.white;
        List<Color> colors = new List<Color>();
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
                    AnimationCurve speedCurve1 = null;
                    JAnimCurves.AnimOneCurve_InGame savedCurve =
                        GetSavedSpeedCurve(true, vfPlayer.ch_master.bm_p1.bonePlayers[i_player]);
                    if (savedCurve == null)
                    {
                        yield return GetSpeedCurve(true, vfPlayer.ch_master.bm_p1.bonePlayers[i_player]);
                        speedCurve1 = tempCurve;
                        if (speedCurve1 == null)
                        {
                            Debug.Log("xxxxxxxxxx");
                        }
                        max = GetCurveMax(speedCurve1);
                        min = GetCurveMin(speedCurve1);
                    }
                    else
                    {
                        speedCurve1 = savedCurve.curve;
                        max = savedCurve.max;
                        min = savedCurve.min;
                    }
                    if (speedCurve1 != null)
                    {
                        if (curve1 == null)
                        {

                            curve1 = speedCurve1;
                            mainColor1 = mainCurveColor;
                            //Debug.Log("set curve1 (ma): " + i_player + "  " + vfPlayer.ch_master.selectBones[i].Type.ToString());
                        }
                        else
                        {
                            curves.Add(speedCurve1);
                            colors.Add(mainCurveColor);
                            //Debug.Log("add to curves (ma): " + i_player + "  " + vfPlayer.ch_master.selectBones[i].Type.ToString());
                        }
                    }
                    else
                    {
                        //curveSelectDropdown.value = 1;
                        Debug.LogError("计算速度曲线失败！！");
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
                    AnimationCurve speedCurve1 = null;
                    JAnimCurves.AnimOneCurve_InGame savedCurve =
                        GetSavedSpeedCurve(false, vfPlayer.ch_student.bm_p1.bonePlayers[i_player]);
                    if (savedCurve == null)
                    {
                        yield return GetSpeedCurve(false, vfPlayer.ch_student.bm_p1.bonePlayers[i_player]);
                        speedCurve1 = tempCurve;
                        if (speedCurve1 == null)
                        {
                            Debug.Log("xxxxxxxxxx");
                        }
                        max = GetCurveMax(speedCurve1);
                        min = GetCurveMin(speedCurve1);
                    }
                    else
                    {
                        speedCurve1 = savedCurve.curve;
                        max = savedCurve.max;
                        min = savedCurve.min;
                    }
                    if (speedCurve1 != null)
                    {
                        if (curve1 == null)
                        {

                            curve1 = speedCurve1;
                            mainColor1 = addedCurvesColor;
                            //Debug.Log("set curve1 (st): " + i_player + "  " + vfPlayer.ch_student.selectBones[i].Type.ToString());
                        }
                        else
                        {
                            curves.Add(speedCurve1);
                            colors.Add(addedCurvesColor);
                            //Debug.Log("add to curves (st): " + i_player + "  " + vfPlayer.ch_student.selectBones[i].Type.ToString());
                        }
                    }
                    else
                    {
                        //curveSelectDropdown.value = 1;
                        Debug.LogError("计算速度曲线失败！！");
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
        vfPlayer.curveShowWindowControl.sliderY.value = 0f;
    }
    private IEnumerator GetSpeedCurve(bool ismaster, BoneOnePlayer bonePlayer1)
    {
        Debug.Log("get speed"); 
        tempCurve = new AnimationCurve();
        if (bonePlayer1 == null) yield break;
        AnimationCurve speedcurve1 = new AnimationCurve();
        AnimOneCurve_ForSave saveData1 = new AnimOneCurve_ForSave(animCurveType.speed, bonePlayer1.boneOneMatch.boneType);
        float fps = 30f;
        float timejiange = 1f / fps;
        int count = (int)((bonePlayer1.animDatas_Time_roa[bonePlayer1.animDatas_Time_roa.Count - 1] - bonePlayer1.animDatas_Time_roa[0]) / timejiange);
        count++;
        float timer = 0f;
        int yield_k = 0;
        for (int i = 0; i < count; i++)
        {
            Quaternion q1 = bonePlayer1.EvaluateThisFrame_Curve(timer);
            Quaternion q2 = bonePlayer1.EvaluateThisFrame_Curve(timer + timejiange / 6f);
            float speed1 = Quaternion.Angle(q1, q2) / (timejiange / 6f);
            speedcurve1.AddKey(timer, speed1);
            saveData1.AddData(speed1, timer);
            timer += timejiange;
            yield_k++;
            if (yield_k >= yieldWaitTick)
            {
                yield_k = 0;
                yield return 0;
            }
            //Debug.Log(((float)i/(float)count));
            vfPlayer.m_progressWindow.Show("当前", ((float)i / (float)count).ToString(), (float)i / (float)count, "全部", nowCurveCount+"/" + AllCurveCount, (float)nowCurveCount / (float)AllCurveCount, "计算速度曲线中");
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
            else if(vfPlayer.ch_master.createdGA_No < vfPlayer.animShowList.Count + vfPlayer.animRecodesShowList.Count)
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

        tempCurve = speedcurve1;
       
    }

    private JAnimCurves.AnimOneCurve_InGame GetSavedSpeedCurve(bool ismaster, BoneOnePlayer bonePlayer1)
    {
        if (ismaster)
        {
            string animName = "";
            if (vfPlayer.ch_master.createdGA_No < vfPlayer.animShowList.Count)//判断是否为内置数据
            {
                animName = vfPlayer.animShowList[vfPlayer.ch_master.createdGA_No].name;
            }
            else if(vfPlayer.ch_master.createdGA_No < vfPlayer.animShowList.Count + vfPlayer.animRecodesShowList.Count)
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
                animName, bonePlayer1.boneOneMatch.boneType, JAnimCurves.animCurveType.speed);
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
                animName, bonePlayer1.boneOneMatch.boneType, JAnimCurves.animCurveType.speed);
            if (savedData1 != null)
            {
                JAnimCurves.AnimOneCurve_InGame data1 = new JAnimCurves.AnimOneCurve_InGame(savedData1);
                return data1;
            }
        }
        return null;
    }
}
