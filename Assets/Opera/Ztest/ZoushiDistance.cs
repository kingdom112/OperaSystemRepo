using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Linq;
using System;


public class ZoushiDistance : MonoBehaviour
{
    public float startTime;
    public float endTime;
    public InputField start_field;//master
    public InputField end_field;//master
    public float studentStartTime;
    public float studentEndTime;
    public InputField student_s_field;
    public InputField student_e_field;
    StreamWriter writer;
    StreamReader reader;
    public GameObject panelPrefab; // 引用你的面板预设
    private GameObject ztestPanel;

    public GameObject staticActionPanelPrefab; // 引用你的面板预设
    private GameObject zStaticActionPanel;

    public GameObject actionEvaluationPanelPrefab; // 引用你的面板预设
    private GameObject zActionEvaluationPanel;

    public InputField masterStaticTime;
    public InputField studentStaticTime;//静态动作相似度对比时间
    public float masterStaticTimeData;
    public float studentStaticTimeData;
    public Text scoreTextField;//angleScore显示组件
    public Text featurePlaneTextField;//featurePlaneScore显示组件

    public InputField masterDtwTime_field;//自动识别对应时间的输入框
    float masterDtwTimeData;
    public Text studentDtwTime;//计算出来的对应时间


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
        mainCanvas = GameObject.Find("UI").GetComponent<Canvas>();
        if (mainCanvas == null)
        {
            Debug.LogError("UI Canvas not found in the scene");
        }
        if (panelPrefab != null && mainCanvas != null)
        {
            ztestPanel = Instantiate(panelPrefab);
            ztestPanel.transform.SetParent(mainCanvas.transform, false);
            ztestPanel.SetActive(false);
        }
        if (staticActionPanelPrefab != null && mainCanvas != null)
        {
            zStaticActionPanel = Instantiate(staticActionPanelPrefab);
            zStaticActionPanel.transform.SetParent(mainCanvas.transform, false);
            zStaticActionPanel.SetActive(false);
        }

