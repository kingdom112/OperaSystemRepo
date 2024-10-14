using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using JNodeBase;

namespace JAnimationSystem
{
    public class JVehicleFrameEditW_BoneMarkerNode : JVehicleFrameEditWNode
    {
        private JVehicleFrameEditWindow controllerWindow;
        private int numberOfBoneMarker = -1;
        private BoneMarker boneMarker;
        private bool ChangingBoneMarker = false;


        /// <summary>
        /// 构造器
        /// </summary>
        public JVehicleFrameEditW_BoneMarkerNode(Vector2 position, GUIStyle nodeStyle,
                  GUIStyle selectedStyle, GUIStyle inPointStyle,
                  GUIStyle outPointStyle,
                  Action<ConnectionPoint> OnClickInPoint,
                  Action<ConnectionPoint> OnClickOutPoint,
                  Action<Node> OnClickRemoveNode) : base(position, nodeStyle,
                   selectedStyle, inPointStyle,
                   outPointStyle,
                   OnClickInPoint,
                  OnClickOutPoint,
                   OnClickRemoveNode)
        {
            title = NodeTitleName;
            controllerWindow = null;
            numberOfBoneMarker = -1;
            boneMarker = null;
            ChangingBoneMarker = false;
        }

        public override bool useConnections_In
        {
            get
            {
                return false;
            }
        }
        public override bool useConnections_Out
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Get this node's name.
        /// </summary>
        protected override string NodeTitleName
        {
            get
            {
                if (IfNodeError == false)
                {
                    if (boneMarker != null)
                    {
                        return "State(" + boneMarker.gameObject.name + ")";
                    }
                    else
                    {
                        return "State(Blank)";
                    }
                }
                else
                {
                    return "State(Error)";
                }

            }
        }
        public override float width
        {
            get
            {
                return 220;
            }
        }
        public override float height
        {
            get
            {
                return 110;
            }
        }

        private bool IfNodeError
        {
            get
            {
                if (controllerWindow != null)
                {
                    if (controllerWindow.jVehicleFrame != null)
                    {
                        if (controllerWindow.jVehicleFrame.boneMarkerNames.Count 
                            > numberOfBoneMarker && numberOfBoneMarker != -1)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public override void DrawInNodeWindow(int id)
        {
            title = NodeTitleName;

            if (IfNodeError)
            {
                EditorGUILayout.HelpBox("Error!", MessageType.Error);
                return;
            }

            if (ChangingBoneMarker == false)
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("BoneMarker", boneMarker, typeof(BoneMarker), true);
                GUI.enabled = true;
            }
            else
            {
                BoneMarker _boneMarker =
                    EditorGUILayout.ObjectField("BoneMarker", boneMarker, typeof(BoneMarker), true) as BoneMarker;
                if(_boneMarker != boneMarker)
                {
                    if(_boneMarker != null)
                    {
                        boneMarker = _boneMarker;
                        controllerWindow.jVehicleFrame.boneMarkerNames[numberOfBoneMarker].Name 
                            = boneMarker.gameObject.name;
                    }
                }
            }
            if (GUILayout.Button("ChangingBoneMarker"))
            {
                ChangingBoneMarker = !ChangingBoneMarker;
            }


            if (boneMarker != null)
            {
                EditorGUILayout.HelpBox("Count of bones :  [" + boneMarker.boneMatch.boneMatchList.Count.ToString()
                    + "]   ", MessageType.Info, true);
            }
            else
            {
                EditorGUILayout.HelpBox("Please select a BoneMarker.", MessageType.Info, true);
            }
        }

        public void SetControllerWindow(JVehicleFrameEditWindow _jVehicleFrameEditWindow)
        {
            controllerWindow = _jVehicleFrameEditWindow;
        }

        public void SetNumberOfBoneMarker(int _number)
        {
            numberOfBoneMarker = _number;
        }

        public bool FindBoneMarkerByName(string _BMname,Transform _parent = null)
        {
            BoneMarker[] bms = GameObject.FindObjectsOfType<BoneMarker>();
            if (_parent != null)
            {
                bms = _parent.GetComponentsInChildren<BoneMarker>();
            }
            for(int i=0; i<bms.Length; i++)
            {
                if(bms[i].gameObject.name == _BMname)
                {
                    boneMarker = bms[i];
                    Debug.Log("found :  " + bms[i].gameObject.name);
                    return true;
                }
            }
            boneMarker = null;
            Debug.Log("not found :  " + _BMname);
            return false;
        }

        protected override void OnMyRemoveNode_AfterRemove()
        {
            if (controllerWindow.jVehicleFrame != null)
            {
                controllerWindow.jVehicleFrame.boneMarkerNames.RemoveAt(numberOfBoneMarker);
            }
            controllerWindow.RefreshNodes();
        }

        protected override void AfterDrag()
        {
            if (IfNodeError) return;
            controllerWindow.jVehicleFrame.boneMarkerNames[numberOfBoneMarker].mousePos.x = rect.x;
            controllerWindow.jVehicleFrame.boneMarkerNames[numberOfBoneMarker].mousePos.y = rect.y;
        }
    }

}