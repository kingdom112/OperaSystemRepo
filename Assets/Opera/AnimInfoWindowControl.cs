using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JAnimationSystem;

public class AnimInfoWindowControl : RightStackMember
{
    public bool isMaster = false;
    public bool sourceIsFile = false;
    public bool canEdit = false;
    public Text nameText;
    public Text introductionText;
    public Dropdown dd_selectModel;  //蒙皮模型选择下拉框
    public GameObject rangesMemberPrefab;
    public float rangesMemberPosStart = 0f;
    public RectTransform rangesMemberParent;
    public Slider showAnotherBoneSlider;
    public Button button_showAnotherBone;
    public Button button_showModel;
    public Sprite button_showModel_Normal;
    public Sprite button_showModel_HighLight;
    public Button button_showBone;
    public Sprite button_showBone_Normal;
    public Sprite button_showBone_HighLight;
    public Button button_ShowHipAngle;
    public Sprite button_ShowHipAngle_Normal;
    public Sprite button_ShowHipAngle_HighLight; 
    public AnimInfoWindow_ShouldHipAngle control_ShowHipAngle;
    private bool isShowingAnotherBone = false;

    private float showAnotherBoneDistance = 3f;

    public float _TimeLength
    {
        get
        {
            return TimeLength;
        }
    }
    public float _Time_Start
    {
        get
        {
            return Time_start;
        }
    }
    public float _TimeTotalLength
    {
        get
        {
            return TimeTotalLength;
        }
    }
    private float TimeLength = 0f;
    private float Time_start = 0f;
    private float TimeTotalLength = 0f;

    [HideInInspector]
    public List<AnimInfoWindow_rangesMember> createdRangesMembers = new List<AnimInfoWindow_rangesMember>();
    private List<GameObject> createdPanels = new List<GameObject>();


    [System.Serializable]
    public class SliderButtonPearData
    { 
        public float time1 = 0; 
        public float time2 = 0;
    } 
    /// <summary>
    /// button键对
    /// </summary>
    public class CreatedButtonPear
    {
        public SliderButton sliderButton1;
        public SliderButton sliderButton2;

        public CreatedButtonPear(SliderButton _button1, SliderButton _button2)
        {
            sliderButton1 = _button1;
            sliderButton2 = _button2;
        } 
    }
    public GameObject buttonPrefab;
    public GameObject panelPrefab;
    private RectTransform buttonParent;
    private Slider targetSlider;
    public List<SliderButtonPearData> buttons = new List<SliderButtonPearData>();
    [HideInInspector]
    public List<CreatedButtonPear> createdButtons = new List<CreatedButtonPear>();
    [HideInInspector]
    public VFPlayer_InGame vfPlayer;

    private void Awake()
    {
        vfPlayer = FindObjectOfType<VFPlayer_InGame>();

        InitSelectModelOptions();
    }
    void Start()
    {
        button_showBone.image.sprite = button_showBone_HighLight;
        button_showModel.image.sprite = button_showModel_Normal;
    }

    void Update()
    {
        if(isShowingAnotherBone && isMaster)
        {
            if(bm_p1_follow != null && followBone != null && followTarget != null)
            {
                bm_p1_follow.PlayDataInThisFrame_Curve(vfPlayer.ch_student.timer, false);
                followBone.position = followTarget.position + Vector3.right * showAnotherBoneDistance * showAnotherBoneSlider.value;
            }
            else
            {
                Debug.Log(bm_p1_follow == null ? "true" : "");
                Debug.Log(followBone == null ? "true" : "");
                Debug.Log(followTarget == null ? "true" : "");
            }
        }
    }

    private void LateUpdate()
    { 
    }



    /// <summary>
    /// 初始化选择模型的下拉菜单
    /// </summary>
    private void InitSelectModelOptions()
    {
        dd_selectModel.options.Clear();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        options.Add(new Dropdown.OptionData("无"));
        for (int i = 0; i < vfPlayer.animModelList.Count; i++)
        {
            options.Add(new Dropdown.OptionData(vfPlayer.animModelList[i].name));
        }
        dd_selectModel.AddOptions(options);
    }


    public void OnButton_ShowHipAngles()
    {
        control_ShowHipAngle.ShowOrHide();
        if(control_ShowHipAngle.IsShowing())
        {
            button_ShowHipAngle.image.sprite = button_ShowHipAngle_HighLight;
        }
        else
        {
            button_ShowHipAngle.image.sprite = button_ShowHipAngle_Normal ;
        }
    }
    public void OnButton_AllOpen()
    {
        for(int i=0; i< createdRangesMembers.Count; i++)
        {
            createdRangesMembers[i].ChangeToMaxSize();
            RangeMemberTryChangeMaxSizeBool(createdRangesMembers[i], true, false);
        }
        RefreshRangesUIs();
        ResetRangesMembersPos();
    }
    public void OnButton_AllClose()
    {
        for (int i = 0; i < createdRangesMembers.Count; i++)
        {
            createdRangesMembers[i].ChangeToMinSize();
            RangeMemberTryChangeMaxSizeBool(createdRangesMembers[i], false, false);
        }
        RefreshRangesUIs();
        ResetRangesMembersPos();
    }

