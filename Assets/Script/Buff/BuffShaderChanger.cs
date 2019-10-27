using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuffShaderChanger 
{
    public Shader newShader;
    public float timeToChangeBack;
    public bool willChangeBack = true;
}
