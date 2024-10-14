using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JAnimationSystem;
using System.IO;

public class JAnimToTXTWindow : EditorWindow
{

    GUIStyle boxStyle;
    GUIStyle richTextStyle_Mid;
    GUIStyle richTextStyle_Left;
    GUIStyle headerStyle;
    GUIStyle smallHeaderStyle;
    GUIStyle lineStyle;


    GameObject ga;
    JAnimation_AutoSetBoneMatchData autoData;
    JAnimationData jad;
    float time_start = 0f;
    float time_end = 0f;
    float fps = 30f;
    bool useOriginFPS = false;

    [MenuItem("JTools/JAnimToTXTWindow")]
    static void Open()
    {
        var win = EditorWindow.GetWindow<JAnimToTXTWindow>(false, "JAnimToTXTWindow", true);
        win.GenerateStyles();
        var icon = Resources.Load("Textures/jicon2") as Texture;
        win.titleContent = new GUIContent("JAnimToTXTWindow", icon);
    }
    void OnGUI()
    {
        GUILayout.Box("JAnimToTXTWindow", boxStyle, GUILayout.Height(60), GUILayout.ExpandWidth(true));
        GUILayout.Box("You can use this tool to turn JAnim To TXT.", richTextStyle_Mid, GUILayout.ExpandWidth(true));

        GUILayout.Space(10);
        DrawALine(3);
        GUILayout.Space(10);
        GUILayout.Label(" BasicSets", headerStyle);
        ga = EditorGUILayout.ObjectField("GameObject", ga, typeof(GameObject), true) as GameObject;
        autoData = EditorGUILayout.ObjectField("AutoData", autoData, typeof(JAnimation_AutoSetBoneMatchData), false) as JAnimation_AutoSetBoneMatchData;
        jad = EditorGUILayout.ObjectField("jad", jad, typeof(JAnimationData), false) as JAnimationData;

        useOriginFPS = EditorGUILayout.Toggle("useOriginFPS", useOriginFPS);
        if(useOriginFPS == false)
        {
            fps = EditorGUILayout.FloatField("fps", fps);
        } 
    
        time_start = EditorGUILayout.FloatField("time_start", time_start);
        time_end = EditorGUILayout.FloatField("time_end", time_end);

        GUILayout.Space(10);
        DrawALine(3);
        GUILayout.Space(10);

        if(GUILayout.Button("Process"))
        {
            List<string> sts = new List<string>();
            sts.Add("");
            sts.Add("");
            sts.Add("");
            sts.Add("");
            sts.Add("");
            List<JAnimationData.BoneType> boneTypes = new List<JAnimationData.BoneType>();
            boneTypes.Add(JAnimationData.BoneType.head);
            boneTypes.Add(JAnimationData.BoneType.leftForeArm);
            boneTypes.Add(JAnimationData.BoneType.leftUpLeg);
            boneTypes.Add(JAnimationData.BoneType.leftLeg);
            boneTypes.Add(JAnimationData.BoneType.leftFoot);
            for(int i=0; i< boneTypes.Count; i++)
            {
                sts[i] += boneTypes[i].ToString();
                sts[i] += "\n" + "时间(从该段第一帧开始),时间(总),x,y,z";
            }
          
            BoneMatch match1 = new BoneMatch();
            match1.StartAutoMatch(ga.transform, autoData, jad.jAD);
            BoneMatch_Player player1 = new BoneMatch_Player(match1);
            player1.SetJAnimationData(jad);

           
            float firsttime = 0f;
            bool hasStart = false;
            if(useOriginFPS)
            {
                for (int i = 0; i < jad.jAD.dataList[0].V4List_roa4.Count; i++)
                {
                    float time1 = jad.jAD.dataList[0].timeList_roa[i];
                    //Debug.Log(time1);
                    if (time1 >= time_start && time1 <= time_end)
                    {
                        if (hasStart == false)
                        {
                            firsttime = time1;
                            hasStart = true;
                        }
                        player1.PlayDataInThisFrame_Curve(time1);
                        for (int j = 0; j < player1.bonePlayers.Count; j++)
                        {
                            if (player1.bonePlayers[j].boneOneMatch.matchedT == null)
                            {
                                continue;
                            }
                            int index1 = boneTypes.IndexOf(player1.bonePlayers[j].boneOneMatch.boneType);
                            if (index1 != -1)
                            {
                                Vector3 pos = player1.bonePlayers[j].boneOneMatch.matchedT.position;
                                sts[index1] += "\n" + (time1 - firsttime) + "," + time1 + "," + pos.x + "," + pos.y + "," + pos.z;
                            }
                        }

                    }
                }
            }
            else
            {
                float jiange = 1f / fps;
                float time1 = time_start;
                firsttime = time1;
                while (time1 <= time_end)
                {
                    player1.PlayDataInThisFrame_Curve(time1);
                    for (int j = 0; j < player1.bonePlayers.Count; j++)
                    {
                        if (player1.bonePlayers[j].boneOneMatch.matchedT == null)
                        {
                            continue;
                        }
                        int index1 = boneTypes.IndexOf(player1.bonePlayers[j].boneOneMatch.boneType);
                        if (index1 != -1)
                        {
                            Vector3 pos = player1.bonePlayers[j].boneOneMatch.matchedT.position;
                            sts[index1] += "\n" + (time1 - firsttime) + "," + time1 + "," + pos.x + "," + pos.y + "," + pos.z;
                        }
                    }

                    time1 += jiange;
                }
                
            }
           
            for (int i = 0; i < boneTypes.Count; i++)
            {
                string path1 = "D:/" + boneTypes[i].ToString() + "_1.txt";
                File.WriteAllText(path1, sts[i]);
            }
          
            /*
            time_start = 122.0106f;
            time_end = 134.383f;
            for (int i = 0; i < boneTypes.Count; i++)
            {
                sts[i] = boneTypes[i].ToString();
                sts[i] += "\n" + "时间(从该段第一帧开始),时间(总),x,y,z";
            }

            firsttime = 0f;
            hasStart = false;
            for (int i = 0; i < jad.jAD.dataList[0].V4List_roa4.Count; i++)
            {
                float time1 = jad.jAD.dataList[0].timeList_roa[i];
                //Debug.Log(time1);
                if (time1 >= time_start && time1 <= time_end)
                {
                    if (hasStart == false)
                    {
                        firsttime = time1;
                        hasStart = true;
                    }
                    player1.PlayDataInThisFrame_Curve(time1);
                    for (int j = 0; j < player1.bonePlayers.Count; j++)
                    {
                        if (player1.bonePlayers[j].boneOneMatch.matchedT == null)
                        {
                            continue;
                        }
                        int index1 = boneTypes.IndexOf(player1.bonePlayers[j].boneOneMatch.boneType);
                        if (index1 != -1)
                        {
                            Vector3 pos = player1.bonePlayers[j].boneOneMatch.matchedT.position;
                            sts[index1] += "\n" + (time1 - firsttime) + "," + time1 + "," + pos.x + "," + pos.y + "," + pos.z;
                        }
                    }

                }
            }
            for (int i = 0; i < boneTypes.Count; i++)
            {
                string path1 = "D:/" + boneTypes[i].ToString() + "_2.txt";
                File.WriteAllText(path1, sts[i]);
            }
            */
        }
    }
     
 
    void DrawALine(int _height)
    {
        GUILayout.Box("", lineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(_height));
    }
    void GenerateStyles()
    {
        boxStyle = new GUIStyle();
        boxStyle.normal.background = Resources.Load("Textures/d_box") as Texture2D;
        boxStyle.normal.textColor = Color.white;
        boxStyle.border = new RectOffset(3, 3, 3, 3);
        boxStyle.margin = new RectOffset(5, 5, 5, 5);
        boxStyle.fontSize = 25;
        boxStyle.fontStyle = FontStyle.Bold;
        boxStyle.font = Resources.Load("Fonts/GAU_Root_Nomal") as Font;
        boxStyle.alignment = TextAnchor.MiddleCenter;

        richTextStyle_Mid = new GUIStyle();
        richTextStyle_Mid.richText = true;
        richTextStyle_Mid.normal.textColor = Color.white;
        richTextStyle_Mid.alignment = TextAnchor.MiddleCenter;

        richTextStyle_Left = new GUIStyle();
        richTextStyle_Left.richText = true;

        headerStyle = new GUIStyle();
        headerStyle.border = new RectOffset(3, 3, 3, 3);
        headerStyle.fontSize = 17;
        headerStyle.fontStyle = FontStyle.Bold;

        smallHeaderStyle = new GUIStyle();
        smallHeaderStyle.border = new RectOffset(3, 3, 3, 3);
        smallHeaderStyle.fontSize = 14;
        smallHeaderStyle.fontStyle = FontStyle.Bold;

        lineStyle = new GUIStyle();
        lineStyle.normal.background = boxStyle.normal.background;
        lineStyle.alignment = TextAnchor.MiddleCenter;
    }
}
