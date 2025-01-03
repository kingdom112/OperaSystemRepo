﻿//#define RTSL_COMPILE_TEMPLATES
#if RTSL_COMPILE_TEMPLATES
//<TEMPLATE_USINGS_START>
using ProtoBuf;
using UnityEngine;
//<TEMPLATE_USINGS_END>
#else
using UnityEngine;
#endif

namespace Battlehub.RTSL.Internal
{
    using PersistentParticleSystemNestedColorBySpeedModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedColorOverLifetimeModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedForceOverLifetimeModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedInheritVelocityModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedLightsModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedLimitVelocityOverLifetimeModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedMainModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedNoiseModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedRotationBySpeedModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedRotationOverLifetimeModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedShapeModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedSizeBySpeedModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedSizeOverLifetimeModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedTrailModule = PersistentSurrogateTemplate;
    using PersistentParticleSystemNestedVelocityOverLifetimeModule = PersistentSurrogateTemplate;

    [PersistentTemplate("UnityEngine.ParticleSystem", new string[0],
        new [] {
           "UnityEngine.ParticleSystem+CollisionModule",
           "UnityEngine.ParticleSystem+ColorBySpeedModule",
           "UnityEngine.ParticleSystem+ColorOverLifetimeModule",
           "UnityEngine.ParticleSystem+CustomDataModule",
           "UnityEngine.ParticleSystem+EmissionModule",
           "UnityEngine.ParticleSystem+ExternalForcesModule",
           "UnityEngine.ParticleSystem+ForceOverLifetimeModule",
           "UnityEngine.ParticleSystem+InheritVelocityModule",
           "UnityEngine.ParticleSystem+LightsModule",
           "UnityEngine.ParticleSystem+LimitVelocityOverLifetimeModule",
           "UnityEngine.ParticleSystem+MainModule",
           "UnityEngine.ParticleSystem+NoiseModule",
           "UnityEngine.ParticleSystem+RotationBySpeedModule",
           "UnityEngine.ParticleSystem+RotationOverLifetimeModule",
           "UnityEngine.ParticleSystem+ShapeModule",
           "UnityEngine.ParticleSystem+SizeBySpeedModule",
           "UnityEngine.ParticleSystem+SizeOverLifetimeModule",
           "UnityEngine.ParticleSystem+SubEmittersModule",
           "UnityEngine.ParticleSystem+TextureSheetAnimationModule",
           "UnityEngine.ParticleSystem+TrailModule",
           "UnityEngine.ParticleSystem+TriggerModule",
           "UnityEngine.ParticleSystem+VelocityOverLifetimeModule",
     })]

    public partial class PersistentParticleSystem_RTSL_Template : PersistentSurrogateTemplate
    {
#if RTSL_COMPILE_TEMPLATES
        //<TEMPLATE_BODY_START>
        [ProtoMember(1)]
        public PersistentParticleSystemNestedCollisionModule_RTSL_Template m_collisionModule;

        [ProtoMember(2)]
        public PersistentParticleSystemNestedColorBySpeedModule m_colorBySpeedModule;

        [ProtoMember(3)]
        public PersistentParticleSystemNestedColorOverLifetimeModule m_colorOverLifetimeModule;

        [ProtoMember(4)]
        public PersistentParticleSystemNestedCustomDataModule_RTSL_Template m_customDataModule;

        [ProtoMember(5)]
        public PersistentParticleSystemNestedEmissionModule_RTSL_Template m_emissonModule;

        [ProtoMember(6)]
        public PersistentParticleSystemNestedExternalForcesModule_RTSL_Template m_externalForcesModule;

        [ProtoMember(7)]
        public PersistentParticleSystemNestedForceOverLifetimeModule m_forceOverlifeTimeModule;

        [ProtoMember(8)]
        public PersistentParticleSystemNestedInheritVelocityModule m_inheritVelocityModule;

        [ProtoMember(9)]
        public PersistentParticleSystemNestedLightsModule m_lightsModule;

