using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 10f;
    private float direction = 1f;
    float lifeTime = 2f;

    void Start()
    {
        
        Destroy(gameObject, lifeTime); //������ ������ �Ѿ��� ����    
    }
    public void SetDirection(float dir) 
    {
        direction = dir;
    }
    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if ( enemy != null ) 
            {
                enemy.OnDie();
            }
            Destroy(gameObject);//�Ѿ� ����

        }
            // �� ü�� ��� �ڵ尡 ������ ���⿡ �ֱ�

        else if (collision.gameObject.tag != "Player") 
        {
            Destroy(gameObject);
        }
    }

}
