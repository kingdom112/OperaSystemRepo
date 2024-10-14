using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Linq;
using System;
public class zStaticActionPanel : MonoBehaviour
{

    public InputField masterStaticTime;
    public InputField studentStaticTime;//静态动作相似度对比时间
    public float masterStaticTimeData;
    public float studentStaticTimeData;
    public Text scoreTextField;//angleScore显示组件
    public Text featurePlaneTextField;//featurePlaneScore显示组件
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (masterStaticTime.text == "")
        {
            masterStaticTimeData = 0.0f;
        }
        else
        {
            masterStaticTimeData = float.Parse(masterStaticTime.text);
        }
        if (studentStaticTime.text == "")
        {
            studentStaticTimeData = 0.0f;
        }
        else
        {
            studentStaticTimeData = float.Parse(studentStaticTime.text);
        }
    }

    public void staticAction()//角度参数静态动作对比打分
    {
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        if (vfplayer.ch_master.createdGA_No != -1)
        {
            if (vfplayer.ch_student.createdGA_No != -1)
            {
                //静态动作差距
                int boneCount = 0;
                float sumScoreDiff = 0.0f;
                for (int j = 0; j < vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones.Count; j++)
                {

                    JAnimationData.BoneType CurBonetype = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones[j].boneType;
                    int index = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.GetTypeIndex(CurBonetype);
                    if (index != -1)
                    {
                        JVehicleFrameSystem.BoneFramework.Bone targetBone = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.GetBoneByType(CurBonetype);
                        int childNum = targetBone.ChildsList.Count;
                        if (childNum == 1)
                        {
                            if (CurBonetype == JAnimationData.BoneType.leftToeBase) continue;
                            if (CurBonetype == JAnimationData.BoneType.rightToeBase) continue;
                            if (CurBonetype == JAnimationData.BoneType.leftFoot) continue;
                            if (CurBonetype == JAnimationData.BoneType.rightFoot) continue;
                            float masterBoneAngle = vfplayer.ch_master.bm_p1.GetWorldRoa_Ztest(masterStaticTimeData, CurBonetype, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            float studentBoneAngle = vfplayer.ch_student.bm_p1.GetWorldRoa_Ztest(studentStaticTimeData, CurBonetype, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            float boneAngleDiff = Mathf.Abs(masterBoneAngle - studentBoneAngle);
                            float scoreDiff = Mathf.Abs(masterBoneAngle - studentBoneAngle) / masterBoneAngle;//记录打分
                            scoreDiff = 1 - scoreDiff;

                            sumScoreDiff += scoreDiff;
                            boneCount++;
                        }

                    }
                    else
                    {
                        continue;
                    }

                }
                sumScoreDiff = 100 * (sumScoreDiff / boneCount);//平均化打分
                if (scoreTextField != null)
                {
                    scoreTextField.text = "角度参数得分：" + sumScoreDiff.ToString("F2");
                }
                Debug.Log("score update ");
            }
        }
    }

    public void featurePlaneDiff()//dtw 角度参数的得分
    {

        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        if (vfplayer.ch_master.createdGA_No != -1)
        {
            if (vfplayer.ch_student.createdGA_No != -1)
            {

                int boneCount = 0;
                //leftArm
                Vector3 masterLeftArmAPos = Vector3.zero;
                Vector3 masterLeftArmBPos = Vector3.zero;
                Vector3 masterLeftArmCPos = Vector3.zero;
                Vector3 studentLeftArmAPos = Vector3.zero;
                Vector3 studentLeftArmBPos = Vector3.zero;
                Vector3 studentLeftArmCPos = Vector3.zero;
                Vector3 masterLeftArmAB = Vector3.zero;
                Vector3 masterLeftArmAC = Vector3.zero;
                Vector3 studentLeftArmAB = Vector3.zero;
                Vector3 studentLeftArmAC = Vector3.zero;

                //rightArm
                Vector3 masterRightArmAPos = Vector3.zero;
                Vector3 masterRightArmBPos = Vector3.zero;
                Vector3 masterRightArmCPos = Vector3.zero;
                Vector3 studentRightArmAPos = Vector3.zero;
                Vector3 studentRightArmBPos = Vector3.zero;
                Vector3 studentRightArmCPos = Vector3.zero;
                Vector3 masterRightArmAB = Vector3.zero;
                Vector3 masterRightArmAC = Vector3.zero;
                Vector3 studentRightArmAB = Vector3.zero;
                Vector3 studentRightArmAC = Vector3.zero;

                //leftLeg
                Vector3 masterLeftLegAPos = Vector3.zero;
                Vector3 masterLeftLegBPos = Vector3.zero;
                Vector3 masterLeftLegCPos = Vector3.zero;
                Vector3 studentLeftLegAPos = Vector3.zero;
                Vector3 studentLeftLegBPos = Vector3.zero;
                Vector3 studentLeftLegCPos = Vector3.zero;
                Vector3 masterLeftLegAB = Vector3.zero;
                Vector3 masterLeftLegAC = Vector3.zero;
                Vector3 studentLeftLegAB = Vector3.zero;
                Vector3 studentLeftLegAC = Vector3.zero;


                //rightLeg
                Vector3 masterRightLegAPos = Vector3.zero;
                Vector3 masterRightLegBPos = Vector3.zero;
                Vector3 masterRightLegCPos = Vector3.zero;
                Vector3 studentRightLegAPos = Vector3.zero;
                Vector3 studentRightLegBPos = Vector3.zero;
                Vector3 studentRightLegCPos = Vector3.zero;
                Vector3 masterRightLegAB = Vector3.zero;
                Vector3 masterRightLegAC = Vector3.zero;
                Vector3 studentRightLegAB = Vector3.zero;
                Vector3 studentRightLegAC = Vector3.zero;

                //Spine
                Vector3 masterSpineAPos = Vector3.zero;
                Vector3 masterSpineBPos = Vector3.zero;
                Vector3 masterSpineCPos = Vector3.zero;
                Vector3 studentSpineAPos = Vector3.zero;
                Vector3 studentSpineBPos = Vector3.zero;
                Vector3 studentSpineCPos = Vector3.zero;
                Vector3 masterSpineAB = Vector3.zero;
                Vector3 masterSpineAC = Vector3.zero;
                Vector3 studentSpineAB = Vector3.zero;
                Vector3 studentSpineAC = Vector3.zero;



                for (int j = 0; j < vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones.Count; j++)
                {

                    JAnimationData.BoneType CurBonetype = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones[j].boneType;
                    int index = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.GetTypeIndex(CurBonetype);
                    if (index != -1)
                    {
                        JVehicleFrameSystem.BoneFramework.Bone targetBone = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.GetBoneByType(CurBonetype);

                        //leftarm特征平面计算
                        if (CurBonetype == JAnimationData.BoneType.leftArm)
                        {
                            masterLeftArmAPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentLeftArmAPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }
                        if (CurBonetype == JAnimationData.BoneType.leftForeArm)
                        {
                            masterLeftArmBPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentLeftArmBPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }
                        if (CurBonetype == JAnimationData.BoneType.leftHand)
                        {
                            masterLeftArmCPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentLeftArmCPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }

                        //rightArm特征平面计算
                        if (CurBonetype == JAnimationData.BoneType.rightArm)
                        {
                            masterRightArmAPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentRightArmAPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }
                        if (CurBonetype == JAnimationData.BoneType.rightForeArm)
                        {
                            masterRightArmBPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentRightArmBPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }
                        if (CurBonetype == JAnimationData.BoneType.rightHand)
                        {
                            masterRightArmCPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentRightArmCPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }

                        //leftLeg特征平面计算
                        if (CurBonetype == JAnimationData.BoneType.leftUpLeg)
                        {
                            masterLeftLegAPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentLeftLegAPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }
                        if (CurBonetype == JAnimationData.BoneType.leftLeg)
                        {
                            masterLeftLegBPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentLeftLegBPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }
                        if (CurBonetype == JAnimationData.BoneType.leftFoot)
                        {
                            masterLeftLegCPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentLeftLegCPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }

                        //rightLeg特征平面计算
                        if (CurBonetype == JAnimationData.BoneType.rightUpLeg)
                        {
                            masterRightLegAPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentRightLegAPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }
                        if (CurBonetype == JAnimationData.BoneType.rightLeg)
                        {
                            masterRightLegBPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentRightLegBPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }
                        if (CurBonetype == JAnimationData.BoneType.rightFoot)
                        {
                            masterRightLegCPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentRightLegCPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }

                        //Spine特征平面计算
                        if (CurBonetype == JAnimationData.BoneType.spine)
                        {
                            masterSpineAPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentSpineAPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }
                        if (CurBonetype == JAnimationData.BoneType.spine1)
                        {
                            masterSpineBPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentSpineBPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }
                        if (CurBonetype == JAnimationData.BoneType.spine2)
                        {
                            masterSpineCPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(masterStaticTimeData, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            studentSpineCPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(studentStaticTimeData, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        }
                    }


                }

                //左臂平面计算相似度
                masterLeftArmAB = masterLeftArmBPos - masterLeftArmAPos;
                masterLeftArmAC = masterLeftArmCPos - masterLeftArmAPos;
                Vector3 masterLeftArmNormal = Vector3.Cross(masterLeftArmAB, masterLeftArmAC);
                masterLeftArmNormal.Normalize();
                studentLeftArmAB = studentLeftArmBPos - studentLeftArmAPos;
                studentLeftArmAC = studentLeftArmCPos - studentLeftArmAPos;
                Vector3 studentLeftArmNormal = Vector3.Cross(studentLeftArmAB, studentLeftArmAC);
                studentLeftArmNormal.Normalize();
                double dotLeftArmProduct = Vector3.Dot(masterLeftArmNormal, studentLeftArmNormal);
                double magnitudeMasterLeftArm = Math.Sqrt(masterLeftArmNormal.x * masterLeftArmNormal.x + masterLeftArmNormal.y * masterLeftArmNormal.y + masterLeftArmNormal.z * masterLeftArmNormal.z);
                double magnitudeStudentLeftArm = Math.Sqrt(studentLeftArmNormal.x * studentLeftArmNormal.x + studentLeftArmNormal.y * studentLeftArmNormal.y + studentLeftArmNormal.z * studentLeftArmNormal.z);

                double angleLeftArmRadians = Math.Acos(dotLeftArmProduct / (magnitudeMasterLeftArm * magnitudeStudentLeftArm));
                double angleCosDataLeftArm = dotLeftArmProduct / (magnitudeMasterLeftArm * magnitudeStudentLeftArm);
                // 将弧度转换为度
                double leftArmAngleDegrees = angleLeftArmRadians * (180.0 / Math.PI);


                //右臂平面计算相似度
                masterRightArmAB = masterRightArmBPos - masterRightArmAPos;
                masterRightArmAC = masterRightArmCPos - masterRightArmAPos;
                Vector3 masterRightArmNormal = Vector3.Cross(masterRightArmAB, masterRightArmAC);
                masterRightArmNormal.Normalize();
                studentRightArmAB = studentRightArmBPos - studentRightArmAPos;
                studentRightArmAC = studentRightArmCPos - studentRightArmAPos;
                Vector3 studentRightArmNormal = Vector3.Cross(studentRightArmAB, studentRightArmAC);
                studentRightArmNormal.Normalize();
                double dotRightArmProduct = Vector3.Dot(masterRightArmNormal, studentRightArmNormal);
                double magnitudeMasterRightArm = Math.Sqrt(masterRightArmNormal.x * masterRightArmNormal.x + masterRightArmNormal.y * masterRightArmNormal.y + masterRightArmNormal.z * masterRightArmNormal.z);
                double magnitudeStudentRightArm = Math.Sqrt(studentRightArmNormal.x * studentRightArmNormal.x + studentRightArmNormal.y * studentRightArmNormal.y + studentRightArmNormal.z * studentRightArmNormal.z);

                double angleRightArmRadians = Math.Acos(dotRightArmProduct / (magnitudeMasterRightArm * magnitudeStudentRightArm));
                double angleCosDataRightArm = dotRightArmProduct / (magnitudeMasterRightArm * magnitudeStudentRightArm);
                // 将弧度转换为度
                double rightArmAngleDegrees = angleLeftArmRadians * (180.0 / Math.PI);


                //左腿平面计算相似度
                masterLeftLegAB = masterLeftLegBPos - masterLeftLegAPos;
                masterLeftLegAC = masterLeftLegCPos - masterLeftLegAPos;
                Vector3 masterLeftLegNormal = Vector3.Cross(masterLeftLegAB, masterLeftLegAC);
                masterLeftLegNormal.Normalize();
                studentLeftLegAB = studentLeftLegBPos - studentLeftLegAPos;
                studentLeftLegAC = studentLeftLegCPos - studentLeftLegAPos;
                Vector3 studentLeftLegNormal = Vector3.Cross(studentLeftLegAB, studentLeftLegAC);
                studentLeftLegNormal.Normalize();
                double dotLeftLegProduct = Vector3.Dot(masterLeftLegNormal, studentLeftLegNormal);
                double magnitudeMasterLeftLeg = Math.Sqrt(masterLeftLegNormal.x * masterLeftLegNormal.x + masterLeftLegNormal.y * masterLeftLegNormal.y + masterLeftLegNormal.z * masterLeftLegNormal.z);
                double magnitudeStudentLeftLeg = Math.Sqrt(studentLeftLegNormal.x * studentLeftLegNormal.x + studentLeftLegNormal.y * studentLeftLegNormal.y + studentLeftLegNormal.z * studentLeftLegNormal.z);

                double angleLeftLegRadians = Math.Acos(dotLeftLegProduct / (magnitudeMasterLeftLeg * magnitudeStudentLeftLeg));
                double angleCosDataLeftLeg = dotLeftLegProduct / (magnitudeMasterLeftLeg * magnitudeStudentLeftLeg);
                // 将弧度转换为度
                double leftLegAngleDegrees = angleLeftLegRadians * (180.0 / Math.PI);

                //右腿平面计算相似度
                masterRightLegAB = masterRightLegBPos - masterRightLegAPos;
                masterRightLegAC = masterRightLegCPos - masterRightLegAPos;
                Vector3 masterRightLegNormal = Vector3.Cross(masterRightLegAB, masterRightLegAC);
                masterRightLegNormal.Normalize();
                studentRightLegAB = studentRightLegBPos - studentRightLegAPos;
                studentRightLegAC = studentRightLegCPos - studentRightLegAPos;
                Vector3 studentRightLegNormal = Vector3.Cross(studentRightLegAB, studentRightLegAC);
                studentRightLegNormal.Normalize();
                double dotRightLegProduct = Vector3.Dot(masterRightLegNormal, studentRightLegNormal);
                double magnitudeMasterRightLeg = Math.Sqrt(masterRightLegNormal.x * masterRightLegNormal.x + masterRightLegNormal.y * masterRightLegNormal.y + masterRightLegNormal.z * masterRightLegNormal.z);
                double magnitudeStudentRightLeg = Math.Sqrt(studentRightLegNormal.x * studentRightLegNormal.x + studentRightLegNormal.y * studentRightLegNormal.y + studentRightLegNormal.z * studentRightLegNormal.z);

                double angleRightLegRadians = Math.Acos(dotRightLegProduct / (magnitudeMasterRightLeg * magnitudeStudentRightLeg));
                double angleCosDataRightLeg = dotRightLegProduct / (magnitudeMasterRightLeg * magnitudeStudentRightLeg);
                // 将弧度转换为度
                double RightLegAngleDegrees = angleRightLegRadians * (180.0 / Math.PI);

                //躯干平面计算相似度
                masterSpineAB = masterSpineBPos - masterSpineAPos;
                masterSpineAC = masterSpineCPos - masterSpineAPos;
                Vector3 masterSpineNormal = Vector3.Cross(masterSpineAB, masterSpineAC);
                masterSpineNormal.Normalize();
                studentSpineAB = studentSpineBPos - studentSpineAPos;
                studentSpineAC = studentSpineCPos - studentSpineAPos;
                Vector3 studentSpineNormal = Vector3.Cross(studentSpineAB, studentSpineAC);
                studentRightLegNormal.Normalize();
                double dotSpineProduct = Vector3.Dot(masterSpineNormal, studentSpineNormal);
                double magnitudeMasterSpine = Math.Sqrt(masterSpineNormal.x * masterSpineNormal.x + masterSpineNormal.y * masterSpineNormal.y + masterSpineNormal.z * masterSpineNormal.z);
                double magnitudeStudentSpine = Math.Sqrt(studentSpineNormal.x * studentSpineNormal.x + studentSpineNormal.y * studentSpineNormal.y + studentSpineNormal.z * studentSpineNormal.z);

                double angleSpineRadians = Math.Acos(dotSpineProduct / (magnitudeMasterSpine * magnitudeStudentSpine));
                double angleCosDataSpine = dotSpineProduct / (magnitudeMasterSpine * magnitudeStudentSpine);
                // 将弧度转换为度
                double SpineAngleDegrees = angleSpineRadians * (180.0 / Math.PI);

                double featurePlaneScore = (angleCosDataLeftArm + angleCosDataLeftLeg + angleCosDataRightArm + angleCosDataRightLeg + angleCosDataSpine) / 5;
                //直接用五个cosdata求和平均后的值就可以了
                featurePlaneTextField.text = "特征平面得分为 " + featurePlaneScore.ToString("F2") + "\n"+ "左臂平面得分: " + angleCosDataLeftArm.ToString("F2") + "\n" + 
                    "左腿平面得分: " + angleCosDataLeftLeg.ToString("F2") +  "\n" + "脊柱平面得分:" + angleCosDataSpine.ToString("F2") + "\n" + "右臂平面得分: " + angleCosDataRightArm.ToString("F2")+
                    "\n" + "右腿平面得分: " + angleCosDataRightLeg.ToString("F2");
                Debug.Log("leftArm: " + angleCosDataLeftArm.ToString() + "  leftLeg: " + angleCosDataLeftLeg.ToString() + " Spine :" + angleCosDataSpine.ToString());
                Debug.Log("rightArm : " + angleCosDataRightArm.ToString() + " rightLeg: " + angleCosDataRightLeg.ToString());
            }
        }
    }
}
