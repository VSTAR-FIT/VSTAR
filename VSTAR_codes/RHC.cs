using UnityEngine;

public class RotationalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform controllerTransform;
    [SerializeField] private Transform idleRotation;
    [SerializeField] private Rigidbody controllerBody;

    [Header("Control Tuning")]
    [SerializeField] private float deadzoneDeg = 1.0f;
    [SerializeField] private float gain = 4.0f;
    [SerializeField] private float smoothing = 10f;

    private Quaternion qFilter;

    void Start()
    {
        qFilter = controllerTransform.rotation;
    }

    Vector3 FixedUpdate()
    {
        // use slerp as a lowpass filter to reduce noise from IMU jitter
        qFilter = Quaternion.Slerp(
            qFilter,
            controllerTransform.rotation,
            smoothing * Time.fixedDeltaTime
        );

        // calculate quaternion error
        Quaternion qIdle = idleRotation.rotation;
        Quaternion qError = qFilter * Quaternion.Inverse(qIdle);

        // convert to axis-angle
        qError.ToAngleAxis(out float angleDeg, out Vector3 axis);

        //don't do anything if controller is in idle state
        if (axis == Vector3.zero)
            return;

        // convert angle and deadzone to radians
        float angleRad = angleDeg * Mathf.Deg2Rad;
        float deadzoneRad = deadzoneDeg * Mathf.Deg2Rad;
        //dont fire thrusters if in deadzone
        if (angleRad < deadzoneRad)
            return;

        //command angle
        float cmd = angleRad - deadzoneRad;
        cmd = cmd * cmd; //quatratic makes large movements much more impactful while minimizing small changes (like jitter)


        //commanded torque
        Vector3 rateCmd = gain * cmd * axis.normalized;
       


        return rateCmd;
    }
}
