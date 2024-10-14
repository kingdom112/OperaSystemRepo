using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag_CurveShowerYSize : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public CurveShowWindowController curveShowWindowControl;

    [Range(0.1f, 1f)]
    public float dragSpeed = 1f;

    private Vector2 lastpos;  //临时记录点击点与UI的相对位置



    public void OnDrag(PointerEventData eventData)
    {
        float offset_y = eventData.position.y - lastpos.y;
        float addy = offset_y / gameObject.GetComponent<RectTransform>().rect.width * dragSpeed;

        curveShowWindowControl.ySizeMult = Mathf.Clamp(curveShowWindowControl.ySizeMult + addy, 0.001f, 100f);

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
