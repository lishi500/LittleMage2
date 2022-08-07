using DuloGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectSkillHandler : MonoBehaviour
{

    public delegate void LootSkillSelectDelegate(UISpellInfo spellInfo);
    public event LootSkillSelectDelegate notifyLootSkillSelect;

    public void OnLootSkillClicked() {
        UISpellSlot spellSlot = GetComponent<UISpellSlot>();
        if (spellSlot != null && notifyLootSkillSelect != null) {
            notifyLootSkillSelect(spellSlot.GetSpellInfo());
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
