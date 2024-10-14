using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JControlSystem
{

    public class JEngineBase : MonoBehaviour
    {

        public float Force_move = 400;
        public float Force_Roate = 450;


        public float MoveAcceleration(float mass)
        {
            return Force_move / mass;
        }
        public float RoateAcceleration(float mass)
        {
            return Force_Roate / mass;
        }


        void Start()
        {

        }


        void Update()
        {

        }



    }
}