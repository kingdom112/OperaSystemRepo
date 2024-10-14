#pragma warning disable 649
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TriLibCore.SFB;
using TriLibCore.General;
using TriLibCore.Extensions;
using TriLibCore.Mappers;
using TriLibCore.Utils;
using TriLibCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using JAnimationSystem;

public class JFBXImport : JFBXImportBase
{


    public float recordFrequency = 60;
    public JAnimation_AutoSetBoneMatchData autoMatchData;
    private JAnimRecoder recoder = new JAnimRecoder();

    public Button button_loadFBX; 
    public GameObject playButton;
     
     
     
     
      
     

    /// <summary>
    /// List of loaded animations.
    /// </summary>
    private List<AnimationClip> _animations;

    /// <summary>
    /// Created animation component for the loaded model.
    /// </summary>
    private Animation _animation;

    /// <summary>Gets the playing Animation State.</summary>
    private AnimationState CurrentAnimationState
    {
        get
        {
            if (_animation != null)
            {
                return _animation[PlaybackAnimation.options[PlaybackAnimation.value].text];
            }
            return null;
        }
    }
    /// <summary>Is there any animation playing?</summary>
    private bool AnimationIsPlaying => _animation != null && _animation.isPlaying;

    /// <summary>Shows the file picker for loading a model from the local file-system.</summary>
    public void LoadModelFromFile()//当想要import fbx文件的时候调用
    {
        base.LoadModelFromFile();
    }
     
    

    public void ResetModelScale()
    {
        if (RootGameObject != null)
        {
            RootGameObject.transform.localScale = Vector3.one;
        }
    }

     
     

    /// <summary>Switches to the animation selected on the Dropdown.</summary>
    /// <param name="index">The selected Animation index.</param>
    public override void PlaybackAnimationChanged(int index)
    { 

    }
     

    /// <summary>Samples the Animation at the given normalized time.</summary>
    /// <param name="value">The Animation normalized time.</param>
    private void SampleAnimationAt(float value)
    {
        if (_animation == null || RootGameObject == null)
        {
            return;
        }
        var animationClip = _animation.GetClip(PlaybackAnimation.options[PlaybackAnimation.value].text);
        animationClip.SampleAnimation(RootGameObject, animationClip.length * value);
    }
     
   
     

     

    /// <summary>Initializes the base-class and clears the skybox Texture.</summary>
    protected override void Start()
    {
        base.Start();
        AssetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        AssetLoaderOptions.ShowLoadingWarnings = true;
        AssetLoaderOptions.Timeout = 180;
        messageText.text = "请选择FBX文件";

        playButton.SetActive(false);
        button_loadFBX.interactable = true;
        PlaybackAnimation.interactable = false;
    }

    /// <summary>Handles the input.</summary>
    private void Update()
    { 
        UpdateHUD();  
    }
     

    

    /// <summary>Updates the HUD information.</summary>
    private void UpdateHUD()
    {
        var animationState = CurrentAnimationState;
        var time = animationState == null ? 0f : PlaybackSlider.value * animationState.length % animationState.length;
        var seconds = time % 60f;
        var milliseconds = time * 100f % 100f;
        PlaybackTime.text = $"{seconds:00}:{milliseconds:00}";//将动画的时间显示为秒数：完整时间的毫秒部分的整数值
        var normalizedTime = jplaying == false ? 0f : timer / time_Length;
        if (jplaying)
        {
            PlaybackSlider.value = float.IsNaN(normalizedTime) ? 0f : normalizedTime;
        } 
    }

    /// <summary>Event triggered when the user selects a file or cancels the Model selection dialog.</summary>
    /// <param name="hasFiles">If any file has been selected, this value is <c>true</c>, otherwise it is <c>false</c>.</param>
    protected override void OnBeginLoadModel(bool hasFiles)
    {
        base.OnBeginLoadModel(hasFiles);
        if (hasFiles)
        {
            _animations = null;
        }
    }

