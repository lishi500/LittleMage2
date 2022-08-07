using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshEffectSwitcher : MonoBehaviour
{
    public void AddMeshEffect(GameObject itemObj, GameObject meshEffect)
    {
        var currentInstance = Instantiate(meshEffect) as GameObject;
        currentInstance.transform.SetParent(itemObj.transform);
        PSMeshRendererUpdater psUpdater = currentInstance.GetComponent<PSMeshRendererUpdater>();
        psUpdater.FadeTime = 0f;
        psUpdater.StartScaleMultiplier = 0;
        psUpdater.UpdateMeshEffect(itemObj);
    }

    public void RemoveMeshEffect(GameObject itemObj) {
        PSMeshRendererUpdater psUpdater = itemObj.GetComponentInChildren<PSMeshRendererUpdater>();
        Destroy(psUpdater.gameObject);
    }
  
}
