using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]

public class PlayerBehaviour : Character
{
   
    private GameStageData gameStageData;
    private PlayerController playerController;
    private Animator animator;
    private Rigidbody rigidbody;
    private AudioSource audioSource;

    private float moveAnimation_Vertical;
    private float moveAnimation_Horizontal;
    public float MoveAnimationSmoothSpeed;

    private void Awake()
    {
        
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        gameStageData = GameFacade.GetInstance().gameStageData;
        playerController = GameFacade.GetInstance().playerController;
    }

    void Start ()
    {
        moveAnimation_Vertical = 0;
        moveAnimation_Horizontal = 0;

    }
	
	// Update is called once per frame
	void Update ()
    {
       // playerController.Move(this);
        


    }

    public void PlayerMove(float moveDirection_Vertical,float moveDirection_Horizontal)
    {
        AnimationBlendTreeControll(animator, "Vertical", moveDirection_Vertical, ref moveAnimation_Vertical, MoveAnimationSmoothSpeed);
        AnimationBlendTreeControll(animator, "Horizontal", moveDirection_Horizontal, ref moveAnimation_Horizontal, MoveAnimationSmoothSpeed);

    }


}
