using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : Role
{
    [Header("Enemy")]
    public List<GameObject> buffsToPlayer;
    public float MaxChaseDistance = 20f;

    [HideInInspector]
    Player player;
    
    public void OnPetAdded()
    {

    }

    public void DropSkillOrb(OrbSkill orbSkill) {

    }

    public override void Awake()
    {
        base.Awake();
        roleType = RoleType.PlayerMinion;
    }

    public override void Start()
    {
        base.Start();

        GameObject tailSlash = Finder.Instance.GetPrafabHolder().GetSkill("Tail Slash");
        AddSkill(tailSlash);
    }
}
