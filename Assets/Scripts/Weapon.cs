using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // ���� ����
    public int id;
    // ������ ����
    public int prefabId;
    // ������
    public float damage;
    // ����
    public int count;
    // ���ݼӵ�
    public float speed;

    // ���� �߻��
    float timer;
    // �÷��̾� ��������
    Player player;

    private void Awake()
    {
        player = GameManager.instance.player;
    }

    private void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // ���� �ֺ��� �����ƴٴϰ� 
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

    // ������ ������ �� ���� ����
    public void LevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        if (id == 0 || id == 2)
            Batch(id);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    // ���� �ʱ� ����
    public void Init(ItemData data)
    {
        // �̸� ����
        name = "Weapon " + data.itemId;
        // �θ�ü�� ������
        transform.parent = player.transform;
        // �ʱ⼳�� 0.0.0
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

        // ���� �߻� �ӵ�
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

        // �ڵ� 0 = ���� 1 = ���Ÿ� 
        Hand hand = player.hands[(int)data.itemType];
        hand.sprite.sprite = data.hand;
        hand.gameObject.SetActive(true);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    // ���� ������Ʈ ��ġ�ϱ�
    void Batch(int id)
    {
        for (int i = 0; i < count; i++)
        {
            Transform bullet;

            // �̹� �ִٸ� ���⿡ �߰�
            if (i < transform.childCount)
            {
                bullet = transform.GetChild(i);
            }
            // �ƴ϶�� ����
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                // �θ� ��ü ����
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            // �����ɶ����� �ڿ������� ���� ����
            Vector3 rotVec = Vector3.forward * 360 * i / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * (1.5f + id), Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -100 �� ���� ����

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Melee);
        }
    }

    void Fire()
    {
        // ������ �� ������ ����
        if (!player.scanner.nearestTarget)
            return;

        // ���� ��ġ �����ͼ� ���� �����ϱ�
        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        // �Ѿ� �����ؼ� ���κ��� ���� ���ϰ� ȸ���ϸ鼭 �̵�
        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }


}
