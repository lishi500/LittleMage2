using ECM.Controllers;
using ECM.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : BaseCharacterController
{
    public bool _isMoving = false;
    Player player;

    protected override void HandleInput()
    {
        if (player.CanMove())
        {
            moveDirection = new Vector3
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = 0.0f,
                z = Input.GetAxisRaw("Vertical")
            };

            if ((moveDirection.x == 0 && moveDirection.z == 0))
            {
                _isMoving = false;
            }
            else
            {
                _isMoving = true;
            }
        }
        else {
            _isMoving = false;
        }
    }

    protected override void UpdateRotation()
    {
        // Rotate towards movement direction (input)
        if (player.CanAction() && !player.isChanneling) {
            RotateTowardsMoveDirection();
        }
    }

    void OnMoveSpeedChange(float moveSpeed) {
        speed = moveSpeed;
        Debug.Log("OnMoveSpeedChange "  +moveSpeed);
    }

    public override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
        player.attribute.notifyMoveSpeedChange += OnMoveSpeedChange;
        speed = player.attribute.moveSpeed;
    }
}
