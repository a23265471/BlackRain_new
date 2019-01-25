using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerBehaviour : Character
{
    public enum PlayerState
    {
        Move,
        Dash,
        Jump,
        Attack,
        Avoid,          
        Damage,
        GetDown,
        GetUp,
        Dead,
    }

    public PlayerState playerState;
    public static PlayerBehaviour playerBehaviour;

    private GameStageData gameStageData;
    private PlayerController playerController;
    private Animator playerAnimator;
    private Rigidbody playerRigidbody;
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
        Rotaion();        
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
        AddVerticalForce(playerRigidbody, playerParameter.jumpParameter.JumpHigh);
    }


    #region AnimationEvent
    public void AvoidState()
    {
        playerState = PlayerState.Avoid;
        Displacement(transform, playerParameter.avoidParameter.AvoidSpeed, playerParameter.avoidParameter.AvoidDistance, avoidDirection_Verticalz, avoidDirection_Horizontal);
    }

    public void IdleState()
    {
        playerState = PlayerState.Move;
        Debug.Log("walk");
    }
    #endregion
}