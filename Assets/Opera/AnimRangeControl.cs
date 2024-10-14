using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 对于动作动画，它的选段（也可以是选点）控制
/// </summary>
public class AnimRangeControl : MonoBehaviour
{

    public enum SelectType
    {
        timePoint,
        timeRange
    }
    [System.Serializable]
    public class SelectOne
    {
        public string name = "";
        public SelectType type = SelectType.timePoint;
        public float timePoint = 0f;
        public float timeRange_start = 0f;
        public float timeRange_end = 0f;
        public bool UIisMaxSize = true;
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
