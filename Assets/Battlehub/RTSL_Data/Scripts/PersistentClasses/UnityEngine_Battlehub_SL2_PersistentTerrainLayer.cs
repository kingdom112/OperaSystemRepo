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
    public partial class PersistentTerrainLayer : PersistentObject
    {
        [ProtoMember(256)]
        public long diffuseTexture;

        [ProtoMember(257)]
        public long normalMapTexture;

        [ProtoMember(258)]
        public long maskMapTexture;

        [ProtoMember(259)]
        public PersistentVector2 tileSize;

        [ProtoMember(260)]
        public PersistentVector2 tileOffset;

        [ProtoMember(261)]
        public PersistentColor specular;

        [ProtoMember(262)]
        public float metallic;

        [ProtoMember(263)]
        public float smoothness;

        [ProtoMember(264)]
        public float normalScale;

        [ProtoMember(265)]
        public PersistentVector4 diffuseRemapMin;

        [ProtoMember(266)]
        public PersistentVector4 diffuseRemapMax;

        [ProtoMember(267)]
        public PersistentVector4 maskMapRemapMin;

        [ProtoMember(268)]
        public PersistentVector4 maskMapRemapMax;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            TerrainLayer uo = (TerrainLayer)obj;
            diffuseTexture = ToID(uo.diffuseTexture);
            normalMapTexture = ToID(uo.normalMapTexture);
            maskMapTexture = ToID(uo.maskMapTexture);
            tileSize = uo.tileSize;
            tileOffset = uo.tileOffset;
            specular = uo.specular;
            metallic = uo.metallic;
            smoothness = uo.smoothness;
            normalScale = uo.normalScale;
            diffuseRemapMin = uo.diffuseRemapMin;
            diffuseRemapMax = uo.diffuseRemapMax;
            maskMapRemapMin = uo.maskMapRemapMin;
            maskMapRemapMax = uo.maskMapRemapMax;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            TerrainLayer uo = (TerrainLayer)obj;
            uo.diffuseTexture = FromID(diffuseTexture, uo.diffuseTexture);
            uo.normalMapTexture = FromID(normalMapTexture, uo.normalMapTexture);
            uo.maskMapTexture = FromID(maskMapTexture, uo.maskMapTexture);
            uo.tileSize = tileSize;
            uo.tileOffset = tileOffset;
            uo.specular = specular;
            uo.metallic = metallic;
            uo.smoothness = smoothness;
            uo.normalScale = normalScale;
            uo.diffuseRemapMin = diffuseRemapMin;
            uo.diffuseRemapMax = diffuseRemapMax;
            uo.maskMapRemapMin = maskMapRemapMin;
            uo.maskMapRemapMax = maskMapRemapMax;
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext context)
        {
            base.GetDepsImpl(context);
            AddDep(diffuseTexture, context);
            AddDep(normalMapTexture, context);
            AddDep(maskMapTexture, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            TerrainLayer uo = (TerrainLayer)obj;
            AddDep(uo.diffuseTexture, context);
            AddDep(uo.normalMapTexture, context);
            AddDep(uo.maskMapTexture, context);
        }
    }
}

