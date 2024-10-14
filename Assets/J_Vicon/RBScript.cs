using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ViconDataStreamSDK.CSharp;


namespace UnityVicon
{
  public class RBScript : MonoBehaviour
  {
    public string HostName = "localhost";
    public string Port = "801";
    public string TrackedRigidBody = "";
    public string FBXRoot = "";

    ViconDataStreamSDK.CSharp.Client Client = new Client();

    public RBScript()
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

      Client.SetAxisMapping(Direction.Forward, Direction.Up, Direction.Right);
      Client.EnableSegmentData();
      // get a frame from the data stream so we can inspect the list of subjects
      Client.GetFrame();
    }

    void LateUpdate()

    {
      Client.GetFrame();

      Output_GetSubjectRootSegmentName OGSRSN = Client.GetSubjectRootSegmentName(TrackedRigidBody);
      if (OGSRSN.SegmentName != TrackedRigidBody)
      {
        throw new Exception("No rigid body " + TrackedRigidBody + " in the stream");
      }
      
      Transform Root = transform.Find(FBXRoot);
      if (Root == null)
      {
        throw new Exception( FBXRoot + " is not fbx root name");
      }
      

      Output_GetSegmentLocalRotationQuaternion ORot = Client.GetSegmentLocalRotationQuaternion(TrackedRigidBody, TrackedRigidBody);
      if (ORot.Result == Result.Success)
      {
        Root.localRotation = new Quaternion(-(float)ORot.Rotation[0], (float)ORot.Rotation[1], (float)ORot.Rotation[2], -(float)ORot.Rotation[3]);
      }
      Output_GetSegmentLocalTranslation OTran = Client.GetSegmentLocalTranslation(TrackedRigidBody, TrackedRigidBody);
      if (OTran.Result == Result.Success)
      {
        Root.localPosition = new Vector3(-(float)OTran.Translation[0] * 0.001f, (float)OTran.Translation[1] * 0.001f, (float)OTran.Translation[2] * 0.001f);
      }

    }
  } //end of program
}// end of namespace

