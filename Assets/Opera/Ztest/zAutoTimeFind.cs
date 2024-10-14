using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Linq;
using System;

public class zAutoTimeFind : MonoBehaviour
{
    public float masterStartTime;
    public float masterEndTime;
    public InputField master_start_field;//master
    public InputField master_end_field;//master
    public float studentStartTime;
    public float studentEndTime;
    public InputField student_s_field;
    public InputField student_e_field;
    StreamWriter writer;
    StreamReader reader;

    public Text dtwText;//angleScore显示组件

    public InputField masterDtwTime_field;//自动识别对应时间的输入框
    public float masterDtwTime;
    public Text studentAutoDtwTime;//计算出来的对应时间


    private Dictionary<string, List<Vector3>> masterData;
    private Dictionary<string, List<Vector3>> studentData;
    private Dictionary<string, List<float>> masterAngleGapData;

    private Dictionary<float, float> masterDtwData;
    private Dictionary<float, float> studentDtwData;
    // Start is called before the first frame update
    void Start()
    {
        dtwText.text = null;
        studentAutoDtwTime.text = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (master_start_field.text == "")
        {
            masterStartTime = 0.0f;
        }
        else
        {
            masterStartTime = float.Parse(master_start_field.text);
        }

        if (master_end_field.text == "")
        {
            masterEndTime = 0.0f;
        }
        else
        {
            masterEndTime = float.Parse(master_end_field.text);
        }

        if (student_s_field.text == "")
        {
            studentStartTime = 0.0f;
        }
        else
        {
            studentStartTime = float.Parse(student_s_field.text);
        }

        if (student_e_field.text == "")
        {
            studentEndTime = 0.0f;
        }
        else
        {
            studentEndTime = float.Parse(student_e_field.text);
        }


        //静态动作对比时间更新检测
        //if (masterStaticTime.text == "")
        //{
        //    masterStaticTimeData = 0.0f;
        //}
        //else
        //{
        //    masterStaticTimeData = float.Parse(masterStaticTime.text);
        //}
        //if (studentStaticTime.text == "")
        //{
        //    studentStaticTimeData = 0.0f;
        //}
        //else
        //{
        //    studentStaticTimeData = float.Parse(studentStaticTime.text);
        //}


        //dtw对应时间检测
        if (masterDtwTime_field.text == "")
        {
            masterDtwTime = 0.0f;
        }
        else
        {
            masterDtwTime = float.Parse(masterDtwTime_field.text);
        }
    }
    public class dtwAngle
    {
        public float Time { get; set; }
        public float Value { get; set; }
    }
    public int newGetMinNeighbourIndex1(float[,] dtwMartix, int row, int col)
    {
        float minValue = Mathf.Min(dtwMartix[row - 1, col], dtwMartix[row, col - 1], dtwMartix[row - 1, col - 1]);
        if (dtwMartix[row - 1, col] == minValue)
        {
            return 0;
        }
        else if (dtwMartix[row, col - 1] == minValue)
        {
            return 1;
        }
        else
        {
            return 2;
        }

    }
    public void newDtwTest()//24.03.03 用的这个
    {
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        //master time
        float interval = masterEndTime - masterStartTime;//选段的间隔
        int TimeCount = (int)(interval / 0.1f);//次数

        //student time

        float studentInterval = studentEndTime - studentStartTime;//选段的间隔
        int studentTimeCount = (int)(studentInterval / 0.1f);//次数



        Dictionary<string, List<dtwAngle>> masterJointSequences = new Dictionary<string, List<dtwAngle>>();//存储关节点名字和时间角度序列的字典
        Dictionary<string, List<dtwAngle>> studentJointSequences = new Dictionary<string, List<dtwAngle>>();

        Dictionary<string, double> jointWeights = new Dictionary<string, double>();//存储关节点权重的字典

        if (vfplayer.ch_master.createdGA_No != -1)
        {
            if (vfplayer.ch_student.createdGA_No != -1)
            {
                int masterBoneCount = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones.Count;
                for (int i = 0; i < masterBoneCount; i++)
                {

                    JAnimationData.BoneType masterBonetype = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones[i].boneType;
                    string masterStr = masterBonetype.ToString();
                    int index = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.GetTypeIndex(masterBonetype);
                    int studentIndex = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.GetTypeIndex(masterBonetype);
                    float curTime = masterStartTime;
                    float studentCurTime = studentStartTime;
                    if (index != -1)
                    {
                        if (masterBonetype == JAnimationData.BoneType.leftToeBase) continue;
                        if (masterBonetype == JAnimationData.BoneType.rightToeBase) continue;
                        if (masterBonetype == JAnimationData.BoneType.leftFoot) continue;
                        if (masterBonetype == JAnimationData.BoneType.rightFoot) continue;

                        //刺虎手指节点跳过
                        if (masterBonetype == JAnimationData.BoneType.rMid1) continue;
                        if (masterBonetype == JAnimationData.BoneType.rMid2) continue;
                        if (masterBonetype == JAnimationData.BoneType.rMid3) continue;
                        if (masterBonetype == JAnimationData.BoneType.rIndex1) continue;
                        if (masterBonetype == JAnimationData.BoneType.rIndex2) continue;
                        if (masterBonetype == JAnimationData.BoneType.rIndex3) continue;
                        if (masterBonetype == JAnimationData.BoneType.rPinky1) continue;
                        if (masterBonetype == JAnimationData.BoneType.rPinky2) continue;
                        if (masterBonetype == JAnimationData.BoneType.rPinky3) continue;
                        if (masterBonetype == JAnimationData.BoneType.rRing1) continue;
                        if (masterBonetype == JAnimationData.BoneType.rRing2) continue;
                        if (masterBonetype == JAnimationData.BoneType.rRing3) continue;
                        if (masterBonetype == JAnimationData.BoneType.rSmallToe1) continue;
                        if (masterBonetype == JAnimationData.BoneType.rSmallToe2) continue;
                        if (masterBonetype == JAnimationData.BoneType.rSmallToe3) continue;
                        if (masterBonetype == JAnimationData.BoneType.rSmallToe4) continue;
                        if (masterBonetype == JAnimationData.BoneType.rThumb1) continue;
                        if (masterBonetype == JAnimationData.BoneType.rThumb2) continue;
                        if (masterBonetype == JAnimationData.BoneType.rThumb3) continue;
                        if (masterBonetype == JAnimationData.BoneType.rCarpal1) continue;
                        if (masterBonetype == JAnimationData.BoneType.rCarpal2) continue;
                        if (masterBonetype == JAnimationData.BoneType.rCarpal3) continue;
                        if (masterBonetype == JAnimationData.BoneType.unknowType) continue;
                        if (masterBonetype == JAnimationData.BoneType.lCarpal1) continue;
                        if (masterBonetype == JAnimationData.BoneType.lCarpal2) continue;
                        if (masterBonetype == JAnimationData.BoneType.lCarpal3) continue;
                        if (masterBonetype == JAnimationData.BoneType.lCarpal4) continue;
                        if (masterBonetype == JAnimationData.BoneType.lIndex1) continue;
                        if (masterBonetype == JAnimationData.BoneType.lIndex2) continue;
                        if (masterBonetype == JAnimationData.BoneType.lIndex3) continue;
                        if (masterBonetype == JAnimationData.BoneType.lMid1) continue;
                        if (masterBonetype == JAnimationData.BoneType.lMid2) continue;
                        if (masterBonetype == JAnimationData.BoneType.lMid3) continue;
                        if (masterBonetype == JAnimationData.BoneType.lPinky1) continue;
                        if (masterBonetype == JAnimationData.BoneType.lPinky2) continue;
                        if (masterBonetype == JAnimationData.BoneType.lPinky3) continue;
                        if (masterBonetype == JAnimationData.BoneType.lRing1) continue;
                        if (masterBonetype == JAnimationData.BoneType.lRing2) continue;
                        if (masterBonetype == JAnimationData.BoneType.lRing3) continue;
                        if (masterBonetype == JAnimationData.BoneType.lSmallToe1) continue;
                        if (masterBonetype == JAnimationData.BoneType.lSmallToe2) continue;
                        if (masterBonetype == JAnimationData.BoneType.lSmallToe3) continue;
                        if (masterBonetype == JAnimationData.BoneType.lSmallToe4) continue;
                        if (masterBonetype == JAnimationData.BoneType.lThumb1) continue;
                        if (masterBonetype == JAnimationData.BoneType.lThumb2) continue;
                        if (masterBonetype == JAnimationData.BoneType.lThumb3) continue;

                        //Debug.Log("index: " + index + " studentIndex: " + studentIndex);
                        List<dtwAngle> masterSequence = new List<dtwAngle>();//存储角度序列
                        List<dtwAngle> studentSequence = new List<dtwAngle>();

                        //master存储角度序列
                        JVehicleFrameSystem.BoneFramework.Bone masterTargetBone = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.GetBoneByType(masterBonetype);
                        int childNum = masterTargetBone.ChildsList.Count;
                        if (childNum == 1)
                        {

                            for (int j = 0; j < TimeCount; j++)
                            {
                                float curMasterBoneAngle = vfplayer.ch_master.bm_p1.GetWorldRoa_Ztest(curTime, masterBonetype, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                masterSequence.Add(new dtwAngle { Time = curTime, Value = curMasterBoneAngle });
                                curTime += 0.1f;
                            }
                            string masterBoneStr = masterBonetype.ToString();

                            masterJointSequences[masterBoneStr] = new List<dtwAngle>(masterSequence);
                            jointWeights[masterBoneStr] = 1.0;
                        }
                        masterSequence.Clear();
                        //student存储角度序列
                        JVehicleFrameSystem.BoneFramework.Bone studentTargetBone = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.GetBoneByType(masterBonetype);
                        int studentChildNum = studentTargetBone.ChildsList.Count;
                        if (studentChildNum == 1)
                        {

                            for (int j = 0; j < studentTimeCount; j++)
                            {
                                float curStudentBoneAngle = vfplayer.ch_student.bm_p1.GetWorldRoa_Ztest(studentCurTime, masterBonetype, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                                studentSequence.Add(new dtwAngle { Time = studentCurTime, Value = curStudentBoneAngle });
                                studentCurTime += 0.1f;
                            }
                            string studentBoneStr = masterBonetype.ToString();
                            studentJointSequences[studentBoneStr] = new List<dtwAngle>(studentSequence);

                        }
                        studentSequence.Clear();
                    }
                }

            }
        }

        //有了两个序列之后进行DTW矩阵的构建和权重设置以及综合
        Dictionary<string, float[,]> distanceMatrices = new Dictionary<string, float[,]>();

        foreach (var jointName in masterJointSequences.Keys)
        {
            if (studentJointSequences.ContainsKey(jointName))
            {
                var masterSequence = masterJointSequences[jointName];
                var studentSequence = studentJointSequences[jointName];
                int mn = masterSequence.Count;
                int sm = studentSequence.Count;
                float[,] dtw = new float[mn, sm];

                //初始化dtw矩阵
                for (int i = 1; i < mn; i++)
                {
                    dtw[i, 0] = float.MaxValue;
                }
                for (int j = 1; j < sm; j++)
                {
                    dtw[0, j] = float.MaxValue;
                }
                dtw[0, 0] = 0;

                //计算每个关节点各自的dtw距离矩阵
                for (int i = 1; i < mn; i++)
                {
                    for (int j = 1; j < sm; j++)
                    {
                        float cost = Mathf.Abs(masterSequence[i - 1].Value - studentSequence[j - 1].Value);
                        dtw[i, j] = cost + Mathf.Min(dtw[i - 1, j - 1], Mathf.Min(dtw[i, j - 1], dtw[i - 1, j]));
                    }
                }
                distanceMatrices[jointName] = dtw;

            }
        }

        //计算综合矩阵
        int n = distanceMatrices.First().Value.GetLength(0);
        int m = distanceMatrices.First().Value.GetLength(1);
        float[,] weightedAverageMatrix = new float[n, m];

        foreach (var item in distanceMatrices)
        {
            string jointName = item.Key;
            float[,] matrix = item.Value;
            //float weight = jointWeights.ContainsKey(jointName) ? jointWeights[jointName] : 1.0f; // 如果没有指定权重，默认为1

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    weightedAverageMatrix[i, j] += matrix[i, j];
                }
            }
        }

        // 寻找最佳路径
        int row = n - 1;
        int col = m - 1;
        List<Tuple<int, int>> bestPath = new List<Tuple<int, int>> { Tuple.Create(row, col) };
        var firstJointName = masterJointSequences.Keys.First();
        string timeStampPath1 = Application.dataPath + "/zTestData/dtwTimeStampData.txt";
        List<KeyValuePair<float, float>> dtwTimePairs = new List<KeyValuePair<float, float>>();//存储dtw计算出来的时间点对




        while (row > 0 && col > 0)
        {
            int minNeighbourIndex = newGetMinNeighbourIndex1(weightedAverageMatrix, row, col);
            if (minNeighbourIndex == 0)
            {
                row--;
            }
            else if (minNeighbourIndex == 1)
            {
                col--;
            }
            else
            {
                row--;
                col--;
            }

            string masterTime = masterJointSequences[firstJointName][row].Time.ToString("F2");
            string studentTime = studentJointSequences[firstJointName][col].Time.ToString("F2");

            float masterDtwTime = masterJointSequences[firstJointName][row].Time;
            float studentDtwTime = studentJointSequences[firstJointName][col].Time;

            dtwTimePairs.Add(new KeyValuePair<float, float>(masterDtwTime, studentDtwTime));
            string sumStr = masterTime + " " + studentTime;
            string[] fileStr = { sumStr };
            File.AppendAllLines(timeStampPath1, fileStr);
        }
        //calculateDtwDiff(dtwTimePairs);//处理dtw时间序列并计算 角度作为参数
        dtwText.text = "两段动作序列已经完成时间对齐操作";
        //featurePlaneDtwScore(dtwTimePairs);//特征平面为参数
        Debug.Log("bones dtw time Series has been writed in " + timeStampPath1);

    }

