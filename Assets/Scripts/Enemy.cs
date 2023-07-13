using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    // �� �̵��ӵ�
    public float speed;
    // �� ���� ü��
    public float health;
    // �� �ִ� ü��
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    // �i�ư� Ÿ��
    public Rigidbody2D target;

    // ��� Ȯ��
    bool isLive = true;

    // ������Ʈ ��������
    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer sprite;
    WaitForFixedUpdate wait;

    // ���� �� �ʱ�ȭ
    private void OnEnable()
    {
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        sprite.sortingOrder = 2;
        anim.SetBool("Dead", false);
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        health = maxHealth;
    }

    // ������Ʈ �ʱ�ȭ
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        // ������� �ƴ϶�� ����
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        // Ÿ���� ��ġ�� �� ��ġ�� Ȯ���ؼ� �����ϱ�
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position + nextVec);
        // �浹 �� �з��� ����
        rigid.velocity = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive)
            return;

        // �¿� ���� ��Ű��
        sprite.flipX = target.position.x < rigid.position.x;
    }

    // ���� ��ȯ �� ���� �����Ϳ� ���� ���� �ʱ�ȭ
    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    // ������ ����� �ε������� ó��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());

        if (health > 0)
        {
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            sprite.sortingOrder = 1;
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp();

            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }
    }

    IEnumerator KnockBack()
    {

        yield return wait; //���� �ϳ��� ���� ������ ������
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}