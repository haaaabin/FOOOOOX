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
    private Rigidbody2D rigid;
    private Animator anim;
    private CapsuleCollider2D playerColl;
    private SpriteRenderer sprite;
    private float dirX = 0.0f;
    private float moveSpeed = 5.0f;
    private float jumpPower = 7.0f;
    public GameObject bullet;
    public GameObject shootPos;
    private float bulletSpeed = 10.0f;

    public Image hpBarImg;
    public static float initHp = 500.0f;
    public static float hp = 500.0f;

    LayerMask playerState;

    private LayerMask groundMask = -1;
    LayerMask shieldMask = -1;

    private bool isDie = false;

    //-- 쉴드
    private float shieldOnTime = 0.0f;
    private float shieldDuration = 10.0f; //15초 동안 발동
    public GameObject shieldObj = null;

    private void Start()
    {
        GlobalValue.LoadGameData();
        Time.timeScale = 1;

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerColl = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        // groundMask = 1 << LayerMask.NameToLayer("Platform") | 1 << LayerMask.NameToLayer("AirPlatform");
        groundMask = LayerMask.GetMask("Platform", "AirPlatform");
        isDie = false;
    }

    private void Update()
    {
        if (isDie)
            return;

        HandleInput();
        UpdateAnimState();
        LimitMove();
        SkillUpdate();
    }

    private void HandleInput()
    {
        dirX = Input.GetAxis("Horizontal");
        transform.Translate(dirX * Time.deltaTime * moveSpeed, 0, 0);

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        if (bullet != null)
        {
            GameObject bulletObject = Instantiate(bullet, shootPos.transform.position, Quaternion.identity);
            BulletController bulletCtrl = bulletObject.GetComponent<BulletController>();
            Vector3 direction = transform.localScale.x == 1 ? Vector3.right : Vector3.left;
            bulletCtrl.InitializeBullet(shootPos.transform.position, direction, bulletSpeed);
            SoundManager.Instance.PlayGUISound("gun", 1.0f);
        }
    }

    private void LimitMove()
    {
        if (transform.position.x <= -10.5f)
        {
            transform.position = new Vector2(-10.5f, transform.position.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(playerColl.bounds.center, playerColl.bounds.size, 0f, Vector2.down, 0.5f, groundMask);
    }

    private void UpdateAnimState()
    {
        if (dirX != 0)  // walk
        {
            anim.SetInteger("state", 1);
            transform.localScale = new Vector3(Mathf.Sign(dirX), 1, 1);
        }
        else // idle
        {
            anim.SetInteger("state", 0);
        }

        if (rigid.velocity.y > 0.1f)    // jump
        {
            anim.SetInteger("state", 2);
        }
        else if (rigid.velocity.y < -0.1f)  // land
        {
            anim.SetInteger("state", 3);
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        switch (coll.gameObject.tag)
        {
            case "Monster":
                PlayerTakeDemage(20, coll.transform.position);
                break;
            case "Snail":
                // 떨어지는 중이고 snail보다 위에 있을 때 -> 밟을 때
                if (rigid.velocity.y < 0 && transform.position.y > coll.transform.position.y)
                {
                    OnAttack(coll.transform);
                }
                else
                {
                    PlayerTakeDemage(20, coll.transform.position);
                }
                break;
            case "M_Bullet":
                Destroy(coll.gameObject);
                PlayerTakeDemage(20, coll.transform.position);
                SoundManager.Instance.PlayGUISound("Hit", 1.0f);
                break;
            case "Boss":
                PlayerTakeDemage(50, coll.transform.position);
                break;
        }

        if (coll.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            PlayerTakeDemage(20, coll.transform.position);
        }

        if (coll.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            PlayerTakeDemage(20, coll.transform.position);
        }

        if (coll.gameObject.name.Contains("DieZone"))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.name.Contains("Coin"))
        {
            GameMgr.Instance.AddGold();
            Destroy(coll.gameObject);
            SoundManager.Instance.PlayGUISound("coin", 1.0f);
        }
        else if (coll.gameObject.CompareTag("Door"))
        {
            GameMgr.gameState = GameState.Boss;
            PlayerPrefs.SetFloat("Hp", GlobalValue.g_Hp);
            SceneManager.LoadScene("BossScene");
            SceneManager.LoadScene("GameUIScene", LoadSceneMode.Additive);
        }

        else if (coll.gameObject.name.Contains("Wall"))
        {
            coll.isTrigger = true;
        }

        else if (coll.gameObject.name.Contains("Dia"))
        {
            GameMgr.gameState = GameState.Ending;

            Destroy(coll.gameObject);
            SoundManager.Instance.PlayGUISound("coin", 1.0f);
            GameMgr.Instance.GameEnding();
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.name.Contains("Wall"))
        {
            coll.isTrigger = false;
        }
    }

    private void OnAttack(Transform enemy)
    {
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        MonsterController mon = enemy.GetComponent<MonsterController>();
        mon.MonsterTakeDamage();
    }

    private void PlayerTakeDemage(float damage, Vector2 position)
    {
        if (shieldOnTime > 0)  //쉴드 스킬 발동 중일 때.. 데미지 스킬
            return;

        GameMgr.Instance.DamageText(-damage, transform.position, Color.blue);
        hp -= damage;
        GlobalValue.g_Hp -= damage;
        PlayerPrefs.SetFloat("Hp", GlobalValue.g_Hp);

        if (hpBarImg != null)
        {
            hpBarImg.fillAmount = hp / initHp;
        }

        if (hp <= 0)
        {
            hp = 0;
            Die();
        }

        SoundManager.Instance.PlayGUISound("Hit", 1.0f);
        OnDamaged(position);
    }

    private void Die()
    {
        isDie = true;
        anim.SetTrigger("Die");
        GameMgr.Instance.GameOver();
        SoundManager.Instance.m_AudioSrc.clip = null;  //배경음 플레이 안함
    }

    public void UseSkill_Item(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Skill_0:
                Heal(initHp * 0.3f);
                break;
            case SkillType.Skill_1:
                ActivateShield();
                break;
        }

        int skillIndex = (int)skillType;    //SkillType 인덱스로 변환
        GlobalValue.g_skillCount[skillIndex]--;    //스킬카운트 차감
        PlayerPrefs.SetInt($"SkItem_{skillIndex}", GlobalValue.g_skillCount[skillIndex]);
    }

    private void Heal(float amount)
    {
        hp += amount;

        GameMgr.Instance.DamageText(amount, transform.position, new Color(0.18f, 0.5f, 0.34f));
        GlobalValue.g_Hp += hp;
        PlayerPrefs.SetFloat("Hp", GlobalValue.g_Hp);

        if (hp > initHp)
            hp = initHp;

        if (hpBarImg != null)
            hpBarImg.fillAmount = hp / initHp;
    }

    private void ActivateShield()
    {
        if (shieldOnTime > 0)
            return;

        shieldOnTime = shieldDuration;
        GameMgr.Instance.SkillTimeMethod(shieldOnTime, shieldDuration);
    }

    private void SkillUpdate()
    {
        //쉴드 상태 업데이트
        if (0.0f < shieldOnTime)
        {
            shieldOnTime -= Time.deltaTime;
            if (shieldObj != null && shieldObj.activeSelf == false)
            {
                shieldObj.SetActive(true);
            }

            SetLayerCollisions(true);
        }
        else
        {
            if (shieldObj != null && shieldObj.activeSelf == true)
            {
                shieldObj.SetActive(false);
            }

            SetLayerCollisions(false);
        }
    }

    private void SetLayerCollisions(bool ignore)
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Monster"), ignore);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Trap"), ignore);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"), ignore);
    }

    private void OnDamaged(Vector2 targetPos)
    {
        gameObject.layer = 10;
        sprite.color = new Color(1, 1, 1, 0.4f);

        int direction = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(direction, 0.5f) * 5, ForceMode2D.Impulse);
        Invoke("OffDamaged", 1);
    }

    private void OffDamaged()
    {
        gameObject.layer = 6;
        sprite.color = new Color(1, 1, 1, 1);
    }


}
