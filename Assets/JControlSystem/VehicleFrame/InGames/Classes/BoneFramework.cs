using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JAnimationSystem;

namespace JVehicleFrameSystem
{
    [System.Serializable]
    public class BoneFramework
    {
        [System.Serializable]
        public class Bone
        {
            [System.Serializable]
            public class BoneChild
            {
                public int numberInList = -1;
                public BoneChild(int _numberInList = -1)
                {
                    numberInList = _numberInList;
                }
            }
            #坐标
            public JAnimationData.BoneType boneType = JAnimationData.BoneType.hips;
            public bool isArmor = false;
            public string armorMainName = "";
            public string armorPartName = "";
            public float boneSize = 0.05f;
            public Vector3 localPos = Vector3.zero;
            public Vector3 localScale = Vector3.one;
            public Vector3 localRotation = Vector3.zero;
            /// <summary>
            /// The childs
            /// </summary>
            public List<BoneChild> ChildsList = new List<BoneChild>();

            public Bone()
            {
                boneType = JAnimationData.BoneType.unknowType;
                isArmor = false;
                boneSize = 0.05f;
                localPos = Vector3.zero;
                localScale = Vector3.one;
                localRotation = Vector3.zero;
                ChildsList = new List<BoneChild>();
            }

            public GameObject buildBone(bool useSphere = true, bool changeSphereScale = false, float sphereScale = 0.05f)
            {
                GameObject ga1;
                if (isArmor == false)
                {
                    ga1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    ga1.GetComponent<Renderer>().enabled = false;
                    if (Application.isEditor)
                    {
                        Object.DestroyImmediate(ga1.GetComponent<SphereCollider>());
                    }
                    else
                    {
                        Object.Destroy(ga1.GetComponent<SphereCollider>());
                    }
                    ga1.name = boneType.ToString();
                    ga1.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    ga1.transform.localPosition = Vector3.zero;
                    ga1.transform.localScale = Vector3.one;

                    if (useSphere)
                    {
                        // float zboneSize = 0.2f;
                        GameObject sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere1.transform.parent = ga1.transform;
                        sphere1.transform.localScale = Vector3.one * (changeSphereScale ? sphereScale : boneSize);//sphereScale可改 内置数据bonesize是由boneframework确定的
                        //sphere1.transform.localScale = Vector3.one * (changeSphereScale ? sphereScale : zboneSize);
                        sphere1.transform.localPosition = Vector3.zero;
                        sphere1.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        Material baseMa = Resources.Load<Material>("baseMaterials/baseMaterial");
                        if (baseMa != null)
                        {
                            sphere1.GetComponent<Renderer>().material = Resources.Load<Material>("baseMaterials/baseMaterial");
                        }
                    }
                }
                else
                {
                    ga1 = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Vehicle/Parts/" + armorMainName + "/Armor/" + armorPartName));
                    ga1.name = "Armor_" + armorMainName + "_" + armorPartName;
                    ga1.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    ga1.transform.localPosition = Vector3.zero;
                    ga1.transform.localScale = Vector3.one;
                }

                return ga1;
            }
        }



        public List<Bone> Bones = new List<Bone>();
        public BoneFramework()
        {
            Bones = new List<Bone>();
        }

