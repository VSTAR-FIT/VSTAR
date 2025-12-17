using System;
using System.Numerics;
using MathNet.Numerics.OdeSolvers;
using MathNet.Numerics.LinearAlgebra;

// undefined variables for mass properties I have not put in yet have fixed values


//shortcutting mathnet numerics' matrix and vector builders for sanity
var M = Matrix<double>.Build;
var V = Vector<double>.Biuld;
public class RotateOrion3
{
	//define the inertia matrix, take inverse (need for state derivative) 
    private var MOI_b = M.DenseofArray(new double[,]
    { 
        {Ixx ,0 ,0},
		{0, Iyy, 0},
		{0, 0, Izz} 
    }
	);
	private double MOI_b_inv = Invert(MOI_b);



	//initialize angular speed vector, quaternion array, and torque matrix (will be used as inputs for the RotateOrion3 function)
	public var w = V.DenseofArray( new double[,] {wx, wy, wz});
	public double[] q = new double[] {q1, q2, q3, q4};
    public var Tb_ext = V.DenseofArray(new double[,] {Tx, Ty, Tz}); // T for torque :3 (values will be determined from stick position input)


	//define xi, which is used for qdot
	public static Matrix<double> Xi(double[] q)  
    {
        double q1 = q[0];
        double q2 = q[1];
        double q3 = q[2];
        double q4 = q[3];

        return M.DenseofArray(new double[,]
        {
            {  q4, -q3,  q2 },
            {  q3,  q4, -q1 },
            { -q2,  q1,  q4 },
            { -q1, -q2, -q3 }
        });
    }

   
	public RotateOrion3(double[,] q, Vector3 w, Vector3 Tb_ext )
    {


        double x = {q , w}; //q is 4x1 and w is 3x1 which makes this a 7x1 array

        double zero4x4 = new double[4,4]; // 4 square matrix of zeroes
        double zero4x3 = new double[4,3]; // 4x3 matrix of zeroes

        //create plant matrix
        double A =
        {
            {zero4x4 , 0.5*Xi(q)},
            {zero4x3 , -MOI_b_inv*Vector3.Cross(w,MOI_b)}
        };

        //create torque matrix
        double B =
        {
            {zero4x3},
            {MOI_b_inv}
        };


		//define state derivative
        double xdot = A * x + B * Tb_ext;



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




