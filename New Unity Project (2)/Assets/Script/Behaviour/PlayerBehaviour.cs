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

    private void FixedUpdate()
    {
       
    }

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

        transform.rotation = Quaternion.Euler(0, rotation_Horizontal, 0);
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

    public void Avoid(int moveDirection_Vertical,int moveDirection_Horizontal)
    {       
        string xDirection;
        string zDirection;

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

        if (moveAnimation_Vertical == 0 || moveAnimation_Horizontal == 0)
        {
            avoidSpeed = Mathf.Sqrt((Mathf.Pow(avoidSpeed, 2) * 2));
        }
        Displacement(transform, playerParameter.avoidParameter.AvoidSpeed, playerParameter.avoidParameter.AvoidDistance, moveDirection_Vertical, moveDirection_Horizontal);
        
    }

    private void AvoidAnimatorTrigger(string Horizontal_Direction, string Vertical_Direction)
    {
        playerAnimator.SetTrigger("Avoid" + Vertical_Direction + Horizontal_Direction);
        
       
    }

    
}