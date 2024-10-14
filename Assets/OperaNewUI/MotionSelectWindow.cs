using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub;
using Battlehub.RTEditor;
using Battlehub.RTCommon;
using UnityEngine.UI;

public class MotionSelectWindow : RuntimeWindow
{
    //用了Battlehub的用法，但是只是函数写法，引用在别的地方 针对窗口写的。
    public Dropdown dp_master;//下拉选择窗口 大师
    public Dropdown dp_student;

    protected override void AwakeOverride()
    {
        WindowType = RuntimeWindowType.Custom;
        base.AwakeOverride();

        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();//获取挂载了VFPlayer_InGame脚本的游戏对象 ，在这个项目里就是controller1 这个游戏对象下面有 Dd_Select_master student
        if(vfplayer)
        {//监听点击下拉窗口的点击事件
            dp_master.onValueChanged.AddListener(vfplayer.dropdown_selectMaster_OnChange);
            dp_student.onValueChanged.AddListener(vfplayer.dropdown_selectStudent_OnChange);
            vfplayer.Dd_Select_Master = dp_master;
            vfplayer.Dd_Select_Student = dp_student;
            vfplayer.m_motionSelectWindow = this;//这个代码要回头试一下
            vfplayer.InitUIs();
        }
    }

    protected override void OnDestroyOverride()
    {
        base.OnDestroyOverride();
        Debug.Log("On Custom Window Destroy");
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        Debug.Log("On Custom Window Activated");
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();
        Debug.Log("On Custom Window Deactivated");
    }
}
