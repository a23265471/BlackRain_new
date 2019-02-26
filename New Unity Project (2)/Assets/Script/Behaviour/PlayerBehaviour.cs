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
        FallingMove = Falling | Jump | DoubleJump,
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
    public float MoveAnimationSmoothSpeed;
    private int avoidDirection_X;
    private int avoidDirection_Z;
    private float avoidSpeed;
    
    #endregion

    #region 物理碰撞   
    private Rigidbody playerRigidbody;
    public CapsuleCollider physicsCollider;
    public float groundedDis;
    public bool isGround;
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
        physicsCollider = GetComponent<CapsuleCollider>();
        animationHash = GetComponent<AnimationHash>();
        playerRigidbody = GetComponent<Rigidbody>();

        gameStageData = GameFacade.GetInstance().gameStageData;
        playerController = GameFacade.GetInstance().playerController;
        playerParameter = gameStageData.CurPlayerStageData.playerData.playerParameter;

        floorMask = LayerMask.GetMask("Floor");
        playerState = PlayerState.Move;
        useGravity = true;
        isGravity = false;
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
       // Debug.Log(playerRigidbody.velocity);

    }

    private void FixedUpdate()
    {
        isGround = Physics.Raycast(physicsCollider.bounds.center, -Vector3.up, physicsCollider.bounds.extents.y + groundedDis, floorMask);

        if (playerState == PlayerState.Jump)
        {
         //  Debug.Log(playerRigidbody.velocity);
        }

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
            StopRigiBodyMoveWithAniamtionCurve();
            Keyframe gravityKey = playerParameter.jumpParameter.GravityCurve.keys[playerParameter.jumpParameter.GravityCurve.keys.Length - 1];
            RigiBodyMoveWithAniamtionCurve(playerRigidbody, playerParameter.jumpParameter.GravityCurve, 0, gravityKey.time, 12, playerParameter.jumpParameter.GravityPerIntervalTime);

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
        if (((playerBehaviour.playerState & PlayerState.Move) != 0)) 
        {
            Debug.Log(playerRigidbody.velocity);

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

            float MoveX = moveAnimation_Horizontal  * curMoveSpeed;
            float MoveZ = moveAnimation_Vertical * curMoveSpeed;


            playerRigidbody.velocity = transform.rotation * new Vector3(MoveX, 0, MoveZ)  ;

        }

    }

    public void FallingMove(float moveDirection_Vertical, float moveDirection_Horizontal)
    {
       
    }
    public void Falling()
    {
        if (!isGround)
        {
            if ((playerState & PlayerState.CanFalling) != 0)
            {
               // Debug.Log("Falling");
                playerAnimator.SetTrigger("Falling");

            }
        }
       
       
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

            if (moveDirection_Vertical == 0 || moveDirection_Horizontal == 0)
            {
                avoidSpeed = Mathf.Sqrt((Mathf.Pow(playerParameter.avoidParameter.AvoidSpeed, 2) * 2));
            }
            else
            {
                avoidSpeed = playerParameter.avoidParameter.AvoidSpeed;
            }
        

            AvoidAnimatorTrigger(xDirection, zDirection);

        }
                          
       
    }

    private void AvoidAnimatorTrigger(string Horizontal_Direction, string Vertical_Direction)
    {
        playerAnimator.ResetTrigger("Avoid" + Vertical_Direction + Horizontal_Direction);
        playerAnimator.SetTrigger("Avoid" + Vertical_Direction + Horizontal_Direction);             
    }
    #endregion

    public void Jump()
    {
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
                playerRigidbody.velocity = new Vector3(0, 0, 0);
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
                if ((playerState & PlayerState.CanFalling) != 0) 
                {
                    playerState = PlayerState.Falling;
                    playerAnimator.ResetTrigger("Falling");
                    StartCoroutine("LandingCheck");
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
                StopRigiBodyMoveWithAniamtionCurve();
                RigiBodyMoveWithAniamtionCurve(playerRigidbody,playerParameter.jumpParameter.JumpCurve, 0, jumpEndKey.time, 12, playerParameter.jumpParameter.JumpPerIntervalTime);              
                break;

            case (int)PlayerState.DoubleJump:
                Keyframe doubleJumpEndKey = playerParameter.jumpParameter.DoubleJumpCurve.keys[playerParameter.jumpParameter.DoubleJumpCurve.keys.Length - 1];
                StopRigiBodyMoveWithAniamtionCurve();
                StopUseGravity(); 
               // playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
                RigiBodyMoveWithAniamtionCurve(playerRigidbody, playerParameter.jumpParameter.DoubleJumpCurve, 0, doubleJumpEndKey.time, 12, playerParameter.jumpParameter.JumpPerIntervalTime);
                StartLandingCheck();
                break;
        }
       
    }

    public void StartLandingCheck()
    {
        StopCoroutine("LandingCheck");
        StartCoroutine("LandingCheck");
    }

    IEnumerator LandingCheck()
    {
        yield return new WaitForSeconds(0.01f);
       // Debug.Log("LandingCheck");
       // Debug.Log(playerState);
        if (isGround)
        {            
            if (animationHash.GetCurrentAnimationState("Idle_Run"))
            {
                playerAnimator.ResetTrigger("Idle");
                playerState = PlayerState.Move;
                StopRigiBodyMoveWithAniamtionCurve();
                StopCoroutine("LandingCheck");
               
            }
            else
            {
                playerAnimator.SetTrigger("Idle");
                StartCoroutine("LandingCheck");
                
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