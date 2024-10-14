using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag_CurveShowerXSize : MonoBehaviour, IDragHandler, IPointerDownHandler
{ 
    public CurveShowWindowController curveShowWindowControl;
     
    [Range(0.1f, 1f)]
    public float dragSpeed = 1f;
    [Range(1f, 20f)]
    public float dragSpeed2 = 1f;
    private Vector2 lastpos;  //临时记录点击点与UI的相对位置



    public void OnDrag(PointerEventData eventData)
    { 
        float offset_x = eventData.position.x - lastpos.x; 
        float addx = offset_x / gameObject.GetComponent<RectTransform>().rect.width * dragSpeed * dragSpeed2; 
        if(curveShowWindowControl.timelineMode == false)
        {
            curveShowWindowControl.xSizeMult = Mathf.Clamp(curveShowWindowControl.xSizeMult - addx, 0.001f, 100f);
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
