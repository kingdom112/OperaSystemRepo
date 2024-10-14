using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayButton : ButtonImageControl
{

    public VFPlayer_InGame vfplayer;
    public bool IsMaster;

    public void Update()
    {
        if (vfplayer == null)
        {
            vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
            return;
        }
        if (IsMaster)
        {
            if (vfplayer.ch_master.isplaying)
            {
                image.overrideSprite = image2;
            }
            else
            {
                image.overrideSprite = image1;
            }
        }
        else
        {
            if (vfplayer.ch_student.isplaying)
            {
                image.overrideSprite = image2;
            }
            else
            {
                image.overrideSprite = image1;
            }
        }
    }

    public override void ButtonPress()
    {
        base.ButtonPress();

        
    }

}
