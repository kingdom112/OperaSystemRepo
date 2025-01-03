using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentGradient : PersistentSurrogate
    {
        [ProtoMember(258)]
        public GradientMode mode;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            Gradient uo = (Gradient)obj;
            mode = uo.mode;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            Gradient uo = (Gradient)obj;
            uo.mode = mode;
            return uo;
        }

        public static implicit operator Gradient(PersistentGradient surrogate)
        {
            if(surrogate == null) return default(Gradient);
            return (Gradient)surrogate.WriteTo(new Gradient());
        }
        
        public static implicit operator PersistentGradient(Gradient obj)
        {
            PersistentGradient surrogate = new PersistentGradient();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

