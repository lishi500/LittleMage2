using DuloGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillController : MonoBehaviour
{
    public GameObject[] spellSlots;
    //public UISpellDatabase skillData;
    PrafabHolder prafabHolder;
    Player player;
    private bool shouldFlush = false;

    public void UpdateSkillSlots(List<Skill> skills) {
        for (int i = 0; i < skills.Count; i++) {
            Skill skill = skills[i];
            int id = skill.skillId;
        }
    }
    public void FlushSpellSlot() {
        //Debug.Log("FlushSpellSlot");
        shouldFlush = true;
    }

    private void Flush() {
        Skill[] skills = player.GetTopOrbSkillsOrderByCDLedt(3);
        for (int i = 0; i < skills.Length; i++)
        {
            spellSlots[i].SetActive(true);
            ReplaceSlotSkill(skills[i], i);
        }

        for (int i = skills.Length; i < spellSlots.Length; i++)
        {
            spellSlots[i].SetActive(false);
        }
        shouldFlush = false;
    }

    public void ReplaceSlotSkill(Skill skill, int indexOfSlot) {
        //UISpellInfo info = GetSpellInfoById(skill.skillId);
        UISpellInfo info = skill.skillData.ConvertToUISpellInfo();
        info.Cooldown = skill.CD;

        GameObject spellSlotObj = spellSlots[indexOfSlot];
        UISpellSlot spellSlot = spellSlotObj.GetComponent<UISpellSlot>();
        spellSlot.Assign(info);

        UpdateUISkillCD(skill, spellSlot.GetComponent<UISkillCooldown>());
    }

    private void UpdateUISkillCD(Skill skill, UISkillCooldown uiSkillCooldown) { 
        if (skill.CDLeft > 0)
        {
            uiSkillCooldown.totalCD = skill.CD;
            uiSkillCooldown.StartCD();
            uiSkillCooldown.SetCDLeft(skill.CDLeft);
        }
        else
        {
            uiSkillCooldown.CompleteCD();
        }
    }

    public void RegisterSkillCD(Skill skill)
    {
    }

    public GameObject GetSlotBySkill(Skill skill) {
        foreach (GameObject slotObj in spellSlots) {
            UISpellSlot spellSlot = slotObj.GetComponent<UISpellSlot>();
            if (spellSlot.ID == skill.skillId) {
                return slotObj;
            }
        }

        return null;
    }

    //private UISpellInfo GetSpellInfoById(int id)
    //{
    //    return skillData.GetByID(id);
    //}
    // REmove ?
    private int GetSkillIdByName(string skillName)
    {
        GameObject skillObj = prafabHolder.GetSkill(skillName);
        return skillObj.GetComponent<Skill>().skillId;
    }

    void Start()
    {
        prafabHolder = Finder.Instance.GetPrafabHolder();
        player = Finder.Instance.GetPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldFlush) {
            Flush();
        }
    }
}
