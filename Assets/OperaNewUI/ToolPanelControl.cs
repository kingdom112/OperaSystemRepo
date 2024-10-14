using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class ToolPanelControl : MonoBehaviour
{
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmndShow);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    const int SW_SHOWMINIMIZED = 2;
    const int SW_SHOWMAXIMIZED = 3;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MinSize()
    {
        ShowWindow(GetForegroundWindow(), 2);
    }

    public void Close()
    {
        Application.Quit();
    }

}
