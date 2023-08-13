using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum MovementState
{
    idle,
    running,
    jumping,
    falling
}

public class PlayerCtrl : MonoBehaviour
{
    public static MovementState state = MovementState.idle;

    private float dirX = 0.0f;
    public float moveSpeed = 5.0f;
    public float jumpPower = 7.0f;

    private Rigidbody2D rigid;
    private Animator anim;
    private CapsuleCollider2D coll;
    private SpriteRenderer sprite;

    public GameObject m_BulletObj = null;
    public GameObject m_shootPos = null;
    float BulletSpeed = 10.0f;

    public Image m_HpBarImg = null;
    float m_HP = 1000;
    float m_curHP = 1000;

    LayerMask playerState;

    bool isDie = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();

        isDie = false;
    }

    void FixedUpdate()
    {
        Vector2 Dir = new Vector2(transform.position.x, transform.position.y - 0.5f);
        RaycastHit2D rayhit = Physics2D.BoxCast(Dir, new Vector2(1,1), 0, Vector2.down, LayerMask.GetMask("Platform"));
        Debug.DrawRay(transform.position, Vector2.down);
        if(rayhit.collider != null) { }
 
    }
    // Update is called once per frame
    void Update()
    {
        if (!isDie)
        { 
            //좌우이동
            dirX = Input.GetAxis("Horizontal");
            transform.Translate(dirX * Time.deltaTime * moveSpeed, 0, 0);

            //점프
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            }

            //총알 발사
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (m_BulletObj == null)
                    return;

                GameObject a_BullObj = Instantiate(m_BulletObj) as GameObject;
                BulletCtrl a_Bullet = a_BullObj.GetComponent<BulletCtrl>();
                a_Bullet.BulletSpawn(m_shootPos.transform.position, Vector3.right, BulletSpeed);

                if (transform.localScale.x == -1)
                {
                    a_Bullet.BulletSpawn(m_shootPos.transform.position, Vector3.left, BulletSpeed);
                }

            }
            UpdateAnimState();
        }
        LimitMove();
    }

    void LimitMove()
    {
        if(transform.position.x < -11.0f)
        {
            transform.position = new Vector2(-11.0f, transform.position.y);
        }
    }
    bool IsGrounded()
    {
        //플레이어로부터 Vector2.down 방향으로 Ray를 쏘아서 Raycast의 충돌로써 땅 위에 있는지를 판정
        return Physics2D.BoxCast(transform.position, new Vector2(1,1), 0f, Vector2.down, 1f, LayerMask.GetMask("Platform"));
    }

    void UpdateAnimState()
    {
        if (dirX > 0)   //오른쪽
        {
            anim.SetInteger("state", 1);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (dirX < 0)  //왼쪽
        {
            anim.SetInteger("state", 1);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            anim.SetInteger("state", 0);
        }

        if (rigid.velocity.y > 0.1f)
        {
            anim.SetInteger("state", 2);
        }
        else if (rigid.velocity.y < -0.1f)
        {
            anim.SetInteger("state", 3);
        }
    }

 

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Monster")
        {
            PlayerTakeDemaged();
            OnDamaged(coll.transform.position);
        }
        if(coll.gameObject.tag == "M_Bullet")
        {
            Destroy(coll.gameObject);
            PlayerTakeDemaged();
            OnDamaged(coll.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.name.Contains("Coin"))
        {
            GameMgr.Inst.AddGold();
            Destroy(coll.gameObject);
        }
    }

    void PlayerTakeDemaged()
    {
        m_curHP -= 100;

        if (m_HpBarImg != null)
            m_HpBarImg.fillAmount = m_curHP / m_HP;

        if(m_curHP <=0)
        {
            m_curHP = 0;
            PlayerDie();
        }
    }



    void OnDamaged(Vector2 targetPos)
    {
        playerState = 1 << 10;
        sprite.color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1f) * 2, ForceMode2D.Impulse);
        Invoke("OffDamaged", 1);
    }

    void OffDamaged()
    {
        playerState = 1 << 9;
        sprite.color = new Color(1, 1, 1, 1);
    }

    void PlayerDie()
    {
        isDie = true;
        anim.SetTrigger("Die");
       // SceneManager.LoadScene("SampleScene")
    }
}
