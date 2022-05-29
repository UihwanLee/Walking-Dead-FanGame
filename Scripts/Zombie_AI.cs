using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Zombie_AI : MonoBehaviour {

    /*
     * 좀비 AI 스크립트
     * 
     * 애니메이션 및 플레이어 상호작용 스크립트
     * 
     * 플레이어와 가까울 시 다가가기 시전
     * 
    */

    [SerializeField] private int hp; // 체력.
    [SerializeField] private int damage; // 공격력

    [SerializeField] private float distance; // player와 접촉 거리
    [SerializeField] private float walkSpeed; // 걷기 스피드.

    private Vector3 direction; // 방향

    private GameObject player;

    private Animator anim;
    private AudioSource theAudio;

    // 시스템 스크립트
    private PlayerController playercontroller;
    private TalkConditionTrigger talkConditionTrigger;
    private UiEventTrigger uiEventTrigger;

    [SerializeField] private AudioClip idleSound;

    // 상태변수
    private bool isWalking; // 걷는지 안 걷는지 판별.
    private bool isFollowing; // 플레이어를 발견하고 따라가는 중인지 체크.
    private bool isAction; // 액션 중인지 체크.
    private bool isDamage; // 피격 받았는지 판별.
    public bool isDead; // 죽었는지 판별.

    [SerializeField] private float walkTime; // 걷기 시간
    [SerializeField] private float waitTime; // 대기 시간.
    private float currentTime;

    // Stall Anim 오브젝트
    private GameObject stallObj;

    // 필요 애니메이션 
    [SerializeField] private GameObject[] anims;
    private int selectAnim = -1;

    // 필요 캠 오브젝트
    private GameObject mainCam;
    private int count = 0;

    // 리지드 바디
    private Rigidbody rigid;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        //audio = GetComponent<AudioSource>();
        playercontroller = FindObjectOfType<PlayerController>();
        talkConditionTrigger = FindObjectOfType<TalkConditionTrigger>();
        uiEventTrigger = FindObjectOfType<UiEventTrigger>();

        // 리지드 바디
        rigid = this.GetComponent<Rigidbody>();

        // 오브젝트 찾기
        mainCam = GameObject.Find("Main Camera");
        stallObj = GameObject.Find("Talk_Event_StallEvent");
        player = GameObject.Find("Clementine");

        // 상태 변수 초기화
        isDamage = false;
        isFollowing = false;
        isAction = true;
        isWalking = false;

        currentTime = waitTime;

        // 애니메이션 비활성
        for (int i = 0; i < anims.Length; i++)
        {
            // 타임라인 Binding 동적 할당
            TimelineAsset ta = anims[i].GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
            IEnumerable<TrackAsset> temp = ta.GetOutputTracks();

            foreach (var kvp in temp)
            {
                if (kvp is CinemachineTrack)
                {
                    anims[i].GetComponent<PlayableDirector>().SetGenericBinding(kvp, mainCam);
                }

                if (kvp is AnimationTrack)
                {
                    count++;
                    if (count % 2 == 0)
                    {
                        anims[i].GetComponent<PlayableDirector>().SetGenericBinding(kvp, player);
                    }
                }
            }
            anims[i].SetActive(false);
        }

        // 오디오
        theAudio = GetComponent<AudioSource>();

        PlaySE(idleSound);
    }
	
	// Update is called once per frame
	void Update () {
        if (!Inventory.inventoryActivated || !PauseMenu.pauseMenuActivated)
        {
            this.GetComponent<Animator>().enabled = true;
            if (!(playercontroller.val || playercontroller.isTalking))
            {
                Playing_AI();
            }
        }
        else
        {
            this.GetComponent<Animator>().enabled = false;
        }
	}

    private void Playing_AI()
    {
        if (!isDead)
        {
            //PlaySE(idleSound);

            if (isFollowing)
            {
                FollowingPlayer();
            }
            else
            {
                // 랜덤 액션
                if(talkConditionTrigger.getKillZombie())
                {

                }
                Move();
                Rotation();
                ElapseTime();
                //anim.SetBool("isWalking", false);
            }
        }
    }

    public void FollowPlayer() { if(!isDead) isFollowing = true; }
    public void StopFollowPlayer() { isFollowing = false; anim.SetBool("isWalking", false); }

    private void checkDistance()
    {
        if (Vector3.Distance(player.transform.position, this.transform.position) > distance)
        {
            StopFollowPlayer();
        }
    }

    // 좀비가 플레이어의 일정사거리에 있을 때 행동
    private void FollowingPlayer()
    {
        currentTime = 1f;
        checkDistance();

        Vector3 dir = player.transform.position - this.transform.position;
        dir.y = 0f;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(dir), 0.1f);

        if (dir.magnitude > 1.5f && !isDamage)
        {
            isWalking = true;
            anim.SetBool("isWalking", isWalking);
            rigid.MovePosition(transform.position + (transform.forward * walkSpeed * Time.deltaTime));
            //this.transform.Translate(0f, 0f, 0.01f * walkSpeed);
        }
        else
        {
            // 좀비가 플레이어를 공격한다.
            TryAttack();
            isWalking = false;
            anim.SetBool("isWalking", isWalking);
        }
    }

    // 공격 애니메이션 판별 함수
    private void TryAttack()
    {
        playercontroller.val = true;

        // 플레이어 체력이 남아있을시 공격
        if(playercontroller.getHP() - damage > 0)
        {
            StartCoroutine(Attack1());
        }
        else
        {
            StartCoroutine(Attack2());
        }
    }

    IEnumerator Attack1()
    {
        Vector3 dir = this.transform.position - player.transform.position;
        dir.y = 0f;
        player.transform.rotation = Quaternion.LookRotation(dir);

        if(playercontroller.getShotMode())
        {
            uiEventTrigger.retShot();
            selectAnim = 0;
        }
        else
        {
            selectAnim = 1;
        }

        anims[selectAnim].SetActive(true);
        PlayableDirector scene1 = anims[selectAnim].GetComponent<PlayableDirector>();
        scene1.Play();

        yield return new WaitForSeconds(1.2f);
        // 플레이어 체력 깍기
        // 캔버스 피해 효과
        playercontroller.Damage(damage);
        yield return new WaitForSeconds(2.05f);
        // 좀비 물러나게 하기
        this.transform.Translate(0f, 0f, -1f * walkSpeed);
        yield return new WaitForSeconds(1.75f);

        anims[selectAnim].SetActive(false);
        playercontroller.val = false;

        if (selectAnim == 0) uiEventTrigger.setShot();
    }

    IEnumerator Attack2()
    {
        Vector3 dir = this.transform.position - player.transform.position;
        dir.y = 0f;
        player.transform.rotation = Quaternion.LookRotation(dir);

        if (playercontroller.getShotMode())
        {
            uiEventTrigger.retShot();
            selectAnim = 2;
        }
        else
        {
            selectAnim = 3;
        }

        anims[selectAnim].SetActive(true);
        PlayableDirector scene1 = anims[selectAnim].GetComponent<PlayableDirector>();
        scene1.Play();

        yield return new WaitForSeconds(1.5f);
        // 좀비 포지션 이동
        //this.transform.Translate(0f, 0f, 1.3f * walkSpeed);
        rigid.MovePosition(transform.position + (transform.forward * 0.5f * 3f));
        yield return new WaitForSeconds(0.5f);
        uiEventTrigger.setDeadUI();
        yield return new WaitForSeconds(3f);

        // canvasDead UI 생성
        //anims[selectAnim].SetActive(false);
    }

    public void Damage(int _dmg)
    {
        // 플레이어 위치 확인
        Vector3 dir = player.transform.position - this.transform.position;
        dir.y = 0f;
        this.transform.rotation = Quaternion.LookRotation(dir);

        isDamage = true;

        // 좀비가 한발자국 물러난다.
        rigid.MovePosition(transform.position + (transform.forward * (-1) * walkSpeed * 2f));
        //this.transform.Translate(0f, 0f, -0.3f * walkSpeed);

        if (!isDead)
        {
            hp -= _dmg;

            if (hp <= 0)
            {
                Dead();
                return;
            }

            //PlaySE(sound_pig_Hurt);
            anim.SetTrigger("isDamage");
        }

        isDamage = false;
    }

    public void Dead()
    {
        Animator anim = GetComponent<Animator>();
        TalkConditionTrigger talkConditionTrigger = FindObjectOfType<TalkConditionTrigger>();

        //PlaySE(sound_pig_Dead);
        isFollowing = false;
        isWalking = false;
        isDead = true;
        anim.SetBool("isDead", true);

        // Stall 애니메이션 체크
        if(!talkConditionTrigger.getKillZombie())
        {
            talkConditionTrigger.MissionComplete(4);
            stallObj = GameObject.Find("Talk_Event_StallEvent");
            Talk_Event_Stall stallAnim = stallObj.GetComponent<Talk_Event_Stall>();
            stallAnim.Talk_Stall();
        }

        StartCoroutine(StarDead());
    }

    IEnumerator StarDead()
    {
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<CapsuleCollider>().enabled = false;

        theAudio.Stop();

        // 1분 후 삭제
        yield return new WaitForSeconds(60f);
        this.gameObject.SetActive(false);
    }

    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    // 랜덤 액션
    private void Move()
    {
        if (isWalking)
            rigid.MovePosition(transform.position + (transform.forward * walkSpeed * Time.deltaTime));
    }

    private void Rotation()
    {
        if (isWalking)
        {
            Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, direction.y, 0f), 0.01f);
            rigid.MoveRotation(Quaternion.Euler(_rotation));
        }
    }

    private void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
                ReSet();
        }
    }

    private void ReSet()
    {
        isWalking = false; isAction = true;
        anim.SetBool("isWalking", isWalking); 
        direction.Set(0f, Random.Range(0f, 360f), 0f);
        RandomAction();
    }

    private void RandomAction()
    {
        int _random = Random.Range(0, 2); // 대기, 걷기.

        if (_random == 0)
            Wait();
        else if (_random == 1)
            TryWalk();
    }

    // 대기
    private void Wait()
    {
        currentTime = waitTime;
    }

    // 걷기
    private void TryWalk()
    {
        isWalking = true;
        anim.SetBool("isWalking", isWalking);
        currentTime = walkTime;
    }

    public bool getFollowing() { return isFollowing; }
    public void retFollowing() { isFollowing = false; }
}
