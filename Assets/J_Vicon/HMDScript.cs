using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using ViconDataStreamSDK.CSharp;

public class HMDScript : MonoBehaviour {
  public String HostName="localhost";
  public String Port="801";
  public String HmdName="";
  public double Offset=0.0;
  public float Height=0;

  private ViconDataStreamSDK.CSharp.RetimingClient m_Client;
  //private GameObject m_CameraObject;
  public enum Bool
  {
    False = 0,
    True
  }

  private static class OVRPluginServices
  {
    public const string OvrPluginName = "OVRPlugin";
    [DllImport(OvrPluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Bool ovrp_SetTrackingPositionEnabled(Bool value);
  }

  bool SetupHMd()
  {
    // Disable positional tracking
    OVRPluginServices.ovrp_SetTrackingPositionEnabled(Bool.False);

    // Cache child camera
    Camera AttachedCamera = GetComponentInChildren<Camera>();
    if (AttachedCamera == null)
    {
      Debug.LogError("Missing Camera component!");
    }
    //else
    //{
    //  m_CameraObject = AttachedCamera.gameObject;
    //}

    return true;
  }

  bool SetupViconClient()
  {
        Debug.Log("sss1");
        m_Client = new ViconDataStreamSDK.CSharp.RetimingClient();
    m_Client.SetAxisMapping(Direction.Forward, Direction.Up, Direction.Right);
    String Host = String.Concat( HostName, ":", Port);
        Debug.Log("sss");
    print("Connecting to " + Host + "...");
    Int32 Attempt = 0;
    while (!m_Client.IsConnected().Connected)
    {
      // Direct connection
      Output_Connect ConnectOutput = m_Client.Connect(Host);
            print("xxx");
      print("Connect result: " + ConnectOutput.Result);

      Attempt += 1;
      if (Attempt == 3)
        break;
      System.Threading.Thread.Sleep(200);
    }

    return true;
  }

  // Use this for initialization
  void Start ()
  {
    SetupHMd();
    SetupViconClient();
  }

  void LateUpdate()
  {
    m_Client.UpdateFrame(Offset);

    Output_GetSubjectRootSegmentName RootName = m_Client.GetSubjectRootSegmentName(HmdName);
    Output_GetSegmentLocalTranslation Translation = m_Client.GetSegmentLocalTranslation(HmdName, RootName.SegmentName);
      Vector3 Pos;
    Pos = new Vector3(-(float)Translation.Translation[0], (float)Translation.Translation[1], (float)Translation.Translation[2] ) * 0.001f;
    Pos.y += Height;
    transform.localPosition = Pos;
  }
}
