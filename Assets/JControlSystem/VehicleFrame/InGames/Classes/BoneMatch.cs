using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JVehicleFrameSystem;

namespace JAnimationSystem
{
    [System.Serializable]
    public class BoneMatch
    {
        public List<BoneOneMatch> boneMatchList = new List<BoneOneMatch>();

        public BoneMatch()
        {
            boneMatchList = new List<BoneOneMatch>();
        }
        

        public virtual void ClearBoneMatchList()
        {
            boneMatchList.Clear();
        }

        /// <summary>
        /// 获取从root到tr骨骼的骨骼层级路径
        /// </summary> 
        public string GetRelativePath(Transform root, Transform tr)
        {
            if (tr == root)
            {
                return "";
            }
            else
            {
                List<string> pths = new List<string>();
                Transform t = tr;
                while (t != root && t != t.root)
                {
                    pths.Add(t.name);
                    t = t.parent;
                }
                pths.Reverse();
                return string.Join("/", pths);
            }
        }

        public virtual void StartAutoMatch(Transform _target, JAnimation_AutoSetBoneMatchData _autoMatchData, JAnimationData.JAD _JAnimData = null)//keywords的属性？ target就是骨头吗？
        {
            // bm1.StartAutoMatch(addedga1.transform, vfPlayer.animModelList[num].autodata_model, vfPlayer.ch_master.jad_created);  jad是onedata的datalist boneframkwork timelength
            if (_target == null)
            {
                Debug.LogWarning("theGameObject Can't Be Null !");
                return;
            }
            if (_autoMatchData == null)
            {
                Debug.LogWarning("AutoSetBoneMatchData Can't Be Null !");
                return;
            }
             
            Transform[] _transformList = _target.GetComponentsInChildren<Transform>();
           
            List<Transform> transformList1 = new List<Transform>();
            for (int k = 0; k < _transformList.Length; k++)
            {
                transformList1.Add(_transformList[k]);
                //Debug.Log("transformlist[]" + k + " name:" + _transformList[k].name);//女孩模型一共有185个children
            }
            transformList1.Sort(delegate (Transform x, Transform y)
            {
                return y.name.CompareTo(x.name);
            });//sort 整理出来的是模型本身的子骨骼列表
            ClearBoneMatchList();
            if (_JAnimData != null)
            {
                for (int j = 0; j < _JAnimData.dataList.Count; j++)//武生基本动作 count 24
                {
                    JAnimationData.BoneType _boneType1 = _JAnimData.dataList[j].boneType;
                    List<string> keyWords = _autoMatchData.GetKeyWords(_boneType1);//autoMatchData是系统内置含bonetype的数据 model的 女孩的数量为89个 但是transformlist个数是185个 寻找bonetype相同的骨骼 keywords是bone名字列表
                 

                    AddNewMatch_BoneType(_boneType1);//添加了骨骼的bonetype信息进行比对 24个
                    int count = 0;
                    for (int i = 0; i < transformList1.Count; i++)
                    {
                        if (_autoMatchData.CanTextMatchOneKeyWord(transformList1[i].name, keyWords))//canMatchOneKeyWord  如果transform的名字跟数据中的keywords匹配 一个匹配就行
                        {
                            count++;
                            boneMatchList[boneMatchList.Count - 1].SetMatchTransform(transformList1[i]);//更改unity检视面板的name是生效的
                           
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < _autoMatchData.DataList.Count; j++)
                {
                    JAnimationData.BoneType _boneType1 = _autoMatchData.DataList[j].boneType; //automatchdata 是存储的骨骼信息制式文件
                    List<string> keyWords = _autoMatchData.GetKeyWords(_boneType1);
                    AddNewMatch_BoneType(_boneType1);
                    for (int i = 0; i < transformList1.Count; i++)
                    {
                        if (_autoMatchData.CanTextMatchOneKeyWord(transformList1[i].name, keyWords))//canMatchOneKeyWord
                        {
                            boneMatchList[boneMatchList.Count - 1].SetMatchTransform(transformList1[i]);
                        }
                    }
                }
            }
           
        }

        /// <summary>
        /// Add an new match and return the lastest number.
        /// If the type has added in list, return the number and not add a new.
        /// </summary>
        public virtual int AddNewMatch_BoneType(JAnimationData.BoneType _bonetype1)
        {
            int _indexOld = GetIndexOfBoneType(_bonetype1);
            if (_indexOld != -1) return _indexOld;
            boneMatchList.Add(new BoneOneMatch(_bonetype1));
            return boneMatchList.Count - 1;
        }

        /// <summary>
        /// return the index of a boneType in the boneMatchList.
        /// If not found, return -1
        /// </summary>
        public int GetIndexOfBoneType(JAnimationData.BoneType _boneType)
        {
            
            for (int i = 0; i < boneMatchList.Count; i++)
            {
                if (boneMatchList[i].boneType == _boneType)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        ///  Get the boneType by a matched T
        /// </summary> 
        public JAnimationData.BoneType GetBoneType_ByT(Transform _t)
        {
            int index = GetIndexOfMatchedT(_t);
            if (index != -1)
            {
                return boneMatchList[index].boneType;
            }
            return JAnimationData.BoneType.unknowType;
        }

        /// <summary>
        ///  Get the matched T by boneType
        /// </summary>
        /// <param name="_boneType"></param>
        /// <returns></returns>
        public Transform GetT_ByBoneType(JAnimationData.BoneType _boneType)
        {
            int index = GetIndexOfBoneType(_boneType);
            if (index != -1)
            {
                return boneMatchList[index].matchedT;
            }
            return null;
        }

        /// <summary>
        /// return the index of a Transform in the boneMatchList.
        /// If not found, return -1
        /// </summary>
        public int GetIndexOfMatchedT(Transform theT)
        {
            for (int i = 0; i < boneMatchList.Count; i++)
            {
                if (boneMatchList[i].matchedT == theT)
                {
                    return i;
                }
            }
            return -1;
        }

       
        public virtual BoneTPoseMarker GetTPoseMarker(Transform t1)
        {
            if (t1 == null) return null;
            BoneTPoseMarker marker1 = t1.root.gameObject.GetComponentInChildren<BoneTPoseMarker>();
            return marker1;
        }
    }

   

    public class BoneMatch_Preview : BoneMatch
    {
        public List<BoneOneMatchedNames> boneMatchNamesList = new List<BoneOneMatchedNames>();
        public List<BoneOnePlayer> bonePlayers = new List<BoneOnePlayer>();

        /// <summary>
        /// 旋转空间坐标系类型
        /// </summary>
        public JAnimationData.RoaSpaceType roaSpaceType = JAnimationData.RoaSpaceType.local;

        public override void ClearBoneMatchList()
        {
            boneMatchList.Clear();
            boneMatchNamesList.Clear();
            bonePlayers.Clear();
        }

        public override void StartAutoMatch(Transform _target, JAnimation_AutoSetBoneMatchData _autoMatchData, JAnimationData.JAD _JAnimData = null)
        {
            if (_target == null)
            {
                Debug.LogWarning("theGameObject Can't Be Null !");
                return;
            }
            if (_autoMatchData == null)
            {
                Debug.LogWarning("AutoSetBoneMatchData Can't Be Null !");
                return;
            }
           
            Transform[] _transformList = _target.GetComponentsInChildren<Transform>();
            List<Transform> transformList1 = new List<Transform>();
            for (int k = 0; k < _transformList.Length; k++)
            {
                transformList1.Add(_transformList[k]);
            }
            transformList1.Sort(delegate (Transform x, Transform y)
            {
                return y.name.CompareTo(x.name);
            });//sort
            ClearBoneMatchList();
            if (_JAnimData != null)
            {
                for (int j = 0; j < _JAnimData.dataList.Count; j++)
                {
                    JAnimationData.BoneType _boneType1 = _JAnimData.dataList[j].boneType;
                    List<string> keyWords = _autoMatchData.GetKeyWords(_boneType1);
                    AddNewMatch_BoneType(_boneType1, keyWords);
                    for (int i = 0; i < transformList1.Count; i++)
                    {
                        if (_autoMatchData.CanTextMatchOneKeyWord(transformList1[i].name, keyWords))//canMatchOneKeyWord
                        {
                            boneMatchList[boneMatchList.Count - 1].SetMatchTransform(transformList1[i]);
                            //Debug.Log("boneMatchList[" + (boneMatchList.Count - 1) + "](" + boneMatchList[boneMatchList.Count - 1].boneType + ") SetMatch:" + transformList1[i].name);
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < _autoMatchData.DataList.Count; j++)
                {
                    JAnimationData.BoneType _boneType1 = _autoMatchData.DataList[j].boneType;
                    List<string> keyWords = _autoMatchData.GetKeyWords(_boneType1);
                    AddNewMatch_BoneType(_boneType1, keyWords);
                    for (int i = 0; i < transformList1.Count; i++)
                    {
                        bool canUseThisBone = true;
                        for (int j1=0; j1< boneMatchList.Count -2; j1++)
                        {
                            if(boneMatchList[j1].matchedT == transformList1[i])
                            {
                                canUseThisBone = false;
                                break;
                            }
                        }
                        if(canUseThisBone == false)
                        {
                            continue;//跳出这个循环
                        }

                        bool nameEquOneKey = false; //判断是不是物体名字完全等于某个关键词，如果是，不进行接下来的匹配直接锁死
                        for (int j1=0; j1< keyWords.Count; j1++)
                        {
                            if(keyWords[j1] == transformList1[i].name)
                            {
                                nameEquOneKey = true;
                                break;
                            }
                        }
                        if(nameEquOneKey == true)
                        {
                            boneMatchList[boneMatchList.Count - 1].SetMatchTransform(transformList1[i]);
                            Debug.Log("骨骼[<color=blue>" + transformList1[i] + "</color>]的名字完全和<color=green>" + _boneType1 + "</color>类型的关键词重合，锁定，不进行接下来的匹配。");
                            break;
                        }
                        else
                        {
                            if (_autoMatchData.CanTextMatchOneKeyWord(transformList1[i].name, keyWords))//canMatchOneKeyWord
                            {
                                boneMatchList[boneMatchList.Count - 1].SetMatchTransform(transformList1[i]);
                                //Debug.Log("boneMatchList[" + (boneMatchList.Count - 1) + "](" + boneMatchList[boneMatchList.Count - 1].boneType + ") SetMatch:" + transformList1[i].name);
                            }
                        } 
                    }
                }
            }
           

        }

        public BoneMatch_Preview()
        {
            boneMatchNamesList = new List<BoneOneMatchedNames>();
            bonePlayers = new List<BoneOnePlayer>();
        }


        public void SetJAnimationData(JAnimationData _data, bool useBoneAdapt = false , bool warning = true)
        {
            SetJAnimationData(_data.jAD, useBoneAdapt, warning);
        }
        public void SetJAnimationData(JAnimationData.JAD _data,bool useBoneAdapt = false, bool warning = true)
        {

            if (_data == null)
            {
                Debug.LogError("JAnimationData == null !!!");
                return;
            }
            if (bonePlayers.Count == 0)
            {
                Debug.LogError("bonePlayers.Count == 0 !!!");
                return;

            }

            roaSpaceType = _data.roaSpaceType;

            if (useBoneAdapt)
            {
                Transform root1 = bonePlayers[0].boneOneMatch.matchedT.root;
                JAnimation_AutoSetBoneMatchData autoMatchData =
                    Resources.Load<JAnimation_AutoSetBoneMatchData>("AutoSetBoneMatchDatas/Auto Set Bone Match Data_TypeName");
                BoneFramework boneFramework1 = new BoneFramework();
                BoneFramework.ReadBoneFrame(root1.gameObject, autoMatchData,out boneFramework1, 1f);
                Debug.Log("ReadBoneFrame");

                BoneTPoseMarker marker1 = null;
                for (int k3=0; k3< bonePlayers.Count; k3++)
                {
                    if(bonePlayers[k3].boneOneMatch != null)
                    {
                        if(bonePlayers[k3].boneOneMatch.matchedT != null)
                        {
                            marker1 = GetTPoseMarker(bonePlayers[k3].boneOneMatch.matchedT);
                            break;
                        }
                    }
                }
                if(marker1 == null)
                {
                    Debug.LogWarning("no marker!!!");

                }
                else
                {
                    

                    List<BoneOneCurve> boneCurves_pre = new List<BoneOneCurve>();
                    for (int i = 0; i < _data.dataList.Count; i++)
                    {
                        boneCurves_pre.Add(new BoneOneCurve(_data.dataList[i].boneType));
                        //Debug.Log(i.ToString()+" add " + _data.dataList[i].boneType);
                    }

                    //-----预处理一次 Preprocessing
                    for (int i = 0; i < _data.dataList.Count; i++)
                    {
                        int _index = -1;
                        for (int j1 = 0; j1 < boneCurves_pre.Count; j1++)
                        {
                            if (boneCurves_pre[j1].boneType == _data.dataList[i].boneType)
                            {
                                _index = j1;
                                break;
                            }
                        }
                        if (_index != -1)
                        {
                            boneCurves_pre[_index].SetAnimDatas(_data.dataList[i].V4List_roa4,
                               _data.dataList[i].V3List_pos, _data.dataList[i].timeList_pos, _data.dataList[i].timeList_roa);

                        }
                        else
                        {

                            Debug.LogWarning("One Bone is Not Match To This GameObejct ! ");
                        }
                    }

                    //开始动画的骨骼匹配 Start the adapt of the animation.
                    for (int i = 0; i < _data.dataList.Count; i++)
                    {
                        List<JAnimationData.BoneType> bPath = _data.boneFramework.GetBonePath(_data.dataList[i].boneType);
                        List<Vector4> newRoaList = new List<Vector4>();
                        /*if (_data.dataList[i].boneType == JAnimationData.BoneType.leftForeArm)
                        {
                            for(int j11 = 0; j11< bPath.Count; j11++)
                            {
                                Debug.Log(""+j11+"  " + bPath[j11]);
                            }
                        }*/
                        if (marker1.tPoseData.GetDataByType(_data.dataList[i].boneType) == null)
                        {
                            Debug.LogWarning("" + marker1.gameObject.name + " don't have [" + _data.dataList[i].boneType + "]");
                            continue;
                        }
                        for (int j = 0; j < _data.dataList[i].V4List_roa4.Count; j++)
                        { 
                            Quaternion roa_world = Quaternion.Euler(Vector3.zero);
                            Quaternion roa_world_parent = Quaternion.Euler(Vector3.zero); 
                            for (int k = 0; k < bPath.Count; k++)
                            {
                                bool found = false;
                                for (int k1 = 0; k1 < boneCurves_pre.Count; k1++)
                                {
                                    if (boneCurves_pre[k1].boneType == bPath[k])
                                    {
                                        roa_world *= boneCurves_pre[k1].
                                            EvaluateThisFrame_Curve(_data.dataList[i].timeList_roa[j]);
                                        if (k < bPath.Count - 1)
                                        {
                                            roa_world_parent *= boneCurves_pre[k1].
                                            EvaluateThisFrame_Curve(_data.dataList[i].timeList_roa[j]);
                                        }
                                        found = true;
                                        break;
                                    }
                                }
                                if (found == false)
                                {
                                    roa_world *=
                                        Quaternion.Euler(_data.boneFramework.GetBoneDataByType(_data.dataList[i].boneType).localRotation);
                                    if (k < bPath.Count - 1)
                                    {
                                        roa_world_parent *= Quaternion.Euler(_data.boneFramework.GetBoneDataByType(_data.dataList[i].boneType).localRotation);
                                    }
                                    //Debug.LogWarning("bonePlayers[" + bPath[k] + "] Not Found !!!");
                                }
                            }//获取了世界旋转


                            /*if (_data.dataList[i].boneType == JAnimationData.BoneType.leftForeArm)
                            {
                                Debug.Log(roa_world+"  "+ roa_world_parent);
                            }*/

                             
                            //--获取相对旋转
                            Quaternion offsetQ = Quaternion.Euler(Vector3.zero);
                            offsetQ =
                                    Quaternion.Inverse(_data.tPoseData.GetDataByType(
                                        _data.dataList[i].boneType).roa_World) *
                                        marker1.tPoseData.GetDataByType(_data.dataList[i].boneType).roa_World;
                            roa_world *= offsetQ;

                            if (bPath.Count > 1)
                            { 
                                if(marker1.tPoseData.GetDataByType(bPath[bPath.Count - 2]) == null)
                                {
                                    Debug.Log("" + marker1.gameObject.name + "  don't have parent [" + bPath[bPath.Count - 2] + "]");
                                    JAnimationData.BoneType parentType = bPath[bPath.Count - 2];
                                    for (int k13= bPath.Count - 2; k13 >=0; k13--)
                                    {
                                        if(marker1.tPoseData.GetDataByType(bPath[k13]) != null)
                                        {
                                            parentType = bPath[k13];
                                            break;
                                        }
                                    }
                                    if(parentType == bPath[bPath.Count - 2])
                                    {
                                        Debug.LogError("" + bPath[bPath.Count - 1] + " can't have a parent in this model!(" + marker1.gameObject.name + ")");

                                    }
                                    else
                                    {
                                        Quaternion offsetParent =
                                            Quaternion.Inverse(_data.tPoseData.GetDataByType(
                                                parentType).roa_World) *
                                                marker1.tPoseData.GetDataByType(parentType).roa_World;

                                        int index_b_Parent = -1;
                                        for (int k11 = bPath.Count - 2; k11 >= 0; k11--)
                                        {
                                            if (bPath[k11] == parentType)
                                            {
                                                index_b_Parent = k11;
                                                break;
                                            }
                                        }
                                        if (index_b_Parent == -1)
                                        {
                                            Debug.LogError("Can't find a bone of [" + parentType + "]!!(b)");
                                        }
                                        else
                                        {
                                            List<Quaternion> QPath_b = _data.boneFramework.GetBonePath_LocalRoa(_data.dataList[i].boneType);
                                            for (int k12 = QPath_b.Count - 2; k12 > index_b_Parent; k12--)
                                            {
                                                roa_world_parent *= Quaternion.Inverse(QPath_b[k12]);
                                            }
                                            roa_world_parent *= offsetParent;
                                            Quaternion finalLocalQ =
                                                Quaternion.Inverse(roa_world_parent) * roa_world;
                                            newRoaList.Add(new Vector4(finalLocalQ.x, finalLocalQ.y, finalLocalQ.z, finalLocalQ.w));
                                            Debug.Log("进行了不足层级的变换 (" + _data.dataList[i].boneType + "  其修正后的parent是 "+ parentType + ")");
                                        }
                                    }


                                }
                                else
                                {
                                    Quaternion offsetParent =
                                    Quaternion.Inverse(_data.tPoseData.GetDataByType(
                                        bPath[bPath.Count - 2]).roa_World) *
                                        marker1.tPoseData.GetDataByType(bPath[bPath.Count - 2]).roa_World;
                                    roa_world_parent *= offsetParent;

                                    Quaternion finalLocalQ =
                                        Quaternion.Inverse(roa_world_parent) * roa_world;

                                    List<JAnimationData.BoneType> mPath =
                                        marker1.boneFramework.GetBonePath(_data.dataList[i].boneType);

                                    if (bPath.Count >= 2 && mPath.Count >= 2)
                                    {
                                        if (mPath[mPath.Count - 2] != bPath[bPath.Count - 2])
                                        //如果模型上对应骨骼的父级有多余的层级
                                        {
                                            JAnimationData.BoneType bParentBT = bPath[bPath.Count - 1];
                                            for (int k11 = bPath.Count - 2; k11 >= 0; k11--)
                                            {
                                                if (bPath[k11] != JAnimationData.BoneType.unknowType)
                                                {
                                                    bParentBT = bPath[k11];
                                                    break;
                                                }
                                            }
                                            if (bParentBT == bPath[bPath.Count - 1])
                                            {
                                                Debug.LogError("Can't find a parent boneType!!(b)");
                                            }

                                            int index_m_Parent = -1;
                                            for (int k11 = mPath.Count - 2; k11 >= 0; k11--)
                                            {
                                                if (mPath[k11] == bParentBT)
                                                {
                                                    index_m_Parent = k11;
                                                    break;
                                                }
                                            }
                                            if (index_m_Parent == -1)
                                            {
                                                Debug.LogError("Can't find a bone of [" + bParentBT + "]!!(m)");
                                                /*
                                                JAnimationData.BoneType mParentBT = mPath[mPath.Count - 1];
                                                for (int k11 = mPath.Count - 2; k11 >= 0; k11--)
                                                {
                                                    if (mPath[k11] != JAnimationData.BoneType.unknowType)
                                                    {
                                                        mParentBT = mPath[k11];
                                                        break;
                                                    }
                                                }
                                                if (mParentBT == mPath[bPath.Count - 1])
                                                {
                                                    Debug.LogError("Can't find a parent boneType!!(m)");
                                                }
                                                else
                                                {
                                                    int index_b_Parent = -1;
                                                    for (int k11 = bPath.Count - 2; k11 >= 0; k11--)
                                                    {
                                                        if (bPath[k11] == mParentBT)
                                                        {
                                                            index_b_Parent = k11;
                                                            break;
                                                        }
                                                    }
                                                    if (index_b_Parent == -1)
                                                    {
                                                        Debug.LogError("Can't find a bone of [" + mParentBT + "]!!(b)");
                                                    }
                                                    else
                                                    {
                                                        List<Quaternion> QPath_b = _data.boneFramework.GetBonePath_LocalRoa(_data.dataList[i].boneType);
                                                        for (int k12 = QPath_b.Count - 2; k12 > index_b_Parent; k12--)
                                                        {
                                                            roa_world_parent *= Quaternion.Inverse(QPath_b[k12]);
                                                        }
                                                        finalLocalQ =
                                                            Quaternion.Inverse(roa_world_parent) * roa_world;
                                                        newRoaList.Add(new Vector4(finalLocalQ.x, finalLocalQ.y, finalLocalQ.z, finalLocalQ.w));
                                                        Debug.Log("进行了不足层级的变换 (" + _data.dataList[i].boneType + ")");
                                                    } 
                                                }*/

                                            }
                                            else
                                            {
                                                List<Quaternion> QPath_m = marker1.boneFramework.GetBonePath_LocalRoa(_data.dataList[i].boneType);

                                                for (int k12 = index_m_Parent; k12 < QPath_m.Count - 1; k12++)
                                                {
                                                    roa_world_parent *= QPath_m[k12];
                                                }
                                                finalLocalQ =
                                                    Quaternion.Inverse(roa_world_parent) * roa_world;
                                                newRoaList.Add(new Vector4(finalLocalQ.x, finalLocalQ.y, finalLocalQ.z, finalLocalQ.w));
                                                Debug.Log("进行了多余层级的变换 (" + _data.dataList[i].boneType + ")");
                                            }

                                        }
                                        else
                                        {
                                            newRoaList.Add(new Vector4(finalLocalQ.x, finalLocalQ.y, finalLocalQ.z, finalLocalQ.w));
                                        }
                                    }
                                }
                                 
                            }
                            else
                            {

                                newRoaList.Add(new Vector4(roa_world.x, roa_world.y, roa_world.z, roa_world.w));
                            }


                        }
                        /*if (_data.dataList[i].boneType == JAnimationData.BoneType.leftForeArm)
                        {
                            for (int j11 = 0; j11 < newRoaList.Count; j11++)
                            {
                                Debug.Log("" + j11 + "  " + newRoaList[j11]);
                            }
                        }*/

                        int _index = GetIndexOfBoneType(_data.dataList[i].boneType);
                        if (_index != -1)
                        {
                            //Debug.Log("bonePlayers[" + bonePlayers[_index].boneOneMatch.boneType + "] " + _data2.dataList[i].V4List_roa4.Count);
                            bonePlayers[_index].SetAnimDatas(roaSpaceType, newRoaList,
                          _data.dataList[i].V3List_pos, _data.dataList[i].timeList_pos, _data.dataList[i].timeList_roa);
                        }
                        else
                        {
                            Debug.LogWarning("One Bone is Not Match To This GameObejct ! ");
                        }


                    }

                }





            }
            else
            {
                //no adapt
                if(roaSpaceType == JAnimationData.RoaSpaceType.local)
                {
                    for (int i = 0; i < _data.dataList.Count; i++)
                    {
                        int _index = GetIndexOfBoneType(_data.dataList[i].boneType);
                        if (_index != -1)
                        {
                            bonePlayers[_index].SetAnimDatas(roaSpaceType,
                                _data.dataList[i].V4List_roa4, 
                             _data.dataList[i].V3List_pos
                             , _data.dataList[i].timeList_pos, _data.dataList[i].timeList_roa);

                        }
                        else
                        {
                            Debug.LogWarning("One Bone is Not Match To This GameObejct ! ");
                        }
                    }
                }
                else if(roaSpaceType == JAnimationData.RoaSpaceType.world)
                {
                    if(_data.tPoseData != null && _data.tPoseData.dataList.Count > 0 && GetTPoseMarker(bonePlayers[0].boneOneMatch.matchedT) != null)
                    {
                        Debug.Log("roa-world-space  adapt !!");
                        JAnimationData.TPoseData targetTPose = GetTPoseMarker(bonePlayers[0].boneOneMatch.matchedT).tPoseData;
                        JAnimationData.TPoseData animTPose = _data.tPoseData;
                        for (int i=0; i< animTPose.dataList.Count; i++)
                        {
                            JAnimationData.TPoseData.TPoseOneData tPoseData1 = targetTPose.GetDataByType(animTPose.dataList[i].boneType);
                            if (tPoseData1 == null) continue;
                            Quaternion q1 = Quaternion.Inverse(animTPose.dataList[i].roa_World) * tPoseData1.roa_World;

                            int _index = GetIndexOfBoneType(animTPose.dataList[i].boneType);
                            int j = _data.GetTypeIndex(animTPose.dataList[i].boneType);
                            if (_index != -1 && j != -1)
                            {
                                bonePlayers[_index].SetAnimDatas(roaSpaceType,
                                   _data.dataList[i].V4List_roa4, 
                                 _data.dataList[i].V3List_pos
                                 , _data.dataList[j].timeList_pos, _data.dataList[j].timeList_roa, q1.eulerAngles);
                                Debug.Log("set!!");
                            }
                            else
                            {
                                Debug.LogWarning("_index == -1 || j == -1");
                            }
                        }
                    }
                    else
                    {
                        if(GetTPoseMarker(bonePlayers[0].boneOneMatch.matchedT) == null)
                        {
                            Debug.LogError("GetTPoseMarker(bonePlayers[0].boneOneMatch.matchedT) == null");
                        }

                        for (int i = 0; i < _data.dataList.Count; i++)
                        {
                            int _index = GetIndexOfBoneType(_data.dataList[i].boneType);
                            if (_index != -1)
                            {
                                bonePlayers[_index].SetAnimDatas(roaSpaceType,
                                _data.dataList[i].V4List_roa4, 
                             _data.dataList[i].V3List_pos
                             , _data.dataList[i].timeList_pos, _data.dataList[i].timeList_roa);

                            }
                            else
                            {
                                Debug.LogWarning("One Bone is Not Match To This GameObejct ! ");
                            }
                        }
                    }
                }
            }




             
        }

        public void PlayDataInThisFrame_Curve(float _timeNow)
        {
            for (int i = 0; i < bonePlayers.Count; i++)
            {
                bonePlayers[i].PlayInThisFrame_Curve(_timeNow);
            }
        }


        

        public void StartMatch(Transform _target)
        {
            Transform[] transformList = _target.GetComponentsInChildren<Transform>();
            for (int i = 0; i < transformList.Length; i++)
            {
                int _matchedIndex = NameMatchOneBone(transformList[i].name);
                if (_matchedIndex == -1) continue;
                boneMatchList[_matchedIndex].SetMatchTransform(transformList[i]);
            }
        }
        
        /// <summary>
        /// Add an new match.
        /// </summary>
        public int AddNewMatch_BoneType(JAnimationData.BoneType _bonetype1, List<string> matchNames = null)
        {
            int _indexOld = GetIndexOfBoneType(_bonetype1);
            if (_indexOld != -1)
            {
                if (matchNames != null)
                    boneMatchNamesList[_indexOld].AddMatchName(matchNames);
                return _indexOld;
            }
            boneMatchList.Add(new BoneOneMatch(_bonetype1));
            int _index = boneMatchList.Count - 1;
            if(matchNames != null)
            {
                boneMatchNamesList.Add(
               new BoneOneMatchedNames(boneMatchList[_index], matchNames));
            }
            else
            {
                boneMatchNamesList.Add(
               new BoneOneMatchedNames(boneMatchList[_index], "DefaultMatchName"));
            } 
            bonePlayers.Add(new BoneOnePlayer(boneMatchList[_index]));
            return _index;
        }
        /// <summary>
        /// Add an new name match.
        /// </summary>
        public void AddNameMatch_BoneType(JAnimationData.BoneType _boneType, string _matchName)
        {
            int _index = GetIndexOfBoneType(_boneType);
            if (_index == -1)
            {
                boneMatchList.Add(new BoneOneMatch(_boneType));
                _index = boneMatchList.Count - 1;
                bonePlayers.Add(new BoneOnePlayer(boneMatchList[_index]));
                boneMatchNamesList.Add(new BoneOneMatchedNames(boneMatchList[_index], _matchName));
            }
            boneMatchNamesList[_index].AddMatchName(_matchName);
        }

        /// <summary>
        /// Return the matched index of bone in the boneMatchList. If the bones can't match the name , return -1.
        /// </summary>
        private int NameMatchOneBone(string _name)
        {
            for (int i = 0; i < boneMatchList.Count; i++)
            {
                for (int j = 0; j < boneMatchNamesList[i].MatchNames.Count; j++)
                {
                    if (_name.Contains(boneMatchNamesList[i].MatchNames[j]) == true)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

    }



    /// <summary>
    /// 对于BoneMatch的播放器类。
    /// </summary>
    public class BoneMatch_Player
    {
        public BoneMatch boneMatch;
        public List<BoneOnePlayer> bonePlayers = new List<BoneOnePlayer>();
        /// <summary>
        /// 旋转空间坐标系类型
        /// </summary>
        public JAnimationData.RoaSpaceType roaSpaceType = JAnimationData.RoaSpaceType.local;


        public BoneMatch_Player(BoneMatch _boneMatch)
        {
            boneMatch = _boneMatch;
            Init();
        }

        public void ClearBonePlayersList()
        {
            bonePlayers.Clear();
        }

        public void Init()
        {
            ClearBonePlayersList();

            for (int i = 0; i < boneMatch.boneMatchList.Count; i++)
            {
                bonePlayers.Add(new BoneOnePlayer(boneMatch.boneMatchList[i]));
            }
        }

        /// <summary>
        /// 获取Root物体
        /// </summary>
        /// <returns></returns>
        public Transform GetRootTransform()
        {
            int _index = GetPlayerIndexByBoneType(JAnimationData.BoneType.hips);
            if(_index != -1)
            {
                return bonePlayers[_index].boneOneMatch.matchedT.root;
            }
            return null;
        }

        /// <summary>
        /// return the index in bonePlayers by BoneType
        /// </summary> 
        public int GetPlayerIndexByBoneType(JAnimationData.BoneType _boneType)
        {
            for(int i=0; i<bonePlayers.Count; i++)
            {
                if(bonePlayers[i].boneOneMatch.matchedT != null && bonePlayers[i].boneOneMatch.boneType == _boneType)
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// return the index in bonePlayers by BoneType
        /// </summary> 
        public int GetPlayerIndexByMatchedT(Transform t1)
        {
            for (int i = 0; i < bonePlayers.Count; i++)
            {
                if (bonePlayers[i].boneOneMatch.matchedT != null && bonePlayers[i].boneOneMatch.matchedT == t1)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Get a world Roa of bone of [_boneType]. 
        /// 
        /// </summary> 
        public Vector3 GetWorldPosByTime(float _time, JAnimationData.BoneType _boneType)
        {
            if(roaSpaceType == JAnimationData.RoaSpaceType.world)
            {
               Debug.LogError("该函数在当前旋转坐标系类型不可用！");
               return Vector3.zero;
            }
            List<Transform> path = new List<Transform>();
            List<Quaternion> localRoaPath = new List<Quaternion>(); 
            Transform hipT = null;
            int number = boneMatch.GetIndexOfBoneType(JAnimationData.BoneType.hips);
            if (number != -1)
            {
                hipT = boneMatch.boneMatchList[number].matchedT;
                number = boneMatch.GetIndexOfBoneType(_boneType);
                if (number != -1)
                {  
                    Transform tnow = bonePlayers[number].boneOneMatch.matchedT; //目标骨骼的transform
                    path.Add(tnow);
                    int test=0;
                    
                    localRoaPath.Add(bonePlayers[number].EvaluateThisFrame_Curve(_time));//目标骨骼当前时间的旋转信息
                    for (int i = 0; i < 101 && tnow.parent != tnow && tnow.parent != hipT; i++)
                    {
                        tnow = tnow.parent;
                        path.Add(tnow);
                        test++;

                        number = boneMatch.GetIndexOfMatchedT(tnow);
                        if (number != -1)
                        {
                            localRoaPath.Add(bonePlayers[number].EvaluateThisFrame_Curve(_time)); 
                        }
                        else
                        {
                            localRoaPath.Add(tnow.localRotation);
                        }
                        if (i == 100)
                        {
                            Debug.LogWarning("i get 100 !!!! not normal !!! the bone is too long or program has error !!!!");
                        }
                    }
                    path.Add(hipT); 
                    path.Reverse();
                    for(int i = 0; i < test; i++)
                    {
                        Debug.Log("path" + i + ":" + path[i]);
                    }
                    
                   
                    localRoaPath.Add(hipT.rotation);
                    localRoaPath.Reverse();
                    /*string st1 = "";
                    st1 += "" + path.Count + " " + localRoaPath.Count;
                    for (int i = 0; i < path.Count; i++)
                    {
                        st1 += ", " + path[i].name;
                    }
                    Debug.Log("  " + st1);*/
                    Vector3 pos1 = Vector3.zero;
                    Quaternion roa1 = Quaternion.Euler(0, 0, 0);
                    pos1 = path[0].position;
                    roa1 *= localRoaPath[0];
                    for(int i=1; i< path.Count; i++)
                    {
                        int index = GetPlayerIndexByMatchedT(path[i]);
              
                       
                    pos1 = pos1 + (roa1 * Vector3.forward * path[i].localPosition.z) +
                            (roa1 * Vector3.right * path[i].localPosition.x) +
                            (roa1 * Vector3.up * path[i].localPosition.y);
                        roa1 *= localRoaPath[i];
                    }


                    return pos1;
                }
                else
                {
                    Debug.LogWarning("not found type[" + _boneType + "].");
                }
            }
            else
            {
                Debug.LogWarning("not found type[" + "hips" + "].");
            }
            return Vector3.zero;
        }
        //public Vector3 GetWorldPosByTime_Ztest(float _time, JAnimationData.BoneType _boneType, BoneFramework boneframework, Vector3 created_pos)
        //{//计算思路是 先计算选定骨骼应该在的位置 再进行误差消除
        //    //如果是hip节点 直接返回消除空物体的误差的值
        //    if (_boneType == JAnimationData.BoneType.hips)
        //    {
        //        int hip_number = boneMatch.GetIndexOfBoneType(JAnimationData.BoneType.hips);
        //        Vector3 hip_pos = bonePlayers[hip_number].EvaluateThisFramePos_Curve(_time);
        //        hip_pos = created_pos +  hip_pos;//消除旋转误差
        //        //Debug.Log("hip pos:" + hip_pos);
        //        //Vector3 correct_pos = new Vector3(hip_pos.x + created_pos.x, hip_pos.y + created_pos.y, hip_pos.z + created_pos.z);
        //        return hip_pos;
        //    }

        //    List<Transform> path = new List<Transform>();
        //    List<JAnimationData.BoneType> zpath = new List<JAnimationData.BoneType>();//骨骼对应bonetype的list
        //    List<Quaternion> localRoaPath = new List<Quaternion>();

        //    Transform hipT = null;
        //    int number = boneMatch.GetIndexOfBoneType(JAnimationData.BoneType.hips);
        //    hipT = boneMatch.boneMatchList[number].matchedT;
        //    number = boneMatch.GetIndexOfBoneType(_boneType);
        //    if (number != -1)
        //    {
        //        if (number != -1)
        //        {
        //            JAnimationData.BoneType ztnow = bonePlayers[number].boneOneMatch.boneType;
        //            Transform tnow = bonePlayers[number].boneOneMatch.matchedT; //目标骨骼的transform
        //            path.Add(tnow);
        //            zpath.Add(ztnow);

        //            localRoaPath.Add(bonePlayers[number].EvaluateThisFrame_Curve(_time));//目标骨骼当前时间的旋转信息
        //            for (int i = 0; i < 101 && tnow.parent != tnow && tnow.parent != hipT; i++)
        //            {
        //                tnow = tnow.parent;
        //                path.Add(tnow);

        //                number = boneMatch.GetIndexOfMatchedT(tnow);
        //                if (number != -1)
        //                {
        //                    zpath.Add(bonePlayers[number].boneOneMatch.boneType);
        //                    localRoaPath.Add(bonePlayers[number].EvaluateThisFrame_Curve(_time));
        //                }
        //                else
        //                {

        //                    localRoaPath.Add(tnow.localRotation);
        //                }
        //                if (i == 100)
        //                {
        //                    Debug.LogWarning("i get 100 !!!! not normal !!! the bone is too long or program has error !!!!");
        //                }
        //            }
        //            int znumber = boneMatch.GetIndexOfBoneType(JAnimationData.BoneType.hips);
        //            zpath.Add(bonePlayers[znumber].boneOneMatch.boneType);
        //            zpath.Reverse();//翻转队列 以便从hip骨骼开始计算位置数据
        //            path.Add(hipT);
        //            path.Reverse();

        //            Quaternion zrotation = bonePlayers[znumber].EvaluateThisFrame_Curve(_time);
        //            localRoaPath.Add(zrotation);
        //            localRoaPath.Reverse();

        //            Quaternion roa1 = Quaternion.Euler(0, 0, 0);
        //            Vector3 pos1 = bonePlayers[znumber].EvaluateThisFramePos_Curve(_time);//获取当前时间hip骨骼的位置 只有hip骨骼能获取
        //            pos1 = created_pos + pos1;
        //            roa1 *= localRoaPath[0];
        //            for (int i = 1; i < zpath.Count; i++)
        //            {
        //                JAnimationData.BoneType zbonetype = zpath[i];
        //                int j = 0;
        //                for (j = 0; j < boneframework.Bones.Count; j++)
        //                {
        //                    if (zbonetype == boneframework.Bones[j].boneType)
        //                        break;
        //                }
        //                Vector3 temp = boneframework.Bones[j].localPos;//获取目标骨骼相对父级骨骼的localpos

        //                Vector3 offset = new Vector3(temp.x * path[i - 1].transform.localScale.x, temp.y * path[i - 1].transform.localScale.y, temp.z * path[i - 1].transform.localScale.z);
        //                //pos1 = pos1 + (roa1 * Vector3.forward * temp.z) + (roa1 * Vector3.right * temp.x) + (roa1 * Vector3.up * temp.y);
        //                pos1 = pos1 + roa1 * offset;
        //                roa1 = localRoaPath[i];
        //            }

        //            return pos1;
        //        }
        //        else
        //        {
        //            Debug.LogWarning("not found type[" + _boneType + "].");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogWarning("not found type[" + "hips" + "].");
        //    }
        //    return Vector3.zero;
        //}
        public Vector3 GetWorldPosByTime_Ztest(float _time, JAnimationData.BoneType _boneType, BoneFramework boneframework, Vector3 created_pos, Quaternion created_roa)
        {
            //计算思路是 先计算选定骨骼应该在的位置 再进行误差消除
            //如果是hip节点 直接返回消除空物体的误差的值
            if (_boneType == JAnimationData.BoneType.hips)
            {
                int hip_number = boneMatch.GetIndexOfBoneType(JAnimationData.BoneType.hips);
                Vector3 hip_pos = bonePlayers[hip_number].EvaluateThisFramePos_Curve(_time);
                hip_pos = created_pos + created_roa * hip_pos;//消除旋转误差
                //Debug.Log("hip pos:" + hip_pos);
                //Vector3 correct_pos = new Vector3(hip_pos.x + created_pos.x, hip_pos.y + created_pos.y, hip_pos.z + created_pos.z);
                return hip_pos;
            }
            
            List<Transform> path = new List<Transform>();
            List<JAnimationData.BoneType> zpath = new List<JAnimationData.BoneType>();//骨骼对应bonetype的list
            List<Quaternion> localRoaPath = new List<Quaternion>();

            Transform hipT = null;
            int number = boneMatch.GetIndexOfBoneType(JAnimationData.BoneType.hips);
            hipT = boneMatch.boneMatchList[number].matchedT;
            number = boneMatch.GetIndexOfBoneType(_boneType);
            if (number != -1)
            {
                if (number != -1)
                {
                    JAnimationData.BoneType ztnow = bonePlayers[number].boneOneMatch.boneType;
                    Transform tnow = bonePlayers[number].boneOneMatch.matchedT; //目标骨骼的transform
                    path.Add(tnow);
                    zpath.Add(ztnow);

                    localRoaPath.Add(bonePlayers[number].EvaluateThisFrame_Curve(_time));//目标骨骼当前时间的旋转信息
                    for (int i = 0; i < 101 && tnow.parent != tnow && tnow.parent != hipT; i++)
                    {
                        tnow = tnow.parent;
                        path.Add(tnow);

                        number = boneMatch.GetIndexOfMatchedT(tnow);
                        if (number != -1)
                        {
                            zpath.Add(bonePlayers[number].boneOneMatch.boneType);
                            localRoaPath.Add(bonePlayers[number].EvaluateThisFrame_Curve(_time));
                        }
                        else
                        {

                            localRoaPath.Add(tnow.localRotation);
                        }
                        if (i == 100)
                        {
                            Debug.LogWarning("i get 100 !!!! not normal !!! the bone is too long or program has error !!!!");
                        }
                    }
                    int znumber = boneMatch.GetIndexOfBoneType(JAnimationData.BoneType.hips);
                    zpath.Add(bonePlayers[znumber].boneOneMatch.boneType);
                    zpath.Reverse();//翻转队列 以便从hip骨骼开始计算位置数据
                    path.Add(hipT);
                    path.Reverse();

                    Quaternion zrotation = bonePlayers[znumber].EvaluateThisFrame_Curve(_time);
                    localRoaPath.Add(zrotation);
                    localRoaPath.Reverse();
                    //Debug.Log("path zpath roapath.count: " + path.Count + " " + zpath.Count + " " + localRoaPath.Count);

                    Quaternion roa1 = localRoaPath[0];
                    

                    roa1 = created_roa * roa1; // 如果anim初始带旋转就需要这行 比如学生基本动作 问樵 沈红 范仲禹

                    roa1.Normalize();
                    Vector3 pos1 = bonePlayers[znumber].EvaluateThisFramePos_Curve(_time);//获取当前时间hip骨骼的位置 只有hip骨骼能获取
                    pos1 = created_pos + created_roa * pos1;
                    

                    for (int i = 1; i < zpath.Count; i++)
                    {
                        JAnimationData.BoneType zbonetype = zpath[i];
                        int j = 0;
                        for (j = 0; j < boneframework.Bones.Count; j++)
                        {
                            if (zbonetype == boneframework.Bones[j].boneType)
                                break;
                        }
                        Vector3 temp = boneframework.Bones[j].localPos;//获取目标骨骼相对父级骨骼的localpos   
                        pos1 = pos1 + roa1 * temp;

                        if(created_roa.x != 0.0 || created_roa.y != 0.0 || created_roa.z != 0.0)//如果有初始旋转
                        {
                            roa1 *= localRoaPath[i];  //叠加旋转的因素影响
                        }
                        else
                        {
                            roa1 = localRoaPath[i];
                        }
                        
                      
                    }
                    
                    return pos1;
                }
                else
                {
                    Debug.LogWarning("not found type[" + _boneType + "].");
                }
            }
            else
            {
                Debug.LogWarning("not found type[" + "hips" + "].");
            }
            return Vector3.zero;
        }

        /// <summary>
        /// 获取角度[_boneType]. 
        /// 
        /// </summary> 
        public float GetWorldRoa_Ztest(float _time, JAnimationData.BoneType _boneType, BoneFramework boneframework, Vector3 created_pos, Quaternion created_roa)
        {
            if (_boneType == JAnimationData.BoneType.hips)
            {
                return 0;
            }
            List<Transform> path = new List<Transform>();
            List<JAnimationData.BoneType> zpath = new List<JAnimationData.BoneType>();//骨骼对应bonetype的list
            List<Quaternion> localRoaPath = new List<Quaternion>();
            int number = boneMatch.GetIndexOfBoneType(_boneType);

            BoneFramework.Bone targetBone = boneframework.GetBoneByType(_boneType);

            int childNum = targetBone.ChildsList[0].numberInList;
            JAnimationData.BoneType childBonetype = boneframework.Bones[childNum].boneType;
            //Debug.Log("targetbone : " + _boneType);
            //Debug.Log("childbonetype: " + childBonetype);
           

            Transform hipT = null;
            number = boneMatch.GetIndexOfBoneType(JAnimationData.BoneType.hips);
            hipT = boneMatch.boneMatchList[number].matchedT;
            number = boneMatch.GetIndexOfBoneType(_boneType);
            if (number != -1)
            {
                if (number != -1)
                {
                    JAnimationData.BoneType ztnow = bonePlayers[number].boneOneMatch.boneType;
                    Transform tnow = bonePlayers[number].boneOneMatch.matchedT; //目标骨骼的transform
                    path.Add(tnow);
                    zpath.Add(ztnow);

                    localRoaPath.Add(bonePlayers[number].EvaluateThisFrame_Curve(_time));//目标骨骼当前时间的旋转信息
                    for (int i = 0; i < 101 && tnow.parent != tnow && tnow.parent != hipT; i++)
                    {
                        tnow = tnow.parent;
                        path.Add(tnow);

                        number = boneMatch.GetIndexOfMatchedT(tnow);
                        if (number != -1)
                        {
                            zpath.Add(bonePlayers[number].boneOneMatch.boneType);
                            localRoaPath.Add(bonePlayers[number].EvaluateThisFrame_Curve(_time));
                        }
                        else
                        {

                            localRoaPath.Add(tnow.localRotation);
                        }
                        if (i == 100)
                        {
                            Debug.LogWarning("i get 100 !!!! not normal !!! the bone is too long or program has error !!!!");
                        }
                    }
                }

            }
            int znumber = boneMatch.GetIndexOfBoneType(JAnimationData.BoneType.hips);
            zpath.Add(bonePlayers[znumber].boneOneMatch.boneType);
         
            JAnimationData.BoneType parentBoneType = zpath[1];
            // Debug.Log("parentBoneType: " + parentBoneType);
         

            Vector3 parentPos = GetWorldPosByTime_Ztest(_time, parentBoneType, boneframework, created_pos, created_roa);
            //Debug.Log("parentpos: " + parentPos);
            Vector3 childPos = GetWorldPosByTime_Ztest(_time, childBonetype, boneframework, created_pos, created_roa);
            Vector3 targetPos = GetWorldPosByTime_Ztest(_time, _boneType, boneframework, created_pos, created_roa);
            //Debug.Log("childpos: " + childPos);
            //Debug.Log("targetpos: " + targetPos);

            Vector3 targetToParent = targetPos - parentPos;
            Vector3 targetToChild = targetPos - childPos;

            float targetAngle = Vector3.Angle(targetToParent, targetToChild);
            return targetAngle;
        }

       



        /// <summary>
        /// 将现存的动画数据转换为AnimationClip
        /// </summary>
        /// <returns></returns>
        public AnimationClip TurnToAnimClip(Transform rootGA, bool useRootMotion = false)
        {
            if (bonePlayers.Count == 0) return null;
            AnimationClip clip1 = new AnimationClip();
            clip1.legacy = true;
            clip1.name = "turnedClip";
            clip1.wrapMode = WrapMode.Loop;
            for (int i=0; i<bonePlayers.Count; i++)
            {
                if (bonePlayers[i].boneOneMatch.matchedT == null) continue;
                string path1 = boneMatch.GetRelativePath(rootGA, bonePlayers[i].boneOneMatch.matchedT);
                //Debug.Log(path1);
                if (bonePlayers[i].boneOneMatch.boneType == JAnimationData.BoneType.hips)
                {
                    if(useRootMotion)
                    { 
                        string path_hipRM = "hipMover";
                        clip1.SetCurve(path_hipRM, typeof(Transform), "localPosition.x", bonePlayers[i].Curve_X_Pos);
                        clip1.SetCurve(path_hipRM, typeof(Transform), "localPosition.y", bonePlayers[i].Curve_Y_Pos);
                        clip1.SetCurve(path_hipRM, typeof(Transform), "localPosition.z", bonePlayers[i].Curve_Z_Pos);
                        clip1.SetCurve(path_hipRM, typeof(Transform), "localRotation.x", bonePlayers[i].Curve_X_4_Roa);
                        clip1.SetCurve(path_hipRM, typeof(Transform), "localRotation.y", bonePlayers[i].Curve_Y_4_Roa);
                        clip1.SetCurve(path_hipRM, typeof(Transform), "localRotation.z", bonePlayers[i].Curve_Z_4_Roa);
                        clip1.SetCurve(path_hipRM, typeof(Transform), "localRotation.w", bonePlayers[i].Curve_W_4_Roa);
                        clip1.SetCurve(path1, typeof(Transform), "localRotation.x", bonePlayers[i].Curve_X_4_Roa);
                        clip1.SetCurve(path1, typeof(Transform), "localRotation.y", bonePlayers[i].Curve_Y_4_Roa);
                        clip1.SetCurve(path1, typeof(Transform), "localRotation.z", bonePlayers[i].Curve_Z_4_Roa);
                        clip1.SetCurve(path1, typeof(Transform), "localRotation.w", bonePlayers[i].Curve_W_4_Roa);
                        /* if (rootMotionLockY)
                         {
                             clip1.SetCurve(path1, typeof(Transform), "localRotation.x", bonePlayers[i].Curve_X_4_Roa);
                             clip1.SetCurve(path1, typeof(Transform), "localRotation.y", bonePlayers[i].Curve_Y_4_Roa);
                             clip1.SetCurve(path1, typeof(Transform), "localRotation.z", bonePlayers[i].Curve_Z_4_Roa);
                             clip1.SetCurve(path1, typeof(Transform), "localRotation.w", bonePlayers[i].Curve_W_4_Roa);

                             clip1.SetCurve(path1, typeof(Transform), "localRotation.x", bonePlayers[i].GetCurve4_RoaExceptY_X());
                             clip1.SetCurve(path1, typeof(Transform), "localRotation.y", bonePlayers[i].GetCurve4_RoaExceptY_Y());
                             clip1.SetCurve(path1, typeof(Transform), "localRotation.z", bonePlayers[i].GetCurve4_RoaExceptY_Z());
                             clip1.SetCurve(path1, typeof(Transform), "localRotation.w", bonePlayers[i].GetCurve4_RoaExceptY_W());
                             clip1.SetCurve(path_hipRM, typeof(Transform), "localRotation.x", bonePlayers[i].GetCurve4_RoaOnlyY_X());
                             clip1.SetCurve(path_hipRM, typeof(Transform), "localRotation.y", bonePlayers[i].GetCurve4_RoaOnlyY_Y());
                             clip1.SetCurve(path_hipRM, typeof(Transform), "localRotation.z", bonePlayers[i].GetCurve4_RoaOnlyY_Z());
                             clip1.SetCurve(path_hipRM, typeof(Transform), "localRotation.w", bonePlayers[i].GetCurve4_RoaOnlyY_W());
                         }
                         else
                         {
                             clip1.SetCurve(path_hipRM, typeof(Transform), "localRotation.x", bonePlayers[i].Curve_X_4_Roa);
                             clip1.SetCurve(path_hipRM, typeof(Transform), "localRotation.y", bonePlayers[i].Curve_Y_4_Roa);
                             clip1.SetCurve(path_hipRM, typeof(Transform), "localRotation.z", bonePlayers[i].Curve_Z_4_Roa);
                             clip1.SetCurve(path_hipRM, typeof(Transform), "localRotation.w", bonePlayers[i].Curve_W_4_Roa);
                         } */
                    }
                    else
                    {
                        clip1.SetCurve(path1, typeof(Transform), "localRotation.x", bonePlayers[i].Curve_X_4_Roa);
                        clip1.SetCurve(path1, typeof(Transform), "localRotation.y", bonePlayers[i].Curve_Y_4_Roa);
                        clip1.SetCurve(path1, typeof(Transform), "localRotation.z", bonePlayers[i].Curve_Z_4_Roa);
                        clip1.SetCurve(path1, typeof(Transform), "localRotation.w", bonePlayers[i].Curve_W_4_Roa);
                        clip1.SetCurve(path1, typeof(Transform), "localPosition.x", bonePlayers[i].Curve_X_Pos);
                        clip1.SetCurve(path1, typeof(Transform), "localPosition.y", bonePlayers[i].Curve_Y_Pos);
                        clip1.SetCurve(path1, typeof(Transform), "localPosition.z", bonePlayers[i].Curve_Z_Pos);
                    } 
                }
                else
                {
                    clip1.SetCurve(path1, typeof(Transform), "localRotation.x", bonePlayers[i].Curve_X_4_Roa);
                    clip1.SetCurve(path1, typeof(Transform), "localRotation.y", bonePlayers[i].Curve_Y_4_Roa);
                    clip1.SetCurve(path1, typeof(Transform), "localRotation.z", bonePlayers[i].Curve_Z_4_Roa);
                    clip1.SetCurve(path1, typeof(Transform), "localRotation.w", bonePlayers[i].Curve_W_4_Roa);
                } 
            }
            return clip1;
        } 

        /// <summary>
        /// Get a world Roa of bone of [_boneType]. 
        /// 
        /// </summary> 
        public Quaternion GetWorldRoaByTime(float _time, JAnimationData.BoneType _boneType)
        {
            //if (roaSpaceType == JAnimationData.RoaSpaceType.world)
            //{
            //    Debug.LogError("该函数在当前旋转坐标系类型不可用！");
            //    return Quaternion.Euler(Vector3.zero);
            //}
            Quaternion roa1 = Quaternion.Euler(0, 0, 0);
            Transform hipT = null;
            int number = boneMatch.GetIndexOfBoneType(JAnimationData.BoneType.hips);
            if (number != -1)
            {
                hipT = boneMatch.boneMatchList[number].matchedT;
                number = boneMatch.GetIndexOfBoneType(_boneType);
                if (number != -1)
                {
                    roa1 = bonePlayers[number].EvaluateThisFrame_Curve(_time) * roa1;
                    Transform tnow = bonePlayers[number].boneOneMatch.matchedT;
                    for(int i=0; i<101 && tnow.parent != tnow && tnow.parent != hipT; i++)
                    {
                        tnow = tnow.parent;
                        number = boneMatch.GetIndexOfMatchedT(tnow);
                        if(number != -1)
                        {
                            roa1 = bonePlayers[number].EvaluateThisFrame_Curve(_time) * roa1;
                        }
                        else
                        {
                            roa1 = tnow.localRotation * roa1;
                        }
                        if(i == 100)
                        {
                            Debug.LogWarning("i get 100 !!!! not normal !!! the bone is too long or program has error !!!!");
                        }
                    } 
                    roa1 = hipT.rotation * roa1;
                    return roa1;
                }
                else
                {
                    Debug.LogWarning("not found type[" + _boneType + "].");
                }
            }
            else
            {
                Debug.LogWarning("not found type[" + "hips" + "].");
            }
            return Quaternion.Euler(0, 0, 0);
        }

        /// <summary>
        /// Get a local Roa of bone of [_boneType].
        /// </summary> 
        public Quaternion GetLocalRoaByTime(float _time, JAnimationData.BoneType _boneType)
        {
            if (roaSpaceType == JAnimationData.RoaSpaceType.world)
            {
                Debug.LogError("该函数在当前旋转坐标系类型不可用！");
                return Quaternion.Euler(Vector3.zero);
            }
            int number = boneMatch.GetIndexOfBoneType(_boneType);
            if(number != -1)
            {
                return bonePlayers[number].EvaluateThisFrame_Curve(_time);
            }
            else
            {
                Debug.LogWarning("not found type[" + _boneType + "].");
            }
            return Quaternion.Euler(0, 0, 0);
        }
        public void SetJAnimationData(JAnimationData _data, bool useBoneAdapt = false)
        {
            SetJAnimationData(_data.jAD, useBoneAdapt);
        }
        public void SetJAnimationData(JAnimationData.JAD _data, bool useBoneAdapt = false)
        {

            if (_data == null)
            {
                Debug.LogError("JAnimationData == null !!!");
                return;
            }
            if (bonePlayers.Count == 0)
            {
                Debug.LogError("bonePlayers.Count == 0 !!!");
                return;

            }

            roaSpaceType = _data.roaSpaceType;

            if (useBoneAdapt)
            {
                if(roaSpaceType == JAnimationData.RoaSpaceType.world)
                {
                    Debug.LogError("useBoneAdapt不能在roaSpaceType为world时使用！");
                    return;
                }
                Transform root1 = bonePlayers[0].boneOneMatch.matchedT.root;
                JAnimation_AutoSetBoneMatchData autoMatchData =
                    Resources.Load<JAnimation_AutoSetBoneMatchData>("AutoSetBoneMatchDatas/Auto Set Bone Match Data_TypeName");
                BoneFramework boneFramework1 = new BoneFramework();
                BoneFramework.ReadBoneFrame(root1.gameObject, autoMatchData, out boneFramework1, 1f);
                Debug.Log("ReadBoneFrame");

                BoneTPoseMarker marker1 = null;
                for (int k3 = 0; k3 < bonePlayers.Count; k3++)
                {
                    if (bonePlayers[k3].boneOneMatch != null)
                    {
                        if (bonePlayers[k3].boneOneMatch.matchedT != null)
                        {
                            marker1 = boneMatch.GetTPoseMarker(bonePlayers[k3].boneOneMatch.matchedT);
                            break;
                        }
                    }
                }
                if (marker1 == null)
                {
                    Debug.LogWarning("no marker!!!");

                }
                else
                {
                     

                    List<BoneOneCurve> boneCurves_pre = new List<BoneOneCurve>();
                    for (int i = 0; i < _data.dataList.Count; i++)
                    {
                        boneCurves_pre.Add(new BoneOneCurve(_data.dataList[i].boneType));
                        //Debug.Log(i.ToString()+" add " + _data.dataList[i].boneType);
                    }

                    //-----预处理一次 Preprocessing
                    for (int i = 0; i < _data.dataList.Count; i++)
                    {
                        int _index = -1;
                        for (int j1 = 0; j1 < boneCurves_pre.Count; j1++)
                        {
                            if (boneCurves_pre[j1].boneType == _data.dataList[i].boneType)
                            {
                                _index = j1;
                                break;
                            }
                        }
                        if (_index != -1)
                        {
                            boneCurves_pre[_index].SetAnimDatas(_data.dataList[i].V4List_roa4,
                            _data.dataList[i].V3List_pos, _data.dataList[i].timeList_pos, _data.dataList[i].timeList_roa);

                        }
                        else
                        {

                            Debug.LogWarning("One Bone is Not Match To This GameObejct ! ");
                        }
                    }

                    //开始动画的骨骼匹配 Start the adapt of the animation.
                    for (int i = 0; i < _data.dataList.Count; i++)
                    {
                        List<JAnimationData.BoneType> bPath = _data.boneFramework.GetBonePath(_data.dataList[i].boneType);
                        List<Vector4> newRoaList = new List<Vector4>();
                        /*if (_data.dataList[i].boneType == JAnimationData.BoneType.leftForeArm)
                        {
                            for(int j11 = 0; j11< bPath.Count; j11++)
                            {
                                Debug.Log(""+j11+"  " + bPath[j11]);
                            }
                        }*/
                        if (marker1.tPoseData.GetDataByType(_data.dataList[i].boneType) == null)
                        {
                            Debug.LogWarning("" + marker1.gameObject.name + " don't have [" + _data.dataList[i].boneType + "]");
                            continue;
                        }
                        for (int j = 0; j < _data.dataList[i].V4List_roa4.Count; j++)
                        {
                            Quaternion roa_world = Quaternion.Euler(Vector3.zero);
                            Quaternion roa_world_parent = Quaternion.Euler(Vector3.zero);
                            for (int k = 0; k < bPath.Count; k++)
                            {
                                bool found = false;
                                for (int k1 = 0; k1 < boneCurves_pre.Count; k1++)
                                {
                                    if (boneCurves_pre[k1].boneType == bPath[k])
                                    {
                                        roa_world *= boneCurves_pre[k1].
                                            EvaluateThisFrame_Curve(_data.dataList[i].timeList_roa[j]);
                                        if (k < bPath.Count - 1)
                                        {
                                            roa_world_parent *= boneCurves_pre[k1].
                                            EvaluateThisFrame_Curve(_data.dataList[i].timeList_roa[j]);
                                        }
                                        found = true;
                                        break;
                                    }
                                }
                                if (found == false)
                                {
                                    roa_world *=
                                        Quaternion.Euler(_data.boneFramework.GetBoneDataByType(_data.dataList[i].boneType).localRotation);
                                    if (k < bPath.Count - 1)
                                    {
                                        roa_world_parent *= Quaternion.Euler(_data.boneFramework.GetBoneDataByType(_data.dataList[i].boneType).localRotation);
                                    }
                                    //Debug.LogWarning("bonePlayers[" + bPath[k] + "] Not Found !!!");
                                }
                            }//获取了世界旋转


                            /*if (_data.dataList[i].boneType == JAnimationData.BoneType.leftForeArm)
                            {
                                Debug.Log(roa_world+"  "+ roa_world_parent);
                            }*/


                            //--获取相对旋转
                            Quaternion offsetQ = Quaternion.Euler(Vector3.zero);
                            offsetQ =
                                    Quaternion.Inverse(_data.tPoseData.GetDataByType(
                                        _data.dataList[i].boneType).roa_World) *
                                        marker1.tPoseData.GetDataByType(_data.dataList[i].boneType).roa_World;
                            roa_world *= offsetQ;

                            if (bPath.Count > 1)
                            {
                                if (marker1.tPoseData.GetDataByType(bPath[bPath.Count - 2]) == null)
                                {
                                    Debug.Log("" + marker1.gameObject.name + "  don't have parent [" + bPath[bPath.Count - 2] + "]");
                                    JAnimationData.BoneType parentType = bPath[bPath.Count - 2];
                                    for (int k13 = bPath.Count - 2; k13 >= 0; k13--)
                                    {
                                        if (marker1.tPoseData.GetDataByType(bPath[k13]) != null)
                                        {
                                            parentType = bPath[k13];
                                            break;
                                        }
                                    }
                                    if (parentType == bPath[bPath.Count - 2])
                                    {
                                        Debug.LogError("" + bPath[bPath.Count - 1] + " can't have a parent in this model!(" + marker1.gameObject.name + ")");

                                    }
                                    else
                                    {
                                        Quaternion offsetParent =
                                            Quaternion.Inverse(_data.tPoseData.GetDataByType(
                                                parentType).roa_World) *
                                                marker1.tPoseData.GetDataByType(parentType).roa_World;

                                        int index_b_Parent = -1;
                                        for (int k11 = bPath.Count - 2; k11 >= 0; k11--)
                                        {
                                            if (bPath[k11] == parentType)
                                            {
                                                index_b_Parent = k11;
                                                break;
                                            }
                                        }
                                        if (index_b_Parent == -1)
                                        {
                                            Debug.LogError("Can't find a bone of [" + parentType + "]!!(b)");
                                        }
                                        else
                                        {
                                            List<Quaternion> QPath_b = _data.boneFramework.GetBonePath_LocalRoa(_data.dataList[i].boneType);
                                            for (int k12 = QPath_b.Count - 2; k12 > index_b_Parent; k12--)
                                            {
                                                roa_world_parent *= Quaternion.Inverse(QPath_b[k12]);
                                            }
                                            roa_world_parent *= offsetParent;
                                            Quaternion finalLocalQ =
                                                Quaternion.Inverse(roa_world_parent) * roa_world;
                                            newRoaList.Add(new Vector4(finalLocalQ.x, finalLocalQ.y, finalLocalQ.z, finalLocalQ.w));
                                            Debug.Log("进行了不足层级的变换 (" + _data.dataList[i].boneType + "  其修正后的parent是 " + parentType + ")");
                                        }
                                    }


                                }
                                else
                                {
                                    Quaternion offsetParent =
                                    Quaternion.Inverse(_data.tPoseData.GetDataByType(
                                        bPath[bPath.Count - 2]).roa_World) *
                                        marker1.tPoseData.GetDataByType(bPath[bPath.Count - 2]).roa_World;
                                    roa_world_parent *= offsetParent;

                                    Quaternion finalLocalQ =
                                        Quaternion.Inverse(roa_world_parent) * roa_world;

                                    List<JAnimationData.BoneType> mPath =
                                        marker1.boneFramework.GetBonePath(_data.dataList[i].boneType);

                                    if (bPath.Count >= 2 && mPath.Count >= 2)
                                    {
                                        if (mPath[mPath.Count - 2] != bPath[bPath.Count - 2])
                                        //如果模型上对应骨骼的父级有多余的层级
                                        {
                                            JAnimationData.BoneType bParentBT = bPath[bPath.Count - 1];
                                            for (int k11 = bPath.Count - 2; k11 >= 0; k11--)
                                            {
                                                if (bPath[k11] != JAnimationData.BoneType.unknowType)
                                                {
                                                    bParentBT = bPath[k11];
                                                    break;
                                                }
                                            }
                                            if (bParentBT == bPath[bPath.Count - 1])
                                            {
                                                Debug.LogError("Can't find a parent boneType!!(b)");
                                            }

                                            int index_m_Parent = -1;
                                            for (int k11 = mPath.Count - 2; k11 >= 0; k11--)
                                            {
                                                if (mPath[k11] == bParentBT)
                                                {
                                                    index_m_Parent = k11;
                                                    break;
                                                }
                                            }
                                            if (index_m_Parent == -1)
                                            {
                                                Debug.LogError("Can't find a bone of [" + bParentBT + "]!!(m)");
                                                /*
                                                JAnimationData.BoneType mParentBT = mPath[mPath.Count - 1];
                                                for (int k11 = mPath.Count - 2; k11 >= 0; k11--)
                                                {
                                                    if (mPath[k11] != JAnimationData.BoneType.unknowType)
                                                    {
                                                        mParentBT = mPath[k11];
                                                        break;
                                                    }
                                                }
                                                if (mParentBT == mPath[bPath.Count - 1])
                                                {
                                                    Debug.LogError("Can't find a parent boneType!!(m)");
                                                }
                                                else
                                                {
                                                    int index_b_Parent = -1;
                                                    for (int k11 = bPath.Count - 2; k11 >= 0; k11--)
                                                    {
                                                        if (bPath[k11] == mParentBT)
                                                        {
                                                            index_b_Parent = k11;
                                                            break;
                                                        }
                                                    }
                                                    if (index_b_Parent == -1)
                                                    {
                                                        Debug.LogError("Can't find a bone of [" + mParentBT + "]!!(b)");
                                                    }
                                                    else
                                                    {
                                                        List<Quaternion> QPath_b = _data.boneFramework.GetBonePath_LocalRoa(_data.dataList[i].boneType);
                                                        for (int k12 = QPath_b.Count - 2; k12 > index_b_Parent; k12--)
                                                        {
                                                            roa_world_parent *= Quaternion.Inverse(QPath_b[k12]);
                                                        }
                                                        finalLocalQ =
                                                            Quaternion.Inverse(roa_world_parent) * roa_world;
                                                        newRoaList.Add(new Vector4(finalLocalQ.x, finalLocalQ.y, finalLocalQ.z, finalLocalQ.w));
                                                        Debug.Log("进行了不足层级的变换 (" + _data.dataList[i].boneType + ")");
                                                    } 
                                                }*/

                                            }
                                            else
                                            {
                                                List<Quaternion> QPath_m = marker1.boneFramework.GetBonePath_LocalRoa(_data.dataList[i].boneType);

                                                for (int k12 = index_m_Parent; k12 < QPath_m.Count - 1; k12++)
                                                {
                                                    roa_world_parent *= QPath_m[k12];
                                                }
                                                finalLocalQ =
                                                    Quaternion.Inverse(roa_world_parent) * roa_world;
                                                newRoaList.Add(new Vector4(finalLocalQ.x, finalLocalQ.y, finalLocalQ.z, finalLocalQ.w));
                                                Debug.Log("进行了多余层级的变换 (" + _data.dataList[i].boneType + ")");
                                            }

                                        }
                                        else
                                        {
                                            newRoaList.Add(new Vector4(finalLocalQ.x, finalLocalQ.y, finalLocalQ.z, finalLocalQ.w));
                                        }
                                    }
                                }

                            }
                            else
                            {

                                newRoaList.Add(new Vector4(roa_world.x, roa_world.y, roa_world.z, roa_world.w));
                            }


                        }
                        /*if (_data.dataList[i].boneType == JAnimationData.BoneType.leftForeArm)
                        {
                            for (int j11 = 0; j11 < newRoaList.Count; j11++)
                            {
                                Debug.Log("" + j11 + "  " + newRoaList[j11]);
                            }
                        }*/

                        int _index = boneMatch.GetIndexOfBoneType(_data.dataList[i].boneType);
                        if (_index != -1)
                        {
                            //Debug.Log("bonePlayers[" + bonePlayers[_index].boneOneMatch.boneType + "] " + _data2.dataList[i].V4List_roa4.Count);
                            bonePlayers[_index].SetAnimDatas(JAnimationData.RoaSpaceType.local, newRoaList,
                          _data.dataList[i].V3List_pos, _data.dataList[i].timeList_pos, _data.dataList[i].timeList_roa);
                        }
                        else
                        {
                            Debug.LogWarning("One Bone is Not Match To This GameObejct ! ");
                        }


                    }

                }





            }
            else
            {

                //no adapt
                if (roaSpaceType == JAnimationData.RoaSpaceType.local)
                {
                    for (int i = 0; i < _data.dataList.Count; i++)
                    {
                        int _index = boneMatch.GetIndexOfBoneType(_data.dataList[i].boneType);
                        if (_index != -1)
                        {
                            bonePlayers[_index].SetAnimDatas(roaSpaceType, 
                                _data.dataList[i].V4List_roa4,
                             _data.dataList[i].V3List_pos
                             , _data.dataList[i].timeList_pos, _data.dataList[i].timeList_roa);

                        }
                        else
                        {
                            Debug.LogWarning("One Bone is Not Match To This GameObejct ! ");
                        }
                    }
                }
                else if (roaSpaceType == JAnimationData.RoaSpaceType.world)
                {
                    if ( _data.tPoseData != null && _data.tPoseData.dataList.Count > 0 && boneMatch.GetTPoseMarker(bonePlayers[0].boneOneMatch.matchedT) != null)
                    {
                        Debug.Log("roa-world-space  adapt !!");
                        JAnimationData.TPoseData targetTPose = boneMatch.GetTPoseMarker(bonePlayers[0].boneOneMatch.matchedT).tPoseData;
                        JAnimationData.TPoseData animTPose = _data.tPoseData;
                        for (int i = 0; i < animTPose.dataList.Count; i++)
                        {
                            JAnimationData.TPoseData.TPoseOneData tPoseData1 = targetTPose.GetDataByType(animTPose.dataList[i].boneType);
                            if (tPoseData1 == null) continue;
                            Quaternion q1 = Quaternion.Inverse(animTPose.dataList[i].roa_World) * tPoseData1.roa_World;

                            int _index = boneMatch.GetIndexOfBoneType(animTPose.dataList[i].boneType);
                            int j = _data.GetTypeIndex(animTPose.dataList[i].boneType);
                            if (_index != -1 && j != -1)
                            {
                                bonePlayers[_index].SetAnimDatas(roaSpaceType,
                                    _data.dataList[j].V4List_roa4,  _data.dataList[j].V3List_pos
                                 , _data.dataList[j].timeList_pos, _data.dataList[j].timeList_roa, q1.eulerAngles);
                                Debug.Log("set!!");
                            }
                            else
                            {
                                Debug.LogWarning("_index == -1 || j == -1");
                            }
                        }
                    }
                    else
                    { 
                        if (boneMatch.GetTPoseMarker(bonePlayers[0].boneOneMatch.matchedT) == null)
                        {
                            Debug.LogWarning("GetTPoseMarker(bonePlayers[0].boneOneMatch.matchedT) == null");
                        }

                        for (int i = 0; i < _data.dataList.Count; i++)
                        {
                            int _index = boneMatch.GetIndexOfBoneType(_data.dataList[i].boneType);
                            if (_index != -1)
                            {
                                bonePlayers[_index].SetAnimDatas(roaSpaceType,
                                   _data.dataList[i].V4List_roa4, 
                             _data.dataList[i].V3List_pos
                                 , _data.dataList[i].timeList_pos, _data.dataList[i].timeList_roa);

                            }
                            else
                            {
                                Debug.LogWarning("One Bone is Not Match To This GameObejct ! ");
                            }
                        }
                    }
                }

                 
            }




        }


        public void PlayDataInThisFrame_Curve(float _timeNow, bool useMove = true, float mix = 1f)
        {
            for (int i = 0; i < bonePlayers.Count; i++)
            {
                bonePlayers[i].PlayInThisFrame_Curve(_timeNow, useMove, mix);
            }
        }
    }



}
