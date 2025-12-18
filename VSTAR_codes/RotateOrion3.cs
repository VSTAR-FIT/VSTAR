using System;
using System.Numerics;
using MathNet.Numerics.OdeSolvers;
using MathNet.Numerics.LinearAlgebra;

// undefined variables for mass properties I have not put in yet have fixed values




public class RotateOrion3
{
	//define the inertia matrix, take inverse (need for state derivative) 
    private Matrix<double> MOI_b = Matrix<double>.Build.DenseOfArray(new double[,]
    { 
        {Ixx ,0 ,0},
		{0, Iyy, 0},
		{0, 0, Izz} 
    }
	);
	private Matrix<double> MOI_b_inv = MOI_b.Inverse();




   
 public double[,] Rotate3(double[] x, Vector3 Tb_ext )
    {

      
     // pull quaternions from state vector (first four entries) 
     private MathNet.Numerics.LinearAlgebra.Vector<double> q = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(x[0..4]);

      

     // pull angular velocity from state vector (last three entries)
     private MathNet.Numerics.LinearAlgebra.Vector<double> w = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(x[4..7]);

     // convert external torque vector to vector (we need it for the martix calculation later)
     private MathNet.Numerics.LinearAlgebra.Vector<double> Tb_ext_arr = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(new double[] { Tb_ext.X, Tb_ext.Y, Tb_ext.Z });

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

    
        var zero4x4 = Matrix<double>.Build.Dense(4,4); // 4 square matrix of zeroes
        var zero4x3 = Matrix<double>.Build.Dense(4,3); // 4x3 matrix of zeroes

        //create plant matrix
        Matrix<double> A = Matrix<double>.Build.DenseOfArray(new double[,]
        {
            {zero4x4 , Xi(q)},
            {zero4x3 , -MOI_b_inv*Vector3.Cross(w,MOI_b)}
        });

        //create torque matrix
        Matrix<double> B = Matrix<double>.Build.DenseOfArray(new double[,]
        {
            {zero4x3},
            {MOI_b_inv}
        });


		//define state derivative
        double[,] xdot = A * x + B * Tb_ext_arr;



		//now we get to integrating - this uses DotNumerics runge-kutta fourth/fifth order
		

		double t0 = 0; //integrator start time
		double tf = 0.02; //integrator end time
		double N = 100; //integrator step #

		double[,] newx = rk4.Integrate(xdot, t0, tf, N, x); //bam pow simulation

		q = newx[N, 0..3]; //grab first four elements of the final results and store them as quaternions
		w = newx[N, 4..]; //grab remaining elements and store them as angular velocity
		return (q, w);

    }
}




