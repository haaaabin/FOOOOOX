using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BossState   //보스 상태
{
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

    //move
    public float MoveSpeed = 3;
    float RunSpeed = 5;

    //hpBar
    public GameObject m_HpBarObj = null;
    public Image m_hpBarImg = null;
    float m_Hp = 100;
    [HideInInspector] public float m_CurHp = 100;

    //--- Monster
    public int turn = 1;
    [Header("--- Plant Monster --- ")]
    public GameObject m_MonBullet = null;
    public GameObject m_shootPos = null;
    public float Bulletspeed = 10.0f;

    [Header("--- Trunk Monster --- ")]
    public GameObject m_TMonBullet = null;
    public GameObject m_TshootPos = null;

    [Header("--- Boss --- ")]
    BossState m_BossState = BossState.Boss_Move;
    int m_ShootCount = 0;
    public GameObject m_bosshpBarObj = null;
    public Image m_bosshpBarImg = null;
    public GameObject m_bossBullet = null;
    public float Boss_Bulletspeed = 15.0f;
    public GameObject m_BShootPos = null;

    [Header("--- Bullet --- ")]
    public GameObject ballPrefab = null;
    int fallCount = 0;
    float shootTime = 0;
    float shootDelay = 0.9f;

    float delay_time = 10;
    bool isChange;  //snail 애니메이션 변화 

    bool isDie = false;
    
    // Start is called before the first frame update
    void Start()
    {
        isDie = false;

        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (m_MonType == MonType.Walk_Monster)
        {
            m_Hp = 50.0f;
            m_CurHp = m_Hp;
        }
        else if (m_MonType == MonType.attack_Monster)
        {
            m_Hp = 200.0f;
            m_CurHp = m_Hp;
        }
        else if (m_MonType == MonType.Boss)
        {
            GameMgr.m_gameState = GameState.Boss;
            m_Hp = 5000.0f;
            m_CurHp = m_Hp;          
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
                        MonBulletCtrl a_Bullet = a_NewObj.GetComponent<MonBulletCtrl>();
                        a_Bullet.MBulletSpawn(m_shootPos.transform.position, Vector3.left, Bulletspeed);
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
                        MonBulletCtrl a_Bullet = a_NewObj.GetComponent<MonBulletCtrl>();
                        a_Bullet.MBulletSpawn(m_shootPos.transform.position, Vector3.right, Bulletspeed);
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

    void Snail_AI()
    {
        rigid.velocity = new Vector2(turn, rigid.velocity.y);
        if (isChange == true)
        {
            rigid.velocity = new Vector2(turn * 7, rigid.velocity.y);
        }
        CheckPlatform();
    }

    void Attack_AI()
    {
        rigid.velocity = new Vector2(turn * MoveSpeed, rigid.velocity.y);
        CheckPlatform();

        Collider2D a_Coll = Physics2D.OverlapBox(transform.position, new Vector2(20, 6), 0, LayerMask.GetMask("Player"));
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
                    if (m_TMonBullet != null)   //총알 발사
                    {
                        GameObject a_NewObj = Instantiate(m_TMonBullet) as GameObject;
                        MonBulletCtrl a_Bullet = a_NewObj.GetComponent<MonBulletCtrl>();
                        a_Bullet.MBulletSpawn(m_TshootPos.transform.position, Vector3.left, Bulletspeed);
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
                        MonBulletCtrl a_Bullet = a_NewObj.GetComponent<MonBulletCtrl>();
                        a_Bullet.MBulletSpawn(m_TshootPos.transform.position, Vector3.right, Bulletspeed);
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

    void Boss_AI()
    {
        Collider2D a_Coll = Physics2D.OverlapBox(transform.position, new Vector2(20, 20), 0, LayerMask.GetMask("Player"));
        if (a_Coll != null)
        {
            if (m_bosshpBarObj != null)
                m_bosshpBarObj.SetActive(true);          
        }

        if (m_BossState == BossState.Boss_Move)
        {
            rigid.velocity = new Vector2(-turn * 4, rigid.velocity.y);

            Vector2 frontVec = new Vector2(rigid.position.x + (-turn * 2.8f), rigid.position.y);
            Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayGHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Wall"));
            if (rayGHit.collider != null)
            {
                turn *= -1;
            }
            if (turn != 0)
                sprite.flipX = turn == -1;

            delay_time -= Time.deltaTime;
            if (delay_time <= 0.0f)
            {
                delay_time = 1f;
                shootTime = 0.5f;
                m_BossState = BossState.Fall_Bull;
            }
        }
        else if (m_BossState == BossState.Fall_Bull)   //공 떨어트리기 
        {
            delay_time -= Time.deltaTime;
            if (delay_time <= 0.0f)
            {
                rigid.velocity = new Vector2(-turn * 5, rigid.velocity.y);
                Vector2 frontVec = new Vector2(rigid.position.x + (-turn * 2.8f), rigid.position.y);
                Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
                RaycastHit2D rayGHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Wall"));
                if (rayGHit.collider != null)
                {
                    turn *= -1;
                }
                if (turn != 0)
                    sprite.flipX = turn == -1;

                shootTime -= Time.deltaTime;
                if (shootTime <= 0.0f)
                {
                    GameObject go = Instantiate(ballPrefab) as GameObject;

                    int dropPosX = Random.Range(23, 38);
                    go.transform.position = new Vector3(dropPosX, 6.2f, 0.0f);

                    fallCount++;
                    if (fallCount < 14)
                        shootTime = 0.5f; 
                    else
                    {
                        fallCount = 0;
                        shootTime = 0.5f;
                        delay_time = 1;
                        m_BossState = BossState.Boss_Attack;
                    }
                    Sound_Mgr.Instance.PlayGUISound("Fall", 1.2f);
                }
            }
        }
        else if (m_BossState == BossState.Boss_Attack)  //이동 & 공격 
        {
            delay_time -= Time.deltaTime;
            if(delay_time <= 0.0f)
            {
                sprite.color = new Color(1, 0, 0, 1);

                anim.SetBool("Attack", true);

                rigid.velocity = new Vector2(-turn * 9, rigid.velocity.y);
                Vector2 frontVec = new Vector2(rigid.position.x + (-turn * 2.8f), rigid.position.y);
                Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
                RaycastHit2D rayGHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Wall"));
                if (rayGHit.collider != null)
                {
                    turn *= -1;
                }
                if (turn != 0)
                    sprite.flipX = turn == -1;

                shootTime -= Time.deltaTime;
                if (shootTime <= 0.0f)
                {
                    MonBulletCtrl a_BulletSc = null;

                    Vector3 a_Pos;
                    GameObject a_CloneObj = null;
                    for (int i = 0; i < 2; i++)
                    {
                        a_CloneObj = (GameObject)Instantiate(m_bossBullet);
                        a_Pos = m_BShootPos.transform.position;
                        a_Pos.y -= 0.4f - (i * 1.2f);
                        a_CloneObj.transform.position = a_Pos;
                        a_BulletSc = a_CloneObj.GetComponent<MonBulletCtrl>();
                        a_BulletSc.MBulletSpawn(a_CloneObj.transform.position, Vector3.left * turn, Boss_Bulletspeed);
                    }
                    m_ShootCount++;
                    if (m_ShootCount < 19)
                        shootTime = 0.7f;
                    else
                    {
                        sprite.color = new Color(1, 1, 1, 1);
                        anim.SetBool("Attack", false);
                        m_ShootCount = 0;
                        delay_time = 10.0f;
                        m_BossState = BossState.Boss_Move;
                    }
                }
            }  
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

        if(coll.gameObject.tag == "Shield")
        {
            if(coll.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                MonsterDie();
            }
        }
    }

    public void TakeDemaged(float a_Value = 50)
    {
        anim.SetTrigger("Hit");

        m_CurHp -= a_Value;

        if (m_HpBarObj != null)
            m_HpBarObj.SetActive(true);

        if (m_hpBarImg != null)
            m_hpBarImg.fillAmount = m_CurHp / m_Hp;

        if(m_MonType == MonType.Snail)
        {
            if (m_CurHp == 50)
            {
                anim.SetTrigger("Hit");
                anim.SetBool("ChangeShell", true);
                isChange = true;
            }
            else if (m_CurHp <= 0.0f)
            {
                anim.SetTrigger("ShellHit");
                m_CurHp = 0.0f;
                GameMgr.Inst.SpawnCoin(transform.position);
                MonsterDie();
            }
        }
        else
        {
            if (m_CurHp <= 0.0f)
            {
                m_CurHp = 0.0f;
                m_HpBarObj.SetActive(false);
                GameMgr.Inst.SpawnCoin(transform.position);
                MonsterDie();
            }
        }       
    }

    public void BossTakeDemaged(float a_Value = 50)
    {
        if (m_CurHp <= 0.0f)
            return;

        m_CurHp -= a_Value;

        if (m_bosshpBarImg != null)
            m_bosshpBarImg.fillAmount = m_CurHp / m_Hp;

        if (m_CurHp <= 0.0f)
        {
            m_CurHp = 0.0f;
            m_bosshpBarObj.SetActive(false);
            MonsterDie();
            GameMgr.Inst.SpawnDia(transform.position);
        }
    }

    public void MonsterDie()
    {
        isDie = true;

        //죽으면 업되면서 바닥으로 떨어지게
        rigid.velocity = Vector2.zero;
        gameObject.GetComponentInChildren<CapsuleCollider2D>().enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
         
        Destroy(gameObject,0.5f);
    }

    void OnDamaged()
    {
        sprite.color = new Color(1, 0, 0, 0.5f);

        Invoke("OffDamaged", 1);
    }

    void OffDamaged()
    {
        sprite.color = new Color(1, 1, 1, 1);
    }

}
