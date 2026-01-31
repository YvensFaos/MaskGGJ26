using UnityEngine;

namespace Core
{
    public class Rotate : MonoBehaviour
    {
        [SerializeField]
        private float speed;
        [SerializeField]
        private Vector3 axis;
        
        private void Update()
        {
            transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * speed, axis);
        }
    }
}
