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
    public partial class PersistentTerrainCollider : PersistentCollider
    {
        [ProtoMember(256)]
        public long terrainData;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            TerrainCollider uo = (TerrainCollider)obj;
            terrainData = ToID(uo.terrainData);
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            TerrainCollider uo = (TerrainCollider)obj;
            uo.terrainData = FromID(terrainData, uo.terrainData);
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext context)
        {
            base.GetDepsImpl(context);
            AddDep(terrainData, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            TerrainCollider uo = (TerrainCollider)obj;
            AddDep(uo.terrainData, context);
        }
    }
}

