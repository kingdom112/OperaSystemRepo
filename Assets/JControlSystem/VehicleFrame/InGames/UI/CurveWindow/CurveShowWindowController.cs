using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CurveShowWindowController : MonoBehaviour, IScrollHandler
{
    private bool followMode = false;
    public bool timelineMode = false;
    public bool timelineMode_master = true;
    public Button followModeButton;
    public Button timelineModeButton;
    public CurveShowerController controller;
    public Drag_CurveShower dragControl;
    public Slider sliderX;
    public Slider sliderY;
    private float time_length = 0f;
    private float y_height
    {
        get
        {
            return Mathf.Abs(y_max - y_min);
        }
    }
    private float y_min = 0f;
    private float y_max = 0f;
    private AnimationCurve curve1;

    public float block_x = 10f;
    [Range(1f, 360f)]
    public float block_y = 5f;
    //[HideInInspector]
    public float blockSizeChange = 1f;
    private float blockSizeChangeSpeed = 0.01f;
    public float ySizeMult = 1f;
    public float xSizeMult = 1f;

    private VFPlayer_InGame vfPlayer = null;
    private BoneInfoWindowControl boneInfoWindow = null;

    public GameObject noCurveMask;

    void Awake()
    {
        boneInfoWindow = FindObjectOfType<BoneInfoWindowControl>();
        vfPlayer = FindObjectOfType<VFPlayer_InGame>();
        SetCurve(controller.curveShower.m_Curve, Color.white);
    }

    void Start()
    {
        InitXScale();
    }

    private void InitXScale()
    { 
        if (vfPlayer.ch_master.hasInit)
        {
            timelineMode_master = true;
        }
        else if (vfPlayer.ch_student.hasInit)
        {
            timelineMode_master = false;
        }
        TimeLineModeProcess(1, 1);//将显示范围初始化在timeline范围
        UpdateCurve();
    }
    void Update()
    {
        UpdateCurve();
    }

    public void UseNoCurveMask(bool _use)
    {
        noCurveMask.SetActive(_use);
    }

    private float nblockSize_x
    {
        get
        {
            return block_x * blockSizeChange;
        }
    }
    private float nblockSize_y
    {
        get
        {
            return y_height * blockSizeChange;
        }
    }
    private void TimeLineModeProcess(float Lerp_xSizeMult, float Lerp_sliderX)
    {
        float timelineXLength = 0f;
        float targetSliderX = 0f;
        if (timelineMode_master)
        {
            timelineXLength =
                vfPlayer.ch_master.max - vfPlayer.ch_master.min;
            float denominator = vfPlayer.ch_master.time - timelineXLength;
            if (denominator == 0f)
            {
                targetSliderX = 1f;
            }
            else
            {
                targetSliderX = vfPlayer.ch_master.min / denominator;
            } 
        }
        else
        {
            timelineXLength =
              vfPlayer.ch_student.max - vfPlayer.ch_student.min;
            float denominator = vfPlayer.ch_student.time - timelineXLength;
            if(denominator == 0f)
            {
                targetSliderX = 1f;
            }
            else
            {
                targetSliderX = vfPlayer.ch_student.min / denominator;
            }
        }
        float target_xSizeMult = timelineXLength / nblockSize_x;
        xSizeMult = Mathf.Lerp(xSizeMult, target_xSizeMult, Lerp_xSizeMult);
        sliderX.value = Mathf.Lerp(sliderX.value, targetSliderX, Lerp_sliderX);
    }

    private void UpdateCurve()
    {
        if(time_length <= 0f && y_height <= 0f)
        {
            return;
        }
        else
        {
            if(timelineMode)
            {
                dragControl.useX = false;
                TimeLineModeProcess(0.5f, 0.9f);
            }
            else
            {
                dragControl.useX = true;
            }
            float blockSize_x = nblockSize_x * xSizeMult;
            float blockSize_y = nblockSize_y * ySizeMult;

            if (time_length > 0f)
            {
                if (time_length > blockSize_x)
                {
                    float v1 = (time_length - blockSize_x) * sliderX.value + blockSize_x / 2f;
                    float left1 = v1 - blockSize_x / 2f;
                    float right1 = v1 + blockSize_x / 2f;
                    controller.curveShower.time_min = left1;
                    controller.curveShower.time_max = right1; 
                    sliderX.interactable = true;
                }
                else
                {/*
                    float left1 = curve1.keys[0].time;
                    float right1 = curve1.keys[curve1.keys.Length - 1].time;
                    controller.curveShower.time_min = left1;
                    controller.curveShower.time_max = right1; 
                    sliderX.interactable = false;*/
                    float v1 = blockSize_x - time_length;
                    float left1 = curve1.keys[0].time - v1 / 2f;
                    float right1 = curve1.keys[curve1.keys.Length - 1].time + v1 / 2f;
                    controller.curveShower.time_min = left1;
                    controller.curveShower.time_max = right1;
                    sliderX.interactable = false;
                }
            }
            controller.lineShower_X.Xchu = controller.curveShower.time_max - controller.curveShower.time_min;//
            controller.lineShower_X.x_min = controller.curveShower.time_min;
            controller.lineShower_Time_master.Xchu = controller.curveShower.time_max - controller.curveShower.time_min;//
            controller.lineShower_Time_master.x_min = controller.curveShower.time_min;
            controller.lineShower_Time_student.Xchu = controller.curveShower.time_max - controller.curveShower.time_min;//
            controller.lineShower_Time_student.x_min = controller.curveShower.time_min;

            if (y_height > 0f)
            {
                if (y_height > blockSize_y)
                {
                    float bottom = y_min + (y_height - blockSize_y) * sliderY.value;
                    float top = bottom + blockSize_y; 
                    controller.curveShower.y_min = bottom;
                    controller.curveShower.y_max = top;  
                    //Debug.Log("blockSize_y: "+ blockSize_y+",bottom: " + bottom + ",top: " + top+",y_max: "+y_max+", y_min: "+y_min+ ", y_height: "+ y_height);
                    sliderY.interactable = true;
                }
                else
                { 
                    float v1 = blockSize_y - y_height;
                    float bottom = y_min - v1 / 2f;
                    float top = y_max + v1 / 2f;
                    controller.curveShower.y_min = bottom;
                    controller.curveShower.y_max = top;
                    //Debug.Log("blockSize_y: " + blockSize_y + ",bottom: " + bottom + ",top: " + top);
                    sliderY.interactable = false;
                    sliderY.value = 0.5f;
                    //Debug.Log("y_height > blockSize_y, y_height:" + y_height + ",blockSize_y:" + blockSize_y);
                }
            }
            controller.lineShower.y_min = controller.curveShower.y_min;

            float jiange1 = controller.curveShower.y_division / 5f;
            jiange1 = myCalculate.GetNearFloat(jiange1);
            /*
            string str_jiange1 = jiange1.ToString(); 
            if(int.Parse(""+str_jiange1[str_jiange1.Length - 1]) >= 5)
            {
                jiange1 = Mathf.Ceil(jiange1 * 10f) / 10f;
            }
            else
            {
                jiange1 = Mathf.Floor(jiange1 * 10f) / 10f;
            }*/
            controller.lineShower.Lines.Clear();
            float linePos = ((float)((int)(controller.curveShower.y_min / jiange1))) * jiange1;

            if(linePos < controller.curveShower.y_min)
            {
                linePos += jiange1;
            }
            controller.lineShower.Lines.Add(linePos);
            for (int k=0; k<100; k++)
            {
                linePos += jiange1;
                //linePos = Mathf.Floor(linePos * 10f) / 10f;
                if (linePos <= controller.curveShower.y_max)
                {
                    controller.lineShower.Lines.Add(linePos);
                }
                else
                {
                    break;
                }
            }

            float jiange_X = controller.lineShower_X.Xchu / 8f;
            jiange_X = myCalculate.GetNearFloat(jiange_X);
            /*str_jiange1 = jiange_X.ToString();
          
            if (int.Parse("" + str_jiange1[str_jiange1.Length - 1]) >= 5)
            {
                jiange_X = Mathf.Ceil(jiange_X * 10f) / 10f;
            }
            else
            {
                jiange_X = Mathf.Floor(jiange_X * 10f) / 10f;
            }*/
            //Debug.Log(jiange_X);
            controller.lineShower_X.Lines.Clear();
            controller.lineShower_X.Lines.Add(controller.curveShower.time_min);
            controller.lineShower_X.Lines.Add(controller.curveShower.time_max);
            linePos = ((float)((int)(controller.lineShower_X.x_min / jiange_X))) * jiange_X;
            if (linePos < controller.lineShower_X.x_min)
            {
                linePos += jiange_X;
            }
            controller.lineShower_X.Lines.Add(linePos);
            //Debug.Log(controller.curveShower.time_max);
            for (int k = 0; k < 100; k++)
            {
                linePos += jiange_X;
                //Debug.Log("linePosold:"+ linePos);
                //linePos = Mathf.Floor(linePos * 10f) / 10f;
                //Debug.Log("linePosnew: "+ linePos);
                if (linePos <= controller.curveShower.time_max)
                {
                    controller.lineShower_X.Lines.Add(linePos);
                }
                else
                {
                    break;
                }
            }

            controller.lineShower_Time_master.Lines.Clear();
            controller.lineShower_Time_student.Lines.Clear();

            if (boneInfoWindow == null)
            {
                boneInfoWindow = FindObjectOfType<BoneInfoWindowControl>();
            }
            if (boneInfoWindow != null)
            {
                if(boneInfoWindow.curveSelectDropdown.value == 3 || boneInfoWindow.curveSelectDropdown.value == 4)
                {
                    if (followMode)
                    {
                        //if (followMode_2 == false)
                        {
                            float offset1 = vfPlayer.ch_student.timer - vfPlayer.ch_master.timer;
                            for (int i = 0; i < controller.AddedCurves.Count; i++)
                            {
                                controller.AddedCurves[i].time_offset = offset1;
                            }
                        }

                        if (vfPlayer.ch_master.timer >= controller.curveShower.time_min && vfPlayer.ch_master.timer <= controller.curveShower.time_max)
                        {
                            controller.lineShower_Time_master.Lines.Add(vfPlayer.ch_master.timer);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < controller.AddedCurves.Count; i++)
                        {
                            controller.AddedCurves[i].time_offset = 0f;
                        }


                        if (vfPlayer.ch_master.timer >= controller.curveShower.time_min && vfPlayer.ch_master.timer <= controller.curveShower.time_max)
                        {
                            controller.lineShower_Time_master.Lines.Add(vfPlayer.ch_master.timer);
                        }
                        if (vfPlayer.ch_student.timer >= controller.curveShower.time_min && vfPlayer.ch_student.timer <= controller.curveShower.time_max)
                        {
                            controller.lineShower_Time_student.Lines.Add(vfPlayer.ch_student.timer);
                        }
                    }
                    controller.Refresh();
                    return;
                }
            }

            if (followMode)
            {
                if(vfPlayer.ch_master.selectBones.Count > 0 && vfPlayer.ch_student.selectBones.Count > 0)
                {
                    //if (followMode_2 == false)
                    {
                        float offset1 = vfPlayer.ch_student.timer - vfPlayer.ch_master.timer;
                        for (int i = 0; i < controller.AddedCurves.Count; i++)
                        {
                            controller.AddedCurves[i].time_offset = offset1;
                        }
                    } 

                    if (vfPlayer.ch_master.timer >= controller.curveShower.time_min && vfPlayer.ch_master.timer <= controller.curveShower.time_max)
                    {
                        controller.lineShower_Time_master.Lines.Add(vfPlayer.ch_master.timer);
                    } 
                }
                else
                {
                    followMode = false;
                    followModeButton.GetComponent<Image>().color = Color.white;
                    for (int i = 0; i < controller.AddedCurves.Count; i++)
                    {
                        controller.AddedCurves[i].time_offset = 0f;
                    }

                    if (vfPlayer.ch_master.selectBones.Count > 0 && vfPlayer.ch_master.timer >= controller.curveShower.time_min && vfPlayer.ch_master.timer <= controller.curveShower.time_max)
                    {
                        controller.lineShower_Time_master.Lines.Add(vfPlayer.ch_master.timer);
                    }
                    if (vfPlayer.ch_student.selectBones.Count > 0 && vfPlayer.ch_student.timer >= controller.curveShower.time_min && vfPlayer.ch_student.timer <= controller.curveShower.time_max)
                    {
                        controller.lineShower_Time_student.Lines.Add(vfPlayer.ch_student.timer);
                    }
                }
           
            }
            else
            {
                for(int i=0; i< controller.AddedCurves.Count; i++)
                {
                    controller.AddedCurves[i].time_offset = 0f;
                }


                if (vfPlayer.ch_master.selectBones.Count > 0 && vfPlayer.ch_master.timer >= controller.curveShower.time_min && vfPlayer.ch_master.timer <= controller.curveShower.time_max)
                {
                    controller.lineShower_Time_master.Lines.Add(vfPlayer.ch_master.timer);
                }
                if (vfPlayer.ch_student.selectBones.Count > 0 && vfPlayer.ch_student.timer >= controller.curveShower.time_min && vfPlayer.ch_student.timer <= controller.curveShower.time_max)
                {
                    controller.lineShower_Time_student.Lines.Add(vfPlayer.ch_student.timer);
                }
            }


            controller.Refresh();
        }
        
    }

    public void Button_TimelineMode()
    {
        if(timelineMode == false)
        {
            if(vfPlayer.ch_master.hasInit)
            {
                timelineMode_master = true;
                timelineMode = true;
            }
            else if(vfPlayer.ch_student.hasInit)
            {
                timelineMode_master = false;
                timelineMode = true;
            }
         
        }
        else
        {
            if(timelineMode_master == true)
            {
                if(vfPlayer.ch_student.hasInit)
                {
                    timelineMode_master = false;
                }
                else
                {
                    timelineMode = false;
                } 
            }
            else
            {
                timelineMode = false;
            }
        }
        if(timelineMode)
        {
            if(timelineMode_master)
            {
                timelineModeButton.GetComponent<Image>().color = Color.red;
                timelineModeButton.GetComponentInChildren<Text>().text = "x大师缩放模式";
            }
            else
            {
                timelineModeButton.GetComponent<Image>().color = Color.blue;
                timelineModeButton.GetComponentInChildren<Text>().text = "x用户缩放模式";
            }
        }
        else
        {
            timelineModeButton.GetComponent<Image>().color = Color.white;
            timelineModeButton.GetComponentInChildren<Text>().text = "x无缩放模式";
        }
    }
    public void Button_FollowMode()
    {
        if(followMode)
        {
            followMode = false;
        }
        else
        {
            if (vfPlayer.ch_master.hasInit && vfPlayer.ch_student.hasInit)
            {
                followMode = true;
            } 
        }
        if(followMode)
        {
            followModeButton.GetComponent<Image>().color = Color.green;
            vfPlayer.SetPlayButtonActive(false); 
            if(vfPlayer.ch_master.isplaying || vfPlayer.ch_student.isplaying)
            {
                vfPlayer.ch_master.isplaying = true;
                vfPlayer.ch_student.isplaying = true;
            }
        }
        else
        {
            followModeButton.GetComponent<Image>().color = Color.white;
            vfPlayer.SetPlayButtonActive(true);
        }
    }

    /// <summary>
    /// 鼠标滚轮
    /// </summary>
    /// <param name="eventData"></param>
    public void OnScroll(PointerEventData eventData)
    {
        float delta_ = 0f;
        if (Mathf.Abs(eventData.scrollDelta.x) > Mathf.Abs(eventData.scrollDelta.y))
            delta_ = eventData.scrollDelta.x;
        else delta_ = eventData.scrollDelta.y; 

        if (blockSizeChange - blockSizeChangeSpeed * delta_ < 0.01f)
        {
            blockSizeChange = 0.01f;
        }
        else
        {
            blockSizeChange = blockSizeChange - blockSizeChangeSpeed * delta_;
        }
        Debug.Log("delta:" + delta_);
        Debug.Log("OnScroll: blockSizeChange:" + blockSizeChange);
    }

    private float GetCurveMin(AnimationCurve _curve)
    {
        if (_curve.keys.Length == 0)
        {
            return 0f;
        }
        float min1 = _curve.keys[0].value;
        for (int i = 1; i < _curve.keys.Length; i++)
        {
            if (_curve.keys[i].value < min1)
            {
                min1 = _curve.keys[i].value;
            }
        }
        return min1;
    }
    private float GetCurveMax(AnimationCurve _curve)
    {
        if (_curve.keys.Length == 0)
        {
            return 0f;
        }
        float max1 = _curve.keys[0].value;
        for (int i = 1; i < _curve.keys.Length; i++)
        {
            if (_curve.keys[i].value > max1)
            {
                max1 = _curve.keys[i].value;
            }
        }
        return max1;
    }
    public void SetY_Min(float _value)
    {
        y_min = _value;
    }
    public void SetY_Max(float _value)
    {
        y_max = _value;
    }
    public void SetCurve(AnimationCurve _curve, float _y_max, float _y_min, Color color1)
    {
        if (_curve == null)
        {
            return;
        }
        time_length = _curve.keys[_curve.keys.Length - 1].time - _curve.keys[0].time;
        float heightTemp = Mathf.Abs(_y_max - _y_min);
        y_min = _y_min - Mathf.Abs(heightTemp * 0);
        y_max = _y_max + Mathf.Abs(heightTemp * 0);
        //y_height = y_max - y_min;
        curve1 = _curve;
        controller.SetCurve(_curve, color1);
        Debug.Log("y_max: " + y_max + ", y_min: " + y_min);

        block_y = Mathf.Abs(y_max - y_min) * 0.3f;
        blockSizeChange = 1f;
        ySizeMult = 5f;
        xSizeMult = 1f;
        InitXScale();
    }
    public void SetCurve(AnimationCurve _curve, Color color1)
    {
        if (_curve == null)
        {
            return;
        }
        time_length = _curve.keys[_curve.keys.Length - 1].time - _curve.keys[0].time;
        y_min = GetCurveMin(_curve);
        y_max = GetCurveMax(_curve);
        float heightTemp = Mathf.Abs(y_max - y_min);
        y_min = y_min - Mathf.Abs(heightTemp * 0);
        y_max = y_max + Mathf.Abs(heightTemp * 0);
        //y_height = y_max - y_min;
        curve1 = _curve;
        controller.SetCurve(_curve, color1);
        Debug.Log("y_max: " + y_max + ", y_min: " + y_min);

        block_y = Mathf.Abs(y_max - y_min) * 0.3f;
        blockSizeChange = 1f;
        ySizeMult = 5f;
        xSizeMult = 1f;
        InitXScale();
    }

    /// <summary>
    /// 设置幅曲线
    /// </summary>
    public void SetCurves(List<AnimationCurve> curves, List<Color> colors)
    { 
        controller.ReSizeAddedCurves(curves.Count);
        for (int i = 0; i < curves.Count; i++)
        {
            controller.AddedCurves[i].m_Curve = curves[i];
            controller.AddedCurves[i].color = colors[i];
        }
    }


    public void OnCloseButtonPressed()
    {
        vfPlayer.SetPlayButtonActive(true);
        vfPlayer.HideCurveShowWindow();
    }
}
