using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JAnimationSystem;
using JVehicleFrameSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.Video;

public class VFPlayer_InGame_beifen : MonoBehaviour
{



    [System.Serializable]
    public class UIclass1
    {
        public Button playButton;
        public Slider slider1;
        public Slider slider2;
        public InputField input_min, input_max;
        public InputField input_min2, input_max2;
        public Text timeNowText_Master;
        public Text timeNowText_student;
    }


    [System.Serializable]
    public class animShow_Model
    {
        public string name = "模型";
        /// <summary>
        /// 每帧动画播放后都使用程序化的校准进行骨骼贴合
        /// </summary>
        public bool useBoneAdapt = false;
        public Vector3 pos_model;
        public Vector3 roa_model;
        public GameObject frame_model;
        public JAnimation_AutoSetBoneMatchData autodata_model;
    }

    [System.Serializable]
    public class animShow
    {
        public string name = "动作数据";
        public string introduction = "介绍";
        public Vector3 pos;
        public Vector3 roa;
        public JAnimationData jAnimData;
        public JVehicleFrame frame;
        public JAnimation_AutoSetBoneMatchData autodata;
        public List<AnimRangeControl.SelectOne> selectRanges = new List<AnimRangeControl.SelectOne>();

        public VideoClip videoClip;
        public float videoTimeOffset = 0;
    }
    [System.Serializable]
    public class transformPair
    {
        public Transform t1;
        public Transform t2;

        public transformPair(Transform _t1, Transform _t2)
        {
            t1 = _t1;
            t2 = _t2;
        }
    }
    /// <summary>
    /// 为了能展示骨骼动画录制数据到界面，这里立一种文件类型
    /// </summary>
    [System.Serializable]
    public class JAnimRecodesShow
    {
        /// <summary>
        /// 文件名（带拓展名）
        /// </summary>
        public string fileName;
        /// <summary>
        /// 文件全名（路径）
        /// </summary>
        public string fileFullName;

        public JAnimRecodesShow(string _fileName, string _fileFullName)
        {
            fileName = _fileName;
            fileFullName = _fileFullName;
        }
    }

    public UIclass1 ui1 = new UIclass1();
    /// <summary>
    /// 内置动作数据列表
    /// </summary>
    public List<animShow> animShowList = new List<animShow>();
    /// <summary>
    /// 内置模型列表
    /// </summary>
    public List<animShow_Model> animModelList = new List<animShow_Model>();
    /// <summary>
    /// 录制动作数据所用的模型预制体列表
    /// </summary>
    public List<GameObject> animPrefabsList = new List<GameObject>();
    private List<JAnimRecodesShow> animRecodesShowList = new List<JAnimRecodesShow>();
    /// <summary>
    /// 从录制的动作数据加载的选段数据
    /// </summary>
    [HideInInspector]
    public JAnimDataToFile.JAnimRanges loadedJARanges = new JAnimDataToFile.JAnimRanges();
    public ScrollViewUI scrollView_master;
    public ScrollViewUI scrollView_student;
    public Dropdown Dd_Select_Master;
    public Dropdown Dd_Select_Student;

    /// <summary>
    /// 大师 学生 录制数据点击应用后会出现的窗口的预制体
    /// </summary>
    public GameObject AnimInfoWindowPrefab;
    /// <summary>
    /// 点击骨骼后会出现的窗口的预制体
    /// </summary>
    public GameObject BoneInfoWindowPrefab;
    /// <summary>
    /// 曲线显示窗口
    /// </summary>
    public CurveShowWindowController curveShowWindowControl;

    public PositionControl posControl;

    public Button PlayVideoButton;
    public videoControl vControl;

    /// <summary>
    /// 选择的骨骼
    /// </summary>
    [System.Serializable]
    public class CharacterSelectBone
    {
        public JAnimationData.BoneType Type
        {
            get
            {
                return type;
            }
        }
        private JAnimationData.BoneType type;
        public Transform Bone
        {
            get
            {
                return bone;
            }
        }
        private Transform bone;
        public Color SrcColor
        {
            get
            {
                return srcColor;
            }
        }
        private Color srcColor;
        public Renderer BoneRender
        {
            get
            {
                return boneRender;
            }
        }
        private Renderer boneRender;
        public CharacterSelectBone(JAnimationData.BoneType _type, Transform _bone, Color _srcColor, Renderer _boneRender)
        {
            type = _type;
            bone = _bone;
            srcColor = _srcColor;
            boneRender = _boneRender;
        }
    }

    /// <summary>
    /// 角色操作的类
    /// </summary>
    [System.Serializable]
    public class ShowingCharacter
    {
        public float timer = 0f;
        public float time = 0f;
        public float min = 0f;
        public float max = 0f;
        public bool isplaying = false;
        public bool hasInit = false;
        public BoneMatch_Player bm_p1 = null;
        public VehicleBuilder builder;
        [HideInInspector]
        public GameObject createdGA = null;
        public JAnimationData.JAD jad_created = null;
        /// <summary>
        /// 是创建出来了哪个数据
        /// </summary>
        public int createdGA_No = -1;
        public float sliderValue = 0f;

        /// <summary>
        /// 点选的骨骼列表
        /// </summary>
        public List<CharacterSelectBone> selectBones = new List<CharacterSelectBone>();

        /// <summary>
        /// 附加的
        /// </summary>
        public List<BoneMatch_Player> added_bm_p = new List<BoneMatch_Player>();
        /// <summary>
        /// 附加的
        /// </summary>
        public List<GameObject> added_createdGA = new List<GameObject>();

        /// <summary>
        /// 附加的模型的序号
        /// </summary>
        public List<int> added_ModelNum = new List<int>();
        /// <summary>
        /// 每帧动画播放后都使用程序化的校准进行骨骼贴合，这里的参数是创建了模型后进行设定。
        /// </summary>
        public bool useBoneAdapt = false;

        /// <summary>
        /// 在选择了的骨骼列表里寻找是否有类型为boneType1的。如果有返回序号，如果没有返回-1
        /// </summary> 
        public int GetIndexOfSelectBone(JAnimationData.BoneType boneType1)
        {
            for (int i = 0; i < selectBones.Count; i++)
            {
                if (selectBones[i].Type == boneType1)
                {
                    return i;
                }
            }
            return -1;
        }

        public ShowingCharacter()
        {
            timer = 0f;
            time = 0f;
            min = 0f;
            max = 0f;
            isplaying = false;
            hasInit = false;
            bm_p1 = null;
            builder = null;
            createdGA = null;
            jad_created = null;
            createdGA_No = -1;
            sliderValue = 0f;

            selectBones = new List<CharacterSelectBone>();

            added_bm_p = new List<BoneMatch_Player>();
            added_createdGA = new List<GameObject>();

            useBoneAdapt = false;
        }
    }
    [HideInInspector]
    public ShowingCharacter ch_master = new ShowingCharacter();
    [HideInInspector]
    public ShowingCharacter ch_student = new ShowingCharacter();



    public List<transformPair> tPairs = new List<transformPair>();


    void Start()
    {
        InitUIs();
        PlayVideoButton.interactable = false;
    }

    public void Button_CloseApp()
    {
        Application.Quit();
    }

    public void Button_OpenFileFolder()
    {
        string pdfPath = Application.persistentDataPath;
        string expPath = pdfPath.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", expPath);
    }

