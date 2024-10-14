using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JAnimationSystem;


public class JAnimFileTransferWindow : EditorWindow
{
    GUIStyle boxStyle;
    GUIStyle richTextStyle_Mid;
    GUIStyle richTextStyle_Left;
    GUIStyle headerStyle;
    GUIStyle smallHeaderStyle;
    GUIStyle lineStyle;

    GameObject ga = null;
    GameObject targetGA = null;
    AnimationClip animClip;
    JAnimation_AutoSetBoneMatchData autoSetBoneMatchData;
    /// <summary>
    /// 除了hip以外的骨骼都不使用位移曲线
    /// </summary>
    bool dontUsePosAnimationExceptHip = true;

    /// <summary>
    /// 使用跟随转化方式
    /// </summary>
    bool UseFollowTrans = false;
    /// <summary>
    /// 将动画Y轴的旋转固定到0度
    /// </summary>
    bool followTrans_LockYto0 = false;
    JAnimation_AutoSetBoneMatchData autoSetBoneMatchData_Target;
    /// <summary>
    /// 保存文件时，把旋转变换到世界坐标系
    /// </summary>
    bool turnRoaToWorldSpace = false;
    /// <summary>
    /// 文件保存名称。为空时自动生成名字
    /// </summary>
    string fileSaveName = "";

    bool useAutoSetMatch = false;
    bool dragPlayMode = false;
    float dragJindu = 0f;
    int boneMatchPageNow = 0;

    [MenuItem("JTools/JAnimFileTransfer")]
    static void Open()
    {
        var win = EditorWindow.GetWindow<JAnimFileTransferWindow>(false, "JAnimFileTransfer", true);
        win.GenerateStyles();
        var icon = Resources.Load("Textures/jicon2") as Texture;
        win.titleContent = new GUIContent("JAnimFileTransfer", icon);
        win.boneMatch = new AnimationBoneMatch();
    }



