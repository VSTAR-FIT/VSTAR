using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class ScreenUpdater : MonoBehaviour
{
    [SerializeField] public GameObject Canvas;
    [SerializeField] private Transform orionBody;
	[SerializeField] private Transform issBody;
    [SerializeField] private RotationalController RHC;
    [SerializeField] private TranslationalController THC;
    [SerializeField] private dynamicsModel dyn;
    [SerializeField] private Vector3 goalDeg = Vector3.zero;
    [SerializeField] private Vector2 rloc = Vector2.zero;
    [SerializeField] private Vector2 tloc = Vector2.zero;
    [SerializeField] private TMP_FontAsset font;
    private double[] position;
    private double[] rotation;


    private Vector3 orionBodyPosition;
    private Vector3 issBodyPosition;
    private Vector3 roterror;
    private Vector3 poserror;
    private Vector3 rcmd;
    private Vector3 tcmd;
    private Quaternion q;
    private Vector3 euler;
    
    TextMeshProUGUI tempt;
    TextMeshProUGUI tempr;
    private string ratesText;
	private string posText;

    
    
    void Start()
    {
        position = dyn.pos;
       
        rotation = dyn.rot;
        q = new Quaternion((float)rotation[0],(float)rotation[1],(float)rotation[2],(float)rotation[3]);
        euler = q.eulerAngles;
        rcmd = RHC.rateCmd;
        tcmd = THC.forceCmd;
        orionBodyPosition = orionBody.position;
        issBodyPosition = issBody.position;

        for (int i = 0 ; i<3 ; i++)
            roterror[i] = goalDeg[i]-(float)euler[i];

        for (int i =0; i<3; i++)
            poserror[i] = orionBodyPosition[i] - issBodyPosition[i];

        ratesText = "    CUR     CMD     ERROR    RATE \n R:  "+ euler.x + "    " + goalDeg[0] + "    " + roterror[0] + "    " + rotation[4] +"\n P:  "+ euler.y + "    " + goalDeg[1] + "    " + roterror[1] + "    " + rotation[5] +"\n Y:  "+ euler.z + "    " + goalDeg[2] + "    " + roterror[2] + "    " + rotation[6];

		posText = "@@ DP-DP POS@DP-DP VEL\n X: @" + poserror[0] +"@" + position[3] + "\n Y: @" + poserror[1] +"@" + position[4] + "\n Z: @" + poserror[2] +"@" + position[5]; 
		posText = posText.Replace("@", "   ");

        //**************************ROTATION DISPLAY******************************************
        // create text object and assign it to the canvas
        GameObject rtobj = new GameObject("Rotational Display"); 
        rtobj.transform.SetParent(Canvas.transform, false);

        //add the text
        tempr = rtobj.AddComponent<TextMeshProUGUI>();
        tempr.text = ratesText; 

        //font
        tempr.font = font;
        tempr.fontSize = 8;
        tempr.alignment = TextAlignmentOptions.TopLeft;
        tempr.enableWordWrapping = false;

        //position it using the rect transform
        RectTransform rectTransformR = rtobj.GetComponent<RectTransform>();
        rectTransformR.anchoredPosition = rloc; 
        rectTransformR.sizeDelta = new Vector2(600, 300); //set size (width/height)

        //anchor
        rectTransformR.anchorMin = new Vector2(0,1);
        rectTransformR.anchorMax = new Vector2(0,1);
        rectTransformR.pivot = new Vector2(0,1);

        //**************************TRANSLATION DISPLAY******************************************
        // create text object and assign it to the canvas
        GameObject ttobj = new GameObject("Translational Display"); 
        ttobj.transform.SetParent(Canvas.transform, false);

        //add the text
        tempt = ttobj.AddComponent<TextMeshProUGUI>();
        tempt.text = posText; 

        //font
        tempt.font = font;
        tempt.fontSize = 8;
        tempt.alignment = TextAlignmentOptions.TopLeft;
        tempt.enableWordWrapping = false;

        //position it using the rect transform
        RectTransform rectTransformT = ttobj.GetComponent<RectTransform>();
        rectTransformT.anchoredPosition = tloc; 
        rectTransformT.sizeDelta = new Vector2(600, 300); //set size (width/height)
        
        //amchor
        rectTransformT.anchorMin = new Vector2(0,1);
        rectTransformT.anchorMax = new Vector2(0,1);
        rectTransformT.pivot = new Vector2(0,1);

    }

    void FixedUpdate()
    {
        //update our values
        position = dyn.pos;
        rotation = dyn.rot;
        q = new Quaternion((float)rotation[0],(float)rotation[1],(float)rotation[2],(float)rotation[3]);
        euler = q.eulerAngles;

        rcmd = RHC.rateCmd;
        tcmd = THC.forceCmd;
        orionBodyPosition = orionBody.position;
        issBodyPosition = issBody.position;

        for (int i = 0 ; i<3 ; i++)
            roterror[i] = goalDeg[i]-(float)euler[i];

        for (int i =0; i<3; i++)
            poserror[i] = orionBodyPosition[i] - issBodyPosition[i];

         string Row(string label, float a, float b, float c, float d)
            {
                return string.Format("{0} {1,8:F2} {2,8:F2} {3,8:F2} {4,8:F2}\n",
                label, a, b, c, d);
            }
        ratesText =
            "      CUR       CMD      ERR     RATE\n" +
            Row("R", euler.x, goalDeg[0], roterror[0], (float)rotation[4]) +
            Row("P", euler.z, goalDeg[1], roterror[2], (float)rotation[6]) +
            Row("Y", euler.y, goalDeg[2], roterror[1], (float)rotation[5]);

        posText =
            "      DP-POS      DP-VEL\n" +
            string.Format("X {0,10:F3} {1,10:F3}\n", -poserror[0], position[3]) +
            string.Format("Y {0,10:F3} {1,10:F3}\n", poserror[1], position[4]) +
            string.Format("Z {0,10:F3} {1,10:F3}", poserror[2], position[5]);





        //edit display texts
        tempr.text = ratesText;
        tempt.text = posText;
    }
}