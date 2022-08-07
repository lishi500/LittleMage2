using DigitalRubyShared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasPanelController : MonoBehaviour
{
    // Level 0 Panel
    public GameObject GamePlayPanel;

    // Level 1 Panel
    private List<GameObject> level1Panel;

    public GameObject SkillSelectPanel;

    // panel stack


    public void PopUpSkillSelectPanel() {
        TurnOffLevel_1_PanelWithException("SkillSelectPanel");
        DisableJoyStick();
        Finder.Instance.GetTimeController().PauseTime();

    }

    public void CloseDownSkillSelectPanel() {
        TurnOffPanel(SkillSelectPanel);
        EnableJoyStick();
        Finder.Instance.GetTimeController().RestartTime();
    }
    
    private void TurnOffAllLevel_1_Panel() {
        TurnOffLevel_1_PanelWithException("");
    }
    private void TurnOffLevel_1_PanelWithException(string panelName) {
        foreach (GameObject panelObj in level1Panel) {
            if (panelObj != null)
            {
                if (panelObj.name != panelName)
                {
                    TurnOffPanel(panelObj);
                }
                else
                {
                    TurnOnPanel(panelObj);
                }
            }
        }
    }

    public void DisableJoyStick() {
        FingersJoystickScript joyStick = UIFinder.Instance.GetJoyStick();
        FingersScript fingers = UIFinder.Instance.GetFingersScript();
        joyStick.Reset();

        CanvasGroup canvasGroup = joyStick.GetComponentInParent<CanvasGroup>();
        DisableCanvasGroup(canvasGroup);
        fingers.gameObject.SetActive(false);
    }
    public void EnableJoyStick() {
        FingersJoystickScript joyStick = UIFinder.Instance.GetJoyStick();
        FingersScript fingers = UIFinder.Instance.GetFingersScript();

        CanvasGroup canvasGroup = joyStick.GetComponentInParent<CanvasGroup>();
        EnableCanvasGroup(canvasGroup);
        fingers.gameObject.SetActive(true);
    }
    public void TurnOffPanel(GameObject panelObj) {
        CanvasGroup canvasGroup = panelObj.GetComponent<CanvasGroup>();
        DisableCanvasGroup(canvasGroup);
    }

    public void TurnOnPanel(GameObject panelObj) {
        CanvasGroup canvasGroup = panelObj.GetComponent<CanvasGroup>();
        EnableCanvasGroup(canvasGroup);
    }

    private void DisableCanvasGroup(CanvasGroup canvasGroup) {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void EnableCanvasGroup(CanvasGroup canvasGroup)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }


    void Start()
    {
        level1Panel = new List<GameObject> { SkillSelectPanel };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
