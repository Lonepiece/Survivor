using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // 무기 종류
    public int id;
    // 프리펩 순서
    public int prefabId;
    // 데미지
    public float damage;
    // 갯수
    public int count;
    // 공격속도
    public float speed;

    // 연속 발사용
    float timer;
    // 플레이어 가져오기
    Player player;

    private void Awake()
    {
        player = GameManager.instance.player;
    }

    private void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // 유저 주변을 떠돌아다니게 
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.forward * speed * Time.deltaTime);
                break;

            case 2:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;

            default:
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    timer = 0;
                    Fire();
                }
                break;
        }
    }

    // 렙업시 데미지 및 개수 증가
    public void LevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        if (id == 0 || id == 2)
            Batch(id);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    // 무기 초기 설정
    public void Init(ItemData data)
    {
        // 이름 변경
        name = "Weapon " + data.itemId;
        // 부모객체를 유저로
        transform.parent = player.transform;
        // 초기설정 0.0.0
        transform.localPosition = Vector3.zero;

        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;

        for (int i = 0; i < GameManager.instance.pool.prefabs.Length; i++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }

        // 무기 발사 속도
        switch (id)
        {
            case 0:
            case 2:
                speed = 150 * Character.WeaponSpeed;
                Batch(id);
                break;

            default:
                speed = 0.3f * Character.WeaponRate;
                break;
        }

        // 핸드 0 = 근접 1 = 원거리 
        Hand hand = player.hands[(int)data.itemType];
        hand.sprite.sprite = data.hand;
        hand.gameObject.SetActive(true);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    // 무기 오브젝트 배치하기
    void Batch(int id)
    {
        for (int i = 0; i < count; i++)
        {
            Transform bullet;

            // 이미 있다면 여기에 추가
            if (i < transform.childCount)
            {
                bullet = transform.GetChild(i);
            }
            // 아니라면 생성
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                // 부모 객체 변경
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            // 생성될때마다 자연스럽게 각도 조절
            Vector3 rotVec = Vector3.forward * 360 * i / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * (1.5f + id), Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -100 은 무한 관통

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Melee);
        }
    }

    void Fire()
    {
        // 근접한 적 없으면 리턴
        if (!player.scanner.nearestTarget)
            return;

        // 몬스터 위치 가져와서 방향 지정하기
        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        // 총알 생성해서 윗부분이 적을 향하게 회전하면서 이동
        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }


}
