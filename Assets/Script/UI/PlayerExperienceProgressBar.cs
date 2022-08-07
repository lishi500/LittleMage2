using DuloGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExperienceProgressBar : MonoBehaviour
{
    public GameObject mainCanvas;
    private CanvasPanelController canvasPanelController;

    UIProgressBar uIProgressBar;
    Player player;
    LevelExpData levelExpData;
    Text text;

    private int currentLevel;

    private int currentExp;
    private int targetExp;
    private int currentLevelMaxExp;
    private int remainingExp;

    private float lerp = 0f;
    private float duration = 1.5f;
    void OnPlayerExpGain(int exp) {
        AddExp(exp);
    }

    void SetExp(int exp) {
        currentExp = exp;
        targetExp = exp;

        UpdateProgressBar();
    }

    void AddExp(int exp) {
        targetExp += exp;
        if (targetExp > currentLevelMaxExp) {
            int extra = targetExp - currentLevelMaxExp;
            remainingExp += extra;
            targetExp = currentLevelMaxExp;
        }
    }

    void UpdateExp() {
        if (currentExp != targetExp)
        {
            lerp += Time.deltaTime / duration;
            currentExp = (int)Mathf.Ceil(Mathf.Lerp(currentExp, targetExp, lerp));
        }
        else {
            lerp = 0f;
        }

        UpdateProgressBar();

        if (currentExp >= currentLevelMaxExp) {
            LevelUp();
        }
    }

    void UpdateProgressBar() {
        uIProgressBar.fillAmount = currentExp / (float) currentLevelMaxExp;
        text.text = "Level " + currentLevel;
    }

    void LevelUp() {
        SetExp(0);

        currentLevel += 1;
        currentLevelMaxExp = levelExpData.expMap[currentLevel + 1];

        if (remainingExp > 0) {
            int temp = remainingExp;
            remainingExp = 0;
            AddExp(temp);
        }

        UpdateProgressBar();

        canvasPanelController.PopUpSkillSelectPanel();
        UILootSkillController uILootSkillController = canvasPanelController.SkillSelectPanel.GetComponent<UILootSkillController>();
        uILootSkillController.LootSkill();
    }

   

    void Start()
    {
        uIProgressBar = GetComponent<UIProgressBar>();
        player = Finder.Instance.GetPlayer();
        text = GetComponentInChildren<Text>();
        canvasPanelController = mainCanvas.GetComponent<CanvasPanelController>();

        player.notifyExpGain += OnPlayerExpGain;
        currentLevel = player.attribute.level;
        currentExp = player.attribute.exp;
        levelExpData = player.levelExpData;

        currentLevelMaxExp = levelExpData.expMap[currentLevel + 1];

        UpdateProgressBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentExp != targetExp || remainingExp > 0) {
            UpdateExp();
        }
    }
}
