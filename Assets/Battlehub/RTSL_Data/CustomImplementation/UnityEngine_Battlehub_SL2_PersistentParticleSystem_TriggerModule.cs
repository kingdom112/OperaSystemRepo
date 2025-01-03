#if !RTSL_MAINTENANCE
using Battlehub.RTSL;
using ProtoBuf;
using UnityEngine;

namespace UnityEngine.Battlehub.SL2
{
    [CustomImplementation]
    public partial class PersistentParticleSystemNestedTriggerModule
    {        

        [ProtoMember(1)]
        public long[] m_colliders;

        public override object WriteTo(object obj)
        {
            obj = base.WriteTo(obj);
            if (obj == null)
            {
                return null;
            }

            ParticleSystem.TriggerModule o = (ParticleSystem.TriggerModule)obj;
            if (m_colliders == null)
            {
                for (int i = 0; i < o.maxColliderCount; ++i)
                {
                    o.SetCollider(i, null);
                }
            }
            else
            {
                for (int i = 0; i < Mathf.Min(o.maxColliderCount, m_colliders.Length); ++i)
                {
                    o.SetCollider(i, FromID<Component>(m_colliders[i]));
                }
            }

            return obj;
        }

        public override void ReadFrom(object obj)
        {
            base.ReadFrom(obj);
            if (obj == null)
            {
                return;
            }

            ParticleSystem.TriggerModule o = (ParticleSystem.TriggerModule)obj;
            if (o.maxColliderCount > 20)
            {
                Debug.LogWarning("maxPlaneCount is expected to be 6 or at least <= 20");
            }
            m_colliders = new long[o.maxColliderCount];
            for (int i = 0; i < o.maxColliderCount; ++i)
            {
                Component collider = o.GetCollider(i);
                m_colliders[i] = ToID(collider);
            }
        }

        public override void GetDeps(GetDepsContext context)
        {
            base.GetDeps(context);
            AddDep(m_colliders, context);
        }

        public override void GetDepsFrom(object obj, GetDepsFromContext context)
        {
            base.GetDepsFrom(obj, context);
            if (obj == null)
            {
                return;
            }

            ParticleSystem.TriggerModule o = (ParticleSystem.TriggerModule)obj;
            for (int i = 0; i < o.maxColliderCount; ++i)
            {
                AddDep(o.GetCollider(i), context);
            }
        }
    }
}
#endif

