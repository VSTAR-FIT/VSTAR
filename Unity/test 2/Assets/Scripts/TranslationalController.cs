
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TranslationalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rotatingBody;
    [SerializeField] private Transform pivotBody;
    

    [Header("Controller Tuning")]
    [SerializeField] private float controllerThrow = 40.0f;
    [SerializeField] private float deadzoneDeg = 20.0f;
    [SerializeField] private float returnSpeed = 5.0f;
    [SerializeField] private float gain = 4.0f;
    [SerializeField] private float railLength = 1f;


    [Header("Output")]
    [SerializeField] public Vector3 forceCmd;

    private Quaternion qIdle;
    private Vector3 railAxis;
    
    private Vector3 idlePos;
    private float pitch;
    private float roll;
    private float ds; //vertical distance from push in/pull out

    private bool grabbed = false;
    private bool railCheck = false;

    void Start()
    {
        
        //idle rotation and idle position in local controller frame
        idlePos = pivotBody.localPosition;
        qIdle = pivotBody.localRotation;
        railAxis = pivotBody.InverseTransformDirection(pivotBody.up);
        
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
          Vector3 referencePosition = idlePos;
          Vector3 handLocal = pivotBody.InverseTransformPoint(hand.position);
          Vector3 displacement = handLocal - idlePos;

          ds = Vector3.Dot(displacement, railAxis)-0.025f; //subtract hand diameter (otherwise we get weird jumps)
          ds = Mathf.Clamp(ds, -railLength, railLength);

          //read displacement into position
          referencePosition = idlePos + railAxis*ds;

          forceCmd[0] = -1* gain * Mathf.Pow(ds/railLength, 2f) * Mathf.Sign(ds); // negative bcuse of parent rotation

          rotatingBody.localPosition = referencePosition;

          railCheck = true;  
        }
        else
        {
          
          //convert position to angle, clamp to mechanical limits
          pitch = localHandPos.x * -60f; //only works if inverted because of quaternion math shenanigans (i think)
          roll = localHandPos.z * 60f;
          
          pitch = Mathf.Clamp(pitch, -controllerThrow, controllerThrow);
          roll = Mathf.Clamp(roll, -controllerThrow, controllerThrow); 

          float ycmd = Mathf.Abs(pitch) - deadzoneDeg;
        if (ycmd > 0)
            forceCmd[1] = -1 *gain * ycmd * Mathf.Sign(pitch);
            else
            forceCmd[1] = 0;
        
        float zcmd = Mathf.Abs(roll) - deadzoneDeg;
        if (zcmd > 0)
            forceCmd[2] = gain * zcmd * Mathf.Sign(roll);
            else
            forceCmd[2] = 0;
          
          //apply rotation to stick
          rotatingBody.localRotation = Quaternion.Euler(roll, 0f, pitch); 
          grabbed = true; //toggle grab state to tell the controller not to perform return behaviour
        }
    grabbed = true; //toggle grab state to tell the controller not to perform return behaviour
    }
 void FixedUpdate()
    {

            forceCmd = Vector3.zero;
        if (grabbed != true) //controller bounceback 
        {
            rotatingBody.localRotation = Quaternion.Lerp(rotatingBody.localRotation, qIdle, Time.deltaTime * returnSpeed);
            
            
        }
            grabbed = false; //if controller is still grabbed at next call it will be reassigned as true before we get back here

        if (railCheck != true)
        {
            rotatingBody.localPosition = Vector3.Lerp(rotatingBody.localPosition, idlePos, Time.deltaTime * returnSpeed);
        }
            railCheck = false;
    }
}