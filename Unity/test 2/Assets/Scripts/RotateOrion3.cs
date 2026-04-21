
using UnityEngine;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;







public class RotateOrion3 : MonoBehaviour
{
	//initialize the inertia matrix, take inverse (need for state derivative) MOIS ARE PLACEHOLDER
    private const double Ixx = 40000 ; 
    private const double Iyy = 40000 ;
    private const double Izz = 40000 ;
    
    private readonly Matrix<double> MOI_b ;
    private readonly Matrix<double> MOI_b_inv ;
      
      

        //skew symmetric matrix for angular velocity
    public static Matrix<double> Skew(MathNet.Numerics.LinearAlgebra.Vector<double> w)
        {
             return Matrix<double>.Build.DenseOfArray(new double[,]
            {
                {  0,   -w[2],  w[1] },
                {  w[2],  0,  -w[0] },
                { -w[1], w[0],   0  }
            });
        }
   

//CREATE FUNCTION THAT WILL TAKE STATE VECTOR AND EXTERNAL TORQUE, RETURN NEW STATE VECTOR
 public double[] Rotate3(double[] x, UnityEngine.Vector3 Tb_ext )
   {
    
     //safety reset
     if (x.Length != 7)
        {
             Debug.LogError($"State array wrong size");
             

            return new double[7] {0, 0, 0, 1, 0, 0, 0} ; // identity quaternion + zero angular velocity
        }
     if (x == null)
        {
             Debug.LogError($"State array is null");
             

            return new double[7] {0, 0, 0, 1, 0, 0, 0} ; // identity quaternion + zero angular velocity
        }


    //build moi matrix and invert it
     var MOI_b = Matrix<double>.Build.DenseOfArray(new double[,]
        { 
            {Ixx ,0 ,0},
		    {0, Iyy, 0},
		    {0, 0, Izz} 
        });
	 var MOI_b_inv = MOI_b.Inverse();
      
     
     // convert external torque vector3 to mathnet vector for type matching
     var Tb_ext_arr = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(new double[] { Tb_ext.x, Tb_ext.y, Tb_ext.z });

   
    
        double[] dxdt = new double[7];

        var q = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(new double[] { x[0], x[1], x[2], x[3]});
        var w = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense( new double[] { x[4], x[5], x[6]});

     //split up row-by-row
        dxdt[0] = 0.5 * (q[3] * w[0] + q[2] * w[1] - q[1] * w[2]);
        dxdt[1] = 0.5 * (q[3] * w[1] + q[0] * w[2] - q[2] * w[0]);
        dxdt[2] = 0.5 * (q[3] * w[2] + q[1] * w[0] - q[0] * w[1]);
        dxdt[3] = 0.5 * (-q[0] * w[0] - q[1] * w[1] - q[2] * w[2]);

        var wdot = MOI_b_inv * (Tb_ext_arr - Skew(w) * MOI_b * w);

        dxdt[4] = wdot[0];
        dxdt[5] = wdot[1];
        dxdt[6] = wdot[2];

         



    

        //simple euler integration
        double dt = 0.015; 
        double[] newx = new double[7];
        for (int i = 0; i < 7; i++)
        {
            newx[i] = x[i] + dxdt[i] * dt;
        }

     // normalize quaternions
     double qnorm = System.Math.Sqrt(newx[0]*newx[0] + newx[1]*newx[1] + newx[2]*newx[2] + newx[3]*newx[3]);

        newx[0] /= qnorm;
        newx[1] /= qnorm;
        newx[2] /= qnorm;
        newx[3] /= qnorm;

        return newx;
    }
}




