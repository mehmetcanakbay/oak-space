using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public double starterTimeScale;
    public double timeScaleStepSize;
    // Start is called before the first frame update
    void Awake()
    {
        Simulation.timeScale = starterTimeScale;  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) {
            Simulation.timeScale /= timeScaleStepSize;
        } 

        if (Input.GetKeyDown(KeyCode.N)) {
            Simulation.timeScale *= timeScaleStepSize;
        }

    }
}
