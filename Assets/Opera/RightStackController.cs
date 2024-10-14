using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightStackController : MonoBehaviour
{

    public RectTransform ContentParent;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OutStack(RectTransform _target, bool useBeforeDestroy = true)
    {
        Debug.Log("OutStack  "+_target.gameObject.name);
        if (_target.GetComponent<RightStackMember>().CanDestroy == false)
        {
            return;
        }

        float h1 = _target.rect.height;
        int index1 = 0;
        for(int i= 0; i<ContentParent.childCount; i++)
        {
            if(ContentParent.GetChild(i).gameObject == _target.gameObject)
            {
                index1 = i;
                break;
            }
        }
        if(useBeforeDestroy)
            _target.GetComponent<RightStackMember>().BeforeDestroy();
        _target.transform.SetParent(null);
        Destroy(_target.gameObject);//destroy
        //Debug.Log("destroy  ContentParent.childCount: " + ContentParent.childCount);
        for (int i = index1; i < ContentParent.childCount; i++)
        {
            /*ContentParent.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(
                ContentParent.GetChild(i).GetComponent<RectTransform>().anchoredPosition.x,
                ContentParent.GetChild(i).GetComponent<RectTransform>().anchoredPosition.y + h1);*/
        }
        ResetStackYSize();
        ResetStackItemsPos();
    }

    public void GetInStack(RectTransform _target)
    {/*
        float newx = _target.rect.width / 2f;
        float newy;
        if(ContentParent.childCount == 0)
        {
            newy = -_target.rect.height / 2f - 10f;//10是空出来的位置
           
        }
        else
        {
            newy =
                ContentParent.GetChild(ContentParent.childCount - 1).GetComponent<RectTransform>().anchoredPosition.y - ContentParent.GetChild(ContentParent.childCount - 1).GetComponent<RectTransform>().rect.height / 2f -
                _target.rect.height / 2f;
           
        }*/
        _target.SetParent(ContentParent);
        //_target.anchoredPosition = new Vector2(0f, newy);
        _target.GetComponent<RightStackMember>().SetStackController(this);
        ResetStackYSize();
        ResetStackItemsPos();
        ContentParent.anchoredPosition = new Vector2(ContentParent.anchoredPosition.x, ContentParent.rect.height / 2f);
        //Debug.Log("xxxxxxxx  " + );

    }

    public void ResetStackItemsPos()
    {
        for(int i=0; i< ContentParent.childCount; i++)
        {
            RectTransform child1 = ContentParent.GetChild(i).GetComponent<RectTransform>();
            float newx = child1.rect.width / 2f;
            float newy;
            if (i == 0)
            {
                newy = -child1.rect.height / 2f - 10f;//10是空出来的位置

            }
            else
            {
                newy =
                    ContentParent.GetChild(i-1).GetComponent<RectTransform>().anchoredPosition.y - ContentParent.GetChild(i - 1).GetComponent<RectTransform>().rect.height / 2f -
                    child1.rect.height / 2f;
            }
            child1.anchoredPosition = new Vector2(0f, newy);
            child1.offsetMin = new Vector2(0f, child1.offsetMin.y);
            child1.offsetMax = new Vector2(0f, child1.offsetMax.y);
            child1.localScale = Vector3.one;
            Vector3 _lpos = child1.localPosition;
            _lpos.z = 0f;
            child1.localPosition = _lpos;
        }
    }

    public void ResetStackYSize()
    {
        float h1 = 0f;
        float w1 = 0f;
        Debug.Log("ContentParent.childCount: " + ContentParent.childCount);
        for (int i = 0; i < ContentParent.childCount; i++)
        {
            //Debug.Log(ContentParent.childCount);
            h1 += ContentParent.GetChild(i).GetComponent<RectTransform>().rect.height;
            if(ContentParent.GetChild(i).GetComponent<RectTransform>().rect.width > w1)
            {
                w1 = ContentParent.GetChild(i).GetComponent<RectTransform>().rect.width;
            }
        }
        h1 += 500f;
        //ContentParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w1);
        ContentParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h1);
        Debug.Log("ResetStackYSize: X: " + w1 + "  Y: " + h1);
        
    }

}
