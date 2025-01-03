#if !RTSL_MAINTENANCE
using Battlehub.RTSL;
using Battlehub.RTCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Battlehub.SL2
{
    [CustomImplementation]
    public partial class PersistentGradient
    {        

        [ProtoMember(1, IsRequired = true)]
        public PersistentGradientColorKey[] colorKeys;

        [ProtoMember(2, IsRequired = true)]
        public PersistentGradientAlphaKey[] alphaKeys;

        public override void ReadFrom(object obj)
        {
            base.ReadFrom(obj);
            if(obj == null)
            {
                return;
            }
            Gradient uo = (Gradient)obj;
            colorKeys = Assign(colorKeys, v_ => (PersistentGradientColorKey)v_);
            alphaKeys = Assign(alphaKeys, v_ => (PersistentGradientAlphaKey)v_);   
        }

        public override object WriteTo(object obj)
        {
            obj = base.WriteTo(obj);
            if(obj == null)
            {
                return obj;
            }
            Gradient uo = (Gradient)obj;
            if(colorKeys != null)
            {
                uo.colorKeys = Assign(colorKeys, v_ => (GradientColorKey)v_);
            }
            if(alphaKeys != null)
            {
                uo.alphaKeys = Assign(alphaKeys, v_ => (GradientAlphaKey)v_);
            }
            return uo;
        }

        public override void GetDeps(GetDepsContext context)
        {
            base.GetDeps(context);
        }

        public override void GetDepsFrom(object obj, GetDepsFromContext context)
        {
            base.GetDepsFrom(obj, context);
        }
    }
}
#endif

