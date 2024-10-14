using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTCommon;
using Battlehub.RTEditor;
using Battlehub.UIControls.MenuControl;


[MenuDefinition]
public static class VFCustomMenu
{
    //Help  [Main & Context Menu]
    [MenuCommand("Record/Record UI")]
    public static void UseRecord()
    {
        Debug.Log("UseRecord");
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        if(vfplayer != null)
        {
            vfplayer.UseRecordUI();
        }
    } 

}
