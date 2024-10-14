using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckWindow : MonoBehaviour
{
    public GUISkin guiSkin;

    public string windowName = "CheckWindow";
    Rect windowRect = new Rect(0, 0, 400, 130);
    private string labelContent = "sure check?";
    private string buttonName1 = "确认";
    private string buttonName2 = "取消";
    private bool isUsing = false;
    public GameObject background;
    public Action checkButtonAction;
    public Action cancelButtonAction;

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
        if (isUsing)
        {
            if (background.activeSelf == false)
            {
                background.SetActive(true);
            }
            windowRect = GUI.Window(2, windowRect, DrawCheckWindow, windowName);
        }
        else
        {
            if (background.activeSelf == true)
            {
                background.SetActive(false);
            }
        }
    }

    public void Show(Action _checkAction, Action _cancelAction, string _labelContent = "????", string _windowName = "CheckWindow")
    {
        labelContent = _labelContent;
        windowName = _windowName;
        if(_checkAction != null)
        {
            checkButtonAction = _checkAction;
        }
        if (_cancelAction != null)
        {
            cancelButtonAction = _cancelAction;
        }
        isUsing = true;
    }

    private float buttonWidht = 50;
    void DrawCheckWindow(int windowID)
    {
        GUILayout.Space(20);
        GUILayout.Label(labelContent);
        GUILayout.Space(30);
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        if(GUILayout.Button(buttonName1, GUILayout.Width(buttonWidht)))
        {
            checkButtonAction();
            isUsing = false;
        } 
        GUILayout.FlexibleSpace();
        if(GUILayout.Button(buttonName2, GUILayout.Width(buttonWidht)))
        {
            cancelButtonAction();
            isUsing = false;
        }
        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }
}
