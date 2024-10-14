using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Linq;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class ZPythonUnityTest : MonoBehaviour
{
   

    private Dictionary<string, List<Vector3>> masterData;
    private Dictionary<string, List<Vector3>> studentData;
    private Dictionary<string, List<float>> masterAngleGapData;

    private Dictionary<float, float> masterDtwData;
    private Dictionary<float, float> studentDtwData;
    // Start is called before the first frame update
    private void Start()
    {

        //Slider slider = GetComponent<Slider>();
        //slider.onValueChanged.AddListener(OnValueChanged);
    }

    public void OnValueChanged(float value)
    {
        Debug.Log("onvaluechanged value:" + value);
    }
    // Update is called once per frame
    void Update()
    {
        

    }

    public class dtwAngle
    {
        public float Time { get; set; }
        public float Value { get; set; }
    }

    public class pyUnityScores {
        public float[] scores;
    }

    [Serializable]
    public class Particle
    {
        public int index;
        public List<float> weights;
    }

    [Serializable]
    public class ParticlesData
    {
        public List<Particle> particles;
    }
    
    [Serializable]
    public class ParticleScores
    {
        public int particleIndex;
        public List<float> actionScores = new List<float>();
    }

    [Serializable]
    public class SimulationResults
    {
        public List<ParticleScores> particleScores = new List<ParticleScores>();
    }

    public void AngleDiffDataCal()
    {
        string fileJsonPath = Application.dataPath + "/zTestData/BoneAngle.json";

        // 检查文件是否存在
        if (File.Exists(fileJsonPath))
        {
            // 如果存在，则删除文件
            File.Delete(fileJsonPath);
        }

        // 创建一个新的空 JSON 文件，初始化为一个空对象
        File.WriteAllText(fileJsonPath, "{}");


        List<KeyValuePair<float, float>> timePairs_1 = newDtwTest(18f, 28f, 21f, 30f);
        List<KeyValuePair<float, float>> timePairs_2 = newDtwTest(30f, 62f, 30f, 64f);
        List<KeyValuePair<float, float>> timePairs_3 = newDtwTest(62f, 87f, 65f, 90f);
        List<KeyValuePair<float, float>> timePairs_4 = newDtwTest(87f, 103f, 90f, 104f);
        List<KeyValuePair<float, float>> timePairs_5 = newDtwTest(103f, 120f, 105f, 123f);
        List<KeyValuePair<float, float>> timePairs_6 = newDtwTest(120f, 141f, 123f, 140f);
        calculateAngleDiff(timePairs_1, 1);
        calculateAngleDiff(timePairs_2, 2);
        calculateAngleDiff(timePairs_3, 3);
        calculateAngleDiff(timePairs_4, 4);
        calculateAngleDiff(timePairs_5, 5);
        calculateAngleDiff(timePairs_6, 6);
    }
    public void qibaScoreCal()
    {
        string particlesJsonFilePath = "D:/python_test/pythonProject2/pso/particles_weights.json";
        string weightsJsonData = File.ReadAllText(particlesJsonFilePath);
        ParticlesData particlesData = JsonUtility.FromJson<ParticlesData>(weightsJsonData);
        Debug.Log(weightsJsonData);
        if (particlesData == null)
        {
            Debug.LogError("Failed to deserialize JSON data.");
        }
        else if (particlesData.particles == null)
        {
            Debug.LogError("particlesData.particles is null.");
        }
        else
        {
            Debug.Log($"Found {particlesData.particles.Count} particles.");
        }

     
        SimulationResults results = new SimulationResults();
        for (int i = 0; i < particlesData.particles.Count; i++)
        {
            Particle particle = particlesData.particles[i];
            Debug.Log("particle index" + particle.index + " " + particle.weights);
            ParticleScores particleScores = new ParticleScores();
            particleScores.particleIndex = i;
            particleScores.actionScores = calculateScore(particle.weights);
            results.particleScores.Add(particleScores);
        }


        string filepath = Application.dataPath + "/zTestData/pyUnityscores.json";
        string jsonScoreDataJson = JsonUtility.ToJson(results, true);
        File.WriteAllText(filepath, jsonScoreDataJson);
        Debug.Log(jsonScoreDataJson);
        Debug.Log("scores written to :" + filepath);

    }
    public class angleDiffTimeSeriesData
    {
        
        public Dictionary<int, List<float>> timeSeriesAngleDiffData = new Dictionary<int, List<float>>();
      

        public void AddSeriesData(int seriesIndex,List<float> data)
        {
            timeSeriesAngleDiffData[seriesIndex] = data;
        }
    }

    public Dictionary<JAnimationData.BoneType, angleDiffTimeSeriesData> allBonesData = new Dictionary<JAnimationData.BoneType, angleDiffTimeSeriesData>();
    public void calculateAngleDiff(List<KeyValuePair<float, float>> timePairs, int timePairsIndex)
    {
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        allBonesData = new Dictionary<JAnimationData.BoneType, angleDiffTimeSeriesData>();

        VFPlayer_InGame.ShowingCharacter master = new VFPlayer_InGame.ShowingCharacter();
        master.builder = new JVehicleFrameSystem.VehicleBuilder(vfplayer.animShowList[3].frame.boneFramework);
        master.builder.Build(1);
        JAnimationSystem.BoneMatch masterbml = new JAnimationSystem.BoneMatch();
        masterbml.StartAutoMatch(master.builder.buildedGA.transform, vfplayer.animShowList[3].autodata, vfplayer.animShowList[3].jAnimData.jAD);
        master.bm_p1 = new JAnimationSystem.BoneMatch_Player(masterbml);
        master.bm_p1.SetJAnimationData(vfplayer.animShowList[3].jAnimData);

        VFPlayer_InGame.ShowingCharacter student = new VFPlayer_InGame.ShowingCharacter();
        student.builder = new JVehicleFrameSystem.VehicleBuilder(vfplayer.animShowList[4].frame.boneFramework);
        student.builder.Build(1);
        JAnimationSystem.BoneMatch studentbml = new JAnimationSystem.BoneMatch();
        studentbml.StartAutoMatch(student.builder.buildedGA.transform, vfplayer.animShowList[4].autodata, vfplayer.animShowList[4].jAnimData.jAD);
        student.bm_p1 = new JAnimationSystem.BoneMatch_Player(studentbml);
        student.bm_p1.SetJAnimationData(vfplayer.animShowList[4].jAnimData);




        int timePairsCount = 0;
        float allBoneAngleDiff = 0.0f;
        float allScoreDiff = 0.0f;
        Debug.Log("timepairs count: " + timePairs.Count);



        int boneCount = 0;
        for (int j = 0; j < vfplayer.animShowList[3].frame.boneFramework.Bones.Count; j++)
        {

            timePairsCount++;
            float sumBoneAngleDiff = 0.0f;
            float sumScoreDiff = 0.0f;

            JAnimationData.BoneType CurBonetype = vfplayer.animShowList[3].frame.boneFramework.Bones[j].boneType;
            int index = vfplayer.animShowList[3].jAnimData.jAD.GetTypeIndex(CurBonetype);
            if (index != -1)
            {
                JVehicleFrameSystem.BoneFramework.Bone targetBone = vfplayer.animShowList[3].frame.boneFramework.GetBoneByType(CurBonetype);
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


                Vector3 masterTransformPos = new Vector3(1.9f, 0, 0.7f);
                Quaternion masterTransformRoa = new Quaternion(0, 0, 0, 1);
                Vector3 studentTransformPos = new Vector3(-1.5f, -0.013f, -1.4f);
                Quaternion studentTransformRoa = new Quaternion(0, 0, 0, 1);
                List<float> angleDiffData = new List<float>();

                if (childNum == 1)
                {

                    foreach (var pair in timePairs)
                    {
                        float masterBoneAngle = master.bm_p1.GetWorldRoa_Ztest(pair.Key, CurBonetype, vfplayer.animShowList[3].frame.boneFramework, masterTransformPos, masterTransformRoa);
                        float studentBoneAngle = student.bm_p1.GetWorldRoa_Ztest(pair.Value, CurBonetype, vfplayer.animShowList[4].frame.boneFramework, studentTransformPos, studentTransformRoa);
                        float boneAngleDiff = Mathf.Abs(masterBoneAngle - studentBoneAngle);
                        boneAngleDiff = boneAngleDiff / masterBoneAngle;
                        angleDiffData.Add(boneAngleDiff);
                    }
                    if (!allBonesData.ContainsKey(CurBonetype))
                    {
                        allBonesData[CurBonetype] = new angleDiffTimeSeriesData();
                    }
                    allBonesData[CurBonetype].AddSeriesData(timePairsIndex, angleDiffData);



                    // Debug.Log("boneAngleDiff: " + CurBonetype + ": " + boneAngleDiff);
                    //float scoreDiff = jointWeights[CurBonetype] * boneAngleDiff / masterBoneAngle;//记录打分
                    //scoreDiff = 1 - scoreDiff;
                    //boneAngleDiff = boneAngleDiff * jointWeights[CurBonetype];
                    //sumBoneAngleDiff += boneAngleDiff;
                    //sumScoreDiff += scoreDiff;
                    boneCount++;
                }

            }
            else
            {
                continue;
            }
            updateJsonWithResults(allBonesData);
        }
    }
    public void updateJsonWithResults(Dictionary<JAnimationData.BoneType, angleDiffTimeSeriesData> allBonesData)
    {
        string fileJsonPath = Application.dataPath + "/zTestData/BoneAngle.json";
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter> { new StringEnumConverter() }
        };

        // 读取现有 JSON 数据或初始化为空 JSON 对象
        Dictionary<JAnimationData.BoneType, angleDiffTimeSeriesData> existingData = new Dictionary<JAnimationData.BoneType, angleDiffTimeSeriesData>();
        if (File.Exists(fileJsonPath))
        {
            string existingJson = File.ReadAllText(fileJsonPath).Trim();
            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                existingData = JsonConvert.DeserializeObject<Dictionary<JAnimationData.BoneType, angleDiffTimeSeriesData>>(existingJson);
            }
        }

        // 合并现有数据和新数据
        foreach (var boneType in allBonesData.Keys)
        {
            if (existingData.ContainsKey(boneType))
            {
                var existingBoneData = existingData[boneType];
                var newBoneData = allBonesData[boneType];
                foreach (var seriesIndex in newBoneData.timeSeriesAngleDiffData.Keys)
                {
                    if (existingBoneData.timeSeriesAngleDiffData.ContainsKey(seriesIndex))
                    {
                        //existingBoneData.timeSeriesAngleDiffData[seriesIndex].AddRange(newBoneData.timeSeriesAngleDiffData[seriesIndex]);
                    }
                    else
                    {
                        existingBoneData.timeSeriesAngleDiffData.Add(seriesIndex, newBoneData.timeSeriesAngleDiffData[seriesIndex]);
                    }
                }
            }
            else
            {
                existingData.Add(boneType, allBonesData[boneType]);
            }
        }

        // 序列化并写入最终的数据
        string updatedJson = JsonConvert.SerializeObject(existingData, settings);
        File.WriteAllText(fileJsonPath, updatedJson);
    }






    public List<KeyValuePair<float, float>> newDtwTest(float masterStartTime,float masterEndTime,float studentStartTime,float studentEndTime)//24.03.03 用的这个
    {
        //大师起霸num 3 学生起霸 num 4
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();

        //master time
        
        float masterInterval = masterEndTime - masterStartTime;//选段的间隔
        int masterTimeCount = (int)(masterInterval / 0.1f);//次数

        //student time
        
        float studentInterval = studentEndTime - studentStartTime;//选段的间隔
        int studentTimeCount = (int)(studentInterval / 0.1f);//次数

        Dictionary<string, List<dtwAngle>> masterJointSequences = new Dictionary<string, List<dtwAngle>>();//存储关节点名字和时间角度序列的字典
        Dictionary<string, List<dtwAngle>> studentJointSequences = new Dictionary<string, List<dtwAngle>>();

        Dictionary<string, double> jointWeights = new Dictionary<string, double>();//存储关节点权重的字典

        VFPlayer_InGame.ShowingCharacter master = new VFPlayer_InGame.ShowingCharacter();
        master.builder = new JVehicleFrameSystem.VehicleBuilder(vfplayer.animShowList[3].frame.boneFramework);
        master.builder.Build(1);
        JAnimationSystem.BoneMatch masterbml = new JAnimationSystem.BoneMatch();
        masterbml.StartAutoMatch(master.builder.buildedGA.transform, vfplayer.animShowList[3].autodata, vfplayer.animShowList[3].jAnimData.jAD);
        master.bm_p1 = new JAnimationSystem.BoneMatch_Player(masterbml);
        master.bm_p1.SetJAnimationData(vfplayer.animShowList[3].jAnimData);

        VFPlayer_InGame.ShowingCharacter student = new VFPlayer_InGame.ShowingCharacter();
        student.builder = new JVehicleFrameSystem.VehicleBuilder(vfplayer.animShowList[4].frame.boneFramework);
        student.builder.Build(1);
        student.builder.buildedGA.transform.rotation = Quaternion.Euler(vfplayer.animShowList[4].roa);
        JAnimationSystem.BoneMatch studentbml = new JAnimationSystem.BoneMatch();
        studentbml.StartAutoMatch(student.builder.buildedGA.transform, vfplayer.animShowList[4].autodata, vfplayer.animShowList[4].jAnimData.jAD);
        student.bm_p1 = new JAnimationSystem.BoneMatch_Player(studentbml);
        student.bm_p1.SetJAnimationData(vfplayer.animShowList[4].jAnimData);

        int masterBoneCount = vfplayer.animShowList[3].frame.boneFramework.Bones.Count;
        for (int i = 0; i < masterBoneCount; i++)
        {

            JAnimationData.BoneType masterBonetype = vfplayer.animShowList[3].frame.boneFramework.Bones[i].boneType;
            string masterStr = masterBonetype.ToString();
            int masterIndex = vfplayer.animShowList[3].jAnimData.jAD.GetTypeIndex(masterBonetype);
            int studentIndex = vfplayer.animShowList[4].jAnimData.jAD.GetTypeIndex(masterBonetype);
            float curTime = masterStartTime;
            float studentCurTime = studentStartTime;
            if (masterIndex != -1)
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

                // Debug.Log("index: " + masterIndex + " studentIndex: " + studentIndex);
                List<dtwAngle> masterSequence = new List<dtwAngle>();//存储角度序列
                List<dtwAngle> studentSequence = new List<dtwAngle>();

                //master存储角度序列
                Vector3 masterTransformPos = new Vector3(1.9f, 0, 0.7f);
                Quaternion masterTransformRoa = new Quaternion(0, 0, 0, 1);

                JVehicleFrameSystem.BoneFramework.Bone masterTargetBone = vfplayer.animShowList[3].frame.boneFramework.GetBoneByType(masterBonetype);


                int childNum = masterTargetBone.ChildsList.Count;
                if (childNum == 1)
                {

                    for (int j = 0; j < masterTimeCount; j++)
                    {
                        
                        float curMasterBoneAngle = master.bm_p1.GetWorldRoa_Ztest(curTime, masterBonetype, vfplayer.animShowList[3].frame.boneFramework, masterTransformPos, masterTransformRoa);
                        masterSequence.Add(new dtwAngle { Time = curTime, Value = curMasterBoneAngle });
                        curTime += 0.1f;
                    }
                    string masterBoneStr = masterBonetype.ToString();

                    masterJointSequences[masterBoneStr] = new List<dtwAngle>(masterSequence);
                    jointWeights[masterBoneStr] = 1.0;
                }
                masterSequence.Clear();
                //student存储角度序列
                JVehicleFrameSystem.BoneFramework.Bone studentTargetBone = vfplayer.animShowList[4].frame.boneFramework.GetBoneByType(masterBonetype);
                Vector3 studentTransformPos = new Vector3(-1.5f, 0f, -1.4f);
                Quaternion studentTransformRoa = new Quaternion(0, 0, 0, 1);

              
                int studentChildNum = studentTargetBone.ChildsList.Count;
                if (studentChildNum == 1)
                {

                    for (int j = 0; j < studentTimeCount; j++)
                    {
                        float curStudentBoneAngle = student.bm_p1.GetWorldRoa_Ztest(studentCurTime, masterBonetype, vfplayer.animShowList[4].frame.boneFramework, studentTransformPos, studentTransformRoa);
                        studentSequence.Add(new dtwAngle { Time = studentCurTime, Value = curStudentBoneAngle });
                        studentCurTime += 0.1f;
                    }
                    string studentBoneStr = masterBonetype.ToString();
                    studentJointSequences[studentBoneStr] = new List<dtwAngle>(studentSequence);

                }
                studentSequence.Clear();



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
        //string timeStampPath1 = Application.dataPath + "/zTestData/dtwTimeStampData.txt";
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

            string masterTime = masterJointSequences[firstJointName][row].Time.ToString();
            string studentTime = studentJointSequences[firstJointName][col].Time.ToString();

            float masterDtwTime = masterJointSequences[firstJointName][row].Time;
            float studentDtwTime = studentJointSequences[firstJointName][col].Time;

            dtwTimePairs.Add(new KeyValuePair<float, float>(masterDtwTime, studentDtwTime));
            string sumStr = masterTime + " " + studentTime;
            string[] fileStr = { sumStr };
            //File.AppendAllLines(timeStampPath1, fileStr);
        }
        return dtwTimePairs;
       
        //featurePlaneDtwScore(dtwTimePairs);//特征平面为参数
        //Debug.Log("bones dtw time Series has been writed in " + timeStampPath1);
    }
    public class psoJointWeights
    {
        public float[] weights;

    }

    public float calculateDtwDiff(List<KeyValuePair<float, float>> timePairs,List<float> weightsData)//处理dtw时间序列
    {
        //关节点权重设置
        Dictionary<JAnimationData.BoneType, float> jointWeights = new Dictionary<JAnimationData.BoneType, float>();
        //string boneDiffPath = Application.dataPath + "/zTestData/dtwBoneDiffData.txt";
        //起霸24个关节点权重设置 
        
        jointWeights[JAnimationData.BoneType.spine] = weightsData[0];
        jointWeights[JAnimationData.BoneType.spine1] = weightsData[1];
        jointWeights[JAnimationData.BoneType.spine2] = weightsData[2];
        jointWeights[JAnimationData.BoneType.neck] = weightsData[3];
        jointWeights[JAnimationData.BoneType.neck1] = weightsData[4];
        jointWeights[JAnimationData.BoneType.rightShoulder] = weightsData[5];
        jointWeights[JAnimationData.BoneType.rightArm] = weightsData[6];
        jointWeights[JAnimationData.BoneType.rightForeArm] = weightsData[7];
        jointWeights[JAnimationData.BoneType.leftShoulder] = weightsData[8];
        jointWeights[JAnimationData.BoneType.leftArm] = weightsData[9];
        jointWeights[JAnimationData.BoneType.leftForeArm] = weightsData[10];
        jointWeights[JAnimationData.BoneType.rightUpLeg] = weightsData[11];
        jointWeights[JAnimationData.BoneType.rightLeg] = weightsData[12];
        jointWeights[JAnimationData.BoneType.leftUpLeg] = weightsData[13];
        jointWeights[JAnimationData.BoneType.leftLeg] = weightsData[14];

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

        VFPlayer_InGame.ShowingCharacter master = new VFPlayer_InGame.ShowingCharacter();
        master.builder = new JVehicleFrameSystem.VehicleBuilder(vfplayer.animShowList[3].frame.boneFramework);
        master.builder.Build(1);
        JAnimationSystem.BoneMatch masterbml = new JAnimationSystem.BoneMatch();
        masterbml.StartAutoMatch(master.builder.buildedGA.transform, vfplayer.animShowList[3].autodata, vfplayer.animShowList[3].jAnimData.jAD);
        master.bm_p1 = new JAnimationSystem.BoneMatch_Player(masterbml);
        master.bm_p1.SetJAnimationData(vfplayer.animShowList[3].jAnimData);

        VFPlayer_InGame.ShowingCharacter student = new VFPlayer_InGame.ShowingCharacter();
        student.builder = new JVehicleFrameSystem.VehicleBuilder(vfplayer.animShowList[4].frame.boneFramework);
        student.builder.Build(1);
        JAnimationSystem.BoneMatch studentbml = new JAnimationSystem.BoneMatch();
        studentbml.StartAutoMatch(student.builder.buildedGA.transform, vfplayer.animShowList[4].autodata, vfplayer.animShowList[4].jAnimData.jAD);
        student.bm_p1 = new JAnimationSystem.BoneMatch_Player(studentbml);
        student.bm_p1.SetJAnimationData(vfplayer.animShowList[4].jAnimData);

        int timePairsCount = 0;
        float allBoneAngleDiff = 0.0f;
        float allScoreDiff = 0.0f;
        Debug.Log("timepairs count: " + timePairs.Count);
        foreach (var pair in timePairs)
        {
            timePairsCount++;
            float sumBoneAngleDiff = 0.0f;
            float sumScoreDiff = 0.0f;


            int boneCount = 0;
            for (int j = 0; j < vfplayer.animShowList[3].frame.boneFramework.Bones.Count; j++)
            {

                JAnimationData.BoneType CurBonetype = vfplayer.animShowList[3].frame.boneFramework.Bones[j].boneType;
                int index = vfplayer.animShowList[3].jAnimData.jAD.GetTypeIndex(CurBonetype);
                if (index != -1)
                {
                    JVehicleFrameSystem.BoneFramework.Bone targetBone = vfplayer.animShowList[3].frame.boneFramework.GetBoneByType(CurBonetype);
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
                    

                    Vector3 masterTransformPos = new Vector3(1.9f, 0, 0.7f);
                    Quaternion masterTransformRoa = new Quaternion(0, 0, 0, 1);
                    Vector3 studentTransformPos = new Vector3(-1.5f, -0.013f, -1.4f);
                    Quaternion studentTransformRoa = new Quaternion(0, 0, 0, 1);

                   
                    if (childNum == 1)
                    {

                        
                        float masterBoneAngle = master.bm_p1.GetWorldRoa_Ztest(pair.Key, CurBonetype, vfplayer.animShowList[3].frame.boneFramework, masterTransformPos, masterTransformRoa);
                        float studentBoneAngle = student.bm_p1.GetWorldRoa_Ztest(pair.Value, CurBonetype, vfplayer.animShowList[4].frame.boneFramework, studentTransformPos, studentTransformRoa);
                        float boneAngleDiff = Mathf.Abs(masterBoneAngle - studentBoneAngle);
                       // Debug.Log("boneAngleDiff: " + CurBonetype + ": " + boneAngleDiff);
                        float scoreDiff = jointWeights[CurBonetype] * boneAngleDiff / masterBoneAngle;//记录打分
                        scoreDiff = 1 - scoreDiff;
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
           // Debug.Log("bonecount : " + boneCount);


            string sumStr = sumBoneAngleDiff.ToString();
            string[] fileStr = { sumStr };
            //File.AppendAllLines(boneDiffPath, fileStr);
            allBoneAngleDiff += sumBoneAngleDiff;
            allScoreDiff += sumScoreDiff;
        }
        float score = allBoneAngleDiff / timePairsCount;
        float percentScore = 100 * (allScoreDiff / timePairsCount);
        string wholeStr = "all bone time series Diff Data is : " + score.ToString();
        string allScoreStr = " all bone score is : " + percentScore.ToString();
        Debug.Log("allboneangleDiff is : " + allBoneAngleDiff.ToString());
        Debug.Log(wholeStr);
        Debug.Log(allScoreStr);
        string[] wholeFileStr = { wholeStr, " ", allScoreStr };
        //File.AppendAllLines(boneDiffPath, wholeFileStr);
        //Debug.Log("bones dtw time Series angle Diff Data has been writed in " + boneDiffPath);
        return percentScore;
    }


    public List<float> calculateScore(List<float> weightsData)
    {
        //关节点权重设置
        Dictionary<JAnimationData.BoneType, float> jointWeights = new Dictionary<JAnimationData.BoneType, float>();
        //string boneDiffPath = Application.dataPath + "/zTestData/dtwBoneDiffData.txt";
        //起霸24个关节点权重设置 

        jointWeights[JAnimationData.BoneType.spine] = weightsData[0];
        jointWeights[JAnimationData.BoneType.spine1] = weightsData[1];
        jointWeights[JAnimationData.BoneType.spine2] = weightsData[2];
        jointWeights[JAnimationData.BoneType.neck] = weightsData[3];
        jointWeights[JAnimationData.BoneType.neck1] = weightsData[4];
        jointWeights[JAnimationData.BoneType.rightShoulder] = weightsData[5];
        jointWeights[JAnimationData.BoneType.rightArm] = weightsData[6];
        jointWeights[JAnimationData.BoneType.rightForeArm] = weightsData[7];
        jointWeights[JAnimationData.BoneType.leftShoulder] = weightsData[8];
        jointWeights[JAnimationData.BoneType.leftArm] = weightsData[9];
        jointWeights[JAnimationData.BoneType.leftForeArm] = weightsData[10];
        jointWeights[JAnimationData.BoneType.rightUpLeg] = weightsData[11];
        jointWeights[JAnimationData.BoneType.rightLeg] = weightsData[12];
        jointWeights[JAnimationData.BoneType.leftUpLeg] = weightsData[13];
        jointWeights[JAnimationData.BoneType.leftLeg] = weightsData[14];

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
        string dataJson = System.IO.File.ReadAllText(Application.dataPath + "/zTestData/BoneAngle.json");
        var data = JsonConvert.DeserializeObject<Dictionary<JAnimationData.BoneType, angleDiffTimeSeriesData>>(dataJson);

        int maxSeriesIndex = data.Values.SelectMany(x => x.timeSeriesAngleDiffData.Keys).Max();
        List<float> allScores = new List<float>();

        for (int index = 1; index <= maxSeriesIndex; index++)
        {
            List<float> weightedAverages = new List<float>();

            //遍历每个关节点
            foreach(var kvp in data)
            {
                JAnimationData.BoneType partName = kvp.Key;
                var partData = kvp.Value;

                if (partData.timeSeriesAngleDiffData.ContainsKey(index))
                {
                    //计算当前索引的加权平均值
                    float average = partData.timeSeriesAngleDiffData[index].Select(x => 1 - x).Average();
                    float weightedAverage = average * jointWeights[partName];
                    weightedAverages.Add(weightedAverage);
                }
            }
            float finalScore = weightedAverages.Average();
            finalScore *= 100;
            allScores.Add(finalScore);
            Debug.Log("inedex : " + index + " AverageScore = " + finalScore);
        }
        return allScores;
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
    public float boneTypeAngleDiff(float mTime, float sTime, JAnimationData.BoneType curBonetype)
    {
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        float mAngle = 0.0f;
        float sAngle = 0.0f;
        if (vfplayer.ch_master.createdGA_No != -1)
        {
            if (vfplayer.ch_student.createdGA_No != -1)
            {
                mAngle = vfplayer.ch_master.bm_p1.GetWorldRoa_Ztest(mTime, curBonetype, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation); ;
                sAngle = vfplayer.ch_student.bm_p1.GetWorldRoa_Ztest(sTime, curBonetype, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
            }
        }
        float diffAngle = Mathf.Abs(mAngle - sAngle);
        return diffAngle;
    }



   


    public int GetMinNeighbourIndex1(float[][] dtwMartix, int row, int col)
    {
        float minValue = Mathf.Min(dtwMartix[row - 1][col], dtwMartix[row][col - 1], dtwMartix[row - 1][col - 1]);
        if (dtwMartix[row - 1][col] == minValue)
        {
            return 0;
        }
        else if (dtwMartix[row][col - 1] == minValue)
        {
            return 1;
        }
        else
        {
            return 2;
        }

    }

    public int GetMinNeighbourIndex(float[,] dtwMartix, int row, int col)
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

    public float dtwAbs(float mAngle, float sAngle)
    {
        return Mathf.Abs(mAngle - sAngle);
    }
    //master角度变化量计算 



    public void pyUnityTest()
    {
        Debug.Log("python to unity cmd test success");
    }
}
