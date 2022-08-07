using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailPoint : MonoBehaviour
{
    public TrailPointType trailType;
    public bool defaultOn;
    private bool isTrailOn;


    public bool IsTrailOn() {
        return isTrailOn;
    }

    public void StartTrail() {
        isTrailOn = true;
        foreach (ParticleSystem ps in GetAllTrails()) {
            ps.Play();
        }
    }

    public void StopTrail() {
        isTrailOn = false;
        foreach (ParticleSystem ps in GetAllTrails())
        {
            ps.Stop();
        }
    }

    public void AddTrail(GameObject trailPrafab) {
        GameObject trailObject = Instantiate(trailPrafab, transform.parent);
        trailObject.transform.rotation = transform.rotation;
    }

    public void DestroyTrail() {
        foreach (ParticleSystem ps in GetAllTrails())
        {
            Destroy(ps);
        }
    }

    private ParticleSystem[] GetAllTrails() {
        return transform.parent.GetComponentsInChildren<ParticleSystem>();
    }
 
    void Start()
    {
        isTrailOn = false;
   
        if (defaultOn)
        {
            StartTrail();
        }
        else {
            StopTrail();
        }
    }
}
