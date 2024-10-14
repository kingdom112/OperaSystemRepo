using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace JControlSystem
{
    public class caster_base : MonoBehaviour
    {
        [Header("射线检测器，会得到发射点到碰撞点之间的最短距离。")]
        [Space(10)]
        [Tooltip("Layers that the character can walk on")]
        public LayerMask groundLayer = 1 << 0;
        [Tooltip("最大检测距离")]
        public float castDistance = 2f;
        [Tooltip("检测起始位置的偏移")]
        public Vector3 castPosOffset = Vector3.zero;
        [Tooltip("检测起始位置的偏移是否使用LocalScale")]
        public bool useLocalScaleToPosOffset = false;

       


        /// <summary>
        /// 如果大于等于10000表面没检测到地面。
        /// (默认向Vector3.down方向，以本身为objectRoot进行检测。)
        /// </summary>
        /// <returns></returns>
        public virtual float CheckGroundDistance()
        {
            return CheckGroundDistance(Vector3.down,transform);
        }
        /// <summary>
        /// 如果大于等于10000表面没检测到地面。
        /// </summary>
        /// <returns></returns>
        public virtual float CheckGroundDistance(Vector3 castDirection,Transform objectRoot)
        {
            nowCastDirection = castDirection;
            Vector3 pos = GetCastOriginPos(castPosOffset, useLocalScaleToPosOffset);
            Ray ray = new Ray(pos, castDirection);
            RaycastHit[] hits = Physics.RaycastAll(ray, castDistance, groundLayer);
            for (int i=0;i<hits.Length;i++)
            {
                if (hits[i].collider.transform.root != objectRoot)
                {
                    return hits[i].distance;
                }
            }
            return 10000;
        }
        

        /// <summary>
        /// Get the Angle between -castDirection and RaycastHit.normal . If no ground here , this will return a '0' .
        /// </summary>
        /// <param name="castDirection"></param>
        /// <param name="objectRoot"></param>
        /// <returns></returns>
        public virtual float CheckGroundAngle(Vector3 castDirection, Transform objectRoot)
        {
            nowCastDirection = castDirection;
            Vector3 pos = GetCastOriginPos(castPosOffset, useLocalScaleToPosOffset);
            Ray ray = new Ray(pos, castDirection);
            RaycastHit[] hits = Physics.RaycastAll(ray, castDistance, groundLayer);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.transform.root != objectRoot)
                {
                    return Vector3.Angle(-castDirection, hits[i].normal);
                }
            }
            return 0;
        }

        /// <summary>
        /// 计算得到检测线的原点的位置
        /// </summary>
        /// <param name="_offset"></param>
        /// <param name="_useLocalScaleToPosOffset"></param>
        /// <returns></returns>
        protected Vector3 GetCastOriginPos(Vector3 _offset,bool _useLocalScaleToPosOffset)
        {
            if(_useLocalScaleToPosOffset)
            {
                return
                     transform.position
                    + transform.right * _offset.x * transform.localScale.x
                    + transform.forward * _offset.z * transform.localScale.z
                    + transform.up * _offset.y * transform.localScale.y;
            }
            else
            {
                return
                    transform.position
                + transform.right * _offset.x
                + transform.forward * _offset.z
                + transform.up * _offset.y;
            }
        }

        Vector3 nowCastDirection = Vector3.down;
        void OnDrawGizmos()//在场景视图显示视野范围
        {
            Gizmos.color = Color.red;
            Vector3 pos = GetCastOriginPos(castPosOffset, useLocalScaleToPosOffset);
            Gizmos.DrawLine(pos, pos + nowCastDirection * castDistance);
        }
    }
}