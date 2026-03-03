using System.Runtime.Serialization; 
using System.Xml.Serialization; 
using UnityEngine; 
using MathNet.Numerics.LinearAlgebra;




public class ControlLaw : MonoBehaviour
{
    [Header("RCS Parameters")]
    [SerializeField] private float thrusterForce = 216f;
    [SerializeField] private float leverArm = 2f;
    [SerializeField] private float MIB = 0.028f;

    [Header("Controller References")]
    [SerializeField] private RotationalController RHC;
    [SerializeField] private TranslationalController THC;

   

    private UnityEngine.Vector3 rotImpulseAccumulator = Vector3.zero;
    private Vector3 posImpulseAccumulator = Vector3.zero;

    public Vector3 Tb_ext = Vector3.zero;
    public Vector3 F_ext  = Vector3.zero;

    public Vector<double> w;

    private static double r = 2;
    private static double L = 4.29;
    private static Matrix<double> thruster_select =  Matrix<double>.Build.DenseOfArray(new double[,]{{0,0,0,0,     -1,-1,1,1,   -1,-1,1,1,     0,0,0,0,    -1,-1,1,1,  -1,-1,1,1},
                                                                                              {1,-1,1,-1,    0,0,0,0,    -1,-1,-1,-1,   1,-1,1,-1,    0,0,0,0,     1,1,1,1},
                                                                                              {-1,-1,-1,-1, -1,-1,-1,-1,   0,0,0,0,     1,1,1,1,     1,1,1,1,     0,0,0,0},
                                                                                              {-r,r,-r,r,     0,0,0,0,     0,0,0,0,    r,-r,r,-r,    0,0,0,0,     0,0,0,0},
                                                                                              {0,0,0,0,      -L,-L,L,L,    0,0,0,0,     0,0,0,0,     L,L,-L,-L,   0,0,0,0},
                                                                                              {0,0,0,0,       0,0,0,0,   -L,-L, L,L,    0,0,0,0,      0,0,0,0,    L,L,-L,-L}});

    void Start()
    {
       
    }
    void FixedUpdate()
    {

       

       var Tcmd = Vector3.zero;
       var Fcmd  = Vector3.zero;

        

        // --- ROTATION ---
        Vector3 torqueCmd = RHC.rateCmd;
        rotImpulseAccumulator += torqueCmd * Time.fixedDeltaTime;

        for (int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(rotImpulseAccumulator[i]) >= MIB)
            {
                Tcmd[i] = Mathf.Sign(rotImpulseAccumulator[i]) * thrusterForce * leverArm;
                rotImpulseAccumulator[i] = 0f;
            }
        }

        // --- TRANSLATION ---
        Vector3 forceCmd = THC.forceCmd;
        posImpulseAccumulator += forceCmd * Time.fixedDeltaTime;

        for (int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(posImpulseAccumulator[i]) >= MIB)
            {
                Fcmd[i] = Mathf.Sign(posImpulseAccumulator[i]) * thrusterForce;
                posImpulseAccumulator[i] = 0f;
            }
        }


        var M = thruster_select.PseudoInverse();
        var f = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(new double[] {Fcmd[0], Fcmd[1], Fcmd[2], Tcmd[0], Tcmd[1], Tcmd[2],} );

        //run command through inverted thruster authority matrix, M*f which gives a 24x1 vactor
        //thrusteroutput is just a list of individual thruster contributions, so run it through the normal thruster authority matrix to turn it back into a 6x1 vector 

        var thrusteroutput = 0.707*M*f; //0.707 represents cos of the angle the thrusters are at
        var w = thruster_select * thrusteroutput;
       

       //read thruster firings into external torques
       F_ext.x = (float)w[0];
       F_ext.y = (float)w[1];
       F_ext.z = (float)w[2];

       Tb_ext.x = (float)w[3];
       Tb_ext.y = (float)w[4];
       Tb_ext.z = (float)w[5];

        
    }
}
