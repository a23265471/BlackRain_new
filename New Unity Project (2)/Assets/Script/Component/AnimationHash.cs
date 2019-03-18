using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHash : MonoBehaviour
{
    public ActionState actionState;
    private Animator animator;
    private GameStageController gameStagecontroller;

    public Dictionary<int, string> playerAniamtionDictionary;
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
       

        Initialize();
        for (int i = 0; i < actionState.Action.Length ; i++)
        {
          //  Debug.Log(Animator.StringToHash(actionState.Action[i]));
            
        }
        
    }

    private void Update()
    {
    
    }

    private void Initialize()
    {
        playerAniamtionDictionary = new Dictionary<int, string>();
       


        for(int i = 0; i < actionState.Action.Length; i++)
        {
            playerAniamtionDictionary[Animator.StringToHash(actionState.Action[i])] = actionState.Action[i];
        }
        
    }

    public float GetAnimationTime()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] stateClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        float animationTime;
        string stateName;
        if (playerAniamtionDictionary.TryGetValue(stateInfo.shortNameHash, out stateName))
        {
            animationTime = stateClipInfo[0].clip.length * stateInfo.normalizedTime;
            return animationTime;
        }
        else
        {
            Debug.LogWarning("Unknown animator state name.");
            return 0;
        }
        
    }

    public bool GetCurrentAnimationState(string animationState)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
              
        string stateName;
        if (playerAniamtionDictionary.TryGetValue(stateInfo.shortNameHash, out stateName))
        {
            if (animationState == stateName)
            {
              //  Debug.Log(stateName);
                return true;
            }
            else
            {
               // Debug.Log(stateName);
                return false;
            }
            
        }
        else
        {
            Debug.LogWarning("Unknown animator state name.");
            return false;
        }

    }

    public bool GetCurrentAnimationTag(string AnimationTag)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.IsTag(AnimationTag);

    }


}
