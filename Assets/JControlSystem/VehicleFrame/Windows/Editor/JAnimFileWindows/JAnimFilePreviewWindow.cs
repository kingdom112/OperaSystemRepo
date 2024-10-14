using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JAnimationSystem;

public class JAnimFilePreviewWindow : EditorWindow
{
    GUIStyle boxStyle;
    GUIStyle richTextStyle_Mid;
    GUIStyle richTextStyle_Left;
    GUIStyle headerStyle;
    GUIStyle smallHeaderStyle;
    GUIStyle lineStyle;

    GameObject ga;
    JAnimationData jAnimData;
    JAnimation_AutoSetBoneMatchData autoSetBoneMatchData;
    bool useBoneAdapt = false;

    bool useAutoSetMatch = true;
    bool dragPlayMode = false;
    float dragJindu = 0f;
    /// <summary>
    /// 只有点击了pre按钮之后才能正式播放动画
    /// </summary>
    bool predData = false;

    [MenuItem("JTools/JAnimFilePreviewer")]
    static void Open()
    {
        var win = EditorWindow.GetWindow<JAnimFilePreviewWindow>(false, "JAnimFilePreviewer", true);
        win.GenerateStyles();
        var icon = Resources.Load("Textures/jicon2") as Texture;
        win.titleContent = new GUIContent("JAnimFilePreviewer", icon);
        win.boneMatch = new BoneMatch_Preview();
    }

