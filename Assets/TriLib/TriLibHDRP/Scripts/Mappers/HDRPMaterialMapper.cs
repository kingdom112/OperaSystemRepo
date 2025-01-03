﻿using System;
using TriLibCore.General;
using TriLibCore.Mappers;
using TriLibCore.Utils;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace TriLibCore.HDRP.Mappers
{
    /// <summary>Represents a Material Mapper that converts TriLib Materials into Unity HDRP Materials.</summary>
    [Serializable]
    [CreateAssetMenu(menuName = "TriLib/Mappers/Material/HDRP Material Mapper", fileName = "HDRPMaterialMapper")]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class HDRPMaterialMapper : MaterialMapper
    {
        static HDRPMaterialMapper()
        {
            AddToRegisteredMappers();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void AddToRegisteredMappers()
        {
            if (RegisteredMappers.Contains("HDRPMaterialMapper"))
            {
                return;
            }
            RegisteredMappers.Add("HDRPMaterialMapper");
        }

        public override Material MaterialPreset => Resources.Load<Material>("Materials/HDRP/TriLibHDRP");

        public override Material AlphaMaterialPreset => Resources.Load<Material>("Materials/HDRP/TriLibHDRPAlphaCutout");

        public override Material AlphaMaterialPreset2 => Resources.Load<Material>("Materials/HDRP/TriLibHDRPAlpha");

        public override Material SpecularMaterialPreset => Resources.Load<Material>("Materials/HDRP/TriLibHDRP");

        public override Material SpecularAlphaMaterialPreset => Resources.Load<Material>("Materials/HDRP/TriLibHDRPAlphaCutout");

        public override Material SpecularAlphaMaterialPreset2 => Resources.Load<Material>("Materials/HDRP/TriLibHDRPAlpha");

        public override Material LoadingMaterial => Resources.Load<Material>("Materials/HDRP/TriLibHDRPLoading");

        ///<inheritdoc />
        public override bool IsCompatible(MaterialMapperContext materialMapperContext)
        {
            return TriLibSettings.GetBool("HDRPMaterialMapper");
        }

        ///<inheritdoc />
        public override void Map(MaterialMapperContext materialMapperContext)
        {
            materialMapperContext.VirtualMaterial = new HDRPVirtualMaterial();
            CheckDiffuseMapTexture(materialMapperContext);
        }

        private void CheckDiffuseMapTexture(MaterialMapperContext materialMapperContext)
        {
            var diffuseTexturePropertyName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.DiffuseTexture);
            if (materialMapperContext.Material.HasProperty(diffuseTexturePropertyName))
            {
                LoadTexture(materialMapperContext, TextureType.Diffuse, materialMapperContext.Material.GetTextureValue(diffuseTexturePropertyName), ApplyDiffuseMapTexture);
            }
            else
            {
                ApplyDiffuseMapTexture(materialMapperContext, TextureType.Diffuse, null);
            }
        }

        private void ApplyDiffuseMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            materialMapperContext.VirtualMaterial.SetProperty("_BaseColorMap", texture);
            CheckGlossinessValue(materialMapperContext);
        }

        private void CheckGlossinessValue(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericPropertyValueMultiplied(GenericMaterialProperty.Glossiness, materialMapperContext.Material.GetGenericFloatValue(GenericMaterialProperty.Glossiness));
            materialMapperContext.VirtualMaterial.SetProperty("_Smoothness", value);
            CheckMetallicValue(materialMapperContext);
        }

        private void CheckMetallicValue(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericFloatValue(GenericMaterialProperty.Metallic);
            materialMapperContext.VirtualMaterial.SetProperty("_Metallic", value);
            CheckEmissionMapTexture(materialMapperContext);
        }

        private void CheckEmissionMapTexture(MaterialMapperContext materialMapperContext)
        {
            var emissionTexturePropertyName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.EmissionTexture);
            if (materialMapperContext.Material.HasProperty(emissionTexturePropertyName))
            {
                LoadTexture(materialMapperContext, TextureType.Emission, materialMapperContext.Material.GetTextureValue(emissionTexturePropertyName), ApplyEmissionMapTexture);
            }
            else
            {
                ApplyEmissionMapTexture(materialMapperContext, TextureType.Emission, null);
            }
        }

        private void ApplyEmissionMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            materialMapperContext.VirtualMaterial.SetProperty("_EmissiveColorMap", texture);
            if (texture)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_EMISSIVE_COLOR_MAP");
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                materialMapperContext.VirtualMaterial.SetProperty("_EmissiveIntensity", materialMapperContext.Material.GetGenericPropertyValueMultiplied(GenericMaterialProperty.EmissionColor, 1f));
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_EMISSIVE_COLOR_MAP");
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
            CheckNormalMapTexture(materialMapperContext);
        }

        private void CheckNormalMapTexture(MaterialMapperContext materialMapperContext)
        {
            var normalMapTexturePropertyName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.NormalTexture);
            if (materialMapperContext.Material.HasProperty(normalMapTexturePropertyName))
            {
                LoadTexture(materialMapperContext, TextureType.NormalMap, materialMapperContext.Material.GetTextureValue(normalMapTexturePropertyName), ApplyNormalMapTexture);
            }
            else
            {
                ApplyNormalMapTexture(materialMapperContext, TextureType.NormalMap, null);
            }
        }

        private void ApplyNormalMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            materialMapperContext.VirtualMaterial.SetProperty("_NormalMap", texture);
            if (texture != null)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_NORMALMAP");
                materialMapperContext.VirtualMaterial.EnableKeyword("_NORMALMAP_TANGENT_SPACE");
                materialMapperContext.VirtualMaterial.SetProperty("_NormalScale", materialMapperContext.Material.GetGenericPropertyValueMultiplied(GenericMaterialProperty.NormalTexture, 1f));
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_NORMALMAP");
                materialMapperContext.VirtualMaterial.DisableKeyword("_NORMALMAP_TANGENT_SPACE");
            }
            CheckSpecularTexture(materialMapperContext);
        }

        private void CheckSpecularTexture(MaterialMapperContext materialMapperContext)
        {
            var specularTexturePropertyName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.SpecularTexture);
            if (materialMapperContext.Material.HasProperty(specularTexturePropertyName))
            {
                LoadTexture(materialMapperContext, TextureType.Specular, materialMapperContext.Material.GetTextureValue(specularTexturePropertyName), ApplySpecGlossMapTexture);
            }
            else
            {
                ApplySpecGlossMapTexture(materialMapperContext, TextureType.Specular, null);
            }
        }

        private void ApplySpecGlossMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            ((HDRPVirtualMaterial)materialMapperContext.VirtualMaterial).SmoothnessTexture = texture;
            CheckOcclusionMapTexture(materialMapperContext);
        }

        private void CheckOcclusionMapTexture(MaterialMapperContext materialMapperContext)
        {
            var occlusionMapTextureName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.OcclusionTexture);
            if (materialMapperContext.Material.HasProperty(occlusionMapTextureName))
            {
                LoadTexture(materialMapperContext, TextureType.Occlusion, materialMapperContext.Material.GetTextureValue(occlusionMapTextureName), ApplyOcclusionMapTexture);
            }
            else
            {
                ApplyOcclusionMapTexture(materialMapperContext, TextureType.Occlusion, null);
            }
        }

        private void ApplyOcclusionMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            ((HDRPVirtualMaterial)materialMapperContext.VirtualMaterial).OcclusionTexture = texture;
            CheckParallaxMapTexture(materialMapperContext);
        }

        private void CheckParallaxMapTexture(MaterialMapperContext materialMapperContext)
        {
            var parallaxMapTextureName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.ParallaxMap);
            if (materialMapperContext.Material.HasProperty(parallaxMapTextureName))
            {
                LoadTexture(materialMapperContext, TextureType.Parallax, materialMapperContext.Material.GetTextureValue(parallaxMapTextureName), ApplyParallaxMapTexture);
            }
            else
            {
                ApplyParallaxMapTexture(materialMapperContext, TextureType.Parallax, null);
            }
        }

        private void ApplyParallaxMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            CheckMetallicGlossMapTexture(materialMapperContext);
        }

        private void CheckMetallicGlossMapTexture(MaterialMapperContext materialMapperContext)
        {
            var metallicGlossMapTextureName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.MetallicGlossMap);
            if (materialMapperContext.Material.HasProperty(metallicGlossMapTextureName))
            {
                LoadTexture(materialMapperContext, TextureType.Metalness, materialMapperContext.Material.GetTextureValue(metallicGlossMapTextureName), ApplyMetallicGlossMapTexture);
            }
            else
            {
                ApplyMetallicGlossMapTexture(materialMapperContext, TextureType.Metalness, null);
            }
        }

        private void ApplyMetallicGlossMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            ((HDRPVirtualMaterial)materialMapperContext.VirtualMaterial).MetallicTexture = texture;
            CheckEmissionColor(materialMapperContext);
        }

        private void CheckEmissionColor(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericColorValue(GenericMaterialProperty.EmissionColor);
            materialMapperContext.VirtualMaterial.SetProperty("_EmissiveColor", value);
            if (value != Color.black)
            {
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                materialMapperContext.VirtualMaterial.SetProperty("_EmissiveIntensity", materialMapperContext.Material.GetGenericPropertyValueMultiplied(GenericMaterialProperty.EmissionColor, 1f));
            }
            else
            {
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
            CheckDiffuseColor(materialMapperContext);
        }

        private void CheckDiffuseColor(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericColorValue(GenericMaterialProperty.DiffuseColor) * materialMapperContext.Material.GetGenericPropertyValueMultiplied(GenericMaterialProperty.DiffuseColor, 1f);
            value.a *= materialMapperContext.Material.GetGenericFloatValue(GenericMaterialProperty.AlphaValue);
            if (!materialMapperContext.VirtualMaterial.HasAlpha && value.a < 1f)
            {
                materialMapperContext.VirtualMaterial.HasAlpha = true;
            }
            materialMapperContext.VirtualMaterial.SetProperty("_BaseColor", value);
            BuildMaterial(materialMapperContext);
            BuildHDRPMask(materialMapperContext);
        }

        private void BuildHDRPMask(MaterialMapperContext materialMapperContext)
        {
            if (materialMapperContext.UnityMaterial == null)
            {
                return;
            }
            var hdrpVirtualMaterial = (HDRPVirtualMaterial)materialMapperContext.VirtualMaterial;
            var maskBaseTexture = hdrpVirtualMaterial.MetallicTexture ?? hdrpVirtualMaterial.OcclusionTexture ?? hdrpVirtualMaterial.DetailMaskTexture ?? hdrpVirtualMaterial.SmoothnessTexture;
            if (maskBaseTexture == null)
            {
                materialMapperContext.UnityMaterial.DisableKeyword("_MASKMAP");
                return;
            }
            var material = new Material(Shader.Find("Hidden/TriLib/BuildHDRPMask"));
            material.SetTexture("_MetallicTex", hdrpVirtualMaterial.MetallicTexture);
            material.SetTexture("_OcclusionTex", hdrpVirtualMaterial.OcclusionTexture);
            material.SetTexture("_DetailMaskTex", hdrpVirtualMaterial.DetailMaskTexture);
            material.SetTexture("_SmoothnessTex", hdrpVirtualMaterial.SmoothnessTexture);
            var graphicsFormat = materialMapperContext.Context.Options.Enforce16BitsTextures ? GraphicsFormat.R16G16B16A16_SFloat : GraphicsFormat.R8G8B8A8_SRGB;
            var renderTexture = RenderTexture.GetTemporary(maskBaseTexture.width, maskBaseTexture.height, 0, graphicsFormat);
            renderTexture.useMipMap = true;
            renderTexture.autoGenerateMips = false;
            Graphics.Blit(null, renderTexture, material);
            var texture2D = new Texture2D(
                maskBaseTexture.width,
                maskBaseTexture.height,
                graphicsFormat,
                materialMapperContext.Context.Options.GenerateMipmaps ? TextureCreationFlags.MipChain : TextureCreationFlags.None)
            {
                name = "Mask"
            };
            materialMapperContext.Context.Allocations.Add(texture2D);
            Graphics.CopyTexture(renderTexture, texture2D);
            materialMapperContext.UnityMaterial.EnableKeyword("_MASKMAP");
            materialMapperContext.UnityMaterial.SetTexture("_MaskMap", texture2D);
            Graphics.SetRenderTarget(null);
            renderTexture.Release();
            if (Application.isPlaying)
            {
                Destroy(material);
            }
            else
            {
                DestroyImmediate(material);
            }
        }
    }
}