using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JControlSystem
{
    public class JInputerBase : MonoBehaviour
    {
        [Header("Inputer")]
        [Space(10)]
        [Header("Default Inputs")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";
        public KeyCode jumpKey = KeyCode.Space;

        [Header("Camera Settings")]
        public string rotateCameraXInput = "Mouse X";
        public string rotateCameraYInput = "Mouse Y";
        public int rotateCamearXSpeed = 65;
        public int rotateCamearYSpeed = 65;


        Vector2 _input = Vector2.zero;
        public Vector2 input
        {
            get
            {
                return _input;
            }
        }
        Vector2 _CameraInput = Vector2.zero;
        public Vector2 cameraInput
        {
            get
            {
                return _CameraInput;
            }
        }
        bool _jumping = false;
        public bool jumping
        {
            get
            {
                return _jumping;
            }
        }

        void Start()
        {

        }

        protected virtual void LateUpdate()
        {
           // if (cc == null) return;             // returns if didn't find the controller		    
            InputHandle();                      // update input methods
            //UpdateCameraStates();               // update camera states
        }

        protected virtual void FixedUpdate()
        {
            //cc.AirControl();
           // CameraInput();
        }

        protected virtual void Update()
        {
            /*cc.UpdateMotor();                   // call ThirdPersonMotor methods               
            cc.UpdateAnimator();                // call ThirdPersonAnimator methods		*/               
        }

        protected virtual void InputHandle()
        {
            CameraInput();
            MoveInput();
        }



        protected virtual void MoveInput()
        {
            _input.x = Input.GetAxis(horizontalInput);
            _input.y = Input.GetAxis(verticallInput);
            _jumping = Input.GetKey(jumpKey);
        }

        protected virtual void CameraInput()
        {
            _CameraInput.y = Input.GetAxis(rotateCameraYInput);
            _CameraInput.x = Input.GetAxis(rotateCameraXInput);
        }
    }
}