using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JAnimationSystem;

public class Recode_Control : RightStackMember
{

    public enum RecordType
    {
        Vicon,
        master,
        student
    }

    [System.Serializable]
    public class RecodeTargetData
    {
        public GameObject prefab;
    }
   
    [System.Serializable] 
    public class UITypePanel
    {
        public GameObject vicon;
        public GameObject master;
        public GameObject student;

        public void ChangeUIType(RecordType _recordType)
        {
            if (_recordType == RecordType.Vicon)
            {
                vicon.SetActive(true);
                master.SetActive(false);
                student.SetActive(false);
            }
            else if (_recordType == RecordType.master)
            {
                vicon.SetActive(false);
                master.SetActive(true);
                student.SetActive(false);
            }
            else if (_recordType == RecordType.student)
            {
                vicon.SetActive(false);
                master.SetActive(false);
                student.SetActive(true);
            }
        }
    }
    [System.Serializable]
    public class UIButtons
    {
        public Text messageText;
        public Button vicon_Button_LoadModel;
        public Button vicon_Button_DeleteModel;
        public Button vicon_Button_StartRecord;
        public Button vicon_Button_StopRecord;
        public InputField vicon_input_IP;
        public InputField vicon_input_Port;
        public Button master_Button_StartRecord;
        public Button master_Button_StopRecord;
        public Button student_Button_StartRecord;
        public Button student_Button_StopRecord;
    }

    [Space(10f)]
    public UITypePanel uiPanels = new UITypePanel();
    public UIButtons uiButtons = new UIButtons();
    /// <summary>
    /// 目前的录制类型
    /// </summary>
    private RecordType recordType = 0;
    public GameObject recodeTarget;
    public JAnimation_AutoSetBoneMatchData autoMatchData;
    public float frequency = 60;
    public Dropdown loadRecodeTargetDropdown;
    public List<RecodeTargetData> recodeTargetDatas = new List<RecodeTargetData>();
    public Slider ReplayProcessSlider;
    public Text ReplayProcessText;
    public GameObject UI_RecodePart;
    public GameObject UI_AfterRecodePart;
    public InputField SaveFileNameInput;
    private JAnimRecoder recoder = new JAnimRecoder();
    private BoneMatch_Preview recodePlayer = new BoneMatch_Preview();
    private bool isReplaying = false;
    private float timer = 0f;
    public float Timer
    {
        get
        {
            return timer;
        }
    }
    private float timeRecoded = 0f;
    public float TimeRecoded
    {
        get
        {
            return timeRecoded;
        }
    }

    private VFPlayer_InGame vfplayer = null;
    private void Awake()
    {
        vfplayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
    }
    void Start()
    {
        isReplaying = false;
        ChangeUIType(RecordType.Vicon);
        vfplayer.AfterCreateCharacterAction += Check;
        uiButtons.messageText.text = "等待操作中";
        uiButtons.vicon_input_IP.text = "localhost";
        uiButtons.vicon_input_Port.text = "801";
    }

    void Update()
    {
        
    }

    public void ChangeUIType(int _recordType)
    { 
        ChangeUIType((RecordType)_recordType);
    }
    public void ChangeUIType(RecordType _recordType)
    {
        recordType = _recordType;//

        uiPanels.ChangeUIType(_recordType);
        Check();
    }

    private void Check()
    {
        if (recordType == RecordType.Vicon)
        {
            DestroyRecodeTarget();
            uiButtons.vicon_Button_LoadModel.interactable = true;
            uiButtons.vicon_Button_DeleteModel.interactable = false;
            uiButtons.vicon_Button_StartRecord.interactable = false;
            uiButtons.vicon_Button_StopRecord.interactable = false;
        
        }
        else if (recordType == RecordType.master)
        {
            DestroyRecodeTarget();
            _player = vfplayer.CreatFollowerBone_master();
            if (_player != null)
            {
                recodeTarget = _player.GetRootTransform().gameObject;
            }
            uiButtons.master_Button_StartRecord.interactable = true;
            uiButtons.master_Button_StopRecord.interactable = false;

        }
        else if (recordType == RecordType.student)
        {
            DestroyRecodeTarget();
             _player = vfplayer.CreatFollowerBone_student();
            if (_player != null)
            {
                recodeTarget = _player.GetRootTransform().gameObject;
            }
            uiButtons.student_Button_StartRecord.interactable = true;
            uiButtons.student_Button_StopRecord.interactable = false;

        }

        if (UI_AfterRecodePart)
        {
            if (recoder.HasRecoded)
            {
                UI_AfterRecodePart.SetActive(false);//close AfterRecode UI
            }
        }
    }

