using UnityEngine;

namespace KayakingVR.Utils
{
    /// <summary>
    /// Lazy version on "Low pass filter for Vector3"
    /// </summary>
    public class LowPassFilter
    {
        private float smoothingFactor;
        private Vector3 filteredValue;

        public LowPassFilter(float smoothingFactor)
        {
            this.smoothingFactor = smoothingFactor;
        }

        public Vector3 Filter(Vector3 rawValue)
        {
            filteredValue = Vector3.Lerp(filteredValue, rawValue, smoothingFactor);
            return filteredValue;
        }

        public void Reset(Vector3 initialValue)
        {
            filteredValue = initialValue;
        }
    }
}