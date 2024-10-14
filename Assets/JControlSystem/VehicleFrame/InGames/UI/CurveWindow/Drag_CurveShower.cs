using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Drag_CurveShower : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    /// <summary>
    /// 当此值为false，拖动滑条或者TryChangeValue()都不可改变value
    /// </summary>
    public bool usingThis = true;
    public bool useX = true;
    public bool useY = true;
    public CurveShowWindowController curveShowWindowControl;
    
    /// <summary>
    /// 拖动长度参照
    /// </summary>
    public Slider refSlider_X;
    /// <summary>
    /// 拖动长度参照
    /// </summary>
    public Slider refSlider_Y;
    [Range(0f,1f)]
    public float dragSmooth = 0.5f;

    private Vector2 lastpos;  //临时记录点击点与UI的相对位置

     

    public void OnDrag(PointerEventData eventData)
    {
        if (usingThis == false) return; 
        float offset_x = eventData.position.x - lastpos.x;
        float offset_y = eventData.position.y - lastpos.y;
        float addx = offset_x / refSlider_X.gameObject.GetComponent<RectTransform>().rect.width ;
        float addy = offset_y / refSlider_Y.gameObject.GetComponent<RectTransform>().rect.height ;
        if (curveShowWindowControl.sliderX.interactable && useX)
        {
            float targetV = curveShowWindowControl.sliderX.value - addx;
            targetV = Mathf.Clamp01(targetV);
            curveShowWindowControl.sliderX.value = targetV;
        }
        if (curveShowWindowControl.sliderY.interactable && useY)
        {
            float targetV = curveShowWindowControl.sliderY.value - addy;
            targetV = Mathf.Clamp01(targetV);
            curveShowWindowControl.sliderY.value = targetV;
        }
        lastpos = eventData.position;
    }


    public void OnPointerDown(PointerEventData eventData)
    { 
        lastpos = eventData.position;
    }

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {  
    }
}
