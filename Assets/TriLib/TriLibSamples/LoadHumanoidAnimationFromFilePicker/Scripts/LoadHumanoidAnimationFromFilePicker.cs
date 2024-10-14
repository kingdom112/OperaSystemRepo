#pragma warning disable 649
using TriLibCore.General;
using TriLibCore.Mappers;
using TriLibCore.Playables;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace TriLibCore.Samples
{
    /// <summary>
    /// Represents a sample that allows user loading Humanoid Animations with a File Picker.
    /// </summary>
    public class LoadHumanoidAnimationFromFilePicker : MonoBehaviour
    {
        /// <summary>
        /// Simple Animation Player component located on the existing Model in the Scene.
        /// The Simple Animation Player is used to play Generic/Humanoid Animations based on their names or indices.
        /// </summary>
        public SimpleAnimationPlayer OriginalModelSimpleAnimationPlayer;

        /// <summary>
        /// Template mecanim animation clip.
        /// Unity runtime API can't access mecanim animation clip settings as root motion baking, animation loop mode, etc.
        /// So we get these settings from the template animation clip.
        public AnimationClip MecanimAnimationClipTemplate;

        /// <summary>
        /// The load Animation Button.
        /// </summary>
        [SerializeField]
        private Button _loadAnimationButton;

        /// <summary>
        /// The template Button used to play the Animations.
        /// </summary>
        [SerializeField]
        private Button _animationPlayTemplate;

        /// <summary>
        /// The last loaded GameObject.
        /// </summary>
        private GameObject _loadedGameObject;

        /// <summary>
        /// All loaded Animation Clips.
        /// </summary>
        private List<AnimationClip> _loadedAnimationClips = new List<AnimationClip>();

        /// <summary>
        /// Creates the AssetLoaderOptions instance and displays the Model file-picker.
        /// First we will point the Simple Animation Player Animations list to the loaded Animation Clips list.
        /// We will disable the AssetUnloader options since we want to keep the loaded Animation Clips in memory.
        /// We will disable Materials, Textures and Meshes importing since we just want the Humanoid Animations to be imported.
        /// We will add the Mixamo and Biped built-in Humanoid Avatar Mapper.
        /// We will add am Animation Clip Mapper used to convert Legacy to Humanoid Animations and a Generic Animation playback Component to the Model.
        /// </summary>
        /// <remarks>
        /// You can create the AssetLoaderOptions by right clicking on the Assets Explorer and selecting "TriLib->Create->AssetLoaderOptions->Pre-Built AssetLoaderOptions".
        /// </remarks>
        public void LoadAnimation()
        {
            OriginalModelSimpleAnimationPlayer.AnimationClips = _loadedAnimationClips;

            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            assetLoaderOptions.AddAssetUnloader = false;
            assetLoaderOptions.ImportMaterials = false;
            assetLoaderOptions.ImportTextures = false;
            assetLoaderOptions.ImportMeshes = false;
            assetLoaderOptions.SampleBindPose = true;
            assetLoaderOptions.AnimationType = AnimationType.Humanoid;
            assetLoaderOptions.HumanoidAvatarMapper = Resources.Load<HumanoidAvatarMapper>("Mappers/Avatar/MixamoAndBipedByNameHumanoidAvatarMapper");

            var legacyToHumanoidAnimationClipMapper = ScriptableObject.CreateInstance<LegacyToHumanoidAnimationClipMapper>();
            legacyToHumanoidAnimationClipMapper.MecanimAnimationClipTemplate = MecanimAnimationClipTemplate;
            legacyToHumanoidAnimationClipMapper.name = "LegacyToHumanoidAnimationClipMapper";
            assetLoaderOptions.FixedAllocations.Add(legacyToHumanoidAnimationClipMapper);

            var simpleAnimationPlayerAnimationClipMapper = ScriptableObject.CreateInstance<SimpleAnimationPlayerAnimationClipMapper>();
            simpleAnimationPlayerAnimationClipMapper.name = "SimpleAnimationPlayerAnimationClipMapper";
            assetLoaderOptions.FixedAllocations.Add(simpleAnimationPlayerAnimationClipMapper);

            assetLoaderOptions.AnimationClipMappers = new AnimationClipMapper[]
            {
                legacyToHumanoidAnimationClipMapper,
                simpleAnimationPlayerAnimationClipMapper
            };

            var assetLoaderFilePicker = AssetLoaderFilePicker.Create();
            assetLoaderFilePicker.LoadModelFromFilePickerAsync("Select a Model file", OnLoad, OnMaterialsLoad, OnProgress, OnBeginLoad, OnError, null, assetLoaderOptions);
        }

        /// <summary>
        /// Called when the the Model begins to load.
        /// </summary>
        /// <param name="filesSelected">Indicates if any file has been selected.</param>
        private void OnBeginLoad(bool filesSelected)
        {
            _loadAnimationButton.interactable = !filesSelected;
        }

        /// <summary>
        /// Called when any error occurs.
        /// </summary>
        /// <param name="obj">The contextualized error, containing the original exception and the context passed to the method where the error was thrown.</param>
        private void OnError(IContextualizedError obj)
        {
            Debug.LogError($"An error ocurred while loading your Model: {obj.GetInnerException()}");
        }

        /// <summary>
        /// Called when the Model loading progress changes.
        /// </summary>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        /// <param name="progress">The loading progress.</param>
        private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
        {
            Debug.Log($"Loading Model. Progress: {progress:P}");
        }

        /// <summary>
        /// Called when the Model (including Textures and Materials) has been fully loaded, or after any error occurs.
        /// Here we get the loaded Animations that have been added to the Simple Animation Player added to the loaded GameObject
        /// and copy them to the Simple Animation Player of the existing Model in the Scene.
        /// We also create Buttons to play the loaded Animation Clips.
        /// </summary>
        /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        {
            if (assetLoaderContext.RootGameObject != null)
            {
                Debug.Log("Materials loaded. Model fully loaded.");
                var simpleAnimationPlayer = assetLoaderContext.RootGameObject.GetComponent<SimpleAnimationPlayer>();
                if (simpleAnimationPlayer != null)
                {
                    for (var i = 0; i < simpleAnimationPlayer.AnimationClips.Count; i++)
                    {
                        var animationClip = simpleAnimationPlayer.AnimationClips[i];
                        // Ignore Animations not-imported.
                        if (animationClip == null)
                        {
                            continue;
                        }
                        var animationIndex = _loadedAnimationClips.Count;
                        var button = Instantiate(_animationPlayTemplate, _animationPlayTemplate.transform.parent);
                        button.GetComponentInChildren<Text>().text = animationClip.name;
                        button.onClick.AddListener(delegate { OriginalModelSimpleAnimationPlayer.PlayAnimation(animationIndex); });
                        button.gameObject.SetActive(true);
                        _loadedAnimationClips.Add(animationClip);
                    }
                }
                Destroy(assetLoaderContext.RootGameObject); //Don't destroy the GameObject if you're using the AssetLoaderOptions.AddAssetUnloader options as this will break the AnimationClip references
            }
            else
            {
                Debug.Log("Model could not be loaded.");
            }
        }

        /// <summary>
        /// Called when the Model Meshes and hierarchy are loaded.
        /// </summary>
        /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        private void OnLoad(AssetLoaderContext assetLoaderContext)
        {
            _loadAnimationButton.interactable = true;
        }
    }
}