        [ProtoMember(10)]
        public PersistentParticleSystemNestedLimitVelocityOverLifetimeModule m_limitVelocityOverLifetimeModule;

        [ProtoMember(11)]
        public PersistentParticleSystemNestedMainModule m_mainModule;

        [ProtoMember(12)]
        public PersistentParticleSystemNestedNoiseModule m_noiseModule;

        [ProtoMember(13)]
        public PersistentParticleSystemNestedRotationBySpeedModule m_rotationBySpeedModule;

        [ProtoMember(14)]
        public PersistentParticleSystemNestedRotationOverLifetimeModule m_rotationOverlifetimeModule;

        [ProtoMember(15)]
        public PersistentParticleSystemNestedShapeModule m_shapeModule;

        [ProtoMember(16)]
        public PersistentParticleSystemNestedSizeBySpeedModule m_sizeBySpeedModule;

        [ProtoMember(17)]
        public PersistentParticleSystemNestedSizeOverLifetimeModule m_sizeOverlifeTimeModule;

        [ProtoMember(18)]
        public PersistentParticleSystemNestedSubEmittersModule_RTSL_Template m_subEmittersModule;

        [ProtoMember(19)]
        public PersistentParticleSystemNestedTextureSheetAnimationModule_RTSL_Template m_textureSheetAnimationModule;

        [ProtoMember(20)]
        public PersistentParticleSystemNestedTrailModule m_trialModule;

        [ProtoMember(21)]
        public PersistentParticleSystemNestedTriggerModule_RTSL_Template m_triggerModule;

        [ProtoMember(22)]
        public PersistentParticleSystemNestedVelocityOverLifetimeModule m_velocityOverLifetimeModule;

        public override object WriteTo(object obj)
        {
            obj = base.WriteTo(obj);
            if (obj == null)
            {
                return null;
            }

            ParticleSystem o = (ParticleSystem)obj;
            WriteSurrogateTo(m_collisionModule, o.collision);
            WriteSurrogateTo(m_colorBySpeedModule, o.colorBySpeed);
            WriteSurrogateTo(m_colorOverLifetimeModule, o.colorOverLifetime);
            WriteSurrogateTo(m_customDataModule, o.customData);
            WriteSurrogateTo(m_emissonModule, o.emission);
            WriteSurrogateTo(m_externalForcesModule, o.externalForces);
            WriteSurrogateTo(m_forceOverlifeTimeModule, o.forceOverLifetime);
            WriteSurrogateTo(m_inheritVelocityModule, o.inheritVelocity);
            WriteSurrogateTo(m_lightsModule, o.lights);
            WriteSurrogateTo(m_limitVelocityOverLifetimeModule, o.limitVelocityOverLifetime);
            WriteSurrogateTo(m_mainModule, o.main);
            WriteSurrogateTo(m_noiseModule, o.noise);
            WriteSurrogateTo(m_rotationBySpeedModule, o.rotationBySpeed);
            WriteSurrogateTo(m_rotationOverlifetimeModule, o.rotationOverLifetime);
            WriteSurrogateTo(m_shapeModule, o.shape);
            WriteSurrogateTo(m_sizeBySpeedModule, o.sizeBySpeed);
            WriteSurrogateTo(m_sizeOverlifeTimeModule, o.sizeOverLifetime);
            WriteSurrogateTo(m_subEmittersModule, o.subEmitters);
            WriteSurrogateTo(m_textureSheetAnimationModule, o.textureSheetAnimation);
            WriteSurrogateTo(m_trialModule, o.trails);
            WriteSurrogateTo(m_triggerModule, o.trigger);
            WriteSurrogateTo(m_velocityOverLifetimeModule, o.velocityOverLifetime);
            return obj;
        }

