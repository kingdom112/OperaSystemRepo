using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JVehicleFrameSystem;


 
[CreateAssetMenu]
public class JAnimationData : ScriptableObject
{

    public enum BoneType
    {
        unknowType,
        head,
        neck,
        hips,
        spine,
        chest,
        upperChest,
        leftShoulder,
        rightShoulder,
        leftArm,
        rightArm,
        leftForeArm,
        rightForeArm,
        leftHand,
        rightHand,
        leftUpLeg,
        rightUpLeg,
        leftLeg,
        rightLeg,
        leftFoot,
        rightFoot,
        spine1,
        spine2,
        spine3,
        leftToeBase,
        leftToeBaseEnd,
        rightToeBase,
        rightToeBaseEnd,
        leftForeFoot,
        rightForeFoot,
        neck1,

        //左手
        lThumb1,
        lThumb2,
        lThumb3,
        lIndex1,
        lIndex2,
        lIndex3,
        lMid1,
        lMid2,
        lMid3,
        lRing1,
        lRing2,
        lRing3,
        lPinky1,
        lPinky2,
        lPinky3,

        //右手
        rThumb1,
        rThumb2,
        rThumb3,
        rIndex1,
        rIndex2,
        rIndex3,
        rMid1,
        rMid2,
        rMid3,
        rRing1,
        rRing2,
        rRing3,
        rPinky1,
        rPinky2,
        rPinky3,

        //Daz3D特有的大小臂的肌肉
        muscle_Arm_L,
        muscle_ForeArm_L,
        muscle_Arm_R,
        muscle_ForeArm_R,

        //嘴
        jaw,
        //舌头
        Tongue01,
        Tongue02,
        Tongue03,

        //眼睛
        leftEye,
        rightEye,

        //Daz3D特有的骨盆位置
        addedPelvis,

        //脚趾
        lBigToe,
        lSmallToe1,
        lSmallToe2,
        lSmallToe3,
        lSmallToe4,
        rBigToe,
        rSmallToe1,
        rSmallToe2,
        rSmallToe3,
        rSmallToe4,

        //Daz3D特有的大腿肌肉
        muscle_UpperLeg_L,
        muscle_UpperLeg_R,
        
        // 手掌
        lCarpal1,
        lCarpal2,
        lCarpal3,
        lCarpal4,
        rCarpal1,
        rCarpal2,
        rCarpal3,
        rCarpal4,


        //道具
        ItemRoot,
        ItemPart1,
        ItemPart2,
        ItemPart3,
        ItemPart4,
        ItemPart5,
        ItemPart6,
        ItemPart7,
        ItemPart8,
        ItemPart9
    }

    /// <summary>
    /// 旋转角坐标系类型
    /// </summary>
    public enum RoaSpaceType
    {
        local,
        world
    }

    [System.Serializable]
    public class OneData
    {
        public BoneType boneType = BoneType.unknowType;
        public bool useTans = false;
        public List<Vector4> V4List_roa4;
        public List<float> timeList_roa;
        public List<Vector3> V3List_pos;
        public List<float> timeList_pos;

        public OneData(BoneType _boneType)
        {
            boneType = _boneType;
            useTans = true;
            V4List_roa4 = new List<Vector4>(); 
            timeList_roa = new List<float>();
            V3List_pos = new List<Vector3>(); 
            timeList_pos = new List<float>();
        }
    }

   



    [System.Serializable]
    public class TPoseData
    {
        public List<TPoseOneData> dataList = new List<TPoseOneData>();
        public TPoseData()
        {
            dataList = new List<TPoseOneData>();
        }

