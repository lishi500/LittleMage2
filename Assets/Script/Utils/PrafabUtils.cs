using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrafabUtils : MonoBehaviour {

    private static PrafabUtils _instance;
    public static PrafabUtils Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public GameObject create(GameObject obj)
    {
        return (GameObject)Object.Instantiate(obj);
    }

    public GameObject create(string path)
    {
       return (GameObject)Object.Instantiate(Resources.Load("Prafabs/" + path));
    }

    public GameObject create(GameObject obj, Vector3 position, GameObject parentObj ) {

        GameObject prafabObj = Object.Instantiate(obj, position, Quaternion.identity);
        Vector3 scale = prafabObj.transform.localScale;
        prafabObj.transform.SetParent(parentObj.transform);
        prafabObj.transform.localScale = scale;

        return prafabObj;
    }

   

    public GameObject create(string path, Vector3 position, GameObject parentObj)
    {

        GameObject prafabObj = (GameObject)Object.Instantiate(Resources.Load("Prafabs/" + path), position, Quaternion.identity);
        if (parentObj != null) {
            Vector3 scale = prafabObj.transform.localScale;
            prafabObj.transform.SetParent(parentObj.transform);
            prafabObj.transform.localScale = scale;

        }
        return prafabObj;
    }

}
