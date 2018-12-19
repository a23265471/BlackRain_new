using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private ParticleSystem particleSystem;

   /* protected void Initialize(Animator CharacterAnimator,AudioSource CharacterAudioSource)
    {
        animator = CharacterAnimator;
        audioSource = CharacterAudioSource;
        
    }*/

    protected virtual void AnimationBlendTreeControll(Animator animator,string parameterName, float targetValue,ref float controllValue,float animationSpeed)
    {
        controllValue = Mathf.Lerp(controllValue, targetValue, animationSpeed * Time.deltaTime);

        if (targetValue > controllValue)
        {
            controllValue = Mathf.Clamp(controllValue, controllValue, targetValue);
        }
        else if(targetValue < controllValue)
        {
            controllValue = Mathf.Clamp(controllValue, targetValue, controllValue);

        }

       
        animator.SetFloat(parameterName, controllValue);


    } 


}
