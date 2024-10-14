using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JControlSystem
{
    public class JMoverBase : MonoBehaviour
    {
        public JInputerBase _inputer;
        public JEngineBase _engine;
        public float moveSpeed = 5f;

        public List<caster_base> casterList = new List<caster_base>();
        public JCameraControlBase cameraC;
        public Transform TRoot;

        private Rigidbody _rigidbody;
       // [HideInInspector]
        public bool isGrounded = false;
        //[HideInInspector]
        public bool isSliding = false;

        [Tooltip("Distance to became not grounded")]
        [SerializeField]
        protected float groundMinDistance = 0.2f;
        [SerializeField]
        protected float groundMaxDistance = 0.5f;

        [Tooltip("ADJUST IN PLAY MODE - Offset height limit for sters - GREY Raycast in front of the legs")]
        public float stepOffsetEnd = 0.45f;
        [Tooltip("ADJUST IN PLAY MODE - Offset height origin for sters, make sure to keep slight above the floor - GREY Raycast in front of the legs")]
        public float stepOffsetStart = 0.05f;
        [Tooltip("Higher value will result jittering on ramps, lower values will have difficulty on steps")]
        public float stepSmooth = 4f;
        [Tooltip("Max angle to walk")]
        [SerializeField]
        protected float slopeLimit = 45f;

        [Tooltip("Apply extra gravity when the character is not grounded")]
        [SerializeField]
        protected float extraGravity = -10f;
        protected float groundDistance;


        [HideInInspector]
        public PhysicMaterial maxFrictionPhysics, frictionPhysics, slippyPhysics;       // create PhysicMaterial for the Rigidbody
        public void Init()
        {
            // slides the character through walls and edges
            frictionPhysics = new PhysicMaterial();
            frictionPhysics.name = "frictionPhysics";
            frictionPhysics.staticFriction = .25f;
            frictionPhysics.dynamicFriction = .25f;
            frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;

            // prevents the collider from slipping on ramps
            maxFrictionPhysics = new PhysicMaterial();
            maxFrictionPhysics.name = "maxFrictionPhysics";
            maxFrictionPhysics.staticFriction = 1f;
            maxFrictionPhysics.dynamicFriction = 1f;
            maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;

            // air physics 
            slippyPhysics = new PhysicMaterial();
            slippyPhysics.name = "slippyPhysics";
            slippyPhysics.staticFriction = 0f;
            slippyPhysics.dynamicFriction = 0f;
            slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;

            // rigidbody info
            _rigidbody = GetComponent<Rigidbody>();
        }
        void Start()
        {
            Init();
            SetTheColliderMaterial(frictionPhysics);
        }

        protected virtual void LateUpdate()
        {
            float roateNeed = cameraC.RotateCameraWithOverflow(
                new Vector2(_inputer.cameraInput.x * _inputer.rotateCamearXSpeed * Time.deltaTime
                , _inputer.cameraInput.y * _inputer.rotateCamearYSpeed * Time.deltaTime)).x;
            float roateReal = GetRealAcceleration(isGrounded, Mathf.Abs(roateNeed),_engine.RoateAcceleration(_rigidbody.mass) , 3);
            transform.rotation *= Quaternion.Euler(0f, roateNeed < 0 ? -roateReal : roateReal, 0f);
        }

        protected virtual void FixedUpdate()
        {
            controlVelocityInFixedUpdate(moveSpeed, 0.4f);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                _rigidbody.velocity += -Camera.main.transform.forward * 25f;
            }
            CheckGround(TRoot);
        }

        protected void controlVelocityInFixedUpdate(float _moveSpeed, float _groundDynamicFrictionFactor)
        {
            Vector3 _velocity = _rigidbody.velocity;
            Vector3 dir = (transform.forward * _inputer.input.y + transform.right * _inputer.input.x).normalized;
            float factor = Mathf.Clamp01(Mathf.Abs(_inputer.input.x) + Mathf.Abs(_inputer.input.y));

            float airResistance = GetAirResistance(_velocity.magnitude);
            float a_air = airResistance / _rigidbody.mass;
            Vector3 deltaV = dir * _moveSpeed * factor - _velocity;
            float a_need = deltaV.magnitude / 0.1f;
            float a_can = _engine.MoveAcceleration(_rigidbody.mass);
            float a_GroundDynamicFriction = _groundDynamicFrictionFactor * 10f;
            float a_real = GetRealAcceleration(isGrounded, a_need, a_can, a_GroundDynamicFriction);

            _velocity = _velocity + deltaV.normalized * a_real * Time.fixedDeltaTime - _velocity.normalized * a_air * Time.fixedDeltaTime;

            if (_inputer.jumping == true)
            {
                _velocity.y = 4f ;
            }
            else
            {
                _velocity.y = _rigidbody.velocity.y;
            }

            _rigidbody.velocity = _velocity;
        }


        protected float GetRealAcceleration(bool _isGrounded,float _acceleration_Need, float _acceleration_can,float _a_GroundDynamicFriction)
        {
            float a_real = 0f;
            if (_isGrounded)
            {//grounded == true
                if (_acceleration_Need <= _a_GroundDynamicFriction)
                {
                    a_real = _acceleration_Need;
                }
                else if (_acceleration_Need <= _a_GroundDynamicFriction + _acceleration_can)
                {
                    a_real = _acceleration_Need;
                }
                else
                {
                    a_real = _a_GroundDynamicFriction + _acceleration_can;
                }
            }
            else
            {//grounded == false
                if (_acceleration_Need <= _acceleration_can)
                {
                    a_real = _acceleration_Need;
                }
                else
                {
                    a_real = _acceleration_can;
                }
            }
            return a_real;
        }

     

        /// <summary>
        /// 空气阻力
        /// </summary>
        /// <returns></returns>
        protected float GetAirResistance (float _speedAbs)
        {
            //F=(1/2)CρSV^2
            return 0.5f * 0.3f * 1.28f * (4f ) * _speedAbs * _speedAbs;
        }

        protected void SetTheColliderMaterial (PhysicMaterial _material)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            for(int i=0;i<colliders.Length; i++)
            {
                colliders[i].material = _material;
            }
        }

        /// <summary>
        /// Check the ground.
        /// </summary>
        protected void CheckGround(Transform theRoot)
        {
            groundDistance = GetGroundDistance(theRoot);

            // change the physics material to very slip when not grounded or maxFriction when is
            if (isGrounded && _inputer.input == Vector2.zero)
                SetTheColliderMaterial(maxFrictionPhysics);
            else if (isGrounded && _inputer.input != Vector2.zero)
                SetTheColliderMaterial(frictionPhysics);
            else
                SetTheColliderMaterial(slippyPhysics);

            var magVel = (float)System.Math.Round(new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z).magnitude, 2);
            magVel = Mathf.Clamp(magVel, 0, 1);

            var groundCheckDistance = groundMinDistance;
            if (magVel > 0.25f) groundCheckDistance = groundMaxDistance;

            // clear the checkground to free the character to attack on air                
            bool onStep = StepOffset(theRoot);

            if (groundDistance <= 0.05f)
            {
                isGrounded = true;
                Sliding(theRoot);
            }
            else
            {
                if (groundDistance >= groundCheckDistance)
                {
                    isGrounded = false;
                    // check vertical velocity
                   // verticalVelocity = _rigidbody.velocity.y;

                    // apply extra gravity when falling
                    if (!onStep && !_inputer.jumping)
                        _rigidbody.AddForce(transform.up * extraGravity * Time.deltaTime, ForceMode.VelocityChange);
                }
                else if (!onStep && !_inputer.jumping)
                {
                    _rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);
                }
            }
        }

        /// <summary>
        /// Get the distance from ground.
        /// </summary>
        /// <returns></returns>
        protected float GetGroundDistance(Transform theRoot)
        {
            float distance = 10000;
            for (int i = 0; i < casterList.Count; i++)
            {
                float _distance1 = casterList[i].CheckGroundDistance(Vector3.down, theRoot);
                if (_distance1 < distance)
                {
                    distance = _distance1;
                }
            }
            return distance;
        }

        protected float GetTheSmallestAngleFromGround (Transform theRoot)
        {
            float angle1 = 90;
            for (int i = 0; i < casterList.Count; i++)
            {
                float _angle1 = casterList[i].CheckGroundAngle(Vector3.down, theRoot);
                if (_angle1 < angle1)
                {
                    angle1 = _angle1;
                }
            }
            return angle1;
        }
        protected float GetTheBiggestAngleFromGround(Transform theRoot)
        {
            float angle1 = 0;
            for (int i = 0; i < casterList.Count; i++)
            {
                float _angle1 = casterList[i].CheckGroundAngle(Vector3.down, theRoot);
                if (_angle1 > angle1)
                {
                    angle1 = _angle1;
                }
            }
            return angle1;
        }


        void Sliding(Transform theRoot)
        {
            bool onStep = StepOffset(theRoot);
            float groundAngle = GetTheSmallestAngleFromGround(theRoot);


            if (groundAngle > slopeLimit + 1f && groundAngle <= 85 &&  groundDistance <= 0.05f && !onStep)
            {
                isSliding = true;
                isGrounded = false;
                var slideVelocity = (groundAngle - slopeLimit) * 2f;
                slideVelocity = Mathf.Clamp(slideVelocity, 0, 10);
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, -slideVelocity, _rigidbody.velocity.z);
            }
            else
            {
                isSliding = false;
                isGrounded = true;
            }
        }

        protected bool StepOffset(Transform theRoot)
        {
            if (_inputer.input.sqrMagnitude < 0.1 || !isGrounded) return false;
            

            var _movementDirection = 
                _inputer.input.magnitude > 0 ? (transform.right * _inputer.input.x + transform.forward * _inputer.input.y).normalized : transform.forward;
            Ray rayStep =
                new Ray((transform.position + new Vector3(0, stepOffsetEnd, 0) +_movementDirection * (Mathf.Abs(stepOffsetEnd - stepOffsetStart) + 0.05f))
                , Vector3.down);

            RaycastHit[] hits = Physics.RaycastAll(rayStep, Mathf.Abs(stepOffsetEnd - stepOffsetStart));
            for(int i=0;i< hits.Length;i++)
            {
                RaycastHit _hit = hits[i];
                if(_hit.collider.isTrigger)  continue;
                
                if(_hit.collider.transform.root != theRoot)
                {
                    if (_hit.point.y >= (transform.position.y) && _hit.point.y <= (transform.position.y + stepOffsetEnd))
                    {
                        var _speed = Mathf.Clamp(_inputer.input.magnitude, 0, 1);
                        var velocityDirection = (_hit.point - transform.position);
                        _rigidbody.velocity = velocityDirection * stepSmooth * (_speed /** (velocity > 1 ? velocity : 1)*/);
                        return true;
                    }
                }
                else
                {
                    continue;
                }
            }
            return false;
        }


    }
}