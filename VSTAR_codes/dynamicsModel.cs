using System.Linq;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System.Numerics;
using System.Threading.Tasks.Dataflow;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

public class dynamicsModel : MonoBehaviour //itialize everything
{
    [SerializeField] int mass_dry = 13486;
    [SerializeField] int mass_prop = 12414;

   
    
    //this makes ICs available to set in unity editor
    [Header("Initial Rotational States")]
    [SerializeField] private Quaternion initialRotation = Quaternion.identity;
    [SerializeField] private Vector3 initialAngularVelocity = Vector3.zero;
    [SerializeField] private UnityEngine.Vector3 tau = Vector3.zero;

    [Header("Initial Positional States")]
    [SerializeField] private Vector3 initialPosition = Vector3.zero;
    [SerializeField] private Vector3 initialTranslationalVelocity = Vector3.zero;


    //initialize rotational state vector before start so that fixedupdate doesnt run before rot gets a value
    private double[] rot = 
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
    
    void Start()
 {
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

    pos = new double[7] { 

        initialPosition.x, 
        initialPosition.y, 
        initialPosition.z, 

        initialTranslationalVelocity.x, 
        initialTranslationalVelocity.y, 
        initialTranslationalVelocity.z 
    }; 

 }

 void FixedUpdate()
 {

    tau = ControlLaw.Tb_ext;
    F_ext = ControlLaw.F_ext;

   //*******************ROTATIONAL DYNAMICS***********************
    double[,] rot =
    {
        {q.x},
        {q.y},
        {q.z},
        {q.w},
        {w.x},
        {w.y},
        {w.z}
    };
    // check if torque input
    if (Keyboard.current.uKey.isPressed) tau.x += 100f;
    if (Keyboard.current.iKey.isPressed) tau.y += 100f;
    if (Keyboard.current.oKey.isPressed) tau.z += 100f;

    if (Keyboard.current.jKey.isPressed) tau.x -= 100f;
    if (Keyboard.current.kKey.isPressed) tau.y -= 100f;
     if (Keyboard.current.lKey.isPressed) tau.z -= 100f;

     
    
    //it is very important to remember that tau x y z corresponds to the x y z axes of the game object - needs to be aliged with repectiv 1 2 3 axes of the actual model

    rot = RotateOrion3.Rotate3(rot, tau); 
    transform.rotation = new Quaternion((float)rot[0], (float)rot[1], (float)rot[2], (float)rot[3]); // actually apply rotation to rigidbody


    //******************TRANSLATIONAL DYNAMICS********************
    

    pos = TranslateOrion3.Translate3(pos, F_ext, mass);
    tr
 }
//create the initial rotational state vector, feed each element in individually because q is a struct and w is vector3


