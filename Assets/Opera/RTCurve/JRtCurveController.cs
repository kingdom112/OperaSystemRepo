using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeCurveEditor;
public class JRtCurveController : MonoBehaviour
{

    public JRTAnimationCurve rtAnimationCurve;
    public CurveWindow curveWindow;

    public AnimationCurve testCurve1;

    void Start()
    {
        rtAnimationCurve.ListenOnDataAlter(OnDataAlter);
        rtAnimationCurve.ShowCurveEditor();

        rtAnimationCurve.Add(ref testCurve1);
    }

    void Update()
    {
        curveWindow.CallUpdateWindowAndGrid();//更新大小设置 重新绘制
    }


    void OnDataAlter()
    {
        Debug.Log("  OnDataAlter ()"); 
    }

}
