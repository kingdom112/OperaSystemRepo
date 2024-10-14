using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightStackMember : MonoBehaviour
{
    public RightStackController stackController;
    public float ysize_min = 0f;
    public float ysize_max = 0f;
    public GameObject panel_max;

    public void SetStackController(RightStackController _controller)
    {
        stackController = _controller;
    }

    public virtual void ChangeSize()
    {
        if(panel_max.activeSelf)
        {
            TurnToMinSize();
        }
        else
        {
            TurnToMaxSize();
        }
    }

    /// <summary>
    /// 变成收起的大小
    /// </summary>
    public virtual void TurnToMinSize()
    {
        panel_max.SetActive(false);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ysize_min);
        stackController.ResetStackYSize();
        stackController.ResetStackItemsPos();
    }
    /// <summary>
    /// 变成展开的大小
    /// </summary>
    public virtual void TurnToMaxSize()
    {
        panel_max.SetActive(true);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ysize_max);
        stackController.ResetStackYSize();
        stackController.ResetStackItemsPos();
    }
    /// <summary>
    /// 一般由界面的关闭按钮调用
    /// </summary>
    public virtual void DestroyThis()
    {
        stackController.OutStack(gameObject.GetComponent<RectTransform>());
    }

    /// <summary>
    /// 由DestroyThis调用或者在stackController.OutStack调用。
    /// </summary>
    public virtual void BeforeDestroy()
    {

    }
    public virtual void Show()
    {

    }
    public virtual bool CanCreate
    {
        get
        {
            return true;
        }
    }
    public virtual bool CanDestroy
    {
        get
        {
            return true;
        }
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
