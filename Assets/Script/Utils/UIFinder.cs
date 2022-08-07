using DigitalRubyShared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFinder : Singleton<UIFinder>
{
    private GameObject uiRoot;
    private FingersJoystickScript joystick;
    private CanvasPanelController canvasPanelController;
    private UISkillController uiSkillController;
    private FingersScript fingersScript;
    // Start is called before the first frame update


    public CanvasPanelController GetCanvasPanelController()
    {
        if (canvasPanelController == null)
        {
            GameObject mainCanvas = FindObjectWithComponent(GameObject.FindGameObjectsWithTag("Canvas"), typeof(CanvasPanelController));
            if (mainCanvas != null)
            {
                canvasPanelController = mainCanvas.GetComponent<CanvasPanelController>();
            }
        }

        return canvasPanelController;
    }

    public FingersJoystickScript GetJoyStick()
    {
        if (joystick == null)
        {
            GameObject joyStickObj = GameObject.FindGameObjectWithTag("JoyStick");
            if (joyStickObj != null)
            {
                joystick = joyStickObj.GetComponent<FingersJoystickScript>();
            }
        }

        return joystick;
    }

    public FingersScript GetFingersScript() {
        if (fingersScript == null) {
            Transform fingersScriptObj = SearchHierarchyByName(GetUIRoot().transform, "FingersScript");
            if (fingersScriptObj != null) {
                fingersScript = fingersScriptObj.GetComponent<FingersScript>();
            }
        }

        return fingersScript;
    }

    public UISkillController GetUISkillController()
    {
        if (uiSkillController == null)
        {
            Transform GamePlayPanelObj = GetCanvasPanelController().GamePlayPanel.transform;
            Transform SkillBarObj = SearchHierarchyByName(GamePlayPanelObj, "SkillBar");
            if (SkillBarObj != null) {
                uiSkillController = SkillBarObj.GetComponent<UISkillController>();
            }
        }

        return uiSkillController;
    }

    private GameObject GetUIRoot() {
        if (uiRoot == null) {
            uiRoot = GameObject.FindGameObjectWithTag("UI");
        }

        return uiRoot;
    }

    private GameObject FindObjectWithComponent(GameObject[] objects, System.Type systemType)
    {
        foreach (GameObject gameObj in objects)
        {
            if (gameObj.GetComponent(systemType) != null)
            {
                return gameObj;
            }
        }

        return null;
    }

    private Transform SearchHierarchyByName(Transform current, string name)
    {
        if (current.name == name)
            return current;
        for (int i = 0; i < current.childCount; ++i)
        {
            Transform found = SearchHierarchyByName(current.GetChild(i), name);
            if (found != null)
                return found;
        }

        return null;
    }
}
