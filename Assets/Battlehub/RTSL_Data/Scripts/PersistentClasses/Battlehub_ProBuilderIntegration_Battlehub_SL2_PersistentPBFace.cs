using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Battlehub.ProBuilderIntegration;
using Battlehub.ProBuilderIntegration.Battlehub.SL2;
using System;

using UnityObject = UnityEngine.Object;
namespace Battlehub.ProBuilderIntegration.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentPBFace : PersistentSurrogate
    {
        [ProtoMember(256)]
        public int[] Indexes;

        [ProtoMember(257)]
        public int SubmeshIndex;

        [ProtoMember(258)]
        public int TextureGroup;

        [ProtoMember(259)]
        public bool IsManualUV;

        [ProtoMember(260)]
        public PersistentPBAutoUnwrapSettings UnwrapSettings;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            PBFace uo = (PBFace)obj;
            Indexes = uo.Indexes;
            SubmeshIndex = uo.SubmeshIndex;
            TextureGroup = uo.TextureGroup;
            IsManualUV = uo.IsManualUV;
            UnwrapSettings = uo.UnwrapSettings;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            PBFace uo = (PBFace)obj;
            uo.Indexes = Indexes;
            uo.SubmeshIndex = SubmeshIndex;
            uo.TextureGroup = TextureGroup;
            uo.IsManualUV = IsManualUV;
            uo.UnwrapSettings = UnwrapSettings;
            return uo;
        }

        public static implicit operator PBFace(PersistentPBFace surrogate)
        {
            if(surrogate == null) return default(PBFace);
            return (PBFace)surrogate.WriteTo(new PBFace());
        }
        
        public static implicit operator PersistentPBFace(PBFace obj)
        {
            PersistentPBFace surrogate = new PersistentPBFace();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