    AnimationBoneMatch boneMatch = new AnimationBoneMatch();
    Vector2 scrollPos_BoneMatches = Vector2.zero;
    Vector2 scrollPos_BoneMatchPage = Vector2.zero;
    bool showAnimationClipBindings = false;
    int showCurveIndex = -1;
    BoneMatch match_source = null;
    BoneMatch match_target = null;
    BoneTPoseMarker tposeMarker_s = null;
    BoneTPoseMarker tposeMarker_t = null; 
    void OnGUI()
    { 
        GUILayout.Box("JAnimFileTransfer", boxStyle, GUILayout.Height(60), GUILayout.ExpandWidth(true));
        GUILayout.Box("You can use this tool to change the AnimationClip into one JAnimFile.", richTextStyle_Mid, GUILayout.ExpandWidth(true));

        GUILayout.Space(10);
        DrawALine(3);
        GUILayout.Space(10);
        GUILayout.Label("BasicSets", headerStyle);
        ga = EditorGUILayout.ObjectField("theGameObject", ga, typeof(GameObject), true) as GameObject;
        AnimationClip animClipTemp = EditorGUILayout.ObjectField("theAnimationClip", animClip, typeof(AnimationClip), false) as AnimationClip;
        if(animClipTemp != animClip)
        {
            animClip = animClipTemp;
            boneMatch = new AnimationBoneMatch();
            fileSaveName = "";
        }
        isLooping = EditorGUILayout.Toggle(new GUIContent("Loop", "循环播放"), isLooping);
       

        UseFollowTrans = EditorGUILayout.Toggle(new GUIContent("UseFollowTrans", "使用跟随转换模式"), UseFollowTrans);
        if(UseFollowTrans)
        {
            targetGA = EditorGUILayout.ObjectField("TargetGA", targetGA, typeof(GameObject), true) as GameObject;
            autoSetBoneMatchData =
                 EditorGUILayout.ObjectField("AutoSetBoneMatchData_Source", autoSetBoneMatchData,
                 typeof(JAnimation_AutoSetBoneMatchData), false) as JAnimation_AutoSetBoneMatchData;
            autoSetBoneMatchData_Target =
                 EditorGUILayout.ObjectField("AutoSetBoneMatchData_Target", autoSetBoneMatchData_Target,
                 typeof(JAnimation_AutoSetBoneMatchData), false) as JAnimation_AutoSetBoneMatchData;
            followTrans_LockYto0 = EditorGUILayout.Toggle("Lock Y to 0", followTrans_LockYto0);
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("DontUsePosAnimationExceptHip", "除了Hip以外的骨骼都不使用位移曲线"));
            dontUsePosAnimationExceptHip = EditorGUILayout.Toggle("", dontUsePosAnimationExceptHip);
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(40);

        DrawALine(3);
        GUILayout.Space(10);

        if (isPlaying)
        {
            GUI.enabled = false;
            GUILayout.Toggle(dragPlayMode, new GUIContent("DragPlayMode"), "Button");
            GUI.enabled = true;
        }
        else
        {
            dragPlayMode = GUILayout.Toggle(dragPlayMode, "DragPlayMode", "Button");
            if (dragPlayMode)
            {
                EditorGUILayout.BeginHorizontal();
                dragJindu = EditorGUILayout.Slider(dragJindu, 0f, 1f);
                EditorGUILayout.EndHorizontal();
            }
        }
        GUILayout.BeginHorizontal();
        if(dragPlayMode)
        {
            GUI.enabled = false;
            GUILayout.Button("Play", "ButtonLeft");
            GUILayout.Button("Pause", "ButtonMid");
            GUILayout.Button("Stop", "ButtonRight");
            GUI.enabled = true;
        }
        else
        {
            if(GUILayout.Button("Play","ButtonLeft"))
            {
                if(CanStartPlay)
                {
                    initPlay();
                }
            }
            if (GUILayout.Button("Pause", "ButtonMid"))
            {
                if (CanStartPlay)
                {
                    isPlaying = !isPlaying;
                }
            }
            if (GUILayout.Button("Stop", "ButtonRight"))
            {
                isPlaying = false;
                playJindu = 0f;
                playTimer = 0f;
            }
        }
        GUILayout.EndHorizontal();

        ShowProcessBar();


        GUILayout.Space(40);

        DrawALine(3);
        GUILayout.Space(10);

        if(UseFollowTrans == false)
        {
            GUILayout.Label("BoneMatch", headerStyle);
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            showAnimationClipBindings = EditorGUILayout.Toggle(showAnimationClipBindings, GUILayout.Width(20));
            EditorGUILayout.LabelField("ShowAnimationClipBindings");
            if (animClip != null)
            {
                int useCount = 0;
                for (int c_c = 0; c_c < boneMatch.useList.Count; c_c++)
                {
                    if (boneMatch.useList[c_c] == true)
                    {
                        useCount++;
                    }
                }
                EditorGUILayout.LabelField("(used:<color=blue>" + useCount + "</color>)", richTextStyle_Left);
            }
            EditorGUILayout.EndHorizontal();

            if (showAnimationClipBindings && animClip != null)
            {
                scrollPos_BoneMatchPage = EditorGUILayout.BeginScrollView(scrollPos_BoneMatchPage,
             GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                GUILayout.BeginHorizontal();
                int bindCount = AnimationUtility.GetCurveBindings(animClip).Length;
                int pageTotal = (bindCount / 10) * 10 == bindCount ? bindCount / 10 : bindCount / 10 + 1;
                int pageLeft = 0;
                int pageRight = 0;
                boneMatchPageNow = Mathf.Clamp(boneMatchPageNow, 0, pageTotal - 1);
                if (boneMatchPageNow - 5 >= 0)
                {
                    pageLeft = boneMatchPageNow - 5;
                }
                else
                {
                    pageLeft = 0;
                }
                if (boneMatchPageNow + 5 < pageTotal)
                {
                    pageRight = boneMatchPageNow + 5;
                }
                else
                {
                    pageRight = pageTotal - 1;
                }
                GUILayout.Label("Page:", GUILayout.Width(40));
                boneMatchPageNow =
                   Mathf.Clamp(EditorGUILayout.IntField(boneMatchPageNow + 1), 1, pageTotal) - 1;
                if (GUILayout.Button("<", "ButtonLeft"))
                {
                    if (boneMatchPageNow - 1 >= 0)
                    {
                        boneMatchPageNow--;
                    }
                }
                for (int i = pageLeft; i <= pageRight; i++)
                {
                    if (GUILayout.Button((i + 1).ToString(), "ButtonMid"))
                    {
                        boneMatchPageNow = i;
                    }
                }
                if (GUILayout.Button(">", "ButtonRight"))
                {
                    if (boneMatchPageNow + 1 < pageTotal)
                    {
                        boneMatchPageNow++;
                    }
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
            }


            DrawALine(2);

            scrollPos_BoneMatches = EditorGUILayout.BeginScrollView(scrollPos_BoneMatches,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            GUILayout.Space(10);

            if (showAnimationClipBindings)
            {
                if (animClip != null)
                {
                    EditorCurveBinding[] binds = AnimationUtility.GetCurveBindings(animClip);
                    boneMatch.ReSetSize(binds.Length);
                    for (int z = boneMatchPageNow * 10;
                        z <= Mathf.Clamp(boneMatchPageNow * 10 + 10, 0, binds.Length - 1); z++)
                    {
                        GUILayout.Label("Index: " + (z + 1).ToString() + (boneMatch.useList[z] ? "    (<color=green>USED!!</color>)" : ""), smallHeaderStyle);
                        EditorGUI.indentLevel += 1;
                        GUILayout.Label(" <color=green>[propertyName]</color>: <color=blue>"
                           + binds[z].propertyName + "</color>", richTextStyle_Left);
                        GUILayout.Label(" <color=green>[path]</color>: <color=blue>" + binds[z].path
                           + "</color>", richTextStyle_Left);

                        if (showCurveIndex != z)
                        {
                            if (GUILayout.Button("ShowCurve", GUILayout.Width(100)))
                            {
                                showCurveIndex = z;
                            }
                        }

                        if (showCurveIndex == z)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.CurveField("Curve", AnimationUtility.GetEditorCurve(animClip, binds[z])
                                , GUILayout.Height(70)
                                , GUILayout.MaxWidth(500));
                            if (GUILayout.Button("Close", GUILayout.Width(80)))
                            {
                                showCurveIndex = -1;
                            }
                            GUILayout.EndHorizontal();
                        }

                        boneMatch.useList[z] = EditorGUILayout.Toggle("UseThisCurve", boneMatch.useList[z]);
                        if (boneMatch.useList[z])
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("BoneType:", GUILayout.Width(80));
                            boneMatch.boneTypeList[z] =
                                (JAnimationData.BoneType)EditorGUILayout.EnumPopup(boneMatch.boneTypeList[z]
                                , GUILayout.MaxWidth(200));
                            GUILayout.Space(40);
                            EditorGUILayout.LabelField("xyzType:", GUILayout.Width(70));
                            boneMatch.xyzTypeList[z] =
                                (AnimationBoneMatch.XYZType)EditorGUILayout.EnumPopup(boneMatch.xyzTypeList[z]
                                 , GUILayout.MaxWidth(180));
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUI.indentLevel -= 1;
                        GUILayout.Space(5);
                        DrawALine(1);
                        GUILayout.Space(40);
                    }
                }
            }
            EditorGUILayout.EndScrollView();
            DrawALine(2);

            GUILayout.Space(10);
            useAutoSetMatch = EditorGUILayout.Toggle("UseAutoSetMatch", useAutoSetMatch);
            if (useAutoSetMatch)
            {
                autoSetBoneMatchData =
                    EditorGUILayout.ObjectField("AutoSetBoneMatchData", autoSetBoneMatchData,
                    typeof(JAnimation_AutoSetBoneMatchData), false) as JAnimation_AutoSetBoneMatchData;
                if (autoSetBoneMatchData != null)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("AutoSetMatch_Roa3"))
                    {
                        boneMatch.AutoSetBoneMatch(animClip, autoSetBoneMatchData, true, dontUsePosAnimationExceptHip);
                        return;
                    }
                    if (GUILayout.Button("AutoSetMatch_Roa4"))
                    {
                        boneMatch.AutoSetBoneMatch(animClip, autoSetBoneMatchData, false, dontUsePosAnimationExceptHip);
                        return;
                    }
                    GUILayout.EndHorizontal();
                    turnRoaToWorldSpace = EditorGUILayout.Toggle("TurnRoaToWorldSpace", turnRoaToWorldSpace);
                }
            }
            else
            {
                turnRoaToWorldSpace = false;
            }
        }
        else
        {
            //use follow trans == true
            if (GUILayout.Button("Pre"))
            {
                if(ga != null && targetGA != null && autoSetBoneMatchData != null && autoSetBoneMatchData_Target != null
                     && ga.GetComponent<BoneTPoseMarker>() != null && targetGA.GetComponent<BoneTPoseMarker>() != null)
                {
                    match_source = new BoneMatch();
                    match_source.StartAutoMatch(ga.transform, autoSetBoneMatchData); 
                    match_target = new BoneMatch();
                    match_target.StartAutoMatch(targetGA.transform, autoSetBoneMatchData_Target);
                    tposeMarker_s = ga.GetComponent<BoneTPoseMarker>();
                    tposeMarker_t = targetGA.GetComponent<BoneTPoseMarker>();
                    Debug.Log("pre!!!");
                }
            }
        }
        
        fileSaveName = EditorGUILayout.TextField("FileSaveName", fileSaveName);
        if (GUILayout.Button("Process"))
        {
            if(UseFollowTrans == false)
            {
                if (turnRoaToWorldSpace == false)
                {
                    boneMatch.TransData(fileSaveName, animClip);
                }
                else
                {
                    boneMatch.TransData_RoaInWorldSpace(fileSaveName, ga, animClip, autoSetBoneMatchData);
                }
            }
            else
            {
                if (match_source != null && match_target != null)
                {
                    JAnimationData asset = ScriptableObject.CreateInstance<JAnimationData>();
                    asset.jAD.roaSpaceType = JAnimationData.RoaSpaceType.local;
                    float totalTime = animClip.length;
                    float fps = 30f;
                    float jiange1 = 1f / fps;
                    float timer1 = 0f;
                    while(timer1 <= totalTime)
                    {
                        PlaySample(ga, timer1);
                        FollowPlay();
                        for (int i = 0; i < match_target.boneMatchList.Count; i++)
                        {  
                            if (match_target.boneMatchList[i].matchedT == null) continue;
                            Quaternion q1 = match_target.boneMatchList[i].matchedT.localRotation;
                            asset.jAD.AddData_roa4(match_target.boneMatchList[i].boneType,
                                timer1, new Vector4(q1.x, q1.y, q1.z, q1.w));
                            if(match_target.boneMatchList[i].boneType == JAnimationData.BoneType.hips)
                            {
                                asset.jAD.AddData_pos(match_target.boneMatchList[i].boneType
                                    , timer1, match_target.boneMatchList[i].matchedT.localPosition );
                            }
                        }

                        timer1 += jiange1; 
                        if (timer1 >= totalTime)
                        {
                            PlaySample(ga, totalTime);
                            FollowPlay();
                            for (int i = 0; i < match_target.boneMatchList.Count; i++)
                            {
                                if (match_target.boneMatchList[i].matchedT == null) continue;
                                Quaternion q1 = match_target.boneMatchList[i].matchedT.localRotation;
                                asset.jAD.AddData_roa4(match_target.boneMatchList[i].boneType,
                                    totalTime, new Vector4(q1.x, q1.y, q1.z, q1.w));
                                if (match_target.boneMatchList[i].boneType == JAnimationData.BoneType.hips)
                                {
                                    asset.jAD.AddData_pos(match_target.boneMatchList[i].boneType
                                        , totalTime, match_target.boneMatchList[i].matchedT.localPosition);
                                }
                            }
                            break;
                        }
                    }

                    string saveFileName = "";
                    if (string.IsNullOrEmpty(fileSaveName))
                    {
                        saveFileName = "JAnimD_" + animClip.name + "_" + "FollowTransed" + ".asset";
                    }
                    else
                    {
                        saveFileName = fileSaveName + ".asset";
                    } 

                    {
                        AssetDatabase.CreateAsset(asset, "Assets/Resources/JAnimDatas/" + saveFileName);
                        AssetDatabase.Refresh();
                        AssetDatabase.SaveAssets();

                        EditorUtility.FocusProjectWindow();

                        Selection.activeObject = asset;

                        Debug.Log("Saved (<color=blue>" + "Local" + "</color>) Data To [" + saveFileName + "](<color=blue>UseFollowTrans</color>).");
                    }
                    Debug.Log("Process Finish ! ! !");
                }

                

            }
          
        }
        GUILayout.Space(10);

        // Updates and Repaint the UI in real time.
        Repaint();
    }
    
