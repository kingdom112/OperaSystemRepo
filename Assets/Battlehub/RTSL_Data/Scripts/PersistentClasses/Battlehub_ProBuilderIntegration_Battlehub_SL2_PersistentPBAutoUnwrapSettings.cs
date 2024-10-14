using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Battlehub.ProBuilderIntegration;
using Battlehub.ProBuilderIntegration.Battlehub.SL2;
using System;
using UnityEngine;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace Battlehub.ProBuilderIntegration.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentPBAutoUnwrapSettings : PersistentSurrogate
    {
        [ProtoMember(258)]
        public PBAutoUnwrapSettings.Anchor anchor;

        [ProtoMember(259)]
        public float rotation;

        [ProtoMember(260)]
        public PersistentVector2 offset;

        [ProtoMember(261)]
        public PersistentVector2 scale;

        [ProtoMember(262)]
        public PBAutoUnwrapSettings.Fill fill;

        [ProtoMember(263)]
        public bool swapUV;

        [ProtoMember(264)]
        public bool flipV;

        [ProtoMember(265)]
        public bool flipU;

        [ProtoMember(266)]
        public bool useWorldSpace;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            PBAutoUnwrapSettings uo = (PBAutoUnwrapSettings)obj;
            anchor = uo.anchor;
            rotation = uo.rotation;
            offset = uo.offset;
            scale = uo.scale;
            fill = uo.fill;
            swapUV = uo.swapUV;
            flipV = uo.flipV;
            flipU = uo.flipU;
            useWorldSpace = uo.useWorldSpace;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            PBAutoUnwrapSettings uo = (PBAutoUnwrapSettings)obj;
            uo.anchor = anchor;
            uo.rotation = rotation;
            uo.offset = offset;
            uo.scale = scale;
            uo.fill = fill;
            uo.swapUV = swapUV;
            uo.flipV = flipV;
            uo.flipU = flipU;
            uo.useWorldSpace = useWorldSpace;
            return uo;
        }

        public static implicit operator PBAutoUnwrapSettings(PersistentPBAutoUnwrapSettings surrogate)
        {
            if(surrogate == null) return default(PBAutoUnwrapSettings);
            return (PBAutoUnwrapSettings)surrogate.WriteTo(new PBAutoUnwrapSettings());
        }
        
        public static implicit operator PersistentPBAutoUnwrapSettings(PBAutoUnwrapSettings obj)
        {
            PersistentPBAutoUnwrapSettings surrogate = new PersistentPBAutoUnwrapSettings();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

