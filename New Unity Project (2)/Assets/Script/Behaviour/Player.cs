using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]

public class Player : Character
{
    private GameStageData gameStageData;
    private Animator animator;
    private Rigidbody rigidbody;
    private AudioSource audioSource;
    

    private float moveAnimationValue_Horizontal;
    private float moveAniamtionValue_Vertical;
    private void Awake()
    {
        moveAnimationValue_Horizontal = 0;
        moveAniamtionValue_Vertical = 0;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        gameStageData = GameFacade.GetInstance().gameStageData;
        
    }

    void Start ()
    {
       
      
	}
	
	// Update is called once per frame
	void Update ()
    {

        


    }

    private void Move()
    {
        AnimationBlendTreeControll(animator, "Vertical", 1, ref moveAniamtionValue_Vertical);
        Debug.Log(moveAniamtionValue_Vertical);
    }
}
