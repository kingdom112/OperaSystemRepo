using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JAnimationSystem;
using Battlehub.RTHandles;
using Battlehub.RTSL.Interface;
using Battlehub.RTSL;
using Battlehub.RTCommon;
using Battlehub.UIControls;
using TMPro;
using Battlehub.RTEditor;
using System.Reflection;
using Battlehub.Utils;

public class RuntimeAnimationListener : MonoBehaviour
{

    public bonesCreater control;

    private RuntimeAnimation ra = null;

    // Start is called before the first frame update
    void Start()
    {
        ra = gameObject.GetComponent<RuntimeAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnRuntimeAnimationNormalizedTimeChanged()
    {
        control.OnRuntimeAnimationNormalizedTimeChanged(ra);
    }

}
