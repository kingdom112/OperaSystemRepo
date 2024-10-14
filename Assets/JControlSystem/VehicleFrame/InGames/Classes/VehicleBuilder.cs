using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JVehicleFrameSystem
{
    public class VehicleBuilder
    {
        public BoneFramework theBoneFramework
        {
            get
            {
                return boneFramework;
            }
        }
        private BoneFramework boneFramework;
        public GameObject buildedGA;


        /// <summary>
        /// 构造器
        /// </summary>
        public VehicleBuilder(BoneFramework _boneFramework)
        {
            boneFramework = _boneFramework;
        }
        /// <summary>
        /// 空构造器
        /// </summary>
        public VehicleBuilder()
        {
            boneFramework = null;
        }

        public void SetBoneFramework(BoneFramework _boneFramework)
        {
            boneFramework = _boneFramework;
        }

        public void Destroy()
        {
            if(Application.isEditor)
            {
                if(buildedGA) 
                    GameObject.DestroyImmediate(buildedGA);
            }
            else
            {
                if (buildedGA)
                    GameObject.Destroy(buildedGA);
            }
        }

        /// <summary>
        /// buildType = 0时是sphere，buildType = 1是line
        /// </summary>
        /// <param name="buildType"></param>
        public void Build(int buildType = 0)
        {
            Destroy();
            if (boneFramework.Bones.Count == 0) return;
          


            buildedGA = new GameObject();
            buildedGA.name = "boneFrameworkBuilt";
            buildedGA.transform.position = Vector3.zero;
         
            
            buildedGA.transform.localScale = Vector3.one;
            buildedGA.transform.rotation = Quaternion.Euler(Vector3.zero);
             

            //寻找每一个独立的没有父级的骨骼分支
            for(int i=0; i< boneFramework.Bones.Count; i++)
            {
                bool hasParent = false;//标记这个骨骼有没有父级
                //开始遍历其他的骨骼，看骨骼i是否是个单独的分支
                {
                    for (int j = 0; j < boneFramework.Bones.Count; j++)
                    {
                        if (j == i) continue;//跳过自己 
                        foreach (var child1 in boneFramework.Bones[j].ChildsList)
                        {
                            if (child1.numberInList == i)
                            {
                                hasParent = true;
                                break;
                            }
                        }
                        if (hasParent) break;
                    }
                } 
                //遍历结束，现在知道这个骨骼是不是独立的了。
                if(hasParent == false)
                {
                    //创建这个骨骼，并开始从这个骨骼进行递归，创建它的子骨骼。
                    Transform buildedGA1 = null;
                    if (buildType == 0)
                    {
                        buildedGA1 = buildOneBone(boneFramework, i).transform;
                    }
                    if (buildType == 1)
                    {
                        buildedGA1 = buildOneBone_line(boneFramework, i).transform;
                    }
                    buildedGA1.parent = buildedGA.transform;
                    buildedGA1.localPosition = boneFramework.Bones[i].localPos;
                    buildedGA1.localScale = boneFramework.Bones[i].localScale;
                    buildedGA1.localRotation = Quaternion.Euler(boneFramework.Bones[i].localRotation);
                }
            }


         
             
            
             
        }

        private GameObject buildOneBone(BoneFramework _boneFramework, int _number)
        {
            if (_number < _boneFramework.Bones.Count)
            {
                GameObject ga1 = null;
                if ((int)boneFramework.Bones[_number].boneType >= 31 && (int)boneFramework.Bones[_number].boneType <= 60)
                {
                    //如果是手的骨骼，缩小十倍
                    Debug.Log("手的骨骼自动缩小显示。");
                    ga1 = _boneFramework.Bones[_number].buildBone(true, true, 0.02f);
                }
                else
                {
                    ga1 = _boneFramework.Bones[_number].buildBone();
                } 
                BoneFramework.Bone bone1 = boneFramework.Bones[_number];
                for (int i = 0; i < bone1.ChildsList.Count; i++)
                {
                    int number1 = bone1.ChildsList[i].numberInList;
                    if (number1 > 0)
                    {
                        Transform buildedGA1 = buildOneBone(boneFramework, number1).transform;
                        buildedGA1.parent = ga1.transform;
                        buildedGA1.localPosition = boneFramework.Bones[number1].localPos;
                        buildedGA1.localScale = boneFramework.Bones[number1].localScale; 
                        buildedGA1.localRotation = Quaternion.Euler(boneFramework.Bones[number1].localRotation);
                    }
                }
                return ga1;
            }
            return null;
        }
         
        private GameObject buildOneBone_line(BoneFramework _boneFramework, int _number)
        {
            if (_number < _boneFramework.Bones.Count)
            {
                GameObject ga1 = null;
                if ((int)boneFramework.Bones[_number].boneType >= 31 && (int)boneFramework.Bones[_number].boneType <= 60)
                {
                    //如果是手的骨骼，缩小十倍
                    Debug.Log("手的骨骼自动缩小显示。");
                    ga1 = _boneFramework.Bones[_number].buildBone(true, true, 0.02f);
                }
                else
                {
                    ga1 = _boneFramework.Bones[_number].buildBone();
                }
                
                BoneFramework.Bone bone1 = boneFramework.Bones[_number];
                for (int i = 0; i < bone1.ChildsList.Count; i++)
                {
                    int number1 = bone1.ChildsList[i].numberInList;
                    if (number1 > 0)
                    {
                        Transform buildedGA1 = buildOneBone_line(boneFramework, number1).transform;
                        buildedGA1.parent = ga1.transform;
                        buildedGA1.localPosition = boneFramework.Bones[number1].localPos;
                        buildedGA1.localScale = boneFramework.Bones[number1].localScale; 
                        buildedGA1.localRotation = Quaternion.Euler(boneFramework.Bones[number1].localRotation);

                        Vector3 f1 = buildedGA1.position - ga1.transform.position; 
                        GameObject capsule1 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        capsule1.transform.parent = ga1.transform;
                        Vector3 scale1 = Vector3.one;
                        scale1 *= 0.02f;
                        scale1.y = f1.magnitude * 0.5f; 
                        capsule1.transform.localScale = scale1;
                        capsule1.transform.position = ga1.transform.position + f1 * 0.5f;
                        capsule1.transform.rotation =  Quaternion.FromToRotation(Vector3.up, f1.normalized);
                        Material baseMa = Resources.Load<Material>("baseMaterials/baseMaterial");
                        if (baseMa != null)
                        {
                            capsule1.GetComponent<Renderer>().material = Resources.Load<Material>("baseMaterials/baseMaterial");
                        }
                    }
                }
                return ga1;
            }
            return null;
        }
    }
} 