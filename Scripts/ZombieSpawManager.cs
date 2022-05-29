using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawManager : MonoBehaviour {

    // 플레이어
    public GameObject player;

    // 좀비 스폰
    public GameObject[] zombieAI;
    public GameObject[] zombieBots;
    public float xStartPos;
    public float xEndPos;
    public float zStartPos;
    public float zEndPos;

    // 로딩
    public void LoadToAllZombieState(int index,bool isActive,bool isDead,Vector3 pos, Vector3 rot)
    {
        zombieBots[index].transform.position = pos;
        zombieBots[index].transform.eulerAngles = rot;
        if (isActive)
        {
            zombieBots[index].SetActive(isActive);
            if (isDead)
            {
                zombieBots[index].GetComponent<Zombie_AI>().isDead = true;
                zombieBots[index].GetComponent<Zombie_AI>().Dead();
                zombieBots[index].GetComponent<Zombie_AI>().retFollowing();
            }
        }
        else
        {
            zombieBots[index].SetActive(isActive);
            if (isDead)
            {
                zombieBots[index].GetComponent<Zombie_AI>().isDead = true;
                zombieBots[index].GetComponent<Zombie_AI>().retFollowing();
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        zombieBots = new GameObject[100];
        Spawn();
    }

    void Update()
    {
        Show();
        CheckPos();
    }

    private void Spawn()
    {
        for (int i = 0; i < zombieBots.Length; i++)
        {
            zombieBots[i] = Instantiate(RandomZombie());
            // x: 70/-70 ~ 330/-330 z: 60/-60 ~ 300/-300
            zombieBots[i].transform.position = new Vector3(RandomXPos(), 6f, RandomZPos());
            zombieBots[i].SetActive(false);
        }
    }

    private GameObject RandomZombie()
    {
        int _random = Random.Range(0, 6); // 좀비 변수
        return zombieAI[_random];
    }

    private float RandomXPos()
    {
        int _sign = Random.Range(0, 2); // +, - 변수 설정
        if (_sign == 0) return Random.Range(xStartPos, xEndPos); 
        else return Random.Range(-xStartPos, -xEndPos); 
    }

    private float RandomZPos()
    {
        int _sign = Random.Range(0, 2); // +, - 변수 설정
        if (_sign == 0) return Random.Range(zStartPos, zEndPos);
        else return Random.Range(-zStartPos, -zEndPos);
    }

    private void Show()
    {
        for (int i = 0; i < zombieBots.Length; i++)
        {
            if (Vector3.Distance(player.transform.position, zombieBots[i].transform.position) < 40 && !zombieBots[i].GetComponent<Zombie_AI>().isDead)
                zombieBots[i].SetActive(true);
        }
    }

    // 좀비가 땅에 떨어졌을 시 재생성
    private void CheckPos()
    {
        for (int i = 0; i < zombieBots.Length; i++)
        {
            if (zombieBots[i].transform.position.y <= -10)
            {
                zombieBots[i].SetActive(false);
                zombieBots[i] = Instantiate(RandomZombie());
                // x: 70/-70 ~ 330/-330 z: 60/-60 ~ 300/-300
                zombieBots[i].transform.position = new Vector3(RandomXPos(), 6f, RandomZPos());
                zombieBots[i].SetActive(false);
            }
        }
    }

    public GameObject[] getZombieBots() { return zombieBots; }

    public void SetAudioVolume(float vol)
    {
        for (int i = 0; i < zombieBots.Length; i++)
        {
            zombieBots[i].GetComponent<AudioSource>().volume = vol;
        }
    }

    public void SetAudioMute()
    {
        for (int i = 0; i < zombieBots.Length; i++)
        {
            zombieBots[i].GetComponent<AudioSource>().mute = true;
        }
    }

    public void SetAudioPlay()
    {
        for (int i = 0; i < zombieBots.Length; i++)
        {
            zombieBots[i].GetComponent<AudioSource>().mute = false;
        }
    }

    public bool CheckFollowing()
    {
        for (int i = 0; i < zombieBots.Length; i++)
        {
            if (zombieBots[i].GetComponent<Zombie_AI>().getFollowing()) return true;
        }
        return false;
    }
}
