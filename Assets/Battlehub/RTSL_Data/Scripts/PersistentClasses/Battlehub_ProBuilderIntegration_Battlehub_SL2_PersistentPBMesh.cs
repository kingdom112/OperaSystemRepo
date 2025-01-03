using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Battlehub.ProBuilderIntegration;
using Battlehub.ProBuilderIntegration.Battlehub.SL2;
using UnityEngine.Battlehub.SL2;
using UnityEngine;

using UnityObject = UnityEngine.Object;
namespace Battlehub.ProBuilderIntegration.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentPBMesh : PersistentMonoBehaviour
    {
        [ProtoMember(256)]
        public PersistentPBFace[] Faces;

        [ProtoMember(257)]
        public PersistentVector3[] Positions;

        [ProtoMember(258)]
        public PersistentVector2[] Textures;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            PBMesh uo = (PBMesh)obj;
            Faces = Assign(uo.Faces, v_ => (PersistentPBFace)v_);
            Positions = Assign(uo.Positions, v_ => (PersistentVector3)v_);
            Textures = Assign(uo.Textures, v_ => (PersistentVector2)v_);
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            PBMesh uo = (PBMesh)obj;
            uo.Faces = Assign(Faces, v_ => (PBFace)v_);
            uo.Positions = Assign(Positions, v_ => (Vector3)v_);
            uo.Textures = Assign(Textures, v_ => (Vector2)v_);
            return uo;
        }
    }
}