        public override void ReadFrom(object obj)
        {
            base.ReadFrom(obj);
            if (obj == null)
            {
                return;
            }

            ParticleSystem o = (ParticleSystem)obj;
            m_collisionModule = ReadSurrogateFrom<PersistentParticleSystemNestedCollisionModule_RTSL_Template>(o.collision);
            m_colorBySpeedModule = ReadSurrogateFrom<PersistentParticleSystemNestedColorBySpeedModule>(o.colorBySpeed);
            m_colorOverLifetimeModule = ReadSurrogateFrom<PersistentParticleSystemNestedColorOverLifetimeModule>(o.colorOverLifetime);
            m_customDataModule = ReadSurrogateFrom<PersistentParticleSystemNestedCustomDataModule_RTSL_Template>(o.customData);
            m_emissonModule = ReadSurrogateFrom<PersistentParticleSystemNestedEmissionModule_RTSL_Template>(o.emission);
            m_externalForcesModule = ReadSurrogateFrom<PersistentParticleSystemNestedExternalForcesModule_RTSL_Template>(o.externalForces);
            m_forceOverlifeTimeModule = ReadSurrogateFrom<PersistentParticleSystemNestedForceOverLifetimeModule>(o.forceOverLifetime);
            m_inheritVelocityModule = ReadSurrogateFrom<PersistentParticleSystemNestedInheritVelocityModule>(o.inheritVelocity);
            m_lightsModule = ReadSurrogateFrom<PersistentParticleSystemNestedLightsModule>(o.lights);
            m_limitVelocityOverLifetimeModule = ReadSurrogateFrom<PersistentParticleSystemNestedLimitVelocityOverLifetimeModule>(o.limitVelocityOverLifetime);
            m_mainModule = ReadSurrogateFrom<PersistentParticleSystemNestedMainModule>(o.main);
            m_noiseModule = ReadSurrogateFrom<PersistentParticleSystemNestedNoiseModule>(o.noise);
            m_rotationBySpeedModule = ReadSurrogateFrom<PersistentParticleSystemNestedRotationBySpeedModule>(o.rotationBySpeed);
            m_rotationOverlifetimeModule = ReadSurrogateFrom<PersistentParticleSystemNestedRotationOverLifetimeModule>(o.rotationOverLifetime);
            m_shapeModule = ReadSurrogateFrom<PersistentParticleSystemNestedShapeModule>(o.shape);
            m_sizeBySpeedModule = ReadSurrogateFrom<PersistentParticleSystemNestedSizeBySpeedModule>(o.sizeBySpeed);
            m_sizeOverlifeTimeModule = ReadSurrogateFrom<PersistentParticleSystemNestedSizeOverLifetimeModule>(o.sizeOverLifetime);
            m_subEmittersModule = ReadSurrogateFrom<PersistentParticleSystemNestedSubEmittersModule_RTSL_Template>(o.subEmitters);
            m_textureSheetAnimationModule = ReadSurrogateFrom<PersistentParticleSystemNestedTextureSheetAnimationModule_RTSL_Template>(o.textureSheetAnimation);
            m_trialModule = ReadSurrogateFrom<PersistentParticleSystemNestedTrailModule>(o.trails);
            m_triggerModule = ReadSurrogateFrom<PersistentParticleSystemNestedTriggerModule_RTSL_Template>(o.trigger);
            m_velocityOverLifetimeModule = ReadSurrogateFrom<PersistentParticleSystemNestedVelocityOverLifetimeModule>(o.velocityOverLifetime);
        }

        public override void GetDeps(GetDepsContext context)
        {
            base.GetDeps(context);
            AddSurrogateDeps(m_collisionModule, context);
            AddSurrogateDeps(m_colorBySpeedModule, context);
            AddSurrogateDeps(m_colorOverLifetimeModule, context);
            AddSurrogateDeps(m_customDataModule, context);
            AddSurrogateDeps(m_emissonModule, context);
            AddSurrogateDeps(m_externalForcesModule, context);
            AddSurrogateDeps(m_forceOverlifeTimeModule, context);
            AddSurrogateDeps(m_inheritVelocityModule, context);
            AddSurrogateDeps(m_lightsModule, context);
            AddSurrogateDeps(m_limitVelocityOverLifetimeModule, context);
            AddSurrogateDeps(m_mainModule, context);
            AddSurrogateDeps(m_noiseModule, context);
            AddSurrogateDeps(m_rotationBySpeedModule, context);
            AddSurrogateDeps(m_rotationOverlifetimeModule, context);
            AddSurrogateDeps(m_shapeModule, context);
            AddSurrogateDeps(m_sizeBySpeedModule, context);
            AddSurrogateDeps(m_sizeOverlifeTimeModule, context);
            AddSurrogateDeps(m_subEmittersModule, context);
            AddSurrogateDeps(m_textureSheetAnimationModule, context);
            AddSurrogateDeps(m_trialModule, context);
            AddSurrogateDeps(m_triggerModule, context);
            AddSurrogateDeps(m_velocityOverLifetimeModule, context);
        }

