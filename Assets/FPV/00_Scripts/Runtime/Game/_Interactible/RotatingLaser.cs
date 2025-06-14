using UnityEngine;

namespace FPV.Runtime
{
    public class RotatingLaser : MonoBehaviour
    {
        public Transform pivot;
        public float rotationSpeed = 30f;

        private void Update()
        {
            if (pivot != null) pivot.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}