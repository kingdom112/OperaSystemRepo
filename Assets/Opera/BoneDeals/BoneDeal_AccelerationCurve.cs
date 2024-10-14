using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JAnimationSystem;
using Dreamteck.Splines;
using JAnimCurves;

public class BoneDeal_AccelerationCurve : BoneDealBase
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
                        GetSavedAccelerationCurve(true, vfPlayer.ch_master.bm_p1.bonePlayers[i_player]);
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
                        GetSavedAccelerationCurve(false, vfPlayer.ch_student.bm_p1.bonePlayers[i_player]);
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
                    AnimationCurve accCurve1 = null;
                    JAnimCurves.AnimOneCurve_InGame savedCurve =
                        GetSavedAccelerationCurve(true, vfPlayer.ch_master.bm_p1.bonePlayers[i_player]);
                    if (savedCurve == null)
                    {
                        yield return GetAccelerationCurve(true, vfPlayer.ch_master.bm_p1.bonePlayers[i_player]);
                        accCurve1 = tempCurve;
                        max = GetCurveMax(accCurve1);
                        min = GetCurveMin(accCurve1);
                    }
                    else
                    {
                        accCurve1 = savedCurve.curve;
                        max = savedCurve.max;
                        min = savedCurve.min;
                    }
                    if (accCurve1 != null)
                    {
                        if (curve1 == null)
                        {

                            curve1 = accCurve1;
                            mainColor1 = mainCurveColor;
                        }
                        else
                        {
                            curves.Add(accCurve1);
                            colors.Add(mainCurveColor);
                        }
                    }
                    else
                    {
                        //curveSelectDropdown.value = 1;
                        Debug.LogError("计算加速度曲线失败！！");
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
                    AnimationCurve accCurve1 = null;
                    JAnimCurves.AnimOneCurve_InGame savedCurve =
                        GetSavedAccelerationCurve(false, vfPlayer.ch_student.bm_p1.bonePlayers[i_player]);
                    if (savedCurve == null)
                    {
                        yield return GetAccelerationCurve(false, vfPlayer.ch_student.bm_p1.bonePlayers[i_player]);
                        accCurve1 = tempCurve;
                        max = GetCurveMax(accCurve1);
                        min = GetCurveMin(accCurve1);
                    }
                    else
                    {
                        accCurve1 = savedCurve.curve;
                        max = savedCurve.max;
                        min = savedCurve.min;
                    }
                    if (accCurve1 != null)
                    {
                        if (curve1 == null)
                        {

                            curve1 = accCurve1;
                            mainColor1 = addedCurvesColor;
                        }
                        else
                        {
                            curves.Add(accCurve1);
                            colors.Add(addedCurvesColor);
                        }
                    }
                    else
                    {
                        //curveSelectDropdown.value = 1;
                        Debug.LogError("计算加速度曲线失败！！");
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

    private IEnumerator GetAccelerationCurve(bool ismaster, BoneOnePlayer bonePlayer1)
    {
        Debug.Log("get Acceleration");
        tempCurve = new AnimationCurve();
        if (bonePlayer1 == null) yield break;
        AnimationCurve accelerationCurve1 = new AnimationCurve();
        AnimOneCurve_ForSave saveData1 = new AnimOneCurve_ForSave(animCurveType.acceleration, bonePlayer1.boneOneMatch.boneType);
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
            Quaternion q3 = bonePlayer1.EvaluateThisFrame_Curve(timer + timejiange / 6f * 2f);
            float speed1 = Quaternion.Angle(q1, q2) / (timejiange / 6f);
            float speed2 = Quaternion.Angle(q2, q3) / (timejiange / 6f);
            float acceleration1 = Mathf.Abs(speed2 - speed1) / (timejiange / 6f);
            accelerationCurve1.AddKey(timer, acceleration1);
            saveData1.AddData(acceleration1, timer);
            timer += timejiange;
            yield_k++;
            if (yield_k >= yieldWaitTick)
            {
                yield_k = 0;
                yield return 0;
            }
            //Debug.Log(((float)i/(float)count));
            vfPlayer.m_progressWindow.Show("当前", ((float)i / (float)count).ToString(), (float)i / (float)count, "全部", nowCurveCount + "/" + AllCurveCount, (float)nowCurveCount / (float)AllCurveCount, "计算加速度曲线中");
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

        tempCurve = accelerationCurve1;

    }


    private JAnimCurves.AnimOneCurve_InGame GetSavedAccelerationCurve(bool ismaster, BoneOnePlayer bonePlayer1)
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
                animName, bonePlayer1.boneOneMatch.boneType, JAnimCurves.animCurveType.acceleration);
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
                animName, bonePlayer1.boneOneMatch.boneType, JAnimCurves.animCurveType.acceleration);
            if (savedData1 != null)
            {
                JAnimCurves.AnimOneCurve_InGame data1 = new JAnimCurves.AnimOneCurve_InGame(savedData1);
                return data1;
            }
        }
        return null;
    }

}
