using System.Collections;
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
        FallingMove = Falling | Jump | DoubleJump | SkyAttack,
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
    private float moveDirection;

    private float moveAnimation_Vertical;
    private float moveAnimation_Horizontal;
    private float FallindAniamtion_Vertical;
    private float FallindAniamtion_Horizontal;
    public float MoveAnimationSmoothSpeed;
    public float JumpMoveAnimationSmoothSpeed;
    private int avoidDirection_X;
    private int avoidDirection_Z;
    private float avoidSpeed;

    private float curJumpMoveSpeedCurveTime;
    #endregion

    #region 物理碰撞   
    private Rigidbody playerRigidbody;
    public CapsuleCollider[] curPhysicsCollider;
    private CapsuleCollider idlePhysicsCollider;
    private CapsuleCollider smallPhysicsCollider;
    private CapsuleCollider mediumPhysicsCollider;

    public float groundedDis;
    public bool isGround;
    public bool isNotGraoundStep;
    private float curGravity;
    public float GravityAcceleration;
    private bool isGravity;
    //private bool useGravity;
    #endregion

    private void Awake()
    {
        playerBehaviour = this;
        playerAnimator = GetComponent<Animator>();
        playerAudioSource = GetComponent<AudioSource>();
        curPhysicsCollider = GetComponents<CapsuleCollider>();
        animationHash = GetComponent<AnimationHash>();
        playerRigidbody = GetComponent<Rigidbody>();

        gameStageData = GameFacade.GetInstance().gameStageData;
        playerController = GameFacade.GetInstance().playerController;
        playerParameter = gameStageData.CurPlayerStageData.playerData.playerParameter;

        floorMask = LayerMask.GetMask("Floor");
        playerState = PlayerState.Move;
        useGravity = true;
        isGravity = false;
        isNotGraoundStep = false;
    }

    void Start()
    {
        moveAnimation_Vertical = 0;
        moveAnimation_Horizontal = 0;
        cameraLookAt= gameObject.transform.Find("CameraLookAt");
       
    }
     
    void Update()
    {
       
       // physicsCollider.height = playerAnimator.GetFloat("ColliderHeight");
        Rotaion();
        // Debug.Log(isGround);
        // Debug.Log(animationHash.GetAnimationState("Jump"));
       //  Debug.Log(playerRigidbody.velocity);

    }

    private void FixedUpdate()
    {
        isGround = Physics.Raycast(curPhysicsCollider[0].bounds.center, -Vector3.up, curPhysicsCollider[0].bounds.extents.y + groundedDis,floorMask);
      //  Debug.DrawLine(curPhysicsCollider[0].bounds.center, -transform.up * (curPhysicsCollider[0].bounds.extents.y + groundedDis), Color.green);
        

        if (playerState == PlayerState.Jump)
        {
           // Debug.Log(Time.time);
        }

       // Debug.Log(curJumpMoveSpeedCurveTime);
        

        // Debug.Log(useGravity);
        if (useGravity)
        {

            if (!isGround)
            {               
                Gravity();

            }
            else
            {
                isGravity = false;

            }
        }

    }

    public void Gravity()
    {
        if (!isGravity)
        {
            isGravity = true;
            StopRigiBodyMoveWithAniamtionCurve_Y();
            Keyframe gravityKey = playerParameter.jumpParameter.GravityCurve.keys[playerParameter.jumpParameter.GravityCurve.keys.Length - 1];
            RigiBodyMoveWithAniamtionCurve_Y(playerRigidbody, playerParameter.jumpParameter.GravityCurve, 0, gravityKey.time, 12, playerParameter.jumpParameter.GravityPerIntervalTime);

        }

    }

    public void StopUseGravity()
    {
        useGravity = false;
        isGravity = false;
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

        
        transform.rotation = Quaternion.Euler(0, rotation_Horizontal, 0);
        
    }

    public void GroundedMove(float moveDirection_Vertical, float moveDirection_Horizontal)
    {
        float MoveX;
        float MoveZ;
        if (isGround)
        {
            if ((playerState & PlayerState.Move) != 0)
            {
                //  Debug.Log(playerRigidbody.velocity);
                AnimationBlendTreeControll(playerAnimator, "Vertical", moveDirection_Vertical, ref moveAnimation_Vertical, MoveAnimationSmoothSpeed);
                AnimationBlendTreeControll(playerAnimator, "Horizontal", moveDirection_Horizontal, ref moveAnimation_Horizontal, MoveAnimationSmoothSpeed);
                moveSpeed = playerParameter.moveParameter.RunSpeed;

                if (moveDirection_Vertical == 0 || moveDirection_Horizontal == 0)
                {
                    curMoveSpeed = Mathf.Sqrt((Mathf.Pow(playerParameter.moveParameter.RunSpeed, 2) * 2));
                }
                else
                {
                    curMoveSpeed = playerParameter.moveParameter.RunSpeed;
                }
               
            }
            MoveX = moveAnimation_Horizontal * curMoveSpeed;
            MoveZ = moveAnimation_Vertical * curMoveSpeed;

            playerRigidbody.velocity = transform.rotation * new Vector3(MoveX, playerRigidbody.velocity.y, MoveZ);
        }
        
       
    }
    public void Falling(int moveDirection_Vertical, int moveDirection_Horizontal)
    {
        if (!isGround)
        {
            if ((playerState & PlayerState.CanFalling) != 0)
            {
               // Debug.Log("Falling");
                playerAnimator.SetTrigger("Falling");
               
            }
            FallingAniamtion(moveDirection_Vertical, moveDirection_Horizontal);
        }

    }
    public void FallingAniamtion(int moveDirection_Vertical, int moveDirection_Horizontal)
    {
        float direction_X = moveDirection_Horizontal;
        float direction_Y = moveDirection_Vertical;
        direction_Y = Mathf.Clamp(direction_Y, 0, 1);
        if ((playerState & PlayerState.FallingMove) != 0)
        {
            if (moveDirection_Vertical == 1)
            {
                switch (moveDirection_Horizontal)
                {
                    case -1:
                        direction_X = -0.5f;
                        break;
                    case 1:
                        direction_X = 0.5f;
                        break;
                }

            }
            // Debug.Log(FallindAniamtion_Vertical);       
            AnimationBlendTreeControll(playerAnimator, "Vertical", moveDirection_Vertical, ref moveAnimation_Vertical, MoveAnimationSmoothSpeed);
            AnimationBlendTreeControll(playerAnimator, "Horizontal", moveDirection_Horizontal, ref moveAnimation_Horizontal, MoveAnimationSmoothSpeed);
            AnimationBlendTreeControll(playerAnimator, "Jump_Rotation", direction_X, ref FallindAniamtion_Horizontal, JumpMoveAnimationSmoothSpeed);
        }

        float fallingMoveX = moveDirection_Horizontal * playerParameter.jumpParameter.JumpMoveSpeed;
        float fallingMoveZ = direction_Y * playerParameter.jumpParameter.JumpMoveSpeed;
     
        playerRigidbody.velocity = transform.rotation * new Vector3(fallingMoveX, playerRigidbody.velocity.y, fallingMoveZ);
   
    }

    public void FallingMove()
    {


    }
    #endregion

    #region 迴避
    public void Avoid(int moveDirection_Vertical,int moveDirection_Horizontal)
    {
        string xDirection;
        string zDirection;
        
        if ((playerBehaviour.playerState & PlayerState.CanAvoid) != 0)
        {
            avoidDirection_X = moveDirection_Horizontal;
            avoidDirection_Z = moveDirection_Vertical;

            AnimationRotation(moveDirection_Vertical, moveDirection_Horizontal);

            if (moveDirection_Vertical == 0 || moveDirection_Horizontal == 0)
            {
                avoidSpeed = Mathf.Sqrt((Mathf.Pow(playerParameter.avoidParameter.AvoidSpeed, 2) * 2));
            }
            else
            {
                avoidSpeed = playerParameter.avoidParameter.AvoidSpeed;
            }

            playerAnimator.SetTrigger("Avoid");
          

        }
                          
       
    }

   
    #endregion

    private void AnimationRotation(int moveDirection_Vertical, int moveDirection_Horizontal)
    {      
        if (moveDirection_Vertical == 1)
        {           
            playerAnimator.SetBool("Forward", true);
            playerAnimator.SetBool("Back", false);

        }
        else if (moveDirection_Vertical == -1)
        {            
            playerAnimator.SetBool("Forward", false);
            playerAnimator.SetBool("Back", true);
        }
        else
        {           
            playerAnimator.SetBool("Forward", false);
            playerAnimator.SetBool("Back", false);
        }

        if (moveDirection_Horizontal == 1)
        {           
            playerAnimator.SetBool("Left", false);
            playerAnimator.SetBool("Right", true);
        }
        else if (moveDirection_Horizontal == -1)
        {            
            playerAnimator.SetBool("Right", false);
            playerAnimator.SetBool("Left", true);
        }
        else
        {           
            playerAnimator.SetBool("Right", false);
            playerAnimator.SetBool("Left", false);
        }
      
    }

    public void Jump(int moveDirection_Vertical, int moveDirection_Horizontal)
    {
        AnimationRotation(moveDirection_Vertical, moveDirection_Horizontal);
        if (((playerBehaviour.playerState & PlayerState.Move) != 0)) 
        {
           // Debug.Log("Jump");           
            playerAnimator.SetTrigger("Jump");

        }
        else if(((playerBehaviour.playerState & PlayerState.CanDoubleJump)!=0) && !isGround)
        {
            //Debug.Log("Double");
            playerAnimator.SetTrigger("DoubleJump");

        }

    }

    #region AnimationEvent

    public void ChangePlayerState(int ChangePlayerState)
    {
     /*   playerAnimator.SetFloat("Horizontal", 0);
        playerAnimator.SetFloat("Vertical", 0);
        
        moveAnimation_Vertical = 0;
        moveAnimation_Horizontal = 0;*/
        switch (ChangePlayerState)
        {
            case (int)PlayerState.Move:               
                playerState = PlayerState.Move;
                break;

            case (int)PlayerState.Jump:
                playerState = PlayerState.Jump;
             //   playerRigidbody.velocity = new Vector3(0, 0, 0);
                break;

            case (int)PlayerState.DoubleJump:
                playerState = PlayerState.DoubleJump;                  
                break;

            case (int)PlayerState.Avoid:
                playerState = PlayerState.Avoid;               
                Displacement(playerRigidbody, transform.rotation, avoidSpeed, playerParameter.avoidParameter.AvoidDistance, avoidDirection_X, 0, avoidDirection_Z,true);                
                break;

            case (int)PlayerState.Falling:
               // Debug.Log(playerState);
                if ((playerState & PlayerState.Falling) != 0)
                {
                    playerState = PlayerState.Falling;
                    StartLandingCheck();
                    //  Debug.Log("hh");
                    playerAnimator.ResetTrigger("Falling");

                }

                break;
        }
    } 

    public void AddForce(int JumpState)
    {
        switch (JumpState)
        {
            case (int)PlayerState.Jump:
                Keyframe jumpEndKey = playerParameter.jumpParameter.JumpCurve.keys[playerParameter.jumpParameter.JumpCurve.keys.Length - 1];
                StopUseGravity();
                StopRigiBodyMoveWithAniamtionCurve_Y();
                RigiBodyMoveWithAniamtionCurve_Y(playerRigidbody,playerParameter.jumpParameter.JumpCurve, 0, jumpEndKey.time, 12, playerParameter.jumpParameter.JumpPerIntervalTime);              
                break;

            case (int)PlayerState.DoubleJump:
                Keyframe doubleJumpEndKey = playerParameter.jumpParameter.DoubleJumpCurve.keys[playerParameter.jumpParameter.DoubleJumpCurve.keys.Length - 1];
                StopRigiBodyMoveWithAniamtionCurve_Y();
                StopUseGravity();
                // playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
                RigiBodyMoveWithAniamtionCurve_Y(playerRigidbody, playerParameter.jumpParameter.DoubleJumpCurve, 0, doubleJumpEndKey.time, 12, playerParameter.jumpParameter.JumpPerIntervalTime);
                StartLandingCheck();
                break;
        }
       
    }

    public void StartLandingCheck()
    {
       // Debug.Log("dd");

        StopCoroutine("LandingCheck");
        StartCoroutine("LandingCheck");
    }

    IEnumerator LandingCheck()
    {
        yield return new WaitForSeconds(0.01f);
      //  Debug.Log(animationHash.GetCurrentAnimationState("Idle_Run"));
       // Debug.Log(playerState);
        if (isGround)
        {            
            if (animationHash.GetCurrentAnimationState("Idle_Run"))
            {
            //    Debug.Log("Idle");

                playerAnimator.ResetTrigger("Idle");
                playerState = PlayerState.Move;
                StopRigiBodyMoveWithAniamtionCurve_Y();
                FallindAniamtion_Vertical = 0;
                FallindAniamtion_Horizontal = 0;
                StopCoroutine("LandingCheck");
               
            }
            else
            {
           //     Debug.Log("Trigger Idle");

                playerAnimator.SetTrigger("Idle");
                StartCoroutine("LandingCheck");
                
            } 
        }
        else
        {
            StartCoroutine("LandingCheck");
            Debug.Log("LC");

           // FallingMove();
            
        }

    }

    #endregion
}