    /// <summary>
    /// Can Start Play ???
    /// </summary>
    private bool CanStartPlay
    {
        get
        {

            if (ga != null && animClip != null )
            {
                return true;
            }
            else
            {
                Debug.LogWarning("[theGameObject] and [theAnimationClip] must have value ! ");
                return false;
            }
        }
    }


    private void FollowPlay()
    {
        if (match_source != null && match_target != null)
        {
            for (int i = 0; i < match_target.boneMatchList.Count; i++)
            {
                //Debug.Log("tttt 0");
                if (match_target.boneMatchList[i].matchedT == null) continue;
                //Debug.Log("tttt 1");
                JAnimationData.BoneType btype1 = match_target.boneMatchList[i].boneType;
                int index1 = match_source.GetIndexOfBoneType(btype1);
                if (index1 == -1) continue;
                //Debug.Log("tttt 2");
                if (match_source.boneMatchList[index1].matchedT == null) continue;
                //Debug.Log("tttt 3");
                JAnimationData.TPoseData.TPoseOneData tData_s = tposeMarker_s.tPoseData.GetDataByType(btype1);
                JAnimationData.TPoseData.TPoseOneData tData_t = tposeMarker_t.tPoseData.GetDataByType(btype1);
                if (tData_s == null) continue;
                //Debug.Log("tttt 4");
                if (tData_t == null) continue;
                //Debug.Log("tttt 5");
                Quaternion q_s = match_source.boneMatchList[index1].matchedT.rotation;
                Quaternion mult1 = Quaternion.Inverse(tData_s.roa_World) * tData_t.roa_World;
                match_target.boneMatchList[i].matchedT.rotation = q_s * mult1;
                //Debug.Log("tttt");
                if(btype1 == JAnimationData.BoneType.hips)
                {
                    match_target.boneMatchList[i].matchedT.localPosition = match_source.boneMatchList[index1].matchedT.localPosition;
                }
            }
        }
    }

