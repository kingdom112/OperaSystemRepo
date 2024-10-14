using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub;
using Battlehub.RTEditor;
using Battlehub.RTCommon;
using UnityEngine.UI;

public class OperaCurveWindow : RuntimeWindow
{
    public CurveShowWindowController curveShowWindowControl;


    protected override void AwakeOverride()
    {
        WindowType = RuntimeWindowType.Custom;
        base.AwakeOverride();


        VFPlayer_InGame vfplayer = FindObjectOfType<VFPlayer_InGame>();
        vfplayer.curveShowWindowControl = curveShowWindowControl;
    }

}
