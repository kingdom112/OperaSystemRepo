using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub;
using Battlehub.RTEditor;
using Battlehub.RTCommon;
using UnityEngine.UI;

public class TimeLineWindow : RuntimeWindow
{
    public VFPlayer_InGame.UIclass1 ui1;
    public Button playButton_master;
    public Button playButton_student;
    public Button allpauseButton;

    protected override void AwakeOverride()
    {
        WindowType = RuntimeWindowType.Custom;
        base.AwakeOverride();

        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        if (vfplayer)
        {
            vfplayer.ui1 = ui1;
            vfplayer.m_timelineWindow = this;
            playButton_master.onClick.AddListener(delegate () { vfplayer.PlayButton(0); });
            playButton_student.onClick.AddListener(delegate () { vfplayer.PlayButton(1); });
            //allplayButton.onClick.AddListener(vfplayer.AllPlay);
            allpauseButton.onClick.AddListener(vfplayer.AllPause_Play);
            ui1.input_min.onEndEdit.AddListener(vfplayer.OnChangeTimeMin1);
            ui1.input_min2.onEndEdit.AddListener(vfplayer.OnChangeTimeMin2);
            ui1.input_max.onEndEdit.AddListener(vfplayer.OnChangeTimeMax1);
            ui1.input_max2.onEndEdit.AddListener(vfplayer.OnChangeTimeMax2);
            ui1.slider1.onValueChanged.AddListener(vfplayer.OnDragSlider);
            ui1.slider2.onValueChanged.AddListener(vfplayer.OnDragSlider2);

            //ztest
            
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
