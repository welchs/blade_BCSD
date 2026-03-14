using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermove : MonoBehaviour
{
    public float maxspeed;
    public float jumppower;
    private Rigidbody2D rigid;
    SpriteRenderer spriterenderer;
    Animator animator;
    private bool isGrounded = false;
    private Vector3 lastSafePosition;
    public GameObject BulletA;
    public Transform pos;
    bool isDamaged = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 피격 시 입력 무시
        if (isDamaged) return;

        // 점프 입력 처리 (Update에서 입력을 감지)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // 물리적인 힘은 FixedUpdate에서 주는 것이 이상적이나, 점프는 즉각적인 반응이 중요하므로 여기서 처리
            rigid.AddForce(Vector2.up * jumppower, ForceMode2D.Impulse);
            animator.SetBool("isjumping", true);
            isGrounded = false; // 점프 직후 땅에서 떨어진 것으로 처리
        }

        // 총알 발사 입력 처리
        if (Input.GetKeyDown(KeyCode.Z))
        {
            shoot();
        }

        // 이동 방향에 따른 캐릭터 좌우 반전 (시각적 처리)
        if (Input.GetButton("Horizontal"))
        {
            spriterenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        // 걷기 애니메이션 상태 변경 (현재 속도 기반)
        // Mathf.Abs는 절대값을 반환
        if (Mathf.Abs(rigid.linearVelocity.x) < 0.3f)
            animator.SetBool("iswalk", false);
        else
            animator.SetBool("iswalk", true);

        // 맵 밖으로 떨어졌을 때 안전한 위치로 복원
        if (transform.position.y < -10)
        {
            transform.position = lastSafePosition;
        }
    }

    void shoot()
    {
        //총알 생성
        GameObject bullet = Instantiate(BulletA, pos.position, Quaternion.identity);
        // 발사 방향 설정 (좌측이면 -1, 우측이면 1)
        float direction = spriterenderer.flipX ? -1f : 1f;
        // 총알 스크립트에 방향 전달
        bullet.GetComponent<Bullet>().SetDirection(direction);
    }

    void FixedUpdate()
    {
        // 땅 감지 (Raycast) 로직을 최상단으로 이동
        // 피격 상태와 관계없이 항상 땅을 감지해야 착지했을 때 isGrounded가 true로 바뀜
        // 아래로 떨어지고 있을 때만 감지하여 불필요한 연산 방지
        if (rigid.linearVelocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                // 바닥과의 거리가 매우 가깝다면
                if (rayHit.distance < 0.7f)
                {
                    isGrounded = true;
                    animator.SetBool("isjumping", false);
                }
            }
            else
            {
                isGrounded = false;
            }
        }
        
        // 땅 위에 있을 때 안전한 위치 저장
        if (isGrounded)
        {
            lastSafePosition = transform.position;
        }

        // 피격 시 아래 물리 효과(이동)는 무시
        if (isDamaged) return;

        // 수평 이동 입력 받기
        float h = Input.GetAxisRaw("Horizontal");

        // 물리적인 이동 처리
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // 최대 속도 제한
        if (rigid.linearVelocity.x > maxspeed) // 오른쪽 최대 속도
            rigid.linearVelocity = new Vector2(maxspeed, rigid.linearVelocity.y);
        else if (rigid.linearVelocity.x < -maxspeed) // 왼쪽 최대 속도
            rigid.linearVelocity = new Vector2(-maxspeed, rigid.linearVelocity.y);
    }

    // 적과 충돌했을 때
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
            OnDamaged(collision.transform.position);
    }

    void OnDamaged(Vector2 targetPos)
    {
        // 피격 상태로 변경
        isDamaged = true;

        // 물리적 움직임 정지
        rigid.linearVelocity = Vector2.zero;
        // 피격 시 점프 애니메이션 상태를 해제
        animator.SetBool("isjumping", false);

        // 레이어를 변경하여 다른 오브젝트와 충돌하지 않도록 설정 (예: PlayerDamaged 레이어)
        gameObject.layer = 11;

        // 플레이어를 반투명하게 변경
        spriterenderer.color = new Color(1, 1, 1, 0.4f);

        // 튕겨나가는 방향 계산 및 힘 적용
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1).normalized * 5, ForceMode2D.Impulse);

        // 피격 애니메이션 실행
        animator.SetTrigger("doDamaged");

        // 1초 후 무적 상태 해제
        Invoke("OffDamaged", 1);
    }

    void OffDamaged()
    {
        isDamaged = false;
        // 레이어를 원래대로 복구 (예: Player 레이어)
        gameObject.layer = 10;
        spriterenderer.color = new Color(1, 1, 1, 1);
    }
}
