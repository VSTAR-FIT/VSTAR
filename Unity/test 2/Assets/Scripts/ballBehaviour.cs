using UnityEngine;

public class ballBehaviour : MonoBehaviour
{
    [SerializeField] public Transform ball;
    [SerializeField] private dynamicsModel dyn;
    private double[] rota = new double[6];

    void FixedUpdate()
    {
        rota = dyn.rot;
        ball.rotation =new Quaternion((float)rota[0], (float)rota[1], (float)rota[2], (float)rota[3]);

        //literally just rotate the ball in the same way as the s/c 
    }
}