using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BossState
{
    Boss_Idle,
    Boss_Move,
    Boss_Attack,
    Fall_Bull
}

public enum MonType
{
    Walk_Monster,
    Run_Monster,
    plant,
    attack_Monster,
    Snail,
    Boss
}
public class MonsterCtrl : MonoBehaviour
{
    public MonType m_MonType = MonType.Walk_Monster;

    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    PlayerCtrl Player = null;
    CapsuleCollider2D capcoll;

    public float MoveSpeed = 3;
    float RunSpeed = 5;
    float m_Hp = 100;
    [HideInInspector] public float m_CurHp = 100;

    public int nextMove;

    float shootTime = 0;
    float shootDelay = 0.9f;

    bool isDie = false;

    public GameObject m_HpBarObj = null;
    public Image m_hpBarImg = null;
    float m_HP = 400;
    float m_curHp = 400;

    private Transform playerTr;

    [Header("--- Plant Monster --- ")]
    public GameObject m_MonBullet = null;
    public GameObject m_shootPos = null;
    public float Bulletspeed = 10f;


    [Header("--- Trunk Monster --- ")]
    public GameObject m_TMonBullet = null;
    public GameObject m_TshootPos = null;

    public int turn = 1;

    bool isChange;

    float delay_time = 0;

    //Boss
    BossState m_BossState = BossState.Boss_Idle;
    int m_ShootCount = 0;
    public GameObject m_bosshpBarObj = null;
    public Image m_bosshpBarImg = null;

    float move_Time = 0.0f;
    float move_delay = 1.0f;

    public GameObject ballPrefab = null;
    float spawn = 0.5f;
    float delta = 0.0f;
    int fallCount = 0;

    public GameObject m_bossBullet = null;
    public GameObject m_BShootPos = null;


