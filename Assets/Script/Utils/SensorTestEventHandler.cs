using SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorTestEventHandler : MonoBehaviour
{
    public void OnEyesDetected(GameObject gameobject) {
        Debug.Log("Eyes Detected");
        Debug.Log(gameobject);
        
    }

    private void Update()
    {
        TriggerSensor sensor = GetComponent<TriggerSensor>();
    }
}
