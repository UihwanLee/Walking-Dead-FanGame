using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour {

    /*
     * 플레이어: 클레멘타인 : Season4:Final Episode (여)
     * 
     * CharacterController 컴포넌트를 활용한 이동방식을 사용한다.
     *  - (A, D) 키에 따라 이동방향 조정 후 앞으로 이동하는 방식
     * 
     * [이동키 및 애니메이션]
     *  W: 앞으로 가기(Animation: Walking)
     *  A: 왼쪽으로 돌기(Animation: Left Turn)
     *  D: 오른쪽으로 돌기(Animation: Right Turn)
     *  W + Shift or Shift + W: 앞으로 달리기(Animation: Running)
     *  Space: 위로 뛰기(Animation: standing jump)
     *  (W + Shift) + Space: 앞으로 뛰기(Animation: Jumping)
     *  Ctrl: 앉기(Animtion: Sitting Kneel)
     *  
     */

    // 체력
    public int hp = 100;
    
    // 걷는 속도
    public float speed = 4f;

    // 뛰는 속도
    public float runSpeed = 12f;

    // 회전 속도
    public float rotSpeed = 80;

    // 회전값
    public float rot = 0f;

    // 중력 변수
    public float gravitiy = 8;

    // Shot Mode 변수
    public bool isGunMode;
    public bool isShotMode;
    private float shotSpeed = 2f;

    public Vector3 moveDir = Vector3.zero;
    private Vector3 velocity;

    CharacterController controller;

    // 카메라 스크립트
    [SerializeField]
    private GameObject camControllerObj;
    private CameraController camController;

    // 애니메이터
    private Animator anim;

    // 상태 변수
    public bool isTalking;
    public bool isMove;
    public bool val;
    public bool isWalk;

    private UiEventTrigger uiEventMangaer;
    private ZombieSpawManager theZombieSpawnManager;

    // 움직임 체크 변수
    private Vector3 lastPos;

    // 크로스헤어 스크립트
    [SerializeField]
    private GameObject theCrosshair;

    // 인벤토리 창
    [SerializeField] private GameObject theInventoryWindow;

    // 플레이어 장착 아이템
    public GameObject theGun_InField; // 데저트 이글(In Field)
    public GameObject theGun_OutField; // 데저트 이글(Out Field)
    public GameObject theBag; // 가방

    // mainCam
    [SerializeField] private Camera theCam;

    public Vector3 moveDirection;


    // Use this for initialization
    void Start () {
        controller = GetComponent<CharacterController>();
        camController = camControllerObj.GetComponent<CameraController>();
        uiEventMangaer = FindObjectOfType<UiEventTrigger>();
        theZombieSpawnManager = FindObjectOfType<ZombieSpawManager>();
        anim = GetComponent<Animator>();
        isTalking = false;
        isGunMode = false;
        isShotMode = false;
        val = false;
	}

    // 세이브 데이터 로드
    public void LoadToPlayer(int _hp, bool _theGun_InField, bool _theGun_OutField, bool _theBag, bool _isGunMode, bool _isShotMode)
    {
        hp = _hp; // 플레이어 체력
        isGunMode = _isGunMode; // 상태변수 - 건 모드
        isShotMode = _isShotMode; // 상태변수 - 샷 모드

        theGun_InField.SetActive(_theGun_InField); // 데저트 이글(In Field)
        theGun_OutField.SetActive(_theGun_OutField); // 데저트 이글(Out Field)
        theBag.SetActive(_theBag); // 가방

        if (isGunMode && !isShotMode)
        {
            ChangeGunMode();
            uiEventMangaer.setHUD();
        }
        if (isShotMode)
        {
            ChangeShotMode();
            uiEventMangaer.setHUD();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!theInventoryWindow.activeSelf)
        {
            this.GetComponent<Animator>().enabled = true;
            if (!(isTalking == true || val == true))
            {
                if (isShotMode) Move_ShotMode();
                else
                {
                    if (isGunMode) Move_GunMode();
                    else Move();
                }
                MoveCheck();
            }
        }
        else this.GetComponent<Animator>().enabled = false;
        CheckHP();
        CheckGunUI();
        CheckZombieBGM();
    }

    public void Move()
    {
        if (controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.W) && anim.GetBool("isSit") == false)
            {
                if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.W))
                {
                    anim.SetBool("isRunning", true);
                    moveDir = new Vector3(0, 0, 1);
                    moveDir *= runSpeed;
                    moveDir = transform.TransformDirection(moveDir);
                }
                else
                {
                    anim.SetBool("isWalking", true);
                    anim.SetBool("isRunning", false);
                    moveDir = new Vector3(0, 0, 1);
                    moveDir *= speed;
                    moveDir = transform.TransformDirection(moveDir);
                }
            }
            else if (Input.GetKey(KeyCode.S) && anim.GetBool("isSit") == false)
            {
                if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.S))
                {
                    anim.SetBool("isRunning", true);
                    moveDir = new Vector3(0, 0, -1);
                    moveDir *= runSpeed;
                    moveDir = transform.TransformDirection(moveDir);
                }
                else
                {
                    anim.SetBool("isWalking", true);
                    anim.SetBool("isRunning", false);
                    moveDir = new Vector3(0, 0, -1);
                    moveDir *= speed;
                    moveDir = transform.TransformDirection(moveDir);
                }
            }
            else
            {
                anim.SetBool("isWalking", false);
                moveDir = new Vector3(0, 0, 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                anim.SetBool("isLeftTurn", true);
            }
            if (!Input.GetKey(KeyCode.A))
            {
                anim.SetBool("isLeftTurn", false);
            }
            if (Input.GetKey(KeyCode.D))
            {
                anim.SetBool("isRightTurn", true);
            }
            if (!Input.GetKey(KeyCode.D))
            {
                anim.SetBool("isRightTurn", false);
            }
            // jump
            if (Input.GetKey(KeyCode.Space))
            {
                anim.SetBool("isJumping", true);
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                anim.SetBool("isJumping", false);
            }
            // sit down
            if (Input.GetKey(KeyCode.LeftControl))
            {
                anim.SetBool("isSit", true);
            }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                anim.SetBool("isSit", false);
            }
            if (Input.GetKeyUp(KeyCode.G))
            {
                isGunMode = !isGunMode;
            }

            if (isGunMode)
            {
                // 총알 UI 켜기
                //uiEventMangaer.setHUD();
                // 마우스 왼쪽 버튼 누르면 ShotMode 변경
                if (Input.GetMouseButtonDown(1))
                {
                    // 카메라 고정
                    anim.SetLayerWeight(anim.GetLayerIndex("GunMode"), 1f);
                    camController.setPos();
                    uiEventMangaer.setShot();
                    isShotMode = true;
                }
            }
        };
        rot += Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, rot, 0);
        moveDir.y -= gravitiy * Time.deltaTime;
        controller.Move(moveDir * Time.deltaTime);
    }

    private void Move_GunMode()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ChangeShotMode();
        }

        if (controller.isGrounded)
        {
            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W))
            {
                anim.SetBool("isWalking", true);
            }
            else
            {
                anim.SetBool("isWalking", false);
            }

            Vector3 forward = theCam.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;
            Vector3 right = new Vector3(forward.z, 0, -forward.x);
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            moveDirection = (h * right + v * forward);

            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            {
                getRotation(theCam.transform.TransformDirection(new Vector3(1f, 0f, 1f)));
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            {
                getRotation(theCam.transform.TransformDirection(new Vector3(-1f, 0f, 1f)));
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                getRotation(theCam.transform.TransformDirection(new Vector3(1f, 0f, -1f)));
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            {
                getRotation(theCam.transform.TransformDirection(new Vector3(-1f, 0f, -1f)));
            }
            else if (Input.GetKey(KeyCode.W))
            {
                getRotation(theCam.transform.TransformDirection(Vector3.forward));
            }
            else if (Input.GetKey(KeyCode.S))
            {
                getRotation(theCam.transform.TransformDirection(-Vector3.forward));
            }
            else if (Input.GetKey(KeyCode.D))
            {
                getRotation(theCam.transform.TransformDirection(Vector3.right));
            }
            else if (Input.GetKey(KeyCode.A))
            {
                getRotation(theCam.transform.TransformDirection(Vector3.left));
            }

            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && ((Input.GetKey(KeyCode.A) 
                || (Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.W)))))
            {
                anim.SetBool("isRunning", true);
                moveDirection *= runSpeed;
            }
            else
            {
                anim.SetBool("isRunning", false);
                moveDirection *= speed;
            }
        }

        moveDirection.y -= gravitiy * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    // 플레이어 회전 이동 업데이트
    private void getRotation(Vector3 playerRot)
    {
        Vector3 relativePos = playerRot;
        relativePos.y = 0.0f;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        float turnSpeed = 3f;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
    }

    private void Move_ShotMode()
    {
        anim.SetBool("isShotMode", true);

        if (Input.GetMouseButtonDown(1) && isGunMode)
        {
            ChangeGunMode();
        }


        if (controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))
            {
                anim.SetBool("isWalking", true);
            }
            else
            {
                anim.SetBool("isWalking", false);
            }
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * shotSpeed * Time.deltaTime);
        velocity.y -= gravitiy * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void ChangeGunMode()
    {
        // 카메라 해제
        anim.SetLayerWeight(anim.GetLayerIndex("GunMode"), 0f);
        uiEventMangaer.retShot();
        camController.resPos();
        anim.SetBool("isShotMode", false);
        isShotMode = false;
    }

    private void ChangeShotMode()
    {
        // 카메라 고정
        anim.SetLayerWeight(anim.GetLayerIndex("GunMode"), 1f);
        camController.setPos();
        uiEventMangaer.setShot();
        isShotMode = true;
    }

    public bool getShotMode()
    {
        return isShotMode;
    }

    public void changeShotMode()
    {
        isShotMode = true;
    }

    public void changeNormalMode()
    {
        isShotMode = false;
    }

    private void MoveCheck()
    {
        if (Vector3.Distance(lastPos, transform.position) >= 0.01f)
            isWalk = true;
        else
            isWalk = false;

        if(theCrosshair.activeSelf)
        {
            Crosshair crosshair = theCrosshair.GetComponent<Crosshair>();
            crosshair.WalkingAinmation(isWalk);
        }
        lastPos = transform.position;
    }

    public void Damage(int damage)
    {
        hp -= damage;
    }

    public void Health(int health)
    {
        uiEventMangaer.canvasHealthFade(0.5f);
        hp += health;
    }

    private void CheckHP()
    {
        if (hp > 100)
            hp = 100;
        else if (hp >= 90)
            uiEventMangaer.canvasDamageDelete();
        else if (hp >= 70)
        {
            uiEventMangaer.canvasDamageFade(0.31f);
        }
        else if (hp >= 40)
        {
            uiEventMangaer.canvasDamageFade(0.58f);
        }
        else if (hp > 0)
            uiEventMangaer.canvasDamageFade(0.7f);
        else
            hp = 0;
    }

    // Get 함수
    public int getHP() { return hp; }

    // Gun UI 체크
    public void CheckGunUI()
    {
        if (theGun_OutField.activeSelf)
        {
            if (!theInventoryWindow.activeSelf)
                uiEventMangaer.setHUD();
        }
        else
            uiEventMangaer.retHUD();
    }

    private void CheckZombieBGM()
    {
        if(theZombieSpawnManager.CheckFollowing() && AudioManager.instance.currentBGM != "Background_Zombie")
        {
            AudioManager.instance.currentBGM = "Background_Zombie";
            AudioManager.instance.PlayBGM("Background_Zombie");
            StartCoroutine(PlayNightBGM());
        }
    }

    IEnumerator PlayNightBGM()
    {
        Debug.Log("Start");
        yield return new WaitForSeconds(76f);
        Debug.Log("Change");
        AudioManager.instance.currentBGM = "Background_Night";
        AudioManager.instance.PlayBGM("Background_Night");
    }
}
