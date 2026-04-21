using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class frameticker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
float timer;
void Update()
{

  timer += Time.deltaTime;

    if (timer >= 1.0f)
    {
        float fps = 1.0f / Time.unscaledDeltaTime;
        Debug.Log($"FPS: {fps:0.}");
        timer = 0f;
}
void OnGUI()
{
    float fps = 1.0f / Time.unscaledDeltaTime;
    GUI.Label(new Rect(10, 10, 200, 40), $"FPS: {fps:0.}");
}
}
}