    private void PlaySample(GameObject _go, float _time)
    {
        animClip.SampleAnimation(_go, _time);
        if(UseFollowTrans)
        {
            if(followTrans_LockYto0)
            {
                if(match_source != null)
                {
                    int index1 = match_source.GetIndexOfBoneType(JAnimationData.BoneType.hips);
                    if(index1 != -1)
                    {
                        if(match_source.boneMatchList[index1].matchedT != null)
                        {
                            Vector3 roa1 = match_source.boneMatchList[index1].matchedT.localEulerAngles;
                            roa1.y = 0f;
                            match_source.boneMatchList[index1].matchedT.localEulerAngles = roa1;
                        }
                    }
                }
            }
        }
    }

    
    private void initPlay()
    {
        playTimer = 0f;
        playJindu = 0f;
        isPlaying = true;
    }

    float playTimer = 0f;
    float playJindu = 0f;
    bool isPlaying = false;
    float _lastTime = 0f;
    bool isLooping = false;
    void Update()
    {
        var now = EditorApplication.timeSinceStartup;
        float delTime = (float)now - _lastTime;
        _lastTime = (float)now;

        if(dragPlayMode)
        {
            if(animClip != null && ga != null)
            {
                PlaySample(ga, dragJindu * animClip.length);
                FollowPlay();
            }
        }
        else
        {
            if (isPlaying)
            {
                if (playJindu < 1f)
                {
                    PlaySample(ga, playTimer);
                    FollowPlay();
                    playJindu = playTimer / animClip.length;
                    playTimer += delTime;
                }
                else
                {
                    playTimer = 0f;
                    playJindu = 0f;
                    if(isLooping == false)
                    {
                        isPlaying = false;
                    }
                }
            }
        }

    }
     

