﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerBehaviour : Character
{
    [Flags]
    public enum PlayerState
    {    
        Move = 0x01,
        Jump = 0x02,
        Falling = 0x04,
        DoubleJump = 0x08,
        Dash = 0x10,       
        Attack = 0x20,
        DashAttack = 0x30,
        Skill = 0x40,
        Avoid = 0x50,      
        SkyAttack = 0x60,
        Damage = 0xc0,
        GetDown = 0xe0,
        Dead = 0xf0,
       
        CanFalling = Move | Attack,
        CanMove = Move | Falling | Jump | DoubleJump,
        CanDoubleJump = Jump | Falling,
        CanDash = Move | Attack | Jump,
        CanDashAttack = CanDash,       
        CanSkyAttack = Jump | Falling | DoubleJump,
        CanAvoid = Move | Attack | Skill,      
        CanSkill = Attack | Move,
        CanDamage = 0xff,
        CaGetDown = 0xff,
        CanDead = 0xff,

        LandingChecking = Jump | Falling | DoubleJump | SkyAttack,

    }

    // public PlayerState DoubleJump;
    public PlayerState playerState;
    public static PlayerBehaviour playerBehaviour;

    private GameStageData gameStageData;
    private PlayerController playerController;
    private Animator playerAnimator;
   
    private AudioSource playerAudioSource;
    private PlayerData.PlayerParameter playerParameter;
    private AnimationHash animationHash;
    private float avoidSpeed;

    #region 圖層
    int floorMask;
    #endregion

    #region 子物件
    public Transform cameraLookAt;
    #endregion

    #region 移動參數
    private float rotation_Horizontal;
    private float curMoveSpeed;
    private float moveSpeed;

    private float moveAnimation_Vertical;
    private float moveAnimation_Horizontal;
    public float MoveAnimationSmoothSpeed;
    private int avoidDirection_Horizontal;
    private int avoidDirection_Verticalz;
    #endregion

    #region 物理碰撞   
    private Rigidbody playerRigidbody;
    public CapsuleCollider physicsCollider;
    public float groundedDis;
    public bool isGround
    {
        get
        {
            return Physics.Raycast(physicsCollider.bounds.center, -Vector3.up, physicsCollider.bounds.extents.y + groundedDis, floorMask);
        }
    }
    #endregion

    private void Awake()
    {
        playerBehaviour = this;
        playerAnimator = GetComponent<Animator>();
        playerAudioSource = GetComponent<AudioSource>();
        physicsCollider = GetComponent<CapsuleCollider>();
        animationHash = GetComponent<AnimationHash>();
        playerRigidbody = GetComponent<Rigidbody>();

        gameStageData = GameFacade.GetInstance().gameStageData;
        playerController = GameFacade.GetInstance().playerController;
        playerParameter = gameStageData.CurPlayerStageData.playerData.playerParameter;

        floorMask = LayerMask.GetMask("Floor");

    }

    void Start()
    {
        moveAnimation_Vertical = 0;
        moveAnimation_Horizontal = 0;
        cameraLookAt= gameObject.transform.Find("CameraLookAt");
       
    }
     
    void Update()
    {
        physicsCollider.height = playerAnimator.GetFloat("ColliderHeight");
        Rotaion();      
        
       // Debug.Log(animationHash.GetAnimationState("Jump"));

    }

    #region 移動
    private void Rotaion()
    {       
        rotation_Horizontal += Input.GetAxis("Mouse X") * playerParameter.moveParameter.RotateSpeed * Time.deltaTime;
              
        if (rotation_Horizontal > 360)
        {
            rotation_Horizontal -= 360;
        }
        else if (rotation_Horizontal < 0)
        {
            rotation_Horizontal += 360;
        }

        if (playerState != PlayerState.Avoid)
        {
            transform.rotation = Quaternion.Euler(0, rotation_Horizontal, 0);
        }
    }

    public void GroundedMove(float moveDirection_Vertical, float moveDirection_Horizontal)
    {
        if (((playerBehaviour.playerState & PlayerState.CanMove) != 0)) 
        {
            
            AnimationBlendTreeControll(playerAnimator, "Vertical", moveDirection_Vertical, ref moveAnimation_Vertical, MoveAnimationSmoothSpeed);
            AnimationBlendTreeControll(playerAnimator, "Horizontal", moveDirection_Horizontal, ref moveAnimation_Horizontal, MoveAnimationSmoothSpeed);

            if (moveDirection_Vertical == 0 || moveDirection_Horizontal == 0)
            {
                curMoveSpeed = Mathf.Sqrt((Mathf.Pow(moveSpeed, 2) * 2));
            }
            else
            {
                curMoveSpeed = moveSpeed;

            }

            float MoveX = moveAnimation_Horizontal * Time.deltaTime * playerParameter.moveParameter.RunSpeed;
            float MoveZ = moveAnimation_Vertical * Time.deltaTime * playerParameter.moveParameter.RunSpeed;
            transform.Translate(MoveX, 0, MoveZ);
           
        }

    }

    public void FallingMove(float moveDirection_Vertical, float moveDirection_Horizontal)
    {
       
    }
    public void Falling()
    {
        if((playerState & PlayerState.CanFalling) != 0)
        {
            playerAnimator.SetTrigger("Falling");
           
        }
       
    }

    #endregion

    #region 迴避
    public void Avoid(int moveDirection_Vertical,int moveDirection_Horizontal)
    {       
        string xDirection;
        string zDirection;

        avoidDirection_Horizontal = moveDirection_Horizontal;
        avoidDirection_Verticalz = moveDirection_Vertical;

        if (moveDirection_Vertical == 1)
        {
            zDirection = "Forward";
        }
        else if (moveDirection_Vertical == -1) 
        {
            zDirection = "Back";
        }
        else
        {
            zDirection = "";
        }

        if (moveDirection_Horizontal == 1)
        {
            xDirection = "Right";
        }
        else if (moveDirection_Horizontal == -1)
        {
            xDirection = "Left";
        }
        else
        {
            xDirection = "";
        }

        AvoidAnimatorTrigger(xDirection, zDirection);               
       
    }

    private void AvoidAnimatorTrigger(string Horizontal_Direction, string Vertical_Direction)
    {
        playerAnimator.ResetTrigger("Avoid" + Vertical_Direction + Horizontal_Direction);
        playerAnimator.SetTrigger("Avoid" + Vertical_Direction + Horizontal_Direction);             
    }
    #endregion

    public void Jump()
    {
        playerAnimator.SetTrigger("Jump");

    }

   /* public void FallingMove()
    {
        AddHorizontalForce(playerRigidbody, playerParameter.jumpParameter.JumpMoveSpeed, playerController.moveDirection_Vertical, playerController.moveDirection_Horizontal);
    }
    */
   /* public void Landing()
    {
        playerAnimator.SetTrigger("Idle");
        if (animationHash.GetAnimationState("Idle_Run"))
        {
            playerState = PlayerState.Move;
        }

    }
    */

    #region AnimationEvent

    public void ChangePlayerState(int ChangePlayerState)
    {
        playerAnimator.SetFloat("Horizontal", 0);
        playerAnimator.SetFloat("Vertical", 0);
        
        moveAnimation_Vertical = 0;
        moveAnimation_Horizontal = 0;
        switch (ChangePlayerState)
        {
            case (int)PlayerState.Move:
                playerState = PlayerState.Move;
                break;
            case (int)PlayerState.Jump:
                playerState = PlayerState.Jump;
                break;
            case (int)PlayerState.DoubleJump:
                playerState = PlayerState.DoubleJump;
                break;
            case (int)PlayerState.Avoid:
                playerState = PlayerState.Avoid;
                Displacement(transform, playerParameter.avoidParameter.AvoidSpeed, playerParameter.avoidParameter.AvoidDistance, avoidDirection_Verticalz, avoidDirection_Horizontal);        
                break;
            case (int)PlayerState.Falling:
                if (playerState != PlayerState.Falling)
                {
                    playerState = PlayerState.Falling;
                    StartCoroutine("LandingCheck");
                }
              
                break;


        }
    } 

   /* public void AvoidState()
    {
        playerState = PlayerState.Avoid;
        Displacement(transform, playerParameter.avoidParameter.AvoidSpeed, playerParameter.avoidParameter.AvoidDistance, avoidDirection_Verticalz, avoidDirection_Horizontal);
    }

    public void IdleState()
    {
        playerState = PlayerState.Move;
        
    }

    public void JumpState()
    {
        playerState = PlayerState.Jump;
       
    }

    public void DoubleJumpState()
    {
        playerState = PlayerState.DoubleJump;          
    }
       */

    public void AddForce(int JumpState)
    {
        switch (JumpState)
        {
            case (int)PlayerState.Jump:
                AddVerticalForce(playerRigidbody, playerParameter.jumpParameter.JumpHigh);
                StartCoroutine("JumpTriggerLandingCheck");
                break;
            case (int)PlayerState.DoubleJump:
                playerRigidbody.mass = 10;
                playerRigidbody.velocity = Vector3.zero;
                playerRigidbody.mass = 500;
                AddVerticalForce(playerRigidbody, playerParameter.jumpParameter.DoubleJumpHigh);
                break;
        }
       
    }

    IEnumerator JumpTriggerLandingCheck()
    {
        Debug.Log("Tr");
        yield return new WaitForSeconds(0.01f);

        if (animationHash.GetCurrentAnimationState("Jump"))
        {
            if (isGround)
            {
                StopCoroutine("JumpTriggerLandingCheck");
                StartCoroutine("JumpTriggerLandingCheck");
            }
            else
            {
                StartCoroutine("LandingCheck");
                StopCoroutine("JumpTriggerLandingCheck");
                Debug.Log("Tr_Landing Check");
            }
        }
        else
        {
            StopCoroutine("JumpGroundedCheck");
            
        }
       
    }

     IEnumerator LandingCheck()
    {
        yield return new WaitForSeconds(0.01f);
        Debug.Log("Lc");
        if (isGround)
        {            
            if (animationHash.GetCurrentAnimationState("Idle_Run"))
            {
                playerAnimator.ResetTrigger("Idle");
                playerState = PlayerState.Move;
                StopCoroutine("LandingCheck");
                Debug.Log("Lc_Change State");
            }
            else
            {
                playerAnimator.SetTrigger("Idle");
                StartCoroutine("LandingCheck");
                Debug.Log("Lc_trigger IdleState");
            } 
        }
        else
        {
            StartCoroutine("LandingCheck");
           // FallingMove();
            
        }

    }

    #endregion
}