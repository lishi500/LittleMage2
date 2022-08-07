using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderChanger : MonoBehaviour
{
    // Start is called before the first frame update

    public Shader newShader;

    public float timeToChangeBack;
    private float timePast;
    public bool willChangeBack = true;

    [HideInInspector]
    public Renderer[] renderers;
    [HideInInspector]
    public Shader originalShader;

    public virtual void ApplyNewMaterial() {
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            foreach (Material material in materials)
            {
                material.shader = newShader;
            }
        }
    }

    public virtual void ChangeBackToOriginal()
    {
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            foreach (Material material in materials)
            {
                material.shader = originalShader;
            }
        }
    }

    public virtual void RecordCurrentMaterials() {
        renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0) {
            Material[] originalMats = renderers[0].materials;
            if (originalMats.Length > 0) {
                originalShader = originalMats[0].shader;
            }
        }

    }
    public virtual void Start()
    {
        RecordCurrentMaterials();
        ApplyNewMaterial();
    }

    public virtual void Update() {
        if (timeToChangeBack != 0) {
            timePast += Time.deltaTime;
            if (timePast >= timeToChangeBack && willChangeBack) {
                ChangeBackToOriginal();
                Destroy(GetComponent<ShaderChanger>());
            }
        }
    }
}
