
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RotationalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rotatingBody;
    [SerializeField] private Transform pivotBody;


    [Header("Controller Tuning")]
    [SerializeField] private float deadzoneDeg = 1.0f;
    [SerializeField] private float returnSpeed = 5.0f;
    [SerializeField] private float gain = 4.0f;
    [SerializeField] private float smoothing = 10f;

    [Header("Output")]
    [SerializeField] public Vector3 rateCmd;

    private Vector3 angles;
    private Quaternion qIdle;
    private float pitch;
    private float roll;

    private Vector2 pad;

    private bool grabbed = false;

    void Start()
    {
        
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
        pitch = localHandPos.x * -60f; //only works if inverted because of quaternion math shenanigans (i think)
        pitch = Mathf.Clamp(pitch, -20, 20);

        roll = localHandPos.z * 60f;
        roll = Mathf.Clamp(roll, -20, 20); 

        float rollcmd = Mathf.Abs(roll) - deadzoneDeg;
        if (rollcmd > 0)
            rateCmd[0] = gain * rollcmd * Mathf.Sign(roll);
            else
            rateCmd[0] = 0;
        
        float pitchcmd = Mathf.Abs(pitch) - deadzoneDeg;
        if (pitchcmd > 0)
            rateCmd[2] = gain * pitchcmd * Mathf.Sign(pitch);
            else
            rateCmd[2] = 0;
        
        

        //apply rotation to stick
        rotatingBody.localRotation =
            Quaternion.Euler(roll, 0f, pitch); 

         //grab current pad x state 
        pad = controller.activateActionValue.action.ReadValue<Vector2>();
        if (Mathf.Abs(pad.x) > deadzoneDeg/100) //if it's significant, read into command
        {
           
            rateCmd[1] = gain * Mathf.Sign(pad.x) * Mathf.Pow(pad.x, 2f);
        } 
        else 
        {
             rateCmd[1] = 0;
        };
           

    grabbed = true; //toggle grab state to tell the controller not to perform return behaviour
    }
    void FixedUpdate()
    {
        
        qIdle = pivotBody.localRotation;

        if (grabbed != true) //controller bounceback 
        {
            rotatingBody.localRotation = Quaternion.Lerp(rotatingBody.localRotation, qIdle, Time.deltaTime * returnSpeed);
            angles =rotatingBody.localRotation.eulerAngles;
            pitch = angles.x;
            roll = angles.y;
            
        }
            grabbed = false; //if controller is still grabbed at next call it will be reassigned as true before we get back here
        
    }
}