    public void OnButton_ShowModel ()
    {
        if (isMaster)
        {
            if (vfPlayer.ch_master.added_createdGA.Count > 0)
            {
                bool t1 = !vfPlayer.ch_master.added_createdGA[0].activeSelf;
                for (int i = 0; i < vfPlayer.ch_master.added_createdGA.Count; i++)
                {
                    vfPlayer.ch_master.added_createdGA[i].SetActive(t1);
                    if (t1)
                    {
                        button_showModel.image.sprite = button_showModel_HighLight;
                    }
                    else
                    {
                        button_showModel.image.sprite = button_showModel_Normal;
                    }
                }
            }
        }
        else
        {
            if (vfPlayer.ch_student.added_createdGA.Count > 0)
            {
                bool t1 = !vfPlayer.ch_student.added_createdGA[0].activeSelf;
                for (int i = 0; i < vfPlayer.ch_student.added_createdGA.Count; i++)
                {
                    vfPlayer.ch_student.added_createdGA[i].SetActive(t1);
                    if (t1)
                    {
                        button_showModel.GetComponent<Image>().color = Color.green;
                    }
                    else
                    {
                        button_showModel.GetComponent<Image>().color = Color.white;
                    }
                }
            }
        } 
    }
    public void OnButton_ShowBones()
    {
        if (isMaster)
        {
            vfPlayer.ch_master.createdGA.SetActive(!vfPlayer.ch_master.createdGA.activeSelf);
            if(vfPlayer.ch_master.createdGA.activeSelf)
            {
                button_showBone.image.sprite = button_showBone_HighLight;
            }
            else
            {
                button_showBone.image.sprite = button_showBone_Normal;
            }
        }
        else
        {
            vfPlayer.ch_student.createdGA.SetActive(!vfPlayer.ch_student.createdGA.activeSelf);
            if (vfPlayer.ch_student.createdGA.activeSelf)
            {
                button_showBone.image.sprite = button_showBone_HighLight;
            }
            else
            {
                button_showBone.image.sprite = button_showBone_Normal;
            }
        }
    }

