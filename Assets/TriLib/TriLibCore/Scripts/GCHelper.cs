using System;
using System.Collections;
using UnityEngine;

namespace TriLibCore
{
    /// <summary>
    /// Represents a class that forces GC collection using a fixed interval.
    /// </summary>
    public class GCHelper : MonoBehaviour
    {
        private float _lastActivactionTime;

        private static GCHelper _instance;
        public static GCHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("TriLibGCHelper").AddComponent<GCHelper>();
                }
                return _instance;
            } 
        }

        public static float Interval = 1f;
        public static float DeactivationDelay = 10f;

        private void Update()
        {
            if (Time.time - _lastActivactionTime >= Interval)
            {
                GC.Collect();
                _lastActivactionTime = Time.time;
            }
        }

        public void Activate()
        {
            enabled = true;
        }

        public void Deactivate()
        {
            if (enabled)
            {
                return;
            }
            StartCoroutine(DeactivateInternal());
        }

        private IEnumerator DeactivateInternal()
        {
            yield return new WaitForSecondsRealtime(DeactivationDelay);
            enabled = false;
        }
    }
}