using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public string gunName; // 총의 이름
    public float range; // 사정 거리
    public float accuaracy; // 정확도
    public float fireRate; // 연사속도
    public float reloadTime; // 재장전 속도

    public int damage; // 데미지

    public int reloadBulletCount; // 총알 재장선 개수
    public int currentBulletCount; // 현재 탄알집에 남아있는 총알의 개수
    public int maxBulletCount; // 최대 소유 가능 총알 개수
    public int carryBullletCount; // 현재 소유하고 있는 총알 개수

    public string gunBulletName; // 총 탄 이름

    public float retroActionForce; // 반동 세기
    //public float retroActionFineSightForce;

    public ParticleSystem muzzleFlash;

    public string fire_Sound;
    public string reload_Sound;
    public string empty_Sound;

    // Inventory 
    private Inventory theInventory;

    private void Start()
    {
        theInventory = FindObjectOfType<Inventory>();
        CheckAmmoAmount();
    }

    void Update()
    {
        CheckAmmoAmount();
    }

    // 총알 수량 체크
    private void CheckAmmoAmount()
    {
        int ammoAmount = theInventory.FindAmmoAmount(gunBulletName);
        if (ammoAmount < currentBulletCount)
            carryBullletCount = 0;
        else
            carryBullletCount = ammoAmount - currentBulletCount;
    }
}
