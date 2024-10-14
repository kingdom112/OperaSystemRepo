using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JAnimationSystem
{

    public class JDragAxis : MonoBehaviour
    {

        public Transform dragTarget;
        public Transform forwardT;
        public float speed = 0.2f;
        private Vector3 lastPos = Vector3.zero;



        void OnMouseDown()
        {
            lastPos = Input.mousePosition;
        }

        void OnMouseDrag()
        {
            Vector3 offset = Input.mousePosition - lastPos;
            lastPos = Input.mousePosition;
            Vector3 dir = Camera.main.WorldToScreenPoint(forwardT.position +
                forwardT.forward * 1f)
                - Camera.main.WorldToScreenPoint(forwardT.position);
            float _length = offset.magnitude;
            dir.Normalize();
            offset.Normalize();
            dragTarget.position += forwardT.forward * _length * Vector3.Dot(dir, offset) * speed * Time.deltaTime;
        }

    }

}