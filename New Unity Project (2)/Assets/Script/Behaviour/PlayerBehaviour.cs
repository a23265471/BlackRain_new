using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerBehaviour : Character
{

    private GameStageData gameStageData;
    private PlayerController playerController;
    private Animator animator;
    private Rigidbody rigidbody;
    private AudioSource audioSource;

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
    #endregion
    private void Awake()
    {

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        physicsCollider = GetComponent<CapsuleCollider>();

        gameStageData = GameFacade.GetInstance().gameStageData;
        playerController = GameFacade.GetInstance().playerController;

    }

    void Start()
    {
        moveAnimation_Vertical = 0;
        moveAnimation_Horizontal = 0;
        cameraLookAt= gameObject.transform.Find("CameraLookAt");

    }

    // Update is called once per frame
    void Update()
    {
        //       playerController.Move(this);
        Debug.Log(moveAnimation_Vertical);
        Debug.Log(moveAnimation_Horizontal);

    }
    private void FixedUpdate()
    {
        Rotaion();


    }


    public void PlayerMove(float moveDirection_Vertical, float moveDirection_Horizontal)
    {
        AnimationBlendTreeControll(animator, "Vertical", moveDirection_Vertical, ref moveAnimation_Vertical, MoveAnimationSmoothSpeed);
        AnimationBlendTreeControll(animator, "Horizontal", moveDirection_Horizontal, ref moveAnimation_Horizontal, MoveAnimationSmoothSpeed);

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            curMoveSpeed = Mathf.Lerp(curMoveSpeed, gameStageData.CurPlayerStageData.playerData.playerParameter.moveParameter.RunSpeed, 0.03f);
            curMoveSpeed = Mathf.Clamp(curMoveSpeed, 0, gameStageData.CurPlayerStageData.playerData.playerParameter.moveParameter.RunSpeed);
        }
        else if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            curMoveSpeed = Mathf.Lerp(curMoveSpeed, 0, 0.1f);
            if (curMoveSpeed <= 0.06f && curMoveSpeed >= -0.06f)
            {
                curMoveSpeed = 0;
            }
        }

        if (Input.GetAxis("Horizontal") == 0 || Input.GetAxis("Vertical") == 0)
        {
            curMoveSpeed = Mathf.Sqrt((Mathf.Pow(curMoveSpeed, 2) * 2));
        }
       
        float MoveX = Input.GetAxis("Horizontal") * Time.deltaTime * curMoveSpeed;
        float MoveZ = Input.GetAxis("Vertical") * Time.deltaTime * curMoveSpeed;

        //Debug.Log(Input.GetAxis("Horizontal"));
        transform.Translate(MoveX, 0, MoveZ);
    }
    private void Rotaion()
    {
        rotation_Horizontal += Input.GetAxis("Mouse X") * Time.deltaTime * gameStageData.CurPlayerStageData.playerData.playerParameter.moveParameter.RotateSpeed;

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

}