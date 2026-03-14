using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ScreenUpdater : Monobehavior
{
    [SerializeField] private Transform orionBody;
	[SerializeField] private Transform issBody;
    [SerializeField] private RotationalController RHC;
    [SerializeField] private TranslationalController THC;
    [SerializeField] private dynamicsModel dyn;
    [SerializeField] private Vector3 goalDeg = [0,0,0];
    private double[] position;
    private double[] rotation;

    private Vector3 rcmd;
    private Vector3 tcmd;

    

    private string ratesText;
	private string posText;


    void Start()
    {
        position = dyn.pos;
        rotation = dyn.rot;
        rcmd = RHC.rateCmd;
        tcmd = THC.forceCmd;

        ratesText = "    CUR     CMD     ERROR    RATE /n R:  "+ rotation[0] + "    " + goalDeg[0] + "    " + goalDeg[0]-rotation[0] + "    " + rotation[3] +"/n P:  "+ rotation[1] + "    " + goalDeg[1] + "    " + goalDeg[1]-rotation[1] + "    " + rotation[4] +"/n Y:  "+ rotation[2] + "    " + goalDeg[2] + "    " + goalDeg[2]-rotation[2] + "    " + rotation[5];
        ratesText = ratesText.Replace("/n", System.Environment.Newline);

		posText = "@@ DP-DP POS@DP-DP VEL/n X: @" + issBody[0] -orionBody.position[0] +"@" + position[3] + "/n Y: @" + issBody[1] -orionBody.position[1] +"@" + position[4] + "/n Z: @" + issBody[2] -orionBody.position[2] +"@" + position[5]; 
		posText = posText.Replace("/n", System.Environment.Newline);
		posText = posText.Replace("@", "   ");
        //create render texture for camera and place it 

    }

    void FixedUpdate()
    {

    }
}