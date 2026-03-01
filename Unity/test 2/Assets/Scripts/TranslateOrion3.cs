using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
using System.Numerics;

public class TranslateOrion3 : MonoBehaviour
{
    public double[] Translate3(double[] x, UnityEngine.Vector3 F_ext, float mass)
    {
       var pos0 = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(x);

       var a = F_ext / mass;
       var dt = 0.2;

        double[] newx = new double[]
        {
         pos0[0] + pos0[3] * dt,
         pos0[1] + pos0[4] * dt,
         pos0[2] + pos0[5] * dt,
         pos0[3] + a.x * dt,
         pos0[4] + a.y * dt,
         pos0[5] + a.z * dt
};
       return newx;
    }
}