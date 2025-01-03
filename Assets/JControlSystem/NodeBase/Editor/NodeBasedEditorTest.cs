﻿using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace JNodeBase
{

    public class NodeBasedEditorTest : EditorWindow
    {

        protected List<Node> nodes = new List<Node>();
        protected List<Connection> connections = new List<Connection>();
        protected GUIStyle nodeStyle;
        protected GUIStyle selectedNodeStyle;
        protected GUIStyle inPointStyle;
        protected GUIStyle outPointStyle;
        protected ConnectionPoint selectedInPoint;
        protected ConnectionPoint selectedOutPoint;
        protected Action OnCreateConnection = null;
        protected Action<Connection> OnRemoveConnection = null;
        protected Vector2 drag;
        protected Vector2 offset;
        protected bool canRemoveFirstNode = true;
        protected bool multiLinkToInNode = true;
        protected bool multiLinkToOutNode = true;

        /* [MenuItem("JTools/NodeTest/Node Based Editor (Test)")]
         private static void OpenWindow()
         {
             NodeBasedEditorTest window = GetWindow<NodeBasedEditorTest>();
             var icon = Resources.Load("Textures/jicon2") as Texture;
             window.titleContent = new GUIContent("Node Based Editor (Test)", icon);
         }*/
        protected void OnEnable()
        {
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
            inPointStyle = new GUIStyle();
            inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            inPointStyle.border = new RectOffset(4, 4, 12, 12);
            outPointStyle = new GUIStyle();
            outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            outPointStyle.border = new RectOffset(4, 4, 12, 12);
        }
        protected void OnGUI()
        {
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);
            DrawNodes();
            DrawConnections();
            DrawConnectionLine(Event.current);
            DrawMyGUI();
            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);
            if (GUI.changed) Repaint();
        }


        protected void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);
            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
            offset += drag * 0.5f;
            Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);
            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }
            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }
            Handles.color = Color.white;
            Handles.EndGUI();
        }

        protected void DrawConnectionLine(Event e)
        {
            if (selectedInPoint != null && selectedOutPoint == null)
            {
                Handles.DrawBezier(selectedInPoint.rect.center, e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white, null, 2f);

                GUI.changed = true;
            }
            if (selectedOutPoint != null && selectedInPoint == null)
            {
                Handles.DrawBezier(selectedOutPoint.rect.center, e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white, null, 2f);

                GUI.changed = true;
            }
        }

        /// <summary>
        /// 画节点
        /// </summary>
        protected void DrawNodes()
        {
            if (nodes != null)
            {
                BeginWindows(); //开始绘制弹出窗口
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].DrawOutNodeWindow();
                    GUI.Window(i, nodes[i].rect, nodes[i].DrawInNodeWindow, nodes[i].title);
                }
                EndWindows();//结束绘制弹出窗口
            }
        }

        protected void DrawConnections()
        {
            if (connections != null)
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    connections[i].Draw();
                }
            }
        }

        protected virtual void DrawMyGUI ()
        {

        }

        protected void ProcessNodeEvents(Event e)
        {
            if (nodes != null)
            {
                for (int i = nodes.Count - 1; i >= 0; i--)
                {
                    bool guiChanged = nodes[i].ProcessEvents(e);
                    if (guiChanged)
                    {
                        GUI.changed = true;
                    }
                }
            }
        }
        protected void ProcessEvents(Event e)
        {
            drag = Vector2.zero;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        ClearConnectionSelection();
                    }
                    if (e.button == 1)
                    {
                        ProcessContextMenu(e.mousePosition);
                    }
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        OnDrag(e.delta);
                    }
                    break;
            }
        }
        protected void OnDrag(Vector2 delta)
        {
            drag = delta;
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].Drag(delta);
                }
            }
            GUI.changed = true;
        }


        /// <summary>
        /// 显示右键菜单
        /// </summary>
        /// <param name="mousePosition"></param>
        protected virtual void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
            genericMenu.ShowAsContext();
        }

        private void OnClickAddNode(Vector2 mousePosition)
        {
            //if (nodes == null)
            //{
                //nodes = new List<Node>();
           // }
            //nodes.Add(new Node(mousePosition, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
        }
        protected void OnClickRemoveNode(Node node)
        {
            if(canRemoveFirstNode == false)
            {
                if (nodes[0] == node)
                {
                    Debug.Log("You can't remove the first node here!");
                    return;
                }
            }

            SystemRemoveNode(node);
        }
        protected void SystemRemoveNode(Node node)
        {
            if (connections != null)
            {
                List<Connection> connectionsToRemove = new List<Connection>();
                for (int i = 0; i < connections.Count; i++)
                {
                    if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                    {
                        connectionsToRemove.Add(connections[i]);
                    }
                }
                for (int i = 0; i < connectionsToRemove.Count; i++)
                {
                    connections.Remove(connectionsToRemove[i]);
                }
                connectionsToRemove = null;
            }
            nodes.Remove(node);
        }


        protected void ClearAllNodesAndConnections ()
        {
            ClearConnectionSelection();
            if(nodes != null)
            {
                for (int i = nodes.Count - 1; i >= 0; i--)
                {
                    SystemRemoveNode(nodes[i]);
                }
            }
            else
            {
                nodes = new List<Node>();
            }
            OnCreateConnection = null;
            OnRemoveConnection = null;
            canRemoveFirstNode = true;
            multiLinkToInNode = true;
            multiLinkToOutNode = true;
        }

        protected void OnClickInPoint(ConnectionPoint inPoint)
        {
            selectedInPoint = inPoint;
            if (selectedOutPoint != null)
            {
                if (selectedOutPoint.node != selectedInPoint.node)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
        }
        protected void OnClickOutPoint(ConnectionPoint outPoint)
        {
            selectedOutPoint = outPoint;
            if (selectedInPoint != null)
            {
                if (selectedOutPoint.node != selectedInPoint.node)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
        }
        protected void OnClickRemoveConnection(Connection connection)
        {
            connections.Remove(connection);
            OnRemoveConnection(connection);
        }
        protected void CreateConnection(bool UseOnCreatConnection = true)
        {
            if (connections == null)
            {
                connections = new List<Connection>();
            }
            bool has = false;
            for(int i=0; i<connections.Count; i++)
            {
                if (connections[i].inPoint == selectedInPoint && connections[i].outPoint == selectedOutPoint)
                {
                    has = true;
                    break;
                }
            }
            if(has == false)
            {
                if(multiLinkToOutNode == false)
                {
                    for (int i = 0; i < connections.Count; i++)
                    {
                        if (connections[i].outPoint == selectedOutPoint)
                        {
                            has = true;
                            break;
                        }
                    }
                }
                if (multiLinkToInNode == false)
                {
                    for (int i = 0; i < connections.Count; i++)
                    {
                        if (connections[i].inPoint == selectedInPoint)
                        {
                            has = true;
                            break;
                        }
                    }
                }
                if (has == false)
                {
                    connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
                    if (UseOnCreatConnection) OnCreateConnection();
                }
               
            }
          
        }
        protected void ClearConnectionSelection()
        {
            selectedInPoint = null;
            selectedOutPoint = null;
        }

    }

}