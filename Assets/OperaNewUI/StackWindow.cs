using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub;
using Battlehub.RTEditor;
using Battlehub.RTCommon;
using UnityEngine.UI;

public class StackWindow : RuntimeWindow
{
    public RectTransform contentParent;
    public GameObject normalPage;
    public GameObject anlyzePage;
    public Button normalButton;
    public Button anlyzeButton;

    protected override void AwakeOverride()
    {
        WindowType = RuntimeWindowType.Custom;
        base.AwakeOverride();

        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        if (vfplayer)
        {
            RightStackController rightstack = GameObject.FindObjectOfType<RightStackController>();
            rightstack.ContentParent = contentParent;
            RightPageControl rightPageControl = GameObject.FindObjectOfType<RightPageControl>();
            rightPageControl.normalRight = normalPage;
            rightPageControl.AnlyzeRight = anlyzePage;
            normalButton.onClick.AddListener(rightPageControl.Button_ChangeToPageNormal);
            anlyzeButton.onClick.AddListener(rightPageControl.Button_ChangeToPageAnlyze);
            rightPageControl.Button_ChangeToPageNormal();//init page to normal
            vfplayer.m_stackWindow = this;
            vfplayer.InitUIs();
        }
    }

    protected override void OnDestroyOverride()
    {
        base.OnDestroyOverride();
        //Debug.Log("On Custom Window Destroy");
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        //Debug.Log("On Custom Window Activated");
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();
        //Debug.Log("On Custom Window Deactivated");
    }
}
