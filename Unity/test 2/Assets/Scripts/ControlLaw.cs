using System.Runtime.Serialization; 
using System.Xml.Serialization; 
using UnityEngine; 
using MathNet.Numerics.LinearAlgebra;
using System.Numerics;
using UnityEngine;



public class ControlLaw : MonoBehaviour
{
    [Header("RCS Parameters")]
    [SerializeField] private float thrusterForce = 216f;
    [SerializeField] private float leverArm = 5f;
    [SerializeField] private float MIB = 0.028f;

    private UnityEngine.Vector3 rotImpulseAccumulator = Vector3.zero;
    private Vector3 posImpulseAccumulator = Vector3.zero;

    public Vector3 Tb_ext { get; public set; } = Vector3.Zero;
    public Vector3 F_ext  { get; public set; } = Vector3.Zero;

    public Vector<double> w;

    private double r = 2;
    private double L = 4.29;
    private Matrix<double> thruster_select =  Matrix<double>.Build.DenseOfArray(new double[,]{{0,0,0,0,     -1,-1,1,1,   -1,-1,1,1,     0,0,0,0,    -1,-1,1,1,  -1,-1,1,1},
                                                                                              {1,-1,1,-1,    0,0,0,0,    -1,-1,-1,-1,   1,-1,1,-1,    0,0,0,0,     1,1,1,1},
                                                                                              {-1,-1,-1,-1, -1,-1,-1,-1,   0,0,0,0,     1,1,1,1,     1,1,1,1,     0,0,0,0},
                                                                                              {-r,r,-r,r,     0,0,0,0,     0,0,0,0,    r,-r,r,-r,    0,0,0,0,     0,0,0,0},
                                                                                              {0,0,0,0,      -L,-L,L,L,    0,0,0,0,     0,0,0,0,     L,L,-L,-L,   0,0,0,0},
                                                                                              {0,0,0,0,       0,0,0,0,   -L,-L, L,L,    0,0,0,0,      0,0,0,0,    L,L,-L,-L}});

    
    void FixedUpdate()
    {
       var Tcmd = Vector3.zero;
       var Fcmd  = Vector3.zero;

        

        // --- ROTATION ---
        Vector3 torqueCmd = RotationalController.rateCmd;
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
        Vector3 forceCmd = TranslationalController.forceCmd;
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

        w = 0.707*M*f; //0.707 represents cos of the angle the thrusters are at

        F_ext.X = (float)w[0];
        F_ext.Y = (float)w[1];
        F_ext.Z = (float)w[2];
        
        Tb_ext.X = (float)w[3];
        Tb_ext.Y = (float)w[4];
        Tb_ext.Z = (float)w[5];
    }
}
