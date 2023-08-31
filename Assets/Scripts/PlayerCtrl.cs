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
    private CapsuleCollider2D playerColl;
    private SpriteRenderer sprite;

    //--- �Ѿ� ���� ---
    public GameObject m_BulletObj = null;
    public GameObject m_shootPos = null;
    float BulletSpeed = 10.0f;

    public Image m_HpBarImg = null;
    public float initHp = 500.0f;
    public float hp = 500.0f;

    LayerMask playerState;

    LayerMask groundMask = -1;
    LayerMask shieldMask = -1;

    bool isDie = false;

    //-- 쉴드
    float m_SdOnTime = 0.0f;
    float m_SdDuration = 10.0f; //15초 동안 발동
    public GameObject ShieldObj = null;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerColl = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();

        isDie = false;

        groundMask = 1 << LayerMask.NameToLayer("Platform") | 1 << LayerMask.NameToLayer("AirPlatform");


    }

    void FixedUpdate()
    {
    }
    // Update is called once per frame

    void Update()
    {

        if (!isDie)
        {
            //이동
            dirX = Input.GetAxis("Horizontal");
            transform.Translate(dirX * Time.deltaTime * moveSpeed, 0, 0);

            //점프
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            }

            //공격
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

        SkillUpdate();
    }

    void LimitMove()
    {
        if (transform.position.x <= -10.5f)
            transform.position = new Vector2(-10.5f, transform.position.y);
    }
    bool IsGrounded()
    {
        return Physics2D.BoxCast(playerColl.bounds.center, playerColl.bounds.size, 0f, Vector2.down, 0.5f, groundMask);
    }

    void UpdateAnimState()
    {
        //좌우 이동
        if (dirX > 0)   
        {
            anim.SetInteger("state", 1);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (dirX < 0)  
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
            PlayerTakeDemaged(10);
            OnDamaged(coll.transform.position);
            
        }
        if(coll.gameObject.tag == "Snail")
        {
            //떨어지는 중이고 snail보다 위에 있을 때 -> 밟을 때
            if (rigid.velocity.y < 0 && transform.position.y > coll.transform.position.y)
            {
                OnAttack(coll.transform);
            }
            else
            {
                PlayerTakeDemaged(50);
                OnDamaged(coll.transform.position);

            }
        }

        if(coll.gameObject.tag == "M_Bullet")   
        {
            Destroy(coll.gameObject);
            PlayerTakeDemaged(50);
            OnDamaged(coll.transform.position);
        }

        if(coll.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            PlayerTakeDemaged(50);
            OnDamaged(coll.transform.position);
        }

        if (coll.gameObject.tag == "Boss")
        {
            PlayerTakeDemaged(50);
            OnDamaged(coll.transform.position);
        }

        if(coll.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            PlayerTakeDemaged(50);
            OnDamaged(coll.transform.position);
        }
    }

    void OnAttack(Transform enemy)
    {
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        MonsterCtrl mon = enemy.GetComponent<MonsterCtrl>();
        mon.TakeDemaged();     
    }

    public bool isLadder;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.name.Contains("Coin"))
        {
            GameMgr.Inst.AddGold();
            Destroy(coll.gameObject);
        }
        if(coll.gameObject.name.Contains("door"))
        {
            GameMgr.m_gameLevel = GameLevel.Boss;
            SceneManager.LoadScene("BossScene");
            
        }
        if(coll.gameObject.name.Contains("Wall"))
        {
            coll.isTrigger = true;
        }
        else if(coll.gameObject.CompareTag("ladder"))
        {
            isLadder = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.name.Contains("Wall"))
        { 
            coll.isTrigger = false;
        }
        else if(coll.gameObject.CompareTag("ladder"))
        {
            isLadder = false;

        }
    }

    void PlayerTakeDemaged(int a_Value)
    {
        if (0.0f < m_SdOnTime)  //쉴드 스킬 발동 중일 때.. 데미지 스킬
            return;

        GameMgr.Inst.DamageText(-a_Value, transform.position, Color.blue);

        hp -= a_Value;

        if (m_HpBarImg != null)
            m_HpBarImg.fillAmount = hp / initHp;
  
        if (hp <=0)
        {
            hp = 0;
            PlayerDie();
        }       

         
    }

    public void UseSkill_Item(SkillType a_SkType)
    {
        if(a_SkType == SkillType.Skill_0)
        {
            hp += (int)(initHp * 0.3f);

            if (initHp < hp)
                hp = initHp;

            if (m_HpBarImg != null)
                m_HpBarImg.fillAmount = hp / initHp;
        }

        else if(a_SkType == SkillType.Skill_1)
        {
            if (0.0f < m_SdOnTime)
                return;

            m_SdOnTime = m_SdDuration;
        
        }

        int a_SkIdx = (int)a_SkType;    //SkillType 인덱스로 변환
        GlobalValue.g_SkillCount[a_SkIdx]--;    //스킬카운트 차감
        string a_Skill = "SkItem_" + (a_SkIdx).ToString();
        PlayerPrefs.SetInt(a_Skill, GlobalValue.g_SkillCount[a_SkIdx]);
    }

    void SkillUpdate()
    {
        //쉴드 상태 업데이트
        if(0.0f < m_SdOnTime)
        {
            m_SdOnTime -= Time.deltaTime;
            if (ShieldObj != null && ShieldObj.activeSelf == false)
                ShieldObj.SetActive(true);

            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
                    LayerMask.NameToLayer("Monster"), true);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
                   LayerMask.NameToLayer("Trap"), true);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
                   LayerMask.NameToLayer("Boss"), true);
        }
        else
        {
            if (ShieldObj != null && ShieldObj.activeSelf == true)
                ShieldObj.SetActive(false);

            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
                   LayerMask.NameToLayer("Monster"), false);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
                   LayerMask.NameToLayer("Trap"), false);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
                   LayerMask.NameToLayer("Boss"), false);
        }
    }  
        

    void OnDamaged(Vector2 targetPos)
    {
        if (0.0f < m_SdOnTime)  //쉴드 스킬 발동 중일 때.. 데미지 스킬
            return;

        gameObject.layer = 10;
        sprite.color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 0.5f) * 5, ForceMode2D.Impulse);
        Invoke("OffDamaged", 1);
    }

    void OffDamaged()
    {
        gameObject.layer = 6;
        sprite.color = new Color(1, 1, 1, 1);
    }

    void PlayerDie()
    {
        isDie = true;      
        anim.SetTrigger("Die");
        Time.timeScale = 0.0f;
    }
}
