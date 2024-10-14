using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testMouse : MonoBehaviour
{
    public RectTransform t1;


    // Start is called before the first frame update
    void Start()
    {
        
    }
    bool ispressed = false;
    Vector2 pos1;
    Vector2 pos2;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ispressed = true;
            pos1 = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(0))
        {
            ispressed = false;
            pos2 = Input.mousePosition;

        }
        if(ispressed)
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
            if(x1<= x2)
            {
                xleft = x1; 
                xright = Screen.width - x2;
            }
            else
            {
                xleft = x2;
                xright = Screen.width - x1;
            }
            if(y1 <= y2)
            {
                ybottom = y1;
                ytop = Screen.height - y2;
            }
            else
            {
                ybottom = y2;
                ytop = Screen.height - y1;
            }
            t1.offsetMin = new Vector2(xleft, ybottom);
            t1.offsetMax = new Vector2(-xright, -ytop);
        }
        
    }
}
