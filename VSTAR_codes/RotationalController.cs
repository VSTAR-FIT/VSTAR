using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RotationalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rotatingBody;
    [SerializeField] private Transform pivotBody;


    [Header("Control Tuning")]
    [SerializeField] private float deadzoneDeg = 1.0f;
    [SerializeField] private float returnSpeed = 5.0f;
    [SerializeField] private float gain = 4.0f;
    [SerializeField] private float smoothing = 10f;

    [Header("Output")]
    [SerializeField] private Vector3 rateCmd;

    private Quaternion qFilter;
    private float pitch;
    private float roll;

    private bool grabbed = false;

    void Start()
    {
        qFilter = rotatingBody.rotation;
    }

    //INTERACTION - triggers as long as collider in contact with controller
    private void OnTriggerStay(Collider other)
    {
        // Try to find a controller from the collider
        ActionBasedController controller =
            other.GetComponentInParent<ActionBasedController>();

        //if no controller or if the grab button isnt pressed, do nothing
        if (controller == null)
            return;
        if (!controller.selectAction.action.IsPressed())
            return;

        //grab hand, determine local position 
        Transform hand = controller.transform;
        Vector3 localHandPos = pivotBody.InverseTransformPoint(hand.position);

        //convert position to angle, clamp to mechanical limits
        pitch = localHandPos.x * -60f;
        pitch = Mathf.Clamp(pitch, -20, 20);

        roll = localHandPos.z * 60f;
        roll = Mathf.Clamp(roll, -20, 20);


        //apply rotation to stick
        rotatingBody.localRotation =
            Quaternion.Euler(roll, 0f, pitch); //roll is negative i do not know why...... thats how we get it to work just roll with it >:)


    grabbed = true;
    }
    void FixedUpdate()
    {

            // use slerp as a lowpass filter to reduce noise from IMU jitter
            qFilter = Quaternion.Slerp(
                qFilter,
                rotatingBody.rotation,
                smoothing * Time.fixedDeltaTime
            );

            // calculate quaternion error
            Quaternion qIdle = pivotBody.parent.rotation;
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

            rateCmd = gain * cmd * axis.normalized;




        if (grabbed != true) //controller bounceback 
        {
            rotatingBody.localRotation = Quaternion.Lerp(rotatingBody.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
            
        }
            grabbed = false; //if controller is still grabbed at next call it will be reassigned as true before we get back here
        
    }
}