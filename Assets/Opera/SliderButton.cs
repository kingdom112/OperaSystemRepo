using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderButton : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public AnimInfoWindowControl control;
    public int indexInControl = -1;
    public bool isleft = true;

    public RectTransform rect1;
    /// <summary>
    /// 当此值为false，拖动滑条或者TryChangeValue()都不可改变value
    /// </summary>
    public bool usingThis = true;
    [Range(0f, 1f)]
    private float value1;
    public float sliderWidth = 5f;

    /// <summary>
    /// 前面的限制
    /// </summary>
    public SliderButton limitFront;
    /// <summary>
    /// 后面的限制
    /// </summary>
    public SliderButton limitBack;

    /// <summary>
    /// 当这个按钮为范围类型时，有一个panel来表示他们之间的范围。
    /// </summary>
    public RectTransform panelRect;

    // Start is called before the first frame update
    void Start()
    {

    }

    

    private float lastpos;  //临时记录点击点与UI的相对位置

    public void OnDrag(PointerEventData eventData)
    {
        if (usingThis == false) return;
        float offset = eventData.position.x - lastpos;
        offset = offset / sliderWidth;
        float wantValue1 = Mathf.Clamp01(value1 + offset);
        TryChangeValue(value1 + offset); //try Change
        lastpos = eventData.position.x;
        //transform.position.x = eventData.position.x - offsetPos;
    }

    public void TryChangeValue(float _value1)
    {
        if (usingThis == false) return;
        float wantValue1 = Mathf.Clamp01(_value1);
        if (limitFront != null)
        {
            if (wantValue1 < limitFront.value1)
            {
                wantValue1 = limitFront.value1;
            } 
        }
        if (limitBack != null)
        {
            if (wantValue1 > limitBack.value1)
            {
                wantValue1 = limitBack.value1;
            } 
        }
        value1 = wantValue1; 
        if(isleft)
        {
            control.buttons[indexInControl].time1 = value1 * control._TimeLength + control._Time_Start;
        }
        else
        {
            control.buttons[indexInControl].time2 = value1 * control._TimeLength + control._Time_Start;
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {

        lastpos = eventData.position.x;
    }


    public void Refresh()
    {
        if (control != null)
        {
            float t_max = control._Time_Start + control._TimeLength;
            float t_min = control._Time_Start;
            if (isleft)
            {
                value1 = (control.buttons[indexInControl].time1 - control._Time_Start) / control._TimeLength;
                if (control.buttons[indexInControl].time1 >= t_min && control.buttons[indexInControl].time1 <= t_max)
                {
                    bool useTimeMagnet = false;
                    if(rect1.anchorMin.x != value1)
                    {
                        useTimeMagnet = true;
                    }
                    rect1.anchorMin = new Vector2(value1, 0f);
                    rect1.anchorMax = new Vector2(value1, 1f);
                    if(useTimeMagnet)
                    {
                        control.vfPlayer.ApplyTimeMagnet(control.isMaster); 
                    }
                    if (panelRect != null)
                    {
                        panelRect.anchorMin = new Vector2(gameObject.GetComponent<RectTransform>().anchorMin.x, panelRect.anchorMin.y);
                    }
                }
                else if (control.buttons[indexInControl].time1 < t_min)
                {
                    rect1.anchorMin = new Vector2(-0.3f, 0f);
                    rect1.anchorMax = new Vector2(-0.3f, 1f);
                    if (panelRect != null)
                    {
                        panelRect.anchorMin = new Vector2(-0.3f, panelRect.anchorMin.y);
                    }
                }
                else
                {
                    rect1.anchorMin = new Vector2(1.3f, 0f);
                    rect1.anchorMax = new Vector2(1.3f, 1f);
                    if (panelRect != null)
                    {
                        panelRect.anchorMin = new Vector2(1.3f, panelRect.anchorMin.y);
                    }
                }
            }
            else
            {
                value1 = (control.buttons[indexInControl].time2 - control._Time_Start) / control._TimeLength;
                if (control.buttons[indexInControl].time2 >= t_min && control.buttons[indexInControl].time2 <= t_max)
                {
                    bool useTimeMagnet = false;
                    if (rect1.anchorMin.x != value1)
                    {
                        useTimeMagnet = true;
                    }
                    rect1.anchorMin = new Vector2(value1, 0f);
                    rect1.anchorMax = new Vector2(value1, 1f);
                    if (useTimeMagnet)
                    {
                        control.vfPlayer.ApplyTimeMagnet(control.isMaster);
                    }
                    if (panelRect != null)
                    {
                        panelRect.anchorMax = new Vector2(gameObject.GetComponent<RectTransform>().anchorMax.x, panelRect.anchorMax.y);
                    }
                }
                else if (control.buttons[indexInControl].time2 < t_min)
                {
                    rect1.anchorMin = new Vector2(-0.3f, 0f);
                    rect1.anchorMax = new Vector2(-0.3f, 1f);
                    if (panelRect != null)
                    {
                        panelRect.anchorMax = new Vector2(-0.3f, panelRect.anchorMax.y);
                    }
                }
                else
                {
                    rect1.anchorMin = new Vector2(1.3f, 0f);
                    rect1.anchorMax = new Vector2(1.3f, 1f);
                    if (panelRect != null)
                    {
                        panelRect.anchorMax = new Vector2(1.3f, panelRect.anchorMax.y);
                    }
                }
            }

        }

    }

    // Update is called once per frame
    void Update()
    {

        Refresh();
    
    }
}
