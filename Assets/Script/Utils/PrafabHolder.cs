using DuloGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrafabHolder : MonoBehaviour
{
    [Header("Bullet")]
    public GameObject Bullet;
    public GameObject OrbBullet;
    public BulletEnhancement defaultBulletEnhancement;
    public UISpellDatabase skillData;

    [SerializeField]
    private StringGameObjectDictionary bulletStore = StringGameObjectDictionary.New<StringGameObjectDictionary>();
    private Dictionary<string, GameObject> bulletStringObject {
        get { return bulletStore.dictionary; }
    }

    [SerializeField]
    private StringGameObjectDictionary bulletImpectStore = StringGameObjectDictionary.New<StringGameObjectDictionary>();
    private Dictionary<string, GameObject> bulletImpectStringObject
    {
        get { return bulletImpectStore.dictionary; }
    }

    [SerializeField]
    private StringGameObjectDictionary skillStore = StringGameObjectDictionary.New<StringGameObjectDictionary>();
    private Dictionary<string, GameObject> skillStringObject
    {
        get { return skillStore.dictionary; }
    }

    [SerializeField]
    private StringGameObjectDictionary buffStore = StringGameObjectDictionary.New<StringGameObjectDictionary>();
    private Dictionary<string, GameObject> buffStringObject
    {
        get { return buffStore.dictionary; }
    }

    [SerializeField]
    private StringGameObjectDictionary effectStore = StringGameObjectDictionary.New<StringGameObjectDictionary>();
    private Dictionary<string, GameObject> effectStringObject
    {
        get { return effectStore.dictionary; }
    }

    [SerializeField]
    private StringShaderDictionary shaderStore = StringShaderDictionary.New<StringShaderDictionary>();
    private Dictionary<string, Shader> shaderStringShader
    {
        get { return shaderStore.dictionary; }
    }

    public GameObject GetBullet(string name, BulletSize size = BulletSize.Small) {
        GameObject value;
        bool exist = bulletStore.dictionary.TryGetValue(name + size, out value);
        return exist ? value : null;
    }

    public GameObject GetBulletImpect(string name, BulletSize size = BulletSize.Small)
    {
        GameObject value;
        bool exist = bulletImpectStore.dictionary.TryGetValue(name + size, out value);
        return exist ? value : null;
    }

    public GameObject GetBuff(string name)
    {
        GameObject value;
        bool exist = buffStore.dictionary.TryGetValue(name, out value);
        return exist ? value : null;
    }

    public GameObject GetSkill(string name)
    {
        GameObject value;
        bool exist = skillStore.dictionary.TryGetValue(name, out value);
        return exist ? value : null;
    }

    public GameObject GetEffect(string name)
    {
        GameObject value;
        bool exist = effectStore.dictionary.TryGetValue(name, out value);
        return exist ? value : null;
    }

    public Shader GetShader(string name)
    {
        Shader value;
        bool exist = shaderStore.dictionary.TryGetValue(name, out value);
        return exist ? value : null;
    }

    public GameObject Skillholder;
    public GameObject OrbSkillholder;
    public GameObject ColliderHolder;
    public GameObject SkillHolderBuff;
    public GameObject SprayColliderBullet;

    [Header("Common")]
    public GameObject attackTargetIndicator;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
