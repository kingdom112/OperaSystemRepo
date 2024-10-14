using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCapturer : MonoBehaviour
{
    private VFPlayer_InGame vfplayer;
    private bool isEnable = false;
    private AnlyzeWindow_RightArea upController;

    private void Awake()
    {
        vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
    }

    public VFPlayer_InGame VFPlayer
    {
        get
        {
            return vfplayer;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public bool Using
    {
        get
        {
            return isEnable;
        }
    }
    public void SetUse(bool _use)
    {
        ispressed = false;
        isEnable = _use;
        vfplayer.SetUseAllScreenMask(isEnable);
        if(isEnable == false)
        {
            vfplayer.HideScreenShotRect();// 隐藏截屏元件
        }
    }

    /// <summary>
    /// 设置AnlyzeWindow_RightArea
    /// </summary>
    public void SetUpController(AnlyzeWindow_RightArea _upController)
    {
        upController = _upController;
    }
    private void ToUpController(Rect _rect)
    {
        if(upController == null)
        {
            Debug.LogError("upController = null !!!!!!");
            return;
        }
        upController.PassCaptureScreenshotRect(_rect);
    }

    bool ispressed = false;
    Vector2 pos1;
    Vector2 pos2;
    // Update is called once per frame
    void Update()
    {
        if (isEnable == false) return;

        if (Input.GetMouseButtonDown(0))
        {
            ispressed = true;
            pos1 = Input.mousePosition;
        } 
        if (Input.GetMouseButtonUp(0) && ispressed)
        {
            ispressed = false;
            pos2 = Input.mousePosition;
            ToUpController(vfplayer.GetScreenShotRect());
        }
        if (ispressed)
        {
            Vector2 _pos2 = Input.mousePosition;
            float x1 = pos1.x;
            float x2 = _pos2.x;
            float y1 = pos1.y;
            float y2 = _pos2.y;

            float xleft = 0;
            float xright = 0;
            float ybottom = 0;
            float ytop = 0;
            if (x1 <= x2)
            {
                xleft = x1;
                xright = Screen.width - x2;
            }
            else
            {
                xleft = x2;
                xright = Screen.width - x1;
            }
            if (y1 <= y2)
            {
                ybottom = y1;
                ytop = Screen.height - y2;
            }
            else
            {
                ybottom = y2;
                ytop = Screen.height - y1;
            }
            vfplayer.SetUseScreenShotRect(new Vector2(xleft, ybottom), new Vector2(-xright, -ytop)); 
        }

    }
}
