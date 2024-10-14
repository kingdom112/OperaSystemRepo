using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JAnimationSystem;

public class JAnimFileEditorWindow : EditorWindow
{
    GUIStyle boxStyle;
    GUIStyle richTextStyle_Mid;
    GUIStyle richTextStyle_Left;
    GUIStyle headerStyle;
    GUIStyle smallHeaderStyle;
    GUIStyle lineStyle;

     
    JAnimationData jAnimData;


    [MenuItem("JTools/JAnimFileEditor")]
    static void Open()
    {
        var win = EditorWindow.GetWindow<JAnimFileEditorWindow>(false, "JAnimFileEditor", true);
        win.GenerateStyles();
        var icon = Resources.Load("Textures/jicon2") as Texture;
        win.titleContent = new GUIContent("JAnimFileEditor", icon); 
    }


    Vector2 scrollPos = Vector2.zero;
    List<bool> jAnimDataListFoldOut = new List<bool>();
    List<bool> foldout_Roa4List = new List<bool>();
    Vector2 roaEditing = new Vector2(-1,-1);
    Vector3 edit_v3 = Vector3.zero;
    void OnGUI()
    {
        GUILayout.Box("JAnimFileEditor", boxStyle, GUILayout.Height(60), GUILayout.ExpandWidth(true));
        GUILayout.Box("You can use this tool to edit one JAnimFile.", richTextStyle_Mid, GUILayout.ExpandWidth(true));

        GUILayout.Space(10);
        DrawALine(3);
        GUILayout.Space(10);
        GUILayout.Label(" BasicSets", headerStyle); 
        jAnimData = EditorGUILayout.ObjectField("JAnimData", jAnimData, typeof(JAnimationData), false) as JAnimationData;

        GUILayout.Space(10);
        DrawALine(3);
        GUILayout.Space(10);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        if(jAnimData != null)
        {
            for(int i1= jAnimDataListFoldOut.Count; i1< jAnimData.dataList.Count; i1++)
            {
                jAnimDataListFoldOut.Add(false);
                foldout_Roa4List.Add(false);

            } 
            GUILayout.Space(10);
            GUILayout.Label(
                  "  <color=blue>This data is Roa4.</color>", richTextStyle_Left);
            GUILayout.Space(10);
            for (int i = 0; i < jAnimData.dataList.Count; i++)
            {
                jAnimDataListFoldOut[i] =  
                    EditorGUILayout.Foldout(jAnimDataListFoldOut[i], jAnimData.dataList[i].boneType.ToString());
                if(jAnimDataListFoldOut[i])
                {
                    EditorGUI.indentLevel++;
                    foldout_Roa4List[i] =
                        EditorGUILayout.Foldout(foldout_Roa4List[i], "V4 Roa List");
                    if (foldout_Roa4List[i])
                    {
                        EditorGUI.indentLevel++;
                        for (int j = 0; j < jAnimData.dataList[i].V4List_roa4.Count; j++)
                        {
                            GUI.enabled = false;
                            EditorGUILayout.Vector3Field("roa" + j, new Quaternion(jAnimData.dataList[i].V4List_roa4[j].x, jAnimData.dataList[i].V4List_roa4[j].y, jAnimData.dataList[i].V4List_roa4[j].z, jAnimData.dataList[i].V4List_roa4[j].w).eulerAngles);
                            GUI.enabled = true;

                            if (roaEditing.x == i && roaEditing.y == j)
                            {
                                edit_v3 =
                               EditorGUILayout.Vector3Field("set to", edit_v3);
                            }
                            if (roaEditing.x != i || roaEditing.y != j)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Space(60);
                                if (GUILayout.Button("edit"))
                                {
                                    roaEditing.x = i;
                                    roaEditing.y = j;
                                    edit_v3 = Vector3.zero;
                                    return;
                                }
                                if(GUILayout.Button("-",GUILayout.Width(40)))
                                {
                                    jAnimData.dataList[i].V4List_roa4.RemoveAt(j);
                                    return;
                                }
                                if (GUILayout.Button("+", GUILayout.Width(40)))
                                {
                                    jAnimData.dataList[i].V4List_roa4.Insert(j+1,new Vector4(0,0,0,1));
                                    return;
                                }
                                GUILayout.EndHorizontal();
                            }
                            else
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Space(60);
                                if (GUILayout.Button("save"))
                                {
                                    roaEditing = new Vector2(-1, -1);
                                    Quaternion q1 = Quaternion.Euler(edit_v3);
                                    jAnimData.dataList[i].V4List_roa4[j] = new Vector4(q1.x, q1.y, q1.z, q1.w);
                                    return;
                                }
                                if (GUILayout.Button("-", GUILayout.Width(40)))
                                {
                                    jAnimData.dataList[i].V4List_roa4.RemoveAt(j);
                                    return;
                                }
                                if (GUILayout.Button("+", GUILayout.Width(40)))
                                {
                                    jAnimData.dataList[i].V4List_roa4.Insert(j+1, new Vector4(0, 0, 0, 1));
                                    return;
                                }
                                GUILayout.EndHorizontal();
                            }

                        }
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(40);
                        if(GUILayout.Button("+",GUILayout.Width(100)))
                        {
                            jAnimData.dataList[i].V4List_roa4.Add(new Vector4(0, 0, 0, 1));
                        }
                        GUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                    }

                    AnimationCurve curve1 = new AnimationCurve();
                    AnimationCurve curve2 = new AnimationCurve();
                    AnimationCurve curve3 = new AnimationCurve();
                    AnimationCurve curve4 = new AnimationCurve();
                    for(int j =0; j < jAnimData.dataList[i].V4List_roa4.Count; j++)
                    {
                        curve1.AddKey(jAnimData.dataList[i].timeList_roa[j], jAnimData.dataList[i].V4List_roa4[j].x);
                    }
                    EditorGUILayout.CurveField("x", curve1); 
                    EditorGUILayout.CurveField("x2", GetCurve4_RoaExceptY_X(jAnimData.dataList[i].V4List_roa4, jAnimData.dataList[i].timeList_roa, false));
                    EditorGUILayout.CurveField("x3", GetCurve4_RoaExceptY_X(jAnimData.dataList[i].V4List_roa4, jAnimData.dataList[i].timeList_roa));

                    curve2 = new AnimationCurve();
                    for (int j = 0; j < jAnimData.dataList[i].V4List_roa4.Count; j++)
                    {
                        curve2.AddKey(jAnimData.dataList[i].timeList_roa[j], jAnimData.dataList[i].V4List_roa4[j].y);
                    }
                    EditorGUILayout.CurveField("y", curve2);
                    EditorGUILayout.CurveField("y2", GetCurve4_RoaExceptY_Y(jAnimData.dataList[i].V4List_roa4, jAnimData.dataList[i].timeList_roa));
                    curve3 = new AnimationCurve();
                    for (int j = 0; j < jAnimData.dataList[i].V4List_roa4.Count; j++)
                    {
                        curve3.AddKey(jAnimData.dataList[i].timeList_roa[j], jAnimData.dataList[i].V4List_roa4[j].z);
                    }
                    EditorGUILayout.CurveField("z", curve3);
                    EditorGUILayout.CurveField("z2", GetCurve4_RoaExceptY_Z(jAnimData.dataList[i].V4List_roa4, jAnimData.dataList[i].timeList_roa));
                    curve4 = new AnimationCurve();
                    for (int j = 0; j < jAnimData.dataList[i].V4List_roa4.Count; j++)
                    {
                        curve4.AddKey(jAnimData.dataList[i].timeList_roa[j], jAnimData.dataList[i].V4List_roa4[j].w);
                    }
                    EditorGUILayout.CurveField("w", curve4);
                    EditorGUILayout.CurveField("w2", GetCurve4_RoaExceptY_W(jAnimData.dataList[i].V4List_roa4, jAnimData.dataList[i].timeList_roa, false));
                    EditorGUILayout.CurveField("w3", GetCurve4_RoaExceptY_W(jAnimData.dataList[i].V4List_roa4, jAnimData.dataList[i].timeList_roa));

                    EditorGUI.indentLevel--;
                }


            }
        } 

        EditorGUILayout.EndScrollView();
    }

    public AnimationCurve GetCurve4_RoaExceptY_X(List<Vector4> animDatas_Roa4, List<float> animDatas_Time_roa, bool smooth = true)
    {
        if (animDatas_Roa4.Count > 0)
        {
            AnimationCurve curve1 = new AnimationCurve();
            for (int i = 0; i < animDatas_Roa4.Count; i++)
            {
                Vector3 roa1 = (new Quaternion(animDatas_Roa4[i].x, animDatas_Roa4[i].y, animDatas_Roa4[i].z, animDatas_Roa4[i].w)).eulerAngles;
                roa1.y = 0f;
                Quaternion q1 = Quaternion.Euler(roa1);
                curve1.AddKey(animDatas_Time_roa[i], q1.x);
              
            }
            if (smooth)
            {
                for (int i = 0; i < curve1.keys.Length; i++)
                {
                    curve1.SmoothTangents(i, 0f);
                }
            }
            return curve1;
        }
        return null;
    }
    public AnimationCurve GetCurve4_RoaExceptY_Y(List<Vector4> animDatas_Roa4, List<float> animDatas_Time_roa, bool smooth = true)
    {
        if (animDatas_Roa4.Count > 0)
        {
            AnimationCurve curve1 = new AnimationCurve();
            for (int i = 0; i < animDatas_Roa4.Count; i++)
            {
                Vector3 roa1 = (new Quaternion(animDatas_Roa4[i].x, animDatas_Roa4[i].y, animDatas_Roa4[i].z, animDatas_Roa4[i].w)).eulerAngles;
                roa1.y = 0f;
                Quaternion q1 = Quaternion.Euler(roa1);
                curve1.AddKey(animDatas_Time_roa[i], q1.y);
              
            }
            if (smooth)
            {
                for (int i = 0; i < curve1.keys.Length; i++)
                {
                    curve1.SmoothTangents(i, 0f);
                }
            }
            return curve1;
        }
        return null;
    }
    public AnimationCurve GetCurve4_RoaExceptY_Z(List<Vector4> animDatas_Roa4, List<float> animDatas_Time_roa, bool smooth = true)
    {
        if (animDatas_Roa4.Count > 0)
        {
            AnimationCurve curve1 = new AnimationCurve();
            for (int i = 0; i < animDatas_Roa4.Count; i++)
            {
                Vector3 roa1 = (new Quaternion(animDatas_Roa4[i].x, animDatas_Roa4[i].y, animDatas_Roa4[i].z, animDatas_Roa4[i].w)).eulerAngles;
                roa1.y = 0f;
                Quaternion q1 = Quaternion.Euler(roa1);
                curve1.AddKey(animDatas_Time_roa[i], q1.z);
             
            }
            if (smooth)
            {
                for (int i = 0; i < curve1.keys.Length; i++)
                {
                    curve1.SmoothTangents(i, 0f);
                }
            }
            return curve1;
        }
        return null;
    }
    public AnimationCurve GetCurve4_RoaExceptY_W(List<Vector4> animDatas_Roa4, List<float> animDatas_Time_roa, bool smooth = true)
    {
        if (animDatas_Roa4.Count > 0)
        {
            AnimationCurve curve1 = new AnimationCurve();
            for (int i = 0; i < animDatas_Roa4.Count; i++)
            {
                Vector3 roa1 = (new Quaternion(animDatas_Roa4[i].x, animDatas_Roa4[i].y, animDatas_Roa4[i].z, animDatas_Roa4[i].w)).eulerAngles;
                roa1.y = 0f;
                Quaternion q1 = Quaternion.Euler(roa1);
                curve1.AddKey(animDatas_Time_roa[i], q1.w);
             
            }
            if(smooth)
            {
                for (int i = 0; i < curve1.keys.Length; i++)
                {
                    curve1.SmoothTangents(i, 0f);
                }
            }
         
            return curve1;
        }
        return null;
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
