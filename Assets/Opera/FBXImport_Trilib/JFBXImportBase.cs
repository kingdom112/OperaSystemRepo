﻿#pragma warning disable 649
using System;
using TriLibCore.General;
using TriLibCore.Utils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TriLibCore;

public class JFBXImportBase : MonoBehaviour
{
    /// <summary>Gets the Asset Viewer Singleton instance.</summary>
    public static JFBXImportBase Instance { get; private set; }


    public Text messageText;

    /// <summary>
    /// Model/skybox loading bar. (Used on platforms with async capabilities)
    /// </summary>
    [SerializeField]
    private RectTransform _loadingBar;

     

    /// <summary>
    /// Loading screen wrapper. (Used on platforms without async capabilities)
    /// </summary>
    [SerializeField]
    private GameObject _loadingWrapper;

    /// <summary>
    /// Model URL loading dialog.
    /// </summary>
    [SerializeField]
    private GameObject _modelUrlDialog;

    /// <summary>
    ///  Model URL loading Input Field.
    /// </summary>
    [SerializeField]
    private InputField _modelUrl;

    /// <summary>
    /// Animation playback slider.
    /// </summary>
    [SerializeField]
    protected Slider PlaybackSlider;

    /// <summary>
    /// Animation playback time.
    /// </summary>
    [SerializeField]
    protected Text PlaybackTime;

    /// <summary>
    /// Animation selector.
    /// </summary>
    [SerializeField]
    protected Dropdown PlaybackAnimation;

    

    /// <summary>
    /// Options used in this sample.
    /// </summary>
    protected AssetLoaderOptions AssetLoaderOptions;
     

    /// <summary>
    /// Loaded game object.
    /// </summary>
    public GameObject RootGameObject { get; protected set; }
     
     

    /// <summary>
    /// Shows the model URL dialog.
    /// </summary>
    public void ShowModelUrlDialog()
    {
        _modelUrlDialog.SetActive(true);
        _modelUrl.Select();
        _modelUrl.ActivateInputField();
    }

    /// <summary>
    /// Hides the model URL dialog.
    /// </summary>
    public void HideModelUrlDialog()
    {
        _modelUrlDialog.SetActive(false);
        _modelUrl.text = null;
    }

    /// <summary>
    /// Shows the file picker for loading a model from local file-system.  use TriLib loading
    /// </summary>
    protected void LoadModelFromFile(GameObject wrapperGameObject = null, Action<AssetLoaderContext> onMaterialsLoad = null)
    {
        SetLoading(false);
        var filePickerAssetLoader = AssetLoaderFilePicker.Create();
        filePickerAssetLoader.LoadModelFromFilePickerAsync("Select a File", OnLoad, onMaterialsLoad ?? OnMaterialsLoad, OnProgress, OnBeginLoadModel, OnError, wrapperGameObject ?? gameObject, AssetLoaderOptions);
    }
     

    /// <summary>Event triggered when the user selects a file or cancels the Model selection dialog.</summary>
    /// <param name="hasFiles">If any file has been selected, this value is <c>true</c>, otherwise it is <c>false</c>.</param>
    protected virtual void OnBeginLoadModel(bool hasFiles)
    {
        if (hasFiles)
        {
            if (RootGameObject != null)
            {
                Destroy(RootGameObject);
            }
            SetLoading(true);
        }
    }

    /// <summary>
    /// Enables/disables the loading flag.
    /// </summary>
    /// <param name="value">The new loading flag.</param>
    protected void SetLoading(bool value)
    {
        var selectables = FindObjectsOfType<Selectable>();
        for (var i = 0; i < selectables.Length; i++)
        {
            var button = selectables[i];
            button.interactable = !value;
        }
#if UNITY_WSA || UNITY_WEBGL || TRILIB_FORCE_SYNC
            _loadingWrapper.gameObject.SetActive(value);
#else
        _loadingBar.gameObject.SetActive(value);
#endif
    }

    /// <summary>Checks if the Dispatcher instance exists and stores this class instance as the Singleton.</summary>
    protected virtual void Start()
    {
        //Dispatcher.CheckInstance();
        //PasteManager.CheckInstance();
        Instance = this;
    }

    /// <summary>Event is triggered when the Model loading progress changes.</summary>
    /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
    /// <param name="value">The loading progress, ranging from 0 to 1.</param>
    protected virtual void OnProgress(AssetLoaderContext assetLoaderContext, float value)
    {
        messageText.text = "载入FBX中";
        _loadingBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * value);
    }

    /// <summary>Event is triggered when any error occurs.</summary>
    /// <param name="contextualizedError">The Contextualized Error that has occurred.</param>
    protected virtual void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError(contextualizedError);
        RootGameObject = null;
        SetLoading(false);
    }

    /// <summary>Event is triggered when the Model Meshes and hierarchy are loaded.</summary>
    /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
    protected virtual void OnLoad(AssetLoaderContext assetLoaderContext)
    {

    }

    /// <summary>Event is triggered when the Model (including Textures and Materials) has been fully loaded.</summary>
    /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
    protected virtual void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        SetLoading(false);
    }

    /// <summary>
    /// Plays the selected animation.
    /// </summary>
    public virtual void PlayAnimation()
    {
    }

    /// <summary>Stops playing the selected animation.</summary>
    public virtual void StopAnimation()
    {
    }

    /// <summary>Switches to the animation selected on the Dropdown.</summary>
    /// <param name="index">The selected Animation index.</param>
    public virtual void PlaybackAnimationChanged(int index)
    {

    }

    /// <summary>
    /// Event triggered when the animation slider value has been changed by the user.
    /// </summary>
    /// <param name="value">The Animation playback normalized position.</param>
    public virtual void PlaybackSliderChanged(float value)
    {

    }
}
