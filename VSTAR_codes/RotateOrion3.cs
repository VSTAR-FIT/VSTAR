
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using DotNumerics.ODE;






public class RotateOrion3
{
	//initialize the inertia matrix, take inverse (need for state derivative) MOIS ARE PLACEHOLDER
    private const double Ixx = 1 ; 
    private const double Iyy = 1 ;
    private const double Izz = 1 ;
    
    private readonly Matrix<double> MOI_b ;
    private readonly Matrix<double> MOI_b_inv ;
      
      //define xi, which is used for qdot
    public static Matrix<double> Xi(MathNet.Numerics.LinearAlgebra.Vector<double> q)  
        {
            double q1 = q[0];
            double q2 = q[1];
            double q3 = q[2];
            double q4 = q[3];

            return Matrix<double>.Build.DenseOfArray(new double[,]
            {

            {  q4, -q3,  q2 },
            {  q3,  q4, -q1 },
            { -q2,  q1,  q4 },
            { -q1, -q2, -q3 }

             });
        }

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
 public double[] Rotate3(double[] x, Vector3 Tb_ext )
    {
    var MOI_b = Matrix<double>.Build.DenseOfArray(new double[,]
        { 
            {Ixx ,0 ,0},
		    {0, Iyy, 0},
		    {0, 0, Izz} 
        });
	var MOI_b_inv = MOI_b.Inverse();
      
     

     //make a mathnet version of x, so types match for matrix calculations
     var x_vec = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(x);

     // convert external torque vector3 to mathnet vector for type matching
     var Tb_ext_arr = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(new double[] { Tb_ext.X, Tb_ext.Y, Tb_ext.Z });

        
	
     var zero4x4 = Matrix<double>.Build.Dense(4,4); // 4x4 matrix of zeroes
     var zero4x3 = Matrix<double>.Build.Dense(4,3); // 4x3 matrix of zeroes


     

     double[] xdot(double t, double[] x)
        {
            double[] dxdt = new double[7]; // empty xdot array

            // pull quaternions from state vector (first four entries) 
            var q = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(x[0..4]);

            // pull angular velocity from state vector (last three entries)
            var w = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(x[4..7]);


            var qdot = 0.5 * Xi(q) * w; // quaternion derivative
            var wdot = MOI_b_inv * (Tb_ext_arr - Skew(w) * MOI_b * w); // angular acceleration

            for (int i = 0; i < 4; i++) //fill in qdot as first four entries of xdot
            {
                dxdt[i] = qdot[i];
            }

            for (int i = 0; i < 3; i++) //fill in wdot for last three entries of xdot
            {
                dxdt[i + 4] = wdot[i];
            }
            return dxdt;
        };
       




		//now we get to integrating - this uses DotNumerics runge-kutta fourth/fifth order

		double t0 = 0; //integrator start time
		double tf = 0.015; //integrator end time
        
      
        var rk45 = new OdeExplicitRungeKutta45(xdot, 7);
        OdeSolution sol = rk45.Solve(x, t0, tf); //perform integration
        double[] newx = sol.Y[sol.Y.Length - 1]; //get final state vector from integrator
        return newx;
    }
}




