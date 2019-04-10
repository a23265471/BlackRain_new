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

        CanFalling = Move | Attack | Dash | SkyAttack ,
        FallingMove = Falling | Jump | DoubleJump | SkyAttack,
        CanDoubleJump = Jump | Falling | Dash,
        CanDash = Move | Attack | Jump | DoubleJump | SkyAttack | Falling,
        CanDashAttack = CanDash,
        CanSkyAttack = Jump | Falling | DoubleJump,
        CanAvoid = Move | Attack | Skill,
        CanAttack = Attack | Move | Avoid,
        CanSkill = Attack | Move,
        DoNotGroudedMove = Attack | Skill | SkyAttack | Avoid,
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
    private PlayerData playerData;
    private AnimationHash animationHash;
    private ParticleManager particleManager;
    private AttackSystem attackSystem;

    public GhostShadow PlayerShader;

   // public GameObject GroundCheckObject;
    public Gravity gravity;

    IEnumerator detectAnimationStateNotAttack;

    #region 圖層
    int floorMask;
    #endregion

    #region 子物件
    public Transform cameraLookAt;
    public Transform GetWeaponHand;
    public Transform WeaponPos;
    #endregion

    #region 移動參數
    private float rotation_Horizontal;
    private float curMoveSpeed;
    private float moveSpeed;

    private float moveAnimation_Vertical;
    private float moveAnimation_Horizontal;
    private float FallindAniamtion_Horizontal;
    public float MoveAnimationSmoothSpeed;
    public float JumpMoveAnimationSmoothSpeed;
    private int jumpCount;
    private int Direction_X;
    private int Direction_Z;
    private float avoidSpeed;
    private bool canfall;
    private bool canTriggerFallingCoroutine;

    //private ParticleSystem jumpParticleSytem;
    // public Transform jumpParticleTransform;
    #endregion

    #region 物理碰撞   
    private Rigidbody playerRigidbody;
    public CapsuleCollider GroudedCollider;
    public GameObject IdlePhysicsCollider;
    public GameObject SmallPhysicsCollider;
    public GameObject MediumPhysicsCollider;

    public float groundedDis;
  //  public bool isGround;
    //public bool isNotGraoundStep;

    private bool ForceMove;
    #endregion

    #region 攻擊

    public bool CanTriggerNextAttack;
    private bool isTriggerAttack;
  
    #endregion

    private void Awake()
    {
        playerBehaviour = this;
        playerAnimator = GetComponent<Animator>();
        playerAudioSource = GetComponent<AudioSource>();
        GroudedCollider = GetComponent<CapsuleCollider>();
        animationHash = GetComponent<AnimationHash>();
        playerRigidbody = GetComponent<Rigidbody>();
        particleManager = GetComponent<ParticleManager>();
        attackSystem = GetComponent<AttackSystem>();
        gravity = GetComponent<Gravity>();
        PlayerShader.enabled = false;
        //  groundCheck = GroundCheckObject.GetComponent<GroundCheck>();

        gameStageData = GameFacade.GetInstance().gameStageData;
        playerController = GameFacade.GetInstance().playerController;
        playerParameter = gameStageData.CurPlayerStageData.playerData.playerParameter;
        playerData = gameStageData.CurPlayerStageData.playerData;

        floorMask = LayerMask.GetMask("Floor");
        playerState = PlayerState.Move;
        CanTriggerNextAttack = true;
        detectAnimationStateNotAttack = null;
        ForceMove = false;
        isTriggerAttack = false;
        canfall = true;
        canTriggerFallingCoroutine = true;
        CreateWeapon();
      //  CreateParticle();
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
      //  Debug.Log(playerRigidbody.velocity.y);
    }

    private void FixedUpdate()
    {
            
    }

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

    #region 初始化
    private void CreateWeapon()
    {
        GameObject weapon = Instantiate(playerData.Weapon, WeaponPos.position, WeaponPos.rotation, GetWeaponHand);

    }

    #endregion

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

    public void GroundedMove(int moveDirection_Vertical, int moveDirection_Horizontal)
    {
        float MoveX;
        float MoveZ;
        if (gravity.groundCheck.IsGround)
        {      
            AnimationBlendTreeControll(playerAnimator, "Vertical", moveDirection_Vertical, ref moveAnimation_Vertical, MoveAnimationSmoothSpeed);
            AnimationBlendTreeControll(playerAnimator, "Horizontal", moveDirection_Horizontal, ref moveAnimation_Horizontal, MoveAnimationSmoothSpeed);
            moveSpeed = playerParameter.moveParameter.RunSpeed;

            if (ForceMove || (playerState & PlayerState.DoNotGroudedMove) == 0) 
            {
               
                curMoveSpeed = FixSpeed(moveDirection_Vertical, moveDirection_Horizontal, playerParameter.moveParameter.RunSpeed);
                MoveX = moveAnimation_Horizontal * curMoveSpeed;
                MoveZ = moveAnimation_Vertical * curMoveSpeed ;
               
                playerRigidbody.velocity = transform.rotation * new Vector3(MoveX, playerRigidbody.velocity.y, MoveZ);
            }            
        }
    }

    public void Falling(int moveDirection_Vertical, int moveDirection_Horizontal)
    {      
        if (!gravity.groundCheck.IsGround)
        {
            /*  if ((playerState & PlayerState.Move) != 0)
              {
                  playerAnimator.SetTrigger("Falling");
                 // playerState = PlayerState.Falling;
              }*/
            if((playerState & PlayerState.Move) != 0)
            {
                if (canTriggerFallingCoroutine)
                {
                    StopCoroutine("StartFalling");
                    StartCoroutine("StartFalling");
                }
            }
            FallingAniamtion(moveDirection_Vertical, moveDirection_Horizontal);
        }

    }

    IEnumerator StartFalling()
    {
        canTriggerFallingCoroutine = false;
        yield return new WaitForSeconds(0.1f);       
        if (!gravity.groundCheck.IsGround)
        {
            playerAnimator.SetTrigger("Falling");         
            // playerState = PlayerState.Falling;
            Debug.Log("Start Falling");
        }
        else
        {           
            Debug.Log("Stop Falling");
        }
        canTriggerFallingCoroutine = true;

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
            AnimationBlendTreeControll(playerAnimator, "Vertical", moveDirection_Vertical, ref moveAnimation_Vertical, MoveAnimationSmoothSpeed);
            AnimationBlendTreeControll(playerAnimator, "Horizontal", moveDirection_Horizontal, ref moveAnimation_Horizontal, MoveAnimationSmoothSpeed);
            AnimationBlendTreeControll(playerAnimator, "Jump_Rotation", direction_X, ref FallindAniamtion_Horizontal, JumpMoveAnimationSmoothSpeed);

            float fallingMoveX = moveDirection_Horizontal * playerParameter.jumpParameter.JumpMoveSpeed;
            float fallingMoveZ = direction_Y * playerParameter.jumpParameter.JumpMoveSpeed;

            playerRigidbody.velocity = transform.rotation * new Vector3(fallingMoveX, playerRigidbody.velocity.y, fallingMoveZ);

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
        else if (((playerBehaviour.playerState & PlayerState.CanDoubleJump) != 0) && !gravity.groundCheck.IsGround && jumpCount < playerParameter.jumpParameter.SkyJumpCount) 
        {
            //Debug.Log("Double");
            playerAnimator.SetTrigger("DoubleJump");
            gravity.StopGroundCheck();

        }

    }

    private float FixSpeed(int moveDirection_Vertical, int moveDirection_Horizontal, float value)
    {

        if (moveDirection_Vertical == 0 || moveDirection_Horizontal == 0)
        {
            return Mathf.Sqrt((Mathf.Pow(value, 2) * 2));
        }
        else
        {
            return value;
        }
 
    }
   
    #endregion

    #region 迴避
    public void Avoid(int moveDirection_Vertical,int moveDirection_Horizontal)
    {
       
        if ((playerBehaviour.playerState & PlayerState.CanAvoid) != 0)
        {
            Direction_X = moveDirection_Horizontal;
            Direction_Z = moveDirection_Vertical;

            AnimationRotation(moveDirection_Vertical, moveDirection_Horizontal);

            
            avoidSpeed = FixSpeed(moveDirection_Vertical, moveDirection_Horizontal, playerParameter.avoidParameter.AvoidSpeed);

      //      Debug.Log(avoidSpeed);

            playerAnimator.SetTrigger("Avoid");
          

        }
                          
       
    }


    #endregion

    #region 攻擊

    public void NormalAttack()
    {
        if (gravity.groundCheck.IsGround)
        {
            if (CanTriggerNextAttack && ((playerState& PlayerState.CanAttack)!=0))
            {
                // Debug.Log("gggg");
                /* playerAnimator.SetTrigger("NormalAttack");
                 CanTriggerNextAttack = false;
                 isTriggerAttack = true;*/

                attackSystem.NormalAttack("NormalAttack");
            }

        }

    }

    #endregion

    #region 衝刺

    public void Dash(int moveDirection_Vertical, int moveDirection_Horizontal)
    {
        
        if (moveDirection_Horizontal == 0 && moveDirection_Vertical == 0) 
        {
            Direction_Z = 1;
            Direction_X = 0;
        }
        else
        {
            if (moveDirection_Vertical < 0)
            {
                Direction_Z = 0;
            }
            else
            {
                Direction_Z = moveDirection_Vertical;
            }

            Direction_X = moveDirection_Horizontal;


        }
        if(moveDirection_Vertical>=0)
        if ((playerState & PlayerState.CanDash) != 0)
        {
            // Debug.Log("jjj");
            AnimationRotation(Direction_Z, Direction_X);
            playerAnimator.SetTrigger("Dash");
        } 
       

    }

    #endregion

    #region AnimationEvent
    public void ChangePlayerState(int ChangePlayerState)
    {    
        switch (ChangePlayerState)
        {
            case (int)PlayerState.Move:        
                
                playerState = PlayerState.Move;
                SwitchCollider(0);
               // CanTriggerNextAttack = true;
                break;

            case (int)PlayerState.Jump:
                SwitchCollider(1);
                playerState = PlayerState.Jump;           
                break;

            case (int)PlayerState.DoubleJump:
                SwitchCollider(2);
                jumpCount += 1;

                playerState = PlayerState.DoubleJump;                  
                break;

            case (int)PlayerState.Avoid:
                playerState = PlayerState.Avoid;
                SwitchCollider(2);

                Displacement(playerRigidbody, transform.rotation, avoidSpeed, playerParameter.avoidParameter.AvoidDistance, Direction_X, 0, Direction_Z);

                break;

            case (int)PlayerState.Falling:             
                if (((playerState & PlayerState.CanFalling) != 0) && canfall)
                {
                    playerState = PlayerState.Falling;
                    playerAnimator.ResetTrigger("Falling");
                    StartLandingCheck();
                   // JumpDoExitGround();
                    SwitchCollider(1);
                 //   Debug.Log("eee");
                }
                break;

            case (int)PlayerState.Attack:
                playerState = PlayerState.Attack;
                StopDetectAnimationStateNotAttack();
                playerRigidbody.velocity = playerRigidbody.velocity*0.1f;
                SwitchMove(0);

                // CanTriggerNextAttack = false;
                break;
            case (int)PlayerState.Dash:
                playerState = PlayerState.Dash;
                //  gravity.StopGroundCheck();
                PlayerShader.enabled = true;

                canfall = false;
                StopCoroutine("LandingCheck");

                break;

        }
    }

    public void EffectPlay(string Id)//------
    {
        ParticlePlay(particleManager.GetParticle(Id));
    }


    public void SwitchMove(int onOff)
    {
        switch (onOff)
        {
            case 0:
                ForceMove = false;
                break;
            case 1:
                ForceMove = true;
                break;
        }
    }

    public void SwitchCollider(int colliderSize)
    {
        switch (colliderSize)
        {
            case 0:
                IdlePhysicsCollider.SetActive(true);
                MediumPhysicsCollider.SetActive(false);
                SmallPhysicsCollider.SetActive(false);
                break;

            case 1:
                IdlePhysicsCollider.SetActive(false);
                MediumPhysicsCollider.SetActive(true);
                SmallPhysicsCollider.SetActive(false);
                break;

            case 2:
                IdlePhysicsCollider.SetActive(false);
                MediumPhysicsCollider.SetActive(false);
                SmallPhysicsCollider.SetActive(true);
                break;
        }

    }

    public void ChangeToIdle(int curState)
    {
        if ((int)playerState == curState)
        {
            if (gravity.groundCheck.IsGround)
            {
                playerAnimator.ResetTrigger("Falling");
                playerAnimator.SetTrigger("Idle");
                playerState = PlayerState.Move;
            }
            else
            {
                playerAnimator.ResetTrigger("Idle");

                playerAnimator.SetTrigger("Falling");

                //Debug.Log("hhh");
            }
        }

    }

    #region 跳躍動畫
    public void AddForce(int JumpState)
    {
        switch (JumpState)
        {
            case (int)PlayerState.Jump:
                Keyframe jumpEndKey = playerParameter.jumpParameter.JumpCurve.keys[playerParameter.jumpParameter.JumpCurve.keys.Length - 1];
                gravity.StopUseGravity();

                StopRigiBodyMoveWithAniamtionCurve_Y();
                RigiBodyMoveWithAniamtionCurve_Y(playerRigidbody,playerParameter.jumpParameter.JumpCurve, 0, jumpEndKey.time, 12, playerParameter.jumpParameter.JumpPerIntervalTime);
                StopCoroutine("StartUseGravity");
                StartCoroutine("StartUseGravity");
                JumpDoExitGround();
                break;

            case (int)PlayerState.DoubleJump:
                Keyframe doubleJumpEndKey = playerParameter.jumpParameter.DoubleJumpCurve.keys[playerParameter.jumpParameter.DoubleJumpCurve.keys.Length - 1];
                StopRigiBodyMoveWithAniamtionCurve_Y();
                gravity.StopUseGravity();
                // playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
                RigiBodyMoveWithAniamtionCurve_Y(playerRigidbody, playerParameter.jumpParameter.DoubleJumpCurve, 0, doubleJumpEndKey.time, 12, playerParameter.jumpParameter.JumpPerIntervalTime);
                StopCoroutine("StartUseGravity");

                StartCoroutine("StartUseGravity");
                
             //   StartLandingCheck();
                
                break;
        }
       
    }

    IEnumerator OpenGroundedCheck()
    {
        yield return new WaitUntil(() => (playerRigidbody.velocity.y < 0));
        gravity.StartGroundCheck();
       
    }

    IEnumerator DoExitGround()
    {
        yield return new WaitUntil(() => !gravity.groundCheck.IsGround);

        gravity.StopGroundCheck();
        StopCoroutine("OpenGroundedCheck");
        StartCoroutine("OpenGroundedCheck");
        StartLandingCheck();
    }

    public void JumpDoExitGround()
    {
        StopCoroutine("DoExitGround");
        StartCoroutine("DoExitGround");
    }

    public void StartLandingCheck()
    {
       
        StopCoroutine("LandingCheck");
        StartCoroutine("LandingCheck");


    }

    IEnumerator StartUseGravity()
    {
        yield return new WaitUntil(() => !RigibodyAnimationCurveIsRunning);
       // Debug.Log(playerState);
        gravity.StartUseGravity();
    }

    
    IEnumerator LandingCheck()
    {
        yield return new WaitUntil(() => gravity.groundCheck.IsGround);
        
        if (animationHash.GetCurrentAnimationState("Idle_Run")) 
        {
            //  Debug.Log("Idle");
            // Debug.Log(playerState);
            playerAnimator.ResetTrigger("DoubleJump");
            ChangePlayerState(1);

            playerAnimator.ResetTrigger("Idle");
            StopRigiBodyMoveWithAniamtionCurve_Y();             
            FallindAniamtion_Horizontal = 0;
            jumpCount = 0;
        }
        else
        {
            
            playerAnimator.SetTrigger("Idle");

            //  Debug.Log(playerState);

            StopCoroutine("LandingCheck");
            StartCoroutine("LandingCheck");
            
        }
    

    }
    #endregion

    #region 攻擊動畫

    public void CanTriggerAttack()
    {
        CanTriggerNextAttack = true;
        isTriggerAttack = false;
        
    }

    public void CantTriggerAttack()
    {
        StopCoroutine("cantTriggerAttack");
        StartCoroutine("cantTriggerAttack");

    }

    IEnumerator cantTriggerAttack()
    {
        yield return new WaitForSeconds(0.2f);
        CanTriggerNextAttack = false;
        ResetCanTriggerNextAttack("NormalAttack");
    }

    public void AttackMoveSwitch()
    {
        if (!isTriggerAttack)
        {
            SwitchMove(1);
        }
    }

    public void ResetCanTriggerNextAttack(string animationTag)
    {
        detectAnimationStateNotAttack = DetectAnimationStateNotAttack(animationTag);
        StopCoroutine(detectAnimationStateNotAttack);

        StartCoroutine(detectAnimationStateNotAttack);

    }

    IEnumerator DetectAnimationStateNotAttack(string animationTag)
    {

        yield return new WaitForSeconds(0.01f);
        if (animationHash.GetCurrentAnimationTag(animationTag))
        {
            

            detectAnimationStateNotAttack = DetectAnimationStateNotAttack(animationTag);

            StartCoroutine(detectAnimationStateNotAttack);

        }
        else
        {
            isTriggerAttack = false;
            CanTriggerNextAttack = true;
            ForceMove = false;
            if (playerState == PlayerState.Attack)
            {
                ChangePlayerState(1);

            }
            playerAnimator.ResetTrigger("NormalAttack");

        }
    }

    public void StopDetectAnimationStateNotAttack()
    {
        if (detectAnimationStateNotAttack != null)
        {
            StopCoroutine(detectAnimationStateNotAttack);


        }
    }

    public void AttackDisplacement(int AttackId)
    {
      //  Debug.Log("Attack hhh");
        if (playerState == PlayerState.Attack) 
        {
            Displacement(playerRigidbody,
               transform.rotation,
               playerParameter.normalAttack[AttackId].MoveSpeed,
               playerParameter.normalAttack[AttackId].MoveDistance,
               playerParameter.normalAttack[AttackId].MoveDirection_X,
               playerParameter.normalAttack[AttackId].MoveDirection_Y,
               playerParameter.normalAttack[AttackId].MoveDirection_Z
               );

        }
        


    }


    #endregion

    #region 衝刺
    public void EndDash()
    {
        PlayerShader.enabled = false;
        gravity.StartUseGravity();
       // gravity.StartGroundCheck();

        canfall = true;


        ChangeToIdle(16);

       
    }

    public void DashMove()
    {
        StopCoroutine("StartUseGravity");

        StopRigiBodyMoveWithAniamtionCurve_Y();
        gravity.StopUseGravity();
        playerRigidbody.velocity = new Vector3(0, 0, 0);
        Displacement(playerRigidbody, transform.rotation, playerParameter.dashParameter.DashSpeed, playerParameter.dashParameter.DashDistance, Direction_X, 0, Direction_Z);


    }

    #endregion

    #endregion


}