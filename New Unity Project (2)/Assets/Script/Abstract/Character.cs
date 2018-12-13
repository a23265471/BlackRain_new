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

    protected virtual void AnimationBlendTreeControll(Animator animator,string parameterName, float targetValue, ref float controllValue)
    {


        controllValue = Mathf.Lerp(controllValue, targetValue, 0.1f);
        
        controllValue = Mathf.Clamp(controllValue, controllValue, targetValue);
        animator.SetFloat(parameterName, controllValue);


    } 


}
