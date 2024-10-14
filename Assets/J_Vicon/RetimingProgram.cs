using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ViconDataStreamSDK.CSharp;


namespace UnityVicon
{
  public class RetimingProgram : MonoBehaviour
  {
    public string HostName = "localhost";
    public string Port = "801";
    public double Offset = 0.0;
    public string SubjectName = "";

    ViconDataStreamSDK.CSharp.RetimingClient RetimingClient = new RetimingClient();

    public RetimingProgram()
    {
    }

    void Start()
    {
      print("Starting...");
      Output_GetVersion OGV = RetimingClient.GetVersion();
      print("GetVersion Major: " + OGV.Major);
      ConnectRetimingClient();
    }

    private void ConnectRetimingClient()
    {
      if (RetimingClient.IsConnected().Connected)
      {
        RetimingClient.Disconnect();
      }

      int noAttempts = 0;
      string Host=HostName + ":" + Port;
      print("Connecting to " + Host + "...");
      while (!RetimingClient.IsConnected().Connected)
      {
        Output_Connect OC = RetimingClient.Connect(Host);
        print("Connect result: " + OC.Result);

        noAttempts += 1;
        if (noAttempts == 3)
          break;
        System.Threading.Thread.Sleep(200);
      }

      RetimingClient.UpdateFrame();
    }

    void LateUpdate()
    {
      RetimingClient.UpdateFrame( Offset );

      Output_GetSubjectRootSegmentName OGSRSN = RetimingClient.GetSubjectRootSegmentName(SubjectName);
      Find(transform, OGSRSN.SegmentName);
     }

  void Find(Transform iTransform, string BoneName)
  {
    int ChildCount = iTransform.childCount;
    for (int i = 0; i < ChildCount; ++i)
    {
      Transform Child = iTransform.GetChild(i);
      if (Child.name == BoneName)
      {
        ApplyBoneTransform(Child);
        break;
      }
      Find(Child, BoneName);
    }
  }
  private void ApplyBoneTransform(Transform Bone)
    {
      // update the bone transform from the data stream
      Output_GetSegmentLocalRotationQuaternion ORot = RetimingClient.GetSegmentLocalRotationQuaternion(SubjectName, Bone.gameObject.name);
      if (ORot.Result == Result.Success)
      {
        Bone.localRotation = new Quaternion(-(float)ORot.Rotation[0], (float)ORot.Rotation[1], (float)ORot.Rotation[2], -(float)ORot.Rotation[3]);
      }

      Output_GetSegmentLocalTranslation OTran = RetimingClient.GetSegmentLocalTranslation(SubjectName, Bone.gameObject.name);
      if (OTran.Result == Result.Success)
      {
        Bone.localPosition = new Vector3(-(float)OTran.Translation[0] * 0.001f, (float)OTran.Translation[1] * 0.001f, (float)OTran.Translation[2] * 0.001f);
      }

      // recurse through children
      for (int iChild = 0; iChild < Bone.childCount; iChild++)
      {
        ApplyBoneTransform(Bone.GetChild(iChild));
      }
    }
  } //end of program
}// end of namespace

