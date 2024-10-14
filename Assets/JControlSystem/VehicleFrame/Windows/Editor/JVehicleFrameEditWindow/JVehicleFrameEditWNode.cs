using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using JNodeBase;

namespace JAnimationSystem
{
    public class JVehicleFrameEditWNode : Node
    {

        /// <summary>
        /// 构造器
        /// </summary>
        public JVehicleFrameEditWNode(Vector2 position, GUIStyle nodeStyle,
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
        }

        /// <summary>
        /// Get this node's name.
        /// </summary>
        protected virtual string NodeTitleName
        {
            get
            {
                return "JAnimControllerWNode";
            }
        }
        public override float width
        {
            get
            {
                return 400;
            }
        }
        public override float height
        {
            get
            {
                return 200;
            }
        }

        public override void DrawInNodeWindow(int id)
        {
            EditorGUILayout.LabelField("This is a example window.");
        }

    }
}
