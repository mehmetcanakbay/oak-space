using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UI_HawkingRadiationInfo : MonoBehaviour
{
    public TMP_Text massText;
    public TMP_Text lumText;
    public TMP_Text evapRateText;
    public TMP_Text tempText;

    void Start()
    {
    }

    void OnEnable() {
        EventManager.AddListener<HawkingRadiationInfoEvent>(ParseIncomingEventInput);
    }

    void OnDisable() {
        EventManager.RemoveListener<HawkingRadiationInfoEvent>(ParseIncomingEventInput);
    }

    void ParseIncomingEventInput(HawkingRadiationInfoEvent evt) {
        massText.text = evt.Mass.ToString();
        lumText.text = evt.Luminosity.ToString();
        evapRateText.text = evt.EvapRate.ToString();
        tempText.text = evt.Temp.ToString();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
