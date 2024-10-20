using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideShowTutorial : MonoBehaviour
{
    GameObject childObj;
    // Start is called before the first frame update
    void Start()
    {
        childObj = this.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) {
            childObj.SetActive(!childObj.activeInHierarchy);
        }
    }
}
