using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_TimeScale : MonoBehaviour
{
    public TMP_Text timeScaleText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //obv need to make this an event but im lazy
        timeScaleText.text = Simulation.timeScale.ToString(); //generates 32B of gorbage okay this is dogshite
    }
}
