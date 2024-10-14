using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using JNodeBase;
using JVehicleFrameSystem;

namespace JAnimationSystem
{
    public class JVehicleFrameEditWindow : NodeBasedEditorTest
    {
        public enum NodeType
        {
            States,
            BoneFramework,
            BoneMarkers
        }

        GUIStyle richTextStyle_Left;

        public JVehicleFrame jVehicleFrame;
        private NodeType nodeType = NodeType.States;
        private VehicleBuilder vehicleBuilder = new VehicleBuilder();

        public static void OpenWindow(JVehicleFrame _JVehicleFrame)
        {
            JVehicleFrameEditWindow window = GetWindow<JVehicleFrameEditWindow>();
            var icon = Resources.Load("Textures/jicon2") as Texture;
            window.titleContent = new GUIContent("JVehicleFrameEditWindow", icon);
            window.GenerateStyles();
            window.jVehicleFrame = _JVehicleFrame;
            window.RefreshNodes();
        }

        [MenuItem("JTools/VehicleFrameEditWindow")]
        private static void OpenWindow()
        {
            JVehicleFrameEditWindow window = GetWindow<JVehicleFrameEditWindow>();
            var icon = Resources.Load("Textures/jicon2") as Texture;
            window.titleContent = new GUIContent("JVehicleFrameEditWindow", icon);
            window.GenerateStyles();

            window.RefreshNodes();
        }

        void GenerateStyles()
        {
            richTextStyle_Left = new GUIStyle();
            richTextStyle_Left.richText = true;
        }

        /// <summary>
        /// 显示右键菜单
        /// </summary>
        protected override void ProcessContextMenu(Vector2 mousePosition)
        {
            if (jVehicleFrame == null) return;
            GenericMenu genericMenu = new GenericMenu();
            if(nodeType == NodeType.States)
            {
                genericMenu.AddItem(new GUIContent("Add New State"), false, () => OnClickAddNewState(mousePosition));
            }
            else if(nodeType == NodeType.BoneMarkers)
            {
                genericMenu.AddItem(new GUIContent("Add New BoneMarker"), false, () => OnClickAddNewBoneMarker(mousePosition));
            }
            else if (nodeType == NodeType.BoneFramework)
            {
                genericMenu.AddItem(new GUIContent("Add New Bone"), false, () => OnClickAddNewBoneOfBoneFramework(mousePosition));
                genericMenu.AddItem(new GUIContent("Add New Armor"), false, () => OnClickAddNewArmorOfBoneFramework(mousePosition));
            }
            genericMenu.ShowAsContext();
        }

        protected void OnClickAddNewState(Vector2 mousePosition)
        {
            if (nodes == null)
            {
                nodes = new List<Node>();
            }
            jVehicleFrame.states.Add(new JVehicleFrame.State(mousePosition, "NewState"));
            RefreshNodes();
        }

        protected void OnClickAddNewBoneMarker(Vector2 mousePosition)
        {
            if (nodes == null)
            {
                nodes = new List<Node>();
            }
            jVehicleFrame.boneMarkerNames.Add(new JVehicleFrame.boneMarkerName(mousePosition));
            RefreshNodes();
        }

        protected void OnClickAddNewBoneOfBoneFramework(Vector2 mousePosition)
        {
            if (nodes == null)
            {
                nodes = new List<Node>();
            }
            jVehicleFrame.boneFramework.Bones.Add(new BoneFramework.Bone());
            int index1 = jVehicleFrame.boneFramework.Bones.Count - 1;
            jVehicleFrame.boneFramework.Bones[index1].isArmor = false;
            jVehicleFrame.boneFrameworkMousePosList.Add(mousePosition);
            RefreshNodes();
        }
        protected void OnClickAddNewArmorOfBoneFramework(Vector2 mousePosition)
        {
            if (nodes == null)
            {
                nodes = new List<Node>();
            }
            jVehicleFrame.boneFramework.Bones.Add(new BoneFramework.Bone());
            int index1 = jVehicleFrame.boneFramework.Bones.Count - 1;
            jVehicleFrame.boneFramework.Bones[index1].isArmor = true;
            jVehicleFrame.boneFrameworkMousePosList.Add(mousePosition);
            RefreshNodes();
        }