        /// <summary>
        /// 根据boneType获取对应类型的骨骼，如果没有返回null
        /// </summary>
        public Bone GetBoneByType(JAnimationData.BoneType _type)
        {
            for (int i = 0; i < Bones.Count; i++)
            {
                if (Bones[i].boneType == _type)
                {
                    return Bones[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 根据boneType获取对应类型的骨骼的序号，如果没有返回-1
        /// </summary>
        public int GetIndexByType(JAnimationData.BoneType _type)
        {
            for (int i = 0; i < Bones.Count; i++)
            {
                if (Bones[i].boneType == _type)
                {
                    return i;
                }
            }
            return -1;
        }

        public Quaternion GetBoneWorldRoation(JAnimationData.BoneType _type)
        {
            List<JAnimationData.BoneType> path1 = GetBonePath(_type);
            Quaternion theQ = Quaternion.Euler(Vector3.zero);
            for (int i = 0; i < path1.Count; i++)
            {
                Bone bone1 = GetBoneDataByType(path1[i]);
                if (bone1 != null)
                {
                    theQ *= Quaternion.Euler(bone1.localRotation);
                }
                else
                {
                    Debug.LogError("(" + path1[i].ToString() + ") Bone is null !!!!");
                }
            }
            return theQ;
        }

        public List<JAnimationData.BoneType> GetBonePath(JAnimationData.BoneType _type)
        {
            if (_type == JAnimationData.BoneType.unknowType)
            {
                Debug.LogError("_type == JAnimationData.BoneType.unknowType!!");
                return null;
            }
            List<JAnimationData.BoneType> newList = new List<JAnimationData.BoneType>();
            Bone bone1 = GetBoneDataByType(_type);
            newList.Add(bone1.boneType);
            bone1 = FindWhoContainTheTypeIndex(Bones.IndexOf(bone1));
            while (bone1 != null)
            {
                newList.Add(bone1.boneType);
                bone1 = FindWhoContainTheTypeIndex(Bones.IndexOf(bone1));
            }
            newList.Reverse();
            return newList;
        }

        public List<Quaternion> GetBonePath_LocalRoa(JAnimationData.BoneType _type)
        {
            if (_type == JAnimationData.BoneType.unknowType)
            {
                Debug.LogError("_type == JAnimationData.BoneType.unknowType!!");
                return null;
            }

            List<Quaternion> newList = new List<Quaternion>();
            Bone bone1 = GetBoneDataByType(_type);

            newList.Add(Quaternion.Euler(bone1.localRotation));
            bone1 = FindWhoContainTheTypeIndex(Bones.IndexOf(bone1));
            while (bone1 != null)
            {
                newList.Add(Quaternion.Euler(bone1.localRotation));
                bone1 = FindWhoContainTheTypeIndex(Bones.IndexOf(bone1));
            }
            newList.Reverse();
            return newList;
        }

        public Bone GetBoneDataByType(JAnimationData.BoneType _type)
        {
            for (int i = 0; i < Bones.Count; i++)
            {
                if (Bones[i].boneType == _type)
                {
                    return Bones[i];
                }
            }
            return null;
        }

        private JAnimationData.BoneType FindWhoContainTheIndex(int index1)
        {
            for (int i = 0; i < Bones.Count; i++)
            {
                for (int j = 0; j < Bones[i].ChildsList.Count; j++)
                {
                    if (Bones[i].ChildsList[j].numberInList == index1)
                    {
                        return Bones[i].boneType;
                    }
                }
            }
            return JAnimationData.BoneType.unknowType;
        }
        private Bone FindWhoContainTheTypeIndex(int index1)
        {
            for (int i = 0; i < Bones.Count; i++)
            {
                for (int j = 0; j < Bones[i].ChildsList.Count; j++)
                {
                    if (Bones[i].ChildsList[j].numberInList == index1)
                    {
                        return Bones[i];
                    }
                }
            }
            return null;
        }






        public static bool ReadBoneFrame(GameObject target, JAnimation_AutoSetBoneMatchData autoMatchData
            , out BoneFramework boneFramework, float amplificateFactor, JVehicleFrame jVehicleFrame = null)
        {
            if (target != null && autoMatchData != null)
            {
                BoneMatch boneMatch1 = new BoneMatch();
                boneMatch1.StartAutoMatch(target.transform, autoMatchData);
                /*
                Debug.Log("matched Count: " + boneMatch1.boneMatchList.Count);
                for (int i = 0; i < boneMatch1.boneMatchList.Count; i++)
                {
                    Debug.Log("boneType: " + boneMatch1.boneMatchList[i].boneType + " , matchedT: " + (boneMatch1.boneMatchList[i].matchedT ? boneMatch1.boneMatchList[i].matchedT.name:"NULL"));

                }*/
                BoneFramework boneFramework1 = new BoneFramework();


                int index1 = boneMatch1.GetIndexOfMatchedT(target.transform);
                if (index1 != -1)
                {
                    int newIndex = -1;
                    boneFramework1.Bones.Clear();
                    boneFramework1.Bones.Add(new BoneFramework.Bone());
                    newIndex = boneFramework1.Bones.Count - 1;


                    if (newIndex != -1)
                    {
                        BoneFramework.Bone newBone = boneFramework1.Bones[newIndex];
                        newBone.isArmor = false;
                        newBone.boneType = boneMatch1.boneMatchList[index1].boneType;
                        newBone.localPos = target.transform.localPosition * amplificateFactor;
                        newBone.localRotation = target.transform.localRotation.eulerAngles;
                        newBone.localScale = target.transform.localScale;


                        //Debug.Log("process : " + target.transform.name + "  " + newIndex + "   " + newBone.boneType);

                    }
                    processOneTransform(boneFramework1, target.transform, boneMatch1, target.transform, amplificateFactor, newIndex, target.transform);
                }
                else
                {
                    //Debug.Log("process : " + target.transform.name + "  " + index1 + "  " + " , index1 == -1");
                    processOneTransform(boneFramework1, target.transform, boneMatch1, target.transform, amplificateFactor);
                }


                boneFramework = boneFramework1;
                if (jVehicleFrame != null)
                {
                    for (int i = jVehicleFrame.boneFrameworkMousePosList.Count; i < boneFramework1.Bones.Count; i++)
                    {
                        jVehicleFrame.boneFrameworkMousePosList.Add(Vector2.zero);
                    }
                }
                return true;
            }
            else
            {
                boneFramework = null;
                return false;
            }
        }
        private static void processOneTransform(BoneFramework boneFramework1, Transform rootT, BoneMatch boneMatch1, Transform oneT, float amplificateFactor, int marker = -1, Transform lastT = null)
        {

            for (int i = 0; i < oneT.childCount; i++)
            {
                Transform t1 = oneT.GetChild(i);
                int index1 = boneMatch1.GetIndexOfMatchedT(t1);
                if (index1 != -1)
                {
                    Debug.Log("name: " + t1.name + "    ,    index: " + index1 + "   ,   marker: " + marker + "");
                    int newIndex = -1;
                    if (marker == -1)
                    {
                        // 骨骼链的最外层.
                        // 可能有多个单独的骨骼链.
                        Debug.Log($"处理骨骼链起始节点:{t1.gameObject.name}");
                        // 创建一个空的root用于过渡坐标系
                        if (rootT == null)
                        {
                            boneFramework1.Bones.Add(new BoneFramework.Bone());
                            newIndex = boneFramework1.Bones.Count - 1;
                            processOneTransform(boneFramework1, rootT, boneMatch1, t1, amplificateFactor, newIndex, t1);
                        }
                        else
                        {
                            // 创建一个空的root用于过渡坐标系
                            
                            // List<Transform> boneParentList1 = new List<Transform>();
                            // Transform parentTemp = t1.parent;
                            // while (parentTemp != rootT)
                            // {
                            //     boneParentList1.Add(parentTemp);
                            //     parentTemp = parentTemp.parent;
                            // }
                            // boneParentList1.Add(rootT);
                            // boneParentList1.Reverse(); // 颠倒队列

                            Quaternion roaTemp = t1.parent.rotation;
                            Vector3 posTemp = t1.parent.position;
                            BoneFramework.Bone rootBone1 = new BoneFramework.Bone();
                            {
                                rootBone1.isArmor = false;
                                rootBone1.boneType = JAnimationData.BoneType.unknowType;
                                rootBone1.localPos = posTemp;
                                rootBone1.localPos.x = 0; // 设置XZ在0
                                rootBone1.localPos.z = 0; // 设置XZ在0
                                rootBone1.localRotation = roaTemp.eulerAngles;
                                rootBone1.localScale = Vector3.one; // 默认root的scale就是1
                            }
                            boneFramework1.Bones.Add(rootBone1); // 添加root骨骼
                            boneFramework1.Bones.Add(new BoneFramework.Bone()); // 添加这层t1的骨骼
                            newIndex = boneFramework1.Bones.Count - 1;
                            rootBone1.ChildsList.Add(new BoneFramework.Bone.BoneChild(newIndex));
                            processOneTransform(boneFramework1, rootT, boneMatch1, t1, amplificateFactor, newIndex, t1);
                        }
                    }
                    else
                    {
                        // 骨骼链除了最外层的逻辑都在这里.
                        boneFramework1.Bones.Add(new BoneFramework.Bone());
                        newIndex = boneFramework1.Bones.Count - 1;
                        //processOneTransform(boneFramework1, boneMatch1, t1, newIndex, t1);

                        BoneFramework.Bone parentBone = boneFramework1.Bones[marker];
                        bool has = false;
                        for (int j = 0; j < parentBone.ChildsList.Count; j++)
                        {
                            if (parentBone.ChildsList[j].numberInList == newIndex)
                            {
                                has = true;
                                break;
                            }
                        }
                        if (has == false)
                        {
                            if (lastT != null)
                            {
                                // 如果lastT和这层的t1并不是直接的父子关系, 那么中间补一个虚拟骨骼用来过渡坐标系.
                                if (t1.parent != lastT)
                                {
                                    Quaternion t1VersusLastT = myCalculate.Rotation_BversusA(t1.rotation, lastT.rotation);
                                    Quaternion midLocalRotation = (t1VersusLastT * Quaternion.Inverse(t1.localRotation));
                                    Quaternion midWorldRotation = lastT.rotation * midLocalRotation;
                                    Vector3 midRight = myCalculate.GetRight(midWorldRotation);
                                    Vector3 midUp = myCalculate.GetUp(midWorldRotation);
                                    Vector3 midForward = myCalculate.GetForward(midWorldRotation);
                                    Vector3 midBoneWorldPos = t1.position -
                                        midRight * t1.localPosition.x
                                        - midUp * t1.localPosition.y
                                        - midForward * t1.localPosition.z;
                                    Vector3 midLocalPos = myCalculate.Pos_BversusA(midBoneWorldPos, lastT);
                                    Vector3 midLocalScale = Vector3.one;
                                    Transform theParent = t1.parent;
                                    while (theParent != lastT)
                                    {
                                        midLocalScale.x *= theParent.localScale.x;
                                        midLocalScale.y *= theParent.localScale.y;
                                        midLocalScale.z *= theParent.localScale.z;
                                        theParent = theParent.parent;
                                    }

                                    boneFramework1.Bones.Add(new BoneFramework.Bone());
                                    int midIndex = boneFramework1.Bones.Count - 1;
                                    BoneFramework.Bone midBone = boneFramework1.Bones[midIndex];
                                    parentBone.ChildsList.Add(new BoneFramework.Bone.BoneChild(midIndex));
                                    midBone.ChildsList.Add(new BoneFramework.Bone.BoneChild(newIndex));

                                    midBone.isArmor = false;
                                    midBone.boneType = JAnimationData.BoneType.unknowType;
                                    midBone.localPos = midLocalPos;
                                    midBone.localRotation = midLocalRotation.eulerAngles;
                                    midBone.localScale = midLocalScale;
                                }
                                else
                                {
                                    parentBone.ChildsList.Add(new BoneFramework.Bone.BoneChild(newIndex));
                                }
                            }
                            else
                            {
                                parentBone.ChildsList.Add(new BoneFramework.Bone.BoneChild(newIndex));
                            }

                        }
                        processOneTransform(boneFramework1, rootT, boneMatch1, t1, amplificateFactor, newIndex, t1);//---
                    }

                    if (newIndex != -1)
                    {
                        BoneFramework.Bone newBone = boneFramework1.Bones[newIndex];
                        newBone.isArmor = false;
                        newBone.boneType = boneMatch1.boneMatchList[index1].boneType;
                        newBone.localPos = t1.localPosition * amplificateFactor;
                        newBone.localRotation = t1.localRotation.eulerAngles;
                        newBone.localScale = t1.localScale;


                        //Debug.Log("process : " + t1.name + "  " + newIndex + "   " + newBone.boneType);

                    }

                }
                else
                {
                    // 当前这层的Transform不能匹配成为骨骼, 继续使用上一层的lastT
                    //Debug.Log("process : " + t1.name + "  " + index1 + "  " + " , index1 == -1");
                    processOneTransform(boneFramework1, rootT, boneMatch1, t1, amplificateFactor, marker, lastT);
                }
            }
        }

    }

}