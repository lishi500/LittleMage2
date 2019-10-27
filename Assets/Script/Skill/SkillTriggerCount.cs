using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTriggerCount
{
    Dictionary<string, int> skillCount = new Dictionary<string, int>();
    public Player player;

    public int GetSkillCount(string skillName) {
        int count;
        skillCount.TryGetValue(skillName, out count);
        return count;
    }
    
    public void AddSkillCount(string skillName) {
        int count;
        skillCount.TryGetValue(skillName, out count);
        skillCount.Add(skillName, ++count);
    }

    public void ClearSkillCount(string skillName) {
        if (skillCount.ContainsKey(skillName)) {
            skillCount.Remove(skillName);
        }
    }

    public int GetMaxSkillCount() {
        return player.maxSkillTriggerTimes;
    }

    public bool CanTriggerNextSkill(string skillName) {
        return GetSkillCount(skillName) < GetMaxSkillCount();
    }

}
