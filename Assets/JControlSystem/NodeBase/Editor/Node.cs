using System;
using UnityEditor;
using UnityEngine;

namespace JNodeBase
{

    public class Node
    {
        public Rect rect;
        public string title = "Example Window";
        public bool isDragged;
        public bool isSelected;
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;
        public GUIStyle style;
        public GUIStyle defaultNodeStyle;
        public GUIStyle selectedNodeStyle;
        public Action<Node> OnRemoveNode;

        public Node(Vector2 position, GUIStyle nodeStyle,
            GUIStyle selectedStyle, GUIStyle inPointStyle,
            GUIStyle outPointStyle,
            Action<ConnectionPoint> OnClickInPoint,
            Action<ConnectionPoint> OnClickOutPoint,
            Action<Node> OnClickRemoveNode)
        {
            rect = new Rect(position.x, position.y, width, height);
            style = nodeStyle;
            inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
            outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
            defaultNodeStyle = nodeStyle;
            selectedNodeStyle = selectedStyle;
            OnRemoveNode = OnClickRemoveNode;
        }

        public virtual bool useConnections_In
        {
            get
            {
                return true;
            }
        }
        public virtual bool useConnections_Out
        {
            get
            {
                return true;
            }
        }

        public virtual float width
        {
            get
            {
                return 200;
            }
        }
        public virtual float height
        {
            get
            {
                return 50;
            }
        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
            AfterDrag();
        }
        protected virtual void AfterDrag()
        {
        }
        public void DrawOutNodeWindow()
        {
            if(useConnections_In)
            {
                inPoint.Draw();
            }
            if (useConnections_Out)
            {
                outPoint.Draw();
            }
        }
        public virtual void DrawInNodeWindow(int id)
        {
            EditorGUILayout.LabelField("This is a example window.");
        }
        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (rect.Contains(e.mousePosition))
                        {
                            isDragged = true;
                            GUI.changed = true;
                            isSelected = true;
                            style = selectedNodeStyle;
                        }
                        else
                        {
                            GUI.changed = true;
                            isSelected = false;
                            style = defaultNodeStyle;
                        }
                    }
                    if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    isDragged = false;
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }
            return false;
        }
        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
            genericMenu.ShowAsContext();
        }
        private void OnClickRemoveNode()
        {
            if (OnRemoveNode != null)
            {
                OnRemoveNode(this);
            }
            OnMyRemoveNode_AfterRemove();
        }
        
        protected virtual void OnMyRemoveNode_AfterRemove()
        {

        }
    }

}