    /// <summary>
    /// 获取一个骨骼下面有多少有效得骨骼
    /// </summary>
    /// <returns></returns>
    private int GetBoneChildCount(BoneMatch_Player _player, Transform _t1)
    {
        if (_t1.childCount > 0)
        {
            int c1 = 0;
            for (int i = 0; i < _t1.childCount; i++)
            {
                if (_player.GetPlayerIndexByMatchedT(_t1.GetChild(i)) != -1)
                {
                    c1++;
                }
            }
            return c1;
        }
        return 0;
    }
    /// <summary>
    /// 获取子对象里面的第一个有效的骨骼类型
    /// </summary>
    /// <returns></returns>
    private JAnimationData.BoneType GetFirstRealBoneChild(BoneMatch_Player _player, Transform _t1)
    {
        if (_t1.childCount > 0)
        {
            for (int i = 0; i < _t1.childCount; i++)
            {
                int index1 = _player.GetPlayerIndexByMatchedT(_t1.GetChild(i));
                if (index1 != -1)
                {
                    return _player.bonePlayers[index1].boneOneMatch.boneType;
                }
            }
        }
        return JAnimationData.BoneType.unknowType;
    }
    private void followHip(BoneMatch_Player target, BoneMatch_Player source)
    {
        int index_source = source.GetPlayerIndexByBoneType(JAnimationData.BoneType.hips);
        int index_target = target.GetPlayerIndexByBoneType(JAnimationData.BoneType.hips);
        Vector3 nPos = source.bonePlayers[index_source].boneOneMatch.matchedT.position;
        nPos.y = target.bonePlayers[index_target].boneOneMatch.matchedT.position.y;
        target.bonePlayers[index_target].boneOneMatch.matchedT.position = nPos;
    }
    private void followBone(BoneMatch_Player target, BoneMatch_Player source, Transform t1)
    {
        if (t1 == null)
        {
            return;
        }
        int index1 = target.GetPlayerIndexByMatchedT(t1);
        if (index1 != -1)
        {
            int index2 = source.GetPlayerIndexByBoneType(target.bonePlayers[index1].boneOneMatch.boneType);
            if (index2 != -1)
            {
                if (source.bonePlayers[index2].boneOneMatch.matchedT != null)
                {
                    t1.position = source.bonePlayers[index2].boneOneMatch.matchedT.position;

                    int chCount_source = GetBoneChildCount(source, source.bonePlayers[index2].boneOneMatch.matchedT);//有效骨骼数量
                    int chCount = GetBoneChildCount(target, t1);//有效骨骼数量
                    if (chCount == 1 && chCount_source == 1)
                    {
                        //Debug.Log("ddadwafwgwag");
                        JAnimationData.BoneType type_t = GetFirstRealBoneChild(target, t1);
                        JAnimationData.BoneType type_s = GetFirstRealBoneChild(source, source.bonePlayers[index2].boneOneMatch.matchedT);
                        if (type_t == type_s && type_t != JAnimationData.BoneType.unknowType) //父和子必须是类型一一对应
                        {
                            Transform parent_s = source.bonePlayers[index2].boneOneMatch.matchedT;
                            Transform child_t = target.bonePlayers[target.GetPlayerIndexByBoneType(type_t)].boneOneMatch.matchedT;
                            Transform child_s = source.bonePlayers[source.GetPlayerIndexByBoneType(type_s)].boneOneMatch.matchedT;
                            Vector3 f1 = child_t.position - t1.position;
                            f1.Normalize();
                            Vector3 f2 = child_s.position - parent_s.position;
                            f2.Normalize();
                            float a1 = Vector3.Angle(f1, f2);
                            Vector3 up1 = Vector3.Cross(f1, f2);
                            t1.rotation = Quaternion.AngleAxis(a1, up1) * t1.rotation;
                            //Debug.Log("ddadwafwgwag");
                        }
                    }
                }
            }
        }
        for (int i = 0; i < t1.childCount; i++)
        {
            followBone(target, source, t1.GetChild(i));
        }
    }

