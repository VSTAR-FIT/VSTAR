
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TranslationalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rotatingBody;
    [SerializeField] private Transform pivotBody;
    

    [Header("Controller Tuning")]
    [SerializeField] private float deadzoneDeg = 1.0f;
    [SerializeField] private float returnSpeed = 5.0f;
    [SerializeField] private float gain = 4.0f;
    [SerializeField] private float smoothing = 10f;
    [SerializeField] private float railLength = 1f;


    [Header("Output")]
    [SerializeField] public Vector3 forceCmd;

    private Quaternion qIdle;
    private Quaternion qFilter;
    private Vector3 idlePos;
    private float pitch;
    private float roll;
    private float ds; //vertical distance from push in/pull out

    private bool grabbed = false;
    private bool railCheck = false;

    void Start()
    {

        //idle rotation and idle position in local controller frame
        qFilter = rotatingBody.rotation;
        qIdle = pivotBody.rotation;
        idlePos = pivotBody.InverseTransformPoint(rotatingBody.position);
    }

    //INTERACTION - triggers as long as collider in contact with controller
    private void OnTriggerStay(Collider other)
    {
        // Try to find a controller from the collider
        ActionBasedController controller = other.GetComponentInParent<ActionBasedController>();

        //if no controller or if the grab button isnt pressed, do nothing
        if (controller == null)
            return;
        if (!controller.selectAction.action.IsPressed())
            return;

        //grab hand, determine local position 
        Transform hand = controller.transform;
        Vector3 localHandPos = pivotBody.InverseTransformPoint(hand.position);

        //figure out where the stick is compared to the pivot
        Vector3 localRotatingBody = pivotBody.InverseTransformPoint(rotatingBody.position);

        //CHECK to see if user is intending x motion or yz motion
        if (controller.activateAction.action.IsPressed())
        {
          //define current stick displacement and reference to idle state
          float offset = localHandPos.y;
          Vector3 referencePosition = idlePos;

          //read displacement into position
          ds = Mathf.Clamp(offset, -railLength, railLength);
          referencePosition.y = idlePos.y + ds;
            Debug.Log("ds:" + ds);

          rotatingBody.localPosition = referencePosition;

          railCheck = true;  
        }
        else
        {
          
          //convert position to angle, clamp to mechanical limits
          pitch = localHandPos.x * -60f; //only works if inverted because of quaternion math shenanigans (i think)
          roll = localHandPos.z * 60f;
          
          pitch = Mathf.Clamp(pitch, -20, 20);
          roll = Mathf.Clamp(roll, -20, 20); 
          
          //apply rotation to stick
          rotatingBody.localRotation = Quaternion.Euler(roll, 0f, pitch); 
          grabbed = true; //toggle grab state to tell the controller not to perform return behaviour
        }
    grabbed = true; //toggle grab state to tell the controller not to perform return behaviour
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
            Quaternion qError = qFilter * Quaternion.Inverse(qIdle);

            // convert to axis-angle
            qError.ToAngleAxis(out float angleDeg, out Vector3 axis);

            //don't do anything if controller is in idle state
            if (axis == Vector3.zero)
                return;

            // convert angle and deadzone to radians
            float angleRad = angleDeg * Mathf.Deg2Rad;
            float deadzoneRad = deadzoneDeg * Mathf.Deg2Rad;
            //dont output command if in deadzone
            if (angleRad < deadzoneRad)
                return;

            //command angle
            float cmd = angleRad - deadzoneRad;
            cmd = cmd * cmd; //quatratic makes large movements much more impactful while minimizing small changes (like jitter)


            //read in commands (MAY HAVE TO FLIP THEM AROUND DEPENDING ON COORDINATE SYSTEM)
            forceCmd[0] = gain * cmd * axis.normalized[0];
            forceCmd[1] = gain * Mathf.Pow(ds/railLength, 2f);
            forceCmd[2] = gain * cmd * axis.normalized[2];
        




        if (grabbed != true) //controller bounceback 
        {
            rotatingBody.localRotation = Quaternion.Lerp(rotatingBody.localRotation, Quaternion.Euler(0,0,0), Time.deltaTime * returnSpeed);
            
        }
            grabbed = false; //if controller is still grabbed at next call it will be reassigned as true before we get back here

        if (railCheck != true)
        {
            rotatingBody.localPosition = Vector3.Lerp(rotatingBody.localPosition, idlePos, Time.deltaTime * returnSpeed);
        }
            railCheck = false;
    }
}