        protected void OnCreateConnection_BoneFramework()
        {
            int numberIN = ((JVehicleFrameEditW_BoneFrameNode)selectedInPoint.node).numberOfBoneOfBoneFramework;
            int numberOUT = ((JVehicleFrameEditW_BoneFrameNode)selectedOutPoint.node).numberOfBoneOfBoneFramework;
            jVehicleFrame.boneFramework.
                Bones[numberOUT].ChildsList.Add(
                new BoneFramework.Bone.BoneChild(numberIN));
        }
        protected void OnRemoveConnection_BoneFramework(Connection _connection)
        {
            int _numberOUT =
                ((JVehicleFrameEditW_BoneFrameNode)_connection.outPoint.node).numberOfBoneOfBoneFramework;
            int _numberIN =
                 ((JVehicleFrameEditW_BoneFrameNode)_connection.inPoint.node).numberOfBoneOfBoneFramework;
            jVehicleFrame.boneFramework.Bones[_numberOUT].ChildsList
                . RemoveAt(_numberIN);
        }

        public void RefreshNodes ()
        {
            ClearAllNodesAndConnections();
            if (jVehicleFrame == null) return;

            if (nodeType == NodeType.States)
            {
                for (int i = 0; i < jVehicleFrame.states.Count; i++)
                {
                    JVehicleFrameEditW_StateNode newNode =
                  new JVehicleFrameEditW_StateNode(jVehicleFrame.states[i].mousePos, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                    nodes.Add(newNode);
                    newNode.SetControllerWindow(this);
                    newNode.SetStateNumber(i);
                }
            }
            else if(nodeType == NodeType.BoneMarkers)
            {
                for (int i = 0; i < jVehicleFrame.boneMarkerNames.Count; i++)
                {
                    JVehicleFrameEditW_BoneMarkerNode newNode =
                  new JVehicleFrameEditW_BoneMarkerNode(jVehicleFrame.boneMarkerNames[i].mousePos, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                    nodes.Add(newNode);
                    newNode.SetControllerWindow(this);
                    newNode.SetNumberOfBoneMarker(i);
                    newNode.FindBoneMarkerByName(jVehicleFrame.boneMarkerNames[i].Name);
                }
            }
            else if (nodeType == NodeType.BoneFramework)
            {
                OnCreateConnection = OnCreateConnection_BoneFramework;
                OnRemoveConnection = OnRemoveConnection_BoneFramework;
                canRemoveFirstNode = false;
                multiLinkToInNode = false;//不允许有多个输出连到同一个输入
                //nodes
                for (int i = 0; i < jVehicleFrame.boneFramework.Bones.Count; i++)
                {
                    JVehicleFrameEditW_BoneFrameNode newNode =
                  new JVehicleFrameEditW_BoneFrameNode(jVehicleFrame.boneFrameworkMousePosList[i], nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, jVehicleFrame.boneFramework.Bones[i].isArmor);
                    nodes.Add(newNode);
                    newNode.SetControllerWindow(this);
                    newNode.SetNumberOfBoneOfBoneFrame(i);
                    if(i == 0)
                    {
                        newNode.SetUseConnections_In(false);
                    }
                }
                //connections
                for (int i = 0; i < jVehicleFrame.boneFramework.Bones.Count; i++)
                {
                    if (jVehicleFrame.boneFramework.Bones[i].ChildsList.Count > 0)
                    {
                        for(int j= jVehicleFrame.boneFramework.Bones[i].ChildsList.Count-1; j>=0 ; j--)
                        {
                            int number = jVehicleFrame.boneFramework.Bones[i].ChildsList[j].numberInList;
                            if (number < nodes.Count)
                            {
                                selectedInPoint = nodes[number].inPoint;
                                selectedOutPoint = nodes[i].outPoint;
                                CreateConnection(false);
                                ClearConnectionSelection();
                            }
                            else
                            {
                                jVehicleFrame.boneFramework.Bones[i].ChildsList.RemoveAt(j);
                            }
                        }
                    }
                }
            }

        }

        protected override void DrawMyGUI()
        {
            EditorGUILayout.Space();
            Rect lastRect = GUILayoutUtility.GetLastRect();
            GUI.Box(new Rect(1, 0, lastRect.width-3, 40), "");
            GUI.Box(new Rect(lastRect.width - 185, 50, 183, 100), "");
          

            JVehicleFrame _jVehicleFrame =
                EditorGUILayout.ObjectField("JVehicleFrame", jVehicleFrame, typeof(JVehicleFrame), false) as JVehicleFrame;
            if(_jVehicleFrame != jVehicleFrame)
            {
                jVehicleFrame = _jVehicleFrame;
                RefreshNodes();
            }

            if (nodeType == NodeType.States)
            {
                GUI.enabled = false;
                GUI.Label(new Rect(lastRect.width - 180, 60, 160, 20), "Now: States", "BoldLabel");
                GUI.enabled = true;
                bool selectBoneMarkers =
                     GUI.Button(new Rect(lastRect.width - 180, 85, 170, 20), "BoneMarkers");
                if (selectBoneMarkers)
                {
                    nodeType = NodeType.BoneMarkers;
                    RefreshNodes();
                    return;
                }
                bool selectBoneFramework =
                    GUI.Button(new Rect(lastRect.width - 180, 115, 170, 20), "BoneFramework");
                if (selectBoneFramework)
                {
                    nodeType = NodeType.BoneFramework;
                    RefreshNodes();
                    return;
                }
            }
            else if (nodeType == NodeType.BoneMarkers)
            {
                GUI.enabled = false;
                GUI.Label(new Rect(lastRect.width - 180, 60, 160, 20), "Now: BoneMarkers", "BoldLabel");
                GUI.enabled = true;
                bool selectState =
                     GUI.Button(new Rect(lastRect.width - 180, 85, 170, 20), "States");
                if (selectState)
                {
                    nodeType = NodeType.States;
                    RefreshNodes();
                    return;
                }
                bool selectBoneFramework =
                    GUI.Button(new Rect(lastRect.width - 180, 115, 170, 20), "BoneFramework");
                if (selectBoneFramework)
                {
                    nodeType = NodeType.BoneFramework;
                    RefreshNodes();
                    return;
                }
            }
            else if (nodeType == NodeType.BoneFramework)
            {
                GUI.enabled = false;
                GUI.Label(new Rect(lastRect.width - 180, 60, 160, 20), "Now: BoneFramework", "BoldLabel");
                GUI.enabled = true;
                bool selectState =
                     GUI.Button(new Rect(lastRect.width - 180, 85, 170, 20), "States");
                if (selectState)
                {
                    nodeType = NodeType.States;
                    RefreshNodes();
                    return;
                }
                bool selectBoneMarkers =
                    GUI.Button(new Rect(lastRect.width - 180, 115, 170, 20), "BoneMarkers");
                if (selectBoneMarkers)
                {
                    nodeType = NodeType.BoneMarkers;
                    RefreshNodes();
                    return;
                }

                if(jVehicleFrame != null)
                {
                    GUILayout.Space(30);
                    if (GUILayout.Button("BuildFramework", GUILayout.Width(160)))
                    {
                        vehicleBuilder.SetBoneFramework(jVehicleFrame.boneFramework);
                        vehicleBuilder.Build(1);
                    }
                }
                
            }
        }

    }

}