using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // �Է� ���Ⱚ
    public Vector2 inputVec;
    // �̵��ӵ�
    public float speed = 3;
    // ������ �� ��������
    public Scanner scanner;
    // �տ� �� ���� 
    public Hand[] hands;
    // �ִϸ����� ��������
    public RuntimeAnimatorController[] animCon;

    // ������Ʈ ��������
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;

    // ������Ʈ �ʱ�ȭ
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
    }

    private void OnEnable()
    {
        speed *= Character.Speed;

        // ������ ĳ���Ϳ� ���� ���� �� �ִϸ��̼� ����
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }

    // �̵��ϴ� �� ���
    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        /// 1. ���� �ش�
        ///rigid.AddForce(inputVec);
        /// 2. �ӵ� ����
        ///rigid.velocity = inputVec;
        Vector2 nextVec = inputVec * speed * Time.deltaTime;
        // 3. ��ġ �̵�
        rigid.MovePosition(rigid.position + nextVec);
    }

    // ��ǲ�ý��� �� ������ ��������
    void OnMove(InputValue value)
    {
        if (!GameManager.instance.isLive)
            return;

        inputVec = value.Get<Vector2>();
    }

    // �۵� ����
    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        // �ִϸ����� �۵�
        anim.SetFloat("Speed", inputVec.magnitude);

        // ��������Ʈ �¿� ���� 
        if (inputVec.x != 0)
            sprite.flipX = inputVec.x < 0;
    }

    // ���̶� ��������� ������
    private void OnCollisionStay2D(Collision2D collision)
    {
        GameManager gm = GameManager.instance;

        if (!gm.isLive)
            return;

        gm.health -= Time.deltaTime * 20;

        if (gm.health < 0)
        {
            for (int i = 2; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);

            anim.SetTrigger("Dead");
            gm.GameOver();
        }
    }
}