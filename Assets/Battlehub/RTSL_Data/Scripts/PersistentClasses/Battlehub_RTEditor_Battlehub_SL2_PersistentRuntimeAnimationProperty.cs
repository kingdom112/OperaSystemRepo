using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Battlehub.RTEditor;
using Battlehub.RTEditor.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace Battlehub.RTEditor.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentRuntimeAnimationProperty : PersistentSurrogate
    {
        
        public static implicit operator RuntimeAnimationProperty(PersistentRuntimeAnimationProperty surrogate)
        {
            if(surrogate == null) return default(RuntimeAnimationProperty);
            return (RuntimeAnimationProperty)surrogate.WriteTo(new RuntimeAnimationProperty());
        }
        
        public static implicit operator PersistentRuntimeAnimationProperty(RuntimeAnimationProperty obj)
        {
            PersistentRuntimeAnimationProperty surrogate = new PersistentRuntimeAnimationProperty();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

