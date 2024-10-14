using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JAnimationSystem;
using Dreamteck.Splines;
using JAnimCurves;

public class BoneDeal_SpineVerticalAngleCurve : BoneDealBase
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
                GetSavedSpineVerticalAngleCurve(true);
            if (savedCurve == null)
            {
                _needCalculateCount++;
            }
        }
        if (vfPlayer.ch_student.hasInit)
        {
            JAnimCurves.AnimOneCurve_InGame savedCurve =
                GetSavedSpineVerticalAngleCurve(false);
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
            AnimationCurve spineVerticalAngleCurve1 = null;
            JAnimCurves.AnimOneCurve_InGame savedCurve =
                GetSavedSpineVerticalAngleCurve(true);
            if (savedCurve == null)
            {
                yield return GetSpineVerticalAngleCurve(true);
                spineVerticalAngleCurve1 = tempCurve;
                max = GetCurveMax(spineVerticalAngleCurve1);
                min = GetCurveMin(spineVerticalAngleCurve1);
            }
            else
            {
                spineVerticalAngleCurve1 = savedCurve.curve;
                max = savedCurve.max;
                min = savedCurve.min;
            }
            if (spineVerticalAngleCurve1 != null)
            {
                curve1 = spineVerticalAngleCurve1;
                mainColor1 = mainCurveColor;
            }
            else
            {
                Debug.LogError("计算脊柱垂直角度曲线失败！！");
            }
        }
        if (vfPlayer.ch_student.hasInit)
        {
            AnimationCurve spineVerticalAngleCurve1 = null;
            JAnimCurves.AnimOneCurve_InGame savedCurve =
                GetSavedSpineVerticalAngleCurve(false);
            if (savedCurve == null)
            {
                yield return GetSpineVerticalAngleCurve(false);
                spineVerticalAngleCurve1 = tempCurve;
                max = GetCurveMax(spineVerticalAngleCurve1);
                min = GetCurveMin(spineVerticalAngleCurve1);
            }
            else
            {
                spineVerticalAngleCurve1 = savedCurve.curve;
                max = savedCurve.max;
                min = savedCurve.min;
            }
            if (spineVerticalAngleCurve1 != null)
            {
                curves.Add(spineVerticalAngleCurve1);
                colors.Add(addedCurvesColor);
            }
            else
            {
                Debug.LogError("计算脊柱垂直角度曲线失败！！");
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

    private IEnumerator GetSpineVerticalAngleCurve(bool ismaster)
    {
        BoneMatch_Player bm_p = null;
        tempCurve = new AnimationCurve();
        if (ismaster)
        {
            if (vfPlayer.ch_master.hasInit)
            {
                bm_p = vfPlayer.ch_master.bm_p1;
            }
        }
        else
        {
            if (vfPlayer.ch_student.hasInit)
            {
                bm_p = vfPlayer.ch_student.bm_p1;
            }
        }
        if (bm_p != null)
        {
            Transform spineTop = GetSpineTop(ismaster);
            if (spineTop != null)
            {
                int hipIndex = bm_p.GetPlayerIndexByBoneType(JAnimationData.BoneType.hips);
                Transform hip1 = bm_p.bonePlayers[hipIndex].boneOneMatch.matchedT;

                float fps = 30f;
                float timejiange = 1f / fps;
                AnimationCurve curve1 = new AnimationCurve();
                AnimOneCurve_ForSave saveData1 = new AnimOneCurve_ForSave(animCurveType.spineVerticalAngle, JAnimationData.BoneType.hips);
                BoneOnePlayer bonePlayer1 = bm_p.bonePlayers[hipIndex];
                int count = (int)((bonePlayer1.animDatas_Time_roa[bonePlayer1.animDatas_Time_roa.Count - 1] - bonePlayer1.animDatas_Time_roa[0]) / timejiange);
                count++;
                float timer = 0f;
                int yield_k = 0;
                for (int i = 0; i < count; i++)
                {
                    bm_p.PlayDataInThisFrame_Curve(timer);
                    float angle1 = Vector3.Angle((spineTop.position - hip1.position).normalized, Vector3.up);
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
                    vfPlayer.m_progressWindow.Show("当前", ((float)i / (float)count).ToString(), (float)i / (float)count, "全部", nowCurveCount + "/" + AllCurveCount, (float)nowCurveCount / (float)AllCurveCount, "计算脊柱垂直角度曲线中");
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
        }
        //return null;
    }


    private JAnimCurves.AnimOneCurve_InGame GetSavedSpineVerticalAngleCurve(bool ismaster)
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
                 animName, JAnimationData.BoneType.hips, JAnimCurves.animCurveType.spineVerticalAngle);
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
                 animName, JAnimationData.BoneType.hips, JAnimCurves.animCurveType.spineVerticalAngle);
            if (savedData1 != null)
            {
                JAnimCurves.AnimOneCurve_InGame data1 = new JAnimCurves.AnimOneCurve_InGame(savedData1);
                return data1;
            }
        }
        return null;
    }


    private Transform GetSpineTop(bool ismaster)
    {
        BoneMatch_Player bm_p = null;
        if (ismaster)
        {
            if (vfPlayer.ch_master.hasInit)
            {
                bm_p = vfPlayer.ch_master.bm_p1;
            }
        }
        else
        {
            if (vfPlayer.ch_student.hasInit)
            {
                bm_p = vfPlayer.ch_student.bm_p1;
            }
        }
        if (bm_p != null)
        {
            int hipIndex = bm_p.GetPlayerIndexByBoneType(JAnimationData.BoneType.hips);
            int neckIndex = bm_p.GetPlayerIndexByBoneType(JAnimationData.BoneType.neck);
            if (hipIndex != -1 && neckIndex != -1)
            {
                Transform neck = bm_p.bonePlayers[neckIndex].boneOneMatch.matchedT;
                if (neck != null)
                {
                    int index1 = bm_p.GetPlayerIndexByMatchedT(neck.parent);
                    if (index1 != -1)
                    {
                        return bm_p.bonePlayers[index1].boneOneMatch.matchedT;
                    }
                }
            }
        }
        return null;
    }

}
