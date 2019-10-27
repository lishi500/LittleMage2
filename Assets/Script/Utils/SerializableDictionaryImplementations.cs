using System;
 
using UnityEngine;
 
// ---------------
//  String => Int
// ---------------
[Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> {}

// ---------------
//  String => GameObject
// ---------------
[Serializable]
public class StringGameObjectDictionary : SerializableDictionary<string, GameObject> { }

// ---------------
//  GameObject => Float
// ---------------
[Serializable]
public class GameObjectFloatDictionary : SerializableDictionary<GameObject, float> {}

// ---------------
//  String => Shader
// ---------------
[Serializable]
public class StringShaderDictionary : SerializableDictionary<string, Shader> { }