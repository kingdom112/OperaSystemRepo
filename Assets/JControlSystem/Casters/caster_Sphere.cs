using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JControlSystem
{
    public class caster_Sphere : caster_base
    {
        [Tooltip("Sphere半径")]
        public float radius = 1f;
        
        /// <summary>
        /// 如果大于等于10000表面没检测到地面。
        /// </summary>
        /// <returns></returns>
        public override float CheckGroundDistance(Vector3 castDirection, Transform objectRoot)
        {
            _nowCastDirection = castDirection;
            Vector3 pos = GetCastOriginPos(castPosOffset, useLocalScaleToPosOffset);
            Ray ray = new Ray(pos, castDirection);
            RaycastHit[] hits = Physics.SphereCastAll(ray,radius, castDistance, groundLayer);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.transform.root != objectRoot)
                {
                    return (hits[i].point - pos).magnitude;
                }
            }
            return 10000;
        }


        Vector3 _nowCastDirection = Vector3.down;
        void OnDrawGizmos()//在场景视图显示视野范围
        {
            Gizmos.color = Color.red;
            Vector3 pos = GetCastOriginPos(castPosOffset, useLocalScaleToPosOffset);
            Gizmos.DrawLine(pos, pos + _nowCastDirection * castDistance);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pos, radius);
        }
    }
}