        public TPoseOneData GetDataByType(BoneType _boneType)
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                if (dataList[i].boneType == _boneType)
                {
                    Debug.Log(i);
                    return dataList[i];
 
                }
            }
            return null;
        }

        [System.Serializable]
        public class TPoseOneData
        {
            public BoneType boneType = BoneType.unknowType;
            public Quaternion roa_Local = Quaternion.Euler(Vector3.zero);
            public Quaternion roa_World = Quaternion.Euler(Vector3.zero);
            public Vector3 pos_World = Vector3.zero;

            public TPoseOneData()
            {
                boneType = BoneType.unknowType;
                roa_Local = Quaternion.Euler(Vector3.zero);
                roa_World = Quaternion.Euler(Vector3.zero);
                pos_World = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// The serializable class of JAnimData
    /// </summary>
    [System.Serializable]
    public class JAD
    {
        public RoaSpaceType roaSpaceType = RoaSpaceType.local;
        public List<OneData> dataList = new List<OneData>();
        public TPoseData tPoseData;
        public BoneFramework boneFramework;



        public void ClearData()
        {
            dataList.Clear();
        }

        public void AddData_roa4(BoneType _boneType, Vector4 _data, float _time)
        {
            int _index = GetTypeIndex(_boneType);
            if (_index == -1)
            {
                dataList.Add(new OneData(_boneType));
                _index = dataList.Count - 1;
            } 
            dataList[_index].V4List_roa4.Add(_data); 
            dataList[_index].timeList_roa.Add(_time);
        }
        public void AddData_roa4(BoneType _boneType, float _time, Vector4 _data)
        {
            int _index = GetTypeIndex(_boneType);
            if (_index == -1)
            {
                dataList.Add(new OneData(_boneType));
                _index = dataList.Count - 1;
            }
            dataList[_index].useTans = false;
            dataList[_index].V4List_roa4.Add(_data); 
            dataList[_index].timeList_roa.Add(_time);
        }
        public void AddData_pos(BoneType _boneType, Vector3 _data, float _time)
        {
            int _index = GetTypeIndex(_boneType);
            if (_index == -1)
            {
                dataList.Add(new OneData(_boneType));
                _index = dataList.Count - 1;
            }
            dataList[_index].V3List_pos.Add(_data);
            dataList[_index].timeList_pos.Add(_time);
        }
        public void AddData_pos(BoneType _boneType, float _time, Vector3 _data)
        {
            int _index = GetTypeIndex(_boneType);
            if (_index == -1)
            {
                dataList.Add(new OneData(_boneType));
                _index = dataList.Count - 1;
            }
            dataList[_index].useTans = false;
            dataList[_index].V3List_pos.Add(_data); 
            dataList[_index].timeList_pos.Add(_time);
        }
        public int GetTypeIndex(BoneType _boneType)
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                if (dataList[i].boneType == _boneType)
                {
                    return i;
                }
            }
            return -1;
        }

        /* public bool RoaIs3Not4
         {
             get
             {
                 for (int i = 0; i < dataList.Count; i++)
                 {
                     if (dataList[i].V3List_roa3.Count > 0 || dataList[i].V4List_roa4.Count > 0)
                     {
                         if (dataList[i].V3List_roa3.Count > 0 && dataList[i].V4List_roa4.Count == 0)
                         {
                             return true;
                         }
                         else if (dataList[i].V3List_roa3.Count == 0 && dataList[i].V4List_roa4.Count > 0)
                         {
                             return false;
                         }
                         else
                         {
                             Debug.LogError("This Data Can't Judge Roa Type !!!! Please Check !!!");
                         }
                     }
                 }
                 return true;
             }
         }*/

        public float TimeLength
        {
            get
            {
                if (dataList.Count == 0)
                {
                    return 0f;
                }
                else
                {
                    float _time = 0f;
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        if (dataList[i].timeList_pos.Count > 0 || dataList[i].timeList_roa.Count > 0)
                        {
                            if (dataList[i].timeList_pos.Count > 0 && dataList[i].timeList_roa.Count == 0)
                            {
                                if (dataList[i].timeList_pos[dataList[i].timeList_pos.Count - 1] > _time)
                                {
                                    _time = dataList[i].timeList_pos[dataList[i].timeList_pos.Count - 1];
                                }
                            }
                            else if (dataList[i].timeList_pos.Count == 0 && dataList[i].timeList_roa.Count > 0)
                            {
                                if (dataList[i].timeList_roa[dataList[i].timeList_roa.Count - 1] > _time)
                                {
                                    _time = dataList[i].timeList_roa[dataList[i].timeList_roa.Count - 1];
                                }
                            }
                            else
                            {
                                if (dataList[i].timeList_roa[dataList[i].timeList_roa.Count - 1]
                                    > dataList[i].timeList_pos[dataList[i].timeList_pos.Count - 1])
                                {
                                    if (dataList[i].timeList_roa[dataList[i].timeList_roa.Count - 1] > _time)
                                    {
                                        _time = dataList[i].timeList_roa[dataList[i].timeList_roa.Count - 1];
                                    }
                                }
                                else if (dataList[i].timeList_pos[dataList[i].timeList_pos.Count - 1]
                                    > dataList[i].timeList_roa[dataList[i].timeList_roa.Count - 1])
                                {
                                    if (dataList[i].timeList_pos[dataList[i].timeList_pos.Count - 1] > _time)
                                    {
                                        _time = dataList[i].timeList_pos[dataList[i].timeList_pos.Count - 1];
                                    }
                                }
                            }
                        }
                    }
                    return _time;
                }
            }
        }

    }

   
    public JAD jAD = new JAD();

    public List<OneData> dataList
    {
        get
        {
            return jAD.dataList;
        }
        set
        {
            jAD.dataList = value;
        }
    }
    public TPoseData tPoseData
    {
        get
        {
            return jAD.tPoseData;
        }
        set
        {
            jAD.tPoseData = value;
        }
    }
    public BoneFramework boneFramework
    {
        get
        {
            return jAD.boneFramework;
        }
        set
        {
            jAD.boneFramework = value;
        }
    }


}