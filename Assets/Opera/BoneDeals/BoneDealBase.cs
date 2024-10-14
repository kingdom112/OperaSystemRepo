using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JAnimationSystem;
using Dreamteck.Splines;
using JAnimCurves;

public class BoneDealBase 
{
    public BoneInfoWindowControl boneWindow = null;
    protected VFPlayer_InGame _vfPlayer;
    protected VFPlayer_InGame vfPlayer
    {
        get
        {
            if(_vfPlayer == null)
            {
                _vfPlayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
            }
            return _vfPlayer;
        }
    }
    public virtual bool needSelectBone
    {
        get
        {
            return true;
        }
    }
    protected Color mainCurveColor
    {
        get
        {
            return boneWindow.mainCurveColor;
        }
    }
    protected Color addedCurvesColor
    {
        get
        {
            return boneWindow.addedCurvesColor;
        }
    }

    public virtual int NeedCalculateCount()
    {
        return 0;
    }

    public virtual IEnumerator DealCalculate()
    {
        yield return new WaitForSecondsRealtime(0.1f);
    }
    protected AnimationCurve tempCurve = null;
    protected int yieldWaitTick = 10;// 

    protected float GetCurveMin(AnimationCurve _curve)
    {
        if (_curve.keys.Length == 0)
        {
            return 0f;
        }
        float min1 = _curve.keys[0].value;
        for (int i = 1; i < _curve.keys.Length; i++)
        {
            if (_curve.keys[i].value < min1)
            {
                min1 = _curve.keys[i].value;
            }
        }
        return min1;
    }
    protected float GetCurveMax(AnimationCurve _curve)
    {
        if (_curve.keys.Length == 0)
        {
            return 0f;
        }
        float max1 = _curve.keys[0].value;
        for (int i = 1; i < _curve.keys.Length; i++)
        {
            if (_curve.keys[i].value > max1)
            {
                max1 = _curve.keys[i].value;
            }
        }
        return max1;
    }
}
