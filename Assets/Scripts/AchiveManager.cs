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
        // �ر��� ���� ��������
        achives = (Achive[])Enum.GetValues(typeof(Achive));
        wait = new WaitForSecondsRealtime(5);
        // ó�� �����̶�� ���� �ʱ�ȭ
        if (!PlayerPrefs.HasKey("MyData"))
            Init();
    }

    void Init()
    {
        // ��ȸ ���� ����
        PlayerPrefs.SetInt("MyData", 1);

        // ���� �ʱ�ȭ
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
            // ���� �̸� ��������
            string achiveName = achives[i].ToString();
            // ��� ���� Ȯ���ϱ�
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1;
            lockCharacter[i].SetActive(!isUnlock);
            unlockCharacter[i].SetActive(isUnlock);
        }
    }

    private void LateUpdate()
    {
        // ����Ŭ ��ȸ�ϸ鼭 ���� �޼� �˻�
        foreach (Achive achive in achives)
        {
            CheckAchive(achive);
        }
    }

    void CheckAchive(Achive achive)
    {
        bool isAchive = false;

        // ���� ���ǿ� ���� ���
        switch (achive)
        {
            case Achive.unlockPotato:
                isAchive = GameManager.instance.kill >= 10;
                break;
            case Achive.unlockBean:
                isAchive = GameManager.instance.gameTime == GameManager.instance.maxGameTime;
                break;
         }

        // ���رݻ����̰� �޼��ߴٸ� �ر�
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
