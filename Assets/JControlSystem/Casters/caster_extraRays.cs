using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JControlSystem
{
    public class caster_extraRays : caster_base
    {
        public List<Vector3> extraOffsetOfRays = new List<Vector3>();


        
        /// <summary>
        /// 如果大于等于10000表面没检测到地面。
        /// </summary>
        /// <returns></returns>
        public override float CheckGroundDistance(Vector3 castDirection, Transform objectRoot)
        {
            _nowCastDirection = castDirection;
            List<float> distances = new List<float>();
            for (int i=-1;i< extraOffsetOfRays.Count;i++)
            {
                Vector3 pos = Vector3.zero;
                if (i >= 0)
                {
                    pos = GetCastOriginPos(extraOffsetOfRays[i], useLocalScaleToPosOffset);
                }
                else
                {
                    pos = GetCastOriginPos(castPosOffset, useLocalScaleToPosOffset);
                }
                Ray ray = new Ray(pos, castDirection);
                distances.Add( GetRayCastMinDistance(ray, objectRoot) );
            }
            distances.Sort();
            return distances[0];
        }


        float GetRayCastMinDistance (Ray ray1, Transform objectRoot)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray1, castDistance, groundLayer);
            float distance1 = 10000;
            for(int i=0;i<hits.Length;i++)
            {
                if (hits[i].collider.transform.root != objectRoot)
                {
                    if (hits[i].distance < distance1)
                    {
                        distance1 = hits[i].distance;
                    }
                }
            }
            return distance1;
        }

        Vector3 _nowCastDirection = Vector3.down;
        void OnDrawGizmos()//在场景视图显示视野范围
        {
            Gizmos.color = Color.red;
            for (int i = -1; i < extraOffsetOfRays.Count; i++)
            {
                Vector3 pos = Vector3.zero;
                if (i >= 0)
                {
                    pos = GetCastOriginPos(extraOffsetOfRays[i], useLocalScaleToPosOffset);
                }
                else
                {
                    pos = GetCastOriginPos(castPosOffset, useLocalScaleToPosOffset);
                }
                Gizmos.DrawLine(pos, pos + _nowCastDirection * castDistance);
            }
        }
    }
}