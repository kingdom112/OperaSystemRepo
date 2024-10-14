using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using JNodeBase;

namespace JAnimationSystem
{
    public class JVehicleFrameEditW_StateNode : JVehicleFrameEditWNode
    {
        private GUIStyle richTextStyle_Left;

        private JVehicleFrameEditWindow controllerWindow;
        private int stateNumber = -1;
        private bool changingStateName = false;


        /// <summary>
        /// 构造器
        /// </summary>
        public JVehicleFrameEditW_StateNode(Vector2 position, GUIStyle nodeStyle,
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
            stateNumber = -1;
            changingStateName = false;
            GenerateStyles();
        }
        void GenerateStyles()
        {
            richTextStyle_Left = new GUIStyle();
            richTextStyle_Left.richText = true;
        }
        public void SetControllerWindow(JVehicleFrameEditWindow _controllerWindow)
        {
            controllerWindow = _controllerWindow;
        }
        public void SetStateNumber(int _stateNumber)
        {
            stateNumber = _stateNumber;
        }

        private bool IfNodeError
        {
            get
            {
                if (controllerWindow != null)
                {
                    if (controllerWindow.jVehicleFrame != null)
                    {
                        if (controllerWindow.jVehicleFrame.states.Count > stateNumber 
                            && stateNumber != -1)
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

        /// <summary>
        /// Get this node's name.
        /// </summary>
        protected override string NodeTitleName
        {
            get
            {
                if(IfNodeError)
                {
                    return "State(Error)";
                }
                else
                {
                    return "State("
                              + controllerWindow.jVehicleFrame.states[stateNumber].stateName + ")";
                }
            }
        }
        public override float width
        {
            get
            {
                return 160;
            }
        }
        public override float height
        {
            get
            {
                return 150;
            }
        }

        public override void DrawInNodeWindow(int id)
        {
            title = NodeTitleName;
            
            if(IfNodeError)
            {
                EditorGUILayout.HelpBox("Error!", MessageType.Error);
                return;
            } 

            EditorGUILayout.LabelField("StateName :  ");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (changingStateName == false)
            {
                EditorGUILayout.LabelField("[<color=blue>"+
                    controllerWindow.jVehicleFrame.states[stateNumber].stateName+"</color>]"
                    , richTextStyle_Left); 
            }
            else
            {
                controllerWindow.jVehicleFrame.states[stateNumber].stateName = 
                    EditorGUILayout.TextField(
                        controllerWindow.jVehicleFrame.states[stateNumber].stateName  );
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("ResourceName :  ");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (changingStateName == false)
            { 
                EditorGUILayout.LabelField("[<color=blue>" +
                    controllerWindow.jVehicleFrame.states[stateNumber].ResourceName + "</color>]"
                    , richTextStyle_Left);
            }
            else
            {
                controllerWindow.jVehicleFrame.states[stateNumber].ResourceName =
                    EditorGUILayout.TextField(
                        controllerWindow.jVehicleFrame.states[stateNumber].ResourceName);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("ChangeStateName"))
            {
                changingStateName = !changingStateName;
            }
            EditorGUILayout.Space();
            if(GUILayout.Button("Enter"))
            {

            }
        }


        protected override void OnMyRemoveNode_AfterRemove()
        {
            if(controllerWindow.jVehicleFrame != null)
            {
                controllerWindow.jVehicleFrame.states.RemoveAt(stateNumber);
            }
            controllerWindow.RefreshNodes();
        }

        protected override void AfterDrag()
        {
            if (IfNodeError) return;
            controllerWindow.jVehicleFrame.states[stateNumber].mousePos.x = rect.x;
            controllerWindow.jVehicleFrame.states[stateNumber].mousePos.y = rect.y;
        }

    }
}