    // Start is called before the first frame update
    void Start()
    {
        isDie = false;

        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        Player = GameObject.FindObjectOfType<PlayerCtrl>();
        capcoll = GetComponent<CapsuleCollider2D>();

        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        if (m_MonType == MonType.Snail)
        {
            m_HP = 200.0f;
            m_curHp = m_HP;
        }

        if (m_MonType == MonType.Boss)
        {
            GameMgr.m_gameLevel = GameLevel.Boss;
            m_Hp = 3000.0f;
            m_curHp = m_Hp;          
        }

 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDie)
        {
            if (m_MonType == MonType.Walk_Monster)
                WalkMonster_AI();
            else if (m_MonType == MonType.Run_Monster)
                RunMonster_AI();
            else if (m_MonType == MonType.plant)
                Plant_AI();
            else if (m_MonType == MonType.attack_Monster)
                Attack_AI();
            else if (m_MonType == MonType.Snail)
                Snail_AI();
            else if (m_MonType == MonType.Boss)
                Boss_AI();
        }
    }

    void Boss_AI()
    {
        Collider2D a_Coll = Physics2D.OverlapBox(transform.position, new Vector2(14, 14), 0, LayerMask.GetMask("Player"));
        if(a_Coll != null)
        {
            m_BossState = BossState.Boss_Move;
            if (m_bosshpBarObj != null)
                m_bosshpBarObj.SetActive(true);
        }

        if (m_BossState == BossState.Boss_Move)
        {          
            rigid.velocity = new Vector2(-turn * MoveSpeed, rigid.velocity.y);

            Vector2 frontVec = new Vector2(rigid.position.x + (-turn * 2.8f), rigid.position.y);
            Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayGHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Wall"));
            if (rayGHit.collider != null)
            {
                turn *= -1;
            }
            if (turn != 0)
                sprite.flipX = turn == -1;

            delay_time += Time.deltaTime;
            if (delay_time >= 5.0f)    //10초 뒤에 돌 떨어트리기
            {   
                delay_time = 0.0f;           
                //m_BossState = BossState.Fall_Bull;
                m_BossState = BossState.Boss_Attack;
            }
            
                           
        }
        else if (m_BossState == BossState.Fall_Bull)
        {
            delta += Time.deltaTime;
            if (delta > spawn)
            {
                GameObject go = Instantiate(ballPrefab) as GameObject;

                int dropPosX = Random.Range(23, 38);
                go.transform.position = new Vector3(dropPosX, 6.2f, 0.0f);

                fallCount++;
                if (fallCount > 9)  //10개 딸
                {
                    fallCount = 0;
                    m_BossState = BossState.Boss_Move;
                }
                delta = 0.0f;
            }

        }
        else if (m_BossState == BossState.Boss_Attack)
        {
            anim.SetTrigger("Attack");

            shootTime += Time.deltaTime;
            if(shootTime > 5)
            {
                shootTime = 0.0f;
                GameObject a_NewObj = Instantiate(m_bossBullet) as GameObject;
                BulletCtrl a_BulletSc = a_NewObj.GetComponent<BulletCtrl>();
                a_BulletSc.BulletSpawn(m_BShootPos.transform.position, Vector3.left, Bulletspeed);
            }
            //shootTime += Time.deltaTime;
            //if(shootTime < shootDelay)
            //{
            //    BulletCtrl a_BulletSc = null;

            //    Vector3 a_Pos;
            //    GameObject a_CloneObj = null;
            //    for(int i = 0; i < 2; i++)
            //    {
            //        a_CloneObj = (GameObject)Instantiate(ballPrefab);
            //        a_Pos = m_BShootPos.transform.position;
            //        a_Pos.y += 0.2f - (i * 0.4f);
            //        a_CloneObj.transform.position = a_Pos;
            //        a_BulletSc = a_CloneObj.GetComponent<BulletCtrl>();
            //        a_BulletSc.BulletSpawn(a_CloneObj.transform.position, Vector3.left, Bulletspeed);

            //    }
            //}
            //shootTime = 0.0f;

        }
    }

    void CheckPlatform()
    {
        Vector2 frontVec = new Vector2(rigid.position.x + turn, rigid.position.y - 0.8f);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayGHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Platform"));
        if (rayGHit.collider == null)
        {
            turn *= -1;
        }
        if (turn != 0)
            sprite.flipX = turn == 1;
    }

    void Snail_AI()
    {
        rigid.velocity = new Vector2(turn, rigid.velocity.y);       
        if(isChange ==true)
        {
             rigid.velocity = new Vector2(turn * 7, rigid.velocity.y);
        }
        CheckPlatform();
    }
    void WalkMonster_AI()
    {
        rigid.velocity = new Vector2(turn, rigid.velocity.y);
        CheckPlatform();
    }
    void RunMonster_AI()
    {
        rigid.velocity = new Vector2(turn * RunSpeed, rigid.velocity.y);
        CheckPlatform();
    }

    void Plant_AI()
    {
        Collider2D a_Coll = Physics2D.OverlapBox(transform.position, new Vector2(14, 10), 0, LayerMask.GetMask("Player"));

        if (a_Coll != null)
        {
 
            anim.SetBool("Attack", true);

            if (a_Coll.transform.position.x < transform.position.x)  //Left
            {
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

    void Attack_AI()
    {
        rigid.velocity = new Vector2(turn * MoveSpeed, rigid.velocity.y);
        CheckPlatform();

        Collider2D a_Coll = Physics2D.OverlapBox(transform.position, new Vector2(20, 10), 0, LayerMask.GetMask("Player"));
        if (a_Coll != null)
        {
            MoveSpeed = 0f;
            anim.SetBool("Attack", true);

            if (a_Coll.transform.position.x < transform.position.x)  //Left
            {
                sprite.flipX = false;
                shootTime += Time.deltaTime;
                if (0.55f <= shootTime)
                {
                    if (m_TMonBullet != null)
                    {
                        GameObject a_NewObj = Instantiate(m_TMonBullet) as GameObject;
                        BulletCtrl a_Bullet = a_NewObj.GetComponent<BulletCtrl>();
                        a_Bullet.BulletSpawn(m_TshootPos.transform.position, Vector3.left, Bulletspeed);
                    }
                    shootTime = 0f;
                }
            }
            else //Right
            {
                sprite.flipX = true;
                shootTime += Time.deltaTime;
                if (0.55f <= shootTime)
                {
                    if (m_TMonBullet != null)
                    {
                        GameObject a_NewObj = Instantiate(m_TMonBullet) as GameObject;
                        BulletCtrl a_Bullet = a_NewObj.GetComponent<BulletCtrl>();
                        a_Bullet.BulletSpawn(m_TshootPos.transform.position, Vector3.right, Bulletspeed);
                    }
                    shootTime = 0f;
                }
            }
        }
        else
        {
            MoveSpeed = 3f;
            anim.SetBool("Attack", false);
        }
    }

    //void WalkJumpMonster_AI()
    //{
    //    rigid.velocity = new Vector2(nextMove * 2f, rigid.velocity.y);

    //    Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.8f, rigid.position.y - 0.8f);
    //    Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
    //    RaycastHit2D rayGHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Platform"));
    //    if (rayGHit.collider == null)
    //    {
    //        nextMove *= -1;
    //        CancelInvoke();
    //        Invoke("Think", 3);
    //    }

    //}

    //void Think()
    //{
    //    nextMove = Random.Range(-1, 2);

    //    anim.SetInteger("WalkSpeed", nextMove);
    //    if (nextMove != 0)
    //        sprite.flipX = nextMove == 1;

    //    Invoke("Think", 3);
    //}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "P_Bullet")
        {
            if (m_MonType == MonType.Snail)
            {
                Destroy(coll.gameObject);
            }
            else
            {
                Destroy(coll.gameObject);
                TakeDemaged();
            }

            if(m_MonType == MonType.Boss)
            {
                BossTakeDemaged();
                OnDamaged();
            }
        }
    }

    public void TakeDemaged(float a_Value = 100)
    {
        anim.SetTrigger("Hit");

        m_curHp -= a_Value;

        if (m_HpBarObj != null)
            m_HpBarObj.SetActive(true);

        if (m_hpBarImg != null)
            m_hpBarImg.fillAmount = m_curHp / m_Hp;

        if(m_MonType == MonType.Snail)
        {
            if (m_curHp == 50)
            {
                anim.SetTrigger("Hit");
                anim.SetBool("ChangeShell", true);
                isChange = true;
            }
            else if (m_curHp <= 0.0f)
            {
                anim.SetTrigger("ShellHit");
                m_curHp = 0.0f;
                GameMgr.Inst.SpawnCoin(transform.position);
                MonsterDie();
            }
        }
        else if(m_MonType == MonType.Boss)
        {
            if (m_bosshpBarImg != null)
                m_bosshpBarImg.fillAmount = m_curHp / m_Hp;
        }
        else
        {
            if (m_curHp <= 0.0f)
            {
                m_curHp = 0.0f;
                m_HpBarObj.SetActive(false);
                GameMgr.Inst.SpawnCoin(transform.position);
                MonsterDie();
            }
        }
        
    }

    public void BossTakeDemaged(float a_Value = 10)
    {
        if (m_curHp <= 0.0f)
            return;

        m_curHp -= a_Value;

        if (m_bosshpBarImg != null)
            m_bosshpBarImg.fillAmount = m_curHp / m_Hp;

        if (m_curHp <= 0.0f)
        {
            m_curHp = 0.0f;
        }
    }


    public void MonsterDie()
    {
        isDie = true;
        rigid.velocity = Vector2.zero;
        gameObject.GetComponentInChildren<CapsuleCollider2D>().enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
         
        Destroy(gameObject,0.5f);
    }

    void OnDamaged()
    {
        sprite.color = new Color(1, 1, 1, 0.4f);

        Invoke("OffDamaged", 1);
    }

    void OffDamaged()
    {
        sprite.color = new Color(1, 1, 1, 1);
    }

}
