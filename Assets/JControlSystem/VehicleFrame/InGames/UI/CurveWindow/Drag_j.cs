using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag_j : MonoBehaviour, IDragHandler, IPointerDownHandler
{

    /// <summary>
    /// 当此值为false，拖动滑条或者TryChangeValue()都不可改变value
    /// </summary>
    public bool usingThis = true;
    public RectTransform dragRange_Canva;
    public RectTransform rect1;

    private Vector2 lastpos;  //临时记录点击点与UI的相对位置

    private float max_x
    {
        get
        {
            return dragRange_Canva.rect.width / 2f - rect1.rect.width / 2f;
        }
    }
    private float min_x
    {
        get
        {
            return -dragRange_Canva.rect.width / 2f + rect1.rect.width / 2f;
        }
    }
    private float max_y
    {
        get
        {
            return dragRange_Canva.rect.height / 2f - rect1.rect.height / 2f;
        }
    }
    private float min_y
    {
        get
        {
            return -dragRange_Canva.rect.height / 2f + rect1.rect.height / 2f;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (usingThis == false) return;
        if (dragRange_Canva == null) return;
        float offset_x = eventData.position.x - lastpos.x;
        float offset_y = eventData.position.y - lastpos.y;
        float np_x = rect1.anchoredPosition.x + offset_x;
        float np_y = rect1.anchoredPosition.y + offset_y;
        if (np_x > max_x) np_x = max_x;
        if (np_x < min_x) np_x = min_x;
        if (np_y > max_y) np_y = max_y;
        if (np_y < min_y) np_y = min_y;
        rect1.anchoredPosition = new Vector2(np_x, np_y);
        lastpos = eventData.position;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (dragRange_Canva == null) return;
        lastpos = eventData.position;
    }

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        if (dragRange_Canva == null) return;
        rect1.anchorMin = new Vector2(0.5f, 0.5f);
        rect1.anchorMax = new Vector2(0.5f, 0.5f);
    }
}
