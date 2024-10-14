using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JAnimationSystem
{
    /// <summary>
    /// One bone-type match one bone-Transform.
    /// </summary>
    [System.Serializable]
    public class BoneOneMatch
    {
        public JAnimationData.BoneType boneType;
        public Transform matchedT = null;

        public BoneOneMatch(JAnimationData.BoneType _boneType)
        {
            boneType = _boneType;
            matchedT = null;
        }
        public void SetMatchTransform(Transform _matchT)
        {
            matchedT = _matchT;
        }
        public void SetBoneType(JAnimationData.BoneType _boneType)
        {
            boneType = _boneType;
        }
    }

    /// <summary>
    /// The names which match the bone.
    /// </summary>
    public class BoneOneMatchedNames
    {
        public BoneOneMatch boneOneMatch;
        public List<string> MatchNames = new List<string>();

        public BoneOneMatchedNames(BoneOneMatch _boneOneMatch, string _matchName)
        {
            boneOneMatch = _boneOneMatch;
            MatchNames = new List<string>();
            MatchNames.Add(_matchName);
        }
        public BoneOneMatchedNames(BoneOneMatch _boneOneMatch, List<string> _matchNames)
        {
            boneOneMatch = _boneOneMatch;
            MatchNames = _matchNames;
        }

        public void AddMatchName(string _matchName)
        {
            if(MatchNames.Contains(_matchName) == false)
            {
                MatchNames.Add(_matchName);
            }
        }

        public void AddMatchName(List<string> _matchNames)
        {
            for(int i=0; i<_matchNames.Count; i++)
            {
                if (MatchNames.Contains(_matchNames[i]) == false)
                {
                    MatchNames.Add(_matchNames[i]);
                }
            } 
        }
    }

    /// <summary>
    /// One bone animationPlayer.
    /// </summary>
    public class BoneOnePlayer
    {
        public BoneOneMatch boneOneMatch;
        public JAnimationData.RoaSpaceType roaSpaceType = JAnimationData.RoaSpaceType.local;
          
        public List<Vector4> animDatas_Roa4 = new List<Vector4>();
        public List<Vector3> animDatas_Pos = new List<Vector3>();
        public List<float> animDatas_Time_roa = new List<float>();
        public List<float> animDatas_Time_pos = new List<float>();
         
        public AnimationCurve Curve_X_4_Roa = new AnimationCurve();
        public AnimationCurve Curve_Y_4_Roa = new AnimationCurve();
        public AnimationCurve Curve_Z_4_Roa = new AnimationCurve();
        public AnimationCurve Curve_W_4_Roa = new AnimationCurve();
        public AnimationCurve Curve_X_Pos = new AnimationCurve();
        public AnimationCurve Curve_Y_Pos = new AnimationCurve();
        public AnimationCurve Curve_Z_Pos = new AnimationCurve();

        /// <summary>
        /// 旋转角度偏移。每次播放动画都会在原来的旋转角度的右边乘上这个值。
        /// </summary>
        public Quaternion roaOffset = Quaternion.Euler(Vector3.zero);


        

        /// <summary>
        /// 获取三维的旋转曲线，由四元数的旋转曲线转化而来
        /// </summary>
        /// <returns></returns>
        public AnimationCurve GetCurve3_Roa_X()
        {
            if(animDatas_Roa4.Count > 0)
            {
                AnimationCurve curve1 = new AnimationCurve();
                for(int i=0; i< animDatas_Roa4.Count; i++)
                {
                    Vector3 roa1 = (new Quaternion(animDatas_Roa4[i].x, animDatas_Roa4[i].y, animDatas_Roa4[i].z, animDatas_Roa4[i].w)).eulerAngles;
                    curve1.AddKey(animDatas_Time_roa[i], roa1.x);
                }
                return curve1;
            }
            return null;
        }
        /// <summary>
        /// 获取三维的旋转曲线，由四元数的旋转曲线转化而来
        /// </summary>
        /// <returns></returns>
        public AnimationCurve GetCurve3_Roa_Y()
        {
            if (animDatas_Roa4.Count > 0)
            {
                AnimationCurve curve1 = new AnimationCurve();
                for (int i = 0; i < animDatas_Roa4.Count; i++)
                {
                    Vector3 roa1 = (new Quaternion(animDatas_Roa4[i].x, animDatas_Roa4[i].y, animDatas_Roa4[i].z, animDatas_Roa4[i].w)).eulerAngles;
                    curve1.AddKey(animDatas_Time_roa[i], roa1.y);
                }
                return curve1;
            }
            return null;
        }
        /// <summary>
        /// 获取三维的旋转曲线，由四元数的旋转曲线转化而来
        /// </summary>
        /// <returns></returns>
        public AnimationCurve GetCurve3_Roa_Z()
        {
            if (animDatas_Roa4.Count > 0)
            {
                AnimationCurve curve1 = new AnimationCurve();
                for (int i = 0; i < animDatas_Roa4.Count; i++)
                {
                    Vector3 roa1 = (new Quaternion(animDatas_Roa4[i].x, animDatas_Roa4[i].y, animDatas_Roa4[i].z, animDatas_Roa4[i].w)).eulerAngles;
                    curve1.AddKey(animDatas_Time_roa[i], roa1.z);
                }
                return curve1;
            }
            return null;
        }



        public BoneOnePlayer(BoneOneMatch _boneOneMatch) 
        {
            boneOneMatch = _boneOneMatch;  
            animDatas_Roa4 = new List<Vector4>();
            animDatas_Pos = new List<Vector3>();
            animDatas_Time_roa = new List<float>();
            animDatas_Time_pos = new List<float>();
             
            Curve_X_4_Roa = new AnimationCurve();
            Curve_Y_4_Roa = new AnimationCurve();
            Curve_Z_4_Roa = new AnimationCurve();
            Curve_W_4_Roa = new AnimationCurve();
            Curve_X_Pos = new AnimationCurve();
            Curve_Y_Pos = new AnimationCurve();
            Curve_Z_Pos = new AnimationCurve();

            roaOffset = Quaternion.Euler(Vector3.zero);
        }
         


        /// <summary>
        /// 解析一个时刻的旋转数据
        /// </summary>
        /// <param name="timeNow"></param>
        /// <returns></returns>
        public Quaternion EvaluateThisFrame_Curve(float timeNow)
        {
            if (Curve_X_4_Roa.keys.Length >= 2 &&
                    Curve_Y_4_Roa.keys.Length >= 2 &&
                    Curve_Z_4_Roa.keys.Length >= 2 &&
                    Curve_W_4_Roa.keys.Length >= 2)
            {
                Quaternion q1 = new Quaternion(
                    Curve_X_4_Roa.Evaluate(timeNow),
                    Curve_Y_4_Roa.Evaluate(timeNow),
                    Curve_Z_4_Roa.Evaluate(timeNow),
                    Curve_W_4_Roa.Evaluate(timeNow));

                q1 = q1 * roaOffset;

                return q1;
            }
            else
            {
               // Debug.LogWarning("No curve !!!");
                return Quaternion.Euler(Vector3.zero);
            }
        }

        /// <summary>
        /// 解析一个时刻的位移数据
        /// </summary>
        /// <param name="timeNow"></param>
        /// <returns></returns>
        public Vector3 EvaluateThisFramePos_Curve(float timeNow)
        {
            if (Curve_X_Pos.keys.Length >= 2 &&
                    Curve_Y_Pos.keys.Length >= 2 &&
                    Curve_Z_Pos.keys.Length >= 2  && boneOneMatch.boneType == JAnimationData.BoneType.hips)
            {
                Vector3 pos1 = new Vector3(
                    Curve_X_Pos.Evaluate(timeNow),
                    Curve_Y_Pos.Evaluate(timeNow),
                    Curve_Z_Pos.Evaluate(timeNow));
                

                return pos1;
            }
            else
            {

                return Vector3.zero;
            }
        }

        public void PlayInThisFrame_Curve(float timeNow, bool useMove = true, float mix = 1f)
        {
            if (boneOneMatch.matchedT == null) return;

            if (Curve_X_4_Roa.keys.Length >= 2 &&
                     Curve_Y_4_Roa.keys.Length >= 2 &&
                     Curve_Z_4_Roa.keys.Length >= 2 &&
                     Curve_W_4_Roa.keys.Length >= 2)
            {
                Quaternion q1 = new Quaternion(
                    Curve_X_4_Roa.Evaluate(timeNow),//获取当前time位置上对应曲线上的value
                    Curve_Y_4_Roa.Evaluate(timeNow),
                    Curve_Z_4_Roa.Evaluate(timeNow),
                    Curve_W_4_Roa.Evaluate(timeNow));

                q1 = q1 * roaOffset;

                if (roaSpaceType == JAnimationData.RoaSpaceType.local)
                {
                    boneOneMatch.matchedT.localRotation =
                        Quaternion.Slerp(boneOneMatch.matchedT.localRotation, q1, mix);
                }
                else if(roaSpaceType == JAnimationData.RoaSpaceType.world)
                {
                    boneOneMatch.matchedT.rotation =
                        Quaternion.Slerp(boneOneMatch.matchedT.rotation, q1, mix);
                }
             
            }

            if (useMove && Curve_X_Pos.keys.Length >= 2 && Curve_Y_Pos.keys.Length >= 2 && Curve_Z_Pos.keys.Length >= 2)
            {
                Vector3 v3 = new Vector3(Curve_X_Pos.Evaluate(timeNow), Curve_Y_Pos.Evaluate(timeNow), Curve_Z_Pos.Evaluate(timeNow));
                boneOneMatch.matchedT.localPosition = Vector3.Lerp(boneOneMatch.matchedT.localPosition, v3, mix);
            }
        }
        /// <summary>
        /// ( _roaOffset_euler = Vector3.zero )
        /// </summary> 
        public void SetAnimDatas(JAnimationData.RoaSpaceType _roaSpaceType,
            List<Vector4> _animDatas_Roa4, 
            List<Vector3> _animDatas_Pos, 
            List<float> _animDatas_Time_Pos,
            List<float> _animDatas_Time_Roa)
        {
            SetAnimDatas(_roaSpaceType, _animDatas_Roa4,
                _animDatas_Pos
                , _animDatas_Time_Pos, _animDatas_Time_Roa, Vector3.zero);
        }
        public void SetAnimDatas(JAnimationData.RoaSpaceType _roaSpaceType,
            List<Vector4> _animDatas_Roa4, 
            List<Vector3> _animDatas_Pos , 
            List<float> _animDatas_Time_Pos, 
            List<float> _animDatas_Time_Roa, Vector3 _roaOffset_euler)
        {
            roaSpaceType = _roaSpaceType;
            animDatas_Roa4 = _animDatas_Roa4;
            animDatas_Pos = _animDatas_Pos;
            animDatas_Time_roa = _animDatas_Time_Roa;
            animDatas_Time_pos = _animDatas_Time_Pos;
            Curve_X_4_Roa = new AnimationCurve();
            Curve_Y_4_Roa = new AnimationCurve();
            Curve_Z_4_Roa = new AnimationCurve();
            Curve_W_4_Roa = new AnimationCurve();
            roaOffset = Quaternion.Euler(_roaOffset_euler);
            for (int i = 0; i < _animDatas_Roa4.Count; i++)
            {  

                Quaternion thisQ = (new Quaternion(_animDatas_Roa4[i].x, _animDatas_Roa4[i].y, _animDatas_Roa4[i].z, _animDatas_Roa4[i].w));
                Vector4 thisRoa = new Vector4(thisQ.x, thisQ.y, thisQ.z, thisQ.w);

                Curve_X_4_Roa.AddKey(_animDatas_Time_Roa[i], thisRoa.x); 
                Curve_Y_4_Roa.AddKey(_animDatas_Time_Roa[i], thisRoa.y); 
                Curve_Z_4_Roa.AddKey(_animDatas_Time_Roa[i], thisRoa.z); 
                Curve_W_4_Roa.AddKey(_animDatas_Time_Roa[i], thisRoa.w); 
            }
            Curve_X_Pos = new AnimationCurve();
            Curve_Y_Pos = new AnimationCurve();
            Curve_Z_Pos = new AnimationCurve();
            for (int i = 0; i < _animDatas_Pos.Count; i++)
            {
                Curve_X_Pos.AddKey(_animDatas_Time_Pos[i], _animDatas_Pos[i].x); 
                Curve_Y_Pos.AddKey(_animDatas_Time_Pos[i], _animDatas_Pos[i].y);  
                Curve_Z_Pos.AddKey(_animDatas_Time_Pos[i], _animDatas_Pos[i].z);  
            }
        }
       
      
    }
    /// <summary>
    /// One bone animationCurve.
    /// </summary>
    public class BoneOneCurve
    {
        public JAnimationData.BoneType boneType;
         
        public List<Vector4> animDatas_Roa4 = new List<Vector4>();
        public List<Vector3> animDatas_Pos = new List<Vector3>();
        public List<float> animDatas_Time = new List<float>();
         
        public AnimationCurve Curve_X_4_Roa = new AnimationCurve();
        public AnimationCurve Curve_Y_4_Roa = new AnimationCurve();
        public AnimationCurve Curve_Z_4_Roa = new AnimationCurve();
        public AnimationCurve Curve_W_4_Roa = new AnimationCurve();
        public AnimationCurve Curve_X_Pos = new AnimationCurve();
        public AnimationCurve Curve_Y_Pos = new AnimationCurve();
        public AnimationCurve Curve_Z_Pos = new AnimationCurve();


        public BoneOneCurve(JAnimationData.BoneType _type)
        {
            boneType = _type; 
            animDatas_Roa4 = new List<Vector4>();
            animDatas_Pos = new List<Vector3>();
            animDatas_Time = new List<float>();
             
            Curve_X_4_Roa = new AnimationCurve();
            Curve_Y_4_Roa = new AnimationCurve();
            Curve_Z_4_Roa = new AnimationCurve();
            Curve_W_4_Roa = new AnimationCurve();
            Curve_X_Pos = new AnimationCurve();
            Curve_Y_Pos = new AnimationCurve();
            Curve_Z_Pos = new AnimationCurve();

        }

        public Quaternion EvaluateThisFrame_Curve(float timeNow)
        {
            if (Curve_X_4_Roa.keys.Length >= 2 &&
                    Curve_Y_4_Roa.keys.Length >= 2 &&
                    Curve_Z_4_Roa.keys.Length >= 2 &&
                    Curve_W_4_Roa.keys.Length >= 2)
            {
                Quaternion q1 = new Quaternion(
                    Curve_X_4_Roa.Evaluate(timeNow),
                    Curve_Y_4_Roa.Evaluate(timeNow),
                    Curve_Z_4_Roa.Evaluate(timeNow),
                    Curve_W_4_Roa.Evaluate(timeNow));
                return q1;
            }
            else
            {
                Debug.LogWarning("No curve !!!");
                return Quaternion.Euler(Vector3.zero);
            }
        }
      
        public void SetAnimDatas(List<Vector4> _animDatas_Roa4, List<Vector3> _animDatas_Pos
            , List<float> _animDatas_Time_Pos, List<float> _animDatas_Time_Roa)
        { 
            animDatas_Roa4 = _animDatas_Roa4;
            animDatas_Pos = _animDatas_Pos;
            Curve_X_4_Roa = new AnimationCurve();
            Curve_Y_4_Roa = new AnimationCurve();
            Curve_Z_4_Roa = new AnimationCurve();
            Curve_W_4_Roa = new AnimationCurve();
            for (int i = 0; i < _animDatas_Roa4.Count; i++)
            {
                Quaternion thisQ = (new Quaternion(_animDatas_Roa4[i].x, _animDatas_Roa4[i].y, _animDatas_Roa4[i].z, _animDatas_Roa4[i].w));
                Vector4 thisRoa = new Vector4(thisQ.x, thisQ.y, thisQ.z, thisQ.w);

                Curve_X_4_Roa.AddKey(_animDatas_Time_Roa[i], thisRoa.x);
                Curve_Y_4_Roa.AddKey(_animDatas_Time_Roa[i], thisRoa.y);
                Curve_Z_4_Roa.AddKey(_animDatas_Time_Roa[i], thisRoa.z);
                Curve_W_4_Roa.AddKey(_animDatas_Time_Roa[i], thisRoa.w);
            }
            Curve_X_Pos = new AnimationCurve();
            Curve_Y_Pos = new AnimationCurve();
            Curve_Z_Pos = new AnimationCurve();
            for (int i = 0; i < _animDatas_Pos.Count; i++)
            {
                Curve_X_Pos.AddKey(_animDatas_Time_Pos[i], _animDatas_Pos[i].x);
                Curve_Y_Pos.AddKey(_animDatas_Time_Pos[i], _animDatas_Pos[i].y);
                Curve_Z_Pos.AddKey(_animDatas_Time_Pos[i], _animDatas_Pos[i].z);
            }
        }


    }

}
