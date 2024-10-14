using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimInfoWindow_rangesMember : MonoBehaviour
{
   
    public AnimInfoWindowControl windowController;
    public Text nameText;
    public GameObject pointTeam;
    public GameObject rangeTeam_left;
    public GameObject rangeTeam_right;
    public Text typeSelectText;
    public InputField point_inputfield;
    public Slider point_slider;
    public InputField range_left_inputfield;
    public Slider range_left_slider;
    public InputField range_right_inputfield;
    public Slider range_right_slider;

    public float minSizeY, maxSizeY;
    public GameObject maxSizePanel;

    void Start()
    {
        
    }


    void Update()
    {
        
    }


    public void Button_ChangeSize()
    {
        if(maxSizePanel.activeSelf)
        {
            ChangeToMinSize();
            windowController.RangeMemberTryChangeMaxSizeBool(this, false);
        }
        else
        {
            ChangeToMaxSize();
            windowController.RangeMemberTryChangeMaxSizeBool(this, true);
        }
        windowController.ResetRangesMembersPos();
    }

    public void ChangeToMinSize()
    {
        maxSizePanel.SetActive(false);
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, minSizeY);
    }
    public void ChangeToMaxSize()
    {
        maxSizePanel.SetActive(true);
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxSizeY);
    }

    public void Button_ChangeType()
    {
        if (typeSelectText.text == "关键点")
        {
            windowController.RangeMemberTryChangeType(this, AnimRangeControl.SelectType.timeRange);
        }
        else if (typeSelectText.text == "关键段")
        {
            windowController.RangeMemberTryChangeType(this, AnimRangeControl.SelectType.timePoint);
        }
        else
        {
            Debug.LogError("Error!");
        }
    }
  
    

    public void Button_Destroy()
    {
        windowController.RangeMemberWantToDestroy(this);
    }

    public void OnPointSliderChange(float _value)
    {
        if (windowController != null)
        {
            if (windowController.canEdit == false) return;
            int num = -1;
            for (int i = 0; i < windowController.createdButtons.Count; i++)
            {
                if (windowController.createdRangesMembers[i] == this)
                {
                    num = i;
                    break;
                }
            }
            if (num != -1)
            {
                windowController.buttons[num].time1 = _value * windowController._TimeTotalLength;
            }
        }
    }
    public void OnRangeSliderLeftChange(float _value)
    {
        if (windowController != null)
        {
            if (windowController.canEdit == false) return;
            int num = -1;
            for (int i = 0; i < windowController.createdButtons.Count; i++)
            {
                if (windowController.createdRangesMembers[i] == this)
                {
                    num = i;
                    break;
                }
            }
            if (num != -1)
            {
                windowController.buttons[num].time1 = _value * windowController._TimeTotalLength;
            }
        }
    }
    public void OnRangeSliderRightChange(float _value)
    {
        if (windowController != null)
        {
            if (windowController.canEdit == false) return;
            int num = -1;
            for (int i = 0; i < windowController.createdButtons.Count; i++)
            {
                if (windowController.createdRangesMembers[i] == this)
                {
                    num = i;
                    break;
                }
            }
            if (num != -1)
            {
                windowController.buttons[num].time2 = _value * windowController._TimeTotalLength;
            }
        }
    }

    public void SetInfo(AnimRangeControl.SelectOne info, AnimInfoWindowControl _windowController, int _count)
    { 
        windowController = _windowController;
        if (info.type == AnimRangeControl.SelectType.timePoint)
        {
            typeSelectText.text = "关键点"; 
            pointTeam.SetActive(true);
            rangeTeam_left.SetActive(false);
            rangeTeam_right.SetActive(false);
           // point_inputfield.text = info.timePoint.ToString();
            //point_slider.value = info.timePoint / timeLength;
        }
        else if(info.type == AnimRangeControl.SelectType.timeRange)
        {
            typeSelectText.text = "关键段"; 
            pointTeam.SetActive(false);
            rangeTeam_left.SetActive(true);
            rangeTeam_right.SetActive(true);
            //range_left_inputfield.text = info.timeRange_start.ToString();
            //range_right_inputfield.text = info.timeRange_end.ToString();
            //range_left_slider.value = info.timeRange_start / timeLength;
            //range_right_slider.value = info.timeRange_end / timeLength;
        }
        nameText.text = _count.ToString() + info.name + "   " + (typeSelectText.text);
        if(info.UIisMaxSize)
        {
            ChangeToMaxSize();
        }
        else
        {
            ChangeToMinSize();
        }
    }

}
