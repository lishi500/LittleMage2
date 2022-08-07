using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;

public class TextUtils : MonoBehaviour
{
    private static bl_HUDText HUDRoot;
    //[SerializeField] private GameObject TextPrefab;
    static int NORMAL_SIZE = 20;
    static int CRITICAL_SIZE = 30;
    private const float UPDATE_INTERVAL = 0.2f;
    private float pastInterval = 0f;

    Dictionary<int, Queue<DamageDef>> damageList;
    Dictionary<int, Transform> transformPool;

    void Update() {
        pastInterval += Time.deltaTime;
        if (damageList.Count > 0 && pastInterval >= UPDATE_INTERVAL) {
            pastInterval = 0;
            ProcessText();
        }
    }
    void Start()
    {
        HUDRoot = bl_UHTUtils.GetHUDText;
        damageList = new Dictionary<int, Queue<DamageDef>>();
        transformPool = new Dictionary<int, Transform>();
    }

    public void ProcessText() {
        List<int> keyList = new List<int>(damageList.Keys);

        foreach (int key in keyList)
        {
            Queue<DamageDef> damageQueues = damageList[key];
            DamageDef damage = damageQueues.Dequeue();
            Transform objTransform;
            transformPool.TryGetValue(key, out objTransform);

            if (objTransform != null && damage != null) {
                if (damage.type == DamageType.HEAL)
                {
                    HealText(objTransform, damage.damage, damage.isCritical);
                }
                else {
                    DamageText(objTransform, damage.damage, damage.isCritical, damage.type, damageQueues.Count);
                }
            }

            if (damageQueues.Count == 0) {
                RemoveDamgeEntry(key);
            }
            
        }
    }
    private void RemoveDamgeEntry(int key) {
        damageList.Remove(key);
        transformPool.Remove(key);
    }
    public void AddDamageEntry(Transform objTransform, DamageDef damageDef) {
        int id = objTransform.GetInstanceID();
        if (transformPool.ContainsKey(id) && damageList.ContainsKey(id))
        {
            Queue<DamageDef> damageQueues;
            damageList.TryGetValue(id, out damageQueues);
            if (damageQueues != null)
            {
                damageQueues.Enqueue(damageDef);
            }
        }
        else {
            Queue<DamageDef> damageQueues = new Queue<DamageDef>();
            damageQueues.Enqueue(damageDef);

            damageList.Add(id, damageQueues);
            transformPool.Add(id, objTransform);
        }
    }

    public static void HealText(Transform transformObj, float amount, bool isCritical) {
        HUDTextInfo info = new HUDTextInfo(transformObj, string.Format("+{0}", ((int)amount).ToString()));
        info.Size = GetSize(isCritical);
        info.Color = Color.green;
        info.VerticalPositionOffset = 3 + Random.Range(-1f, 1f);
        info.FadeSpeed = 500;

        WriteText(info);
    }

    public static void DamageText(Transform transformObj, float amount, bool isCritical, DamageType type = DamageType.NORMAL, int RemaingCount = 0) {
        HUDTextInfo info = new HUDTextInfo(transformObj, string.Format("-{0}", ((int)amount).ToString()));
        info.Size = GetSize(isCritical);
        info.Color = GetColor(type, isCritical);
        info.VerticalPositionOffset = 3 + Random.Range(-RemaingCount, RemaingCount);
        info.FadeSpeed = 500;
        //Debug.Log("remain " + RemaingCount + " " + info.VerticalPositionOffset);

        WriteText(info);
    }


    private static int GetSize(bool isCritical) {
        if (isCritical)
        {
            return CRITICAL_SIZE + Random.Range(5, 10);
        }
        else {
            return NORMAL_SIZE + Random.Range(-5, 3);
        }
    }

    private static Color GetColor(DamageType type, bool isCritical) {
        Color textColor = Color.white;
        switch (type) {
            case DamageType.FIRE:
                textColor = ToColor(199, 72, 18);
                break;
            case DamageType.ICE:
                textColor = ToColor(39, 169, 209);
                break;
            case DamageType.ELETRIC:
                textColor = ToColor(238, 245, 37);
                break;
            case DamageType.NORMAL:
                textColor = isCritical ? ToColor(250, 0, 63) : ToColor(255, 255, 255);
                break;
            case DamageType.HOLY:
                textColor = ToColor(247, 232, 19);
                break;
            case DamageType.HEAL:
                textColor = ToColor(31, 242, 95);
                break;
            case DamageType.SHIELD:
                textColor = ToColor(178, 230, 237);
                break;
            case DamageType.DARK:
                textColor = ToColor(71, 35, 138);
                break;
        }

        if (isCritical) {
            float H, S, V;

            Color.RGBToHSV(textColor, out H, out S, out V);
            return Color.HSVToRGB(H , S , V * 1.4f);
        }

        return textColor;
    }

    private static Color ToColor(int r, int g, int b, float a = 1) {
        return new Color(r / 255f, g / 255f, b / 255f, a);
    }
    //private Color Darken() {

    //}


    private static void WriteText(HUDTextInfo info) {
        if (HUDRoot == null)
        {
            HUDRoot = bl_UHTUtils.GetHUDText;
        }
        HUDRoot.NewText(info);
    }
}
