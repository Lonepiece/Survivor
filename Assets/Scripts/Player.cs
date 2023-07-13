using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // 입력 방향값
    public Vector2 inputVec;
    // 이동속도
    public float speed = 3;
    // 근접한 적 가져오기
    public Scanner scanner;
    // 손에 든 무기 
    public Hand[] hands;
    // 애니메이터 가져오기
    public RuntimeAnimatorController[] animCon;

    // 컴포넌트 가져오기
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;

    // 컴포넌트 초기화
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

        // 선택한 캐릭터에 따라 외형 및 애니메이션 변경
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }

    // 이동하는 힘 계산
    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        /// 1. 힘을 준다
        ///rigid.AddForce(inputVec);
        /// 2. 속도 제어
        ///rigid.velocity = inputVec;
        Vector2 nextVec = inputVec * speed * Time.deltaTime;
        // 3. 위치 이동
        rigid.MovePosition(rigid.position + nextVec);
    }

    // 인풋시스템 중 움직임 가져오기
    void OnMove(InputValue value)
    {
        if (!GameManager.instance.isLive)
            return;

        inputVec = value.Get<Vector2>();
    }

    // 작동 구현
    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        // 애니메이터 작동
        anim.SetFloat("Speed", inputVec.magnitude);

        // 스프라이트 좌우 변경 
        if (inputVec.x != 0)
            sprite.flipX = inputVec.x < 0;
    }

    // 몹이랑 닿아있으면 데미지
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