        if (actionEvaluationPanelPrefab != null && mainCanvas != null)
        {
            zActionEvaluationPanel = Instantiate(actionEvaluationPanelPrefab);
            zActionEvaluationPanel.transform.SetParent(mainCanvas.transform, false);
            zActionEvaluationPanel.SetActive(false);
        }
    }

    public void OnValueChanged(float value)
    {
        Debug.Log("onvaluechanged value:" + value);
    }
    // Update is called once per frame
    void Update()
    {
        if (start_field.text == "")
        {
            startTime = 0.0f;
        }
        else
        {
            startTime = float.Parse(start_field.text);
        }

        if (end_field.text == "")
        {
            endTime = 0.0f;
        }
        else
        {
            endTime = float.Parse(end_field.text);
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


        //dtw对应时间检测
        if (masterDtwTime_field.text == "")
        {
            masterDtwTimeData = 0.0f;
        }
        else
        {
            masterDtwTimeData = float.Parse(masterDtwTime_field.text);
        }
    }

    private Canvas mainCanvas;
    public void CreateNewPanel()
    {
        if(ztestPanel != null)
        {
            ztestPanel.SetActive(!ztestPanel.activeSelf);
        }


    }
    public void createStaticActionPanel()
    {
        if (zStaticActionPanel != null)
        {
            zStaticActionPanel.SetActive(!zStaticActionPanel.activeSelf);
        }


    }

    public void createActionEvaluationPanel()
    {
        if (zActionEvaluationPanel != null)
        {
            zActionEvaluationPanel.SetActive(!zActionEvaluationPanel.activeSelf);
        }


    }
    //处理master数据 去除bonetype行 只剩数据行 同样处理student数据 然后计算oushidis 并保存文件
    public void dataCalculate()
    {

        List<int> dataList;
        masterData = new Dictionary<string, List<Vector3>>();
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        float timeCount = (int)((endTime - startTime) / 0.1f);
        int masterBoneCount = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones.Count;
        dataList = new List<int>();

        //处理master数据
        string fileMasterPath = Application.dataPath + "/zTestData/masterBoneData.txt";
        if (vfplayer.ch_master.createdGA_No != -1)
        {
            for (int i = 0; i < masterBoneCount; i++)
            {
                JAnimationData.BoneType masterBonetype = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones[i].boneType;
                string masterStr = masterBonetype.ToString();
                int index = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.GetTypeIndex(masterBonetype);
                if (index != -1)
                {
                    masterData.Add(masterStr, new List<Vector3>());
                    using (StreamReader reader = new StreamReader(fileMasterPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line == masterStr)
                            {

                                for (int j = 0; j < timeCount; j++)
                                {
                                    line = reader.ReadLine();
                                    string[] values = line.Split(':');
                                    float timeStamp = float.Parse(values[0]);
                                    string vectorString = values[1].Replace("(", "").Replace(")", "");

                                    // 分割Vector3坐标字符串为数组
                                    string[] vectorValues = vectorString.Split(',');

                                    // 从分割的数组元素创建新对象
                                    float x = float.Parse(vectorValues[0]);
                                    float y = float.Parse(vectorValues[1]);
                                    float z = float.Parse(vectorValues[2]);
                                    Vector3 masterTimeData = new Vector3(x, y, z);
                                    //Debug.Log("Time Stamp: " + timeStamp);
                                    //Debug.Log("Vector3: " + masterTimeData);

                                    string fileCalDataPath = Application.dataPath + "/zTestData/CalMasterData.txt";
                                    string masterPos = masterTimeData.ToString();
                                    string[] masterCalData = { masterPos };
                                    File.AppendAllLines(fileCalDataPath, masterCalData);

                                    masterData[masterStr].Add(masterTimeData);
                                }
                                break;
                            }

                        }
                        foreach (KeyValuePair<string, List<Vector3>> kvPair in masterData)
                        {
                            List<Vector3> debugMasterData = kvPair.Value;
                            foreach (Vector3 masterdata in debugMasterData)
                            {
                                Debug.Log("list " + masterStr + "data: " + masterdata);
                            }
                        }
                    }


                }

            }
        }

        //处理student数据
        string fileStudentPath = Application.dataPath + "/zTestData/studentBoneData.txt";
        studentData = new Dictionary<string, List<Vector3>>();
        int studentBoneCount = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.Bones.Count;
        if (vfplayer.ch_student.createdGA_No != -1)
        {
            for (int i = 0; i < studentBoneCount; i++)
            {
                JAnimationData.BoneType studentBonetype = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.Bones[i].boneType;
                string studentStr = studentBonetype.ToString();
                int index = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.GetTypeIndex(studentBonetype);
                if (index != -1)
                {
                    studentData.Add(studentStr, new List<Vector3>());
                    using (StreamReader reader = new StreamReader(fileStudentPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line == studentStr)
                            {

                                for (int j = 0; j < timeCount; j++)
                                {
                                    line = reader.ReadLine();
                                    string[] values = line.Split(':');
                                    float timeStamp = float.Parse(values[0]);
                                    string vectorString = values[1].Replace("(", "").Replace(")", "");

                                    // 分割Vector3坐标字符串为数组
                                    string[] vectorValues = vectorString.Split(',');

                                    // 从分割的数组元素创建新对象
                                    float x = float.Parse(vectorValues[0]);
                                    float y = float.Parse(vectorValues[1]);
                                    float z = float.Parse(vectorValues[2]);
                                    Vector3 studentTimeData = new Vector3(x, y, z);
                                    //Debug.Log("Time Stamp: " + timeStamp);
                                    //Debug.Log("Vector3: " + masterTimeData);

                                    string fileCalDataPath = Application.dataPath + "/zTestData/CalStudentData.txt";
                                    string studentPos = studentTimeData.ToString();
                                    string[] studentCalData = { studentPos };
                                    File.AppendAllLines(fileCalDataPath, studentCalData);

                                    studentData[studentStr].Add(studentTimeData);
                                }
                                break;
                            }

                        }
                        foreach (KeyValuePair<string, List<Vector3>> kvPair in studentData)
                        {
                            List<Vector3> debugStudentData = kvPair.Value;
                            foreach (Vector3 studentData in debugStudentData)
                            {
                                Debug.Log("list " + studentStr + "  data: " + studentData);
                            }
                        }
                    }


                }

            }

            //比较大师和学生数据并写入文件
            string fileCompareDataPath = Application.dataPath + "/zTestData/compareData.txt";
            string fileCompareCalDataPath = Application.dataPath + "/zTestData/compareCalData.txt";
            Dictionary<float, List<float>> dataDict = new Dictionary<float, List<float>>();
            Dictionary<float, float> sumDataDict = new Dictionary<float, float>();
            foreach (KeyValuePair<string, List<Vector3>> kvp in masterData)
            {
                string key = kvp.Key;
                string[] boneName = { key };
                List<Vector3> masterList, studentList;
                if (masterData.TryGetValue(key, out masterList) && studentData.TryGetValue(key, out studentList))
                {
                    File.AppendAllLines(fileCompareCalDataPath, boneName);
                    for (int i = 0; i < masterList.Count; i++)
                    {
                        float curTime = startTime + i * 0.1f;
                        string time = curTime.ToString() + ": ";
                        Vector3 masV = masterList[i];
                        Vector3 stuV = studentList[i];
                        Vector3 test = masV - stuV;
                        float oushiTestData = (masV.x - stuV.x) * (masV.x - stuV.x) + (masV.y - stuV.y) * (masV.y - stuV.y) + (masV.z - stuV.z) * (masV.z - stuV.z);

                        if (dataDict.ContainsKey(curTime))
                        {
                            dataDict[curTime].Add(oushiTestData);
                        }
                        else
                        {
                            dataDict.Add(curTime, new List<float> { oushiTestData });

                        }


                        string testString = test.ToString();
                        string[] testStr = { testString };
                        File.AppendAllLines(fileCompareDataPath, testStr);

                        string oushiTestString = time + oushiTestData.ToString();
                        string[] oushiTestStr = { oushiTestString };
                        File.AppendAllLines(fileCompareCalDataPath, oushiTestStr);

                    }
                }
            }

            string fileOushiDisDataPath = Application.dataPath + "/zTestData/oushiDisData.txt";
            foreach (KeyValuePair<float, List<float>> kvp in dataDict)
            {
                float sum = 0.0f;
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    sum += kvp.Value[i];
                }

                string curTime = kvp.Key.ToString();
                string sumStr = sum.ToString();
                string oushiData = curTime + ":" + sumStr;
                string[] oushiDataStr = { oushiData };
                File.AppendAllLines(fileOushiDisDataPath, oushiDataStr);
            }


        }



        Debug.Log("have calculate master data and student data");
    }


    //将student的关节点数据 按照bonetype 以0.1f间隔 将worldpos写入文件
    public void oushiTestStudent()
    {
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        Debug.Log("selectBones count : " + vfplayer.ch_student.selectBones.Count);
        if (vfplayer.ch_student.createdGA_No != -1)
        {
            Debug.Log("bones count :" + vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.Bones.Count);
            int boneCount = 1;
            float interval = studentEndTime - studentStartTime;//选段的间隔
            int TimeCount = (int)(interval / 0.1f);//次数
            for (int j = 0; j < vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.Bones.Count; j++)
            {
                //将骨骼类型写入文件

                string path = Application.dataPath + "/zTestData/studentBoneData.txt";
                JAnimationData.BoneType CurBonetype = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.Bones[j].boneType;

                int index = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.GetTypeIndex(CurBonetype);
                if (index != -1)
                {

                    string bonetypeText = CurBonetype.ToString();
                    //string bonetypeCount = boneCount.ToString();
                    string[] boneStr = { bonetypeText };
                    File.AppendAllLines(path, boneStr);

                    float curTime = studentStartTime;
                    for (int i = 0; i < TimeCount; i++)
                    {
                        Vector3 target_pos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(curTime, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                        string vec3Str = target_pos.ToString();
                        string timeStr = curTime.ToString();
                        string sumStr = timeStr + ": " + vec3Str;
                        string[] fileStr = { sumStr };
                        File.AppendAllLines(path, fileStr);
                        curTime += 0.1f;

                    }
                    boneCount++;
                }
            }

            string path1 = Application.dataPath + "/zTestData/studentBoneData.txt";
            //float test = 0.0f;
            //test = startTime + endTime;
            //Debug.Log("testTime :" + test);
            Debug.Log("txt has been created in " + path1);
        }
    }


    //将master的关节点数据 按照bonetype 以0.1f间隔 将worldpos写入文件
    public void oushiTestMaster()
    {
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        Debug.Log("selectBones count : " + vfplayer.ch_master.selectBones.Count);
        if (vfplayer.ch_master.createdGA_No != -1)
        {
            Debug.Log("bones count :" + vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones.Count);
            int boneCount = 1;
            float interval = endTime - startTime;//选段的间隔
            int TimeCount = (int)(interval / 0.1f);//次数
            for (int j = 0; j < vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones.Count; j++)
            {
                //将骨骼类型写入文件

                string path = Application.dataPath + "/zTestData/masterBoneData.txt";
                JAnimationData.BoneType CurBonetype = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones[j].boneType;

                int index = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.GetTypeIndex(CurBonetype);
                if (index != -1)
                {

                    string bonetypeText = CurBonetype.ToString();
                    //string bonetypeCount = boneCount.ToString();
                    string[] boneStr = { bonetypeText };
                    File.AppendAllLines(path, boneStr);

                    float curTime = startTime;
                    for (int i = 0; i < TimeCount; i++)
                    {
                        Vector3 target_pos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(curTime, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                            vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                        string vec3Str = target_pos.ToString();
                        string timeStr = curTime.ToString();
                        string sumStr = timeStr + ": " + vec3Str;
                        string[] fileStr = { sumStr };
                        File.AppendAllLines(path, fileStr);
                        curTime += 0.1f;

                    }
                    boneCount++;
                }
            }

            string path1 = Application.dataPath + "/zTestData/masterBoneData.txt";
            FileInfo file = new FileInfo(Application.dataPath + "/zTestData/masterBoneData.txt");
            Debug.Log("txt has been created in " + path1);
        }

    }

    //master运动矢量增加图
    public void timeV3Gap()
    {
        List<int> dataList;
        masterData = new Dictionary<string, List<Vector3>>();
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        float timeCount = (int)((endTime - startTime) / 0.1f);
        int masterBoneCount = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones.Count;
        dataList = new List<int>();

        //处理master数据
        string fileMasterPath = Application.dataPath + "/zTestData/masterBoneData.txt";
        string fileTimeGapPath = Application.dataPath + "/zTestData/TimeV3MasterData.txt";
        string fileTimeGapPath1 = Application.dataPath + "/zTestData/TimeGapMasterData.txt";
        if (vfplayer.ch_master.createdGA_No != -1)
        {
            for (int i = 0; i < masterBoneCount; i++)
            {
                JAnimationData.BoneType masterBonetype = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones[i].boneType;
                string masterStr = masterBonetype.ToString();
                int index = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.GetTypeIndex(masterBonetype);
                if (index != -1)
                {

                    using (StreamReader reader = new StreamReader(fileMasterPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {

                            if (line == masterStr)
                            {
                                float sumTimeGap = 0.0f;
                                string[] masterBoneName = { masterStr };
                                File.AppendAllLines(fileTimeGapPath, masterBoneName);
                                File.AppendAllLines(fileTimeGapPath1, masterBoneName);
                                line = reader.ReadLine();
                                string[] values1 = line.Split(':');
                                string vectorString1 = values1[1].Replace("(", "").Replace(")", "");
                                string[] vectorValues1 = vectorString1.Split(',');

                                float x1 = float.Parse(vectorValues1[0]);
                                float y1 = float.Parse(vectorValues1[1]);
                                float z1 = float.Parse(vectorValues1[2]);
                                Vector3 masterTimePreData = new Vector3(x1, y1, z1);

                                for (int j = 1; j < timeCount; j++)
                                {
                                    line = reader.ReadLine();
                                    string[] values = line.Split(':');
                                    float timeStamp = float.Parse(values[0]);
                                    string vectorString = values[1].Replace("(", "").Replace(")", "");

                                    // 分割Vector3坐标字符串为数组
                                    string[] vectorValues = vectorString.Split(',');

                                    // 从分割的数组元素创建新对象
                                    float x = float.Parse(vectorValues[0]);
                                    float y = float.Parse(vectorValues[1]);
                                    float z = float.Parse(vectorValues[2]);
                                    Vector3 masterTimeDataBack = new Vector3(x, y, z);
                                    //Debug.Log("Time Stamp: " + timeStamp);
                                    //Debug.Log("Vector3: " + masterTimeData);

                                    Vector3 timeGapData = masterTimeDataBack - masterTimePreData;
                                    float timeGapf = timeGapData.x * timeGapData.x + timeGapData.y * timeGapData.y + timeGapData.z * timeGapData.z;
                                    sumTimeGap += timeGapf;
                                    string timeStampStr = timeStamp.ToString() + ":";
                                    string timeV3Gapdata = timeStampStr + timeGapData.ToString();
                                    string timeGapfStr = timeStampStr + timeGapf.ToString();

                                    string[] timeGapDataStr = { timeV3Gapdata };
                                    File.AppendAllLines(fileTimeGapPath, timeGapDataStr);
                                    string[] timeGapFloatStr = { timeGapfStr };
                                    File.AppendAllLines(fileTimeGapPath1, timeGapFloatStr);
                                    masterTimePreData = masterTimeDataBack;

                                }
                                //string sumStr = "sum: " + sumTimeGap;
                                //string[] sumGapf = { sumStr };
                                //File.AppendAllLines(fileTimeGapPath1, sumGapf);
                                break;
                            }

                        }
                    }


                }

            }
            Debug.Log("txt has been created in " + fileTimeGapPath);
        }

    }


    //master角度计算
    public void angleCalculate()
    {
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        Debug.Log("selectBones count : " + vfplayer.ch_master.selectBones.Count);




        if (vfplayer.ch_master.createdGA_No != -1)
        {
            Debug.Log("bones count :" + vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones.Count);
            int boneCount = 1;
            float interval = endTime - startTime;//选段的间隔

            int TimeCount = (int)(interval / 0.1f);//次数
            for (int j = 0; j < vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones.Count; j++)
            {
                //将骨骼类型写入文件
                string path = Application.dataPath + "/zTestData/masterAngleData.txt";
                JAnimationData.BoneType CurBonetype = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones[j].boneType;
                int index = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.GetTypeIndex(CurBonetype);
                if (index != -1)
                {
                    JVehicleFrameSystem.BoneFramework.Bone targetBone = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.GetBoneByType(CurBonetype);
                    int childNum = targetBone.ChildsList.Count;
                    if (childNum == 1)
                    {
                        string bonetypeText = CurBonetype.ToString();
                        //string bonetypeCount = boneCount.ToString();
                        string[] boneStr = { bonetypeText };
                        File.AppendAllLines(path, boneStr);
                        float curTime = startTime;



                        for (int i = 0; i < TimeCount; i++)
                        {
                            float target_angle = vfplayer.ch_master.bm_p1.GetWorldRoa_Ztest(curTime, CurBonetype, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                            string angleStr = target_angle.ToString();
                            string timeStr = curTime.ToString("F2");
                            string sumStr = timeStr + ": " + angleStr;
                            string[] fileStr = { sumStr };
                            File.AppendAllLines(path, fileStr);

                            curTime += 0.1f;
                        }
                        boneCount++;


                    }
                    else
                    {
                        continue;
                    }

                }
            }

            string path1 = Application.dataPath + "/zTestData/masterAngleData.txt";
            FileInfo file = new FileInfo(Application.dataPath + "/zTestData/masterAngleData.txt");
            Debug.Log("txt has been created in " + path1);
        }



        if (vfplayer.ch_student.createdGA_No != -1)
        {
            Debug.Log("bones count :" + vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.Bones.Count);
            int boneCount = 1;



            float interval = studentEndTime - studentStartTime;//选段的间隔

            int TimeCount = (int)(interval / 0.1f);//次数
            for (int j = 0; j < vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.Bones.Count; j++)
            {
                //将骨骼类型写入文件
                string path = Application.dataPath + "/zTestData/studentAngleData.txt";
                JAnimationData.BoneType CurBonetype = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.Bones[j].boneType;
                int index = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.GetTypeIndex(CurBonetype);
                if (index != -1)
                {
                    JVehicleFrameSystem.BoneFramework.Bone targetBone = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.GetBoneByType(CurBonetype);
                    int childNum = targetBone.ChildsList.Count;
                    if (childNum == 1)
                    {
                        string bonetypeText = CurBonetype.ToString();
                        //string bonetypeCount = boneCount.ToString();
                        string[] boneStr = { bonetypeText };
                        File.AppendAllLines(path, boneStr);
                        float curTime = studentStartTime;



                        for (int i = 0; i < TimeCount; i++)
                        {
                            float target_angle = vfplayer.ch_student.bm_p1.GetWorldRoa_Ztest(curTime, CurBonetype, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);

                            string angleStr = target_angle.ToString();
                            string timeStr = curTime.ToString("F2");
                            string sumStr = timeStr + ": " + angleStr;
                            string[] fileStr = { sumStr };
                            File.AppendAllLines(path, fileStr);
                            curTime += 0.1f;


                        }
                        boneCount++;


                    }
                    else
                    {
                        continue;
                    }

                }
            }
            string path1 = Application.dataPath + "/zTestData/studentAngleData.txt";
            FileInfo file = new FileInfo(Application.dataPath + "/zTestData/studentAngleData.txt");
            Debug.Log("txt has been created in " + path1);


        }
    }


    //master angle dtw data
    public void angleDtwData()
    {
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();

        //master
        if (vfplayer.ch_master.createdGA_No != -1)
        {
            float preSumAngle = 0.0f;
            float sumGapAngle = 0.0f;
            float interval = endTime - startTime;//选段的间隔
            int TimeCount = (int)(interval / 0.1f);//次数
            float curTime = startTime;
            string path = Application.dataPath + "/zTestData/masterDtwAngleData.txt";
            string gapSumAnglePath = Application.dataPath + "/zTestData/masterSumGapAngleData.txt";
            for (int i = 0; i < TimeCount; i++)
            {
                float sumAngle = 0.0f;

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
                            float curBoneAngle = vfplayer.ch_master.bm_p1.GetWorldRoa_Ztest(curTime, CurBonetype, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);

                            sumAngle += curBoneAngle;
                        }
                    }
                    else
                    {
                        continue;
                    }

                }

                string timeStr = curTime.ToString();
                string sumAngleStr = sumAngle.ToString();
                string dtwStr = timeStr + ":" + sumAngleStr;
                string[] dtwAngleStr = { dtwStr };
                File.AppendAllLines(path, dtwAngleStr);
               
                
                sumGapAngle = 0.0f;
                if (i == 0)
                {
                    preSumAngle = sumAngle;
                    continue;
                }
                else
                {
                    sumGapAngle = Mathf.Abs(sumAngle - preSumAngle);
                    string dataStr = sumGapAngle.ToString();
                    string fileStr = timeStr + ":" + dataStr;
                    string[] sumGapAngleStr = { fileStr };
                    File.AppendAllLines(gapSumAnglePath, sumGapAngleStr);
                    preSumAngle = sumAngle;
                }
               
                curTime += 0.1f;
            }

            string path1 = Application.dataPath + "/zTestData/masterDtwAngleData.txt";
            FileInfo file = new FileInfo(Application.dataPath + "/zTestData/masterDtwAngleData.txt");
            Debug.Log("txt has been created in " + path1);
        }

        //student
        if (vfplayer.ch_student.createdGA_No != -1)
        {
            float preSumAngle = 0.0f;
            float interval = studentEndTime - studentStartTime;//选段的间隔
            int TimeCount = (int)(interval / 0.1f);//次数
            float curTime = studentStartTime;
            string path = Application.dataPath + "/zTestData/studentDtwAngleData.txt";
            string gapSumAngleStudentPath = Application.dataPath + "/zTestData/studentSumGapAngleData.txt";
            for (int i = 0; i < TimeCount; i++)
            {
                float sumAngle = 0.0f;

                for (int j = 0; j < vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.Bones.Count; j++)
                {
                    JAnimationData.BoneType CurBonetype = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.Bones[j].boneType;
                    int index = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.GetTypeIndex(CurBonetype);
                    if (index != -1)
                    {
                        JVehicleFrameSystem.BoneFramework.Bone targetBone = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.GetBoneByType(CurBonetype);
                        int childNum = targetBone.ChildsList.Count;
                        if (childNum == 1)
                        {
                            if (CurBonetype == JAnimationData.BoneType.leftToeBase) continue;
                            if (CurBonetype == JAnimationData.BoneType.rightToeBase) continue;
                            if (CurBonetype == JAnimationData.BoneType.leftFoot) continue;
                            if (CurBonetype == JAnimationData.BoneType.rightFoot) continue;
                            float curBoneAngle = vfplayer.ch_student.bm_p1.GetWorldRoa_Ztest(curTime, CurBonetype, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);

                            sumAngle += curBoneAngle;
                        }
                    }
                    else
                    {
                        continue;
                    }

                }
                string timeStr = curTime.ToString();
                string sumAngleStr = sumAngle.ToString();
                string dtwStr = timeStr + ":" + sumAngleStr;
                string[] dtwAngleStr = { dtwStr };
                File.AppendAllLines(path, dtwAngleStr);


               
                float sumGapAngle = 0.0f;
                if (i == 0)
                {
                    preSumAngle = sumAngle;
                    continue;
                }
                else
                {
                    sumGapAngle = Mathf.Abs(sumAngle - preSumAngle);
                    string dataStr = sumGapAngle.ToString();
                    string fileStr = timeStr + ":" + dataStr;
                    string[] sumGapAngleStr = { fileStr };
                    File.AppendAllLines(gapSumAngleStudentPath, sumGapAngleStr);
                    preSumAngle = sumAngle;
                }
                

                curTime += 0.1f;
                
            }

            string path1 = Application.dataPath + "/zTestData/studentDtwAngleData.txt";
            FileInfo file = new FileInfo(Application.dataPath + "/zTestData/studentDtwAngleData.txt");
            Debug.Log("txt has been created in " + path1);
        }

    }

    public void dtwTest()
    {
        string masterPath = Application.dataPath + "/zTestData/masterDtwAngleData.txt";
        string studentPath = Application.dataPath + "/zTestData/studentDtwAngleData.txt";
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();


        //处理master的dtw数据
        Dictionary<float, float> masterDtwDict = new Dictionary<float, float>();

        if (vfplayer.ch_master.createdGA_No != -1)
        {

            if (!File.Exists(masterPath))
            {
                Debug.LogError("File does not exist :" + masterPath);
                return;
            }
            string[] masterLines = File.ReadAllLines(masterPath);
            foreach (string line in masterLines)
            {
                string[] masterData = line.Split(':');
                if (masterData.Length == 2)
                {
                    float masterTime = float.Parse(masterData[0]);
                    float masterAngle = float.Parse(masterData[1]);

                    masterDtwDict.Add(masterTime, masterAngle);
                }
                else
                {
                    Debug.LogError("Invalid data format " + line);
                }
            }
        }

        //处理student数据
        Dictionary<float, float> studentDtwDict = new Dictionary<float, float>();
        if (vfplayer.ch_student.createdGA_No != -1)
        {

            if (!File.Exists(studentPath))
            {
                Debug.LogError("File does not exist :" + studentPath);
                return;
            }
            string[] studentLines = File.ReadAllLines(studentPath);
            foreach (string line in studentLines)
            {
                string[] studentData = line.Split(':');
                if (studentData.Length == 2)
                {
                    float studentTime = float.Parse(studentData[0]);
                    float studentAngle = float.Parse(studentData[1]);

                    studentDtwDict.Add(studentTime, studentAngle);
                }
                else
                {
                    Debug.LogError("Invalid data format " + line);
                }
            }
            Debug.Log("student dict has been added");
        }

        int masterLen = masterDtwDict.Count;
        int studentLen = studentDtwDict.Count;
        float[,] dtwMatrix = new float[masterLen, studentLen];//dtw 存放数据的二维矩阵

        dtwMatrix[0, 0] = 0;//dtw矩阵的第一行第一列初始化
        //填充边界值
        for (int i = 1; i < masterLen; i++)
        {
            dtwMatrix[i, 0] = float.MaxValue;
        }
        for (int j = 1; j < studentLen; j++)
        {
            dtwMatrix[0, j] = float.MaxValue;
        }

        //填充整个矩阵
        for (int i = 1; i < masterLen; i++)
        {
            for (int j = 1; j < studentLen; j++)
            {
                float dis = dtwAbs(masterDtwDict.Values.ElementAt(i - 1), studentDtwDict.Values.ElementAt(j - 1));
                float cost = dis + Mathf.Min(dtwMatrix[i - 1, j], dtwMatrix[i, j - 1], dtwMatrix[i - 1, j - 1]);
                dtwMatrix[i, j] = cost;
            }
        }

        //diff mas stu
        Dictionary<JAnimationData.BoneType, float> diffDtwData = new Dictionary<JAnimationData.BoneType, float>();


        List<string> timeList = new List<string>();
        int row = masterLen - 1;
        int col = studentLen - 1;
        float mTime = masterDtwDict.Keys.ElementAt(row);
        float sTime = studentDtwDict.Keys.ElementAt(col);
        string mStr = mTime.ToString();
        string sStr = sTime.ToString();
        string sumStr = mStr + " " + sStr;
        timeList.Add(sumStr);

        while (row > 0 && col > 0)
        {
            int minNeighbourIndex = GetMinNeighbourIndex(dtwMatrix, row, col);
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
            mTime = masterDtwDict.Keys.ElementAt(row);
            sTime = studentDtwDict.Keys.ElementAt(col);
            mStr = mTime.ToString();
            sStr = sTime.ToString();
            sumStr = mStr + " " + sStr;
            timeList.Add(sumStr);
        }

        string timeStampPath = Application.dataPath + "/zTestData/dtwTimeStampData.txt";
        foreach (var temp in timeList)
        {
            string[] dtwTimeStampStr = { temp };
            File.AppendAllLines(timeStampPath, dtwTimeStampStr);
        }
        Debug.Log("txt has been created in " + timeStampPath);


        //关节点差异
        if (vfplayer.ch_master.createdGA_No != -1)
        {
            if (vfplayer.ch_student.createdGA_No != -1)
            {
                float allBoneTypeDiffData = 0.0f;
                int averageTimeCount = 0;
                string boneAngleDiffPath = Application.dataPath + "/zTestData/boneAngleDiffData.txt";
                List<string> angleDiffList = new List<string>();

                float sumDiffAngle = 0.0f;
                for (int j = 0; j < vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones.Count; j++)
                {
                    averageTimeCount = 0;
                    row = masterLen - 1;
                    col = studentLen - 1;
                    mTime = masterDtwDict.Keys.ElementAt(row);
                    sTime = studentDtwDict.Keys.ElementAt(col);
                    sumDiffAngle = 0.0f;
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
                            float diffAngle = boneTypeAngleDiff(mTime, sTime, CurBonetype);

                            sumDiffAngle += diffAngle;
                            while (row > 0 && col > 0)
                            {
                                int minNeighbourIndex = GetMinNeighbourIndex(dtwMatrix, row, col);
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
                                mTime = masterDtwDict.Keys.ElementAt(row);
                                sTime = studentDtwDict.Keys.ElementAt(col);
                                diffAngle = boneTypeAngleDiff(mTime, sTime, CurBonetype);
                                sumDiffAngle += diffAngle;
                                averageTimeCount++;
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                    diffDtwData.Add(CurBonetype, sumDiffAngle);
                    
                    allBoneTypeDiffData += sumDiffAngle;
                }
                float averageBonetypeData = allBoneTypeDiffData / averageTimeCount;

                string[] allBonetypeDataStr = { "all bonetype all timecount average diff Data: ", averageBonetypeData.ToString() };
                string averageAllBoneAllTimecountPath = Application.dataPath + "/zTestData/allBTAverageDiffData.txt";
                File.AppendAllLines(averageAllBoneAllTimecountPath, allBonetypeDataStr);

                for (int i = 0; i < diffDtwData.Count; i++)
                {
                    JAnimationData.BoneType curbonetype = diffDtwData.Keys.ElementAt(i);
                    float diffAngle = diffDtwData.Values.ElementAt(i);
                    string bonetypeStr = curbonetype.ToString();
                    string diffAngleStr = diffAngle.ToString();
                    string[] sumDiffAngleStr = { bonetypeStr, ": ", diffAngleStr };
                    File.AppendAllLines(boneAngleDiffPath, sumDiffAngleStr);
                }
                Debug.Log("txt has been created in " + boneAngleDiffPath);
            }
        }


    }

    public void dtwTest1()
    {
        string masterPath = Application.dataPath + "/zTestData/masterDtwAngleData.txt";
        string studentPath = Application.dataPath + "/zTestData/studentDtwAngleData.txt";
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();


        //处理master的dtw数据
        List<float> masterTimeData = new List<float>();
        List<float> masterAngleData = new List<float>();


        if (vfplayer.ch_master.createdGA_No != -1)
        {

            if (!File.Exists(masterPath))
            {
                Debug.LogError("File does not exist :" + masterPath);
                return;
            }
            string[] masterLines = File.ReadAllLines(masterPath);
            foreach (string line in masterLines)
            {
                string[] masterData = line.Split(':');
                if (masterData.Length == 2)
                {
                    float masterTime = float.Parse(masterData[0]);
                    float masterAngle = float.Parse(masterData[1]);
                    masterTimeData.Add(masterTime);
                    masterAngleData.Add(masterAngle);
                }
                else
                {
                    Debug.LogError("Invalid data format " + line);
                }
            }
        }

        //处理student数据
        List<float> studentTimeData = new List<float>();
        List<float> studentAngleData = new List<float>();

        if (vfplayer.ch_student.createdGA_No != -1)
        {

            if (!File.Exists(studentPath))
            {
                Debug.LogError("File does not exist :" + studentPath);
                return;
            }
            string[] studentLines = File.ReadAllLines(studentPath);
            foreach (string line in studentLines)
            {
                string[] studentData = line.Split(':');
                if (studentData.Length == 2)
                {
                    float studentTime = float.Parse(studentData[0]);
                    float studentAngle = float.Parse(studentData[1]);
                    studentTimeData.Add(studentTime);
                    studentAngleData.Add(studentAngle);

                }
                else
                {
                    Debug.LogError("Invalid data format " + line);
                }
            }
            Debug.Log("student dict has been added");
        }

        int masterLen = masterTimeData.Count;

        int studentLen = studentTimeData.Count;
        float[][] dtwMatrix = new float[masterLen][];//dtw 存放数据的二维矩阵

        for (int i = 0; i < masterLen; i++)
        {
            dtwMatrix[i] = new float[studentLen];
            for (int j = 0; j < studentLen; j++)
            {
                dtwMatrix[i][j] = float.PositiveInfinity;
            }
        }

        dtwMatrix[0][0] = 0;//dtw矩阵的第一行第一列初始化


        //填充整个矩阵
        for (int i = 1; i < masterLen; i++)
        {
            for (int j = 1; j < studentLen; j++)
            {
                float cost = Mathf.Abs(masterAngleData[i] - studentAngleData[j]);
                float minCost = Mathf.Min(Mathf.Min(dtwMatrix[i - 1][j], dtwMatrix[i][j - 1]), dtwMatrix[i - 1][j - 1]);
                dtwMatrix[i][j] = cost + minCost;
            }
        }


        List<string> timeList = new List<string>();
        List<float> distanceMS = new List<float>();//master student 差距


        int row = masterLen - 1;
        int col = studentLen - 1;
        float mTime = masterTimeData[row];
        float sTime = studentTimeData[col];

        float mAngle = masterAngleData[row];
        float sAngle = studentAngleData[col];

        float disMS = Mathf.Abs(mAngle - sAngle);
        distanceMS.Add(disMS);
        float sumdisMS = disMS;

        string mStr = mTime.ToString();
        string sStr = sTime.ToString();
        string sumStr = mStr + " " + sStr;
        timeList.Add(sumStr);

        while (row > 0 && col > 0)
        {
            int minNeighbourIndex = GetMinNeighbourIndex1(dtwMatrix, row, col);
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
            mTime = masterTimeData[row];
            sTime = studentTimeData[col];

            mAngle = masterAngleData[row];
            sAngle = studentAngleData[col];
            disMS = Mathf.Abs(mAngle - sAngle);
            sumdisMS += disMS;
            distanceMS.Add(disMS);

            mStr = mTime.ToString();
            sStr = sTime.ToString();
            sumStr = mStr + " " + sStr;
            timeList.Add(sumStr);
        }
        string distanceMSPath = Application.dataPath + "/zTestData/distanceMSData.txt";
        foreach (var temp in distanceMS)
        {
            string distanceStr = temp.ToString();
            string[] distanceSStr = { distanceStr };
            File.AppendAllLines(distanceMSPath, distanceSStr);
        }

        float averageDistance = sumdisMS / distanceMS.Count;
        string averageDistanceStr = averageDistance.ToString();
        string[] averageDistanceSStr = { "averageDis: ", averageDistanceStr };
        File.AppendAllLines(distanceMSPath, averageDistanceSStr);

        string timeStampPath1 = Application.dataPath + "/zTestData/dtwTimeStampData.txt";
        foreach (var temp in timeList)
        {
            string[] dtwTimeStampStr = { temp };
            File.AppendAllLines(timeStampPath1, dtwTimeStampStr);
        }


        Debug.Log("txt has been created in " + timeStampPath1);
        Dictionary<JAnimationData.BoneType, float> diffDtwData = new Dictionary<JAnimationData.BoneType, float>();
        //关节点差异
        if (vfplayer.ch_master.createdGA_No != -1)
        {
            if (vfplayer.ch_student.createdGA_No != -1)
            {
                string boneAngleDiffPath = Application.dataPath + "/zTestData/boneAngleDiffData.txt";
                List<string> angleDiffList = new List<string>();
                float allBoneTypeDiffData = 0.0f;
                int averageTimeCount = 0;
                float sumDiffAngle = 0.0f;
                float allBoneDiffAngle = 0.0f;
                for (int j = 0; j < vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones.Count; j++)
                {
                    row = masterLen - 1;
                    col = studentLen - 1;
                    mTime = masterTimeData[row];
                    sTime = studentTimeData[col];
                    sumDiffAngle = 0.0f;
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
                            float diffAngle = boneTypeAngleDiff(mTime, sTime, CurBonetype);
                            averageTimeCount = 1;

                            sumDiffAngle += diffAngle;
                            while (row > 0 && col > 0)
                            {
                                int minNeighbourIndex = GetMinNeighbourIndex1(dtwMatrix, row, col);
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
                                mTime = masterTimeData[row];
                                sTime = studentTimeData[col];
                                diffAngle = boneTypeAngleDiff(mTime, sTime, CurBonetype);

                                averageTimeCount++;

                                sumDiffAngle += diffAngle;
                            }
                            allBoneDiffAngle += sumDiffAngle;
                            diffDtwData.Add(CurBonetype, sumDiffAngle);
                        }

                    }
                    else
                    {
                        continue;
                    }

                    float averageBonetypeData = allBoneTypeDiffData / averageTimeCount;

                    string[] allBonetypeDataStr = { "all bonetype all timecount average diff Data: ", averageBonetypeData.ToString() };
                    string averageAllBoneAllTimecountPath = Application.dataPath + "/zTestData/allBTAverageDiffData.txt";
                    File.AppendAllLines(averageAllBoneAllTimecountPath, allBonetypeDataStr);
                }
   

                for (int i = 0; i < diffDtwData.Count; i++)
                {
                    JAnimationData.BoneType curbonetype = diffDtwData.Keys.ElementAt(i);
                    float diffAngle = diffDtwData.Values.ElementAt(i);
                    float diffPercent = diffAngle / allBoneDiffAngle;
                    string bonetypeStr = curbonetype.ToString();
                    string diffAngleStr = diffAngle.ToString();
                    string diffPercentStr = diffPercent.ToString();
                    string sumDiffStr = bonetypeStr + ": " + diffAngleStr + " percent: " + diffPercentStr;
                    string[] sumDiffAngleStr = { sumDiffStr };

                

                    File.AppendAllLines(boneAngleDiffPath, sumDiffAngleStr);
                }

                Debug.Log("txt has been created in " + boneAngleDiffPath);
            }
        }

    }

    public class dtwAngle
    {
        public float Time { get; set; }
        public float Value { get; set; }
    }

  
    public void newDtwTest()//24.03.03 用的这个
    {
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();

        //master time

        float interval = endTime - startTime;//选段的间隔
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
                    float curTime = startTime;
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
                        
                        Debug.Log("index: " + index + " studentIndex: " + studentIndex);
                        List<dtwAngle> masterSequence = new List<dtwAngle>();//存储角度序列
                        List<dtwAngle> studentSequence = new List<dtwAngle>();

                        //master存储角度序列
                        JVehicleFrameSystem.BoneFramework.Bone masterTargetBone = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.GetBoneByType(masterBonetype);
                        int childNum = masterTargetBone.ChildsList.Count;
                        if (childNum == 1)
                        {
                            
                            for(int j = 0; j < TimeCount; j++)
                            {
                                float curMasterBoneAngle = vfplayer.ch_master.bm_p1.GetWorldRoa_Ztest(curTime,masterBonetype, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
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
                        if(studentChildNum == 1)
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
        Dictionary<string,float[,]> distanceMatrices = new Dictionary<string, float[,]>();

        foreach (var jointName in masterJointSequences.Keys)
        {
            if (studentJointSequences.ContainsKey(jointName))
            {
                var masterSequence = masterJointSequences[jointName];
                var studentSequence = studentJointSequences[jointName];
                int mn = masterSequence.Count;
                int sm = studentSequence.Count;
                float[,] dtw = new float[mn , sm ];

                //初始化dtw矩阵
                for(int i = 1;i < mn; i++)
                {
                    dtw[i, 0] = float.MaxValue;
                }
                for(int j = 1; j < sm; j++)
                {
                    dtw[0, j] = float.MaxValue;
                }
                dtw[0, 0] = 0;
                
                //计算每个关节点各自的dtw距离矩阵
                for(int i = 1; i < mn; i++)
                {
                    for(int j = 1;j < sm; j++)
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

            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < m; j++)
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
        calculateDtwDiff(dtwTimePairs);//处理dtw时间序列并计算 角度作为参数
        //featurePlaneDtwScore(dtwTimePairs);//特征平面为参数
        Debug.Log("bones dtw time Series has been writed in " + timeStampPath1);

    }
    public class psoJointWeights
    {
        Dictionary<JAnimationData.BoneType, float> jointWeights = new Dictionary<JAnimationData.BoneType, float>();
        
    }

    public void calculateDtwDiff(List<KeyValuePair<float,float>> timePairs)//处理dtw时间序列
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
                                float scoreDiff =jointWeights[CurBonetype] * boneAngleDiff / masterBoneAngle;//记录打分
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
        string[] wholeFileStr = { wholeStr, " " , allScoreStr };
        File.AppendAllLines(boneDiffPath, wholeFileStr);
        Debug.Log("bones dtw time Series angle Diff Data has been writed in " + boneDiffPath);
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
                featurePlaneTextField.text = "feature plane score: " + featurePlaneScore.ToString();
                Debug.Log("leftArm: " + angleCosDataLeftArm.ToString() + "  leftLeg: " + angleCosDataLeftLeg.ToString() + " Spine :" + angleCosDataSpine.ToString());
                Debug.Log("rightArm : " + angleCosDataRightArm.ToString() + " rightLeg: " + angleCosDataRightLeg.ToString());
            }
        }
    }

    public void featurePlaneDtwScore(List<KeyValuePair<float, float>> timePairs)//dtw 特征平面参数得分
    {
        Dictionary<JAnimationData.BoneType, float> jointWeights = new Dictionary<JAnimationData.BoneType, float>();
        string fPDiffPath = Application.dataPath + "/zTestData/dtwBoneFeaturePlaneDiffData.txt";
        //起霸24个关节点权重设置
        jointWeights[JAnimationData.BoneType.hips] = 1.0f;
        jointWeights[JAnimationData.BoneType.spine] = 1.0f;
        jointWeights[JAnimationData.BoneType.spine1] = 1.0f;
        jointWeights[JAnimationData.BoneType.spine2] = 1.0f;
        jointWeights[JAnimationData.BoneType.spine3] = 1.0f;
        jointWeights[JAnimationData.BoneType.neck] = 1.0f;
        jointWeights[JAnimationData.BoneType.neck1] = 1.0f;
        jointWeights[JAnimationData.BoneType.head] = 1.0f;
        jointWeights[JAnimationData.BoneType.rightShoulder] = 1.0f;
        jointWeights[JAnimationData.BoneType.rightArm] = 1.0f;
        jointWeights[JAnimationData.BoneType.rightForeArm] = 1.0f;
        jointWeights[JAnimationData.BoneType.rightHand] = 1.0f;
        jointWeights[JAnimationData.BoneType.leftShoulder] = 1.0f;
        jointWeights[JAnimationData.BoneType.leftArm] = 1.0f;
        jointWeights[JAnimationData.BoneType.leftForeArm] = 1.0f;
        jointWeights[JAnimationData.BoneType.leftHand] = 1.0f;
        jointWeights[JAnimationData.BoneType.rightUpLeg] = 1.0f;
        jointWeights[JAnimationData.BoneType.rightLeg] = 1.0f;
        jointWeights[JAnimationData.BoneType.rightFoot] = 1.0f;
        jointWeights[JAnimationData.BoneType.rightToeBase] = 1.0f;
        jointWeights[JAnimationData.BoneType.leftUpLeg] = 1.0f;
        jointWeights[JAnimationData.BoneType.leftLeg] = 1.0f;
        jointWeights[JAnimationData.BoneType.leftFoot] = 1.0f;
        jointWeights[JAnimationData.BoneType.leftToeBase] = 1.0f;

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
        double allFeaturePlaneScoreDiff = 0.0;
        
        foreach (var pair in timePairs)
        {
            timePairsCount++;
            float sumBoneAngleDiff = 0.0f;
            float sumScoreDiff = 0.0f;
            double featurePlaneScore = 0.0;
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
                                masterLeftArmAPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentLeftArmAPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }
                            if (CurBonetype == JAnimationData.BoneType.leftForeArm)
                            {
                                masterLeftArmBPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentLeftArmBPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }
                            if (CurBonetype == JAnimationData.BoneType.leftHand)
                            {
                                masterLeftArmCPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentLeftArmCPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }

                            //rightArm特征平面计算
                            if (CurBonetype == JAnimationData.BoneType.rightArm)
                            {
                                masterRightArmAPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentRightArmAPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }
                            if (CurBonetype == JAnimationData.BoneType.rightForeArm)
                            {
                                masterRightArmBPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentRightArmBPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }
                            if (CurBonetype == JAnimationData.BoneType.rightHand)
                            {
                                masterRightArmCPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentRightArmCPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }

                            //leftLeg特征平面计算
                            if (CurBonetype == JAnimationData.BoneType.leftUpLeg)
                            {
                                masterLeftLegAPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentLeftLegAPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }
                            if (CurBonetype == JAnimationData.BoneType.leftLeg)
                            {
                                masterLeftLegBPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentLeftLegBPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }
                            if (CurBonetype == JAnimationData.BoneType.leftFoot)
                            {
                                masterLeftLegCPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentLeftLegCPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }

                            //rightLeg特征平面计算
                            if (CurBonetype == JAnimationData.BoneType.rightUpLeg)
                            {
                                masterRightLegAPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentRightLegAPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }
                            if (CurBonetype == JAnimationData.BoneType.rightLeg)
                            {
                                masterRightLegBPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentRightLegBPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }
                            if (CurBonetype == JAnimationData.BoneType.rightFoot)
                            {
                                masterRightLegCPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentRightLegCPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }

                            //Spine特征平面计算
                            if (CurBonetype == JAnimationData.BoneType.spine)
                            {
                                masterSpineAPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentSpineAPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }
                            if (CurBonetype == JAnimationData.BoneType.spine1)
                            {
                                masterSpineBPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentSpineBPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_student.builder.theBoneFramework, vfplayer.ch_student.createdGA.transform.position, vfplayer.ch_student.createdGA.transform.rotation);
                            }
                            if (CurBonetype == JAnimationData.BoneType.spine2)
                            {
                                masterSpineCPos = vfplayer.ch_master.bm_p1.GetWorldPosByTime_Ztest(pair.Key, vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.dataList[index].boneType,
                                vfplayer.ch_master.builder.theBoneFramework, vfplayer.ch_master.createdGA.transform.position, vfplayer.ch_master.createdGA.transform.rotation);
                                studentSpineCPos = vfplayer.ch_student.bm_p1.GetWorldPosByTime_Ztest(pair.Value, vfplayer.animShowList[vfplayer.ch_student.createdGA_No].jAnimData.jAD.dataList[index].boneType,
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

                    featurePlaneScore = (angleCosDataLeftArm + angleCosDataLeftLeg + angleCosDataRightArm + angleCosDataRightLeg + angleCosDataSpine) / 5;
                }
            }
            allFeaturePlaneScoreDiff += featurePlaneScore;
        }
        allFeaturePlaneScoreDiff = (allFeaturePlaneScoreDiff / timePairsCount) * 100;
        Debug.Log("dtw featureplane score is : " + allFeaturePlaneScoreDiff);
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
        public float FindClosestStudentTime(List<autoDtwTimeData> dataList,float inputMasterTime)
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
        foreach(var line in lines)
        {
            var parts = line.Split(' ');
            var filteredParts = parts.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray(); // 手动过滤空字符串
            if (filteredParts.Length == 2)
            {
                if (float.TryParse(filteredParts[0], out float masterTime) && float.TryParse(filteredParts[1], out float studentTime))
                {
                    dataList.Add(new autoDtwTimeData { masterTime = masterTime,studentTime = studentTime });
                }
            }
        }
        var finder = new TimeDataFinder(dataList);
        float closestStudentTime = finder.FindClosestStudentTime(dataList,masterDtwTimeData);
        studentDtwTime.text = closestStudentTime.ToString("F2");
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



    public void staticAction()//静态动作对比打分
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
                if(scoreTextField != null)
                {
                    scoreTextField.text = sumScoreDiff.ToString();
                }
                Debug.Log("score update ");
            }
        }
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
    public void angleGap()
    {
        List<int> dataList;
        masterAngleGapData = new Dictionary<string, List<float>>();
        VFPlayer_InGame vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        float timeCount = (int)((endTime - startTime) / 0.1f);
        int masterBoneCount = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones.Count;
        dataList = new List<int>();

        //处理master数据
        string fileMasterPath = Application.dataPath + "/zTestData/masterAngleData.txt";
        string fileTimeGapPath = Application.dataPath + "/zTestData/AngleTimeGapMasterData.txt";
        string fileTimeGapSumPath = Application.dataPath + "/zTestData/AngleTimeGapSumMasterData.txt";
        if (vfplayer.ch_master.createdGA_No != -1)
        {
            for (int i = 0; i < masterBoneCount; i++)
            {
                JAnimationData.BoneType masterBonetype = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.Bones[i].boneType;
                string masterStr = masterBonetype.ToString();
                int index = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.GetTypeIndex(masterBonetype);
                if (index != -1)
                {
                    JVehicleFrameSystem.BoneFramework.Bone targetBone = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].frame.boneFramework.GetBoneByType(masterBonetype);
                    int childNum = targetBone.ChildsList.Count;
                    if (childNum == 1)
                    {
                        if (masterBonetype == JAnimationData.BoneType.leftToeBase) continue;
                        if (masterBonetype == JAnimationData.BoneType.rightToeBase) continue;
                        if (masterBonetype == JAnimationData.BoneType.leftFoot) continue;
                        if (masterBonetype == JAnimationData.BoneType.rightFoot) continue;
                        using (StreamReader reader = new StreamReader(fileMasterPath))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line == masterStr)
                                {
                                    float sumTimeGap = 0.0f;
                                    string[] masterBoneName = { masterStr };
                                    File.AppendAllLines(fileTimeGapPath, masterBoneName);
                                    line = reader.ReadLine();
                                    string[] values1 = line.Split(':');

                                    float masterAngleTimePreData = float.Parse(values1[1]);


                                    for (int j = 1; j < timeCount; j++)
                                    {
                                        line = reader.ReadLine();
                                        string[] values = line.Split(':');
                                        float timeStamp = float.Parse(values[0]);
                                        float timeAngleData = float.Parse(values[1]);

                                        float timeGapData = System.Math.Abs(timeAngleData - masterAngleTimePreData);


                                        sumTimeGap += timeGapData;
                                        string timeStampStr = timeStamp.ToString("F2") + ":";
                                        string timeAngleGapData = timeStampStr + timeGapData.ToString();


                                        string[] timeGapDataStr = { timeAngleGapData };
                                        File.AppendAllLines(fileTimeGapPath, timeGapDataStr);
                                        masterAngleTimePreData = timeAngleData;
                                    }
                                    sumTimeGap = sumTimeGap / timeCount;
                                    string sumStr = masterStr + ":" + sumTimeGap;
                                    string[] sumGapf = { sumStr };
                                    File.AppendAllLines(fileTimeGapSumPath, sumGapf);
                                    break;
                                }

                            }
                        }
                    }


                }

            }
            Debug.Log("txt has been created in " + fileTimeGapPath);
        }
        //处理student数据

        float studentTimeCount = (int)((studentEndTime - studentStartTime) / 0.1f);
        int studentBoneCount = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.Bones.Count;
        string fileStudentPath = Application.dataPath + "/zTestData/studentAngleData.txt";
        string fileTimeGapStudentPath = Application.dataPath + "/zTestData/AngleTimeGapStudentData.txt";
        string fileTimeGapSumStudentPath = Application.dataPath + "/zTestData/AngleTimeGapSumStudentData.txt";
        if (vfplayer.ch_student.createdGA_No != -1)
        {
            for (int i = 0; i < studentBoneCount; i++)
            {
                JAnimationData.BoneType studentBonetype = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.Bones[i].boneType;
                string studentStr = studentBonetype.ToString();
                int index = vfplayer.animShowList[vfplayer.ch_master.createdGA_No].jAnimData.jAD.GetTypeIndex(studentBonetype);
                if (index != -1)
                {
                    JVehicleFrameSystem.BoneFramework.Bone targetBone = vfplayer.animShowList[vfplayer.ch_student.createdGA_No].frame.boneFramework.GetBoneByType(studentBonetype);
                    int childNum = targetBone.ChildsList.Count;
                    if (childNum == 1)
                    {
                        if (studentBonetype == JAnimationData.BoneType.leftToeBase) continue;
                        if (studentBonetype == JAnimationData.BoneType.rightToeBase) continue;
                        if (studentBonetype == JAnimationData.BoneType.leftFoot) continue;
                        if (studentBonetype == JAnimationData.BoneType.rightFoot) continue;
                        using (StreamReader reader = new StreamReader(fileStudentPath))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line == studentStr)
                                {
                                    float sumTimeGap = 0.0f;
                                    string[] studentBoneName = { studentStr };
                                    File.AppendAllLines(fileTimeGapStudentPath, studentBoneName);
                                    line = reader.ReadLine();
                                    string[] values1 = line.Split(':');

                                    float studentAngleTimePreData = float.Parse(values1[1]);


                                    for (int j = 1; j < studentTimeCount; j++)
                                    {
                                        line = reader.ReadLine();
                                        string[] values = line.Split(':');
                                        float timeStamp = float.Parse(values[0]);
                                        float timeAngleData = float.Parse(values[1]);

                                        float timeGapData = System.Math.Abs(timeAngleData - studentAngleTimePreData);


                                        sumTimeGap += timeGapData;
                                        string timeStampStr = timeStamp.ToString() + ":";
                                        string timeAngleGapData = timeStampStr + timeGapData.ToString();


                                        string[] timeGapDataStr = { timeAngleGapData };
                                        File.AppendAllLines(fileTimeGapStudentPath, timeGapDataStr);
                                        studentAngleTimePreData = timeAngleData;
                                    }
                                    sumTimeGap = sumTimeGap / studentTimeCount;
                                    string sumStr = studentStr + ":" + sumTimeGap;
                                    string[] sumGapf = { sumStr };
                                    File.AppendAllLines(fileTimeGapSumStudentPath, sumGapf);
                                    break;
                                }

                            }
                        }
                    }


                }

            }
            Debug.Log("txt has been created in " + fileTimeGapStudentPath);
        }
    }


    static void pyUnityTest()
    {
        Debug.Log("python to unity cmd test success");
    }
}
