using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomAnimationController : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public SimpleEventHelper eventHelper;
    [HideInInspector]
    public AnimationState animationState;
    [HideInInspector]
    public AnimationState previousState;

    BaseAI ai;
    private List<ExceptionState> exceptionList;
    private Queue<ExceptionState> exceptionOnceQueue;
    protected float attackClipLength;
     

    public string[] animationStates = new string[]
    { "walk", "run", "attack", "fight_stance", "defence", "get_hit", "die", "cast", "channeling", "channeling_loop",
    "play", "fly"};

    public Dictionary<AnimationState, string> stateMapping = new Dictionary<AnimationState, string>() {

        { AnimationState.NONE, "" },
        { AnimationState.IDLE, "idle" },
        { AnimationState.WALK, "walk" },
        { AnimationState.RUN, "run" },
        { AnimationState.ATTACK, "attack" },
        { AnimationState.FIGHT_STANCE, "fight_stance" },
        { AnimationState.DEFENCE1, "defence" },
        { AnimationState.GET_HIT, "get_hit" },
        { AnimationState.DIE, "die" },
        { AnimationState.CAST, "cast" },
        { AnimationState.CHANNELING, "channeling" },
        { AnimationState.CHANNELING_LOOP, "channeling_loop" },
        // dragon
        { AnimationState.FLY, "fly"},
        { AnimationState.PLAY, "play"},
        // parameters
        { AnimationState.RANDOM_100, "Random 100"},
        { AnimationState.FLY_PROGRESS, "fly_progress"},
        { AnimationState.SKILL_ANIMATION, "skill_animation"}

    };

    public abstract void UpdateAnimationByState();

    public void SetAllFalse() {
        SetBoolState("");
    }

    public void SetBoolState(AnimationState state)
    {
        SetBoolState(GetStateMapping(state));
    }

    public void SetBoolState(string stateName)
    {
        //Debug.Log("- SetBoolState -" + stateName);
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                if (param.name == stateName)
                {
                    animator.SetBool(param.name, true);
                }
                else
                {
                    animator.SetBool(param.name, false);
                }
            }
        }
    }

    public void SetBoolStates(string[] stateNames)
    {
        foreach (string state in stateNames) {
            SetBoolState(state);
        }
    }

    public void SetBoolStates(AnimationState[] states)
    {
        foreach (AnimationState state in states)
        {
            SetBoolState(GetStateMapping(state));
        }
    }


    private void UpdateExceptionState()
    {
        if (exceptionList.Count > 0)
        {
            foreach (ExceptionState exceptionState in exceptionList)
            {
                //Debug.Log("exceptionState " + exceptionState.name + " - " + exceptionState.state);
                SetBool(exceptionState.name, exceptionState.state);
            }
        }
        while (exceptionOnceQueue.Count > 0)
        {
            ExceptionState exceptionState = exceptionOnceQueue.Dequeue();
            SetBool(exceptionState.name, exceptionState.state);
        }
    }

    protected void SetAttackSpeedMultiplier(float multiplier)
    {
        animator.SetFloat("attack_speed_multiplier", multiplier);
    }

  
    public void SetBool(AnimationState name, bool state)
    {
        SetBool(GetStateMapping(name), state);
    }
    public void SetFloat(AnimationState name, float value)
    {
        SetFloat(GetStateMapping(name), value);
    }
    public void SetInt(AnimationState name, int value)
    {
        SetInt(GetStateMapping(name), value);
    }

    protected void SetBool(string name, bool state)
    {
        if (ContainsParam(animator, name))
        {
            //Debug.Log("Set Bool " + name + " -> " + state);
            animator.SetBool(name, state);
        }
    }
    protected void SetFloat(string name, float value)
    {
        if (ContainsParam(animator, name))
        {
            animator.SetFloat(name, value);
        }
    }

    protected void SetInt(string name, int value)
    {
        if (ContainsParam(animator, name))
        {
            animator.SetInteger(name, value);
            //Debug.Log("Set Int " + name + " " + value);
        }
    }

    public void AddAutoClearState(AnimationState name, bool state, bool isExclusive = true, float time = 0.2f)
    {
        //Debug.Log("AddAutoClearState " + name);
        if (isExclusive)
        {
            //animationState = AnimationState.NONE;
            SetBoolState(name);
        }
        else {
            AddExceptionOnce(name, state);
        }
       
        StartCoroutine(ClearState(name, !state, time));
    }

    private IEnumerator ClearState(AnimationState name, bool state, float time)
    {
        yield return new WaitForSeconds(time);
        //Debug.Log("ClearState " + name + "  " + state);
        AddExceptionOnce(name, state);

        yield return null;
    }

    private string GetStateMapping(AnimationState name)
    {
        string animationName;
        stateMapping.TryGetValue(name, out animationName);

        return animationName;
    }

    // listener
    public void OnAttackSpeedChange(float speed)
    {
        //Debug.Log("OnAttackSpeedChange " + speed);
        float attackSpeed = 10 / speed;
        if (attackSpeed < attackClipLength && attackSpeed > 0)
        {
            SetAttackSpeedMultiplier(attackClipLength / attackSpeed);
        }
        else
        {
            SetAttackSpeedMultiplier(1f);
        }
    }

    public void SecondPhase(AnimationState firstPhash)
    {
        //Debug.Log(" SecondPhase Start from " + firstPhash);
        switch (firstPhash)
        {
            case AnimationState.CHANNELING:
                //Debug.Log("Add  channeling_loop");
                ClearException();
                AddException("channeling", false);
                AddException("channeling_loop", true);
                break;
            default:
                break;
        }
    }

    public void OnWalkSpeedChange(float speed)
    {

    }

    public void SetAnimatorSpeed(float speed)
    {
        animator.speed = speed;
    }

    public void RestartGetHit() {
        //animationState = AnimationState.GET_HIT;
        if (animationState == AnimationState.GET_HIT)
        {
            //Debug.Log("RestartGetHit");
            animator.Play(0, 0, 0f);
        }

        animationState = AnimationState.GET_HIT;
    }

    public AnimationClip GetAnimationClipByName(string name)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == name)
            {
                return ac.animationClips[i];
            }
        }
        return null;
    }

    public void AddException(AnimationState name, bool state)
    {
        AddException(GetStateMapping(name), state);
    }
    public void AddException(string name, bool state)
    {
        exceptionList.Add(new ExceptionState(name, state));
    }

    public void AddExceptionOnce(AnimationState name, bool state)
    {
        AddExceptionOnce(GetStateMapping(name), state);
    }
    public void AddExceptionOnce(string name, bool state)
    {
        exceptionOnceQueue.Enqueue(new ExceptionState(name, state));
    }
    public void ClearException()
    {
        exceptionList.RemoveAll(exception => true);
    }
    private void Awake()
    {
        animator = gameObject.transform.GetComponent<Animator>();
        eventHelper = GetComponent<SimpleEventHelper>();
        eventHelper.notifyFirstPhaseComplete += SecondPhase;

        animationState = AnimationState.IDLE;
        previousState = AnimationState.NONE;
        ai = GetComponent<BaseAI>();
        exceptionList = new List<ExceptionState>();
        exceptionOnceQueue = new Queue<ExceptionState>();

        AnimationClip attackClip = GetAnimationClipByName("attack");
        if (attackClip != null)
        {
            attackClipLength = attackClip.length;
        }
    }
    public virtual void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimationByState();
        UpdateExceptionState();
    }

    public static bool ContainsParam(Animator _Anim, string _ParamName)
    {
        foreach (AnimatorControllerParameter param in _Anim.parameters)
        {
            if (param.name == _ParamName) return true;
        }
        return false;
    }

    class ExceptionState
    {
        public string name;
        public bool state;
        public ExceptionState(string name, bool state)
        {
            this.name = name;
            this.state = state;
        }
    }
}
