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
        //如更改順序,記得到該動作animation更改ChangePlayerState的int
        Move = 0x01,
        Jump = 0x02,
        DoubleJump = Move|Jump|Fall,
        Fall = 0x04,
        Dash = Move|Attack|Jump,
        Attack = 0x08,
        Skill = Attack|Move,
        Avoid = Move|Attack,

        Damage,
        GetDown,
        GetUp,
        Dead,
    }

    [Flags]
    public enum aa
    {
        ss=0x00,
        dd=0x01,
        qq=0x02,
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

    static void main()
    {


    }


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

        aa DoubleJump = aa.dd | aa.qq;
        PlayerState sss = PlayerState.DoubleJump | PlayerState.Move;

        
        Debug.Log(DoubleJump);
        Debug.Log(sss);
       

       
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
        
        Debug.Log(animationHash.GetAnimationState("Jump"));

    }

    #region 移動
    private void Rotaion()
    {
        rotation_Horizontal += Input.GetAxis("Mouse X") * Time.deltaTime * playerParameter.moveParameter.RotateSpeed;

        if (rotation_Horizontal > 360)
        {
            rotation_Horizontal -= 360;
        }
        else if (rotation_Horizontal < 0)
        {
            rotation_Horizontal += 360;
        }

        /*if ((int)playerState < 4)
        {*/
            transform.rotation = Quaternion.Euler(0, rotation_Horizontal, 0);
        //}
    }

    public void PlayerMove(float moveDirection_Vertical, float moveDirection_Horizontal)
    {
        AnimationBlendTreeControll(playerAnimator, "Vertical", moveDirection_Vertical, ref moveAnimation_Vertical, MoveAnimationSmoothSpeed);
        AnimationBlendTreeControll(playerAnimator, "Horizontal", moveDirection_Horizontal, ref moveAnimation_Horizontal, MoveAnimationSmoothSpeed);

        if(moveAnimation_Vertical==0 || moveAnimation_Horizontal == 0)
        {
            curMoveSpeed = Mathf.Sqrt((Mathf.Pow(playerParameter.moveParameter.RunSpeed, 2) * 2));
        }
        else
        {
            curMoveSpeed = playerParameter.moveParameter.RunSpeed;
        }

        float MoveX = moveAnimation_Horizontal * Time.deltaTime * curMoveSpeed;
        float MoveZ = moveAnimation_Vertical * Time.deltaTime * curMoveSpeed;
        transform.Translate(MoveX, 0, MoveZ);
    }
    #endregion

    #region 迴避
    public void Avoid()
    {       
        string xDirection;
        string zDirection;

        avoidDirection_Horizontal = playerController.moveDirection_Horizontal;
        avoidDirection_Verticalz = playerController.moveDirection_Vertical;

        if (playerController.moveDirection_Vertical == 1)
        {
            zDirection = "Forward";
        }
        else if (playerController.moveDirection_Vertical == -1) 
        {
            zDirection = "Back";
        }
        else
        {
            zDirection = "";
        }

        if (playerController.moveDirection_Horizontal == 1)
        {
            xDirection = "Right";
        }
        else if (playerController.moveDirection_Horizontal == -1)
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


    #region AnimationEvent

    public void ChangePlayerState(int ChangePlayerState)
    {     
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
            
        }
    } 

    public void AvoidState()
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

   /* public void FallingState()
    {
        if (playerState != PlayerState.Falling)
        {
            playerState = PlayerState.Falling;
            Debug.Log("start");
            StartCoroutine("GroundedCheck");
        }
       
    }*/

   /* IEnumerator GroundedCheck()
    {
        yield return new WaitForSeconds(0.01f);
        if (isGround)
        {
            
            playerAnimator.SetTrigger("Idle");
            if (playerState == PlayerState.Falling)
            {
                playerState = PlayerState.Move;
            }
            StopCoroutine("GroundedCheck");

        }
        else
        {
            Debug.Log("Falling");
            StartCoroutine("GroundedCheck");
        }
    }*/

    public void AddForce(int JumpState)
    {
        switch (JumpState)
        {
            case (int)PlayerState.Jump:
                AddVerticalForce(playerRigidbody, playerParameter.jumpParameter.JumpHigh);
                break;
            case (int)PlayerState.DoubleJump:
                playerRigidbody.mass = 10;
                playerRigidbody.velocity = Vector3.zero;
                playerRigidbody.mass = 500;
                AddVerticalForce(playerRigidbody, playerParameter.jumpParameter.DoubleJumpHigh);
                break;
        }
       
    }

    #endregion
}