using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class HUD : MonoBehaviour
{
    // 인터페이스 종류
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    Text myText;
    Slider mySlider;

    private void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        var gm = GameManager.instance;

        // 인터페이스 갱신
        switch (type)
        {
            case InfoType.Exp:
                float curExp = gm.exp;
                float maxExp = gm.nextExp[Mathf.Min(gm.level, gm.nextExp.Length - 1)];
                mySlider.value = curExp / maxExp;
                break;
            
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", gm.level);
                break;
            
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", gm.kill);
                break;
            
            case InfoType.Time:
                float remainTime = gm.maxGameTime - gm.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            
            case InfoType.Health:
                float curHealth = gm.health;
                float maxHealth = gm.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }
    }
}
