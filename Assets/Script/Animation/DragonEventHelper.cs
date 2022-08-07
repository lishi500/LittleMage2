using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonEventHelper : SimpleEventHelper
{
    // Start is called before the first frame update
    public delegate void PlayEndDelegate();
    public event PlayEndDelegate notifyPlayEnd;

    public void OnPlayTrigger(string state)
    {
        if (notifyPlayEnd != null && state == "End")
        {
            notifyPlayEnd();
        }
    }

    public void Eyes() {
        //Debug.Log("Eyes event");
    }

}