    /// <summary>
    /// 变成展开的大小
    /// </summary>
    public override void TurnToMaxSize()
    {
        panel_max.SetActive(true);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ysize_max + RangesMembersSize);
        stackController.ResetStackYSize();
        stackController.ResetStackItemsPos();
    }

    private float RangesMembersSize
    {
        get
        {
            float sizeY = 10f;
            for (int i = 0; i < createdRangesMembers.Count; i++)
            {
                RectTransform rectt1 = createdRangesMembers[i].gameObject.GetComponent<RectTransform>();
                sizeY += rectt1.rect.height;
            }
            return sizeY;
        }
    }

    private void FixedUpdate()
    { 
        for (int i = 0; i < buttons.Count; i++)
        { 
            if (createdButtons[i].sliderButton1 != null)
                createdButtons[i].sliderButton1.sliderWidth = targetSlider.gameObject.GetComponent<RectTransform>().rect.width;
            if(createdButtons[i].sliderButton2 != null)
                createdButtons[i].sliderButton2.sliderWidth = targetSlider.gameObject.GetComponent<RectTransform>().rect.width;


            int num1 = findInVFPlayerAnims();
            if (num1 != -1)
            {
                if (vfPlayer.animShowList[num1].selectRanges[i].type == AnimRangeControl.SelectType.timePoint)
                {
                    vfPlayer.animShowList[num1].selectRanges[i].timePoint = buttons[i].time1;
                    createdRangesMembers[i].point_slider.value = (buttons[i].time1) / TimeTotalLength;
                    createdRangesMembers[i].point_inputfield.text = (buttons[i].time1).ToString();
                }
                else if (vfPlayer.animShowList[num1].selectRanges[i].type == AnimRangeControl.SelectType.timeRange)
                {
                    vfPlayer.animShowList[num1].selectRanges[i].timeRange_start = buttons[i].time1;
                    vfPlayer.animShowList[num1].selectRanges[i].timeRange_end = buttons[i].time2;
                    createdRangesMembers[i].range_left_slider.value = (buttons[i].time1) / TimeTotalLength;
                    createdRangesMembers[i].range_right_slider.value = (buttons[i].time2) / TimeTotalLength;
                    createdRangesMembers[i].range_left_inputfield.text = (buttons[i].time1).ToString();
                    createdRangesMembers[i].range_right_inputfield.text = (buttons[i].time2).ToString();
                }
            }
            else
            {
                if (sourceIsFile == true)
                {
                    if(isMaster)
                    {
                        if (vfPlayer.loadedJARanges_master.selectRanges[i].type == AnimRangeControl.SelectType.timePoint)
                        {
                            vfPlayer.loadedJARanges_master.selectRanges[i].timePoint = buttons[i].time1;
                            createdRangesMembers[i].point_slider.value = (buttons[i].time1) / TimeTotalLength;
                            createdRangesMembers[i].point_inputfield.text = (buttons[i].time1).ToString();
                        }
                        else if (vfPlayer.loadedJARanges_master.selectRanges[i].type == AnimRangeControl.SelectType.timeRange)
                        {
                            vfPlayer.loadedJARanges_master.selectRanges[i].timeRange_start = buttons[i].time1;
                            vfPlayer.loadedJARanges_master.selectRanges[i].timeRange_end = buttons[i].time2;
                            createdRangesMembers[i].range_left_slider.value = (buttons[i].time1) / TimeTotalLength;
                            createdRangesMembers[i].range_right_slider.value = (buttons[i].time2) / TimeTotalLength;
                            createdRangesMembers[i].range_left_inputfield.text = (buttons[i].time1).ToString();
                            createdRangesMembers[i].range_right_inputfield.text = (buttons[i].time2).ToString();
                        }
                    }
                    else//student
                    {
                        if (vfPlayer.loadedJARanges_student.selectRanges[i].type == AnimRangeControl.SelectType.timePoint)
                        {
                            vfPlayer.loadedJARanges_student.selectRanges[i].timePoint = buttons[i].time1;
                            createdRangesMembers[i].point_slider.value = (buttons[i].time1) / TimeTotalLength;
                            createdRangesMembers[i].point_inputfield.text = (buttons[i].time1).ToString();
                        }
                        else if (vfPlayer.loadedJARanges_student.selectRanges[i].type == AnimRangeControl.SelectType.timeRange)
                        {
                            vfPlayer.loadedJARanges_student.selectRanges[i].timeRange_start = buttons[i].time1;
                            vfPlayer.loadedJARanges_student.selectRanges[i].timeRange_end = buttons[i].time2;
                            createdRangesMembers[i].range_left_slider.value = (buttons[i].time1) / TimeTotalLength;
                            createdRangesMembers[i].range_right_slider.value = (buttons[i].time2) / TimeTotalLength;
                            createdRangesMembers[i].range_left_inputfield.text = (buttons[i].time1).ToString();
                            createdRangesMembers[i].range_right_inputfield.text = (buttons[i].time2).ToString();
                        }
                    }
                  
                }
                else
                {
                    Debug.LogError("Error!!! 当前数据找不到！");
                } 
            }
        }
    }

    /// <summary>
    /// 重新绘制选段列表
    /// </summary>
    public void RefreshRangesUIs()
    {
        if(isMaster)
        {
            button_showAnotherBone.gameObject.SetActive(true);
            showAnotherBoneSlider.gameObject.SetActive(true);
        }
        else
        {
            button_showAnotherBone.gameObject.SetActive(false);
            showAnotherBoneSlider.gameObject.SetActive(false);
        }
        ClearCreatedRangesMembersAndButtons();

        if(isMaster)
        {
            if (sourceIsFile == false)
            {
                int num1 = findInVFPlayerAnims();
                if (num1 != -1)
                {
                    for (int i = 0; i < vfPlayer.animShowList[num1].selectRanges.Count; i++)
                    {
                        AddRangesMember(vfPlayer.animShowList[num1].selectRanges[i], i + 1);
                    }
                    ResetRangesMembersPos();//reset pos

                    for (int i = 0; i < vfPlayer.animShowList[num1].selectRanges.Count; i++)//重新设置层级
                    {
                        if (vfPlayer.animShowList[num1].selectRanges[i].type == AnimRangeControl.SelectType.timePoint)
                        {
                            createdButtons[i].sliderButton1.transform.SetParent(null);
                            createdButtons[i].sliderButton1.transform.SetParent(buttonParent);

                            createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localScale = Vector3.one;
                            Vector3 _lpos = createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localPosition;
                            _lpos.z = 0f;
                            createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localPosition = _lpos;
                            createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
                            createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().offsetMin = new Vector2(-10, 20);
                            createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().offsetMax = new Vector2(10, 20);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Error!!! 当前动作数据在VFPlayer上的animShowList找不到！");
                }
            }
            else
            {//source is file
                for (int i = 0; i < vfPlayer.loadedJARanges_master.selectRanges.Count; i++)
                {
                    AddRangesMember(vfPlayer.loadedJARanges_master.selectRanges[i], i + 1);
                }
                ResetRangesMembersPos();//reset pos

                for (int i = 0; i < vfPlayer.loadedJARanges_master.selectRanges.Count; i++)//重新设置层级
                { 
                    if (vfPlayer.loadedJARanges_master.selectRanges[i].type == AnimRangeControl.SelectType.timePoint)
                    {
                        createdButtons[i].sliderButton1.transform.SetParent(null);
                        createdButtons[i].sliderButton1.transform.SetParent(buttonParent);

                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localScale = Vector3.one;
                        Vector3 _lpos = createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localPosition;
                        _lpos.z = 0f;
                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localPosition = _lpos;
                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().offsetMin = new Vector2(-10, 20);
                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().offsetMax = new Vector2(10, 20);
                    }
                }
            }
            /*
            int num1 = findInVFPlayerAnims();
            if (num1 != -1)
            {
                for(int i=0; i< vfPlayer.animShowList[num1].selectRanges.Count; i++)
                {
                    AddRangesMember(vfPlayer.animShowList[num1].selectRanges[i], i+1);
                }
                ResetRangesMembersPos();//reset pos

                for (int i = 0; i < vfPlayer.animShowList[num1].selectRanges.Count; i++)//重新设置层级
                {
                    if (vfPlayer.animShowList[num1].selectRanges[i].type == AnimRangeControl.SelectType.timePoint)
                    {
                        createdButtons[i].sliderButton1.transform.SetParent(null);
                        createdButtons[i].sliderButton1.transform.SetParent(buttonParent);

                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localScale = Vector3.one;
                        Vector3 _lpos = createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localPosition;
                        _lpos.z = 0f;
                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localPosition = _lpos;
                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().offsetMin = new Vector2(-10, 20);
                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().offsetMax = new Vector2(10, 20);
                    }
                }
            }
            else
            {
                Debug.LogError("Error!!! 当前大师动作数据在VFPlayer上的animShowList找不到！");
            }*/
        }
        else//student
        {
            if(sourceIsFile == false)
            {
                int num1 = findInVFPlayerAnims();
                if (num1 != -1)
                {
                    for (int i = 0; i < vfPlayer.animShowList[num1].selectRanges.Count; i++)
                    {
                        AddRangesMember(vfPlayer.animShowList[num1].selectRanges[i], i+1);
                    }
                    ResetRangesMembersPos();//reset pos

                    for (int i = 0; i < vfPlayer.animShowList[num1].selectRanges.Count; i++)//重新设置层级
                    {
                        if (vfPlayer.animShowList[num1].selectRanges[i].type == AnimRangeControl.SelectType.timePoint)
                        {
                            createdButtons[i].sliderButton1.transform.SetParent(null);
                            createdButtons[i].sliderButton1.transform.SetParent(buttonParent);

                            createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localScale = Vector3.one;
                            Vector3 _lpos = createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localPosition;
                            _lpos.z = 0f;
                            createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localPosition = _lpos;
                            createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
                            createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().offsetMin = new Vector2(-10, 20);
                            createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().offsetMax = new Vector2(10, 20);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Error!!! 当前动作数据在VFPlayer上的animShowList找不到！");
                }
            }
            else
            {//source is file
                for (int i = 0; i < vfPlayer.loadedJARanges_student.selectRanges.Count; i++)
                {
                    AddRangesMember(vfPlayer.loadedJARanges_student.selectRanges[i], i+1);
                }
                ResetRangesMembersPos();//reset pos

                for (int i = 0; i < vfPlayer.loadedJARanges_student.selectRanges.Count; i++)//重新设置层级
                {
                    if (vfPlayer.loadedJARanges_student.selectRanges[i].type == AnimRangeControl.SelectType.timePoint)
                    {
                        createdButtons[i].sliderButton1.transform.SetParent(null);
                        createdButtons[i].sliderButton1.transform.SetParent(buttonParent);

                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localScale = Vector3.one;
                        Vector3 _lpos = createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localPosition;
                        _lpos.z = 0f;
                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().localPosition = _lpos;
                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().offsetMin = new Vector2(-10, 20);
                        createdButtons[i].sliderButton1.transform.GetComponent<RectTransform>().offsetMax = new Vector2(10, 20);
                    }
                }
            }
        }
 
    }

    private void ClearModel()
    {
        if (isMaster)
        {
            for (int i = 0; i < vfPlayer.ch_master.added_createdGA.Count; i++)
            {
                Destroy(vfPlayer.ch_master.added_createdGA[i]);
            }
            vfPlayer.ch_master.added_createdGA.Clear();
            vfPlayer.ch_master.added_bm_p.Clear();
            vfPlayer.ch_master.added_ModelNum.Clear();
        }
        else
        {
            for (int i = 0; i < vfPlayer.ch_student.added_createdGA.Count; i++)
            {
                Destroy(vfPlayer.ch_student.added_createdGA[i]);
            }
            vfPlayer.ch_student.added_createdGA.Clear();
            vfPlayer.ch_student.added_bm_p.Clear();
            vfPlayer.ch_student.added_ModelNum.Clear();
        }
    }

    public void OnSelectModel(int _num)//模型选择功能
    {
        if(_num == 0)
        {
            button_showModel.image.sprite = button_showModel_Normal;
            ClearModel();
        }
        else
        {
            if(_num <= vfPlayer.animModelList.Count && _num > 0)
            {
                ClearModel();
                int num = _num - 1;
                if (isMaster)
                {
                    if (vfPlayer.animModelList[num].frame_model != null && vfPlayer.animModelList[num].autodata_model != null)
                    {
                        GameObject addedga1 = Instantiate<GameObject>(vfPlayer.animModelList[num].frame_model);
                        vfPlayer.ch_master.added_createdGA.Add(addedga1);
                        vfPlayer.ch_master.added_ModelNum.Add(num);
                        addedga1.transform.position = vfPlayer.animModelList[num].pos_model;
                        Debug.Log("model.pos_model: " + addedga1.transform.position);
                        addedga1.transform.rotation = Quaternion.Euler(vfPlayer.animModelList[num].roa_model);
                        vfPlayer.SetAllChildLayer(addedga1, LayerMask.NameToLayer("cam1"));//set layer  设置layer是cam1 但是注释掉之后是基本没区别的 不知道是哪里用

                        int num_anim = vfPlayer.ch_master.createdGA_No;
                        BoneMatch bm1 = new BoneMatch();//bonetype的列表
                        bm1.StartAutoMatch(addedga1.transform, vfPlayer.animModelList[num].autodata_model, vfPlayer.ch_master.jad_created);//蒙皮跟着骨骼动的关键   
                        BoneMatch_Player p1 = new BoneMatch_Player(bm1);
                        vfPlayer.ch_master.added_bm_p.Add(p1);
                        p1.SetJAnimationData(vfPlayer.ch_master.jad_created);//数据跟随的关键
                        vfPlayer.ch_master.useBoneAdapt = vfPlayer.animModelList[num].useBoneAdapt;//设置为true 才会比较缩放大小跟骨骼基本匹配
                         
                        button_showModel.image.sprite = button_showModel_HighLight;
                    }
                }
                else
                {
                    if (vfPlayer.animModelList[num].frame_model != null && vfPlayer.animModelList[num].autodata_model != null)
                    {
                        GameObject addedga1 = Instantiate<GameObject>(vfPlayer.animModelList[num].frame_model);
                        vfPlayer.ch_student.added_createdGA.Add(addedga1);
                        vfPlayer.ch_student.added_ModelNum.Add(num);
                        addedga1.transform.position = vfPlayer.animModelList[num].pos_model;
                        addedga1.transform.rotation = Quaternion.Euler(vfPlayer.animModelList[num].roa_model);
                        vfPlayer.SetAllChildLayer(addedga1, LayerMask.NameToLayer("cam2"));//set layer 

                        int num_anim = vfPlayer.ch_student.createdGA_No;
                        BoneMatch bm1 = new BoneMatch();
                        bm1.StartAutoMatch(addedga1.transform, vfPlayer.animModelList[num].autodata_model, vfPlayer.ch_student.jad_created);
                        BoneMatch_Player p1 = new BoneMatch_Player(bm1);
                        vfPlayer.ch_student.added_bm_p.Add(p1);
                        p1.SetJAnimationData(vfPlayer.ch_student.jad_created);
                        vfPlayer.ch_student.useBoneAdapt = vfPlayer.animModelList[num].useBoneAdapt;

                        button_showModel.image.sprite = button_showModel_HighLight;
                    }
                }
            }
            else
            {
                dd_selectModel.value = 0;
            }
            vfPlayer.posControl.Refresh();
        }
    }

    /// <summary>
    /// 增加一个选段
    /// </summary>
    public void Button_AddOneRange()
    {
        if (canEdit == false) return;
        if (sourceIsFile == true)//  is from file
        {
            if(isMaster)
            {
                vfPlayer.loadedJARanges_master.selectRanges.Add(new AnimRangeControl.SelectOne());
            }
            else
            {
                vfPlayer.loadedJARanges_student.selectRanges.Add(new AnimRangeControl.SelectOne());
            } 
            RefreshRangesUIs();//重新绘制
        }
        else
        {
            int num1 = findInVFPlayerAnims();
            if (num1 != -1)
            {
                vfPlayer.animShowList[num1].selectRanges.Add(new AnimRangeControl.SelectOne());
                RefreshRangesUIs();//重新绘制
            }
            else
            {
                Debug.LogError("ERROR!!  NOT FOUND !");
                return;
            }
        }
    }
    public void Button_Save()
    { 
        if (sourceIsFile == true)//is from file
        {
            if(isMaster)
            {
                JAnimDataToFile.SaveJAnimRanges(JAnimDataToFile.JADSavePath, nameText.text.Substring(0, nameText.text.Length - 5), vfPlayer.loadedJARanges_master);
            }
            else
            {
                JAnimDataToFile.SaveJAnimRanges(JAnimDataToFile.JADSavePath, nameText.text.Substring(0, nameText.text.Length - 5), vfPlayer.loadedJARanges_student);
            }
        }
        else
        {
            int num1 = findInVFPlayerAnims();
            if (num1 != -1)
            {
                JAnimDataToFile.JAnimRanges nranges = new JAnimDataToFile.JAnimRanges();
                nranges.selectRanges = vfPlayer.animShowList[num1].selectRanges;
                JAnimDataToFile.SaveJAnimRanges(JAnimDataToFile.JADSavePath, vfPlayer.animShowList[num1].name, nranges);
            }
            else
            {
                Debug.LogError("ERROR!");
            }
        }
    }


    BoneMatch_Player bm_p1_follow = null;
    Transform followBone = null;
    Transform followTarget = null;
    public void Button_ShowAnotherBone()
    {
        if(isShowingAnotherBone)
        {
            isShowingAnotherBone = false;
            button_showAnotherBone.GetComponent<Image>().color = Color.white;
            Destroy(followBone.root.gameObject);
            bm_p1_follow = null;
            followBone = null;
            followTarget = null; 
        }
        else
        {
            if(isMaster)
            {
                if(vfPlayer.ch_student.hasInit)
                {
                    bm_p1_follow = vfPlayer.CreatFollowerBone(vfPlayer.ch_student.createdGA_No);
                    int index_followBone = bm_p1_follow.GetPlayerIndexByBoneType(JAnimationData.BoneType.hips);
                    followBone = bm_p1_follow.bonePlayers[index_followBone].boneOneMatch.matchedT;
                    int index_followTarget = vfPlayer.ch_master.bm_p1.GetPlayerIndexByBoneType(JAnimationData.BoneType.hips);
                    followTarget = vfPlayer.ch_master.bm_p1.bonePlayers[index_followTarget].boneOneMatch.matchedT;
                    isShowingAnotherBone = true;
                    button_showAnotherBone.GetComponent<Image>().color = Color.green;
                } 
            }
        }
    }

    /// <summary>
    /// 向数据注册是否界面展开状态
    /// </summary> 
    public void RangeMemberTryChangeMaxSizeBool(AnimInfoWindow_rangesMember member1, bool useMaxSize, bool refreshUIs = true)
    {
        if (canEdit == false) return;
        if (sourceIsFile == true)
        {
            for (int i = 0; i < createdRangesMembers.Count; i++)
            {
                if (createdRangesMembers[i] == member1)
                {
                    if(isMaster)
                    {
                        vfPlayer.loadedJARanges_master.selectRanges[i].UIisMaxSize = useMaxSize;
                    }
                    else
                    {
                        vfPlayer.loadedJARanges_student.selectRanges[i].UIisMaxSize = useMaxSize;
                    } 
                    if(refreshUIs) RefreshRangesUIs();
                    break;
                }

            }
        }
        else
        {
            //内置数据

            for (int i = 0; i < createdRangesMembers.Count; i++)
            {
                if (createdRangesMembers[i] == member1)
                {
                    int num1 = findInVFPlayerAnims();
                    if (num1 != -1)
                    {
                        vfPlayer.animShowList[num1].selectRanges[i].UIisMaxSize = useMaxSize;
                        if (refreshUIs) RefreshRangesUIs();
                        break;
                    }
                    else
                    {
                        Debug.LogError("ERROR!!  NOT FOUND !");
                        return;
                    }

                }

            }
        }
    }

    public void RangeMemberTryChangeType(AnimInfoWindow_rangesMember member1, AnimRangeControl.SelectType _type1)
    {
        if (canEdit == false) return;
        if (sourceIsFile == true)
        {
            if(isMaster)
            {
                for (int i = 0; i < createdRangesMembers.Count; i++)
                {
                    if (createdRangesMembers[i] == member1)
                    {
                        if (_type1 == AnimRangeControl.SelectType.timePoint)
                        {
                            vfPlayer.loadedJARanges_master.selectRanges[i].type = AnimRangeControl.SelectType.timePoint;
                            vfPlayer.loadedJARanges_master.selectRanges[i].timePoint = 0f;
                        }
                        else if (_type1 == AnimRangeControl.SelectType.timeRange)
                        {
                            vfPlayer.loadedJARanges_master.selectRanges[i].type = AnimRangeControl.SelectType.timeRange;
                            vfPlayer.loadedJARanges_master.selectRanges[i].timeRange_start = Time_start;
                            vfPlayer.loadedJARanges_master.selectRanges[i].timeRange_end = Time_start + TimeLength;
                        }
                        RefreshRangesUIs();
                        break;
                    }

                }
            }
            else//student
            {
                for (int i = 0; i < createdRangesMembers.Count; i++)
                {
                    if (createdRangesMembers[i] == member1)
                    {
                        if (_type1 == AnimRangeControl.SelectType.timePoint)
                        {
                            vfPlayer.loadedJARanges_student.selectRanges[i].type = AnimRangeControl.SelectType.timePoint;
                            vfPlayer.loadedJARanges_student.selectRanges[i].timePoint = 0f;
                        }
                        else if (_type1 == AnimRangeControl.SelectType.timeRange)
                        {
                            vfPlayer.loadedJARanges_student.selectRanges[i].type = AnimRangeControl.SelectType.timeRange;
                            vfPlayer.loadedJARanges_student.selectRanges[i].timeRange_start = Time_start;
                            vfPlayer.loadedJARanges_student.selectRanges[i].timeRange_end = Time_start + TimeLength;
                        }
                        RefreshRangesUIs();
                        break;
                    }

                }
            }
           
        }
        else
        {
            //内置数据

            for (int i = 0; i < createdRangesMembers.Count; i++)
            {
                if (createdRangesMembers[i] == member1)
                {
                    int num1 = findInVFPlayerAnims();
                    if (num1 != -1)
                    {
                        if (_type1 == AnimRangeControl.SelectType.timePoint)
                        {
                            vfPlayer.animShowList[num1].selectRanges[i].type = AnimRangeControl.SelectType.timePoint;
                            vfPlayer.animShowList[num1].selectRanges[i].timePoint = 0f;
                        }
                        else if (_type1 == AnimRangeControl.SelectType.timeRange)
                        {
                            vfPlayer.animShowList[num1].selectRanges[i].type = AnimRangeControl.SelectType.timeRange;
                            vfPlayer.animShowList[num1].selectRanges[i].timeRange_start = Time_start;
                            vfPlayer.animShowList[num1].selectRanges[i].timeRange_end = Time_start + TimeLength;
                        }
                        RefreshRangesUIs();
                        break;
                    }
                    else
                    {
                        Debug.LogError("ERROR!!  NOT FOUND !");
                        return;
                    }

                }

            }
        }
    }
    /// <summary>
    /// 在某个选段的右上角点了X，想要删除这个选段
    /// </summary>
    public void RangeMemberWantToDestroy(AnimInfoWindow_rangesMember member1)
    {
        if (canEdit == false) return;
        if(isMaster == true)
        {
            if (sourceIsFile)
            {
                for (int i = 0; i < createdRangesMembers.Count; i++)
                {
                    if (createdRangesMembers[i] == member1)
                    {
                        vfPlayer.loadedJARanges_master.selectRanges.RemoveAt(i);
                        RefreshRangesUIs();//重新绘制
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < createdRangesMembers.Count; i++)
                {
                    if (createdRangesMembers[i] == member1)
                    {
                        int num1 = findInVFPlayerAnims();
                        if (num1 != -1)
                        {
                            vfPlayer.animShowList[num1].selectRanges.RemoveAt(i);
                            RefreshRangesUIs();//重新绘制
                            break;
                        }
                        else
                        {
                            Debug.LogError("ERROR!!  NOT FOUND !");
                            return;
                        }
                    }
                }
            }
          
        }
        else if (isMaster == false)
        {
            if(sourceIsFile)
            {
                for (int i = 0; i < createdRangesMembers.Count; i++)
                {
                    if (createdRangesMembers[i] == member1)
                    {
                        vfPlayer.loadedJARanges_student.selectRanges.RemoveAt(i);
                        RefreshRangesUIs();//重新绘制
                        break;
                    }
                }
            }
            else
            {
                //
                for (int i = 0; i < createdRangesMembers.Count; i++)
                {
                    if (createdRangesMembers[i] == member1)
                    {
                        int num1 = findInVFPlayerAnims();
                        if (num1 != -1)
                        {
                            vfPlayer.animShowList[num1].selectRanges.RemoveAt(i);
                            RefreshRangesUIs();//重新绘制
                            break;
                        }
                        else
                        {
                            Debug.LogError("ERROR!!  NOT FOUND !");
                            return;
                        }
                    }
                }
            }
        } 
    }

    /// <summary>
    /// 当前对应的动作数据在VFPlayer里面的哪个
    /// </summary>
    /// <returns></returns>
    int findInVFPlayerAnims()
    {
        for(int i=0; i<vfPlayer.animShowList.Count; i++)
        {
            if(vfPlayer.animShowList[i].name == nameText.text)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 在销毁时，由RightStackController调用
    /// </summary>
    public override void BeforeDestroy()
    { 
        if (isMaster)
        {
            if (isShowingAnotherBone)
            {
                isShowingAnotherBone = false;
                button_showAnotherBone.GetComponent<Image>().color = Color.white;
                Destroy(followBone.root.gameObject);
                bm_p1_follow = null;
                followBone = null;
                followTarget = null;
            }

            vfPlayer.ch_master.selectBones.Clear();
            /*
            if(vfPlayer.ch_student.selectBones.Count == 0)
            {
                vfPlayer.DestroyBoneInfoWindow();
            } */
            if (vfPlayer.ch_student.hasInit == false)
            {
                vfPlayer.DestroyBoneInfoWindow();
            }
        }
        else 
        {
            vfPlayer.ch_student.selectBones.Clear();
            /*
            if (vfPlayer.ch_master.selectBones.Count == 0)
            {
                vfPlayer.DestroyBoneInfoWindow();
            }*/
            if (vfPlayer.ch_master.hasInit == false)
            {
                vfPlayer.DestroyBoneInfoWindow();
            }
        }

        ClearCreatedRangesMembersAndButtons();
    }
    public override void Show()
    {

    }


    public void SetTime(float _start, float _length, float _totalLength)
    {
        TimeLength = _length;
        Time_start = _start;
        TimeTotalLength = _totalLength; 
    }

    public void SetBasicInfo(string name1, string introduction1, bool _isMaster, bool _sourceIsFile, bool _canEdit, Slider _targetSlider)
    {
        nameText.text = name1;
        introductionText.text = introduction1;
        isMaster = _isMaster; 
        sourceIsFile = _sourceIsFile;
        canEdit = _canEdit;

        targetSlider = _targetSlider;
        for(int i=0; i<targetSlider.transform.childCount; i++)
        {
            if(targetSlider.transform.GetChild(i).name == "Handle Slide Area")
            {
                buttonParent = targetSlider.transform.GetChild(i).GetComponent<RectTransform>();
                break;
            }
        }
    }

    public void ResetRangesMembersPos()
    {
        for (int i = 0; i < createdRangesMembers.Count; i++)
        {
            RectTransform rectt1 = createdRangesMembers[i].gameObject.GetComponent<RectTransform>();
            float newx = rectt1.rect.width / 2f + 10f;
            float newy;
            if (i == 0)
            {
                newy = -rectt1.rect.height / 2f  + rangesMemberPosStart;// 

            }
            else
            {
                newy =
                    createdRangesMembers[i-1].gameObject.GetComponent<RectTransform>().anchoredPosition.y - createdRangesMembers[i-1].gameObject.GetComponent<RectTransform>().rect.height / 2f -
                    rectt1.rect.height / 2f;
            }
            rectt1.anchoredPosition = new Vector2(newx, newy);
            rectt1.localScale = Vector3.one;
            Vector3 _lpos = rectt1.localPosition;
            _lpos.z = 0f;
            rectt1.localPosition = _lpos;
        }

        TurnToMaxSize();
    }

    public void ClearCreatedRangesMembersAndButtons()
    {
        for(int i=0; i< createdRangesMembers.Count; i++)
        {
            Destroy(createdRangesMembers[i].gameObject);
        }
        createdRangesMembers.Clear();
        for (int i = 0; i < createdButtons.Count; i++)
        {
            Destroy(createdButtons[i].sliderButton1.gameObject);
            if(createdButtons[i].sliderButton2) Destroy(createdButtons[i].sliderButton2.gameObject);
        }
        createdButtons.Clear();

        buttons.Clear();

        for(int i=0; i< createdPanels.Count; i++)
        {
            createdPanels[i].transform.SetParent(null);
            Destroy(createdPanels[i]);
        }
        createdPanels.Clear();
    }

    public void AddRangesMember(AnimRangeControl.SelectOne info, int _count)
    {
        GameObject ga = Instantiate<GameObject>(rangesMemberPrefab);
        ga.SetActive(true);
        AnimInfoWindow_rangesMember c1 = ga.GetComponent<AnimInfoWindow_rangesMember>();
        createdRangesMembers.Add(c1);
        ga.GetComponent<RectTransform>().SetParent(rangesMemberParent);
        c1.SetInfo(info, this, _count);

        buttons.Add(new SliderButtonPearData());
        createdButtons.Add(new CreatedButtonPear(null, null));
         

        if (info.type == AnimRangeControl.SelectType.timePoint)
        {
            buttons[buttons.Count - 1].time1 = info.timePoint;

            bool canCreate = true;
            if (isMaster)//判断是不是不可以显示在范围内
            {
                if (info.timePoint < vfPlayer.ch_master.min || info.timePoint > vfPlayer.ch_master.max)
                {
                    //canCreate = false;
                }
            }
            else
            {
                if (info.timePoint < vfPlayer.ch_student.min || info.timePoint > vfPlayer.ch_student.max)
                {
                    //canCreate = false;
                }
            }

            if(canCreate)
            {
                //create button on slider
                GameObject button1ga = Instantiate<GameObject>(buttonPrefab);
                button1ga.SetActive(true);
                SliderButton sliderButton1 = button1ga.GetComponent<SliderButton>();
                sliderButton1.control = this;
                sliderButton1.indexInControl = buttons.Count - 1;
                sliderButton1.isleft = true;
                sliderButton1.usingThis = canEdit;
                sliderButton1.sliderWidth = targetSlider.gameObject.GetComponent<RectTransform>().rect.width;
                button1ga.GetComponent<RectTransform>().SetParent(buttonParent); 
                button1ga.GetComponent<RectTransform>().localScale = Vector3.one;
                Vector3 _lpos = button1ga.GetComponent<RectTransform>().localPosition;
                _lpos.z = 0f;
                button1ga.GetComponent<RectTransform>().localPosition = _lpos;
                button1ga.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
                button1ga.GetComponent<RectTransform>().offsetMin = new Vector2(-10, 20);
                button1ga.GetComponent<RectTransform>().offsetMax = new Vector2(10, 20);
                button1ga.GetComponent<Image>().color = Color.yellow;//set color
                createdButtons[createdButtons.Count - 1].sliderButton1 = sliderButton1;
            } 
        }
        else if(info.type == AnimRangeControl.SelectType.timeRange)
        {
            buttons[buttons.Count - 1].time1 = info.timeRange_start;
            buttons[buttons.Count - 1].time2 = info.timeRange_end;
            bool canCreate1 = true;
            bool canCreate2 = true;
            if (isMaster)//判断是不是不可以显示在范围内
            {
                if (info.timeRange_start < vfPlayer.ch_master.min)
                {
                    //canCreate1 = false; 
                }
            }
            else
            {
                if (info.timeRange_start < vfPlayer.ch_student.min)
                {
                    //canCreate1 = false;
                }
            }
             
            if (isMaster)//判断是不是不可以显示在范围内
            {
                if (info.timeRange_end > vfPlayer.ch_master.max)
                {
                    //canCreate2 = false;
                }
            }
            else
            {
                if (info.timeRange_end > vfPlayer.ch_student.max)
                {
                    //canCreate2 = false; 
                }
            }
            GameObject button1ga = null;
            GameObject button2ga = null;
            SliderButton sliderButton1 = null;
            SliderButton sliderButton2 = null;
            if (canCreate1)
            {
                //create button on slider
                button1ga = Instantiate<GameObject>(buttonPrefab);
                button1ga.SetActive(true);
                sliderButton1 = button1ga.GetComponent<SliderButton>();
                sliderButton1.control = this;
                sliderButton1.indexInControl = buttons.Count - 1;
                sliderButton1.isleft = true;
                sliderButton1.usingThis = canEdit;
                sliderButton1.sliderWidth = targetSlider.gameObject.GetComponent<RectTransform>().rect.width;
                button1ga.GetComponent<RectTransform>().SetParent(buttonParent);
                button1ga.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
                button1ga.GetComponent<RectTransform>().offsetMin = new Vector2(-10, 20);
                button1ga.GetComponent<RectTransform>().offsetMax = new Vector2(10, 20);
                button1ga.GetComponent<RectTransform>().localScale = Vector3.one;
                Vector3 _lpos = button1ga.GetComponent<RectTransform>().localPosition;
                _lpos.z = 0f;
                button1ga.GetComponent<RectTransform>().localPosition = _lpos;
                button1ga.GetComponent<Image>().color = Color.blue;//set color
                createdButtons[createdButtons.Count - 1].sliderButton1 = sliderButton1;
            }
            if (canCreate2)
            {
                //create button on slider
                button2ga = Instantiate<GameObject>(buttonPrefab);
                button2ga.SetActive(true);
                sliderButton2 = button2ga.GetComponent<SliderButton>();
                sliderButton2.control = this;
                sliderButton2.indexInControl = buttons.Count - 1;
                sliderButton2.isleft = false;
                sliderButton2.usingThis = canEdit;
                sliderButton2.sliderWidth = targetSlider.gameObject.GetComponent<RectTransform>().rect.width;
                sliderButton2.GetComponent<RectTransform>().SetParent(buttonParent);
                sliderButton2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
                sliderButton2.GetComponent<RectTransform>().offsetMin = new Vector2(-10, 20);
                sliderButton2.GetComponent<RectTransform>().offsetMax = new Vector2(10, 20);
                sliderButton2.GetComponent<RectTransform>().localScale = Vector3.one;
                Vector3 _lpos = sliderButton2.GetComponent<RectTransform>().localPosition;
                _lpos.z = 0f;
                sliderButton2.GetComponent<RectTransform>().localPosition = _lpos;
                sliderButton2.GetComponent<Image>().color = Color.blue;//set color
                createdButtons[createdButtons.Count - 1].sliderButton2 = sliderButton2;
            }
            if(canCreate1 && canCreate2)
            {
                GameObject panel1 = Instantiate<GameObject>(panelPrefab);
                createdPanels.Add(panel1);
                panel1.SetActive(true);
                panel1.GetComponent<Image>().color = Color.blue;
                panel1.GetComponent<RectTransform>().SetParent(buttonParent);
                panel1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
                panel1.GetComponent<RectTransform>().offsetMin = new Vector2(-10, 20);
                panel1.GetComponent<RectTransform>().offsetMax = new Vector2(10, 20);
                panel1.GetComponent<RectTransform>().localScale = Vector3.one;
                Vector3 _lpos = panel1.GetComponent<RectTransform>().localPosition;
                _lpos.z = 0f;
                panel1.GetComponent<RectTransform>().localPosition = _lpos;
                button1ga.GetComponent<RectTransform>().SetParent(null);//重新设置button1的父级，让它显示在panel1前面
                button1ga.GetComponent<RectTransform>().SetParent(buttonParent);
                button1ga.GetComponent<RectTransform>().localScale = Vector3.one;
                _lpos = button1ga.GetComponent<RectTransform>().localPosition;
                _lpos.z = 0f;
                button1ga.GetComponent<RectTransform>().localPosition = _lpos;
                button1ga.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
                button1ga.GetComponent<RectTransform>().offsetMin = new Vector2(-10, 20);
                button1ga.GetComponent<RectTransform>().offsetMax = new Vector2(10, 20);
                button2ga.GetComponent<RectTransform>().SetParent(null);//重新设置button2的父级，让它显示在panel1前面
                button2ga.GetComponent<RectTransform>().SetParent(buttonParent);
                button2ga.GetComponent<RectTransform>().localScale = Vector3.one;
                _lpos = button2ga.GetComponent<RectTransform>().localPosition;
                _lpos.z = 0f;
                button2ga.GetComponent<RectTransform>().localPosition = _lpos;
                button2ga.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
                button2ga.GetComponent<RectTransform>().offsetMin = new Vector2(-10, 20);
                button2ga.GetComponent<RectTransform>().offsetMax = new Vector2(10, 20);
                sliderButton1.limitBack = sliderButton2;
                sliderButton2.limitFront = sliderButton1;
                sliderButton1.panelRect = panel1.GetComponent<RectTransform>();
                sliderButton2.panelRect = panel1.GetComponent<RectTransform>();
                sliderButton1.Refresh();
                sliderButton2.Refresh();
            }
             
        }
     
    }



    public void ClearCreatedButtons()
    {
        for (int i = 0; i < createdButtons.Count; i++)
        {
            Destroy(createdButtons[i].sliderButton1.gameObject);
            if(createdButtons[i].sliderButton2 != null)
                Destroy(createdButtons[i].sliderButton2.gameObject);
        }
        createdButtons.Clear();
        buttons.Clear();
    }


    public override void DestroyThis()
    {
        if (isMaster)
        {
            vfPlayer.DestroyCreatedMaster(true);
        }
        else
        {
            vfPlayer.DestroyCreatedStudent(true);
        } 
    }

}
