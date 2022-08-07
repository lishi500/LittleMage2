using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "player exp", menuName = "Player/levelExp", order = 1)]
public class LevelExpData : ScriptableObject
{
    public int[] expMap;

    public bool CanLevelUp(int currentLevel, int exp) {
        if (expMap.Length > currentLevel) {
            int neededExp = expMap[currentLevel + 1];
            return exp > neededExp;
        }

        return true;
    }

    public int[] expCalculator(int maxLevel) {

        float points = 0;
        float output = 0;
        int minlevel = 2; // first level to display
        int[] exps = new int[maxLevel];

        for (int lvl = 1; lvl <= maxLevel; lvl++)
        {
            points += Mathf.Floor(lvl + 300 * Mathf.Pow(2, lvl / 7f));
            if (lvl >= minlevel)

            output = Mathf.Floor(points / 4);
            exps[lvl] = (int) output;
        }

        return exps;
    }
}
