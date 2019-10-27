using ECM.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMovementController : BaseAgentController
{
    Role role;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnMoveSpeedChange(float moveSpeed)
    {
        speed = moveSpeed;
    }

    public override void Awake()
    {
        base.Awake();
        role = GetComponent<Role>();
        speed = role.attribute.moveSpeed;
    }
}
