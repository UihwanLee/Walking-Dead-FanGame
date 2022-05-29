using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name; // 곡의 이름.
    public AudioClip clip; // 곡.
}

public class AudioManager : MonoBehaviour {

    static public AudioManager instance;
    #region singleton
    void Awake() // 객체 생성시 최초 실행.
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
            Destroy(gameObject);
    }
    #endregion singleton

    public AudioSource[] audioSourceEffects;
    public AudioSource audioSourceBgm;

    public string[] playSoundName;

    public Sound[] effectSounds;
    public Sound[] bgmSounds;

    // 오디오 저장
    public string currentBGM;

    public void LoadToAudio(bool isNight)
    {
        if (!isNight) currentBGM = "Background_Normal";
        else currentBGM = "Background_Night";
        PlayBGM(currentBGM);
    }

    void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];

        // 초반 BGM 틀기
        currentBGM = "Background_Normal";
        PlayBGM(currentBGM);
    }

    // PauseMenu: Setting에서 오디오 정보 가져오기
    public void SetMusicVolume(float vol)
    {
        audioSourceBgm.volume = vol;
    }
    public void SetMusicMute()
    {
        audioSourceBgm.mute = true;
    }
    public void SetMusicPlay()
    {
        audioSourceBgm.mute = false;
    }
    public void SetSFXVolume(float vol)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].volume = vol;
        }
    }
    public void SetSFXMute()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].mute = true;
        }
    }
    public void SetSFXPlay()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].mute = false;
        }
    }

    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if (_name == effectSounds[i].name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용중입니다.");
                return;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager에 등록되지 않았습니다.");
    }

    public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (playSoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                break;
            }
        }
        Debug.Log("재생 중인" + _name + "사운드가 없습니다.");
    }

    public void PlayBGM(string _name)
    {
        for (int i = 0; i < bgmSounds.Length; i++)
        {
            if (_name == bgmSounds[i].name)
            {
                audioSourceBgm.clip = bgmSounds[i].clip;
                audioSourceBgm.Play();
            }
        }
    }

    public void StopBGM()
    {
        audioSourceBgm.Stop();
    }

    public void ContinueBGM()
    {
        audioSourceBgm.Play();
    }
}