    /// <summary>
    /// 只在master和student类型时使用
    /// </summary>
    private BoneMatch_Player _player;
    private void FixedUpdate()
    { 
        if(isReplaying)
        {
            if(recodeTarget != null)
            {
                if (recodePlayer.boneMatchList.Count == 0) return;
                if (ReplayProcessSlider)
                {
                    ReplayProcessSlider.value = timer / timeRecoded;
                }
                if (ReplayProcessText)
                {
                    ReplayProcessText.text = "Time:" + timer.ToString() + "/" + timeRecoded.ToString();
                }
                if (timer <= timeRecoded)
                {
                    recodePlayer.PlayDataInThisFrame_Curve(timer);
                    timer += Time.fixedDeltaTime;
                }
                else
                {
                    isReplaying = false;
                }
            }
            else
            {
                isReplaying = false;
                timer = 0f;
               
            }



        }
        else
        {
            if (recordType == RecordType.master && recodeTarget != null)
            {
                _player.PlayDataInThisFrame_Curve(vfplayer.ch_master.timer);
            }
            else if (recordType == RecordType.student && recodeTarget != null)
            {
                _player.PlayDataInThisFrame_Curve(vfplayer.ch_student.timer);
            }
        }

        recoder.CallFixedUpdate();
    }

    private void InitUIs()
    {
        if(loadRecodeTargetDropdown)
        {
            loadRecodeTargetDropdown.ClearOptions();
            List<Dropdown.OptionData> datas = new List<Dropdown.OptionData>();

            for (int i=0; i< recodeTargetDatas.Count; i++)
            {
                Dropdown.OptionData data1 = new Dropdown.OptionData();
                data1.text = recodeTargetDatas[i].prefab.name;
                datas.Add(data1);
            }
            loadRecodeTargetDropdown.AddOptions(datas);
        }
    }

    /// <summary>
    /// 销毁recodeTarget
    /// </summary>
    private void DestroyRecodeTarget()
    {
        if (recodeTarget != null)
        {
            Destroy(recodeTarget);
            recodeTarget = null;
        }
        if(recoder.IsRecoding == true)
        {
            recoder.StopRecode(); 
        }
        uiButtons.messageText.text = "等待操作中";
    }
    public void DeleteTheRecodeTarget()
    {
        if (UI_RecodePart)
        {
            if (recoder.IsRecoding == false)
            {
                DestroyRecodeTarget();
            }
        }
    }

    public void LoadTheRecodeTargetOfDropdownNow()
    {
        if (recoder.IsRecoding == true) return;
        if (loadRecodeTargetDropdown)
        {
            if(recodeTargetDatas.Count > loadRecodeTargetDropdown.value 
                && loadRecodeTargetDropdown.value >= 0)
            {
                LoadRecodeTarget(recodeTargetDatas[loadRecodeTargetDropdown.value].prefab);
            }
        }
    }

    private void LoadRecodeTarget(GameObject prefab1)
    {
        if (recoder.IsRecoding == true) return;
        if (prefab1 == null) return;
        GameObject newga = Instantiate<GameObject>(prefab1);
        DestroyRecodeTarget();
        recodeTarget = newga;

        JViconControl viconControl = recodeTarget.GetComponentInChildren<JViconControl>();
        if(viconControl == null)
        {
            Debug.LogError("NO JViconControl on the model !!!!");
            DestroyRecodeTarget();
            return;
        }
        uiButtons.vicon_Button_LoadModel.interactable = false;
        uiButtons.vicon_Button_DeleteModel.interactable = true;
        uiButtons.messageText.text = "连接Vicon中......";
        viconControl.StartLink(
            uiButtons.vicon_input_IP.text, 
            uiButtons.vicon_input_Port.text,
            LinkViconSuccess,
            LinkViconError);//link 
    }
    private void LinkViconSuccess()
    {
        Debug.Log("连接Vicon成功，可进行录制");
        uiButtons.messageText.text = "连接Vicon成功，可进行录制";
        uiButtons.master_Button_StartRecord.interactable = true;
    }
    private void LinkViconError()
    {
        DestroyRecodeTarget();
        Debug.Log("连接Vicon失败");
        uiButtons.messageText.text = "连接Vicon失败";
        uiButtons.vicon_Button_LoadModel.interactable = true;
        uiButtons.vicon_Button_DeleteModel.interactable = false;
    }
   
    public override void Show()
    {
        if (UI_RecodePart)
        {
            UI_RecodePart.SetActive(true);
            if (UI_AfterRecodePart)
            {
                if (recoder.HasRecoded)
                {
                    UI_AfterRecodePart.SetActive(true);
                }
                else
                {
                    UI_AfterRecodePart.SetActive(false);
                }
            }

            InitUIs();
        }
    }
         
