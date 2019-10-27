using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager
{
    private HashSet<RoleStatus> statusSet;
    private bool canMove;
    private bool canAction;
    private bool canCast;
    private bool canTakeDamage;
    private bool canBeTarget;

    private readonly List<RoleStatus> canNotAction = new List<RoleStatus> { RoleStatus.STUN, RoleStatus.FROZEN };
    private readonly List<RoleStatus> canNotMove = new List<RoleStatus> { RoleStatus.STUN, RoleStatus.FROZEN, RoleStatus.ROOTED };
    private readonly List<RoleStatus> canNotCast = new List<RoleStatus> { RoleStatus.STUN, RoleStatus.FROZEN, RoleStatus.SCLIENCE };
    private readonly List<RoleStatus> canNotTakeDamage = new List<RoleStatus> { RoleStatus.IMMU };
    private readonly List<RoleStatus> canNotBeTarget = new List<RoleStatus> { RoleStatus.INVISIABLE };

    public StatusManager() {
        statusSet = new HashSet<RoleStatus>();
        AddStatus(RoleStatus.NORMAL);
    }
    public void AddStatus(RoleStatus status) {
        statusSet.Add(status);
        UpdateRoleState();
        if (notifyStatusChange != null) {
            notifyStatusChange(status, true);
        }
    }

    public void RemoveStatus(RoleStatus status)
    {
        statusSet.Remove(status);
        UpdateRoleState();
        if (notifyStatusChange != null)
        {
            notifyStatusChange(status, false);
        }
    }

    public bool CanMove() { return canMove; }
    public bool CanAction() { return canAction; }
    public bool CanCast() { return canCast; }
    public bool CanTakeDamage() { return canTakeDamage; }
    public bool CanBeTarget() { return canBeTarget; }

    private void UpdateRoleState()
    {
        canMove = intersectCount(canNotMove) == 0;
        canAction = intersectCount(canNotAction) == 0;
        canCast = intersectCount(canNotCast) == 0;
        canTakeDamage = intersectCount(canNotTakeDamage) == 0;
        canBeTarget = intersectCount(canNotBeTarget) == 0;
    }

    private int intersectCount(List<RoleStatus> list) {
        return statusSet.Intersect<RoleStatus>(list).Count<RoleStatus>();
    }

    public delegate void StatusChangeDelegate(RoleStatus status, bool isAdd);
    public event StatusChangeDelegate notifyStatusChange;

}
