using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JControlSystem
{
    public class JCameraControlBase : MonoBehaviour
    {

        public float angleXmax = 60f;
        public float angleXmin = -60f;
        public float angleYmax = 80f;
        public float angleYmin = -70f;
        public Transform parentX;
        public Transform parentY;
        public bool lockX = true;
        public bool lockY = false;

        /// <summary>
        /// 使用_r作为旋转角度对拥有层级的主摄像机进行旋转。
        /// 会返回角度溢出值。
        /// </summary>
        /// <param name="_r"></param>
        /// <returns></returns>
        public Vector2 RotateCameraWithOverflow (Vector2 _r)
        {
            Vector2 _overflow = Vector2.zero;
            if(lockY == false)
            {
                Quaternion newRotationY;
                _overflow.y = -RotationAroundXAxis_Overflow(parentY.localRotation * Quaternion.Euler(-_r.y, 0f, 0f), angleYmin, angleYmax, out newRotationY);
                parentY.localRotation = newRotationY;
            }
            else
            {
                _overflow.y = _r.y;
                parentY.localRotation = Quaternion.Lerp(parentY.localRotation, Quaternion.Euler(0, 0, 0), 0.2f);
            }

            if (lockX == false)
            {
                Quaternion newRotationX;
                _overflow.x = RotationAroundYAxis_Overflow(parentX.localRotation * Quaternion.Euler(0f, _r.x, 0f), angleXmin, angleXmax, out newRotationX);
                parentX.localRotation = newRotationX;
            }
            else
            {
                _overflow.x = _r.x;
                parentX.localRotation = Quaternion.Lerp(parentX.localRotation, Quaternion.Euler(0, 0, 0), 0.2f);
            }
            return _overflow;
        }



        /// <summary>
        ///  摄像机X转轴的角度溢出。用于机体的视角旋转。(X转轴控制上下)
        /// </summary>
        /// <param name="q"></param>
        /// <param name="min1"></param>
        /// <param name="max1"></param>
        /// <param name="_result"></param>
        /// <returns></returns>
        float RotationAroundXAxis_Overflow(Quaternion q, float min1, float max1,out Quaternion _result)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            float overflow = angleX - Mathf.Clamp(angleX, min1, max1);
            if (angleX > max1 || angleX < min1)
            {
                angleX = Mathf.Clamp(angleX, min1, max1);
                q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
                _result = q;
                return overflow;
            }
            else
            {
                angleX = Mathf.Clamp(angleX, min1, max1);
                q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
                _result = q;
                return 0f;
            }
        }

        /// <summary>
        /// 摄像机Y转轴的角度溢出。用于机体的视角旋转。(Y转轴控制左右)
        /// </summary>
        /// <param name="q"></param>
        /// <param name="min1"></param>
        /// <param name="max1"></param>
        /// <param name="_result"></param>
        /// <returns></returns>
        float RotationAroundYAxis_Overflow(Quaternion q, float min1, float max1, out Quaternion _result)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);

            float overflow = angleY - Mathf.Clamp(angleY, min1, max1);
            if (angleY > max1 || angleY < min1)
            {
                angleY = Mathf.Clamp(angleY, min1, max1);
                q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);
                _result = q;
                return overflow;
            }
            else
            {
                angleY = Mathf.Clamp(angleY, min1, max1);
                q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);
                _result = q;
                return 0f;
            }
        }

    }
}