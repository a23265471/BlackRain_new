using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private ParticleSystem particleSystem;

    private void Initialize(Animator CharacterAnimator,AudioSource CharacterAudioSource, ParticleSystem CharacterParticleSystem)
    {
        animator = CharacterAnimator;
        audioSource = CharacterAudioSource;
        particleSystem = CharacterParticleSystem;
    }

    protected virtual void AnimationBlendTreeControll()
    {
        

    } 


}
