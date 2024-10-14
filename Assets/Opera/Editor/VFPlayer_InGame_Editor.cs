using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JAnimationSystem;
using JVehicleFrameSystem;
using JAnimCurves;


[CustomEditor(typeof(VFPlayer_InGame))]
public class VFPlayer_InGame_Editor : Editor
{

    /// <summary>
    /// 预计算曲线用到的fps
    /// </summary>
    float fps_animCurve = 30f;
    string filename = "";
    AnimationCurve curve_show = new AnimationCurve();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);
        filename = EditorGUILayout.TextField(filename);
        EditorGUILayout.CurveField(curve_show);
        VFPlayer_InGame _script = target as VFPlayer_InGame;
        if (GUILayout.Button("从文件读取选段数据(animShowList.selectRanges)"))
        {
            for(int i=0; i< _script.animShowList.Count; i++)
            {
                string path = JAnimDataToFile.JADSavePath;
                JAnimDataToFile.JAnimRanges janimRanges = JAnimDataToFile.LoadJAnimRanges(path, _script.animShowList[i].name);//加载动作选段
                if(janimRanges != null)
                {
                    _script.animShowList[i].selectRanges = janimRanges.selectRanges;
                    Debug.Log("加载了 [" + _script.animShowList[i].name + "]的选段数据");
                } 
            }
        }
        if(GUILayout.Button("预计算所有animShowList里面的曲线"))
        {
            int countAll = 0;
            float timejiange = 1f / fps_animCurve;
            for (int i=0; i< _script.animShowList.Count; i++)
            { 
                int count1 = (int)(_script.animShowList[i].jAnimData.jAD.TimeLength / timejiange);
                count1++;
                countAll += count1 * _script.animShowList[i].jAnimData.jAD.dataList.Count; 
            }

            int counter = 0;
            for (int i = 0; i < _script.animShowList.Count; i++)
            { 
                if (_script.animShowList[i].jAnimData == null || _script.animShowList[i].autodata == null || _script.animShowList[i].frame == null)
                {
                    Debug.LogWarning("animShowList[" + i + "]由于数据缺失，不能进行曲线预计算！");
                    continue;
                }
               
                VehicleBuilder builder1 = new VehicleBuilder(_script.animShowList[i].frame.boneFramework);
                builder1.Build(0);
                GameObject ga1 = builder1.buildedGA;
                BoneMatch bm1 = new BoneMatch();
                bm1.StartAutoMatch(ga1.transform, _script.animShowList[i].autodata, _script.animShowList[i].jAnimData.jAD);
                BoneMatch_Player bm_p = new BoneMatch_Player(bm1);
                bm_p.SetJAnimationData(_script.animShowList[i].jAnimData.jAD);
                 
                int count1 = (int)(_script.animShowList[i].jAnimData.jAD.TimeLength / timejiange);
                count1++;
               
                for(int k = 0; k< bm_p.bonePlayers.Count; k++)
                {
                    if (bm_p.bonePlayers[k].boneOneMatch.matchedT == null)
                    {
                        counter += count1;
                        continue; 
                    }
                   
                    Transform boneTarget1 = bm_p.bonePlayers[k].boneOneMatch.matchedT;
                    AnimOneCurve_ForSave angleData1 = new AnimOneCurve_ForSave( animCurveType.angle, bm_p.bonePlayers[k].boneOneMatch.boneType);
                    AnimOneCurve_ForSave speedData1 = new AnimOneCurve_ForSave(animCurveType.speed, bm_p.bonePlayers[k].boneOneMatch.boneType);
                    float timer = 0f;
                    for (int j1 = 0; j1 < count1; j1++)
                    {
                        bm_p.PlayDataInThisFrame_Curve(timer);
                        Vector3 v1 = (boneTarget1.parent.position - boneTarget1.position).normalized;
                        Vector3 v2 = Vector3.zero;
                        for (int j = 0; j < boneTarget1.childCount; j++)
                        {
                            if (bm_p.boneMatch.GetIndexOfMatchedT(boneTarget1.GetChild(j)) != -1)
                            {
                                v2 = (boneTarget1.GetChild(j).position - boneTarget1.position).normalized;
                                break;
                            }
                        }
                        float angle1 = Vector3.Angle(v1, v2);
                        angleData1.AddData(angle1, timer);

                        Quaternion q1 = bm_p.bonePlayers[k].EvaluateThisFrame_Curve(timer);
                        Quaternion q2 = bm_p.bonePlayers[k].EvaluateThisFrame_Curve(timer + timejiange / 6f);
                        float speed1 = Quaternion.Angle(q1, q2) / (timejiange / 6f);
                        speedData1.AddData(speed1, timer);

                        timer += timejiange;
                        counter++;
                        EditorUtility.DisplayProgressBar("处理中", "正在预计算曲线", Mathf.Clamp01((float)counter/(float)countAll));
                    }

                    AnimCurvesSaver saver01 = new AnimCurvesSaver();
                    saver01.SaveOne(angleData1, _script.animShowList[i].name, bm_p.bonePlayers[k].boneOneMatch.boneType);
                    saver01.SaveOne(speedData1, _script.animShowList[i].name, bm_p.bonePlayers[k].boneOneMatch.boneType);

                     
                }
                DestroyImmediate(ga1);
                 
            }
            EditorUtility.ClearProgressBar(); 
         
        }
         
        if(GUILayout.Button("adwadawdawds"))
        {
            AnimCurvesSaver saver2 = new AnimCurvesSaver();
            AnimOneCurve_InGame oncurve
                = new AnimOneCurve_InGame(saver2.LoadOne(filename));
            curve_show = oncurve.curve;
        }
    }

     

    void ShowProcessBar(float _Jindu)
    {
        if (Event.current.type == EventType.Repaint)
        {
            var lastRect = GUILayoutUtility.GetLastRect();
            EditorGUI.ProgressBar(new Rect(lastRect.x, lastRect.y + lastRect.height, lastRect.width, 20), Mathf.Clamp01(_Jindu), "");
        }
        GUILayout.Space(20);
    }
}
