using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonType
{
    bunny,
    Mushroom,
    plant,
    Rock,
    MiniRock
}
public class MonsterCtrl : MonoBehaviour
{

    public MonType m_MonType = MonType.Mushroom;

    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    public Transform PlayerTr;

    public float speed = 7;
    private float m_Hp = 100;
    private float m_CurHp = 100;

    public int nextMove;
    public float distance;

    float shootTime = 0;
    float shootDelay = 1f;

    bool isDie = false;

    public GameObject m_HpBarObj = null;
    public Image m_hpBarImg = null;
    float m_HP = 100;
    float m_curHp = 100;

    [Header("--- Plant Monster --- ")]
    public GameObject m_MonBullet = null;
    public GameObject m_shootPos = null;
    public float Bulletspeed = 10f;
    float attackDist = 7f;

    float turn = 1;

    void Awake()
    {
        if(m_MonType == MonType.Mushroom)
            Invoke("Think", 5);
    }

    // Start is called before the first frame update
    void Start()
    {
        isDie = false;

        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        PlayerTr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDie)
        {
            if (m_MonType == MonType.bunny)
                bunny_AI();

            if (m_MonType == MonType.Mushroom)
                mushroom_AI();

            if (m_MonType == MonType.plant)
                plant_AI();

            if (m_MonType == MonType.Rock)
                rock_AI();
        }
    }

    void mushroom_AI()
    {
        //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y - 0.8f);
        Debug.DrawRay(frontVec, Vector2.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 0.5f, LayerMask.GetMask("Platform"));
        if(rayHit.collider == null)
        {
            nextMove *= -1;
            sprite.flipX = nextMove == 1;
            CancelInvoke(); //현재 작동 중인 모든 Invoke 함수를 멈춤
            Invoke("Think", 2);
        }
    }
    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2);
       
        //Animation
        anim.SetInteger("WalkSpeed", nextMove);

        //Flip Sprite
        if (nextMove != 0)
            sprite.flipX = nextMove == 1;   //오른쪽으로 가면 flipx =true
        
        //Set Next Active
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    void plant_AI()
    {
        Debug.DrawRay(transform.position, Vector3.left, new Color(0, 1, 0));
        Debug.DrawRay(transform.position, Vector3.right, new Color(0, 1, 0));

        RaycastHit2D rayLHit = Physics2D.Raycast(transform.position, Vector3.left, 10, LayerMask.GetMask("Player"));
        RaycastHit2D rayRHit = Physics2D.Raycast(transform.position, Vector3.right, 10, LayerMask.GetMask("Player"));

        if (rayLHit.collider != null)
        {
            transform.localScale = new Vector3(1, 1, 1);

            Debug.Log(rayLHit.collider.name);
            float distance = Vector2.Distance(transform.position, rayLHit.collider.transform.position);
            if (distance < attackDist)
            {
                anim.SetBool("Attack", true);

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
            else
            {
                anim.SetBool("Attack", false);
            }
        }

        else if (rayRHit.collider != null)
        {
            transform.localScale = new Vector3(-1, 1, 1);

            float distance = Vector2.Distance(transform.position, rayRHit.collider.transform.position);
            if (distance < attackDist)
            {
                anim.SetBool("Attack", true);

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
            else
            {
                anim.SetBool("Attack", false);
            }
        }
    }

    void rock_AI()
    {
        rigid.velocity = new Vector2(turn * 3f, rigid.velocity.y);

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
    void bunny_AI()
    {
        rigid.velocity = new Vector2(nextMove * 3f, rigid.velocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.7f, rigid.position.y - 0.5f);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayGHit = Physics2D.Raycast(frontVec, Vector2.down, 1f, LayerMask.GetMask("Platform"));
        if (rayGHit.collider == null)
        {
            nextMove *= -1;
        }
        if (nextMove == 1)
            sprite.flipX = false;
        else if (nextMove == -1)
            sprite.flipX = true;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag =="P_Bullet")
        {
            Destroy(coll.gameObject);
            TakeDemaged();
        }
    }
    public void TakeDemaged()
    {
        if (m_HpBarObj != null)
            m_HpBarObj.SetActive(true);

        m_curHp -= 20;

        if (m_hpBarImg != null)
            m_hpBarImg.fillAmount = m_curHp / m_Hp;

        anim.SetTrigger("Hit");

        if (m_curHp <= 0)
        {
            m_curHp = 0;
            m_HpBarObj.SetActive(false);
            MonsterDie();          
        }

        //OnMonDamaged();

    }

    void MonsterDie()
    {
        isDie = true;
        
        Destroy(gameObject, 0.4f);

        if(GameMgr.Inst.m_CoinItem != null)
        {
            GameObject a_CoinObj = Instantiate(GameMgr.Inst.m_CoinItem) as GameObject;
            a_CoinObj.transform.position = this.transform.position;
            
        }
    }

    void OnMonDamaged()
    {
        gameObject.layer = 9;
        sprite.color = new Color(0, 0, 0, 0.4f);
        Invoke("OffDamaged", 0.1f);
    }

    void OffDamaged()
    {
        gameObject.layer = 8;
        sprite.color = new Color(1, 1, 1, 1);
    }
}
