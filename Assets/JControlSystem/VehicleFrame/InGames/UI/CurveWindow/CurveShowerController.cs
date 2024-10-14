using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CurveShowerController : MonoBehaviour
{

    public CurveShower curveShower;
    public LineShower lineShower;
     
    public LineShower_X lineShower_X;

    public LineShower_X lineShower_Time_master;
    public LineShower_X lineShower_Time_student;

    public RectTransform panel;

    private RectTransform rectT;

    public List<CurveShower> AddedCurves
    {
        get
        {
            return addedCurves;
        }
    }
    private List<CurveShower> addedCurves = new List<CurveShower>();

    private void Awake()
    {
        rectT = GetComponent<RectTransform>();
        addedCurves = new List<CurveShower>();
    }

    void Start()
    {
       
    }

    /// <summary>
    /// 重新调整幅曲线的数量
    /// </summary>
    public void ReSizeAddedCurves(int count)
    {
        if(addedCurves.Count < count)
        {
            for(int i=0; i< count - addedCurves.Count; i++)
            {
                GameObject ga1 = Instantiate<GameObject>(curveShower.gameObject);
                ga1.transform.SetParent(curveShower.transform.parent);
                addedCurves.Add(ga1.GetComponent<CurveShower>());
                ga1.GetComponent<RectTransform>().localScale = Vector3.one;
                Vector3 _locp = ga1.GetComponent<RectTransform>().localPosition;
                _locp.z = 0f;
                ga1.GetComponent<RectTransform>().localPosition = _locp;
                ga1.GetComponent<RectTransform>().anchoredPosition = curveShower.rectTransform.anchoredPosition;
                ga1.GetComponent<RectTransform>().offsetMax = curveShower.rectTransform.offsetMax;
                ga1.GetComponent<RectTransform>().offsetMin = curveShower.rectTransform.offsetMin;


            }
        }
        if (addedCurves.Count > count)
        {
            for (int i = addedCurves.Count - 1; i >= count; i--)
            {
                addedCurves[i].gameObject.transform.SetParent(null);
                Destroy(addedCurves[i].gameObject);
                addedCurves.RemoveAt(i);
            }
        }
    }

    void Update()
    {
        lineShower.Ychu = curveShower.y_division;

        if (lineShower.Lines.Count > 0)
        {
            string text1 = lineShower.Lines[0].ToString(); 
            for (int i = 1; i < lineShower.Lines.Count; i++)
            {
                text1 = text1+"\n" + lineShower.Lines[i].ToString("F2");
            }
            lineShower.textControl.text = text1;
        }
        else
        {
            lineShower.textControl.text = "";
        }

        if (lineShower_X.Lines.Count > 0)
        {
            string text1 = lineShower_X.Lines[0].ToString();
            for (int i = 1; i < lineShower_X.Lines.Count; i++)
            {
                text1 = text1 + "\n" + lineShower_X.Lines[i].ToString("F2");
            }
            lineShower_X.textControl.text = text1;
        }
        else
        {
            lineShower_X.textControl.text = "";
        }

        if (lineShower_Time_master.Lines.Count > 0)
        {
            string text1 = lineShower_Time_master.Lines[0].ToString();
            for (int i = 1; i < lineShower_Time_master.Lines.Count; i++)
            {
                text1 = text1 + "\n" + lineShower_Time_master.Lines[i].ToString("F2");
            }
            lineShower_Time_master.textControl.text = text1;
        }
        else
        {
            lineShower_Time_master.textControl.text = "";
        }
        if (lineShower_Time_student.Lines.Count > 0)
        {
            string text1 = lineShower_Time_student.Lines[0].ToString();
            for (int i = 1; i < lineShower_Time_student.Lines.Count; i++)
            {
                text1 = text1 + "\n" + lineShower_Time_student.Lines[i].ToString("F2");
            }
            lineShower_Time_student.textControl.text = text1;
        }
        else
        {
            lineShower_Time_student.textControl.text = "";
        }

        //将幅曲线和主曲线的设置同步
        for (int i=0; i<addedCurves.Count; i++)
        {
            addedCurves[i].time_min = curveShower.time_min;
            addedCurves[i].time_max = curveShower.time_max;
            addedCurves[i].y_min = curveShower.y_min;
            addedCurves[i].y_max = curveShower.y_max;
            //addedCurves[i].y_division = curveShower.y_division;  
            addedCurves[i].enabled = false;
            addedCurves[i].enabled = true;
        }
    }


  


    public void SetCurve(AnimationCurve _curve, Color color1)
    {
        curveShower.m_Curve = _curve;
        curveShower.color = color1;
        Refresh();
    }

  

    public void Refresh()
    {
        curveShower.enabled = false;
        curveShower.enabled = true;
        lineShower.enabled = false;
        lineShower.enabled = true;
        lineShower_X.enabled = false;
        lineShower_X.enabled = true;
        lineShower_Time_master.enabled = false;
        lineShower_Time_master.enabled = true;
        lineShower_Time_student.enabled = false;
        lineShower_Time_student.enabled = true;
        lineShower.textControl.enabled = false;
        lineShower.textControl.enabled = true;
        lineShower_X.textControl.enabled = false;
        lineShower_X.textControl.enabled = true;
        lineShower_Time_master.textControl.enabled = false;
        lineShower_Time_master.textControl.enabled = true;
        lineShower_Time_student.textControl.enabled = false;
        lineShower_Time_student.textControl.enabled = true;
    }

}
