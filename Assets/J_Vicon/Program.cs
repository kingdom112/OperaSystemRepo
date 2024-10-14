using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ViconDataStreamSDK.CSharp;


namespace UnityVicon
{
  public class Program : MonoBehaviour
  {
    public string HostName = "localhost";
    public string Port = "801";
    public string SubjectName = "";

    ViconDataStreamSDK.CSharp.Client Client = new Client();

    public Program()
    {
    }

    void Start()
    {
      print("Starting...");
      Output_GetVersion OGV = Client.GetVersion();
      print("GetVersion Major: " + OGV.Major);
      ConnectClient();
    }

    private void ConnectClient()
    {
      if (Client.IsConnected().Connected)
      {
        Client.Disconnect();
      }

      String Host = HostName + ":" + Port;
      int noAttempts = 0;
      print("Connecting to " + Host + "...");
      while (!Client.IsConnected().Connected)
      {
        Output_Connect OC = Client.Connect(Host);
        print("Connect result: " + OC.Result);

        noAttempts += 1;
        if (noAttempts == 3)
          break;
        System.Threading.Thread.Sleep(200);
      }

      if (!Client.IsConnected().Connected)
      {
        throw new Exception("Could not connect to server.");
      }

      Client.EnableSegmentData();
      // get a frame from the data stream so we can inspect the list of subjects
      Client.GetFrame();
    }

    void LateUpdate()

    {
      Client.GetFrame();
            
      Output_GetSubjectRootSegmentName OGSRSN = Client.GetSubjectRootSegmentName(SubjectName);
          
            //Transform Root = transform.Find(OGSRSN.SegmentName);
            //ApplyBoneTransform(Root);
            Find(transform, OGSRSN.SegmentName);

    }
    void Find(Transform iTransform, string BoneName)
    {
            if(BoneName != "")
            {
                //Debug.Log(BoneName);
            }
      int ChildCount = iTransform.childCount;
      for (int i = 0; i < ChildCount; ++i)
      {
        Transform Child = iTransform.GetChild(i);
        if( Child.name == BoneName )
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
      Output_GetSegmentLocalRotationQuaternion ORot = Client.GetSegmentLocalRotationQuaternion(SubjectName, Bone.gameObject.name);
      if (ORot.Result == Result.Success)
      {
        Bone.localRotation = new Quaternion(-(float)ORot.Rotation[0], (float)ORot.Rotation[1], (float)ORot.Rotation[2], -(float)ORot.Rotation[3]);
      }

      Output_GetSegmentLocalTranslation OTran = Client.GetSegmentLocalTranslation(SubjectName, Bone.gameObject.name);
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

