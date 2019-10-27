using UnityEngine;
using UnityEngine.UI;
 
using UnityEditor;
 
// ---------------
//  String => Int
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringIntDictionary))]
public class StringIntDictionaryDrawer : SerializableDictionaryDrawer<string, int> {
    protected override SerializableKeyValueTemplate<string, int> GetTemplate() {
        return GetGenericTemplate<SerializableStringIntTemplate>();
    }
}
internal class SerializableStringIntTemplate : SerializableKeyValueTemplate<string, int> {}
 
// ---------------
//  GameObject => Float
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(GameObjectFloatDictionary))]
public class GameObjectFloatDictionaryDrawer : SerializableDictionaryDrawer<GameObject, float> {
    protected override SerializableKeyValueTemplate<GameObject, float> GetTemplate() {
        return GetGenericTemplate<SerializableGameObjectFloatTemplate>();
    }
}
internal class SerializableGameObjectFloatTemplate : SerializableKeyValueTemplate<GameObject, float> {}


// ---------------
//  String => GameObject
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringGameObjectDictionary))]
public class StringGameObjectDictionaryDrawer : SerializableDictionaryDrawer<string, GameObject>
{
    protected override SerializableKeyValueTemplate<string, GameObject> GetTemplate()
    {
        return GetGenericTemplate<SerializableStringGameObjectTemplate>();
    }
}
internal class SerializableStringGameObjectTemplate : SerializableKeyValueTemplate<string, GameObject> { }


// ---------------
//  String => Shader
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringShaderDictionary))]
public class StringShaderDictionaryDrawer : SerializableDictionaryDrawer<string, Shader>
{
    protected override SerializableKeyValueTemplate<string, Shader> GetTemplate()
    {
        return GetGenericTemplate<SerializableStringShaderTemplate>();
    }
}
internal class SerializableStringShaderTemplate : SerializableKeyValueTemplate<string, Shader> { }