    /// <summary>Event triggered when the Model Meshes and hierarchy are loaded.</summary>
    /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
    protected override void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        base.OnLoad(assetLoaderContext);
        ResetModelScale();
        if (assetLoaderContext.RootGameObject != null)
        {
            PlaybackAnimation.options.Clear();
            if (assetLoaderContext.Options.AnimationType == AnimationType.Legacy)
            {
                _animation = assetLoaderContext.RootGameObject.GetComponent<Animation>();//获取fbx文件的animation
                if (_animation != null)
                {
                    _animations = _animation.GetAllAnimationClips();
                    if (_animations.Count > 0)
                    {
                        for (var i = 0; i < _animations.Count; i++)
                        {
                            var animationClip = _animations[i];
                            animationClip.wrapMode = WrapMode.Once; //循环模式 只循环一次
                            PlaybackAnimation.options.Add(new Dropdown.OptionData(animationClip.name));
                        }

                        PlaybackAnimation.captionText.text = _animations[0].name;
                    }
                    else
                    {
                        _animation = null;
                    }
                }
                if (_animation == null)
                {
                    PlaybackAnimation.captionText.text = null;
                }
            }
            PlaybackAnimation.value = 0;
            StopAnimation();
            RootGameObject = assetLoaderContext.RootGameObject;
            
        } 

    }

   

    /// <summary>
    /// Event is triggered when any error occurs.
    /// </summary>
    /// <param name="contextualizedError">The Contextualized Error that has occurred.</param>
    protected override void OnError(IContextualizedError contextualizedError)
    {
        base.OnError(contextualizedError);
        StopAnimation();
    }



    private bool isRecoding = false;
    private void StartRecode()
    { 
        if (RootGameObject == null)
        {
            Debug.Log("无对象！");
            return;
        }
        if (autoMatchData == null)
        {
            Debug.Log("autoMatchData == null    ！");
            return;
        }
        playButton.SetActive(false);
        button_loadFBX.interactable = false;
        PlaybackAnimation.interactable = false;

        recoder = new JAnimRecoder(); 
        recoder.StartRecode(RootGameObject, autoMatchData, recordFrequency);
        Debug.Log("rootgameobject:" + RootGameObject);


        Debug.Log("开始转录");
    }
    private void StopRecode()
    {
        recoder.StopRecode(); 
        float timeRecoded = recoder.jAD.TimeLength; 
        //ReplayProcessText.text = "Time:" + timer.ToString() + "/" + timeRecoded.ToString();  
        Debug.Log("停止转录");
    }
    private void SaveRecodedToFile()
    {
        string path = JAnimDataToFile.JADSavePath;

        if (RootGameObject == null) return;
        if (autoMatchData == null) return;
        string filenName1 = RootGameObject.name;
        recoder.SaveToFile(RootGameObject, autoMatchData, path, filenName1, "");
       
    }
    float timer = 0f;
    float time_Length = 0f;
    /// <summary>
    /// 用于播放和记录。
    /// </summary>
    bool jplaying = false;
    /// <summary>
    /// 开始播放并记录
    /// </summary>
    public void StartPlayAndRecord()
    {
        if (_animation == null || jplaying == true)
        {
            return;
        }
        timer = 0f;
        time_Length = CurrentAnimationState.clip.length;
        StartRecode();
        jplaying = true;  
    }
    private void FixedUpdate()
    {
        if (jplaying)
        {
            playButton.SetActive(false);
            if (RootGameObject != null)
            { 
                if (timer <= time_Length)
                {
                    messageText.text = "转录中， 请稍等";
                    SampleAnimationAt(timer / time_Length);  
                    timer += Time.fixedDeltaTime;
                }
                else
                {
                    StopRecode();
                    SaveRecodedToFile();

                    Destroy(RootGameObject);
                    playButton.SetActive(false);
                    button_loadFBX.interactable = true;
                    PlaybackAnimation.interactable = false;
                    PlaybackAnimation.options.Clear();
                    PlaybackAnimation.captionText.text = null;
                    VFPlayer_InGame vfplayer = FindObjectOfType<VFPlayer_InGame>();
                    if(vfplayer != null)
                    {
                        vfplayer.RefreshSelectMotionsList();
                    }

                    messageText.text = "转录完成。";
                    jplaying = false;
                }
            }
            else
            {
                jplaying = false;
                timer = 0f; 
            }

        }
        else
        {
            if(RootGameObject != null)
            {
                if (_animation != null)
                {
                    messageText.text = "载入FBX完毕，请开始转录";
                    playButton.SetActive(true);
                }
                else
                {
                    messageText.text = "载入的FBX无动画，无法转录!";
                    playButton.SetActive(false);
                }

            } 
        }
        recoder.CallFixedUpdate(); 
    }
}
