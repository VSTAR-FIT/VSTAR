using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using MathNet.Numerics.LinearAlgebra;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;



public class dynamicsModel : MonoBehaviour //itialize everything
{
     
   [SerializeField] private float mass = 25900;
   
    
    //this makes ICs available to set in unity editor
    [Header("Initial Rotational States")]
    [SerializeField] private Quaternion initialRotation = Quaternion.identity;
    [SerializeField] private Vector3 initialAngularVelocity = Vector3.zero;
    [SerializeField] private UnityEngine.Vector3 tau = Vector3.zero;

    [Header("Initial Positional States")]
    [SerializeField] private Vector3 initialPosition = Vector3.zero;
    [SerializeField] private Vector3 initialTranslationalVelocity = Vector3.zero;
    [SerializeField] private UnityEngine.Vector3 F_ext = Vector3.zero;
    private Rigidbody rb; //need for velocity


    //initialize rotational state vector before start so that fixedupdate doesnt run before rot gets a value
    public double[] rot = 
    new double[7] 
    { 
       0,
       0,
       0,
       1,

       0,
       0,
       0 
    }; 
    public double[] pos = new double[6];
    
    private RotateOrion3 Spin;
    private TranslateOrion3 Move;
    private ControlLaw ControlLaw;
    

   

 


 void Start()
 {
   
    ControlLaw = GetComponent<ControlLaw>();
    Spin = GetComponent<RotateOrion3>();
    Move = GetComponent<TranslateOrion3>();
     
    //fill in rotational state array from inspector values
    rot = new double[7] { 

        initialRotation.x, 
        initialRotation.y, 
        initialRotation.z, 
        initialRotation.w, 

        initialAngularVelocity.x, 
        initialAngularVelocity.y, 
        initialAngularVelocity.z 
    }; 

    pos = new double[6] { 

        initialPosition.x, 
        initialPosition.y, 
        initialPosition.z, 

        initialTranslationalVelocity.x, 
        initialTranslationalVelocity.y, 
        initialTranslationalVelocity.z 
    };

    //apply the effects of initial conditions before loop start
    rot = Spin.Rotate3(rot, tau); 
    transform.rotation = new Quaternion((float)rot[0], (float)rot[1], (float)rot[2], (float)rot[3]);

    pos = Move.Translate3(pos, F_ext, mass);
    transform.position = new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);
 }

 void FixedUpdate()
 {
   
   tau = ControlLaw.Tb_ext;
   F_ext = ControlLaw.F_ext;


   //it is very important to remember that tau x y z corresponds to the x y z axes of the game object - needs to be aliged with repectiv 1 2 3 axes of the actual model
   //*******************ROTATIONAL DYNAMICS***********************
    rot = Spin.Rotate3(rot, tau); 

    transform.rotation = new Quaternion((float)rot[0], (float)rot[1], (float)rot[2], (float)rot[3]); // actually apply rotation to rigidbody

    tau = Vector3.zero; // reset torques after loop


    //******************TRANSLATIONAL DYNAMICS********************
    pos = Move.Translate3(pos, F_ext, mass);

    transform.position = new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);

    F_ext = Vector3.zero; //reset force afer loop
 }
}
