using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextmove;
    Animator animator;
    SpriteRenderer spriterenderer;
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriterenderer = GetComponent<SpriteRenderer>();
        Think();
        Invoke("Think", 5);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move
        rigid.linearVelocity = new Vector2(nextmove, rigid.linearVelocity.y);

        //PlatForm Check
        Vector2 frontvec = new Vector2(rigid.position.x + nextmove*0.5f, rigid.position.y);
        Debug.DrawRay(frontvec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontvec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
            Turn();
    }
    // ����Լ�
    void Think() 
    {
        nextmove = Random.Range(-1, 2);

        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);

        animator.SetInteger("WalkSpeed", nextmove);

        if (nextmove != 0) 
            spriterenderer.flipX = nextmove == 1;
    }

    void Turn() 
    {
        nextmove *= -1;
        CancelInvoke();
        Invoke("Think", 5);
        spriterenderer.flipX = nextmove == 1;
    }

    public void OnDie() 
    {
        // �浹 ��Ȱ��ȭ
        GetComponent<Collider2D>().enabled = false;
        rigid.linearVelocity = Vector2.zero;
        rigid.isKinematic = true;

        //�ִϸ��̼� ���
        animator.SetTrigger("doDie");

        // ����ȭ (����)
        spriterenderer.color = new Color(1, 1, 1, 0.6f);

        //0.5���� ������Ʈ ����
        Destroy(gameObject, 0.5f);
    }
}