    public override bool CanDestroy
    {
        get
        {
            if (UI_RecodePart)
            {
                if (recoder.IsRecoding == false)
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 在销毁时，由RightStackController调用
    /// </summary>
    public override void BeforeDestroy()
    {
        vfplayer.AfterCreateCharacterAction -= Check;
        if (recodeTarget)
        {
            Destroy(recodeTarget);
        }
    }
   

    public void SaveRecodedToFile ()
    {
        string path = JAnimDataToFile.JADSavePath;

        if(SaveFileNameInput)
        {
            if (recodeTarget == null) return;
            if (autoMatchData == null) return;
            if (recodeTargetDatas.Count <= loadRecodeTargetDropdown.value || loadRecodeTargetDropdown.value < 0) return;
            if (recodeTargetDatas[loadRecodeTargetDropdown.value] == null) return;
            string filenName1 = SaveFileNameInput.text;
            recoder.SaveToFile(recodeTarget, autoMatchData, path, filenName1, recodeTargetDatas[loadRecodeTargetDropdown.value].prefab.name);
            VFPlayer_InGame vfPlayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
            vfPlayer.RefreshSelectMotionsList();
        }
        else
        {
            Debug.LogError("Can't save !!! No SaveFileNameText !!! Please check it !!!");
        }
    }

    public void StartReplayTheRecode()
    {
        if (recodeTarget == null) return;
        if (recoder.HasRecoded == false) return;
        if (recodePlayer.boneMatchList.Count == 0) return;
        if (recoder.IsRecoding == true) return;
        isReplaying = true;
        timer = 0f;
        Debug.Log("Start ReplayTheRecode");
    }
    public void PauseReplayTheRecode()
    {
        if (recoder.IsRecoding == true) return;
        isReplaying = !isReplaying;
    }
    public void StopReplayTheRecode()
    {
        if (recoder.IsRecoding == true) return;
        isReplaying = false;
        timer = 0f;
        Debug.Log("Stop ReplayTheRecode");
    }


    public void StopRecode()
    {
        recoder.StopRecode();
        recodePlayer.StartAutoMatch(recodeTarget.transform, autoMatchData);
        recodePlayer.SetJAnimationData(recoder.jAD);
        timeRecoded = recoder.jAD.TimeLength;
        timer = 0f;
        ReplayProcessText.text = "Time:" + timer.ToString() + "/" + timeRecoded.ToString();
        //UI
        if (UI_AfterRecodePart)
        {
            if (recoder.HasRecoded)
            {
                UI_AfterRecodePart.SetActive(true);
            }
        }
        if (recordType == RecordType.Vicon)
        {
            uiButtons.vicon_Button_StartRecord.interactable = true;
            uiButtons.vicon_Button_StopRecord.interactable = false;
        }
        else if (recordType == RecordType.master)
        {
            uiButtons.master_Button_StartRecord.interactable = true;
            uiButtons.master_Button_StopRecord.interactable = false;
        }
        else if (recordType == RecordType.student)
        {
            uiButtons.student_Button_StartRecord.interactable = true;
            uiButtons.student_Button_StopRecord.interactable = false;
        }
        uiButtons.messageText.text = "录制数据完成，可重播数据";
        Debug.Log("Stop Recode");
    }

    public void DragReplaySlider(float newValue)
    {
        recodePlayer.PlayDataInThisFrame_Curve(newValue * timeRecoded);
    }
    public void StartRecode()
    {
        if (UI_AfterRecodePart)
        {
            if (recoder.HasRecoded)
            {
                UI_AfterRecodePart.SetActive(false);//close AfterRecode UI
            }
        }
        if (recodeTarget == null)
        {
            Debug.Log("无录制对象！");
            return;
        }
        if (autoMatchData == null)
        {
            Debug.Log("autoMatchData == null    ！");
            return;
        }
        isReplaying = false;
        recoder = new JAnimRecoder();
        recodePlayer = new BoneMatch_Preview();
        recoder.StartRecode(recodeTarget, autoMatchData, frequency);
        if (recordType == RecordType.Vicon)
        {
            uiButtons.vicon_Button_StartRecord.interactable = false;
            uiButtons.vicon_Button_StopRecord.interactable = true;
        }
        else if (recordType == RecordType.master)
        {
            uiButtons.master_Button_StartRecord.interactable = false;
            uiButtons.master_Button_StopRecord.interactable = true;
        }
        else if (recordType == RecordType.student)
        {
            uiButtons.student_Button_StartRecord.interactable = false;
            uiButtons.student_Button_StopRecord.interactable = true;
        }
        uiButtons.messageText.text = "录制中...";
        Debug.Log("Start Recode");
    }

}
