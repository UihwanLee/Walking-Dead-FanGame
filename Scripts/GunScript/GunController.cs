using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour {

    // 플레이어
    [SerializeField]
    private GameObject player;

    // 현재 장착된 총
    [SerializeField]
    private Gun currentGun;

    // 연사 속도 계산
    private float currentFireRate;

    // 상태 변수
    private bool isReload = false;

    // 플레이어 originPos
    private Vector3 originPos;

    // 효과음
    private AudioSource audioSource;

    // 플레이어 스크립트
    private PlayerController playerController;

    // 충돌 정보 변수
    private RaycastHit hitInfo;

    // 카메라 변수
    [SerializeField]
    private Camera theCam;

    // 피격 이펙트
    [SerializeField]
    private GameObject hit_effect_prefab;
    [SerializeField]
    private GameObject blood_effect_prefab;

    // 총알 개수 텍스트에 반영
    [SerializeField]
    private Text[] text_Bullet;

    // 크로스헤어 스크립트
    [SerializeField] private Crosshair theCrosshair;

    // 애니메이터
    private Animator anim;

    // 시스템 스크립트
    private TalkConditionTrigger talkCondionTrigger;
    private UiEventTrigger uiEventManager;

    // 인벤토리 창
    private Inventory theInventory;
    [SerializeField] private GameObject theInventoryWindow; 

    // Use this for initialization
    void Start () {
        anim = player.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerController = FindObjectOfType<PlayerController>();
        talkCondionTrigger = FindObjectOfType<TalkConditionTrigger>();
        uiEventManager = FindObjectOfType<UiEventTrigger>();

        // 인벤토리
        theInventory = FindObjectOfType<Inventory>();

    }
	
	// Update is called once per frame
	void Update () {
        if (!theInventoryWindow.activeSelf || !uiEventManager.isHelpActivated || !PauseMenu.pauseMenuActivated)
        {
            if (playerController.getShotMode())
            {
                GunFireRateCalc();
                TryFire();
            }
            TryReload();
            CheckBullet();
        }
    }

    // 연사 속도 계산
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime;
    }

    // 발사 시도
    private void TryFire()
    {
        if(playerController.getShotMode())
        {
            if (Input.GetMouseButtonDown(0) && currentFireRate <= 0 && !isReload)
            {
                Fire();
            }
        }
    }

    // 발사 전 계산
    private void Fire()
    {
        if (!isReload)
        {
            if (currentGun.currentBulletCount > 0)
                Shoot();
            else
            {
                AudioManager.instance.PlaySE(currentGun.empty_Sound);

                // 좀비 사냥 튜토리얼 안내문
                if (!(talkCondionTrigger.getKillZombie()))
                {
                    uiEventManager.canvasExplainFade(3f, "R키를 눌러 재장전 할 수 있습니다.");
                }
            }
        }
    }

    // 발사 후 계산
    private void Shoot()
    {
        theCrosshair.FireAnimation();
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; // 연사 속도 재계산.
        AudioManager.instance.PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();
        Hit();
        //StopAllCoroutines();
        //StartCoroutine(RetroActionCoroutine());

        // 인벤토리 탄약 제거
        CutInventoryAmmo();
    }

    // 인벤토리 탄약 제거
    private void CutInventoryAmmo()
    {
        theInventory.CutItem(currentGun.gunBulletName, 1);
    }

    private void Hit()
    {
        if(Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hitInfo, currentGun.range))
        {
            if(hitInfo.transform.tag == "Zombie" || hitInfo.transform.tag == "NPC")
            {
                if (hitInfo.transform.tag == "Zombie")
                {
                    Debug.Log("Zombie Hit");
                    theCrosshair.HitEnemy();
                    hitInfo.transform.GetComponent<Zombie_AI>().Damage(currentGun.damage);
                }
                GameObject clone = Instantiate(blood_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(clone, 2f);
            }
            else
            {
                GameObject clone = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(clone, 2f);
            }
        }
    }

    // 재장전 시도
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    // 재장전
    IEnumerator ReloadCoroutine()
    {

        if (currentGun.carryBullletCount > 0)
        {
            isReload = true;

            if (playerController.isWalk)
            {
                anim.SetTrigger("Reload_Walk");
            }
            else
                anim.SetTrigger("Reload");

            AudioManager.instance.PlaySE(currentGun.reload_Sound);

            currentGun.carryBullletCount += currentGun.currentBulletCount;
            //currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if(currentGun.carryBullletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBullletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBullletCount;
                currentGun.carryBullletCount = 0;
            }

            isReload = false;
        }
        else
        {
            Debug.Log("소유한 총알이 없습니다.");
        }
    }

    // 반동 코루틴
    IEnumerator RetroActionCoroutine()
    {
        originPos = player.transform.position;
        Vector3 recoilBack = new Vector3(originPos.z, originPos.y, currentGun.retroActionForce);

        currentGun.transform.localPosition = originPos;

        // 반동 시작
        while (player.transform.localPosition.z <= currentGun.retroActionForce - 0.02f)
        {
            player.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
            yield return null;
        }

        // 원위치
        while (player.transform.localPosition != originPos)
        {
            player.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
            yield return null;
        }
    }

    // 사운드 재생
    private void  PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    private void CheckBullet()
    {
        text_Bullet[0].text = currentGun.currentBulletCount.ToString();
        text_Bullet[1].text = currentGun.carryBullletCount.ToString();
    }
}