        public override void GetDepsFrom(object obj, GetDepsFromContext context)
        {
            base.GetDepsFrom(obj, context);
            if (obj == null)
            {
                return;
            }

            ParticleSystem o = (ParticleSystem)obj;
            AddSurrogateDeps(o.collision, v_ => (PersistentParticleSystemNestedCollisionModule_RTSL_Template)v_, context);
            AddSurrogateDeps(o.colorBySpeed, v_ => (PersistentParticleSystemNestedColorBySpeedModule)v_, context);
            AddSurrogateDeps(o.colorOverLifetime, v_ => (PersistentParticleSystemNestedColorOverLifetimeModule)v_, context);
            AddSurrogateDeps(o.customData, v_ => (PersistentParticleSystemNestedCustomDataModule_RTSL_Template)v_, context);
            AddSurrogateDeps(o.emission, v_ => (PersistentParticleSystemNestedEmissionModule_RTSL_Template)v_, context);
            AddSurrogateDeps(o.externalForces, v_ => (PersistentParticleSystemNestedExternalForcesModule_RTSL_Template)v_, context);
            AddSurrogateDeps(o.forceOverLifetime, v_ => (PersistentParticleSystemNestedForceOverLifetimeModule)v_, context);
            AddSurrogateDeps(o.inheritVelocity, v_ => (PersistentParticleSystemNestedInheritVelocityModule)v_, context);
            AddSurrogateDeps(o.lights, v_ => (PersistentParticleSystemNestedLightsModule)v_, context);
            AddSurrogateDeps(o.limitVelocityOverLifetime, v_ => (PersistentParticleSystemNestedLimitVelocityOverLifetimeModule)v_, context);
            AddSurrogateDeps(o.main, v_ => (PersistentParticleSystemNestedMainModule)v_, context);
            AddSurrogateDeps(o.noise, v_ => (PersistentParticleSystemNestedNoiseModule)v_, context);
            AddSurrogateDeps(o.rotationBySpeed, v_ => (PersistentParticleSystemNestedRotationBySpeedModule)v_, context);
            AddSurrogateDeps(o.rotationOverLifetime, v_ => (PersistentParticleSystemNestedRotationOverLifetimeModule)v_, context);
            AddSurrogateDeps(o.shape, v_ => (PersistentParticleSystemNestedShapeModule)v_, context);
            AddSurrogateDeps(o.sizeBySpeed, v_ => (PersistentParticleSystemNestedSizeBySpeedModule)v_, context);
            AddSurrogateDeps(o.sizeOverLifetime, v_ => (PersistentParticleSystemNestedSizeOverLifetimeModule)v_, context);
            AddSurrogateDeps(o.subEmitters, v_ => (PersistentParticleSystemNestedSubEmittersModule_RTSL_Template)v_, context);
            AddSurrogateDeps(o.textureSheetAnimation, v_ => (PersistentParticleSystemNestedTextureSheetAnimationModule_RTSL_Template)v_, context);
            AddSurrogateDeps(o.trails, v_ => (PersistentParticleSystemNestedTrailModule)v_, context);
            AddSurrogateDeps(o.trigger, v_ => (PersistentParticleSystemNestedTriggerModule_RTSL_Template)v_, context);
            AddSurrogateDeps(o.velocityOverLifetime, v_ => (PersistentParticleSystemNestedVelocityOverLifetimeModule)v_, context);
        }

        //<TEMPLATE_BODY_END>
#endif
    }
}