using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthRotate : MonoBehaviour
{
    [SerializeField] private Transform earth;
    [SerializeField] private float rotationPerFrame = 0.0129032258f;
    // Start is called before the first frame update
    private Quaternion rot1;
    private Vector3 tempAngles;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rot1 = earth.rotation;
        tempAngles = rot1.eulerAngles;
        tempAngles.y = tempAngles.y + rotationPerFrame;
        earth.rotation = Quaternion.Euler(tempAngles);
    }
}
