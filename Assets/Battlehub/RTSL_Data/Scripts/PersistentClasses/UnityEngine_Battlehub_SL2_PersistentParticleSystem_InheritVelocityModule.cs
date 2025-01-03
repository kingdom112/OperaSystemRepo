using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine;
using UnityEngine.Battlehub.SL2;
using System;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentParticleSystemNestedInheritVelocityModule : PersistentSurrogate
    {
        [ProtoMember(256)]
        public bool enabled;

        [ProtoMember(257)]
        public ParticleSystemInheritVelocityMode mode;

        [ProtoMember(258)]
        public PersistentParticleSystemNestedMinMaxCurve curve;

        [ProtoMember(259)]
        public float curveMultiplier;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            ParticleSystem.InheritVelocityModule uo = (ParticleSystem.InheritVelocityModule)obj;
            enabled = uo.enabled;
            mode = uo.mode;
            curve = uo.curve;
            curveMultiplier = uo.curveMultiplier;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            ParticleSystem.InheritVelocityModule uo = (ParticleSystem.InheritVelocityModule)obj;
            uo.enabled = enabled;
            uo.mode = mode;
            uo.curve = curve;
            uo.curveMultiplier = curveMultiplier;
            return uo;
        }

        public static implicit operator ParticleSystem.InheritVelocityModule(PersistentParticleSystemNestedInheritVelocityModule surrogate)
        {
            if(surrogate == null) return default(ParticleSystem.InheritVelocityModule);
            return (ParticleSystem.InheritVelocityModule)surrogate.WriteTo(new ParticleSystem.InheritVelocityModule());
        }
        
        public static implicit operator PersistentParticleSystemNestedInheritVelocityModule(ParticleSystem.InheritVelocityModule obj)
        {
            PersistentParticleSystemNestedInheritVelocityModule surrogate = new PersistentParticleSystemNestedInheritVelocityModule();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

