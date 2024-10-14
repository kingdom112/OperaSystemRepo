using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CameraController : MonoBehaviour
{
    public VFPlayer_InGame VFPlayer;
    public Camera cam_main;
    public Camera cam_2;
    [HideInInspector]
    public ScreenMode screenMode = ScreenMode.One;
    public enum ScreenMode
    {
        One,
        Two,
        FullScreen
    }

    private void Awake()
    {
        SetCamMode(ScreenMode.One);
    }
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.F1))
        {
            SetCamMode(ScreenMode.One);
        }
        if (Input.GetKeyUp(KeyCode.F2))
        {
            SetCamMode(ScreenMode.Two);
        }
    }
    void Start()
    {
       
    }


    private void LateUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true) return;
        float x = Input.mousePosition.x / Screen.width;
        float y = Input.mousePosition.y / Screen.height;
        if(screenMode == ScreenMode.One)
        {
            if(cam_IsInScreen_One(x, y))
            {
                camerarotate(CamCount.One);
                camerazoom(CamCount.One);
            }
        }
        else
        {
            if (cam_IsInScreen_Two(x, y, CamNum.One))
            {
                camerarotate(CamCount.Two, CamNum.One);
                camerazoom(CamCount.Two, CamNum.One);
            }
            else if(cam_IsInScreen_Two(x, y, CamNum.Two))
            {
                camerarotate(CamCount.Two, CamNum.Two);
                camerazoom(CamCount.Two, CamNum.Two);
            }
        }
    }

    public void ChangeScreenModeInOneOrTwo()
    {
        if(screenMode == ScreenMode.One)
        {
            SetCamMode(ScreenMode.Two);
        }
        else if(screenMode == ScreenMode.Two || screenMode == ScreenMode.FullScreen)
        {
            SetCamMode(ScreenMode.One);
        }
    }

    private void SetCamMode(ScreenMode mode)
    {
        if (cam_main == null || cam_2 == null) return;

        if(mode == ScreenMode.One)
        {
            screenMode = ScreenMode.One;
            SetCamPos_One(cam_main);
            cam_main.gameObject.SetActive(true);
            cam_2.gameObject.SetActive(false);
            cam_main.cullingMask |= (1 << LayerMask.NameToLayer("cam1"));//打开cam1层
            cam_main.cullingMask |= (1 << LayerMask.NameToLayer("cam2"));//打开cam2层
        }
        else if(mode == ScreenMode.Two)
        {
            screenMode = ScreenMode.Two;
            SetCamPos_Two(cam_main, cam_2);
            cam_main.gameObject.SetActive(true);
            cam_2.gameObject.SetActive(true);
            cam_main.cullingMask &= ~(1 << LayerMask.NameToLayer("cam1")); // 关闭cam1层
            cam_main.cullingMask |= (1 << LayerMask.NameToLayer("cam2"));//打开cam2层
        }
        else if(mode == ScreenMode.FullScreen)
        {

        }
    }

    private bool cam_IsInScreen_One(float _x, float _y)
    {
        if(_x >= 0.2f && _x <= 0.8f && _y >= 0.2f && _y <= 0.9f)
        {
            return true;
        }else
        {
            return false;
        }
    }
    private bool cam_IsInScreen_Two(float _x, float _y, CamNum camNum)
    {
        if(camNum == CamNum.One)
        {
            if (_x >= 0.2f && _x <= 0.8f && _y >= 0.2f && _y <= 0.55f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (_x >= 0.2f && _x <= 0.8f && _y > 0.55f && _y <= 0.9f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
       
    }

    public enum CamNum
    {
        One,
        Two
    }
    public enum CamCount
    {
        One,
        Two
    }
    private void camerarotate(CamCount camCount, CamNum camNum = CamNum.One) //摄像机围绕目标旋转操作
    {
        var mouse_x = Input.GetAxis("Mouse X");//获取鼠标X轴移动
        var mouse_y = -Input.GetAxis("Mouse Y");//获取鼠标Y轴移动

        if (Input.GetMouseButton(2))
        {
            if(camCount == CamCount.One || (camCount == CamCount.Two && camNum == CamNum.One))
            {
                cam_main.transform.Translate(Vector3.left * (mouse_x * 15f) * Time.deltaTime);
                cam_main.transform.Translate(Vector3.up * (mouse_y * 15f) * Time.deltaTime);
            }
            else
            {
                cam_2.transform.Translate(Vector3.left * (mouse_x * 15f) * Time.deltaTime);
                cam_2.transform.Translate(Vector3.up * (mouse_y * 15f) * Time.deltaTime);
            }
          
        }

        if (Input.GetMouseButton(1))
        {
            if(camCount == CamCount.One)
            {
                if (VFPlayer.ch_master.createdGA == null && VFPlayer.ch_student.createdGA == null)
                {
                    cam_main.transform.RotateAround(Vector3.zero, Vector3.up, mouse_x * 5);
                    cam_main.transform.RotateAround(Vector3.zero, Camera.main.transform.right, mouse_y * 5);
                }
                else
                {
                    cam_main.transform.RotateAround(VFPlayer.ch_master.createdGA ? VFPlayer.ch_master.createdGA.transform.position : VFPlayer.ch_student.createdGA.transform.position, Vector3.up, mouse_x * 5);
                    cam_main.transform.RotateAround(VFPlayer.ch_master.createdGA ? VFPlayer.ch_master.createdGA.transform.position : VFPlayer.ch_student.createdGA.transform.position, Camera.main.transform.right, mouse_y * 5);
                }
            }
            else
            {
                if(camNum == CamNum.One)
                {
                    if (VFPlayer.ch_master.createdGA == null)
                    {
                        cam_main.transform.RotateAround(Vector3.zero, Vector3.up, mouse_x * 5);
                        cam_main.transform.RotateAround(Vector3.zero, cam_main.transform.right, mouse_y * 5);
                    }
                    else
                    {
                        cam_main.transform.RotateAround(VFPlayer.ch_master.createdGA.transform.position, Vector3.up, mouse_x * 5);
                        cam_main.transform.RotateAround(VFPlayer.ch_master.createdGA.transform.position, cam_main.transform.right, mouse_y * 5);
                    }
                }
                else
                {
                    if (VFPlayer.ch_student.createdGA == null)
                    {
                        cam_2.transform.RotateAround(Vector3.zero, Vector3.up, mouse_x * 5);
                        cam_2.transform.RotateAround(Vector3.zero, cam_2.transform.right, mouse_y * 5);
                    }
                    else
                    {
                        cam_2.transform.RotateAround(VFPlayer.ch_student.createdGA.transform.position, Vector3.up, mouse_x * 5);
                        cam_2.transform.RotateAround(VFPlayer.ch_student.createdGA.transform.position, cam_2.transform.right, mouse_y * 5);
                    }
                }
            }
         
           
        }
       
       
    }

    private void camerazoom(CamCount camCount, CamNum camNum = CamNum.One) //摄像机滚轮缩放
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if(camCount == CamCount.One || (camCount == CamCount.Two && camNum == CamNum.One))
            {
                cam_main.transform.Translate(Vector3.forward * 0.5f);
            }
            else
            {
                cam_2.transform.Translate(Vector3.forward * 0.5f);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (camCount == CamCount.One || (camCount == CamCount.Two && camNum == CamNum.One))
            {
                cam_main.transform.Translate(Vector3.forward * -0.5f);
            }
            else
            {
                cam_2.transform.Translate(Vector3.forward * -0.5f);
            }
        }
    }

    private void SetCamPos_One(Camera _cam)
    {
        Rect newRect = new Rect();
        newRect.x = 0.2f;
        newRect.y = 0.2f;
        newRect.width = 0.6f;
        newRect.height = 0.7f;
        _cam.rect = newRect;
    }
    private void SetCamPos_Two(Camera _cam1, Camera _cam2)
    {
        Rect newRect = new Rect();
        newRect.x = 0.2f;
        newRect.y = 0.2f;
        newRect.width = 0.6f;
        newRect.height = 0.35f;
        _cam1.rect = newRect;
        Rect newRect2 = new Rect();
        newRect2.x = 0.2f;
        newRect2.y = 0.55f;
        newRect2.width = 0.6f;
        newRect2.height = 0.35f;
        _cam2.rect = newRect2;
    }
   
}
