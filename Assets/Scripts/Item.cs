using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    // 아이템 데이터
    public ItemData data;
    // 아이템 레벨
    public int level;
    // 무기 데이터
    public Weapon weapon;
    // 기어 데이터
    public Gear gear;

    // 아이콘, 텍스트
    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    private void Awake()
    {
        // 아이콘 정보 초기화
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        // 텍스트 정보 초기화
        Text[] texts = GetComponentsInChildren<Text>();
        // 히에라키에 순서대로
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;
    }

    private void OnEnable()
    {
        // 레벨이 오르면 텍스트 변경, 만렙시 빨간 Max
        if (level == data.damages.Length)
        {
            textLevel.text = "Lv.MAX";
            textLevel.color = Color.red;
        }
        else
            textLevel.text = "Lv." + (level + 1);

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = 
                    string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                break;
         
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = 
                    string.Format(data.itemDesc, data.damages[level] * 100);
                break;
                
            case ItemData.ItemType.Heal:
                textDesc.text = 
                    string.Format(data.itemDesc);
                break;
        }
    }

    // 버튼 클릭 시 이벤트
    public void OnClick()
    {
        // 아이템 타입에 따라 변경
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];

                    weapon.LevelUp(nextDamage, nextCount);
                }


                level++;
                break;

            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }

                level++;
                break;

            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }

        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
