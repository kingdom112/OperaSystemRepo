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
    public partial class PersistentMeshRenderer : PersistentRenderer
    {
        [ProtoMember(256)]
        public long additionalVertexStreams;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            MeshRenderer uo = (MeshRenderer)obj;
            additionalVertexStreams = ToID(uo.additionalVertexStreams);
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            MeshRenderer uo = (MeshRenderer)obj;
            uo.additionalVertexStreams = FromID(additionalVertexStreams, uo.additionalVertexStreams);
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext context)
        {
            base.GetDepsImpl(context);
            AddDep(additionalVertexStreams, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            MeshRenderer uo = (MeshRenderer)obj;
            AddDep(uo.additionalVertexStreams, context);
        }
    }
}

