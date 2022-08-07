using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillCooldown : MonoBehaviour
{
    public GameObject cdMask;
    public GameObject cdCount;
    private Text cdText;
    private Image maskImage;
    private Image backgroundImage;

    public Sprite cdBackgroundImage;
    public Sprite readyBackgroundImage;


    [HideInInspector]
    public float totalCD;
    [HideInInspector]
    public float cdLeft;

    private bool isCDInprogress;

    public void StartCD() {

        cdLeft = totalCD;
        isCDInprogress = true;
        maskImage.fillAmount = 1;
        backgroundImage.sprite = cdBackgroundImage;
    }

    public void StopCD() {
        isCDInprogress = false;
    }

    public void ResumeCD() {
        isCDInprogress = true;
    }

    public void SetCDLeft(float cdLeft) {
        this.cdLeft = cdLeft;
    }

    public void CompleteCD() {
        cdLeft = 0;
        isCDInprogress = false;
        maskImage.fillAmount = 0;
        backgroundImage.sprite = readyBackgroundImage;

        DisplayText();
    }

    private void DisplayText() {
        if (cdLeft == 0)
        {
            cdText.text = "";
        }
        else {
            cdText.text = Mathf.RoundToInt(cdLeft).ToString();
        }
    }

    private void UpdateCDProgress() {
        cdLeft -= Time.deltaTime;
        if (cdLeft <= 0)
        {
            CompleteCD();
        }
        else {
            if (totalCD > 0) {
                float progress = cdLeft / totalCD;
                maskImage.fillAmount = progress;
            }
        }
    }

    void Start()
    {
        maskImage = cdMask.GetComponent<Image>();
        cdText = cdCount.GetComponent<Text>();
        backgroundImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isCDInprogress) {
            UpdateCDProgress();
            DisplayText();
        }
    }
}
