using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject uiNotice;

    enum Achive { unlockPotato, unlockBean  }
    Achive[] achives;
    WaitForSecondsRealtime wait;

    private void Awake()
    {
        // 해금할 업적 가져오기
        achives = (Achive[])Enum.GetValues(typeof(Achive));
        wait = new WaitForSecondsRealtime(5);
        // 처음 접속이라면 전부 초기화
        if (!PlayerPrefs.HasKey("MyData"))
            Init();
    }

    void Init()
    {
        // 초회 접속 저장
        PlayerPrefs.SetInt("MyData", 1);

        // 업적 초기화
        foreach (Achive achive in achives) 
            PlayerPrefs.SetInt(achive.ToString(), 0);
    }

    private void Start()
    {
        UnlockCharacter();
    }

    void UnlockCharacter()
    {
        for (int i = 0; i < lockCharacter.Length; i++)
        {
            // 업정 이름 가져오기
            string achiveName = achives[i].ToString();
            // 언락 상태 확인하기
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1;
            lockCharacter[i].SetActive(!isUnlock);
            unlockCharacter[i].SetActive(isUnlock);
        }
    }

    private void LateUpdate()
    {
        // 사이클 순회하면서 업적 달성 검사
        foreach (Achive achive in achives)
        {
            CheckAchive(achive);
        }
    }

    void CheckAchive(Achive achive)
    {
        bool isAchive = false;

        // 업적 조건에 따라 등록
        switch (achive)
        {
            case Achive.unlockPotato:
                isAchive = GameManager.instance.kill >= 10;
                break;
            case Achive.unlockBean:
                isAchive = GameManager.instance.gameTime == GameManager.instance.maxGameTime;
                break;
         }

        // 미해금상태이고 달성했다면 해금
        if (isAchive && PlayerPrefs.GetInt(achive.ToString()) == 0) 
        {
            PlayerPrefs.SetInt(achive.ToString(), 1);

            for (int i = 0; i < uiNotice.transform.childCount; i++)
            {
                bool isActive = i == (int)achive;
                uiNotice.transform.GetChild(i).gameObject.SetActive(isActive);
            }

            StartCoroutine(NoticeRoutine());
        }
    }

    IEnumerator NoticeRoutine()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);

        uiNotice.SetActive(true);
        yield return wait;
        uiNotice.SetActive(false);
    }
}
