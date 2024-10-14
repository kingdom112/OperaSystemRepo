using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using JNodeBase;
using JVehicleFrameSystem;

namespace JAnimationSystem
{
    public class JVehicleFrameEditW_BoneFrameNode : JVehicleFrameEditWNode
    {
        private JVehicleFrameEditWindow controllerWindow;
        public int numberOfBoneOfBoneFramework = -1;
        private bool changingBone = false;
        private bool isArmor = false;

        /// <summary>
        /// 构造器
        /// </summary>
        public JVehicleFrameEditW_BoneFrameNode(Vector2 position, GUIStyle nodeStyle,
                  GUIStyle selectedStyle, GUIStyle inPointStyle,
                  GUIStyle outPointStyle,
                  Action<ConnectionPoint> OnClickInPoint,
                  Action<ConnectionPoint> OnClickOutPoint,
                  Action<Node> OnClickRemoveNode,bool _isArmor) : base(position, nodeStyle,
                   selectedStyle, inPointStyle,
                   outPointStyle,
                   OnClickInPoint,
                  OnClickOutPoint,
                   OnClickRemoveNode)
        {
            title = NodeTitleName;
            controllerWindow = null;
            numberOfBoneOfBoneFramework = -1;
            changingBone = false;
            isArmor = _isArmor;
            if(isArmor)
            {
                SetUseConnections_Out(false);
            }
        }


       

        private bool _useConnection_in = true;
        public override bool useConnections_In
        {
            get
            {
                return _useConnection_in;
            }
        }
        public void SetUseConnections_In(bool _value)
        {
            _useConnection_in = _value;
        }
        private bool _useConnection_out = true;
        public override bool useConnections_Out
        {
            get
            {
                return _useConnection_out;
            }
        }
        public void SetUseConnections_Out(bool _value)
        {
            _useConnection_out = _value;
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
                    if (isArmor == false)
                    {
                        return controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].boneType.ToString();
                    }
                    else
                    {
                        return controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].armorMainName + "_" +
                             controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].armorPartName;
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
                if(isArmor == false)
                {
                    return 205;
                }
                else
                {
                    return 205;
                }
              
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
                        if (controllerWindow.jVehicleFrame.boneFramework.Bones.Count
                            > numberOfBoneOfBoneFramework && numberOfBoneOfBoneFramework != -1)
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
            rect.width = width;
            rect.height = height;

            if (IfNodeError)
            {
                EditorGUILayout.HelpBox("Error!", MessageType.Error);
                return;
            }
            if(isArmor == false)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("BoneType", GUILayout.Width(60));
                if (changingBone == false)
                {
                    GUI.enabled = false;
                    EditorGUILayout.EnumPopup(controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].boneType);
                    GUI.enabled = true;
                }
                else
                {
                    controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].boneType =
                        (JAnimationData.BoneType)EditorGUILayout.EnumPopup(controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].boneType);
                }
                EditorGUILayout.EndHorizontal();
                if (changingBone == false)
                {
                    GUI.enabled = false; 
                    EditorGUILayout.FloatField("BoneSize", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].boneSize);
                    EditorGUILayout.Vector3Field("LocalPos", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localPos);
                    EditorGUILayout.Vector3Field("LocalScale", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localScale);
                    EditorGUILayout.Vector3Field("LocalRotation", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localRotation);
                    GUI.enabled = true;
                }
                else
                {
                    controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].boneSize
                        = EditorGUILayout.FloatField("BoneSize", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].boneSize);
                    controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localPos
                        = EditorGUILayout.Vector3Field("LocalPos", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localPos);
                    controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localScale
                        = EditorGUILayout.Vector3Field("LocalScale", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localScale);
                    controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localRotation
                       = EditorGUILayout.Vector3Field("LocalRotation", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localRotation);
                }

                if (GUILayout.Button("ChangingBoneType"))
                {
                    changingBone = !changingBone;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ArmorMainName", GUILayout.Width(60));
                if (changingBone == false)
                {
                    GUI.enabled = false;
                    EditorGUILayout.TextField(controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].armorMainName);
                    GUI.enabled = true;
                }
                else
                {
                    controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].armorMainName =
                        EditorGUILayout.TextField(controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].armorMainName);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ArmorPartName", GUILayout.Width(60));
                if (changingBone == false)
                {
                    GUI.enabled = false;
                    EditorGUILayout.TextField(controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].armorPartName);
                    GUI.enabled = true;
                }
                else
                {
                    controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].armorPartName =
                        EditorGUILayout.TextField(controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].armorPartName);
                }
                EditorGUILayout.EndHorizontal();
                if (changingBone == false)
                {
                    GUI.enabled = false;
                    EditorGUILayout.Vector3Field("LocalPos", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localPos);
                    EditorGUILayout.Vector3Field("LocalScale", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localScale);
                    EditorGUILayout.Vector3Field("LocalRotation", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localRotation);
                    GUI.enabled = true;
                }
                else
                {
                    controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localPos
                        = EditorGUILayout.Vector3Field("LocalPos", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localPos);
                    controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localScale
                        = EditorGUILayout.Vector3Field("LocalScale", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localScale);
                    controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localRotation
                      = EditorGUILayout.Vector3Field("LocalRotation", controllerWindow.jVehicleFrame.boneFramework.Bones[numberOfBoneOfBoneFramework].localRotation);
                }

                if (GUILayout.Button("Changing"))
                {
                    changingBone = !changingBone;
                }
            }
            
        }

        public void SetControllerWindow(JVehicleFrameEditWindow _jVehicleFrameEditWindow)
        {
            controllerWindow = _jVehicleFrameEditWindow;
        }

        public void SetNumberOfBoneOfBoneFrame(int _number)
        {
            numberOfBoneOfBoneFramework = _number;
        }

      

        protected override void OnMyRemoveNode_AfterRemove()
        {
            if (controllerWindow.jVehicleFrame != null)
            {
                if(numberOfBoneOfBoneFramework != 0)
                {
                    for (int i = 0; i < controllerWindow.jVehicleFrame.boneFramework.Bones.Count; i++)
                    {
                        BoneFramework.Bone bone1 = controllerWindow.jVehicleFrame.boneFramework.Bones[i];
                        for (int j = bone1.ChildsList.Count - 1; j >= 0; j--)
                        {
                            BoneFramework.Bone.BoneChild child1 = bone1.ChildsList[j];
                            if (child1.numberInList == numberOfBoneOfBoneFramework)
                            {
                                bone1.ChildsList.RemoveAt(j);
                            }
                            if (child1.numberInList > numberOfBoneOfBoneFramework)
                            {
                                child1.numberInList -= 1;
                            }
                        }
                    }
                    controllerWindow.jVehicleFrame.boneFramework.Bones.RemoveAt(numberOfBoneOfBoneFramework);
                    controllerWindow.jVehicleFrame.boneFrameworkMousePosList.RemoveAt(numberOfBoneOfBoneFramework);
                }
            }
            controllerWindow.RefreshNodes();
        }

        protected override void AfterDrag()
        {
            if (IfNodeError) return;
            controllerWindow.jVehicleFrame.boneFrameworkMousePosList[numberOfBoneOfBoneFramework] = new Vector2(rect.x,rect.y);
        }
    }
}
   