    void DrawALine (int _height)
    {
        GUILayout.Box("", lineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(_height));
    }
    void ShowProcessBar()
    {
        if (Event.current.type == EventType.Repaint)
        {
            var lastRect = GUILayoutUtility.GetLastRect();
            EditorGUI.ProgressBar(new Rect(lastRect.x, lastRect.y + lastRect.height, lastRect.width, 20), Mathf.Clamp01(playJindu), "");
        }
        GUILayout.Space(20);
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


public class AnimationBoneMatch
{

    /// <summary>
    /// 曲线的类型。其中Roa_3_X表示三条曲线组成的旋转角度，Roa_4_X表示四条曲线组成的旋转角度。
    /// </summary>
    public enum XYZType
    {
        UnknowType,
        Pos_X,
        Pos_Y,
        Pos_Z,
        Roa_3_X,
        Roa_3_Y,
        Roa_3_Z,
        Roa_4_X,
        Roa_4_Y,
        Roa_4_Z,
        Roa_4_W
    }
    public List<JAnimationData.BoneType> boneTypeList = new List<JAnimationData.BoneType>();
    public List<XYZType> xyzTypeList = new List<XYZType>();
    public List<bool> useList = new List<bool>();

    public AnimationBoneMatch()
    {
        boneTypeList = new List<JAnimationData.BoneType>();
        xyzTypeList = new List<XYZType>();
        useList = new List<bool>();
    }

    public void ReSetSize (int _size)
    {
        if(boneTypeList.Count != _size)
        {
            if(boneTypeList.Count > _size)
            {
                while(boneTypeList.Count > _size)
                {
                    boneTypeList.RemoveAt(boneTypeList.Count - 1);
                }
            }
            else
            {
                while (boneTypeList.Count < _size)
                {
                    boneTypeList.Add(JAnimationData.BoneType.unknowType);
                }
            }
        }
        if(xyzTypeList.Count != _size)
        {
            if (xyzTypeList.Count > _size)
            {
                while (xyzTypeList.Count > _size)
                {
                    xyzTypeList.RemoveAt(xyzTypeList.Count - 1);
                }
            }
            else
            {
                while (xyzTypeList.Count < _size)
                {
                    xyzTypeList.Add(XYZType.UnknowType);
                }
            }
        }
        if(useList.Count != _size)
        {
            if (useList.Count > _size)
            {
                while (useList.Count > _size)
                {
                    useList.RemoveAt(useList.Count - 1);
                }
            }
            else
            {
                while (useList.Count < _size)
                {
                    useList.Add(false);
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_fileSaveName"></param>
    /// <param name="_ga"></param>
    /// <param name="_animclip"></param>
    /// <param name="_autoMatchData"></param>
    /// <param name="toResourcePath">写入resource路径。如果这项为true，_fileSaveName仅仅是文件名而不是路径</param>
    public JAnimationData TransData_RoaInWorldSpace(string _fileSaveName, GameObject _ga, AnimationClip _animclip, JAnimation_AutoSetBoneMatchData _autoMatchData, bool toResourcePath = true)
    {
        if(_ga == null)
        {
            Debug.LogWarning("When you want turn roa to world space, the ga Can't Be Null !!!");
            return null;
        }
        if (_animclip == null)
        {
            Debug.LogWarning("theAnimationClip Can't Be Null !!!");
            return null;
        }
        if(_autoMatchData == null)
        {
            Debug.LogWarning("the autoMatchData Can't Be Null !!!");
            return null;
        }

        //将JAnimationData创建为asset,为没有_data而准备一个要生成的数据
        JAnimationData asset = ScriptableObject.CreateInstance<JAnimationData>();
        asset.jAD = new JAnimationData.JAD();
        asset.jAD.roaSpaceType = JAnimationData.RoaSpaceType.world;

        List<JAnimationData.BoneType> typeList = new List<JAnimationData.BoneType>();
        for (int i = 0; i < boneTypeList.Count; i++)
        {
            if (typeList.Contains(boneTypeList[i]) == false && boneTypeList[i] != JAnimationData.BoneType.unknowType)
            {
                typeList.Add(boneTypeList[i]);
            }
        }

        BoneMatch match1 = new BoneMatch();
        match1.StartAutoMatch(_ga.transform, _autoMatchData);
        /*for(int i=0; i< match1.boneMatchList.Count; i++)
        {
            if (match1.boneMatchList[i].matchedT == null) continue;
            if (typeList.Contains(match1.boneMatchList[i].boneType) == false) continue;

            Debug.Log(match1.boneMatchList[i].boneType + "matchedT: " + match1.boneMatchList[i].matchedT.name);
        }*/
       


        EditorCurveBinding[] binds = AnimationUtility.GetCurveBindings(_animclip); 
        for (int i = 0; i < typeList.Count; i++)
        { 
            List<Vector3> roaList_3 = new List<Vector3>();
            List<Vector4> roaList_4 = new List<Vector4>();
            List<float> timeList_roa = new List<float>();
            List<Vector3> posList = new List<Vector3>();
            List<float> timeList_pos = new List<float>();

            Vector3 haveValue_Roa_3 = -Vector3.one;
            Vector4 haveValue_Roa_4 = -Vector4.one;
            Vector3 haveValue_Pos = -Vector3.one;

            for (int j = 0; j < boneTypeList.Count; j++)
            {
                if (boneTypeList[j] == typeList[i] && useList[j] == true)
                {
                    switch (xyzTypeList[j])
                    {
                        case XYZType.Pos_X:
                            haveValue_Pos.x = j;
                            break;
                        case XYZType.Pos_Y:
                            haveValue_Pos.y = j;
                            break;
                        case XYZType.Pos_Z:
                            haveValue_Pos.z = j;
                            break;
                        case XYZType.Roa_3_X:
                            haveValue_Roa_3.x = j;
                            break;
                        case XYZType.Roa_3_Y:
                            haveValue_Roa_3.y = j;
                            break;
                        case XYZType.Roa_3_Z:
                            haveValue_Roa_3.z = j;
                            break;
                        case XYZType.Roa_4_X:
                            haveValue_Roa_4.x = j;
                            break;
                        case XYZType.Roa_4_Y:
                            haveValue_Roa_4.y = j;
                            break;
                        case XYZType.Roa_4_Z:
                            haveValue_Roa_4.z = j;
                            break;
                        case XYZType.Roa_4_W:
                            haveValue_Roa_4.w = j;
                            break;
                    }
                }
            }


            //-------------------三元旋转曲线信息----------
            {
                if (haveValue_Roa_3.x != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_3.x]);
                    Keyframe[] keys = curve.keys;
                    ReSetV3ListSize(roaList_3, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector4 _v4 = roaList_3[k];
                        _v4.x = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_3[k] = _v4;
                    }
                }
                if (haveValue_Roa_3.y != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_3.y]);
                    Keyframe[] keys = curve.keys;
                    ReSetV3ListSize(roaList_3, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector4 _v4 = roaList_3[k];
                        _v4.y = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_3[k] = _v4;
                    }
                }
                if (haveValue_Roa_3.z != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_3.z]);
                    Keyframe[] keys = curve.keys;
                    ReSetV3ListSize(roaList_3, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector4 _v4 = roaList_3[k];
                        _v4.z = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_3[k] = _v4;
                    }
                }
            }


            //--------------------四元旋转曲线信息------
            {
                if (haveValue_Roa_4.x != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_4.x]);
                    Keyframe[] keys = curve.keys;
                    ReSetV4ListSize(roaList_4, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector4 _v4 = roaList_4[k];
                        _v4.x = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_4[k] = _v4;
                    }
                }
                if (haveValue_Roa_4.y != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_4.y]);
                    Keyframe[] keys = curve.keys;
                    ReSetV4ListSize(roaList_4, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector4 _v4 = roaList_4[k];
                        _v4.y = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_4[k] = _v4;
                    }
                }
                if (haveValue_Roa_4.z != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_4.z]);
                    Keyframe[] keys = curve.keys;
                    ReSetV4ListSize(roaList_4, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector4 _v4 = roaList_4[k];
                        _v4.z = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_4[k] = _v4;
                    }
                }
                if (haveValue_Roa_4.w != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_4.w]);
                    Keyframe[] keys = curve.keys;
                    ReSetV4ListSize(roaList_4, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector4 _v4 = roaList_4[k];
                        _v4.w = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_4[k] = _v4;
                    }
                }
            }

            //-------------------Pos信息-----------------
            {
                if (haveValue_Pos.x != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Pos.x]);
                    Keyframe[] keys = curve.keys;
                    ReSetV3ListSize(posList, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_pos, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector3 _v3 = posList[k];
                        _v3.x = keys[k].value;
                        timeList_pos[k] = keys[k].time;
                        posList[k] = _v3;
                    }
                }
                if (haveValue_Pos.y != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Pos.y]);
                    Keyframe[] keys = curve.keys;
                    ReSetV3ListSize(posList, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_pos, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector3 _v3 = posList[k];
                        _v3.y = keys[k].value;
                        timeList_pos[k] = keys[k].time;
                        posList[k] = _v3;
                    }
                }
                if (haveValue_Pos.z != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Pos.z]);
                    Keyframe[] keys = curve.keys;
                    ReSetV3ListSize(posList, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_pos, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector3 _v3 = posList[k];
                        _v3.z = keys[k].value;
                        timeList_pos[k] = keys[k].time;
                        posList[k] = _v3;
                    }
                }
            }

            //---setValues-----------------------------------------------------
            if (haveValue_Roa_3 != -Vector3.one && haveValue_Roa_4 == -Vector4.one)
            {
                for (int k = 0; k < roaList_3.Count; k++)
                {
                    Transform t1 = match1.GetT_ByBoneType(typeList[i]);
                    if(t1 != null)
                    {
                        _animclip.SampleAnimation(_ga, timeList_roa[k]);
                        Quaternion q1 = t1.rotation;
                        asset.jAD.AddData_roa4(typeList[i], timeList_roa[k], new Vector4(q1.x, q1.y, q1.z, q1.w)   );
                    }
                    else
                    {
                        Debug.LogError("we have type [" + typeList[i] + "] but we NOT have its matchedT !!!!");
                    } 
                }
                Debug.Log("this anim is roa3 , but we trans it to roa4.");
            }
            else if (haveValue_Roa_3 == -Vector3.one && haveValue_Roa_4 != -Vector4.one)
            {
                for (int k = 0; k < roaList_4.Count; k++)
                {
                    Transform t1 = match1.GetT_ByBoneType(typeList[i]);
                    if (t1 != null)
                    {
                        _animclip.SampleAnimation(_ga, timeList_roa[k]);
                        Quaternion q1 = t1.rotation;
                        asset.jAD.AddData_roa4(typeList[i], timeList_roa[k], new Vector4(q1.x, q1.y, q1.z, q1.w)   );
                    }
                    else
                    {
                        Debug.LogError("we have type [" + typeList[i] + "] but we NOT have its matchedT !!!!");
                    }
                }
                Debug.Log("this anim is roa4.");
            }
            else if (haveValue_Roa_3 != -Vector3.one && haveValue_Roa_4 != -Vector4.one)
            {
                Debug.LogError("Can't judge your rotation information type , so, can't write ["
                    + typeList[i].ToString() + "]'s rotation into JData !!!" +
                    "/n  不能正确判断旋转信息的类型，所以不能写入[" + typeList[i].ToString() + "]的旋转信息 !!!");
            }

            for (int k = 0; k < posList.Count; k++)
            {
                asset.jAD.AddData_pos(typeList[i], timeList_pos[k], posList[k]);
            }

        }


        {
            string fileName1 = _fileSaveName;
            if (string.IsNullOrEmpty(fileName1) == true)
            {
                fileName1 = "JAnimD_" + _animclip.name + "_RoaInWorldSpace.asset";
                AssetDatabase.CreateAsset(asset, "Assets/Resources/JAnimDatas/" + fileName1);
            }
            else
            {
                fileName1 = fileName1 + ".asset";
                if(toResourcePath)
                {
                    AssetDatabase.CreateAsset(asset, "Assets/Resources/JAnimDatas/" + fileName1);
                }
                else
                {
                    AssetDatabase.CreateAsset(asset, fileName1);
                }
            }
          
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;

            Debug.Log("Saved Data To [" + fileName1 + "]."); 
        }
        Debug.Log("Process Finish ! ! !");
        return asset;

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_fileSaveName"></param>
    /// <param name="_animclip"></param>
    /// <param name="toResourcePath">写入resource路径。如果这项为true，_fileSaveName仅仅是文件名而不是路径</param>
    public JAnimationData TransData (string _fileSaveName, AnimationClip _animclip, bool toResourcePath = true)
    {
        if (_animclip == null )
        {
            Debug.LogWarning("theAnimationClip Can't Be Null !!!");
            return null;
        }
        //将JAnimationData创建为asset,为没有_data而准备一个要生成的数据
        JAnimationData asset = ScriptableObject.CreateInstance<JAnimationData>();
       

        List<JAnimationData.BoneType> typeList = new List<JAnimationData.BoneType>();
        for (int i = 0; i < boneTypeList.Count; i++)
        {
            if (typeList.Contains(boneTypeList[i]) == false && boneTypeList[i] != JAnimationData.BoneType.unknowType)
            {
                typeList.Add(boneTypeList[i]);
            }
        }
        

        EditorCurveBinding[] binds = AnimationUtility.GetCurveBindings(_animclip);

        for (int i=0; i< typeList.Count; i++)
        {

            List<Vector3> roaList_3 = new List<Vector3>();
            List<Vector4> roaList_4 = new List<Vector4>();
            List<float> timeList_roa = new List<float>();
            List<Vector3> posList = new List<Vector3>();
            List<float> timeList_pos = new List<float>();

            Vector3 haveValue_Roa_3 = -Vector3.one;
            Vector4 haveValue_Roa_4 = -Vector4.one;
            Vector3 haveValue_Pos = -Vector3.one;

            for (int j=0; j< boneTypeList.Count; j++)
            {
                if(boneTypeList[j] == typeList[i] && useList[j] == true)
                {
                    switch (xyzTypeList[j])
                    {
                        case XYZType.Pos_X:
                            haveValue_Pos.x = j;
                            break;
                        case XYZType.Pos_Y:
                            haveValue_Pos.y = j;
                            break;
                        case XYZType.Pos_Z:
                            haveValue_Pos.z = j;
                            break;
                        case XYZType.Roa_3_X:
                            haveValue_Roa_3.x = j;
                            break;
                        case XYZType.Roa_3_Y:
                            haveValue_Roa_3.y = j;
                            break;
                        case XYZType.Roa_3_Z:
                            haveValue_Roa_3.z = j;
                            break;
                        case XYZType.Roa_4_X:
                            haveValue_Roa_4.x = j;
                            break;
                        case XYZType.Roa_4_Y:
                            haveValue_Roa_4.y = j;
                            break;
                        case XYZType.Roa_4_Z:
                            haveValue_Roa_4.z = j;
                            break;
                        case XYZType.Roa_4_W:
                            haveValue_Roa_4.w = j;
                            break;
                    }
                }
            }


            //-------------------三元旋转曲线信息----------   getEditorCurve将获取的浮点曲线中 符合三元数格式的关键帧提取出来 放在了curve曲线中
            {
                if (haveValue_Roa_3.x != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_3.x]);
                    Keyframe[] keys = curve.keys;
                    ReSetV3ListSize(roaList_3, keys.Length);//ReSize 将队列调整到一个长度
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector3 _v3 = roaList_3[k];
                        _v3.x = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_3[k] = _v3;
                    }
                }
                if (haveValue_Roa_3.y != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_3.y]);
                    Keyframe[] keys = curve.keys;
                    ReSetV3ListSize(roaList_3, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector3 _v3 = roaList_3[k];
                        _v3.y = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_3[k] = _v3;
                    }
                }
                if (haveValue_Roa_3.z != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_3.z]);
                    Keyframe[] keys = curve.keys;
                    ReSetV3ListSize(roaList_3, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector3 _v3 = roaList_3[k];
                        _v3.z = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_3[k] = _v3;
                    }
                }
            }


            //--------------------四元旋转曲线信息------
            {
                if (haveValue_Roa_4.x != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_4.x]);
                    Keyframe[] keys = curve.keys;
                    ReSetV4ListSize(roaList_4, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector4 _v4 = roaList_4[k];
                        _v4.x = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_4[k] = _v4;
                    }
                }
                if (haveValue_Roa_4.y != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_4.y]);
                    Keyframe[] keys = curve.keys;
                    ReSetV4ListSize(roaList_4, keys.Length);//ReSize 
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector4 _v4 = roaList_4[k];
                        _v4.y = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_4[k] = _v4; 
                    }
                }
                if (haveValue_Roa_4.z != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_4.z]);
                    Keyframe[] keys = curve.keys;
                    ReSetV4ListSize(roaList_4, keys.Length);//ReSize 
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector4 _v4 = roaList_4[k];
                        _v4.z = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_4[k] = _v4; 
                    }
                }
                if (haveValue_Roa_4.w != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Roa_4.w]);
                    Keyframe[] keys = curve.keys;
                    ReSetV4ListSize(roaList_4, keys.Length);//ReSize
                    ReSetFloatListSize(timeList_roa, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector4 _v4 = roaList_4[k];
                        _v4.w = keys[k].value;
                        timeList_roa[k] = keys[k].time;
                        roaList_4[k] = _v4; 
                    }
                }
            }

            //-------------------Pos信息-----------------
            {
                if (haveValue_Pos.x != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Pos.x]);
                    Keyframe[] keys = curve.keys;
                    ReSetV3ListSize(posList, keys.Length);//ReSize 
                    ReSetFloatListSize(timeList_pos, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector3 _v3 = posList[k];
                        _v3.x = keys[k].value;
                        timeList_pos[k] = keys[k].time;
                        posList[k] = _v3; 
                    }
                }
                if (haveValue_Pos.y != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Pos.y]);
                    Keyframe[] keys = curve.keys;
                    ReSetV3ListSize(posList, keys.Length);//ReSize 
                    ReSetFloatListSize(timeList_pos, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector3 _v3 = posList[k];
                        _v3.y = keys[k].value;
                        timeList_pos[k] = keys[k].time;
                        posList[k] = _v3; 
                    }
                }
                if (haveValue_Pos.z != -1)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animclip, binds[(int)haveValue_Pos.z]);
                    Keyframe[] keys = curve.keys;
                    ReSetV3ListSize(posList, keys.Length);//ReSize 
                    ReSetFloatListSize(timeList_pos, keys.Length);//ReSize
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Vector3 _v3 = posList[k];
                        _v3.z = keys[k].value;
                        timeList_pos[k] = keys[k].time;
                        posList[k] = _v3; 
                    }
                }
            }

            //---setValues-----------------------------------------------------   将数据存入JAD格式的adddata里
            if(haveValue_Roa_3 != -Vector3.one && haveValue_Roa_4 == -Vector4.one)
            {
                for (int k = 0; k < roaList_3.Count; k++)
                {
                    Quaternion q1 = Quaternion.Euler(roaList_3[k].x, roaList_3[k].y, roaList_3[k].z);
                    asset.jAD.AddData_roa4(typeList[i], timeList_roa[k], new Vector4(q1.x, q1.y, q1.z, q1.w)   );
                }
                Debug.Log("this anim is roa3 , but we trans it to roa4.");
            }
            else if(haveValue_Roa_3 == -Vector3.one && haveValue_Roa_4 != -Vector4.one)
            {
                for (int k = 0; k < roaList_4.Count; k++)
                {
                    asset.jAD.AddData_roa4(typeList[i], roaList_4[k], timeList_roa[k]);
                }
                Debug.Log("this anim is roa4.");
            }
            else if(haveValue_Roa_3 != -Vector3.one && haveValue_Roa_4 != -Vector4.one)
            {
                Debug.LogError("Can't judge your rotation information type , so, can't write ["
                    + typeList[i].ToString() + "]'s rotation into JData !!!" +
                    "/n  不能正确判断旋转信息的类型，所以不能写入[" + typeList[i].ToString() + "]的旋转信息 !!!");
            }
            
            for (int k = 0; k < posList.Count; k++)
            {
                asset.jAD.AddData_pos(typeList[i], posList[k], timeList_pos[k]);
            }
           
        }


        {
            string fileName1 = _fileSaveName;
            if(string.IsNullOrEmpty(fileName1) == true)
            {
                fileName1 = "JAnimD_" + _animclip.name + ".asset";
                AssetDatabase.CreateAsset(asset, "Assets/Resources/JAnimDatas/" + fileName1);
            }
            else
            {
                fileName1 = fileName1 + ".asset";
                if(toResourcePath)
                {
                    AssetDatabase.CreateAsset(asset, "Assets/Resources/JAnimDatas/" + fileName1);
                }
                else
                {
                    AssetDatabase.CreateAsset(asset,  fileName1);
                }
            }
            
            
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;

            Debug.Log("Saved Data To [" + fileName1 + "].");
        }
        Debug.Log("Process Finish ! ! !");
        return asset;
    }

    /// <summary>
    /// 将AnimationClip数据存在项目内
    /// </summary> 
    public static void SaveAnimationClip(AnimationClip clip, string pathAndName)
    {
        AssetDatabase.CreateAsset(clip, pathAndName);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = clip;

        Debug.Log("Saved AnimationClip To [<color=blue>" + pathAndName + "</color>].");
    }

    public class BindWithNumber
    {
        public EditorCurveBinding bind;
        public int number = -1;
        public BindWithNumber(EditorCurveBinding _bind, int _number)
        {
            bind = _bind;
            number = _number;
        }
    }

    public void AutoSetBoneMatch (AnimationClip _animclip,
        JAnimation_AutoSetBoneMatchData _autoMatchData, bool _roaIs3Not4, bool _dontUsePosAnimationExceptHip)
    {
        if (_animclip == null) return;
        
        EditorCurveBinding[] _binds = AnimationUtility.GetCurveBindings(_animclip);

        List<JAnimationData.BoneType> boneTypeList_Temp = new List<JAnimationData.BoneType>();
        List<XYZType> xyzTypeList_Temp = new List<XYZType>();
        List<bool> useList_Temp = new List<bool>();

        List<BindWithNumber> binds = new List<BindWithNumber>();
        for (int i=0; i< _binds.Length; i++)
        {
            binds.Add(new BindWithNumber(_binds[i],i));
        }
        binds.Sort(delegate (BindWithNumber bindX, BindWithNumber bindY) 
        {
            return bindX.bind.path.CompareTo(bindY.bind.path);
        });//Sort

        for (int i=0; i< binds.Count; i++)
        {
            string _name = binds[i].bind.propertyName;
            string _path = binds[i].bind.path;
            List<string> _posKeyWords = new List<string>();
            _posKeyWords.Add("pos");
            _posKeyWords.Add("position");
            _posKeyWords.Add("Position"); 
            List<string> _roaKeyWords = new List<string>();
            _roaKeyWords.Add("roa");
            _roaKeyWords.Add("Rotation");
            _roaKeyWords.Add("rotation"); 
            XYZType _xyzType1 = judgeXYZTypeFromText(_name, _roaKeyWords, _posKeyWords, _roaIs3Not4);
            
            
            string[] path_parts = _path.Split('/');
            string _lastPath = path_parts[path_parts.Length - 1];
            if(string.IsNullOrEmpty(_path))
            {
                Debug.Log("这个动画的binds不太对劲！ 没有path！ 已自动使用propertyName进行匹配！");
                _lastPath = binds[i].bind.propertyName;
            }
            JAnimationData.BoneType _boneType1 = _autoMatchData.GetBoneType(_lastPath);
            //Debug.Log(_lastPath +"  "+_boneType1);
            
            for(int j=0; j< boneTypeList_Temp.Count; j++)
            {
                if(boneTypeList_Temp[j] == _boneType1 && xyzTypeList_Temp[j] == _xyzType1)
                {//这个类型已经匹配了较短的骨骼名字。
                    _xyzType1 = XYZType.UnknowType;
                    _boneType1 = JAnimationData.BoneType.unknowType;
                    break;
                }
            }
            boneTypeList_Temp.Add(_boneType1);
            xyzTypeList_Temp.Add(_xyzType1);
            if (_boneType1 != JAnimationData.BoneType.unknowType && _xyzType1 != XYZType.UnknowType)
            {
                if(_dontUsePosAnimationExceptHip && (_xyzType1== XYZType.Pos_X || _xyzType1 == XYZType.Pos_Y || _xyzType1 == XYZType.Pos_Z) && _boneType1 != JAnimationData.BoneType.hips)
                {
                    useList_Temp.Add(false);
                }
                else
                {
                    useList_Temp.Add(true);
                } 
            }
            else
            {
                useList_Temp.Add(false);
            }
        }

        boneTypeList.Clear();
        xyzTypeList.Clear();
        useList.Clear();
        ReSetSize(binds.Count);
        
        for(int i=0; i< _binds.Length; i++ )
        {
            for (int k=0; k<binds.Count; k++)
            {
               if(binds[k].number == i)
                {
                    boneTypeList[i] = boneTypeList_Temp[k];
                    xyzTypeList[i] = xyzTypeList_Temp[k];
                    useList[i] = useList_Temp[k];
                    break;
                }
            }
        }
    }

    private XYZType judgeXYZTypeFromText (string _text,List<string> RoaKeyWords,List<string>PosKeyWords,bool RoaIs3Not4)
    {
        bool isRoa = false;
        if(_text.Length > 3 && _text[_text.Length - 3] == 'Q')
        {
            isRoa = true;
        }
        else
        {
            for (int i = 0; i < RoaKeyWords.Count; i++)
            {
                if (_text.Contains(RoaKeyWords[i]) == true)
                {
                    isRoa = true;
                    break;
                }
            }
        } 
        bool isPos = false;
        if (_text.Length > 3 && _text[_text.Length - 3] == 'T')
        {
            isPos = true;
        }
        else
        {
            for (int i = 0; i < PosKeyWords.Count; i++)
            {
                if (_text.Contains(PosKeyWords[i]) == true)
                {
                    isPos = true;
                    break;
                }
            }
        } 

        if(isRoa)
        {
            if (_text[_text.Length - 1] == ('x') || _text[_text.Length - 1] == ('X'))
            {
                if(RoaIs3Not4)
                {
                    return XYZType.Roa_3_X;
                }
                else
                {
                    return XYZType.Roa_4_X;
                }
            }
            else if (_text[_text.Length - 1] == ('y') || _text[_text.Length - 1] == ('Y'))
            {
                if (RoaIs3Not4)
                {
                    return XYZType.Roa_3_Y;
                }
                else
                {
                    return XYZType.Roa_4_Y;
                }
            }
            else if (_text[_text.Length - 1] == ('z') || _text[_text.Length - 1] == ('Z'))
            {
                if (RoaIs3Not4)
                {
                    return XYZType.Roa_3_Z;
                }
                else
                {
                    return XYZType.Roa_4_Z;
                }
            }
            else if (_text[_text.Length - 1] == ('w') || _text[_text.Length - 1] == ('W'))
            {
                return XYZType.Roa_4_W;
            }
        }
        else if(isPos)
        {
            if (_text[_text.Length - 1] == ('x') || _text[_text.Length - 1] == ('X'))
            {
                return XYZType.Pos_X;
            }
            else if (_text[_text.Length - 1] == ('y') || _text[_text.Length - 1] == ('Y'))
            {
                return XYZType.Pos_Y;
            }
            else if (_text[_text.Length - 1] == ('z') || _text[_text.Length - 1] == ('Z'))
            {
                return XYZType.Pos_Z;
            }
        }
        return XYZType.UnknowType;
    }
    private void ReSetV3ListSize(List<Vector3> _v3List, int _size)
    {
        if (_v3List.Count != _size)
        {
            if (_v3List.Count > _size)
            {
                while (_v3List.Count > _size)
                {
                    _v3List.RemoveAt(_v3List.Count - 1);
                }
            }
            else
            {
                while (_v3List.Count < _size)
                {
                    _v3List.Add(Vector3.zero);
                }
            }
        }
    }
    private void ReSetV4ListSize (List<Vector4> _v4List, int _size)
    {
        if(_v4List.Count != _size)
        {
            if(_v4List.Count > _size)
            {
                while(_v4List.Count > _size)
                {
                    _v4List.RemoveAt(_v4List.Count - 1);
                }
            }
            else
            {
                while (_v4List.Count < _size)
                {
                    _v4List.Add(Vector4.zero);
                }
            }
        }
    }
    private void ReSetFloatListSize(List<float> _floatList, int _size)
    {
        if (_floatList.Count != _size)
        {
            if (_floatList.Count > _size)
            {
                while (_floatList.Count > _size)
                {
                    _floatList.RemoveAt(_floatList.Count - 1);
                }
            }
            else
            {
                while (_floatList.Count < _size)
                {
                    _floatList.Add(0f);
                }
            }
        }
    }
    
}