using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeToLive = 3.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    public void DestoryNow() {
        Destroy(gameObject);
    }
}
