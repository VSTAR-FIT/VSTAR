using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
public class dockingchecker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private dynamicsModel dyn;
    [SerializeField] public GameObject Canvas;
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private Transform orionNose; //docking hatch on orion  
    [SerializeField] private Transform issDock; // docking hatch on ISS
    [SerializeField] private Transform orionBody; // Orion's body (for collision checking)
    [SerializeField] private Transform issBody; //iSS body for collision checking

    [Header("Sim Parameters")]
    [SerializeField] private float posAccuracy = 0.02f;
    [SerializeField] private float rotAccuracy = 2f;
    [SerializeField] private float rateaccuracy = 0.01f;
    [SerializeField] private Vector2 cloc = new Vector2(300f,300f); //where text is positioned

    private Vector3 approachDir = new Vector3(1f,0f,0f);
    private Vector3 noseDir;

    private Quaternion q;
    private Quaternion qerror;
    private float eulererror;

    private Vector3 eulerrate;

    private Vector3 poserror;
    private Vector3 posrate;

    private bool aligncheck = false;
    private bool poscheck = false;
    private bool rotcheck = false;
    private bool ratecheck = false;

    private bool fail;
    public bool endSim;
    private bool uicreated;
 
    private float starttime;
    TextMeshProUGUI ENDSTATE;
    
    private string FAIL = "FAILURE \n TRY AGAIN";
    private string WIN = "SUCCESS \n good job :-)";




void Awake()
{
    Debug.Log("Awake endSim = " + endSim);
    poserror=orionNose.position - issDock.position;
        Debug.Log("Initial distance at awake: " + poserror.magnitude);
}

    // Start is called before the first frame update

    void Start()
    {
        Debug.Log("start!");
        Debug.Log("Start endSim = " + endSim);
        starttime = Time.time;
        endSim = false;
        fail = false;
        uicreated=false;

        poserror=orionNose.position - issDock.position;
        Debug.Log("Initial distance at start: " + poserror.magnitude);
        approachDir = issDock.forward;

    }

   
    float timer;
    void Update()
    {   //ignore first second
        if (Time.time < starttime + 1f) return;
       // Debug.Log("Endsim:" + endSim);
       // Debug.Log("Uicreated" + uicreated);

      // timer += Time.deltaTime;

    
        if(endSim && !uicreated)
        {
            CreateEndScreen();
            uicreated = true;
        };
        if(endSim)return;
        noseDir = orionNose.forward;
        //are we in the right position? -> are we in the right attitude? -> are we spinning?
        float alignment = Vector3.Dot(noseDir, approachDir);

        eulererror = Quaternion.Angle(orionNose.rotation, issDock.rotation);
        poserror=orionNose.position - issDock.position;
       // Debug.Log(" distance at update: " + poserror.magnitude);
        eulerrate = new Vector3((float)dyn.rot[4], (float)dyn.rot[5], (float)dyn.rot[6]);
        posrate = new Vector3((float)dyn.pos[3], (float)dyn.pos[4], (float)dyn.pos[5]);


     
        if(alignment >= 0.9f) //aligned with docking port?
        {
            aligncheck = true;
        }
        else{aligncheck = false;};


        if(poserror.magnitude <= posAccuracy)//check if at dock position
        {
            poscheck = true;
           // Debug.Log("Position reached");
                

        }
            
        if(eulererror <= rotAccuracy) //check if at dock attitude
        {
            rotcheck = true;
           // Debug.Log("Rotation OK");
                
                
        }
        else
        {
            rotcheck = false;
                
        };
        if(eulerrate.magnitude <= rateaccuracy && posrate.magnitude <=rateaccuracy) //check if rates ok
        {
            ratecheck = true;
           // Debug.Log("Rates OK");
                
        }
        else
        {
            ratecheck = false;
           // Debug.Log("FAIL: velocity");
                
        }
        

        
        
            if(poscheck ==true &&aligncheck == true && rotcheck==true && ratecheck ==true && !endSim)
            {
                endSim = true;
                fail = false;
                Debug.Log(" endSim set TRUE at: " + Time.frameCount);
            }
            //else{endSim = true;fail = true;Debug.Log(" endSim set TRUE at: " + Time.frameCount);}
        
           if (timer >= 1.0f)
             {
                 Debug.Log($"pos:{poscheck} align:{aligncheck} rot:{rotcheck} rate:{ratecheck}");
                 Debug.Log("Distance: " + poserror.magnitude);
                 Debug.Log("Alignment: " + alignment);
                 Debug.Log("eulererror: " + eulererror);
        

                timer = 0f;
            }

    } 
        
    void CreateEndScreen() //make endscreen
    {
        GameObject canvobj = new GameObject("Rotational Display"); 
        canvobj.transform.SetParent(Canvas.transform, false);

         //add the text
        ENDSTATE = canvobj.AddComponent<TextMeshProUGUI>();
        if(!fail)
         {
         ENDSTATE.text = WIN; 
         ENDSTATE.color = Color.green;
         }
         else
        {
           ENDSTATE.text = FAIL;
           ENDSTATE.color = Color.red;  
         }

            //font
            ENDSTATE.font = font;
            ENDSTATE.fontSize = 200;
            ENDSTATE.alignment = TextAlignmentOptions.TopLeft;
            ENDSTATE.enableWordWrapping = false;

            //position it using the rect transform
            RectTransform rectTransformC = canvobj.GetComponent<RectTransform>();
            rectTransformC.anchoredPosition = cloc; 
            rectTransformC.sizeDelta = new Vector2(600, 600); //set size (width/height)

            //anchor
            rectTransformC.anchorMin = new Vector2(0,1);
            rectTransformC.anchorMax = new Vector2(0,1);
            rectTransformC.pivot = new Vector2(0,1);
        }
}


    

