using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using JVehicleFrameSystem;
using JAnimationSystem;

 
public class floorCreater : EditorWindow
{

    GUIStyle boxStyle;
    GUIStyle richTextStyle_Mid;
    GUIStyle richTextStyle_Left;
    GUIStyle headerStyle;
    GUIStyle smallHeaderStyle;
    GUIStyle lineStyle;

    GameObject prefab;
    Transform parent;
    int count_x = 5;
    int count_y = 5;
    float celSize = 10;
    float scale = 2f;
    [MenuItem("JTools/FloorCreater")]
    static void Open()
    {
        var win = EditorWindow.GetWindow<floorCreater>(false, "FloorCreater", true);
        win.GenerateStyles();
        var icon = Resources.Load("Textures/jicon2") as Texture;
        win.titleContent = new GUIContent("FloorCreater", icon);
         
    }

    void OnGUI()
    {
        GUILayout.Box("FloorCreater", boxStyle, GUILayout.Height(60), GUILayout.ExpandWidth(true));
        GUILayout.Box("You can use this tool to create floors", richTextStyle_Mid, GUILayout.ExpandWidth(true));

        GUILayout.Space(10);
        DrawALine(3);
        GUILayout.Space(10);

        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;
        parent = EditorGUILayout.ObjectField("parent", parent, typeof(Transform), true) as Transform;
        count_x = EditorGUILayout.IntField("count_x", count_x);
        count_y = EditorGUILayout.IntField("count_y", count_y);
        celSize = EditorGUILayout.FloatField("celSize", celSize);
        scale = EditorGUILayout.FloatField("scale", scale);

        GUILayout.Space(10);
        DrawALine(3);
        GUILayout.Space(10);

        if (GUILayout.Button("Create"))
        {
            if(prefab != null)
            {
                int xc = count_x / 2;
                int xy = count_y / 2;

                Vector3 ps = new Vector3(-((float)xc ) * scale * celSize, 0f, -((float)xy ) * scale * celSize);
                for (int i = 0; i < count_x; i++)
                {
                    for (int j = 0; j < count_y; j++)
                    {
                        Vector3 p1 = ps + Vector3.right * i * scale* celSize + Vector3.forward * j * scale * celSize;
                        GameObject ga1 = Instantiate<GameObject>(prefab);
                        ga1.transform.position = p1;
                        ga1.transform.localScale = new Vector3(scale, 1f, scale);
                        ga1.transform.SetParent(parent);
                    }
                }
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
