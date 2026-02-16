using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public class TranslateOrion3 : MonoBehaviour
{
    public double[] Translate3(double[] x, UnityEngine.Vector3 F_ext, double mass)
    {
       var x = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(x);

       var a = F_ext / mass;
       var dt = 0.2;

        newx = new double[] 
        {
       
            x(3) *dt,
            x(4) *dt,
            x(5) *dt,
            a(0) *dt,
            a(1) *dt,
            a(2) *dt,
       
        };
       return newx;
    }
}