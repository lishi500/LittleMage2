using DuloGames.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UILootSkillController : MonoBehaviour
{
    public GameObject[] spellSlots;
    //public UISpellInfo[] spells;
    public GameObject descriptionObj;

    GameState gameState;

    private Text descriptionText;
    private bool firstSelect;
    private UISpellInfo clickedSkill;

    public int numberOfLootSkills = 3;

    public void LootSkill() {
        SkillData[] lootedSkills = LootSkills(numberOfLootSkills);
        clickedSkill = null;

        AssignLootSkills(lootedSkills);
    }

    private void AssignLootSkills(SkillData[] skills) {
        firstSelect = false;

        for (int i = 0; i < skills.Length; i++) {
            UISpellInfo skillInfo = skills[i].ConvertToUISpellInfo();
            if (skillInfo != null)
            {
                AssignSkill(skillInfo, i);
            }
            else {
                Debug.LogError("Skill id not found in all Spells " + skills[i].Name);
            }
        }
    }

    private SkillData[] LootSkills(int k)
    {
        int totalSize = gameState.unlockedSkill.Length;
        List<int> indexs = new List<int>();
        SkillData[] skillDatas = new SkillData[k];
        List<Skill> currentSkills = Finder.Instance.GetPlayer().skills;


        for (int i = 0; i < totalSize; i++)
        {
            indexs.Add(i);
        }

        //IEnumerable<int> selectedIndex = indexs.OrderBy(x => Random.Range(-1, 1));
        IEnumerable<int> selectedIndex = indexs.OrderBy(a => Guid.NewGuid());

        int j = 0;
        foreach (int index in selectedIndex)
        {
            if (j >= k) {
                break;
            };

            SkillData skillData = gameState.unlockedSkill[index];
            if (skillData.CanDuplicate) {
                skillDatas[j++] = gameState.unlockedSkill[index];
            }
            else
            {
                if (!IsContainSkill(currentSkills, skillData)) {
                    skillDatas[j++] = gameState.unlockedSkill[index];
                }
            }
            //Debug.Log("LootSkill " + gameState.unlockedSkill[index].Name);
        }

        return skillDatas;
    }
    private bool IsContainSkill(List<Skill> currentSkills, SkillData skillDate) {
        foreach (Skill skill in currentSkills) {
            if (skill.skillId == skillDate.ID) {
                return true;
            }
        }

        return false;
    }

    private void AssignSkill(UISpellInfo uISpell, int index) {
        GameObject slotObj = spellSlots[index];
        UISpellSlot spellSlot = slotObj.GetComponent<UISpellSlot>();
        UISelectSkillHandler selectSkillHandler = slotObj.GetComponent<UISelectSkillHandler>();

        spellSlot.Assign(uISpell);
        selectSkillHandler.notifyLootSkillSelect += OnSkillClick;
    }

    public void OnSkillClick(UISpellInfo skill) {
        if (clickedSkill != skill)
        {
            clickedSkill = skill;
            FlushSkillDescription(skill);
        }
        else {
            Debug.Log("Choose " + skill.Name);

            GameObject skillObj = Finder.Instance.GetPrafabHolder().GetSkill(skill.Name);

            Finder.Instance.GetPlayer().AddSkill(skillObj);
            CanvasPanelController panelController = UIFinder.Instance.GetCanvasPanelController();
            panelController.CloseDownSkillSelectPanel();
        }
    }

    public void FlushSkillDescription(UISpellInfo skill) {
        descriptionText.text = skill.Description;
    }


    void Start()
    {
        gameState = Finder.Instance.GetGameState();
        descriptionText = descriptionObj.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
