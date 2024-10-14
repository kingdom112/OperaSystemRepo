using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JAnimationSystem;
using JVehicleFrameSystem;

public class JAnimFileTransferWindow_Mult : EditorWindow
{

    public class Member
    {
        public GameObject ga;
        public AnimationClip clip;
        public JAnimation_AutoSetBoneMatchData automatcher;
    }

    GUIStyle boxStyle;
    GUIStyle richTextStyle_Mid;
    GUIStyle richTextStyle_Left;
    GUIStyle headerStyle;
    GUIStyle smallHeaderStyle;
    GUIStyle lineStyle;

    bool turnRoaToWorldSpace = false;
    JAnimation_AutoSetBoneMatchData automatcher_all = null;
    AnimationBoneMatch boneMatch = new AnimationBoneMatch();
    List<Member> members = new List<Member>();

    [MenuItem("JTools/JAnimFileMultTransfer")]
    static void Open()
    {
        var win = EditorWindow.GetWindow<JAnimFileTransferWindow_Mult>(false, "JAnimFileMultTransfer", true);
        win.GenerateStyles();
        var icon = Resources.Load("Textures/jicon2") as Texture;
        win.titleContent = new GUIContent("JAnimFileMultTransfer", icon);
        win.boneMatch = new AnimationBoneMatch();
        win.members = new List<Member>();
    }
    /// <summary>
    /// 给所有数据使用相同的autoMatcher
    /// </summary>
    bool useOneAutoMatcherForAll = false;
    Vector2 scrollPos_BoneMatches = Vector2.zero;
    void OnGUI()
    {
        GUILayout.Box("JAnimFileTransfer [Mult]", boxStyle, GUILayout.Height(60), GUILayout.ExpandWidth(true));
        GUILayout.Box("You can use this tool to change mult AnimationClips into JAnimFiles.", richTextStyle_Mid, GUILayout.ExpandWidth(true));
        GUILayout.Box("你可以使用这个工具令多条 AnimationClips 各自转换为 JAnimFiles.", richTextStyle_Mid, GUILayout.ExpandWidth(true));

        GUILayout.Space(10);
        DrawALine(3);
        GUILayout.Space(10);
        GUILayout.Label("BasicSets", headerStyle);
        turnRoaToWorldSpace = GUILayout.Toggle(turnRoaToWorldSpace, new GUIContent("turnRoaToWorldSpace", "把旋转角度转换到世界空间"));
        useOneAutoMatcherForAll = GUILayout.Toggle(useOneAutoMatcherForAll, new GUIContent("useOneAutoMatcherForAll", "给所有数据使用相同的autoMatcher"));
        if(useOneAutoMatcherForAll)
        {
            automatcher_all =
               EditorGUILayout.ObjectField("Automatcher_all", automatcher_all,
               typeof(JAnimation_AutoSetBoneMatchData), false) as JAnimation_AutoSetBoneMatchData;
        }
        GUILayout.Space(5);
        DrawALine(2);
        scrollPos_BoneMatches = EditorGUILayout.BeginScrollView(scrollPos_BoneMatches,
               GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        for(int i=0; i< members.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("第" + i + "个"); 
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                members.RemoveAt(i);
                Debug.Log("remove At  " + i);
                return;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            members[i].ga = EditorGUILayout.ObjectField("theGameObject", members[i].ga, typeof(GameObject), true) as GameObject;
            EditorGUILayout.EndHorizontal();
            members[i].clip = EditorGUILayout.ObjectField("Clip", members[i].clip, typeof(AnimationClip), true) as AnimationClip;
            if(useOneAutoMatcherForAll == false)
            {
                members[i].automatcher =
                    EditorGUILayout.ObjectField("Automatcher", members[i].automatcher,
                    typeof(JAnimation_AutoSetBoneMatchData), false) as JAnimation_AutoSetBoneMatchData;
            }
            else
            {
                members[i].automatcher = automatcher_all;
            }
            GUILayout.Space(5);
            DrawALine(1);
            EditorGUI.indentLevel--;
        }
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("+"))
        {
            members.Add(new Member());
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
        GUILayout.Space(10);

        if(GUILayout.Button("Process!"))
        {
            for(int i=0; i<members.Count; i++)
            {
                Process(members[i]);
            }
        }
        GUILayout.Space(10);
    }
    /// <summary>
    /// 处理一个
    /// </summary> 
    void Process(Member member)
    {
        if(ProcessBoneFrame(member.ga, member.automatcher))
        {
            TransClip(member.ga, member.clip, member.automatcher); 
        }
    }
    private bool TransClip(GameObject ga, AnimationClip clip, JAnimation_AutoSetBoneMatchData autoMatcher)
    {
        if(clip == null)
        {
            Debug.LogError("clip == null !!!");
            return false;
        }
        if(boneFramework1_temp == null)
        {
            Debug.LogError("boneFramework1_temp == null !!!");
            return false;
        }
        AnimationBoneMatch boneMatch = new AnimationBoneMatch();
        boneMatch.AutoSetBoneMatch(clip, autoMatcher, false, true);
        string clipPath = "";//
        clipPath = "Assets/Opera/Frames_JAnims_Output/" + "JAnimD_AOP_" + ga.name;
        if (turnRoaToWorldSpace == false)
        {
            JAnimationData d1 =  boneMatch.TransData(clipPath, clip, false);
            VehicleBuilder vehicleBuilder = new VehicleBuilder();
            vehicleBuilder.SetBoneFramework(boneFramework1_temp);
            vehicleBuilder.Build(1);
            GameObject builded = vehicleBuilder.buildedGA;
            JAnimationDataEditor.GetTPoseData(builded.transform, d1, autoMatcher);
            if(Application.isEditor)
            {
                DestroyImmediate(builded);
            }
            else
            {
                Destroy(builded);
            }
        }
        else
        {
            JAnimationData d1 = boneMatch.TransData_RoaInWorldSpace(clipPath, ga, clip, autoMatcher, false);
            VehicleBuilder vehicleBuilder = new VehicleBuilder();
            vehicleBuilder.SetBoneFramework(boneFramework1_temp);
            vehicleBuilder.Build(1);
            GameObject builded = vehicleBuilder.buildedGA;
            JAnimationDataEditor.GetTPoseData(builded.transform, d1, autoMatcher);
            if (Application.isEditor)
            {
                DestroyImmediate(builded);
            }
            else
            {
                Destroy(builded);
            }
        }

        return true;
    }

    private bool CanProcessBoneFrame(GameObject target, JAnimation_AutoSetBoneMatchData autoMatchData)
    {
        if (target != null && autoMatchData != null) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    BoneFramework boneFramework1_temp = null;
    private bool ProcessBoneFrame(GameObject ga, JAnimation_AutoSetBoneMatchData autoMatcher)
    {
        if (CanProcessBoneFrame(ga, autoMatcher))
        {
            BoneFramework boneFramework1 = new BoneFramework();
            if (BoneFramework.ReadBoneFrame(ga, autoMatcher, out boneFramework1, 1f))
            {
                if(boneFramework1.Bones.Count <= 2)
                {
                    Debug.LogError("[<color=red>" + ga.name + "</color>] 匹配到的骨骼太少了！已经跳过了这条数据！");
                    return false;
                }
                JVehicleFrame asset = ScriptableObject.CreateInstance<JVehicleFrame>(); 
                asset.boneFramework = boneFramework1;
                boneFramework1_temp = boneFramework1;
                while (asset.boneFrameworkMousePosList.Count < boneFramework1.Bones.Count)
                {
                    asset.boneFrameworkMousePosList.Add(new Vector2(0, 0));
                    if(asset.boneFrameworkMousePosList.Count >= 100)
                    {
                        Debug.LogError("数量太多了！ 请检查[" + ga.name + "] 是否有问题！");
                        break;
                    }
                }
                string saveFileName = "";
                saveFileName = "VF_" + ga.name + ".asset";
                {
                    string fullPath = "Assets/Opera/Frames_JAnims_Output/" + saveFileName;
                    AssetDatabase.CreateAsset(asset, fullPath);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();

                    EditorUtility.FocusProjectWindow();

                    Selection.activeObject = asset;

                    Debug.Log("Saved (<color=blue>" + "JVehicleFrame" + "</color>) Data To [<color=yellow>" + fullPath + "</color>].");
                }
                Debug.Log("Process Done! (From JBoneReader)");
                return true;
            }
            else
            {
                Debug.Log("Process Error! (From JBoneReader)");
                return false;
            }

        }
        else
        {
            Debug.Log("Can't Process! ["+ ga.name+"]");
            return false;
        } 
    }
    void DrawALine(int _height)
    {
        GUILayout.Box("", lineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(_height));
    }
    void ShowProcessBar(float playJindu)
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
        richTextStyle_Mid.normal.textColor = Color.black;
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
