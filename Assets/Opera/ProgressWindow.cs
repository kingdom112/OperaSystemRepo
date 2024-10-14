using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressWindow : MonoBehaviour
{

    public GUISkin guiSkin;

    public string windowName = "ProgressWindow";
    Rect windowRect = new Rect(0, 0, 400, 130);
   
    [Range(0,1)]
    public float hSbarValue1 = 0;
    [Range(0, 1)]
    public float hSbarValue2 = 0;
    private string progressName1 = "name1";
    private string progressName2 = "name2";
    public string addName1 = "";
    public string addName2 = "";
    private bool isUsing = false;
    public GameObject background;

    void Start()
    {
        windowRect.x = (Screen.width - windowRect.width) / 2;
        windowRect.y = (Screen.height - windowRect.height) / 2;
    }
    public void Use(bool _use = true)
    {
        isUsing = _use;
    }

    void OnGUI()
    {
        GUI.skin = guiSkin;
        if(isUsing)
        {
            if(background.activeSelf == false)
            {
                background.SetActive(true);
            }
            windowRect = GUI.Window(1, windowRect, DrawProgressWindow, windowName);
        }
        else
        {
            if (background.activeSelf == true)
            {
                background.SetActive(false);
            }
        }
    }

    public void Show(string _progressName1 , string _addName1, float _progressValue1, string _progressName2, string _addName2, float _progressValue2, string _windowName = "ProgressWindow")
    {
        progressName1 = _progressName1;
        addName1 = _addName1;
        hSbarValue1 = _progressValue1;
        progressName2 = _progressName2;
        addName2 = _addName2;
        hSbarValue2 = _progressValue2;
        windowName = _windowName;
        isUsing = true;
    }
    public void Close()
    {
        isUsing = false;
    }

    void DrawProgressWindow(int windowID)
    {
        GUILayout.Space(20);
        GUILayout.Label(progressName1 + "   " + addName1);
        GUILayout.HorizontalScrollbar(0, hSbarValue1, 0.0f, 1.0f);
        GUILayout.Label(progressName2 + "   " + addName2);
        GUILayout.HorizontalScrollbar(0, hSbarValue2, 0.0f, 1.0f);

        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }
}