    void Update()
    {
        if (ch_master.hasInit)
        {
            if (ch_master.isplaying)
            {
                ch_master.timer += Time.deltaTime;
                ch_master.bm_p1.PlayDataInThisFrame_Curve(ch_master.timer);
                if (ch_master.timer < ch_master.min)
                {
                    ch_master.timer = ch_master.min;
                }
                ui1.timeNowText_Master.text = ch_master.timer.ToString("f2") + "/" + ch_master.time.ToString("f2");
                //ui1.timeNowText.text = timer.ToString();
                ui1.slider1.value = time1ToSlider(ch_master.timer);

                ch_master.sliderValue = time1ToSlider(ch_master.timer);
                if (ch_master.timer >= ch_master.max)
                {
                    ch_master.timer = ch_master.max;
                    ch_master.isplaying = false;
                    /*if (selectDropdown.value == 0)
                    {
                        ui1.slider1.value = 1f;
                    }*/
                    ui1.slider1.value = 1f;

                    ch_master.sliderValue = 1f;
                }

                if (vControl.isShowing)
                {
                    if (vControl.player.isPaused)
                    {
                        vControl.player.time = ch_master.timer + animShowList[ch_master.createdGA_No].videoTimeOffset;
                        vControl.Play();
                    }
                    if (vControl.player.isPlaying == false)
                    {
                        vControl.player.time = ch_master.timer + animShowList[ch_master.createdGA_No].videoTimeOffset;
                        vControl.Play();
                    }
                    if (vControl.player.time - (ch_master.timer + animShowList[ch_master.createdGA_No].videoTimeOffset) >= 0.01f)
                    {
                        vControl.player.Pause();
                        vControl.player.time = ch_master.timer + animShowList[ch_master.createdGA_No].videoTimeOffset;
                        vControl.Play();
                    }
                }
            }
            else
            {//isplaying == false
                if (ch_master.timer < ch_master.min)
                {
                    ch_master.timer = ch_master.min;
                }
                if (ch_master.timer > ch_master.max)
                {
                    ch_master.timer = ch_master.max;
                }
                ch_master.timer = sliderToTime1(ui1.slider1.value);
                ui1.timeNowText_Master.text = (ch_master.timer).ToString("f2") + "/" + ch_master.time.ToString("f2");
                ch_master.sliderValue = ui1.slider1.value;

                ch_master.bm_p1.PlayDataInThisFrame_Curve(sliderToTime1(ui1.slider1.value));

                //吸附
                if (ch_master.createdGA_No != -1)
                {
                    float testJiange1 = 20f / ui1.slider1.gameObject.GetComponent<RectTransform>().rect.width * ch_master.time / 2f;
                    float xifuDistance = 10000f;
                    for (int i = 0; i < animShowList[ch_master.createdGA_No].selectRanges.Count; i++)
                    {
                        if (animShowList[ch_master.createdGA_No].selectRanges[i].type == AnimRangeControl.SelectType.timePoint)
                        {
                            if (isTimeClose(animShowList[ch_master.createdGA_No].selectRanges[i].timePoint, ch_master.timer, testJiange1) == true)
                            {
                                if (Mathf.Abs(animShowList[ch_master.createdGA_No].selectRanges[i].timePoint - ch_master.timer) < xifuDistance)
                                {
                                    xifuDistance = Mathf.Abs(animShowList[ch_master.createdGA_No].selectRanges[i].timePoint - ch_master.timer);

                                    ch_master.timer = animShowList[ch_master.createdGA_No].selectRanges[i].timePoint;
                                    OnDragSlider(time1ToSlider(animShowList[ch_master.createdGA_No].selectRanges[i].timePoint));
                                    ui1.slider1.value = time1ToSlider(ch_master.timer);
                                }
                            }
                        }
                        else if (animShowList[ch_master.createdGA_No].selectRanges[i].type == AnimRangeControl.SelectType.timeRange)
                        {
                            if (isTimeClose(animShowList[ch_master.createdGA_No].selectRanges[i].timeRange_start, ch_master.timer, testJiange1) == true)
                            {
                                if (Mathf.Abs(animShowList[ch_master.createdGA_No].selectRanges[i].timeRange_start - ch_master.timer) < xifuDistance)
                                {
                                    xifuDistance = Mathf.Abs(animShowList[ch_master.createdGA_No].selectRanges[i].timeRange_start - ch_master.timer);

                                    ch_master.timer = animShowList[ch_master.createdGA_No].selectRanges[i].timeRange_start;
                                    OnDragSlider(time1ToSlider(animShowList[ch_master.createdGA_No].selectRanges[i].timeRange_start));
                                    ui1.slider1.value = time1ToSlider(ch_master.timer);
                                }
                            }
                            if (isTimeClose(animShowList[ch_master.createdGA_No].selectRanges[i].timeRange_end, ch_master.timer, testJiange1) == true)
                            {
                                if (Mathf.Abs(animShowList[ch_master.createdGA_No].selectRanges[i].timeRange_end - ch_master.timer) < xifuDistance)
                                {
                                    xifuDistance = Mathf.Abs(animShowList[ch_master.createdGA_No].selectRanges[i].timeRange_end - ch_master.timer);

                                    ch_master.timer = animShowList[ch_master.createdGA_No].selectRanges[i].timeRange_end;
                                    OnDragSlider(time1ToSlider(animShowList[ch_master.createdGA_No].selectRanges[i].timeRange_end));
                                    ui1.slider1.value = time1ToSlider(ch_master.timer);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("Error!!!");
                }

                if (vControl.isShowing)
                {
                    if (vControl.player.isPaused == false)
                    {
                        vControl.player.Pause();
                    }
                    vControl.player.time = ch_master.timer + animShowList[ch_master.createdGA_No].videoTimeOffset;
                }
            }

            for (int i = 0; i < ch_master.added_bm_p.Count; i++)//added
            {
                ch_master.added_bm_p[i].PlayDataInThisFrame_Curve(sliderToTime1(ui1.slider1.value));
                if (ch_master.useBoneAdapt)
                {
                    followBone(ch_master.added_bm_p[i], ch_master.bm_p1, ch_master.added_bm_p[i].bonePlayers[0].boneOneMatch.matchedT.root);
                }
                else
                {
                    followHip(ch_master.added_bm_p[i], ch_master.bm_p1);
                }
            }
        }
        else
        {
            ui1.timeNowText_Master.text = "0/0";
            ch_master.sliderValue = 0f;
        }

        if (ch_student.hasInit)
        {
            if (ch_student.isplaying)
            {
                ch_student.timer += Time.deltaTime;
                ch_student.bm_p1.PlayDataInThisFrame_Curve(ch_student.timer);
                if (ch_student.timer < ch_student.min)
                {
                    ch_student.timer = ch_student.min;
                }
                ui1.timeNowText_student.text = ch_student.timer.ToString("f2") + "/" + ch_student.time.ToString("f2");
                ui1.slider2.value = time2ToSlider(ch_student.timer);

                ch_student.sliderValue = time2ToSlider(ch_student.timer);
                if (ch_student.timer >= ch_student.max)
                {
                    ch_student.timer = ch_student.max;
                    ch_student.isplaying = false;
                    /*if (selectDropdown.value == 1)
                    {
                        ui1.slider1.value = 1f;
                    }*/
                    ui1.slider2.value = 1f;
                    ch_student.sliderValue = 1f;
                }

            }
            else
            {//isplaying2 == false
                if (ch_student.timer < ch_student.min)
                {
                    ch_student.timer = ch_student.min;
                }
                if (ch_student.timer > ch_student.max)
                {
                    ch_student.timer = ch_student.max;
                }
                ch_student.timer = sliderToTime2(ui1.slider2.value);
                ui1.timeNowText_student.text = (ch_student.timer).ToString("f2") + "/" + ch_student.time.ToString("f2");
                ch_student.sliderValue = ui1.slider2.value;

                ch_student.bm_p1.PlayDataInThisFrame_Curve(sliderToTime2(ui1.slider2.value));

                //吸附
                if (ch_student.createdGA_No != -1)
                {
                    float testJiange1 = 20f / ui1.slider2.gameObject.GetComponent<RectTransform>().rect.width * ch_student.time / 2f;
                    float xifuDistance = 10000f;
                    if (ch_student.createdGA_No < animShowList.Count)//内置数据
                    {
                        for (int i = 0; i < animShowList[ch_student.createdGA_No].selectRanges.Count; i++)
                        {
                            if (animShowList[ch_student.createdGA_No].selectRanges[i].type == AnimRangeControl.SelectType.timePoint)
                            {
                                if (isTimeClose(animShowList[ch_student.createdGA_No].selectRanges[i].timePoint, ch_student.timer, testJiange1) == true)
                                {
                                    if (Mathf.Abs(animShowList[ch_student.createdGA_No].selectRanges[i].timePoint - ch_student.timer) < xifuDistance)
                                    {
                                        xifuDistance = Mathf.Abs(animShowList[ch_student.createdGA_No].selectRanges[i].timePoint - ch_student.timer);

                                        ch_student.timer = animShowList[ch_student.createdGA_No].selectRanges[i].timePoint;
                                        OnDragSlider2(time2ToSlider(animShowList[ch_student.createdGA_No].selectRanges[i].timePoint));
                                        ui1.slider2.value = time2ToSlider(ch_student.timer);
                                    }
                                }
                            }
                            else if (animShowList[ch_student.createdGA_No].selectRanges[i].type == AnimRangeControl.SelectType.timeRange)
                            {
                                if (isTimeClose(animShowList[ch_student.createdGA_No].selectRanges[i].timeRange_start, ch_student.timer, testJiange1) == true)
                                {
                                    if (Mathf.Abs(animShowList[ch_student.createdGA_No].selectRanges[i].timeRange_start - ch_student.timer) < xifuDistance)
                                    {
                                        xifuDistance = Mathf.Abs(animShowList[ch_student.createdGA_No].selectRanges[i].timeRange_start - ch_student.timer);

                                        ch_student.timer = animShowList[ch_student.createdGA_No].selectRanges[i].timeRange_start;
                                        OnDragSlider2(time2ToSlider(animShowList[ch_student.createdGA_No].selectRanges[i].timeRange_start));
                                        ui1.slider2.value = time2ToSlider(ch_student.timer);
                                    }
                                }
                                if (isTimeClose(animShowList[ch_student.createdGA_No].selectRanges[i].timeRange_end, ch_student.timer, testJiange1) == true)
                                {
                                    if (Mathf.Abs(animShowList[ch_student.createdGA_No].selectRanges[i].timeRange_end - ch_student.timer) < xifuDistance)
                                    {
                                        xifuDistance = Mathf.Abs(animShowList[ch_student.createdGA_No].selectRanges[i].timeRange_end - ch_student.timer);

                                        ch_student.timer = animShowList[ch_student.createdGA_No].selectRanges[i].timeRange_end;
                                        OnDragSlider2(time2ToSlider(animShowList[ch_student.createdGA_No].selectRanges[i].timeRange_end));
                                        ui1.slider2.value = time2ToSlider(ch_student.timer);
                                    }
                                }
                            }
                        }
                    }
                    else if (ch_student.createdGA_No < animShowList.Count + animRecodesShowList.Count)
                    {
                        for (int i = 0; i < loadedJARanges.selectRanges.Count; i++)
                        {
                            if (loadedJARanges.selectRanges[i].type == AnimRangeControl.SelectType.timePoint)
                            {
                                if (isTimeClose(loadedJARanges.selectRanges[i].timePoint, ch_student.timer, testJiange1) == true)
                                {
                                    if (Mathf.Abs(loadedJARanges.selectRanges[i].timePoint - ch_student.timer) < xifuDistance)
                                    {
                                        xifuDistance = Mathf.Abs(loadedJARanges.selectRanges[i].timePoint - ch_student.timer);

                                        ch_student.timer = loadedJARanges.selectRanges[i].timePoint;
                                        OnDragSlider2(time2ToSlider(loadedJARanges.selectRanges[i].timePoint));
                                        ui1.slider2.value = time2ToSlider(ch_student.timer);
                                    }
                                }
                            }
                            else if (loadedJARanges.selectRanges[i].type == AnimRangeControl.SelectType.timeRange)
                            {
                                if (isTimeClose(loadedJARanges.selectRanges[i].timeRange_start, ch_student.timer, testJiange1) == true)
                                {
                                    if (Mathf.Abs(loadedJARanges.selectRanges[i].timeRange_start - ch_student.timer) < xifuDistance)
                                    {
                                        xifuDistance = Mathf.Abs(loadedJARanges.selectRanges[i].timeRange_start - ch_student.timer);

                                        ch_student.timer = loadedJARanges.selectRanges[i].timeRange_start;
                                        OnDragSlider2(time2ToSlider(loadedJARanges.selectRanges[i].timeRange_start));
                                        ui1.slider2.value = time2ToSlider(ch_student.timer);
                                    }
                                }
                                if (isTimeClose(loadedJARanges.selectRanges[i].timeRange_end, ch_student.timer, testJiange1) == true)
                                {
                                    if (Mathf.Abs(loadedJARanges.selectRanges[i].timeRange_end - ch_student.timer) < xifuDistance)
                                    {
                                        xifuDistance = Mathf.Abs(loadedJARanges.selectRanges[i].timeRange_end - ch_student.timer);

                                        ch_student.timer = loadedJARanges.selectRanges[i].timeRange_end;
                                        OnDragSlider2(time2ToSlider(loadedJARanges.selectRanges[i].timeRange_end));
                                        ui1.slider2.value = time2ToSlider(ch_student.timer);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Error!!!");
                    }
                }
                else
                {
                    Debug.LogError("Error!!!");
                }
            }

            for (int i = 0; i < ch_student.added_bm_p.Count; i++)//added
            {
                ch_student.added_bm_p[i].PlayDataInThisFrame_Curve(sliderToTime2(ui1.slider2.value));
                if (ch_student.useBoneAdapt)
                {
                    followBone(ch_student.added_bm_p[i], ch_student.bm_p1, ch_student.added_bm_p[i].bonePlayers[0].boneOneMatch.matchedT.root);
                }
                else
                {

                    followHip(ch_student.added_bm_p[i], ch_student.bm_p1);
                }
            }
        }
        else
        {
            ui1.timeNowText_student.text = "0/0";
            ch_student.sliderValue = 0f;
        }


        //小键盘移动
        float fps = 60f;
        float jiange_frame = 1f / fps;
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (ch_master.max > 0f)
                {
                    int frameNow = Mathf.FloorToInt(ch_master.timer / jiange_frame);
                    float tempTime = frameNow * jiange_frame - jiange_frame * 0.5f;
                    if (tempTime >= ch_master.timer) tempTime -= jiange_frame;//确保有效操作
                    if (tempTime < 0f) tempTime = 0f;
                    if (tempTime > ch_master.max) tempTime = ch_master.max;
                    ch_master.timer = tempTime;
                    OnDragSlider(time1ToSlider(tempTime));
                    ui1.slider1.value = time1ToSlider(tempTime);
                }
            }
            else
            {
                if (ch_student.max > 0f)
                {
                    int frameNow = Mathf.FloorToInt(ch_student.timer / jiange_frame);
                    float tempTime = frameNow * jiange_frame - jiange_frame * 0.5f;
                    if (tempTime >= ch_student.timer) tempTime -= jiange_frame;//确保有效操作
                    if (tempTime < 0f) tempTime = 0f;
                    if (tempTime > ch_student.max) tempTime = ch_student.max;
                    ch_student.timer = tempTime;
                    OnDragSlider(time2ToSlider(tempTime));
                    ui1.slider2.value = time2ToSlider(tempTime);
                }
            }

        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (ch_master.max > 0f)
                {
                    int frameNow = Mathf.FloorToInt(ch_master.timer / jiange_frame);
                    float tempTime = frameNow * jiange_frame + jiange_frame * 0.5f + jiange_frame;
                    if (tempTime <= ch_master.timer) tempTime += jiange_frame;//确保有效操作
                    if (tempTime < 0f) tempTime = 0f;
                    if (tempTime > ch_master.max) tempTime = ch_master.max;
                    ch_master.timer = tempTime;
                    OnDragSlider(time1ToSlider(tempTime));
                    ui1.slider1.value = time1ToSlider(tempTime);
                }
            }
            else
            {
                if (ch_student.max > 0f)
                {
                    int frameNow = Mathf.FloorToInt(ch_student.timer / jiange_frame);
                    float tempTime = frameNow * jiange_frame + jiange_frame * 0.5f + jiange_frame;
                    if (tempTime <= ch_student.timer) tempTime += jiange_frame;//确保有效操作
                    if (tempTime < 0f) tempTime = 0f;
                    if (tempTime > ch_student.max) tempTime = ch_student.max;
                    ch_student.timer = tempTime;
                    OnDragSlider(time2ToSlider(tempTime));
                    ui1.slider2.value = time2ToSlider(tempTime);
                }
            }
        }



        if (EventSystem.current.IsPointerOverGameObject() == false)
        {


            if (Input.GetKeyUp(KeyCode.P))
            {
                AllPlay();
            }
            if (Input.GetKeyUp(KeyCode.L))
            {
                AllPause();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    ch_master.isplaying = !ch_master.isplaying;
                }
                else
                {
                    ch_student.isplaying = !ch_student.isplaying;
                }
            }
            if (Input.GetKeyUp(KeyCode.O) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
            {
                Debug.Log("    O key");
                Vector3 p1 = Vector3.zero;
                Quaternion r1 = Quaternion.Euler(0, 0, 0);
                float t1 = 0f;
                t1 = ui1.slider1.value * (ch_master.max - ch_master.min) + ch_master.min;
                p1 = ch_master.bm_p1.GetWorldPosByTime(t1, JAnimationData.BoneType.leftArm);
                r1 = ch_master.bm_p1.GetWorldRoaByTime(t1, JAnimationData.BoneType.leftArm);
                GameObject sphere1 = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("baseModels/Sphere"));
                sphere1.name = "created S1";
                sphere1.transform.parent = null;
                sphere1.transform.localScale = Vector3.one * 0.1f;
                sphere1.transform.position = p1;
                sphere1.transform.rotation = r1;
                p1 = ch_master.bm_p1.GetWorldPosByTime(t1, JAnimationData.BoneType.leftForeArm);
                r1 = ch_master.bm_p1.GetWorldRoaByTime(t1, JAnimationData.BoneType.leftForeArm);
                sphere1 = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("baseModels/Sphere"));
                sphere1.name = "created S2";
                sphere1.transform.parent = null;
                sphere1.transform.localScale = Vector3.one * 0.1f;
                sphere1.transform.position = p1;
                sphere1.transform.rotation = r1;
                p1 = ch_master.bm_p1.GetWorldPosByTime(t1, JAnimationData.BoneType.leftHand);
                r1 = ch_master.bm_p1.GetWorldRoaByTime(t1, JAnimationData.BoneType.leftHand);
                sphere1 = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("baseModels/Sphere"));
                sphere1.name = "created S3";
                sphere1.transform.parent = null;
                sphere1.transform.localScale = Vector3.one * 0.1f;
                sphere1.transform.position = p1;
                sphere1.transform.rotation = r1;
            }

            if (Input.GetMouseButtonUp(0))
            {
                CameraController camControl1 = GameObject.FindObjectOfType<CameraController>();
                if (camControl1.screenMode == CameraController.ScreenMode.One)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RayCastBoneHitter(ray);
                }
                else if (camControl1.screenMode == CameraController.ScreenMode.Two)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RayCastBoneHitter(ray);
                    Ray ray2 = camControl1.cam_2.ScreenPointToRay(Input.mousePosition);
                    RayCastBoneHitter(ray2);
                }
                else if (camControl1.screenMode == CameraController.ScreenMode.FullScreen)
                {
                    Debug.LogError("此功能需要添加!");
                }

            }

        }


    }
    public void AllPause()
    {
        Debug.Log("All Pause");
        ch_master.isplaying = false;
        ch_student.isplaying = false;
    }
    public void AllPlay()
    {
        Debug.Log("All Play");
        ch_master.isplaying = true;
        ch_student.isplaying = true;
    }
    public void RefreshCurveShowWindow()
    {
        CurveShowWindowController window_1 = FindObjectOfType<CurveShowWindowController>();
        if (window_1 != null)
        {
            if (window_1.gameObject.activeSelf)
            {
                RightStackController rightStackController1 = GameObject.FindObjectOfType<RightStackController>();
                BoneInfoWindowControl[] boneInfoWindows = FindObjectsOfType<BoneInfoWindowControl>();
                bool hasCreatedInfoWindow = false;
                if (boneInfoWindows.Length > 0)
                {
                    hasCreatedInfoWindow = true;
                }
                if (hasCreatedInfoWindow == true)
                {
                    for (int i1 = boneInfoWindows.Length - 1; i1 >= 0; i1--)
                    {
                        boneInfoWindows[i1].Button_OpenWindow();
                    }
                }
            }
        }
    }
    public void HideCurveShowWindow()
    {
        CurveShowWindowController window_1 = FindObjectOfType<CurveShowWindowController>();
        if (window_1 != null)
        {
            window_1.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 射线检测关节点
    /// </summary>
    /// <param name="_ray1"></param>
    private void RayCastBoneHitter(Ray _ray1)
    {
        RaycastHit hit;
        if (Physics.Raycast(_ray1, out hit, 100))
        {
            GameObject target = hit.collider.gameObject;//获得点击的物体 
            Debug.Log("ray: " + target.name);

            if (ch_master.bm_p1 != null)
            {
                JAnimationData.BoneType type1
                    = ch_master.bm_p1.boneMatch.GetBoneType_ByT(target.transform.parent);
                if (type1 != JAnimationData.BoneType.unknowType)
                {
                    int index1 = ch_master.GetIndexOfSelectBone(type1);
                    if (index1 == -1)
                    {
                        Renderer render1 = target.GetComponent<Renderer>();
                        Color color1 = render1.material.color;
                        Debug.Log("redner:  " + target.name);
                        render1.sharedMaterial.color = Color.green;
                        ch_master.selectBones.Add(new CharacterSelectBone(type1, target.transform.parent, color1, render1));
                    }
                    else
                    {
                        ch_master.selectBones[index1].BoneRender.material.color =
                            ch_master.selectBones[index1].SrcColor;
                        ch_master.selectBones.RemoveAt(index1);
                    }
                    RefreshCurveShowWindow();
                }
            }
            if (ch_student.bm_p1 != null)
            {
                JAnimationData.BoneType type1
                   = ch_student.bm_p1.boneMatch.GetBoneType_ByT(target.transform.parent);
                if (type1 != JAnimationData.BoneType.unknowType)
                {
                    int index1 = ch_student.GetIndexOfSelectBone(type1);
                    if (index1 == -1)
                    {
                        Renderer render1 = target.GetComponent<Renderer>();
                        Color color1 = render1.material.color;
                        ch_student.selectBones.Add(new CharacterSelectBone(type1, target.transform.parent, color1, render1));
                        render1.material.color = Color.green;
                    }
                    else
                    {
                        ch_student.selectBones[index1].BoneRender.material.color =
                            ch_student.selectBones[index1].SrcColor;
                        ch_student.selectBones.RemoveAt(index1);
                    }
                    RefreshCurveShowWindow();
                }
            }

            if (ch_student.selectBones.Count + ch_master.selectBones.Count > 0)
            {
                RightStackController rightStackController1 = GameObject.FindObjectOfType<RightStackController>();
                BoneInfoWindowControl[] boneInfoWindows = FindObjectsOfType<BoneInfoWindowControl>();
                if (boneInfoWindows.Length == 0)
                {
                    CreateBoneInfoWindow();//当选中一个骨骼后，创建这个骨骼的信息窗口
                    HideCurveShowWindow();
                }

            }
            else
            {
                DestroyBoneInfoWindow();
                HideCurveShowWindow();
            }

        }
    }

    /// <summary>
    /// 销毁已经创建出的骨骼信息窗口
    /// </summary>
    public void DestroyBoneInfoWindow()
    {
        RightStackController rightStackController1 = GameObject.FindObjectOfType<RightStackController>();
        BoneInfoWindowControl[] boneInfoWindows = FindObjectsOfType<BoneInfoWindowControl>();
        bool hasCreatedInfoWindow = false;
        if (boneInfoWindows.Length > 0)
        {
            hasCreatedInfoWindow = true;
        }
        if (hasCreatedInfoWindow == true)
        {
            for (int i1 = boneInfoWindows.Length - 1; i1 >= 0; i1--)
            {
                if (boneInfoWindows[i1].CanDestroy)
                {
                    rightStackController1.OutStack(boneInfoWindows[i1].GetComponent<RectTransform>());
                }
            }
        }
    }
    /// <summary>
    /// 当选中一个骨骼后，创建这个骨骼的信息窗口
    /// </summary>
    private void CreateBoneInfoWindow()
    {
        RightStackController rightStackController1 = GameObject.FindObjectOfType<RightStackController>();
        DestroyBoneInfoWindow();// 销毁已经创建出的骨骼信息窗口
        GameObject ga1 = Instantiate<GameObject>(BoneInfoWindowPrefab);
        rightStackController1.GetInStack(ga1.GetComponent<RectTransform>());
        ga1.GetComponent<RightStackMember>().Show();

    }




    private void FixedUpdate()
    {

    }

    /// <summary>
    /// 如果两个时间值靠得很近，返回true
    /// </summary> 
    private bool isTimeClose(float time1, float time2, float _jiange)
    {
        if (Mathf.Abs(time1 - time2) <= _jiange)
        {
            return true;
        }
        return false;
    }

    private void OnGUI()
    {

    }

    Transform GetTFromBMP(BoneMatch_Player bmp1, JAnimationData.BoneType boneType)
    {
        for (int i = 0; i < bmp1.boneMatch.boneMatchList.Count; i++)
        {
            if (bmp1.boneMatch.boneMatchList[i].matchedT != null)
            {
                if (bmp1.boneMatch.boneMatchList[i].boneType == boneType)
                {
                    return bmp1.boneMatch.boneMatchList[i].matchedT;
                }
            }
        }
        return null;
    }

    float getSimAngle(BoneMatch_Player bmp1)
    {
        Transform ltopArm = GetTFromBMP(bmp1, JAnimationData.BoneType.leftArm);
        Transform lforeArm = GetTFromBMP(bmp1, JAnimationData.BoneType.leftForeArm);
        Transform lhand = GetTFromBMP(bmp1, JAnimationData.BoneType.leftHand);

        Vector3 v3_ltopArm = ltopArm.position - lforeArm.position;
        v3_ltopArm = v3_ltopArm.normalized;
        Vector3 v3_lforeArm = lforeArm.position - lhand.position;
        v3_lforeArm = v3_lforeArm.normalized;
        Vector3 ve_p1 = Vector3.Cross(v3_ltopArm, v3_lforeArm);


        Transform hip1 = GetTFromBMP(bmp1, JAnimationData.BoneType.hips);
        Transform spine = GetTFromBMP(bmp1, JAnimationData.BoneType.spine);
        Vector3 v3_hip = spine.position - hip1.position;
        v3_hip = v3_hip.normalized;

        float sim_p1 = Vector3.Dot(ve_p1, v3_hip) / (ve_p1.magnitude * v3_hip.magnitude);
        return sim_p1;
    }

    private void drawRoa()
    {

        float sim_p1 = getSimAngle(ch_master.bm_p1);
        float sim_p1_2 = getSimAngle(ch_student.bm_p1);

        GUI.Label(new Rect(0, 0, 300, 20), (sim_p1 - sim_p1_2).ToString());


        for (int i = 0; i < tPairs.Count; i++)
        {
            float angle1 = Vector3.Angle(tPairs[i].t1.forward, tPairs[i].t2.forward);
            Vector3 p1 = Camera.main.WorldToScreenPoint(tPairs[i].t1.position);
            if (angle1 <= 10f)
            {
                GUI.color = Color.white;
            }
            else if (angle1 <= 30f)
            {
                GUI.color = Color.green;
            }
            else
            {
                GUI.color = Color.red;
            }
            GUI.Label(new Rect(p1.x, Screen.height - p1.y, 300, 20), angle1.ToString());
        }
    }

    public void OnChangeTimeMin1(string value1)
    {
        float v1 = float.Parse(value1);

        if (v1 >= 0f && v1 < ch_master.max)
        {
            ch_master.min = v1;
        }
        else
        {
            ui1.input_min.text = ch_master.min.ToString();
        }
        RefreshSliderButtons(true);//刷新进度条上滑块的显示
    }
    public void OnChangeTimeMin2(string value1)
    {
        float v1 = float.Parse(value1);

        if (v1 >= 0f && v1 < ch_student.max)
        {
            ch_student.min = v1;
        }
        else
        {
            ui1.input_min2.text = ch_student.min.ToString();
        }
        RefreshSliderButtons(false);//刷新进度条上滑块的显示
    }








    public void OnDragSlider(float value1)
    {
        ch_master.timer = sliderToTime1(value1);
        ch_master.sliderValue = value1;
    }
    public void OnDragSlider2(float value1)
    {
        ch_student.timer = sliderToTime2(value1);
        ch_student.sliderValue = value1;
    }
    /// <summary>
    /// 将滑块上得值变换到真实时间
    /// </summary> 
    public float sliderToTime1(float value1)
    {
        return value1 * (ch_master.max - ch_master.min) + ch_master.min;
    }
    /// <summary>
    /// 将滑块上得值变换到真实时间
    /// </summary> 
    public float sliderToTime2(float value1)
    {
        return value1 * (ch_student.max - ch_student.min) + ch_student.min;
    }
    /// <summary>
    /// 将真实时间变换为滑块的范围
    /// </summary> 
    public float time1ToSlider(float _time)
    {
        if (_time < ch_master.min)
        {
            return 0;
        }
        if (_time > ch_master.max)
        {
            return 1;
        }
        return (_time - ch_master.min) / (ch_master.max - ch_master.min);
    }
    /// <summary>
    /// 将真实时间变换为滑块的范围
    /// </summary> 
    public float time2ToSlider(float _time)
    {
        if (_time < ch_student.min)
        {
            return 0;
        }
        if (_time > ch_student.max)
        {
            return 1;
        }
        return (_time - ch_student.min) / (ch_student.max - ch_student.min);
    }
    public void OnSelectDropDownSelect(int value1)
    {
        if (value1 == 0)
        {
            ui1.slider1.value = ch_master.sliderValue;
        }
        else
        {
            ui1.slider2.value = ch_student.sliderValue;
        }
        refreshUIs();
    }

    /// <summary>
    /// 刷新进度条上滑块的显示
    /// </summary>
    public void RefreshSliderButtons(bool _isMaster)
    {
        RightStackController rightStackController1 = GameObject.FindObjectOfType<RightStackController>();
        AnimInfoWindowControl[] animWindows = FindObjectsOfType<AnimInfoWindowControl>();
        bool hasCreatedInfoWindow = false;
        for (int i = 0; i < animWindows.Length; i++)
        {
            if (animWindows[i].isMaster == _isMaster)
            {
                hasCreatedInfoWindow = true;
                break;
            }
        }
        if (hasCreatedInfoWindow == true)
        {
            for (int i = animWindows.Length - 1; i >= 0; i--)
            {
                if (animWindows[i].isMaster == _isMaster)
                {
                    if (_isMaster)
                    {
                        animWindows[i].SetTime(ch_master.min, ch_master.max - ch_master.min, ch_master.time);
                    }
                    else
                    {
                        animWindows[i].SetTime(ch_student.min, ch_student.max - ch_student.min, ch_student.time);
                    }
                    animWindows[i].RefreshRangesUIs();
                }
            }
        }
    }
    public void OnChangeTimeMax1(string value1)
    {
        float v1 = float.Parse(value1);

        if (v1 > ch_master.min && v1 <= ch_master.time)
        {
            ch_master.max = v1;
        }
        else
        {
            ui1.input_max.text = ch_master.max.ToString();
        }
        RefreshSliderButtons(true);//刷新进度条上滑块的显示
    }
    public void OnChangeTimeMax2(string value1)
    {
        float v1 = float.Parse(value1);

        if (v1 > ch_student.min && v1 <= ch_student.time)
        {
            ch_student.max = v1;
        }
        else
        {
            ui1.input_max2.text = ch_student.max.ToString();
        }
        RefreshSliderButtons(false);//刷新进度条上滑块的显示
    }





    public void refreshUIs()
    {


        ui1.input_max.text = ch_master.max.ToString();
        ui1.input_min.text = ch_master.min.ToString();
        ui1.input_max2.text = ch_student.max.ToString();
        ui1.input_min2.text = ch_student.min.ToString();



        tPairs.Clear();
        if (ch_master.bm_p1 != null && ch_student.bm_p1 != null)
        {
            for (int i = 0; i < ch_master.bm_p1.boneMatch.boneMatchList.Count; i++)
            {
                if (ch_master.bm_p1.boneMatch.boneMatchList[i].matchedT != null)
                {
                    Transform t2 = null;
                    for (int j = 0; j < ch_student.bm_p1.boneMatch.boneMatchList.Count; j++)
                    {
                        if (ch_student.bm_p1.boneMatch.boneMatchList[j].matchedT != null &&
                            ch_student.bm_p1.boneMatch.boneMatchList[j].boneType == ch_master.bm_p1.boneMatch.boneMatchList[i].boneType)
                        {
                            t2 = ch_student.bm_p1.boneMatch.boneMatchList[j].matchedT;
                            break;
                        }
                    }
                    if (t2 != null)
                    {
                        tPairs.Add(new transformPair(ch_master.bm_p1.boneMatch.boneMatchList[i].matchedT, t2));
                    }
                }
            }
        }

    }

    public void InitUIs()
    {

        for (int i = 0; i < animShowList.Count; i++)//检查内置学生动作数据的autodata有没有设置
        {
            if (animShowList[i].autodata == null)
            {
                Debug.LogError("animShowList[" + i + "] don't have autodata !!!");
            }
        }

        RefreshSelectMotionsList();

        refreshUIs();
    }

    /// <summary>
    /// 刷新一次选择动作的列表，包括大师的动作数据列表和学生的动作数据列表(内置和实时录制)
    /// </summary>
    public void RefreshSelectMotionsList()
    {
        scrollView_master.ClearItems();
        scrollView_student.ClearItems();
        for (int i = 0; i < animShowList.Count; i++)
        {
            scrollView_master.AddOneItem(string.IsNullOrEmpty(animShowList[i].name) ? animShowList[i].jAnimData.name : animShowList[i].name);
        }
        for (int i = 0; i < animShowList.Count; i++)
        {
            scrollView_student.AddOneItem(string.IsNullOrEmpty(animShowList[i].name) ? animShowList[i].jAnimData.name : animShowList[i].name);
        }

        animRecodesShowList = new List<JAnimRecodesShow>();
        string recodesPath = Application.persistentDataPath + "/Recodes";
        if (Directory.Exists(recodesPath))
        {
            DirectoryInfo direction = new DirectoryInfo(recodesPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".JADF"))
                {
                    //Debug.Log("Name:" + files[i].Name);  //打印出来
                    //Debug.Log("Full:" + files[i].FullName);  //打印出来
                    scrollView_student.AddOneItem(files[i].Name);
                    animRecodesShowList.Add(new JAnimRecodesShow(files[i].Name, files[i].FullName));

                }
            }

        }

        scrollView_master.myDelegate = ScrollView_master_selectCreate;
        scrollView_student.myDelegate = ScrollView_student_selectCreate;


        //dropdown-----
        Dd_Select_Master.options.Clear();
        Dd_Select_Student.options.Clear();
        Dd_Select_Master.options.Add(new Dropdown.OptionData("无"));
        Dd_Select_Student.options.Add(new Dropdown.OptionData("无"));
        for (int i = 0; i < animShowList.Count; i++)
        {
            Dd_Select_Master.options.Add(new Dropdown.OptionData(string.IsNullOrEmpty(animShowList[i].name) ? animShowList[i].jAnimData.name : animShowList[i].name));
            Dd_Select_Student.options.Add(new Dropdown.OptionData(string.IsNullOrEmpty(animShowList[i].name) ? animShowList[i].jAnimData.name : animShowList[i].name));
        }
        if (Directory.Exists(recodesPath))
        {
            DirectoryInfo direction = new DirectoryInfo(recodesPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".JADF"))
                {
                    Dd_Select_Student.options.Add(new Dropdown.OptionData(files[i].Name));
                }
            }

        }
    }

    /// <summary>
    /// 销毁现存的模型
    /// </summary>
    public void DestroyCreatedMaster(bool setDropdownTo0 = false)
    {
        if (ch_master.builder != null) ch_master.builder.Destroy();
        if (ch_master.createdGA != null) Destroy(ch_master.createdGA);
        ch_master.createdGA_No = -1;
        ch_master.hasInit = false;
        ch_master.jad_created = null;

        for (int i = 0; i < ch_master.added_createdGA.Count; i++)
        {
            Destroy(ch_master.added_createdGA[i]);
        }
        ch_master.added_createdGA.Clear();
        ch_master.added_bm_p.Clear();
        ch_master.selectBones.Clear();

        if (setDropdownTo0) Dd_Select_Master.value = 0;

        RefreshCurveShowWindow();
        PlayVideoButton.interactable = false;
        vControl.Hide();
    }
    /// <summary>
    /// 销毁现存的模型
    /// </summary>
    public void DestroyCreatedStudent(bool setDropdownTo0 = false)
    {
        if (ch_student.builder != null) ch_student.builder.Destroy();
        if (ch_student.createdGA != null) Destroy(ch_student.createdGA);
        loadedJARanges = new JAnimDataToFile.JAnimRanges();
        ch_student.createdGA_No = -1;
        ch_student.hasInit = false;
        ch_student.jad_created = null;

        for (int i = 0; i < ch_student.added_createdGA.Count; i++)
        {
            Destroy(ch_student.added_createdGA[i]);
        }
        ch_student.added_createdGA.Clear();
        ch_student.added_bm_p.Clear();
        ch_student.selectBones.Clear();

        if (setDropdownTo0) Dd_Select_Student.value = 0;

        RefreshCurveShowWindow();
    }

    public void dropdown_selectMaster_OnChange(int _num)
    {
        if (_num > 0)
        {
            ScrollView_master_selectCreate(_num - 1);
        }
        else
        {
            DestroyCreatedMaster();
            RightStackController rightStackController1 = GameObject.FindObjectOfType<RightStackController>();
            AnimInfoWindowControl[] animWindows = FindObjectsOfType<AnimInfoWindowControl>();
            bool hasCreatedInfoWindow = false;
            for (int i = 0; i < animWindows.Length; i++)
            {
                if (animWindows[i].isMaster)
                {
                    hasCreatedInfoWindow = true;
                    break;
                }
            }
            if (hasCreatedInfoWindow == true)
            {
                for (int i = animWindows.Length - 1; i >= 0; i--)
                {
                    if (animWindows[i].isMaster)
                    {
                        if (animWindows[i].CanDestroy)
                        {
                            rightStackController1.OutStack(animWindows[i].GetComponent<RectTransform>());
                        }
                    }
                }
            }
        }
    }
    public void dropdown_selectStudent_OnChange(int _num)
    {
        if (_num > 0)
        {
            ScrollView_student_selectCreate(_num - 1);
        }
        else
        {
            DestroyCreatedStudent();
            RightStackController rightStackController1 = GameObject.FindObjectOfType<RightStackController>();
            AnimInfoWindowControl[] animWindows = FindObjectsOfType<AnimInfoWindowControl>();
            bool hasCreatedInfoWindow = false;
            for (int i = 0; i < animWindows.Length; i++)
            {
                if (animWindows[i].isMaster == false)
                {
                    hasCreatedInfoWindow = true;
                    break;
                }
            }
            if (hasCreatedInfoWindow == true)
            {
                for (int i = animWindows.Length - 1; i >= 0; i--)
                {
                    if (animWindows[i].isMaster == false)
                    {
                        if (animWindows[i].CanDestroy)
                        {
                            rightStackController1.OutStack(animWindows[i].GetComponent<RectTransform>());
                        }
                    }
                }
            }
        }
    }
    private void SetALLMaterial(Transform t1, string maNameInResource)
    {
        Material m1 = Resources.Load<Material>(maNameInResource);
        if (m1 == null) return;
        Renderer[] renders = t1.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].material = m1;
        }
    }
    public void Button_ShowVideo()
    {
        if (ch_master.hasInit == true)
        {
            if (vControl.isShowing == false)
            {
                if (animShowList[ch_master.createdGA_No].videoClip != null)
                {
                    vControl.PreOne(animShowList[ch_master.createdGA_No].videoClip);
                    vControl.player.time = ch_master.timer + animShowList[ch_master.createdGA_No].videoTimeOffset;
                }
            }
            else
            {
                vControl.Hide();
            }
        }
    }




    public void ScrollView_master_selectCreate(int num)
    {
        Debug.Log(num);
        DestroyCreatedMaster();//销毁现存
        if (num >= animShowList.Count)
        {
            Debug.LogError("Error !!!  animShowList.Count < num !!!");
            return;
        }

        //创建信息窗口到右侧堆栈
        RightStackController rightStackController1 = GameObject.FindObjectOfType<RightStackController>();
        AnimInfoWindowControl[] animWindows = FindObjectsOfType<AnimInfoWindowControl>();
        bool hasCreatedInfoWindow = false;
        for (int i = 0; i < animWindows.Length; i++)
        {
            if (animWindows[i].isMaster)
            {
                hasCreatedInfoWindow = true;
                break;
            }
        }
        if (hasCreatedInfoWindow == true)
        {
            for (int i = animWindows.Length - 1; i >= 0; i--)
            {
                if (animWindows[i].isMaster)
                {
                    if (animWindows[i].CanDestroy)
                    {
                        rightStackController1.OutStack(animWindows[i].GetComponent<RectTransform>());
                    }
                }
            }
        }
        ch_master.selectBones.Clear();
        GameObject ga1 = Instantiate<GameObject>(AnimInfoWindowPrefab);
        rightStackController1.GetInStack(ga1.GetComponent<RectTransform>());
        ga1.GetComponent<RightStackMember>().Show();
        ga1.GetComponent<AnimInfoWindowControl>().SetBasicInfo(animShowList[num].name, animShowList[num].introduction, true, false, true, ui1.slider1);
        ch_master.min = 0f;
        ch_master.max = animShowList[num].jAnimData.jAD.TimeLength;
        //加载保存了的选段数据
        string path = JAnimDataToFile.JADSavePath;
        JAnimDataToFile.JAnimRanges janimRanges = JAnimDataToFile.LoadJAnimRanges(path, animShowList[num].name);//加载动作选段
        if (janimRanges != null)
        {
            animShowList[num].selectRanges = janimRanges.selectRanges;
        }
        ga1.GetComponent<AnimInfoWindowControl>().SetTime(ch_master.min, ch_master.max - ch_master.min, animShowList[num].jAnimData.jAD.TimeLength);
        ga1.GetComponent<AnimInfoWindowControl>().RefreshRangesUIs();


        if (animShowList[num].frame)
        {
            ch_master.builder = new VehicleBuilder(animShowList[num].frame.boneFramework);
            ch_master.builder.Build(1);
            ch_master.createdGA = ch_master.builder.buildedGA;
            ch_master.createdGA.transform.position = animShowList[num].pos;
            ch_master.createdGA.transform.rotation = Quaternion.Euler(animShowList[num].roa);
            SetAllChildLayer(ch_master.createdGA, LayerMask.NameToLayer("cam1"));//set layer
            ch_master.jad_created = animShowList[num].jAnimData.jAD;
            Init(ch_master.builder.buildedGA, animShowList[num].autodata, animShowList[num].jAnimData.jAD, true);
            /*
            if(animShowList[num].frame_model != null && animShowList[num].autodata_model != null)
            {
                GameObject addedga1 = Instantiate<GameObject>(animShowList[num].frame_model);
                ch_master.added_createdGA.Add(addedga1);
                addedga1.transform.position = animShowList[num].pos_model;
                addedga1.transform.rotation = Quaternion.Euler(animShowList[num].roa_model);
                SetAllChildLayer(addedga1, LayerMask.NameToLayer("cam1"));//set layer 

                BoneMatch bm1 = new BoneMatch();
                bm1.StartAutoMatch(addedga1.transform, animShowList[num].autodata_model, animShowList[num].jAnimData.jAD);
                BoneMatch_Player p1 = new BoneMatch_Player(bm1);
                ch_master.added_bm_p.Add(p1);
                p1.SetJAnimationData(animShowList[num].jAnimData.jAD);
            }*/
        }
        else
        {
            Debug.LogError("ERROR!!!");
        }


        ch_master.createdGA_No = num;

        if (animShowList[num].videoClip != null)
            PlayVideoButton.interactable = true;

        refreshUIs();
        posControl.Refresh();
    }
    public void ScrollView_student_selectCreate(int num)
    {
        Debug.Log(num);
        DestroyCreatedStudent();//销毁现存
        if (num >= animShowList.Count + animRecodesShowList.Count)
        {
            Debug.LogError("Error !!!  animShowList.Count + animRecodesShowList.Count < num !!!");
            return;
        }

        //创建信息窗口到右侧堆栈
        RightStackController rightStackController1 = GameObject.FindObjectOfType<RightStackController>();
        AnimInfoWindowControl[] animWindows = FindObjectsOfType<AnimInfoWindowControl>();
        bool hasCreatedInfoWindow = false;
        for (int i = 0; i < animWindows.Length; i++)
        {
            if (animWindows[i].isMaster == false)
            {
                hasCreatedInfoWindow = true;
                break;
            }
        }
        if (hasCreatedInfoWindow == true)
        {
            for (int i = animWindows.Length - 1; i >= 0; i--)
            {
                if (animWindows[i].isMaster == false)
                {
                    if (animWindows[i].CanDestroy)
                    {
                        rightStackController1.OutStack(animWindows[i].GetComponent<RectTransform>());
                    }
                }
            }
        }
        ch_student.selectBones.Clear();
        GameObject ga1 = Instantiate<GameObject>(AnimInfoWindowPrefab);
        rightStackController1.GetInStack(ga1.GetComponent<RectTransform>());


        if (num < animShowList.Count)//内置数据
        {
            if (animShowList[num].frame)
            {
                ch_student.builder = new VehicleBuilder(animShowList[num].frame.boneFramework);
                ch_student.builder.Build(1);
                ch_student.createdGA = ch_student.builder.buildedGA;
                SetALLMaterial(ch_student.createdGA.transform, "baseMaterials/baseMaterial2");
                ch_student.createdGA.name = "内置动作数据animShowList[" + num + "]";
                SetAllChildLayer(ch_student.createdGA, LayerMask.NameToLayer("cam2"));//set layer
                ch_student.createdGA.transform.position = animShowList[num].pos;
                ch_student.createdGA.transform.rotation = Quaternion.Euler(animShowList[num].roa);
                ch_student.jad_created = animShowList[num].jAnimData.jAD;
                Init(ch_student.builder.buildedGA, animShowList[num].autodata, animShowList[num].jAnimData.jAD, false);
                /*
                if (animShowList[num].frame_model != null && animShowList[num].autodata_model != null)
                {
                    GameObject addedga1 = Instantiate<GameObject>(animShowList[num].frame_model);
                    ch_student.added_createdGA.Add(addedga1);
                    addedga1.transform.position = animShowList[num].pos_model;
                    addedga1.transform.rotation = Quaternion.Euler(animShowList[num].roa_model);
                    SetAllChildLayer(addedga1, LayerMask.NameToLayer("cam2"));//set layer 

                    BoneMatch bm1 = new BoneMatch();
                    bm1.StartAutoMatch(addedga1.transform, animShowList[num].autodata_model, animShowList[num].jAnimData.jAD);
                    BoneMatch_Player p1 = new BoneMatch_Player(bm1);
                    ch_student.added_bm_p.Add(p1);
                    p1.SetJAnimationData(animShowList[num].jAnimData.jAD);
                }*/
            }
            else
            {
                Debug.LogError("ERROR!!!");
            }
            ch_student.min = 0f;
            ch_student.max = animShowList[num].jAnimData.jAD.TimeLength;
            //加载保存了的选段数据
            string path = JAnimDataToFile.JADSavePath;
            JAnimDataToFile.JAnimRanges janimRanges = JAnimDataToFile.LoadJAnimRanges(path, animShowList[num].name);//加载动作选段
            if (janimRanges != null)
            {
                animShowList[num].selectRanges = janimRanges.selectRanges;
            }
            //显示界面
            ga1.GetComponent<RightStackMember>().Show();
            ga1.GetComponent<AnimInfoWindowControl>().SetBasicInfo(animShowList[num].name, animShowList[num].introduction, false, false, true, ui1.slider2);
            ga1.GetComponent<AnimInfoWindowControl>().SetTime(ch_student.min, ch_student.max - ch_student.min, animShowList[num].jAnimData.jAD.TimeLength);
            ga1.GetComponent<AnimInfoWindowControl>().RefreshRangesUIs();


        }
        else//软件实时录制数据
        {
            int newnum = num - animShowList.Count;
            string fileFullName = animRecodesShowList[newnum].fileFullName;
            Debug.Log("Load recoded JAnimData [" + animRecodesShowList[newnum].fileName + "]");
            string path = fileFullName.Substring(0, fileFullName.Length - animRecodesShowList[newnum].fileName.Length - 1);
            path = path.Replace("\\", "/");
            string theName = animRecodesShowList[newnum].fileName.Substring(0, animRecodesShowList[newnum].fileName.Length - 5);
            //Debug.Log("path : " + path);
            //Debug.Log("theName : " + theName);
            JAnimDataToFile.JAnimDataSaved recodedData = JAnimDataToFile.LoadJAD(path, theName);
            loadedJARanges = JAnimDataToFile.LoadJAnimRanges(path, theName);//加载动作选段
            if (loadedJARanges == null)
            {
                loadedJARanges = new JAnimDataToFile.JAnimRanges();
            }
            if (string.IsNullOrEmpty(recodedData.modelPrefabName))//无预制体名称，只有骨骼数据
            {
                ch_student.builder = new VehicleBuilder(recodedData.boneFramework);
                ch_student.builder.Build(1);
                ch_student.createdGA = ch_student.builder.buildedGA;
                SetALLMaterial(ch_student.createdGA.transform, "baseMaterials/baseMaterial2");
                ch_student.createdGA.name = "录制动作数据animRecodesShowList[" + newnum + "]";
                SetAllChildLayer(ch_student.createdGA, LayerMask.NameToLayer("cam2"));//set layer
                ch_student.createdGA.transform.position = Vector3.zero;
                ch_student.createdGA.transform.rotation = Quaternion.Euler(Vector3.zero);
                ch_student.jad_created = recodedData.jad;
                Init(ch_student.builder.buildedGA, animShowList[0].autodata, recodedData.jad, false);
            }
            else//使用了预制体的录制数据
            {
                ch_student.builder = new VehicleBuilder(recodedData.boneFramework);
                ch_student.builder.Build(1);
                ch_student.createdGA = ch_student.builder.buildedGA;
                SetALLMaterial(ch_student.createdGA.transform, "baseMaterials/baseMaterial2");
                ch_student.createdGA.name = "录制动作数据animRecodesShowList[" + newnum + "]";
                SetAllChildLayer(ch_student.createdGA, LayerMask.NameToLayer("cam2"));//set layer
                ch_student.createdGA.transform.position = Vector3.zero;
                ch_student.createdGA.transform.rotation = Quaternion.Euler(Vector3.zero);
                ch_student.jad_created = recodedData.jad;
                Init(ch_student.builder.buildedGA, animShowList[0].autodata, recodedData.jad, false);
                /*
                bool found = false;
                for(int i=0; i<animPrefabsList.Count; i++)
                {
                    if(animPrefabsList[i].name == recodedData.modelPrefabName)
                    {
                        found = true;
                        ch_student.createdGA = Instantiate<GameObject>(animPrefabsList[i]);
                        ch_student.createdGA.name = animPrefabsList[i].name;
                        SetAllChildLayer(ch_student.createdGA, LayerMask.NameToLayer("cam2"));//set layer
                        ch_student.createdGA.transform.position = Vector3.zero;
                        ch_student.createdGA.transform.rotation = Quaternion.Euler(Vector3.zero);
                        ch_student.jad_created = recodedData.jad;
                        Init(ch_student.createdGA, animShowList[0].autodata, recodedData.jad, false);
                        for(int j1 = 0; j1< ch_student.bm_p1.boneMatch.boneMatchList.Count; j1 ++)//创建球形透明关节，用于点击
                        {
                            if (ch_student.bm_p1.boneMatch.boneMatchList[j1].matchedT == null) continue;
                            GameObject sphere1 = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("baseModels/Sphere"));
                            sphere1.transform.SetParent(ch_student.bm_p1.boneMatch.boneMatchList[j1].matchedT);
                            sphere1.transform.localPosition = Vector3.zero;
                            sphere1.transform.localScale = Vector3.one * 0.1f;
                            Material touming = Resources.Load<Material>("baseMaterials/touming");
                            sphere1.GetComponent<Renderer>().material = touming;
                            sphere1.name = "boneHitter";
                            sphere1.layer = LayerMask.NameToLayer("cam2");
                        }
                        break;
                    }
                }
                if(found == false)
                {
                    Debug.Log("录制数据所用的预制体[" + recodedData.modelPrefabName + "]未找到！");
                    return;
                }*/
            }
            ch_student.min = 0f;
            ch_student.max = recodedData.jad.TimeLength;

            //显示界面
            ga1.GetComponent<RightStackMember>().Show();
            ga1.GetComponent<AnimInfoWindowControl>().SetBasicInfo(animRecodesShowList[newnum].fileName, "暂无简介", false, true, true, ui1.slider2);
            ga1.GetComponent<AnimInfoWindowControl>().SetTime(ch_student.min, ch_student.max - ch_student.min, recodedData.jad.TimeLength);
            ga1.GetComponent<AnimInfoWindowControl>().RefreshRangesUIs();

        }

        ch_student.createdGA_No = num;

        refreshUIs();
        posControl.Refresh();
    }



    public void PlayButton(int _select)
    {
        if (_select == 0)
        {
            if (ch_master.timer >= ch_master.max)
            {
                ch_master.timer = ch_master.min;
                ch_master.isplaying = true;
            }
            else
            {
                ch_master.isplaying = !ch_master.isplaying;
            }

        }
        else
        {
            if (ch_student.timer >= ch_student.max)
            {
                ch_student.timer = ch_student.min;
                ch_student.isplaying = true;
            }
            else
            {
                ch_student.isplaying = !ch_student.isplaying;
            }
        }

    }


    public void SetAllChildLayer(GameObject _ga, int _layer)
    {
        _ga.layer = _layer;
        Transform[] gas = _ga.GetComponentsInChildren<Transform>();
        for (int i = 0; i < gas.Length; i++)
        {
            gas[i].gameObject.layer = _layer;
        }
    }

    public void Init(GameObject ga, JAnimation_AutoSetBoneMatchData autoSetBoneData, JAnimationData.JAD jAnimData, bool ismaster)
    {
        if (ismaster)
        {
            ch_master.time = jAnimData.TimeLength;

            BoneMatch bm1 = new BoneMatch();
            bm1.StartAutoMatch(ga.transform, autoSetBoneData, jAnimData);
            ch_master.bm_p1 = new BoneMatch_Player(bm1);
            ch_master.bm_p1.SetJAnimationData(jAnimData);
            ui1.input_min.text = "0";
            ui1.input_max.text = jAnimData.TimeLength.ToString();
            ch_master.min = 0f;
            ch_master.timer = 0f;
            ch_master.max = jAnimData.TimeLength;
            Debug.Log("load anim[" + ga.name + "]   ");
            ch_master.hasInit = true;
        }
        else
        {
            ch_student.time = jAnimData.TimeLength;

            BoneMatch bm1 = new BoneMatch();
            bm1.StartAutoMatch(ga.transform, autoSetBoneData, jAnimData);
            ch_student.bm_p1 = new BoneMatch_Player(bm1);
            ch_student.bm_p1.SetJAnimationData(jAnimData);
            ui1.input_min2.text = "0";
            ui1.input_max2.text = jAnimData.TimeLength.ToString();
            ch_student.min = 0f;
            ch_student.timer = 0f;
            ch_student.max = jAnimData.TimeLength;
            Debug.Log("load anim2[" + ga.name + "]   ");
            ch_student.hasInit = true;
        }

    }


}
