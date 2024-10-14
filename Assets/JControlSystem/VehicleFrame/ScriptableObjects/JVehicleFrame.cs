using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JVehicleFrameSystem;

/// <summary>
/// J动画系统的控制器
/// </summary>
[CreateAssetMenu]
public class JVehicleFrame : ScriptableObject
{

    [System.Serializable]
    public class State
    {
        public string stateName = "";
        public string ResourceName = "";
        public Vector2 mousePos;

        public State(Vector2 _mousePos, string _stateName = "NoName",string _ResourceName = "NoResourceName")
        {
            mousePos = _mousePos;
            stateName = _stateName;
            ResourceName = _ResourceName;
        }
        public void SetStateName(string _stateName)
        {
            stateName = _stateName;
        }
        public void SetResourceName(string _ResourceName = "NoResourceName")
        {
            ResourceName = _ResourceName;
        }
    }

    [System.Serializable]
    public class boneMarkerName
    {
        public string Name = "";
        public Vector2 mousePos;

        public boneMarkerName(Vector2 _mousePos, string _Name = "_NO_BONEMARKER_NAME")
        {
            mousePos = _mousePos;
            Name = _Name;
        }
        public void SetName(string _Name)
        {
            Name = _Name;
        }
    }

    public List<State> states = new List<State>();
    /// <summary>
    /// the Names of the gameObject which has a BoneMarker.
    /// </summary>
    public List<boneMarkerName> boneMarkerNames = new List<boneMarkerName>();

    //[HideInInspector]
    public BoneFramework boneFramework = new BoneFramework();
    //[HideInInspector]
    public List<Vector2> boneFrameworkMousePosList = new List<Vector2>();
}
