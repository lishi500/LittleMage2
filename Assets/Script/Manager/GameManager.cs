using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject utilManager;
    AIUtils aiUtils;


    public SkillTriggerCount skillTriggerCount;

    private void Awake()
    {

        utilManager = transform.Find("UtilManager").gameObject;
        aiUtils = utilManager.GetComponent<AIUtils>();

        // TODO reposition to gameState
        skillTriggerCount = new SkillTriggerCount();
        skillTriggerCount.player = Finder.Instance.GetPlayer();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    protected virtual void HandleInput()
    {
        bool fire = Input.GetButton("Fire1");
        if (fire)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                aiUtils.IsWalkable(hit.point);
            }
            
        }
    }
}
