using UnityEngine; 

public class Translationalcontroller : MonoBehaviour
{
     [Header("References")]
    [SerializeField] private Transform controllerTransform;
    [SerializeField] private Transform idleTransform;
    [SerializeField] private Rigidbody controllerBody;

    [Header("Control Tuning")]
    [SerializeField] private float deadzoneDeg = 1.0f;
    [SerializeField] private float gain = 4.0f;
    [SerializeField] private float maxTorque = 167f;
    [SerializeField] private float smoothing = 10f;
}