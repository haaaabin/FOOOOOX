using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonType
{
    Walk_Monster,
    Run_Monster,
    plant,
    MiniRock,
    Fly_Monster,
    WalkJumpMonster
}
public class MonsterCtrl : MonoBehaviour
{
    public MonType m_MonType = MonType.Walk_Monster;

    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    public Transform PlayerTr;

    public float MoveSpeed = 2;
    float RunSpeed = 5;
    float m_Hp = 100;
    float m_CurHp = 100;

    public int nextMove;

    float shootTime = 0;
    float shootDelay = 0.9f;

    bool isDie = false;

    public GameObject m_HpBarObj = null;
    public Image m_hpBarImg = null;
    float m_HP = 100;
    float m_curHp = 100;

    [Header("--- Plant Monster --- ")]
    public GameObject m_MonBullet = null;
    public GameObject m_shootPos = null;
    public float Bulletspeed = 10f;

    float turn = 1;

    void Awake()
    {
        Invoke("Think", 3);
    }

    // Start is called before the first frame update
    void Start()
    {
        isDie = false;

        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        PlayerTr = GetComponent<Transform>();


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDie)
        {
            if (m_MonType == MonType.Walk_Monster)
                WalkMonster_AI();
            if (m_MonType == MonType.Run_Monster)
                RunMonster_AI();
            if (m_MonType == MonType.plant)
                Plant_AI();
            if (m_MonType == MonType.Fly_Monster)
                FlyMonster_AI();
            if (m_MonType == MonType.WalkJumpMonster)
                WalkJumpMonster_AI();
        }
    }

    void WalkMonster_AI()
    {
        rigid.velocity = new Vector2(turn, rigid.velocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + turn * 0.8f, rigid.position.y - 0.8f);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayGHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Platform"));
        if (rayGHit.collider == null)
        {
            turn *= -1;
        }
        if (turn != 0)
            sprite.flipX = turn == 1;
    }

    void Plant_AI()
    {
        Collider2D a_Coll = Physics2D.OverlapBox(transform.position, new Vector2(14, 14), 0, LayerMask.GetMask("Player"));

        if(a_Coll != null)
        {
            if(a_Coll.transform.position.x < transform.position.x)  //Left
            {
                anim.SetBool("Attack", true);
                sprite.flipX = false;
                shootTime += Time.deltaTime;
                if (shootDelay <= shootTime)
                {
                    if (m_MonBullet != null)
                    {
                        GameObject a_NewObj = Instantiate(m_MonBullet) as GameObject;
                        BulletCtrl a_Bullet = a_NewObj.GetComponent<BulletCtrl>();
                        a_Bullet.BulletSpawn(m_shootPos.transform.position, Vector3.left, Bulletspeed);
                    }
                    shootTime = 0f;
                }
            }
            else //Right
            {
                anim.SetBool("Attack", true);

                sprite.flipX = true;
                shootTime += Time.deltaTime;
                if (shootDelay <= shootTime)
                {
                    if (m_MonBullet != null)
                    {
                        GameObject a_NewObj = Instantiate(m_MonBullet) as GameObject;
                        BulletCtrl a_Bullet = a_NewObj.GetComponent<BulletCtrl>();
                        a_Bullet.BulletSpawn(m_shootPos.transform.position, Vector3.right, Bulletspeed);
                    }
                    shootTime = 0f;
                }
            }   
        }
        else
        {
            anim.SetBool("Attack", false);
        }
    }

    void RunMonster_AI()
    {
        rigid.velocity = new Vector2(turn * RunSpeed, rigid.velocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + turn, rigid.position.y - 0.7f);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayGHit = Physics2D.Raycast(frontVec, Vector2.down, 0.5f, LayerMask.GetMask("Platform"));
        if (rayGHit.collider == null)
        {
            turn *= -1;
        }
        if (turn != 0)
            sprite.flipX = turn == 1;
    }

    void FlyMonster_AI()
    {

    }

    void WalkJumpMonster_AI()
    {
        rigid.velocity = new Vector2(nextMove * 2f, rigid.velocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.8f, rigid.position.y - 0.8f);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayGHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Platform"));
        if (rayGHit.collider == null)
        {
            nextMove *= -1;
            CancelInvoke();
            Invoke("Think", 3);
        }
        
    }

    void Think()
    {
        nextMove = Random.Range(-1, 2);

        anim.SetInteger("WalkSpeed", nextMove);
        if (nextMove != 0)
            sprite.flipX = nextMove == 1;

        Invoke("Think", 3);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag =="P_Bullet")
        {
            Destroy(coll.gameObject);
            TakeDemaged(20);
        }
    }

    public void TakeDemaged(float a_Value)
    {
        anim.SetTrigger("Hit");

        if (m_HpBarObj != null)
            m_HpBarObj.SetActive(true);

        m_curHp -= a_Value;

        if (m_hpBarImg != null)
            m_hpBarImg.fillAmount = m_curHp / m_Hp;

        if (m_curHp <= 0)
        {
            m_curHp = 0;
            m_HpBarObj.SetActive(false);
            MonsterDie();       
            
        }

    }

    void MonsterDie()
    {
        isDie = true;
        anim.SetTrigger("Death");
        Destroy(gameObject, 0.3f);

        SpawnCoin();     
    }

    void SpawnCoin()
    {
        if (GameMgr.Inst.m_CoinItem != null)
        {
            GameObject a_CoinObj = Instantiate(GameMgr.Inst.m_CoinItem) as GameObject;
            a_CoinObj.transform.position = this.transform.position;

        }
    }

}
