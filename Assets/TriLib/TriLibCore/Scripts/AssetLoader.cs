﻿#pragma warning disable 168
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using StbImageSharp;
using TriLibCore.Extensions;
using TriLibCore.General;
using TriLibCore.Interfaces;
using TriLibCore.Mappers;
using TriLibCore.Utils;
using UnityEngine;
using FileMode = System.IO.FileMode;
using HumanDescription = UnityEngine.HumanDescription;
using Object = UnityEngine.Object;
#if TRILIB_DRACO
using TriLibCore.Gltf.Reader;
using TriLibCore.Gltf.Draco;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace TriLibCore
{
    /// <summary>Represents the main class containing methods to load the Models.</summary>
    public static class AssetLoader
    {
        /// <summary>
        /// The step-index used by progress handling when the Animations are processing.
        /// </summary>
        private const int ProcessingAnimationsStep = 0;

        /// <summary>
        /// The step-index used by progress handling when the Textures are processing.
        /// </summary>
        private const int ProcessingTexturesStep = 1;

        /// <summary>
        /// The step-index used by progress handling when the Material Renderers are processing.
        /// </summary>
        private const int ProcessingRenderersStep = 2;

        /// <summary>
        /// The step-index used by progress handling when everything has been processed.
        /// </summary>
        private const int FinalStep = 3;

        /// <summary>
        /// The number of steps the AssetLoader takes while processing the Models.
        /// </summary>
        private const int ExtraStepsCount = 4;

        /// <summary>Loads a Model from the given path asynchronously.</summary>
        /// <param name="path">The Model file path.</param>
        /// <param name="onLoad">The Method to call on the Main Thread when the Model is loaded but resources may still pending.</param>
        /// <param name="onMaterialsLoad">The Method to call on the Main Thread when the Model and resources are loaded.</param>
        /// <param name="onProgress">The Method to call when the Model loading progress changes.</param>
        /// <param name="onError">The Method to call on the Main Thread when any error occurs.</param>
        /// <param name="wrapperGameObject">The Game Object that will be the parent of the loaded Game Object. Can be null.</param>
        /// <param name="assetLoaderOptions">The options to use when loading the Model.</param>
        /// <param name="customContextData">The Custom Data that will be passed along the Context.</param>
        /// <returns>The Asset Loader Context, containing Model loading information and the output Game Object.</returns>
        public static AssetLoaderContext LoadModelFromFile(string path, Action<AssetLoaderContext> onLoad, Action<AssetLoaderContext> onMaterialsLoad, Action<AssetLoaderContext, float> onProgress, Action<IContextualizedError> onError = null, GameObject wrapperGameObject = null, AssetLoaderOptions assetLoaderOptions = null, object customContextData = null)
        {
#if UNITY_WEBGL || UNITY_UWP || TRILIB_FORCE_SYNC
            AssetLoaderContext assetLoaderContext = null;
            try
            {
                assetLoaderContext = LoadModelFromFileNoThread(path, onError, wrapperGameObject, assetLoaderOptions, customContextData);
                onLoad?.Invoke(assetLoaderContext);
                onMaterialsLoad?.Invoke(assetLoaderContext);
            }
            catch (Exception exception)
            {
                if (exception is IContextualizedError contextualizedError)
                {
                    HandleError(contextualizedError);
                }
                else
                {
                    HandleError(new ContextualizedError<AssetLoaderContext>(exception, null));
                }
            }
            return assetLoaderContext;
#else
            var assetLoaderContext = new AssetLoaderContext
            {
                Options = assetLoaderOptions ?? CreateDefaultLoaderOptions(),
                Filename = path,
                BasePath = FileUtils.GetFileDirectory(path),
                WrapperGameObject = wrapperGameObject,
                OnMaterialsLoad = onMaterialsLoad,
                OnLoad = onLoad,
                OnProgress = onProgress,
                HandleError = HandleError,
                OnError = onError,
                CustomData = customContextData,
            };
            if (assetLoaderContext.Options.ForceGCCollectionWhileLoading)
            {
                GCHelper.Instance.Activate();
            }
#if TRILIB_USE_THREAD_NAMES
            var threadName = "TriLib_LoadModelFromFile";
#else
            var threadName = string.Empty;
#endif
            assetLoaderContext.Tasks.Add(ThreadUtils.RunThread(assetLoaderContext, ref assetLoaderContext.CancellationToken, LoadModel, ProcessRootModel, HandleError, assetLoaderContext.Options.Timeout, threadName));
            return assetLoaderContext;
#endif
        }

        /// <summary>Loads a Model from the given Stream asynchronously.</summary>
        /// <param name="stream">The Stream containing the Model data.</param>
        /// <param name="filename">The Model filename.</param>
        /// <param name="fileExtension">The Model file extension. (Eg.: fbx)</param>
        /// <param name="onLoad">The Method to call on the Main Thread when the Model is loaded but resources may still pending.</param>
        /// <param name="onMaterialsLoad">The Method to call on the Main Thread when the Model and resources are loaded.</param>
        /// <param name="onProgress">The Method to call when the Model loading progress changes.</param>
        /// <param name="onError">The Method to call on the Main Thread when any error occurs.</param>
        /// <param name="wrapperGameObject">The Game Object that will be the parent of the loaded Game Object. Can be null.</param>
        /// <param name="assetLoaderOptions">The options to use when loading the Model.</param>
        /// <param name="customContextData">The Custom Data that will be passed along the Context.</param>
        /// <returns>The Asset Loader Context, containing Model loading information and the output Game Object.</returns>
        public static AssetLoaderContext LoadModelFromStream(Stream stream, string filename = null, string fileExtension = null, Action<AssetLoaderContext> onLoad = null, Action<AssetLoaderContext> onMaterialsLoad = null, Action<AssetLoaderContext, float> onProgress = null, Action<IContextualizedError> onError = null, GameObject wrapperGameObject = null, AssetLoaderOptions assetLoaderOptions = null, object customContextData = null)
        {
#if UNITY_WEBGL || UNITY_UWP || TRILIB_FORCE_SYNC
            AssetLoaderContext assetLoaderContext = null;
            try
            {
                assetLoaderContext = LoadModelFromStreamNoThread(stream, filename, fileExtension, onError, wrapperGameObject, assetLoaderOptions, customContextData);
                onLoad?.Invoke(assetLoaderContext);
                onMaterialsLoad?.Invoke(assetLoaderContext);
            }
            catch (Exception exception)
            {
                if (exception is IContextualizedError contextualizedError)
                {
                    HandleError(contextualizedError);
                }
                else
                {
                    HandleError(new ContextualizedError<AssetLoaderContext>(exception, null));
                }
            }
            return assetLoaderContext;
#else
            var assetLoaderContext = new AssetLoaderContext
            {
                Options = assetLoaderOptions ?? CreateDefaultLoaderOptions(),
                Stream = stream,
                Filename = filename,
                FileExtension = fileExtension ?? FileUtils.GetFileExtension(filename, false),
                BasePath = FileUtils.GetFileDirectory(filename),
                WrapperGameObject = wrapperGameObject,
                OnMaterialsLoad = onMaterialsLoad,
                OnLoad = onLoad,
                OnProgress = onProgress,
                HandleError = HandleError,
                OnError = onError,
                CustomData = customContextData
            };
            if (assetLoaderContext.Options.ForceGCCollectionWhileLoading)
            {
                GCHelper.Instance.Activate();
            }
#if TRILIB_USE_THREAD_NAMES
            var threadName = "TriLib_LoadModelFromStream";
#else
            var threadName = string.Empty;
#endif
            assetLoaderContext.Tasks.Add(ThreadUtils.RunThread(assetLoaderContext, ref assetLoaderContext.CancellationToken, LoadModel, ProcessRootModel, HandleError, assetLoaderContext.Options.Timeout, threadName));
            return assetLoaderContext;
#endif
        }

        /// <summary>Loads a Model from the given path synchronously.</summary>
        /// <param name="path">The Model file path.</param>
        /// <param name="onError">The Method to call on the Main Thread when any error occurs.</param>
        /// <param name="wrapperGameObject">The Game Object that will be the parent of the loaded Game Object. Can be null.</param>
        /// <param name="assetLoaderOptions">The options to use when loading the Model.</param>
        /// <param name="customContextData">The Custom Data that will be passed along the Context.</param> 
        /// <returns>The Asset Loader Context, containing Model loading information and the output Game Object.</returns>
        public static AssetLoaderContext LoadModelFromFileNoThread(string path, Action<IContextualizedError> onError = null, GameObject wrapperGameObject = null, AssetLoaderOptions assetLoaderOptions = null, object customContextData = null)
        {
            var assetLoaderContext = new AssetLoaderContext
            {
                Options = assetLoaderOptions == null ? CreateDefaultLoaderOptions() : assetLoaderOptions,
                Filename = path,
                BasePath = FileUtils.GetFileDirectory(path),
                CustomData = customContextData,
                HandleError = HandleError,
                OnError = onError,
                WrapperGameObject = wrapperGameObject,
                Async = false
            };
            try
            {
                if (assetLoaderContext.Options.ForceGCCollectionWhileLoading)
                {
                    GCHelper.Instance.Activate();
                }
                LoadModel(assetLoaderContext);
                if (assetLoaderContext.Reader != null)
                {
                    ProcessRootModel(assetLoaderContext);
                }
            }
            catch (Exception exception)
            {
                HandleError(new ContextualizedError<AssetLoaderContext>(exception, assetLoaderContext));
            }
            return assetLoaderContext;
        }

        /// <summary>Loads a Model from the given Stream synchronously.</summary>
        /// <param name="stream">The Stream containing the Model data.</param>
        /// <param name="filename">The Model filename.</param>
        /// <param name="fileExtension">The Model file extension. (Eg.: fbx)</param>
        /// <param name="onError">The Method to call on the Main Thread when any error occurs.</param>
        /// <param name="wrapperGameObject">The Game Object that will be the parent of the loaded Game Object. Can be null.</param>
        /// <param name="assetLoaderOptions">The options to use when loading the Model.</param>
        /// <param name="customContextData">The Custom Data that will be passed along the Context.</param>
        /// <returns>The Asset Loader Context, containing Model loading information and the output Game Object.</returns>
        public static AssetLoaderContext LoadModelFromStreamNoThread(Stream stream, string filename = null, string fileExtension = null, Action<IContextualizedError> onError = null, GameObject wrapperGameObject = null, AssetLoaderOptions assetLoaderOptions = null, object customContextData = null)
        {
            var assetLoaderContext = new AssetLoaderContext
            {
                Options = assetLoaderOptions ? assetLoaderOptions : CreateDefaultLoaderOptions(),
                Stream = stream,
                Filename = filename,
                FileExtension = fileExtension ?? FileUtils.GetFileExtension(filename, false),
                BasePath = FileUtils.GetFileDirectory(filename),
                CustomData = customContextData,
                HandleError = HandleError,
                OnError = onError,
                WrapperGameObject = wrapperGameObject,
                Async = false
            };
            try
            {
                if (assetLoaderContext.Options.ForceGCCollectionWhileLoading)
                {
                    GCHelper.Instance.Activate();
                }
                LoadModel(assetLoaderContext);
                if (assetLoaderContext.Reader != null)
                {
                    ProcessRootModel(assetLoaderContext);
                }
            }
            catch (Exception exception)
            {
                HandleError(new ContextualizedError<AssetLoaderContext>(exception, assetLoaderContext));
            }
            return assetLoaderContext;
        }

#if UNITY_EDITOR
        private static Object LoadOrCreateScriptableObject(string type, string subFolder)
        {
            string mappersFilePath;
            var triLibMapperAssets = AssetDatabase.FindAssets("TriLibMappersPlaceholder");
            if (triLibMapperAssets.Length > 0)
            {
                mappersFilePath = AssetDatabase.GUIDToAssetPath(triLibMapperAssets[0]);
            }
            else
            {
                throw new Exception("Could not find \"TriLibMappersPlaceholder\" file. Please re-import TriLib package.");
            }
            var mappersDirectory = $"{FileUtils.GetFileDirectory(mappersFilePath)}";
            var assetDirectory = $"{mappersDirectory}/{subFolder}";
            if (!AssetDatabase.IsValidFolder(assetDirectory))
            {
                AssetDatabase.CreateFolder(mappersDirectory, subFolder);
            }
            var assetPath = $"{assetDirectory}/{type}.asset";
            var scriptableObject = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            if (scriptableObject == null)
            {
                scriptableObject = ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(scriptableObject, assetPath);
            }
            return scriptableObject;
        }
#endif

        /// <summary>Creates an Asset Loader Options with the default settings and Mappers.</summary>
        /// <param name="generateAssets">Indicates whether created Scriptable Objects will be saved as assets.</param>
        /// <returns>The Asset Loader Options containing the default settings.</returns>
        public static AssetLoaderOptions CreateDefaultLoaderOptions(bool generateAssets = false)
        {
            var assetLoaderOptions = ScriptableObject.CreateInstance<AssetLoaderOptions>();
            ByNameRootBoneMapper byNameRootBoneMapper;
#if UNITY_EDITOR
            if (generateAssets)
            {
                byNameRootBoneMapper = (ByNameRootBoneMapper)LoadOrCreateScriptableObject("ByNameRootBoneMapper", "RootBone");
            }
            else
            {
                byNameRootBoneMapper = ScriptableObject.CreateInstance<ByNameRootBoneMapper>();
            }
#else
            byNameRootBoneMapper = ScriptableObject.CreateInstance<ByNameRootBoneMapper>();
#endif
            byNameRootBoneMapper.name = "ByNameRootBoneMapper";
            assetLoaderOptions.RootBoneMapper = byNameRootBoneMapper;
            if (MaterialMapper.RegisteredMappers.Count == 0)
            {
                Debug.LogWarning("Please add at least one MaterialMapper name to the MaterialMapper.RegisteredMappers static field to create the right MaterialMapper for the Render Pipeline you are using.");
            }
            else
            {
                var materialMappers = new List<MaterialMapper>();
                foreach (var materialMapperName in MaterialMapper.RegisteredMappers)
                {
                    if (materialMapperName == null)
                    {
                        continue;
                    }
                    MaterialMapper materialMapper;
#if UNITY_EDITOR
                    if (generateAssets)
                    {
                        materialMapper = (MaterialMapper) LoadOrCreateScriptableObject(materialMapperName, "Material");
                    }
                    else
                    {
                        materialMapper = (MaterialMapper) ScriptableObject.CreateInstance(materialMapperName);
                    }
#else
                    materialMapper = ScriptableObject.CreateInstance(materialMapperName) as MaterialMapper;
#endif
                    if (materialMapper != null)
                    {
                        materialMapper.name = materialMapperName;
                        if (materialMapper.IsCompatible(null))
                        {
                            materialMappers.Add(materialMapper);
                            assetLoaderOptions.FixedAllocations.Add(materialMapper);
                        }
                        else
                        {
#if UNITY_EDITOR
                            var assetPath = AssetDatabase.GetAssetPath(materialMapper);
                            if (assetPath == null)
                            {
                                Object.DestroyImmediate(materialMapper);
                            }
#else
                            Object.Destroy(materialMapper);
#endif
                        }
                    }
                }

                if (materialMappers.Count == 0)
                {
                    Debug.LogWarning("TriLib could not find any suitable MaterialMapper on the project.");
                }
                else
                {
                    assetLoaderOptions.MaterialMappers = materialMappers.ToArray();
                }
            }
            assetLoaderOptions.FixedAllocations.Add(assetLoaderOptions);
            return assetLoaderOptions;
        }

        /// <summary>Processes the Model from the given context and begin to build the Game Objects.</summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        private static void ProcessModel(AssetLoaderContext assetLoaderContext)
        {
            if (assetLoaderContext.RootModel != null)
            {
                ParseModel(assetLoaderContext, assetLoaderContext.WrapperGameObject != null ? assetLoaderContext.WrapperGameObject.transform : null, assetLoaderContext.RootModel, assetLoaderContext.RootModel, out assetLoaderContext.RootGameObject);
                assetLoaderContext.RootGameObject.transform.localScale = Vector3.one;
                if (assetLoaderContext.Options.AnimationType != AnimationType.None || assetLoaderContext.Options.ImportBlendShapes)
                {
                    SetupRootBone(assetLoaderContext);
                    SetupModelBones(assetLoaderContext, assetLoaderContext.RootModel);
                    SetupModelLod(assetLoaderContext, assetLoaderContext.RootModel);
                    BuildGameObjectsPaths(assetLoaderContext);
                    SetupRig(assetLoaderContext);
                }
                if (assetLoaderContext.Options.Static)
                {
                    assetLoaderContext.RootGameObject.isStatic = true;
                }
            }
            assetLoaderContext.OnLoad?.Invoke(assetLoaderContext);
        }

        /// <summary>Configures the context Model LODs (levels-of-detail) if there are any.</summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        /// <param name="model">The Model containing the LOD data.</param>
        private static void SetupModelLod(AssetLoaderContext assetLoaderContext, IModel model)
        {
            if (model.Children != null && model.Children.Count > 0)
            {
                var lodModels = new Dictionary<int, Renderer[]>(model.Children.Count);
                var minLod = int.MaxValue;
                var maxLod = 0;
                for (var i = 0; i < model.Children.Count; i++)
                {
                    var child = model.Children[i];
                    var match = Regex.Match(child.Name, "_LOD(?<number>[0-9]+)|LOD_(?<number>[0-9]+)");
                    if (match.Success)
                    {
                        var lodNumber = Convert.ToInt32(match.Groups["number"].Value);
                        if (lodModels.ContainsKey(lodNumber))
                        {
                            continue;
                        }

                        minLod = Mathf.Min(lodNumber, minLod);
                        maxLod = Mathf.Max(lodNumber, maxLod);
                        lodModels.Add(lodNumber, assetLoaderContext.GameObjects[child].GetComponentsInChildren<Renderer>());
                    }
                }

                if (lodModels.Count > 1)
                {
                    var newGameObject = assetLoaderContext.GameObjects[model];
                    var lods = new LOD[lodModels.Count + 1];
                    var lodGroup = newGameObject.AddComponent<LODGroup>();
                    var index = 0;
                    var lastPosition = 1f;
                    for (var i = minLod; i <= maxLod; i++)
                    {
                        if (lodModels.TryGetValue(i, out var renderers))
                        {
                            lods[index++] = new LOD(lastPosition, renderers);
                            lastPosition *= 0.5f;
                        }
                    }
                    lods[index] = new LOD(lastPosition, null);
                    lodGroup.SetLODs(lods);
                }
            }
        }

        /// <summary>Tries to find the Context Model root-bone.</summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        private static void SetupRootBone(AssetLoaderContext assetLoaderContext)
        {
            var bones = new List<Transform>(assetLoaderContext.Models.Count);
            assetLoaderContext.RootModel.GetBones(assetLoaderContext, bones);
            assetLoaderContext.BoneTransforms = bones.ToArray();
            if (assetLoaderContext.Options.RootBoneMapper != null)
            {
                assetLoaderContext.RootBone = assetLoaderContext.Options.RootBoneMapper.Map(assetLoaderContext);
            }
        }

        /// <summary>Builds the Game Object Converts hierarchy paths.</summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        private static void BuildGameObjectsPaths(AssetLoaderContext assetLoaderContext)
        {
            using (var values = assetLoaderContext.GameObjects.Values.GetEnumerator())
            {
                while (values.MoveNext())
                {
                    assetLoaderContext.GameObjectPaths.Add(values.Current, values.Current.transform.BuildPath(assetLoaderContext.RootGameObject.transform));
                }
            }
        }

        /// <summary>Configures the context Model rigging if there is any.</summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        private static void SetupRig(AssetLoaderContext assetLoaderContext)
        {
            var animations = assetLoaderContext.RootModel.AllAnimations;
            AnimationClip[] animationClips = null;
            switch (assetLoaderContext.Options.AnimationType)
            {
                case AnimationType.Legacy:
                    {
                        if (animations != null)
                        {
                            animationClips = new AnimationClip[animations.Count];
                            var unityAnimation = assetLoaderContext.RootGameObject.AddComponent<Animation>();
                            for (var i = 0; i < animations.Count; i++)
                            {
                                var triLibAnimation = animations[i];
                                var animationClip = ParseAnimation(assetLoaderContext, triLibAnimation);
                                unityAnimation.AddClip(animationClip, animationClip.name);
                                unityAnimation.clip = animationClip;
                                unityAnimation.wrapMode = assetLoaderContext.Options.AnimationWrapMode;
                                animationClips[i] = animationClip;
                                assetLoaderContext.Reader.UpdateLoadingPercentage(i, assetLoaderContext.Reader.LoadingStepsCount + ProcessingAnimationsStep, animations.Count);
                            }
                        }
                        break;
                    }
                case AnimationType.Generic:
                    {
                        var animator = assetLoaderContext.RootGameObject.AddComponent<Animator>();
                        if (assetLoaderContext.Options.AvatarDefinition == AvatarDefinitionType.CopyFromOtherAvatar)
                        {
                            animator.avatar = assetLoaderContext.Options.Avatar;
                        }
                        else
                        {
                            SetupGenericAvatar(assetLoaderContext, animator);
                        }
                        if (animations != null)
                        {
                            animationClips = new AnimationClip[animations.Count];
                            for (var i = 0; i < animations.Count; i++)
                            {
                                var triLibAnimation = animations[i];
                                var animationClip = ParseAnimation(assetLoaderContext, triLibAnimation);
                                animationClips[i] = animationClip;
                                assetLoaderContext.Reader.UpdateLoadingPercentage(i, assetLoaderContext.Reader.LoadingStepsCount + ProcessingAnimationsStep, animations.Count);
                            }
                        }
                        break;
                    }
                case AnimationType.Humanoid:
                    {
                        var animator = assetLoaderContext.RootGameObject.AddComponent<Animator>();
                        if (assetLoaderContext.Options.AvatarDefinition == AvatarDefinitionType.CopyFromOtherAvatar)
                        {
                            animator.avatar = assetLoaderContext.Options.Avatar;
                        }
                        else if (assetLoaderContext.Options.HumanoidAvatarMapper != null)
                        {
                            SetupHumanoidAvatar(assetLoaderContext, animator);
                        }
                        if (animations != null)
                        {
                            animationClips = new AnimationClip[animations.Count];
                            for (var i = 0; i < animations.Count; i++)
                            {
                                var originalAnimation = animations[i];
                                var animationClip = ParseAnimation(assetLoaderContext, originalAnimation);
                                animationClips[i] = animationClip;
                                assetLoaderContext.Reader.UpdateLoadingPercentage(i, assetLoaderContext.Reader.LoadingStepsCount + ProcessingAnimationsStep, animations.Count);
                            }
                        }
                        break;
                    }
            }
            if (animationClips != null)
            {
                if (assetLoaderContext.Options.AnimationClipMappers != null)
                {
                    for (var i = 0; i < assetLoaderContext.Options.AnimationClipMappers.Length; i++)
                    {
                        var animationClipMapper = assetLoaderContext.Options.AnimationClipMappers[i];
                        if (animationClipMapper == null)
                        {
                            continue;
                        }

                        animationClips = animationClipMapper.MapArray(assetLoaderContext, animationClips);
                    }
                }

                for (var i = 0; i < animationClips.Length; i++)
                {
                    var animationClip = animationClips[i];
                    assetLoaderContext.Allocations.Add(animationClip);
                }
            }
        }

        /// <summary>Creates a Skeleton Bone for the given Transform.</summary>
        /// <param name="boneTransform">The bone Transform to use on the Skeleton Bone.</param>
        /// <returns>The created Skeleton Bone.</returns>
        private static SkeletonBone CreateSkeletonBone(Transform boneTransform)
        {
            var skeletonBone = new SkeletonBone
            {
                name = boneTransform.name,
                position = boneTransform.localPosition,
                rotation = boneTransform.localRotation,
                scale = boneTransform.localScale
            };
            return skeletonBone;
        }

        /// <summary>Creates a Human Bone for the given Bone Mapping, containing the relationship between the Transform and Bone.</summary>
        /// <param name="boneMapping">The Bone Mapping used to create the Human Bone, containing the information used to search for bones.</param>
        /// <param name="boneName">The bone name to use on the created Human Bone.</param>
        /// <returns>The created Human Bone.</returns>
        private static HumanBone CreateHumanBone(BoneMapping boneMapping, string boneName)
        {
            var humanBone = new HumanBone
            {
                boneName = boneName,
                humanName = GetHumanBodyName(boneMapping.HumanBone),
                limit =
                {
                    useDefaultValues = boneMapping.HumanLimit.useDefaultValues,
                    axisLength = boneMapping.HumanLimit.axisLength,
                    center = boneMapping.HumanLimit.center,
                    max = boneMapping.HumanLimit.max,
                    min = boneMapping.HumanLimit.min
                }
            };
            return humanBone;
        }

        /// <summary>Returns the given Human Body Bones name as String.</summary>
        /// <param name="humanBodyBones">The Human Body Bones to get the name from.</param>
        /// <returns>The Human Body Bones name.</returns>
        private static string GetHumanBodyName(HumanBodyBones humanBodyBones)
        {
            return HumanTrait.BoneName[(int)humanBodyBones];
        }

        /// <summary>Creates a Generic Avatar to the given context Model.</summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        /// <param name="animator">The Animator assigned to the given Context Root Game Object.</param>
        private static void SetupGenericAvatar(AssetLoaderContext assetLoaderContext, Animator animator)
        {
            var parent = assetLoaderContext.RootGameObject.transform.parent;
            assetLoaderContext.RootGameObject.transform.SetParent(null, true);
            var avatar = AvatarBuilder.BuildGenericAvatar(assetLoaderContext.RootGameObject, assetLoaderContext.RootBone != null ? assetLoaderContext.RootBone.name : "");
            avatar.name = $"{assetLoaderContext.RootGameObject.name}Avatar";
            animator.avatar = avatar;
            assetLoaderContext.RootGameObject.transform.SetParent(parent, true);
        }

        /// <summary>Creates a Humanoid Avatar to the given context Model.</summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        /// <param name="animator">The Animator assigned to the given Context Root Game Object.</param>
        private static void SetupHumanoidAvatar(AssetLoaderContext assetLoaderContext, Animator animator)
        {
            var mapping = assetLoaderContext.Options.HumanoidAvatarMapper.Map(assetLoaderContext);
            if (mapping.Count > 0)
            {
                var parent = assetLoaderContext.RootGameObject.transform.parent;
                var rootGameObjectPosition = assetLoaderContext.RootGameObject.transform.position;
                assetLoaderContext.RootGameObject.transform.SetParent(null, false);
                assetLoaderContext.Options.HumanoidAvatarMapper.PostSetup(assetLoaderContext, mapping);
                Transform hipsTransform = null;
                var humanBones = new HumanBone[mapping.Count];
                var boneIndex = 0;
                using (var keys = mapping.Keys.GetEnumerator())
                using (var values = mapping.Values.GetEnumerator())
                {
                    while (keys.MoveNext() && values.MoveNext())
                    {
                        if (keys.Current.HumanBone == HumanBodyBones.Hips)
                        {
                            hipsTransform = values.Current;
                        }
                        humanBones[boneIndex++] = CreateHumanBone(keys.Current, values.Current.name);
                    }
                }
                if (hipsTransform != null)
                {
                    var hipsRotation = hipsTransform.rotation;
                    var skeletonBones = new Dictionary<Transform, SkeletonBone>();
                    var extraTransforms = new List<Transform>();
                    var parentTransform = hipsTransform.parent;
                    while (parentTransform != null)
                    {
                        extraTransforms.Add(parentTransform);
                        parentTransform = parentTransform.parent;
                    }
                    for (var i = extraTransforms.Count - 1; i >= 0; i--)
                    {
                        var extraTransform = extraTransforms[i];
                        if (!skeletonBones.ContainsKey(extraTransform))
                        {
                            extraTransform.up = Vector3.up;
                            extraTransform.forward = Vector3.forward;
                            skeletonBones.Add(extraTransform, CreateSkeletonBone(extraTransform));
                        }
                    }
                    var hipsRotationOffset = hipsRotation * Quaternion.Inverse(hipsTransform.rotation);
                    hipsTransform.rotation = hipsRotationOffset * hipsTransform.rotation;
                    hipsTransform.position = hipsRotationOffset * hipsTransform.position;
                    using (var keys = assetLoaderContext.GameObjects.Keys.GetEnumerator())
                    using (var values = assetLoaderContext.GameObjects.Values.GetEnumerator())
                        while (keys.MoveNext() && values.MoveNext())
                        {
                            if (keys.Current.IsBone)
                            {
                                if (!skeletonBones.ContainsKey(values.Current.transform))
                                {
                                    skeletonBones.Add(values.Current.transform, CreateSkeletonBone(values.Current.transform));
                                }
                            }
                        }
                    var triLibHumanDescription = assetLoaderContext.Options.HumanDescription ?? new General.HumanDescription();
                    var humanDescription = new HumanDescription
                    {
                        armStretch = triLibHumanDescription.armStretch,
                        feetSpacing = triLibHumanDescription.feetSpacing,
                        hasTranslationDoF = triLibHumanDescription.hasTranslationDof,
                        legStretch = triLibHumanDescription.legStretch,
                        lowerArmTwist = triLibHumanDescription.lowerArmTwist,
                        lowerLegTwist = triLibHumanDescription.lowerLegTwist,
                        upperArmTwist = triLibHumanDescription.upperArmTwist,
                        upperLegTwist = triLibHumanDescription.upperLegTwist,
                        skeleton = skeletonBones.Values.ToArray(),
                        human = humanBones
                    };
                    var avatar = AvatarBuilder.BuildHumanAvatar(assetLoaderContext.RootGameObject, humanDescription);
                    avatar.name = $"{assetLoaderContext.RootGameObject.name}Avatar";
                    animator.avatar = avatar;
                }
                assetLoaderContext.RootGameObject.transform.SetParent(parent, false);
                assetLoaderContext.RootGameObject.transform.position = rootGameObjectPosition;
            }
        }

        /// <summary>Converts the given Model into a Game Object.</summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        /// <param name="parentTransform">The parent Game Object Transform.</param>
        /// <param name="rootModel">The root Model.</param>
        /// <param name="model">The Model to convert.</param>
        /// <param name="newGameObject">The Game Object to receive the converted Model.</param>
        private static void ParseModel(AssetLoaderContext assetLoaderContext, Transform parentTransform, IRootModel rootModel, IModel model, out GameObject newGameObject)
        {
            newGameObject = new GameObject(model.Name);
            assetLoaderContext.GameObjects.Add(model, newGameObject);
            assetLoaderContext.Models.Add(newGameObject, model);
            newGameObject.transform.parent = parentTransform;
            newGameObject.transform.localPosition = model.LocalPosition;
            newGameObject.transform.localRotation = model.LocalRotation;
            newGameObject.transform.localScale = model.LocalScale;
            if (model.GeometryGroup != null)
            {
                ParseGeometry(assetLoaderContext, newGameObject, rootModel, model);
            }
            if (model.Children != null && model.Children.Count > 0)
            {
                for (var i = 0; i < model.Children.Count; i++)
                {
                    var child = model.Children[i];
                    ParseModel(assetLoaderContext, newGameObject.transform, rootModel, child, out _);
                }
            }
        }

        /// <summary>Configures the given Model skinning if there is any.</summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        /// <param name="model">The Model containing the bones.</param>
        private static void SetupModelBones(AssetLoaderContext assetLoaderContext, IModel model)
        {
            var loadedGameObject = assetLoaderContext.GameObjects[model];
            var skinnedMeshRenderer = loadedGameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                var bones = model.Bones;
                if (bones != null && bones.Count > 0)
                {
                    var boneIndex = 0;
                    var gameObjectBones = new Transform[bones.Count];
                    for (var i = 0; i < bones.Count; i++)
                    {
                        var bone = bones[i];
                        gameObjectBones[boneIndex++] = assetLoaderContext.GameObjects[bone].transform;
                    }
                    skinnedMeshRenderer.bones = gameObjectBones;
                    skinnedMeshRenderer.rootBone = assetLoaderContext.RootBone;
                }
            }
            if (model.Children != null && model.Children.Count > 0)
            {
                for (var i = 0; i < model.Children.Count; i++)
                {
                    var subModel = model.Children[i];
                    SetupModelBones(assetLoaderContext, subModel);
                }
            }
        }

        /// <summary>Converts the given Animation into an Animation Clip.</summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        /// <param name="animation">The Animation to convert.</param>
        /// <returns>The converted Animation Clip.</returns>
        private static AnimationClip ParseAnimation(AssetLoaderContext assetLoaderContext, IAnimation animation)
        {
            var animationClip = new AnimationClip { name = animation.Name, legacy = assetLoaderContext.Options.AnimationType != AnimationType.Generic, frameRate = animation.FrameRate };
            var animationCurveBindings = animation.AnimationCurveBindings;
            if (animationCurveBindings == null)
            {
                return animationClip;
            }
            var rootModel = assetLoaderContext.RootBone != null ? assetLoaderContext.Models[assetLoaderContext.RootBone.gameObject] : null;
            for (var i = 0; i < animationCurveBindings.Count; i++)
            {
                var animationCurveBinding = animationCurveBindings[i];
                var animationCurves = animationCurveBinding.AnimationCurves;
                var gameObject = assetLoaderContext.GameObjects[animationCurveBinding.Model];
                for (var j = 0; j < animationCurves.Count; j++)
                {
                    var animationCurve = animationCurves[j];
                    var unityAnimationCurve = animationCurve.AnimationCurve;
                    var gameObjectPath = assetLoaderContext.GameObjectPaths[gameObject];
                    var propertyName = animationCurve.Property;
                    var propertyType = animationCurve.Property.StartsWith("blendShape.") ? typeof(SkinnedMeshRenderer) : typeof(Transform); 
                    if (assetLoaderContext.Options.AnimationType == AnimationType.Generic)
                    {
                        TryToRemapGenericCurve(rootModel, animationCurve, animationCurveBinding, ref gameObjectPath, ref propertyName, ref propertyType);
                    }
                    animationClip.SetCurve(gameObjectPath, propertyType, propertyName, unityAnimationCurve);
                }
            }
            if (assetLoaderContext.Options.EnsureQuaternionContinuity)
            {
                animationClip.EnsureQuaternionContinuity();
            }
            return animationClip;
        }

        /// <summary>Tries to convert a legacy Animation Curve path into a generic Animation path.</summary>
        /// <param name="rootBone">The root bone Model.</param>
        /// <param name="animationCurve">The Animation Curve Map to remap.</param>
        /// <param name="animationCurveBinding">The Animation Curve Binding to remap.</param>
        /// <param name="gameObjectPath">The GameObject containing the Curve path.</param>
        /// <param name="propertyName">The remapped Property name.</param>
        /// <param name="propertyType">The remapped Property type.</param>
        private static void TryToRemapGenericCurve(IModel rootBone, IAnimationCurve animationCurve, IAnimationCurveBinding animationCurveBinding, ref string gameObjectPath, ref string propertyName, ref Type propertyType)
        {
            if (animationCurveBinding.Model == rootBone)
            {
                var remap = false;
                switch (animationCurve.Property)
                {
                    case Constants.LocalPositionXProperty:
                        propertyName = Constants.RootPositionXProperty;
                        remap = true;
                        break;
                    case Constants.LocalPositionYProperty:
                        propertyName = Constants.RootPositionYProperty;
                        remap = true;
                        break;
                    case Constants.LocalPositionZProperty:
                        propertyName = Constants.RootPositionZProperty;
                        remap = true;
                        break;
                    case Constants.LocalRotationXProperty:
                        propertyName = Constants.RootRotationXProperty;
                        remap = true;
                        break;
                    case Constants.LocalRotationYProperty:
                        propertyName = Constants.RootRotationYProperty;
                        remap = true;
                        break;
                    case Constants.LocalRotationZProperty:
                        propertyName = Constants.RootRotationZProperty;
                        remap = true;
                        break;
                    case Constants.LocalRotationWProperty:
                        propertyName = Constants.RootRotationWProperty;
                        remap = true;
                        break;
                }
                if (remap)
                {
                    gameObjectPath = "";
                    propertyType = typeof(Animator);
                }
            }
        }

        /// <summary>Converts the given Geometry Group into a Mesh.</summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        /// <param name="meshGameObject">The Game Object where the Mesh belongs.</param>
        /// <param name="rootModel">The root Model.</param>
        /// <param name="meshModel">The Model used to generate the Game Object.</param>
        private static void ParseGeometry(AssetLoaderContext assetLoaderContext, GameObject meshGameObject, IRootModel rootModel, IModel meshModel)
        {
            var geometryGroup = meshModel.GeometryGroup;
            var geometries = geometryGroup.Geometries;
            var mesh = new Mesh { name = geometryGroup.Name, subMeshCount = geometries?.Count ?? 0, indexFormat = assetLoaderContext.Options.IndexFormat };
            assetLoaderContext.Allocations.Add(mesh);
            if (geometries != null)
            {
                if (assetLoaderContext.Options.ReadAndWriteEnabled)
                {
                    mesh.MarkDynamic();
                }
                if (assetLoaderContext.Options.LipSyncMappers != null)
                {
                    for (var i = 0; i < assetLoaderContext.Options.LipSyncMappers.Length; i++)
                    {
                        var lipSyncMapper = assetLoaderContext.Options.LipSyncMappers[i];
                        if (lipSyncMapper == null)
                        {
                            continue;
                        }

                        if (lipSyncMapper.Map(assetLoaderContext, geometryGroup, out var visemeToBlendTargets))
                        {
                            var lipSyncMapping = meshGameObject.AddComponent<LipSyncMapping>();
                            lipSyncMapping.VisemeToBlendTargets = visemeToBlendTargets;
                            break;
                        }
                    }
                }
                geometries = mesh.CombineGeometries(geometryGroup, meshModel.BindPoses, assetLoaderContext);
                if (assetLoaderContext.Options.GenerateColliders)
                {
                    if (assetLoaderContext.RootModel.AllAnimations != null && assetLoaderContext.RootModel.AllAnimations.Count > 0)
                    {
                        if (assetLoaderContext.Options.ShowLoadingWarnings)
                        {
                            Debug.LogWarning("Trying to add a MeshCollider to an animated object.");
                        }
                    }
                    else
                    {
                        var meshCollider = meshGameObject.AddComponent<MeshCollider>();
                        meshCollider.sharedMesh = mesh;
                        meshCollider.convex = assetLoaderContext.Options.ConvexColliders;
                    }
                }
                Renderer renderer = null;
                if (assetLoaderContext.Options.AnimationType != AnimationType.None || assetLoaderContext.Options.ImportBlendShapes)
                {
                    var bones = meshModel.Bones;
                    var geometryGroupBlendShapeGeometryBindings = geometryGroup.BlendShapeGeometryBindings;
                    if (bones != null && bones.Count > 0 || geometryGroupBlendShapeGeometryBindings != null && geometryGroupBlendShapeGeometryBindings.Count > 0)
                    {
                        var skinnedMeshRenderer = meshGameObject.AddComponent<SkinnedMeshRenderer>();
                        skinnedMeshRenderer.sharedMesh = mesh;
                        skinnedMeshRenderer.enabled = !assetLoaderContext.Options.ImportVisibility || meshModel.Visibility;
                        renderer = skinnedMeshRenderer;
                    }
                }
                if (renderer == null)
                {
                    var meshFilter = meshGameObject.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = mesh;
                    var meshRenderer = meshGameObject.AddComponent<MeshRenderer>();
                    meshRenderer.enabled = !assetLoaderContext.Options.ImportVisibility || meshModel.Visibility;
                    renderer = meshRenderer;
                }
                Material loadingMaterial = null;
                if (assetLoaderContext.Options.MaterialMappers != null)
                {
                    for (var i = 0; i < assetLoaderContext.Options.MaterialMappers.Length; i++)
                    {
                        var mapper = assetLoaderContext.Options.MaterialMappers[i];
                        if (mapper != null && mapper.IsCompatible(null))
                        {
                            loadingMaterial = mapper.LoadingMaterial;
                            break;
                        }
                    }
                }
                var unityMaterials = new Material[geometries.Count];
                if (loadingMaterial == null)
                {
                    if (assetLoaderContext.Options.ShowLoadingWarnings)
                    {
                        Debug.LogWarning("Could not find a suitable loading Material.");
                    }
                }
                else
                {
                    for (var i = 0; i < unityMaterials.Length; i++)
                    {
                        unityMaterials[i] = loadingMaterial;
                    }
                }
                renderer.sharedMaterials = unityMaterials;
                var materialIndices = meshModel.MaterialIndices;
                for (var i = 0; i < unityMaterials.Length; i++)
                {
                    if (i >= materialIndices.Count)
                    {
                        continue;
                    }
                    var materialIndex = materialIndices[i];
                    if (materialIndex < 0 || materialIndex >= rootModel.AllMaterials.Count)
                    {
                        continue;
                    }
                    var sourceMaterial = rootModel.AllMaterials[materialIndex];
                    if (sourceMaterial == null)
                    {
                        continue;
                    }
                    var geometryIndex = geometries[i].Index;
                    if (geometryIndex < 0 || geometryIndex >= renderer.sharedMaterials.Length)
                    {
                        continue;
                    }
                    var materialRenderersContext = new MaterialRendererContext
                    {
                        Context = assetLoaderContext,
                        Renderer = renderer,
                        GeometryIndex = geometryIndex,
                        Material = sourceMaterial
                    };
                    if (assetLoaderContext.MaterialRenderers.TryGetValue(sourceMaterial, out var materialRendererContextList))
                    {
                        materialRendererContextList.Add(materialRenderersContext);
                    }
                    else
                    {
                        assetLoaderContext.MaterialRenderers.Add(sourceMaterial, new List<MaterialRendererContext> { materialRenderersContext });
                    }
                }
            }
        }

        /// <summary>Loads the root Model.</summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        private static void LoadModel(AssetLoaderContext assetLoaderContext)
        {
            if (assetLoaderContext.Stream == null && string.IsNullOrWhiteSpace(assetLoaderContext.Filename))
            {
                throw new Exception("TriLib is unable to load the given file.");
            }
            if (assetLoaderContext.Options.MaterialMappers != null)
            {
                Array.Sort(assetLoaderContext.Options.MaterialMappers, (a, b) => a.CheckingOrder > b.CheckingOrder ? -1 : 1);
            }
            else
            {
                if (assetLoaderContext.Options.ShowLoadingWarnings)
                {
                    Debug.LogWarning("Your AssetLoaderOptions instance has no MaterialMappers. TriLib can't process materials without them.");
                }
            }
#if TRILIB_DRACO
            GltfReader.DracoDecompressorCallback = DracoMeshLoader.DracoDecompressorCallback;
#endif
            var fileExtension = assetLoaderContext.FileExtension ?? FileUtils.GetFileExtension(assetLoaderContext.Filename, false);
            if (assetLoaderContext.Stream == null)
            {
                var fileStream = new FileStream(assetLoaderContext.Filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                assetLoaderContext.Stream = fileStream;
                var reader = Readers.FindReaderForExtension(fileExtension);
                if (reader != null)
                {
                    reader.ExtraStepsCount = ExtraStepsCount;
                    assetLoaderContext.RootModel = reader.ReadStream(fileStream, assetLoaderContext, assetLoaderContext.Filename, assetLoaderContext.OnProgress);
                }
            }
            else
            {
                var reader = Readers.FindReaderForExtension(fileExtension);
                if (reader != null)
                {
                    reader.ExtraStepsCount = ExtraStepsCount;
                    assetLoaderContext.RootModel = reader.ReadStream(assetLoaderContext.Stream, assetLoaderContext, assetLoaderContext.Filename, assetLoaderContext.OnProgress);
                }
                else
                {
                    throw new Exception("Could not find a suitable reader for the given model. Please fill the 'fileExtension' parameter when calling any model loading method.");
                }
            }
        }

        /// <summary>Processes the root Model.</summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        private static void ProcessRootModel(AssetLoaderContext assetLoaderContext)
        {
            ProcessModel(assetLoaderContext);
            if (assetLoaderContext.Async)
            {
                ThreadUtils.RunThread(assetLoaderContext, ref assetLoaderContext.CancellationToken, ProcessTextures, null, assetLoaderContext.HandleError ?? assetLoaderContext.OnError, assetLoaderContext.Options.Timeout);
            }
            else
            {
                ProcessTextures(assetLoaderContext);
            }
        }

        /// <summary>
        /// Processes the Model Materials, if all source Materials have been loaded.
        /// </summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        private static void TryProcessMaterials(AssetLoaderContext assetLoaderContext)
        {
            var allMaterialsLoaded = assetLoaderContext.RootModel?.AllTextures == null || assetLoaderContext.LoadedTexturesCount == assetLoaderContext.RootModel.AllTextures.Count;
            if (!assetLoaderContext.MaterialsProcessed && allMaterialsLoaded)
            {
                assetLoaderContext.MaterialsProcessed = true;
                if (assetLoaderContext.RootModel?.AllMaterials != null)
                {
                    ProcessMaterialRenderers(assetLoaderContext);
                }
                if (assetLoaderContext.Options.AddAssetUnloader && assetLoaderContext.RootGameObject != null || assetLoaderContext.WrapperGameObject != null)
                {
                    var gameObject = assetLoaderContext.RootGameObject ?? assetLoaderContext.WrapperGameObject;
                    var assetUnloader = gameObject.AddComponent<AssetUnloader>();
                    assetUnloader.Id = AssetUnloader.GetNextId();
                    assetUnloader.Allocations = assetLoaderContext.Allocations;
                }
                assetLoaderContext.Reader.UpdateLoadingPercentage(1f, assetLoaderContext.Reader.LoadingStepsCount + FinalStep);
                assetLoaderContext.OnMaterialsLoad?.Invoke(assetLoaderContext);
                TryToCloseStream(assetLoaderContext);
                GCHelper.Instance.Deactivate();
            }
        }

        /// <summary>
        /// Processes the Model Textures.
        /// </summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        private static void ProcessTextures(AssetLoaderContext assetLoaderContext)
        {
            if (assetLoaderContext.RootModel?.AllTextures != null)
            {
                for (var i = 0; i < assetLoaderContext.RootModel.AllTextures.Count; i++)
                {
                    var texture = assetLoaderContext.RootModel.AllTextures[i];
                    TextureLoadingContext textureLoadingContext = null;
                    if (assetLoaderContext.Options.TextureMapper != null)
                    {
                        textureLoadingContext = assetLoaderContext.Options.TextureMapper.Map(assetLoaderContext, texture);
                    }
                    if (textureLoadingContext == null)
                    {
                        textureLoadingContext = new TextureLoadingContext
                        {
                            Context = assetLoaderContext,
                            Texture = texture
                        };
                    }
                    StbImage.LoadTexture(textureLoadingContext);
                    assetLoaderContext.Reader.UpdateLoadingPercentage(i, assetLoaderContext.Reader.LoadingStepsCount + ProcessingTexturesStep, assetLoaderContext.RootModel.AllTextures.Count);
                }
            }
            if (assetLoaderContext.Async)
            {
                Dispatcher.InvokeAsync(new ContextualizedAction<AssetLoaderContext>(TryProcessMaterials, assetLoaderContext));
            }
            else
            {
                TryProcessMaterials(assetLoaderContext);
            }
        }

        /// <summary>
        /// Processes the Model Renderer Materials.
        /// </summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        private static void ProcessMaterialRenderers(AssetLoaderContext assetLoaderContext)
        {
            if (assetLoaderContext.Options.MaterialMappers != null)
            {
                for (var i = 0; i < assetLoaderContext.RootModel.AllMaterials.Count; i++)
                {
                    var material = assetLoaderContext.RootModel.AllMaterials[i];
                    MaterialMapper usedMaterialMapper = null;
                    var materialMapperContext = new MaterialMapperContext
                    {
                        Context = assetLoaderContext,
                        Material = material
                    };
                    for (var j = 0; j < assetLoaderContext.Options.MaterialMappers.Length; j++)
                    {
                        var materialMapper = assetLoaderContext.Options.MaterialMappers[j];
                        if (materialMapper == null || !materialMapper.IsCompatible(materialMapperContext))
                        {
                            continue;
                        }

                        materialMapper.Map(materialMapperContext);
                        usedMaterialMapper = materialMapper;
                        break;
                    }

                    if (usedMaterialMapper != null)
                    {
                        if (assetLoaderContext.MaterialRenderers.TryGetValue(material, out var materialRendererList))
                        {
                            for (var j = 0; j < materialRendererList.Count; j++)
                            {
                                var materialRendererContext = materialRendererList[j];
                                usedMaterialMapper.ApplyMaterialToRenderer(materialRendererContext, materialMapperContext);
                            }
                        }
                    }
                    assetLoaderContext.Reader.UpdateLoadingPercentage(i, assetLoaderContext.Reader.LoadingStepsCount + ProcessingRenderersStep, assetLoaderContext.RootModel.AllMaterials.Count);
                }
            }
            else if (assetLoaderContext.Options.ShowLoadingWarnings)
            {
                Debug.LogWarning("The given AssetLoaderOptions contains no MaterialMapper. Materials will not be created.");
            }
        }

        /// <summary>Handles all Model loading errors, unloads the partially loaded Model (if suitable), and calls the error callback (if existing).</summary>
        /// <param name="error">The Contextualized Error that has occurred.</param>
        private static void HandleError(IContextualizedError error)
        {
            var exception = error.GetInnerException();
            if (error.GetContext() is IAssetLoaderContext iassetLoaderContext)
            {
                var assetLoaderContext = iassetLoaderContext.Context;
                if (assetLoaderContext != null)
                {
                    TryToCloseStream(assetLoaderContext);
                    if (assetLoaderContext.Options.DestroyOnError && assetLoaderContext.RootGameObject != null)
                    {
#if UNITY_EDITOR
                        Object.DestroyImmediate(assetLoaderContext.RootGameObject);
#else
                        Object.Destroy(assetLoaderContext.RootGameObject);
#endif
                        assetLoaderContext.RootGameObject = null;
                    }
                    if (assetLoaderContext.Async)
                    {
                        Dispatcher.InvokeAsync(new ContextualizedAction<IContextualizedError>(assetLoaderContext.OnError, error));
                    }
                    else if (assetLoaderContext.OnError != null)
                    {
                        assetLoaderContext.OnError(error);
                    }
                }
            }
            else
            {
                var contextualizedAction = new ContextualizedAction<ContextualizedError<object>>(Rethrow, new ContextualizedError<object>(exception, null));
                Dispatcher.InvokeAsync(contextualizedAction);
            }
        }

        /// <summary>
        /// Tries to close the Model Stream, if the used AssetLoaderOptions.CloseSteamAutomatically option is enabled.
        /// </summary>
        /// <param name="assetLoaderContext">The Asset Loader Context reference. Asset Loader Context contains the Model loading data.</param>
        private static void TryToCloseStream(AssetLoaderContext assetLoaderContext)
        {
            if (assetLoaderContext.Stream != null && assetLoaderContext.Options.CloseStreamAutomatically)
            {
                try
                {
                    assetLoaderContext.Stream.Dispose();
                }
                catch (Exception e)
                {

                }
            }
        }

        /// <summary>Throws the given Contextualized Error on the main Thread.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contextualizedError">The Contextualized Error to throw.</param>
        private static void Rethrow<T>(ContextualizedError<T> contextualizedError)
        {
            throw contextualizedError;
        }
    }
}
