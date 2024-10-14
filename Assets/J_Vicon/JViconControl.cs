using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ViconDataStreamSDK.CSharp;

public class JViconControl : MonoBehaviour
{
    public string HostName = "localhost";
    public string Port = "801";
    public string SubjectName = "";

    ViconDataStreamSDK.CSharp.Client Client = new Client();

    public Action LinkErrorAction;
    public Action LinkSuccessAction;

    void Start()
    {
        LinkErrorAction = delegate
        {

        };
        LinkSuccessAction = delegate
        {

        };
    }
    

    public void StartLink(string _ip, string _port, Action _linkSuccessAction, Action _linkErrorAction)
    {
        HostName = _ip;
        Port = _port;
        LinkSuccessAction = _linkSuccessAction;
        LinkErrorAction = _linkErrorAction;
        print("Starting Link To Vicon...");
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
            //throw new Exception("Could not connect to server.");
            Debug.Log("连接Vicon失败");
            LinkErrorAction();
            return;
        }

        Client.EnableSegmentData();
        // get a frame from the data stream so we can inspect the list of subjects
        Client.GetFrame();
        LinkSuccessAction();
        Debug.Log("连接Vicon成功");
    }

    void LateUpdate()
    {
        if (!Client.IsConnected().Connected)
        {
            return;
        }

        Client.GetFrame();

        Output_GetSubjectRootSegmentName OGSRSN = Client.GetSubjectRootSegmentName(SubjectName);

        //Transform Root = transform.Find(OGSRSN.SegmentName);
        //ApplyBoneTransform(Root);
        Find(transform, OGSRSN.SegmentName);

    }
    void Find(Transform iTransform, string BoneName)
    {
        if (BoneName != "")
        {
            //Debug.Log(BoneName);
        }
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
}
