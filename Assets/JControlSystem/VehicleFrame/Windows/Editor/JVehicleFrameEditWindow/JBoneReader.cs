 using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using JVehicleFrameSystem;
using JAnimationSystem;


public class JBoneReader : EditorWindow
{

    GUIStyle boxStyle; 
    GUIStyle richTextStyle_Mid;
    GUIStyle richTextStyle_Left;
    GUIStyle headerStyle;
    GUIStyle smallHeaderStyle;
    GUIStyle lineStyle;

    GameObject target;
    JAnimation_AutoSetBoneMatchData autoMatchData; 
    JVehicleFrame jVehicleFrame;
    /// <summary>
    /// 放大因子
    /// </summary>
    float amplificateFactor = 1f;

    [MenuItem("JTools/JBoneReader")]
    static void Open()
    {
        var win = EditorWindow.GetWindow<JBoneReader>(false, "JBoneReader", true);
        win.GenerateStyles();
        var icon = Resources.Load("Textures/jicon2") as Texture;
        win.titleContent = new GUIContent("JBoneReader", icon);

        win.target = null;
    }

    private bool canProcess
    {
        get
        {
            if(target != null && autoMatchData != null  
                && jVehicleFrame != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Box("JBoneReader", boxStyle, GUILayout.Height(60), GUILayout.ExpandWidth(true));
        GUILayout.Box("You can use this tool to read the bones to a VehicleFrame.", richTextStyle_Mid, GUILayout.ExpandWidth(true));

        GUILayout.Space(10);
        DrawALine(3);
        GUILayout.Space(10);

        GUILayout.Label("BasicSets", headerStyle);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Target");
        target = EditorGUILayout.ObjectField(target, typeof(GameObject), true) as GameObject;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("AutoMatchData");
        autoMatchData = EditorGUILayout.ObjectField(autoMatchData, typeof(JAnimation_AutoSetBoneMatchData), false) as JAnimation_AutoSetBoneMatchData;
        EditorGUILayout.EndHorizontal();
         

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("JVehicleFrame");
        jVehicleFrame = EditorGUILayout.ObjectField(jVehicleFrame, typeof(JVehicleFrame), false) as JVehicleFrame;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("AmplificateFactor");
        amplificateFactor = EditorGUILayout.Slider(amplificateFactor, 0f, 10f);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(30);

        if (GUILayout.Button("Process", GUILayout.Height(50)))
        {
            Process();
        }
    }

    private void Process()
    {
        if (canProcess)
        {
            BoneFramework boneFramework1 = new BoneFramework();
            if (
                BoneFramework.ReadBoneFrame(target, autoMatchData,
                out boneFramework1, amplificateFactor, jVehicleFrame))
            {
                jVehicleFrame.boneFramework = boneFramework1;
                Debug.Log("Process Done! (From JBoneReader)");
            }
            else
            {
                Debug.Log("Process Error! (From JBoneReader)");
            }
          
        }
    }
     

    void DrawALine(int _height)
    {
        GUILayout.Box("", lineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(_height));
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