    public class psoJointWeights
    {
        Dictionary<JAnimationData.BoneType, float> jointWeights = new Dictionary<JAnimationData.BoneType, float>();

    }

    public void calculateDtwDiff(List<KeyValuePair<float, float>> timePairs)//处理dtw时间序列
    {
        //关节点权重设置
        Dictionary<JAnimationData.BoneType, float> jointWeights = new Dictionary<JAnimationData.BoneType, float>();
        string boneDiffPath = Application.dataPath + "/zTestData/dtwBoneDiffData.txt";
        //起霸24个关节点权重设置
        jointWeights[JAnimationData.BoneType.spine] = 1.77f;
        jointWeights[JAnimationData.BoneType.spine1] = 0f;
        jointWeights[JAnimationData.BoneType.spine2] = 1.80f;
        jointWeights[JAnimationData.BoneType.neck] = 1.49f;
        jointWeights[JAnimationData.BoneType.neck1] = 0f;
        jointWeights[JAnimationData.BoneType.rightShoulder] = 0.97f;
        jointWeights[JAnimationData.BoneType.rightArm] = 0f;
        jointWeights[JAnimationData.BoneType.rightForeArm] = 0.26f;
        jointWeights[JAnimationData.BoneType.leftShoulder] = 1.66f;
        jointWeights[JAnimationData.BoneType.leftArm] = 2f;
        jointWeights[JAnimationData.BoneType.leftForeArm] = 2f;
        jointWeights[JAnimationData.BoneType.rightUpLeg] = 2f;
        jointWeights[JAnimationData.BoneType.rightLeg] = 2f;
        jointWeights[JAnimationData.BoneType.leftUpLeg] = 0f;
        jointWeights[JAnimationData.BoneType.leftLeg] = 0f;

        //关节点角度不会计算到
        jointWeights[JAnimationData.BoneType.head] = 1.0f;
        jointWeights[JAnimationData.BoneType.spine3] = 1.0f;
        jointWeights[JAnimationData.BoneType.leftHand] = 1.0f;
        jointWeights[JAnimationData.BoneType.rightHand] = 1.0f;
        jointWeights[JAnimationData.BoneType.hips] = 1.0f;
        jointWeights[JAnimationData.BoneType.leftFoot] = 1.0f;
        jointWeights[JAnimationData.BoneType.leftToeBase] = 1.0f;
        jointWeights[JAnimationData.BoneType.rightFoot] = 1.0f;
        jointWeights[JAnimationData.BoneType.rightToeBase] = 1.0f;
        //问樵补充
        jointWeights[JAnimationData.BoneType.rThumb1] = 1.0f;
        jointWeights[JAnimationData.BoneType.rMid1] = 1.0f;
        jointWeights[JAnimationData.BoneType.lThumb1] = 1.0f;
        jointWeights[JAnimationData.BoneType.lMid1] = 1.0f;
        jointWeights[JAnimationData.BoneType.rightForeFoot] = 1.0f;
        jointWeights[JAnimationData.BoneType.rightToeBaseEnd] = 1.0f;
        jointWeights[JAnimationData.BoneType.leftForeFoot] = 1.0f;
        jointWeights[JAnimationData.BoneType.leftToeBaseEnd] = 1.0f;


        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        int timePairsCount = 0;
        float allBoneAngleDiff = 0.0f;
        float allScoreDiff = 0.0f;
        Debug.Log("master num is :" + vfplayer.ch_master.createdGA_No);
        Debug.Log("master num is :" + vfplayer.ch_student.createdGA_No);
        foreach (var pair in timePairs)
        {
            timePairsCount++;
            float sumBoneAngleDiff = 0.0f;
            float sumScoreDiff = 0.0f;
            if (vfplayer.ch_master.createdGA_No != -1)
            {
                if (vfplayer.ch_student.createdGA_No != -1)
                {

                    int boneCount = 0;
                    for (int j = 0; j < vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones.Count; j++)
                    {

                        JAnimationData.BoneType CurBonetype = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones[j].boneType;
                        int index = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.GetTypeIndex(CurBonetype);
                        if (index != -1)
                        {
                            JVehicleFrameSystem.BoneFramework.Bone targetBone = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.GetBoneByType(CurBonetype);
                            int childNum = targetBone.ChildsList.Count;
                            if (CurBonetype == JAnimationData.BoneType.leftToeBase) continue;
                            if (CurBonetype == JAnimationData.BoneType.rightToeBase) continue;
                            if (CurBonetype == JAnimationData.BoneType.leftFoot) continue;
                            if (CurBonetype == JAnimationData.BoneType.rightFoot) continue;

                            //刺虎手指节点跳过
                            if (CurBonetype == JAnimationData.BoneType.rMid1) continue;
                            if (CurBonetype == JAnimationData.BoneType.rMid2) continue;
                            if (CurBonetype == JAnimationData.BoneType.rMid3) continue;
                            if (CurBonetype == JAnimationData.BoneType.rIndex1) continue;
                            if (CurBonetype == JAnimationData.BoneType.rIndex2) continue;
                            if (CurBonetype == JAnimationData.BoneType.rIndex3) continue;
                            if (CurBonetype == JAnimationData.BoneType.rPinky1) continue;
                            if (CurBonetype == JAnimationData.BoneType.rPinky2) continue;
                            if (CurBonetype == JAnimationData.BoneType.rPinky3) continue;
                            if (CurBonetype == JAnimationData.BoneType.rRing1) continue;
                            if (CurBonetype == JAnimationData.BoneType.rRing2) continue;
                            if (CurBonetype == JAnimationData.BoneType.rRing3) continue;
                            if (CurBonetype == JAnimationData.BoneType.rSmallToe1) continue;
                            if (CurBonetype == JAnimationData.BoneType.rSmallToe2) continue;
                            if (CurBonetype == JAnimationData.BoneType.rSmallToe3) continue;
                            if (CurBonetype == JAnimationData.BoneType.rSmallToe4) continue;
                            if (CurBonetype == JAnimationData.BoneType.rThumb1) continue;
                            if (CurBonetype == JAnimationData.BoneType.rThumb2) continue;
                            if (CurBonetype == JAnimationData.BoneType.rThumb3) continue;
                            if (CurBonetype == JAnimationData.BoneType.rCarpal1) continue;
                            if (CurBonetype == JAnimationData.BoneType.rCarpal2) continue;
                            if (CurBonetype == JAnimationData.BoneType.rCarpal3) continue;
                            if (CurBonetype == JAnimationData.BoneType.unknowType) continue;
                            if (CurBonetype == JAnimationData.BoneType.lCarpal1) continue;
                            if (CurBonetype == JAnimationData.BoneType.lCarpal2) continue;
                            if (CurBonetype == JAnimationData.BoneType.lCarpal3) continue;
                            if (CurBonetype == JAnimationData.BoneType.lCarpal4) continue;
                            if (CurBonetype == JAnimationData.BoneType.lIndex1) continue;
                            if (CurBonetype == JAnimationData.BoneType.lIndex2) continue;
                            if (CurBonetype == JAnimationData.BoneType.lIndex3) continue;
                            if (CurBonetype == JAnimationData.BoneType.lMid1) continue;
                            if (CurBonetype == JAnimationData.BoneType.lMid2) continue;
                            if (CurBonetype == JAnimationData.BoneType.lMid3) continue;
                            if (CurBonetype == JAnimationData.BoneType.lPinky1) continue;
                            if (CurBonetype == JAnimationData.BoneType.lPinky2) continue;
                            if (CurBonetype == JAnimationData.BoneType.lPinky3) continue;
                            if (CurBonetype == JAnimationData.BoneType.lRing1) continue;
                            if (CurBonetype == JAnimationData.BoneType.lRing2) continue;
                            if (CurBonetype == JAnimationData.BoneType.lRing3) continue;
                            if (CurBonetype == JAnimationData.BoneType.lSmallToe1) continue;
                            if (CurBonetype == JAnimationData.BoneType.lSmallToe2) continue;
                            if (CurBonetype == JAnimationData.BoneType.lSmallToe3) continue;
                            if (CurBonetype == JAnimationData.BoneType.lSmallToe4) continue;
                            if (CurBonetype == JAnimationData.BoneType.lThumb1) continue;
                            if (CurBonetype == JAnimationData.BoneType.lThumb2) continue;
                            if (CurBonetype == JAnimationData.BoneType.lThumb3) continue;

                            if (childNum == 1)
                            {

                                float masterBoneAngle = vfplayer.ch_master.bm_p1.GetWorldRoa_Ztest(pair.Key, CurBonetype, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                // Debug.Log("vfplayer.ch_master.createdGA.transform.position : " + vfplayer.ch_master.createdGA.transform.position);
                                // Debug.Log("vfplayer.ch_master.createdGA.transform.rotataion : " + vfplayer.ch_master.createdGA.transform.rotation);
                                float studentBoneAngle = vfplayer.ch_student.bm_p1.GetWorldRoa_Ztest(pair.Value, CurBonetype, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                                //Debug.Log("vfplayer.ch_student.createdGA.transform.position : " + vfplayer.ch_student.createdGA.transform.position);
                                //Debug.Log("vfplayer.ch_master.createdGA.transform.rotataion : " + vfplayer.ch_student.createdGA.transform.rotation);
                                float boneAngleDiff = Mathf.Abs(masterBoneAngle - studentBoneAngle);
                                float scoreDiff = jointWeights[CurBonetype] * boneAngleDiff / masterBoneAngle;//记录打分
                                scoreDiff = (1 - scoreDiff);
                                boneAngleDiff = boneAngleDiff * jointWeights[CurBonetype];
                                sumBoneAngleDiff += boneAngleDiff;
                                sumScoreDiff += scoreDiff;
                                boneCount++;
                            }

                        }
                        else
                        {
                            continue;
                        }

                    }
                    sumScoreDiff = sumScoreDiff / boneCount;//平均化打分
                    Debug.Log("bonecount : " + boneCount);
                }
            }
            string sumStr = sumBoneAngleDiff.ToString();
            string[] fileStr = { sumStr };
            File.AppendAllLines(boneDiffPath, fileStr);
            allBoneAngleDiff += sumBoneAngleDiff;
            allScoreDiff += sumScoreDiff;
        }
        float score = allBoneAngleDiff / timePairsCount;
        float percentScore = 100 * (allScoreDiff / timePairsCount);
        string wholeStr = "all bone time series Diff Data is : " + score.ToString();
        string allScoreStr = " all bone score is : " + percentScore.ToString();
        string[] wholeFileStr = { wholeStr, " ", allScoreStr };
        File.AppendAllLines(boneDiffPath, wholeFileStr);
        Debug.Log("bones dtw time Series angle Diff Data has been writed in " + boneDiffPath);
    }

    public class autoDtwTimeData
    {
        public float masterTime { get; set; }
        public float studentTime { get; set; }
    }
    public class TimeDataComparer : IComparer<autoDtwTimeData>
    {
        public int Compare(autoDtwTimeData x, autoDtwTimeData y)
        {
            return x.masterTime.CompareTo(y.masterTime);
        }
    }
    public class TimeDataFinder
    {
        private List<autoDtwTimeData> _timeDataList;
        public TimeDataFinder(List<autoDtwTimeData> timeDataList)
        {
            _timeDataList = timeDataList;
        }
        public float FindClosestStudentTime(List<autoDtwTimeData> dataList, float inputMasterTime)
        {
            float closestStudentTime = dataList[0].studentTime; // Initialize with the first student time
            float minDifference = Math.Abs(dataList[0].masterTime - inputMasterTime); // Initial difference

            foreach (var data in dataList)
            {
                float currentDifference = Math.Abs(data.masterTime - inputMasterTime);
                if (currentDifference < minDifference)
                {
                    minDifference = currentDifference;
                    closestStudentTime = data.studentTime;
                }
            }

            return closestStudentTime;
        }
    }
        public void autoDtwTime()//自动识别dtw对应时间
        {
            var dataList = new List<autoDtwTimeData>();
            string timeStampPath1 = Application.dataPath + "/zTestData/dtwTimeStampData.txt";
            var lines = File.ReadAllLines(timeStampPath1);
            foreach (var line in lines)
            {
                var parts = line.Split(' ');
                var filteredParts = parts.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray(); // 手动过滤空字符串
                if (filteredParts.Length == 2)
                {
                    if (float.TryParse(filteredParts[0], out float masterTime) && float.TryParse(filteredParts[1], out float studentTime))
                    {
                        dataList.Add(new autoDtwTimeData { masterTime = masterTime, studentTime = studentTime });
                    }
                }
            }
            var finder = new TimeDataFinder(dataList);
            float closestStudentTime = finder.FindClosestStudentTime(dataList, masterDtwTime);
            studentAutoDtwTime.text = "学生动作对应的时间为" + closestStudentTime.ToString("F2");
        }
    }
