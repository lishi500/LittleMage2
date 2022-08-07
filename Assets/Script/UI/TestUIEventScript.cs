using DuloGames.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUIEventScript : MonoBehaviour
{
    public UISpellDatabase skillData;
    public GameObject spellSlot;
    public Sprite glow;
    public Item item;

    public GameObject testObject;
    public GameObject testObject2;

    EquipmentController equipmentController;

    public void OnTestButtonClick() {
        //UISpellInfo spellInfo = skillData.Get(0);
        //Debug.Log(spellInfo.Name + " CD: " + spellInfo.Cooldown);
        //spellInfo.Cooldown = spellInfo.Cooldown / 2;
        //Debug.Log(" CD After Reduce: " + spellInfo.Cooldown);
        Image image = spellSlot.GetComponent<Image>();
        image.sprite = glow;

        //ReAssignSpellObject();
        CdSecondSpell();
    }

    public void Test2Clicked() {
        UISkillCooldown uISkillCooldown = spellSlot.GetComponent<UISkillCooldown>();
        uISkillCooldown.SetCDLeft(uISkillCooldown.cdLeft - 2);
        uISkillCooldown.CompleteCD();
    }

    public void TestItemDrop() {
        ItemDropController itemDropController = testObject.GetComponent<ItemDropController>();
        for (int i = 0; i < 2; i++) {
            itemDropController.Drop();
        }
    }

    private bool toogle = true;
    public void TestToogleTrail() {
        Role testRole = testObject2.GetComponent<Role>();
        TrailManager trailManager = testObject2.GetComponent<TrailManager>();
        if (toogle)
        {
            trailManager.TrunOffTrailByTypes(new TrailPointType[] { TrailPointType.Tail });
        }
        else {
            trailManager.TrunOnTrailByTypes(new TrailPointType[] { TrailPointType.Tail });
        }
        toogle = !toogle;
    }

    public void TestAddTrail() {
        Role testRole = testObject2.GetComponent<Role>();
        TrailManager trailManager = testObject2.GetComponent<TrailManager>();
        trailManager.AddTrail(testObject, TrailPointType.Tail);
    }

    public void TestDragonAnimation() {
        Pet pet = testObject.GetComponent<Pet>();
        //DragonAnimationController animationController = testObject.GetComponentInChildren<DragonAnimationController>();
        //animationController.AddAutoClearState(AnimationState.PLAY, true);
        //animationController.animationState = AnimationState.WALK;

        PetAI ai = pet.GetComponent<PetAI>();
        //ai.FlyTo(testObject2.transform);

    }

    public void SwapEquipment() {
        equipmentController.SwapEquipment(item);
    }

    public void CdSecondSpell()
    {
        UISpellSlot uispellSlot = spellSlot.GetComponent<UISpellSlot>();
        UISkillCooldown uISkillCooldown = spellSlot.GetComponent<UISkillCooldown>();
        uISkillCooldown.totalCD = uispellSlot.GetSpellInfo().Cooldown;
        uISkillCooldown.StartCD();

        //UISlotCooldown cd = uispellSlot.m_Cooldown;
        //UISlotCooldown.CooldownInfo cdInfo;
        //UISlotCooldown.spellCooldowns.TryGetValue(2, out cdInfo);
        //Debug.Log("LEft " + (cdInfo.endTime - Time.time));

        uispellSlot.OnPointerClick(null);
    }
    public void ReAssignSpellObject() {
        //UISpellDatabase tempSpellDate = ScriptableObject.CreateInstance<UISpellDatabase>();
        UISpellInfo cloneInfo = cloneSpellInfo(skillData, 1);
        cloneInfo.Cooldown = cloneInfo.Cooldown / 2;

        Debug.Log(cloneInfo.Name + " CD: " + cloneInfo.Cooldown);
        

        UISpellSlot uiSpellSlot = spellSlot.GetComponent<UISpellSlot>();
        uiSpellSlot.Assign(cloneInfo);
    }

    public UISlotCooldown GenerateCoolDown(float totalCD, float leftCD) {
        float endTime = Time.time + leftCD;
        float startTime = Time.time + totalCD - leftCD;

        return new UISlotCooldown();
    }

    public UISpellInfo cloneSpellInfo(UISpellDatabase spellDate, int id) {
        UISpellInfo spellInfo = spellDate.GetByID(id);
        UISpellInfo cloneInfo = new UISpellInfo();

        cloneInfo.ID = spellInfo.ID;
        cloneInfo.Name = spellInfo.Name;
        cloneInfo.Icon = spellInfo.Icon;
        cloneInfo.Description = spellInfo.Description;
        cloneInfo.Range = spellInfo.Range;
        cloneInfo.Cooldown = spellInfo.Cooldown;
        cloneInfo.CastTime = spellInfo.CastTime;
        cloneInfo.PowerCost = spellInfo.PowerCost;
        cloneInfo.Flags = spellInfo.Flags;

        return cloneInfo;
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        equipmentController = gameManager.GetComponent<EquipmentController>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
