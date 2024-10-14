using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Battlehub.RTTerrain;
using Battlehub.RTTerrain.Battlehub.SL2;
using UnityEngine.Battlehub.SL2;
using UnityEngine;
using System;

using UnityObject = UnityEngine.Object;
namespace Battlehub.RTTerrain.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentTerrainBrushSource : PersistentMonoBehaviour
    {
        [ProtoMember(258)]
        public long[] UserTextures;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            TerrainBrushSource uo = (TerrainBrushSource)obj;
            UserTextures = ToID(uo.UserTextures);
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            TerrainBrushSource uo = (TerrainBrushSource)obj;
            uo.UserTextures = FromID(UserTextures, uo.UserTextures);
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext context)
        {
            base.GetDepsImpl(context);
            AddDep(UserTextures, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            TerrainBrushSource uo = (TerrainBrushSource)obj;
            AddDep(uo.UserTextures, context);
        }
    }
}