    BoneMatch_Preview boneMatch = new BoneMatch_Preview();
    Vector2 scrollPos = Vector2.zero;
    Vector2 changeTexPos = new Vector2(-1, -1);
    int showCurve = -1;
    int changeBoneType = -1;
    int boneMatchSelectT = -1;
    void OnGUI()
    {
        GUILayout.Box("JAnimFilePreviewer", boxStyle, GUILayout.Height(60), GUILayout.ExpandWidth(true));
        GUILayout.Box("You can use this tool to preview one JAnimFile.", richTextStyle_Mid, GUILayout.ExpandWidth(true));

        GUILayout.Space(10);
        DrawALine(3);
        GUILayout.Space(10);
        GUILayout.Label("BasicSets", headerStyle);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("theGameObject");
        GameObject ga1 = EditorGUILayout.ObjectField(ga, typeof(GameObject), true) as GameObject;
        if(ga1 != ga)
        {
            predData = false;
            ga = ga1;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("JAnimData");
        JAnimationData jAnimData1 = EditorGUILayout.ObjectField(jAnimData, typeof(JAnimationData), false) as JAnimationData;
        if(jAnimData1 != jAnimData)
        {
            predData = false;
            jAnimData = jAnimData1;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("UseBoneAdapt");
        useBoneAdapt = EditorGUILayout.Toggle(useBoneAdapt );
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(30);
        DrawALine(3);
        GUILayout.Space(15);
        if(isPlaying)
        {
            GUI.enabled = false;
            GUILayout.Toggle(dragPlayMode, "DragPlayMode", "Button");
            GUI.enabled = true;
        }
        else
        {
            dragPlayMode = GUILayout.Toggle(dragPlayMode, "DragPlayMode", "Button");
            if(dragPlayMode)
            {
                EditorGUILayout.BeginHorizontal();
                dragJindu = EditorGUILayout.Slider(dragJindu, 0f, 1f);
                EditorGUILayout.EndHorizontal();
            }
        }
        
        GUILayout.BeginHorizontal();
        if(dragPlayMode == false)
        {
            if (GUILayout.Button("Pre", "ButtonLeft"))
            {
                if (CanStartProcess)
                {
                    isPlaying = false;
                    initProcess(); 
                    
                }
                else
                {
                    Debug.LogWarning("Can't start Pre !!!");
                }
            }
            if(predData == false)
            {
                GUI.enabled = false;
            }
            if(isPlaying == true)
            {
                if (GUILayout.Button("Stop", "ButtonMid"))
                {
                    isPlaying = false; 
                }
            }
            else
            {
                if (GUILayout.Button("Start", "ButtonMid"))
                {
                    isPlaying = true;
                    processTimer = 0f;
                    processJindu = 0f;
                }
            }
            if (GUILayout.Button("Pause", "ButtonMid"))
            {
                isPlaying = !isPlaying; 
            }
            if (GUILayout.Button("Save to JAnimData", "ButtonRight"))
            {
                SaveDataToJanimdataAfterPre(ga, boneMatch, jAnimData);
            }
            GUI.enabled = true;
        }
        else
        {
            GUI.enabled = false;
            GUILayout.Button("Pre", "ButtonLeft");
            GUILayout.Button("Start", "ButtonMid");
            GUILayout.Button("Pause", "ButtonMid");
            GUILayout.Button("Save to JAnimData", "ButtonRight");
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();

        ShowProcessBar();

        GUILayout.Space(20);
        DrawALine(3);
        GUILayout.Space(10);
        GUILayout.Label("BoneMatch",headerStyle);
        useAutoSetMatch = EditorGUILayout.Toggle("UseAutoSetMatch", useAutoSetMatch);
        if (useAutoSetMatch)
        {
            autoSetBoneMatchData =
               EditorGUILayout.ObjectField("AutoSetBoneMatchData", autoSetBoneMatchData,
               typeof(JAnimation_AutoSetBoneMatchData), false) as JAnimation_AutoSetBoneMatchData;
            if (autoSetBoneMatchData != null)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("AutoMatchBones"))
                {
                    if (ga == null)
                    {
                        Debug.LogWarning("theGameObject is NONE !!!");
                    }
                    else
                    {
                        Debug.Log("AutoMatchBones");
                        boneMatch.StartAutoMatch(ga.transform, autoSetBoneMatchData, jAnimData != null ? jAnimData.jAD : null);
                    } 
                    //return;
                }
                GUILayout.EndHorizontal();
                if(boneMatch.boneMatchList.Count > 0)
                {
                    int usedCount = 0;
                    for(int jj=0; jj< boneMatch.boneMatchList.Count; jj++)
                    {
                        if(boneMatch.boneMatchList[jj].matchedT != null)
                        {
                            usedCount++;
                        }
                    }
                    GUILayout.Label("   (Used:<color=blue>" + usedCount + "</color>/" + boneMatch.boneMatchList.Count + ")", richTextStyle_Left);
                }
            }
        }

        DrawALine(2);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        for (int i = 0; i < boneMatch.boneMatchList.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("  " + (i + 1).ToString() + ": " + (boneMatch.boneMatchList[i].matchedT == null ? "<color=red>" : "<color=black>") + boneMatch.boneMatchList[i].boneType.ToString() + "</color>"
                , smallHeaderStyle);
            if (GUILayout.Button("ChangeBoneType", "ButtonLeft"))
            {
                changeBoneType = i;
            }
            if (GUILayout.Button("-", "ButtonRight"))
            {
                boneMatch.boneMatchList.RemoveAt(i);
                return;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            if (changeBoneType == i)
            {
                EditorGUILayout.BeginHorizontal();
                boneMatch.boneMatchList[i].boneType = (JAnimationData.BoneType)EditorGUILayout.EnumPopup("BoneType", boneMatch.boneMatchList[i].boneType);
                if (GUILayout.Button("OK"))
                {
                    changeBoneType = -1;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel++;
            string matchedTName = "Null";
            if (boneMatch.boneMatchList[i].matchedT != null)
            {
                matchedTName = boneMatch.boneMatchList[i].matchedT.name;
            }
            GUILayout.Label("  <color=green>[MatchedName]</color>: <color=blue>" +
               boneMatch.boneMatchNamesList[i].MatchNames.Count.ToString()+ "</color>", richTextStyle_Left);
            GUILayout.Label("  <color=green>[MatchedT]</color>: <color=blue>" + matchedTName+"</color>"
                , richTextStyle_Left);
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            if (boneMatchSelectT != i)
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField(boneMatch.boneMatchList[i].matchedT, typeof(Transform), true);
                GUI.enabled = true;
            }
            else
            {
                boneMatch.boneMatchList[i].matchedT = 
                    EditorGUILayout.ObjectField(boneMatch.boneMatchList[i].matchedT, typeof(Transform), true) as Transform;
            } 
            if (GUILayout.Button("SelectT"))
            {
                if(boneMatchSelectT == i)
                {
                    boneMatchSelectT = -1;
                }
                else 
                { 
                    boneMatchSelectT = i;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("  <color=green>[MatchNames]</color>:",richTextStyle_Left,  GUILayout.Width(200));
            if (GUILayout.Button("+",GUILayout.Width(80)))
            {
                boneMatch.boneMatchNamesList[i].AddMatchName("DefaultMatchName" + boneMatch.boneMatchNamesList[i].MatchNames.Count);
                
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            for (int j = 0; j < boneMatch.boneMatchNamesList[i].MatchNames.Count; j++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("   [<color=blue>" + boneMatch.boneMatchNamesList[i].MatchNames[j] + 
                    "</color>]",richTextStyle_Left);
                if (GUILayout.Button("Change", "ButtonLeft"))
                {
                    changeTexPos.x = i;
                    changeTexPos.y = j;
                }
                if (GUILayout.Button("-", "ButtonRight"))
                {
                    if(boneMatch.boneMatchNamesList[i].MatchNames.Count > 1)
                    {
                        boneMatch.boneMatchNamesList[i].MatchNames.RemoveAt(j);
                    }
                    else
                    {
                        Debug.LogWarning("You Can't Remove The Last Name !");
                    }
                }
                GUILayout.EndHorizontal();

                if (i == changeTexPos.x && j == changeTexPos.y)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("ChangeTo: ");
                    boneMatch.boneMatchNamesList[i].MatchNames[j] = GUILayout.TextField(boneMatch.boneMatchNamesList[i].MatchNames[j]);
                    if (GUILayout.Button("OK"))
                    {
                        changeTexPos = new Vector2(-1, -1);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.Space(5);
            if (showCurve != i)
            {
                if (GUILayout.Button("ShowCurve"))
                {
                    showCurve = i;
                }
            }
            else
            {
                if (GUILayout.Button("Close"))
                {
                    showCurve = -1;
                }

                boneMatch.SetJAnimationData(jAnimData,false, false);

                if (boneMatch.bonePlayers[i].Curve_X_4_Roa.keys.Length >= 2)
                {
                    GUILayout.Label("Roa4:", smallHeaderStyle);
                    EditorGUILayout.CurveField(boneMatch.bonePlayers[i].Curve_X_4_Roa);
                    EditorGUILayout.CurveField(boneMatch.bonePlayers[i].Curve_Y_4_Roa);
                    EditorGUILayout.CurveField(boneMatch.bonePlayers[i].Curve_Z_4_Roa);
                    EditorGUILayout.CurveField(boneMatch.bonePlayers[i].Curve_W_4_Roa);
                }
                /*else if (boneMatch.bonePlayers[i].Curve_X_3_Roa.keys.Length >= 2)
                {
                    GUILayout.Label("Roa3:", smallHeaderStyle);
                    EditorGUILayout.CurveField(boneMatch.bonePlayers[i].Curve_X_3_Roa);
                    EditorGUILayout.CurveField(boneMatch.bonePlayers[i].Curve_Y_3_Roa);
                    EditorGUILayout.CurveField(boneMatch.bonePlayers[i].Curve_Z_3_Roa);
                }*/
                if (boneMatch.bonePlayers[i].Curve_X_Pos.keys.Length >= 2)
                {
                    GUILayout.Label("Pos:", smallHeaderStyle);
                    EditorGUILayout.CurveField(boneMatch.bonePlayers[i].Curve_X_Pos);
                    EditorGUILayout.CurveField(boneMatch.bonePlayers[i].Curve_Y_Pos);
                    EditorGUILayout.CurveField(boneMatch.bonePlayers[i].Curve_Z_Pos);
                }
            }
            EditorGUI.indentLevel--;
            GUILayout.Space(20);
            DrawALine(1);
            GUILayout.Space(20);
           
            GUILayout.Space(10);
        }
        if (GUILayout.Button("+"))
        {
            boneMatch.AddNewMatch_BoneType(JAnimationData.BoneType.unknowType);
        }
        GUILayout.Space(30);
        EditorGUILayout.EndScrollView();
        DrawALine(2);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("StartMatch"))
        {
            if (ga == null)
            {
                Debug.LogWarning("theGameObject is NONE !!!");
            }
            else
            {
                boneMatch.StartMatch(ga.transform);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        // Updates and Repaint the UI in real time.
        Repaint();
    }

    
    private void initProcess()
    {
        boneMatch.SetJAnimationData(jAnimData,useBoneAdapt);
        predData = true;
    }

    float processTimer = 0f;
    float processJindu = 0f;
    bool isPlaying = false;
    float _lastTime = 0f;
    void Update()
    {
        var now = EditorApplication.timeSinceStartup;
        float delTime = (float)now - _lastTime;
        _lastTime = (float)now;

        if(dragPlayMode)
        {
            if(ga != null && jAnimData != null)
            {
                boneMatch.PlayDataInThisFrame_Curve(dragJindu * jAnimData.jAD.TimeLength);
            }
        }
        else
        {
            if (isPlaying)
            {
                if (processJindu <= 1f)
                {
                    processJindu = processTimer / jAnimData.jAD.TimeLength;
                    //Debug.Log(jAnimData.TimeLength);
                    boneMatch.PlayDataInThisFrame_Curve(processTimer);
                    processTimer += delTime;
                }
                else
                {
                    isPlaying = false;
                }
            }
        }
    }


    void DrawALine(int _height)
    {
        GUILayout.Box("", lineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(_height));
    }
    void ShowProcessBar()
    {
        if (Event.current.type == EventType.Repaint)
        {
            var lastRect = GUILayoutUtility.GetLastRect();
            EditorGUI.ProgressBar(new Rect(lastRect.x, lastRect.y + lastRect.height, lastRect.width, 20), Mathf.Clamp01(processJindu), "");
        }
        GUILayout.Space(20);
    }

    /// <summary>
    /// Can Start Process ???
    /// </summary>
    private bool CanStartProcess
    {
        get
        {
            for (int i = 0; i < boneMatch.boneMatchList.Count; i++)
            {
                if (boneMatch.boneMatchList[i].matchedT == null)
                {
                    Debug.LogWarning("Each Match Of BoneMatch Must Has A MatchedTransform !  " +
                        "The boneMatch["+(i+1)+"].matchedT is null !!!");
                    //return false;
                }
                if(boneMatch.boneMatchList[i].boneType == JAnimationData.BoneType.unknowType)
                {
                    Debug.LogWarning("Each Match Of BoneMatch Must't Be  UnknowType !  " +
                        "The boneMatch[" + (i + 1) + "].boneType is unknowType !!!");
                    return false;
                }
            }
            if (ga != null && jAnimData != null)
            {
                return true;
            }
            else
            {
                Debug.LogWarning("[theGameObject] and the [JAnimData] must have value ! ");
                return false;
            }
        }
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



    /// <summary>
    /// 把转化完的数据生成动画文件
    /// </summary>
    /// <param name="_animclip"></param>
    public void SaveDataToJanimdataAfterPre(GameObject targetG,BoneMatch_Preview boneMatch1,JAnimationData janimData1, JAnimationData.RoaSpaceType saveType = JAnimationData.RoaSpaceType.local)
    {
        if (targetG == null)
        {
            Debug.LogError("targetG Can't Be Null !!!");
            return;
        }
        if(boneMatch1 == null)
        {
            Debug.LogError("boneMatch1 Can't Be Null !!!");
            return; 
        }
        if(janimData1 == null)
        {
            Debug.LogError("janimData1 Can't Be Null !!!");
            return;
        }
        if (boneMatch1.bonePlayers.Count <= 0)
        {
            Debug.LogWarning("No Data can Trans!!!!");
            return;
        }
        //将JAnimationData创建为asset,为没有_data而准备一个要生成的数据
        JAnimationData asset = ScriptableObject.CreateInstance<JAnimationData>();
        asset.jAD.roaSpaceType = JAnimationData.RoaSpaceType.local;
        BoneTPoseMarker marker1 = null;
        for (int k3 = 0; k3 < boneMatch1.bonePlayers.Count; k3++)
        {
            if (boneMatch1.bonePlayers[k3].boneOneMatch != null)
            {
                if (boneMatch1.bonePlayers[k3].boneOneMatch.matchedT != null)
                {
                    marker1 = boneMatch1.GetTPoseMarker(boneMatch1.bonePlayers[k3].boneOneMatch.matchedT);
                    break;
                }
            }
        }
        if (marker1 == null)
        {
            Debug.LogWarning("no marker!!! can't get T Pose and framework from marker !!!");

        }

        //---setValues-----------------------------------------------------
        for (int i = 0; i < boneMatch1.bonePlayers.Count; i++)
        {
            if (boneMatch1.bonePlayers[i].boneOneMatch != null)
            {
                if (boneMatch1.bonePlayers[i].boneOneMatch.matchedT != null)
                {
                    for (int j = 0; j < boneMatch1.bonePlayers[i].animDatas_Roa4.Count; j++)
                    {
                        boneMatch1.PlayDataInThisFrame_Curve(boneMatch1.bonePlayers[i].animDatas_Time_roa[j]);
                        /*asset.jAD.AddData_roa4(boneMatch1.bonePlayers[i].boneOneMatch.boneType,
                            boneMatch1.bonePlayers[i].animDatas_Roa4[j],
                            boneMatch1.bonePlayers[i].animDatas_Time_roa[j]);*/
                        Vector4 roaData1 = Vector4.zero;
                        if(saveType == JAnimationData.RoaSpaceType.local)
                        {
                            Quaternion q1 = boneMatch1.bonePlayers[i].boneOneMatch.matchedT.localRotation;
                            roaData1 = new Vector4(q1.x, q1.y, q1.z, q1.w);
                        }else if(saveType == JAnimationData.RoaSpaceType.world)
                        {
                            Quaternion q1 = boneMatch1.bonePlayers[i].boneOneMatch.matchedT.rotation;
                            roaData1 = new Vector4(q1.x, q1.y, q1.z, q1.w);
                        }
                        else
                        {
                            Debug.LogError("ERROR!!");
                        }
                        asset.jAD.AddData_roa4(boneMatch1.bonePlayers[i].boneOneMatch.boneType
                            , boneMatch1.bonePlayers[i].animDatas_Time_roa[j], roaData1    );

                    }
                    for (int k = 0; k < boneMatch1.bonePlayers[i].animDatas_Time_pos.Count; k++)
                    {
                        asset.jAD.AddData_pos(boneMatch1.bonePlayers[i].boneOneMatch.boneType
                            , boneMatch1.bonePlayers[i].animDatas_Time_pos[k], boneMatch1.bonePlayers[i].animDatas_Pos[k]
                            );
                    }
                }
            }
        }

        string saveFileName = "";
        saveFileName = "trans_" + janimData1.name + "_" + targetG.name + ".asset";

        if (boneMatch1.bonePlayers.Count > 0)
            Debug.Log("Saved roa and pos Datas.("+ saveFileName+")");

        if(marker1 != null)
        {
            if (marker1.tPoseData != null)
            {
                if (marker1.tPoseData.dataList.Count > 0)
                {
                    asset.tPoseData = marker1.tPoseData;
                    Debug.Log("Saved tPoseData from BoneTPoseMarker.(" + saveFileName + ")");
                }
            }
            if (marker1.boneFramework != null)
            {
                if (marker1.boneFramework.Bones.Count > 0)
                {
                    asset.boneFramework = marker1.boneFramework;
                    Debug.Log("Saved boneFramework from BoneTPoseMarker.(" + saveFileName + ")");
                }
            }
        } 

        {
            AssetDatabase.CreateAsset(asset, "Assets/Resources/JAnimDatas/" + saveFileName  );
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;

            Debug.Log("Saved (<color=blue>" + saveType.ToString() + "</color>) Data To [" + saveFileName + "].");
        }
        Debug.Log("Process Finish ! ! !");
    }



}


/*
public class BoneMatch_Preview
{

    public class BoneOneMatch
    {
        JAnimationData.BoneType boneType;
        public bool RoaIs3Not4 = true;
        public List<string> MatchNames = new List<string>();
        Transform matchedT = null;
        public List<Vector3> animDatas_Roa3 = new List<Vector3>();
        public List<Vector4> animDatas_Roa4 = new List<Vector4>();
        public List<Vector3> animDatas_Pos = new List<Vector3>();
        public List<float> animDatas_Time = new List<float>();

        public AnimationCurve Curve_X_3_Roa = new AnimationCurve();
        public AnimationCurve Curve_Y_3_Roa = new AnimationCurve();
        public AnimationCurve Curve_Z_3_Roa = new AnimationCurve();
        public AnimationCurve Curve_X_4_Roa = new AnimationCurve();
        public AnimationCurve Curve_Y_4_Roa = new AnimationCurve();
        public AnimationCurve Curve_Z_4_Roa = new AnimationCurve();
        public AnimationCurve Curve_W_4_Roa = new AnimationCurve();
        public AnimationCurve Curve_X_Pos = new AnimationCurve();
        public AnimationCurve Curve_Y_Pos = new AnimationCurve();
        public AnimationCurve Curve_Z_Pos = new AnimationCurve();

        public BoneOneMatch(JAnimationData.BoneType _boneType, string _matchName1)
        {
            boneType = _boneType;
            RoaIs3Not4 = true;
            MatchNames = new List<string>();
            MatchNames.Add(_matchName1);
            matchedT = null;
            animDatas_Roa3 = new List<Vector3>();
            animDatas_Roa4 = new List<Vector4>();
            animDatas_Pos = new List<Vector3>();
            animDatas_Time = new List<float>();

            Curve_X_3_Roa = new AnimationCurve();
            Curve_Y_3_Roa = new AnimationCurve();
            Curve_Z_3_Roa = new AnimationCurve();
            Curve_X_4_Roa = new AnimationCurve();
            Curve_Y_4_Roa = new AnimationCurve();
            Curve_Z_4_Roa = new AnimationCurve();
            Curve_W_4_Roa = new AnimationCurve();
            Curve_X_Pos = new AnimationCurve();
            Curve_Y_Pos = new AnimationCurve();
            Curve_Z_Pos = new AnimationCurve();
        }
        public BoneOneMatch(JAnimationData.BoneType _boneType, List<string> _matchNames)
        {
            boneType = _boneType;
            RoaIs3Not4 = true;
            MatchNames = new List<string>();
            MatchNames = _matchNames;
            matchedT = null;
            animDatas_Roa3 = new List<Vector3>();
            animDatas_Roa4 = new List<Vector4>();
            animDatas_Pos = new List<Vector3>();
            animDatas_Time = new List<float>();

            Curve_X_3_Roa = new AnimationCurve();
            Curve_Y_3_Roa = new AnimationCurve();
            Curve_Z_3_Roa = new AnimationCurve();
            Curve_X_4_Roa = new AnimationCurve();
            Curve_Y_4_Roa = new AnimationCurve();
            Curve_Z_4_Roa = new AnimationCurve();
            Curve_W_4_Roa = new AnimationCurve();
            Curve_X_Pos = new AnimationCurve();
            Curve_Y_Pos = new AnimationCurve();
            Curve_Z_Pos = new AnimationCurve();
        }
        public JAnimationData.BoneType BoneType
        {
            get
            {
                return boneType;
            }
            set
            {
                boneType = value;
            }
        }
        public Transform MatchedT
        {
            get
            {
                return matchedT;
            }
        }
        public void PlayInThisFrame_Curve(float timeNow)
        {
            if(RoaIs3Not4)
            {
                if (Curve_X_3_Roa.keys.Length >= 2 && 
                    Curve_Y_3_Roa.keys.Length >= 2 && 
                    Curve_Z_3_Roa.keys.Length >= 2)
                {
                    Quaternion q1 = Quaternion.Euler(
                        Curve_X_3_Roa.Evaluate(timeNow), 
                        Curve_Y_3_Roa.Evaluate(timeNow), 
                        Curve_Z_3_Roa.Evaluate(timeNow));
                    matchedT.localRotation = Quaternion.Slerp(matchedT.localRotation, q1, 1f);
                }
            }
            else
            {
                if (Curve_X_4_Roa.keys.Length >= 2 &&
                    Curve_Y_4_Roa.keys.Length >= 2 &&
                    Curve_Z_4_Roa.keys.Length >= 2 &&
                    Curve_W_4_Roa.keys.Length >= 2)
                {
                    Quaternion q1 = new Quaternion(
                        Curve_X_4_Roa.Evaluate(timeNow), 
                        Curve_Y_4_Roa.Evaluate(timeNow), 
                        Curve_Z_4_Roa.Evaluate(timeNow), 
                        Curve_W_4_Roa.Evaluate(timeNow));
                    matchedT.localRotation = Quaternion.Slerp(matchedT.localRotation, q1, 1f);
                }
            }
           
            if (Curve_X_Pos.keys.Length >= 2 && Curve_Y_Pos.keys.Length >= 2 && Curve_Z_Pos.keys.Length >= 2)
            {
                Vector3 v3 = new Vector3(Curve_X_Pos.Evaluate(timeNow), Curve_Y_Pos.Evaluate(timeNow), Curve_Z_Pos.Evaluate(timeNow));
                matchedT.localPosition = v3;
            } 
        }
        public void SetAnimDatas (List<Vector3> _animDatas_Roa3, List<Vector3> _animDatas_Pos
            , List<float> _animDatas_Time_Pos, List<float> _animDatas_Time_Roa)
        {
            RoaIs3Not4 = true;
            animDatas_Roa3 = _animDatas_Roa3;
            animDatas_Pos = _animDatas_Pos;
            Curve_X_3_Roa = new AnimationCurve();
            Curve_Y_3_Roa = new AnimationCurve();
            Curve_Z_3_Roa = new AnimationCurve();
            for (int i=0; i< _animDatas_Roa3.Count; i++)
            {
                Curve_X_3_Roa.AddKey(_animDatas_Time_Roa[i], _animDatas_Roa3[i].x);
                Curve_Y_3_Roa.AddKey(_animDatas_Time_Roa[i], _animDatas_Roa3[i].y);
                Curve_Z_3_Roa.AddKey(_animDatas_Time_Roa[i], _animDatas_Roa3[i].z);
            }
            Curve_X_Pos = new AnimationCurve();
            Curve_Y_Pos = new AnimationCurve();
            Curve_Z_Pos = new AnimationCurve();
            for (int i = 0; i < _animDatas_Pos.Count; i++)
            {
                Curve_X_Pos.AddKey(_animDatas_Time_Pos[i], _animDatas_Pos[i].x);
                Curve_Y_Pos.AddKey(_animDatas_Time_Pos[i], _animDatas_Pos[i].y);
                Curve_Z_Pos.AddKey(_animDatas_Time_Pos[i], _animDatas_Pos[i].z);
            }
        }
        public void SetAnimDatas(List<Vector4> _animDatas_Roa4, List<Vector3> _animDatas_Pos
            , List<float> _animDatas_Time_Pos, List<float> _animDatas_Time_Roa)
        {
            RoaIs3Not4 = false;
            animDatas_Roa4 = _animDatas_Roa4;
            animDatas_Pos = _animDatas_Pos;
            Curve_X_4_Roa = new AnimationCurve();
            Curve_Y_4_Roa = new AnimationCurve();
            Curve_Z_4_Roa = new AnimationCurve();
            Curve_W_4_Roa = new AnimationCurve();
            for (int i = 0; i < _animDatas_Roa4.Count; i++)
            {
                Curve_X_4_Roa.AddKey(_animDatas_Time_Roa[i], _animDatas_Roa4[i].x);
                Curve_Y_4_Roa.AddKey(_animDatas_Time_Roa[i], _animDatas_Roa4[i].y);
                Curve_Z_4_Roa.AddKey(_animDatas_Time_Roa[i], _animDatas_Roa4[i].z);
                Curve_W_4_Roa.AddKey(_animDatas_Time_Roa[i], _animDatas_Roa4[i].w);
            }
            Curve_X_Pos = new AnimationCurve();
            Curve_Y_Pos = new AnimationCurve();
            Curve_Z_Pos = new AnimationCurve();
            for (int i = 0; i < _animDatas_Pos.Count; i++)
            {
                Curve_X_Pos.AddKey(_animDatas_Time_Pos[i], _animDatas_Pos[i].x);
                Curve_Y_Pos.AddKey(_animDatas_Time_Pos[i], _animDatas_Pos[i].y);
                Curve_Z_Pos.AddKey(_animDatas_Time_Pos[i], _animDatas_Pos[i].z);
            }
        }
        public void SetMatchTransform(Transform _matchT)
        {
            matchedT = _matchT;
        }
        public void SetBoneType(JAnimationData.BoneType _boneType)
        {
            boneType = _boneType;
        }
        public void AddMatchName(string _matchName)
        {
            if (MatchNames.Contains(_matchName) == false)
            {
                MatchNames.Add(_matchName);
            }
        }
    }

    public List<BoneOneMatch> boneMatchList = new List<BoneOneMatch>();

    public BoneMatch_Preview()
    {

    }

    public void SetJAnimationData (JAnimationData _data)
    {
        for(int i=0;i<_data.dataList.Count; i++)
        {
            int _index = GetIndexOfBoneType(_data.dataList[i].boneType);
            if(_index != -1)
            {
                if(_data.RoaIs3Not4)
                {
                    boneMatchList[_index].SetAnimDatas(_data.dataList[i].V3List_roa3,
                        _data.dataList[i].V3List_pos, _data.dataList[i].timeList_pos, _data.dataList[i].timeList_roa);
                }
                else
                {
                    boneMatchList[_index].SetAnimDatas(_data.dataList[i].V4List_roa4,
                       _data.dataList[i].V3List_pos, _data.dataList[i].timeList_pos, _data.dataList[i].timeList_roa);
                }
               
            }
            else
            {
                Debug.LogWarning("One Bone is Not Match To This GameObejct ! ");
            }
        }
    }
    
    public void PlayDataInThisFrame_Curve(float _timeNow)
    {
        for (int i = 0; i < boneMatchList.Count; i++)
        {
            boneMatchList[i].PlayInThisFrame_Curve(_timeNow);
        }
    }

    public void ClearBoneMatchList()
    {
        boneMatchList.Clear();
    }

    public void StartAutoMatch(Transform _target, JAnimation_AutoSetBoneMatchData _autoMatchData, JAnimationData _JAnimData)
    {
        if (_target == null)
        {
            Debug.LogWarning("theGameObject Can't Be Null !");
            return;
        }
        if (_autoMatchData == null)
        {
            Debug.LogWarning("AutoSetBoneMatchData Can't Be Null !");
            return;
        }
        if (_JAnimData == null)
        {
            Debug.LogWarning("JAnimData Can't Be Null !");
            return;
        }
        Transform[] _transformList = _target.GetComponentsInChildren<Transform>();
        List<Transform> transformList1 = new List<Transform>();
        for (int k = 0; k < _transformList.Length; k++)
        {
            transformList1.Add(_transformList[k]);
        }
        transformList1.Sort(delegate (Transform x, Transform y)
        {
            return y.name.CompareTo(x.name);
        });//sort
        ClearBoneMatchList();
        for(int j=0; j< _JAnimData.dataList.Count; j++)
        {
            JAnimationData.BoneType _boneType1 =  _JAnimData.dataList[j].boneType;
            List<string> keyWords = _autoMatchData.GetKeyWords(_boneType1);
            boneMatchList.Add(new BoneOneMatch(_boneType1, keyWords));
            for (int i = 0; i < transformList1.Count; i++)
            {
                if(_autoMatchData.CanTextMatchOneKeyWord(transformList1[i].name, keyWords))//canMatchOneKeyWord
                {
                    boneMatchList[boneMatchList.Count - 1].SetMatchTransform(transformList1[i]);
                }
            }
        }
        
    }

    public void StartMatchInDefault(Transform _target)
    {
        UseDefaultMatchNames();
        StartMatch(_target);
    }

    public void StartMatch(Transform _target)
    {
        Transform[] transformList = _target.GetComponentsInChildren<Transform>();
        for (int i = 0; i < transformList.Length; i++)
        {
            int _matchedIndex = NameMatchOneBone(transformList[i].name);
            if (_matchedIndex == -1) continue;
            boneMatchList[_matchedIndex].SetMatchTransform(transformList[i]);
        }
    }
    public void UseDefaultMatchNames()
    {
        ClearBoneMatchList();
        AddMatch_BoneType(JAnimationData.BoneType.head, "head");
        AddMatch_BoneType(JAnimationData.BoneType.neck, "neck");
        AddMatch_BoneType(JAnimationData.BoneType.hips, "main");
        AddMatch_BoneType(JAnimationData.BoneType.spine, "spine_01");
        AddMatch_BoneType(JAnimationData.BoneType.chest, "spine_02");
        AddMatch_BoneType(JAnimationData.BoneType.shoulder_L, "l_shoulder");
        AddMatch_BoneType(JAnimationData.BoneType.shoulder_R, "r_shoulder");
        AddMatch_BoneType(JAnimationData.BoneType.upperArm_L, "l_upperArm");
        AddMatch_BoneType(JAnimationData.BoneType.upperArm_R, "r_upperArm");
        AddMatch_BoneType(JAnimationData.BoneType.lowerArm_L, "l_lowerArm");
        AddMatch_BoneType(JAnimationData.BoneType.lowerArm_R, "r_lowerArm");
        AddMatch_BoneType(JAnimationData.BoneType.hand_L, "l_hand");
        AddMatch_BoneType(JAnimationData.BoneType.hand_R, "r_hand");
        AddMatch_BoneType(JAnimationData.BoneType.upperLeg_L, "l_upperLeg");
        AddMatch_BoneType(JAnimationData.BoneType.upperLeg_R, "r_upperLeg");
        AddMatch_BoneType(JAnimationData.BoneType.lowerLeg_L, "l_lowerLeg");
        AddMatch_BoneType(JAnimationData.BoneType.lowerLeg_R, "r_lowerLeg");
        AddMatch_BoneType(JAnimationData.BoneType.foot_L, "l_foot");
        AddMatch_BoneType(JAnimationData.BoneType.foot_R, "r_foot");
    }
    public void UseDefaultMatchNames2()
    {
        ClearBoneMatchList();
        AddMatch_BoneType(JAnimationData.BoneType.head, "head");
        AddMatch_BoneType(JAnimationData.BoneType.neck, "neck");
        AddMatch_BoneType(JAnimationData.BoneType.hips, "main");
        AddMatch_BoneType(JAnimationData.BoneType.spine, "spine_01");
        AddMatch_BoneType(JAnimationData.BoneType.chest, "spine_02");
        AddMatch_BoneType(JAnimationData.BoneType.shoulder_L, "l_collar");
        AddMatch_BoneType(JAnimationData.BoneType.shoulder_R, "r_collar");
        AddMatch_BoneType(JAnimationData.BoneType.upperArm_L, "l_shoulder");
        AddMatch_BoneType(JAnimationData.BoneType.upperArm_R, "r_shoulder");
        AddMatch_BoneType(JAnimationData.BoneType.lowerArm_L, "l_elbow");
        AddMatch_BoneType(JAnimationData.BoneType.lowerArm_R, "r_elbow");
        AddMatch_BoneType(JAnimationData.BoneType.hand_L, "l_wrist");
        AddMatch_BoneType(JAnimationData.BoneType.hand_R, "r_wrist");
        AddMatch_BoneType(JAnimationData.BoneType.upperLeg_L, "l_hip");
        AddMatch_BoneType(JAnimationData.BoneType.upperLeg_R, "r_hip");
        AddMatch_BoneType(JAnimationData.BoneType.lowerLeg_L, "l_knee");
        AddMatch_BoneType(JAnimationData.BoneType.lowerLeg_R, "r_knee");
        AddMatch_BoneType(JAnimationData.BoneType.foot_L, "l_ankle");
        AddMatch_BoneType(JAnimationData.BoneType.foot_R, "r_ankle");
    }
    public void AddMatch_BoneType(JAnimationData.BoneType _boneType, string _matchName)
    {
        int _index = GetIndexOfBoneType(_boneType);
        if (_index == -1)
        {
            boneMatchList.Add(new BoneOneMatch(_boneType, _matchName));
            _index = boneMatchList.Count - 1;
        }
        boneMatchList[_index].AddMatchName(_matchName);
    }

    /// <summary>
    /// Return the matched index of bone in the boneMatchList. If the bones can't match the name , return -1.
    /// </summary>
    private int NameMatchOneBone(string _name)
    {
        for (int i = 0; i < boneMatchList.Count; i++)
        {
            for (int j = 0; j < boneMatchList[i].MatchNames.Count; j++)
            {
                if (_name.Contains(boneMatchList[i].MatchNames[j]) == true)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    private int GetIndexOfBoneType(JAnimationData.BoneType _boneType)
    {
        for (int i = 0; i < boneMatchList.Count; i++)
        {
            if (boneMatchList[i].BoneType == _boneType)
            {
                return i;
            }
        }
        return -1;
    }
}*/
