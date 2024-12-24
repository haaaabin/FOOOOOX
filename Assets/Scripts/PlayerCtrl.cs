using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MovementState
{
    idle,
    running,
    jumping,
    falling
}

public class PlayerCtrl : MonoBehaviour
{
    public static PlayerCtrl Instance { get; private set; }

    public MovementState state;
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D coll;
    private SpriteRenderer sprite;
    private float dirX = 0.0f;
    private float moveSpeed = 5f;
    private float jumpPower = 15.0f;
    private float inputX = 0f;
    private bool isJumping = false;

    public GameObject bullet;
    public GameObject shootPos;
    private float bulletSpeed = 10.0f;
    [HideInInspector] public int maxHp = 5;
    [HideInInspector] public int currentHp = 5;
    [HideInInspector] public int score = 0;

    private LayerMask groundMask = -1;
    private bool isDie = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        groundMask = LayerMask.GetMask("Platform", "AirPlatform");

        Time.timeScale = 1;
        InIt();
    }

    private void Update()
    {
        if (isDie)
            return;

        MoveUpdateAnim();
        // HandleInput();
        // UpdateAnimState();
        LimitMove();
        InGameUI.instance.UpdateHeart();
    }

    public void InIt()
    {
        if (SceneManager.GetActiveScene().name == "BossScene")           
            transform.position = new Vector2(0, -3);
        else
            transform.position = Vector2.zero;

        isDie = false;
        currentHp = maxHp;
        score = 0;

        state = MovementState.idle;
        anim.ResetTrigger("Die");
        anim.SetInteger("state", 0);
        anim.Play("Player-Idle");
    }

    private void MoveUpdateAnim()
    {
        transform.Translate(inputX * Time.deltaTime * moveSpeed, 0, 0);

        if (isJumping)
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isJumping = false;
        }

        if (inputX != 0)  // walk
        {
            anim.SetInteger("state", 1);
            transform.localScale = new Vector3(Mathf.Sign(inputX), 1, 1);
        }
        else // idle
        {
            anim.SetInteger("state", 0);
        }

        if (rb.velocity.y > 0.1f)    // jump
        {
            anim.SetInteger("state", 2);
        }
        else if (rb.velocity.y < -0.1f)  // land
        {
            anim.SetInteger("state", 3);
        }
    }

    public void MoveLeft()
    {
        inputX = -1f;
    }

    public void MoveRight()
    {
        inputX = 1f;
    }

    public void StopMove()
    {
        inputX = 0f;
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            isJumping = true;
        }
    }

    public void Fire()
    {
        ShootBullet();
    }

    private void HandleInput()
    {
        dirX = Input.GetAxis("Horizontal");
        transform.Translate(dirX * Time.deltaTime * moveSpeed, 0, 0);

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ShootBullet();
        }
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

        if (rb.velocity.y > 0.1f)    // jump
        {
            anim.SetInteger("state", 2);
        }
        else if (rb.velocity.y < -0.1f)  // land
        {
            anim.SetInteger("state", 3);
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
        if (transform.position.x <= -10f)
        {
            transform.position = new Vector2(-10f, transform.position.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.5f, groundMask);
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        switch (coll.gameObject.tag)
        {
            case "Monster":
                TakeDamage(1, coll.transform.position);
                break;
            case "Snail":
                // 떨어지는 중이고 snail보다 위에 있을 때 -> 밟을 때
                if (rb.velocity.y < 0 && transform.position.y > coll.transform.position.y)
                {
                    OnAttack(coll.transform);
                }
                else
                {
                    TakeDamage(1, coll.transform.position);
                }
                break;
            case "M_Bullet":
                Destroy(coll.gameObject);
                TakeDamage(1, coll.transform.position);
                SoundManager.Instance.PlayGUISound("Hit", 1.0f);
                break;
            case "Boss":
                TakeDamage(1, coll.transform.position);
                break;
        }

        if (coll.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            TakeDamage(1, coll.transform.position);
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
            AddScore();
            SoundManager.Instance.PlayGUISound("coin", 1.0f);
            Destroy(coll.gameObject);
        }
        else if (coll.gameObject.CompareTag("Door"))
        {
            InGameUI.instance.ChangeScene();
        }

        else if (coll.gameObject.name.Contains("Wall"))
        {
            coll.isTrigger = true;
        }

        else if (coll.gameObject.name.Contains("Dia"))
        {
            SoundManager.Instance.PlayGUISound("coin", 1.0f);
            GameManager.Instance().GameOver();
            Destroy(coll.gameObject);
        }
        else if (coll.gameObject.name.Contains("Heart"))
        {
            if (currentHp < 5)
            {
                currentHp += 1;
            }
            InGameUI.instance.UpdateHeart();
            Destroy(coll.gameObject);
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
        rb.AddForce(Vector2.up * 15, ForceMode2D.Impulse);
        MonsterController mon = enemy.GetComponent<MonsterController>();
        mon.MonsterTakeDamage();
    }

    public void TakeDamage(int damage, Vector2 position)
    {
        currentHp -= damage;
        InGameUI.instance.UpdateHeart();
        SoundManager.Instance.PlayGUISound("Hit", 1.0f);
        InGameUI.instance.DamageText(-damage, transform.position, Color.blue);

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        // 넉백 효과
        gameObject.layer = 10;
        sprite.color = new Color(1, 1, 1, 0.4f);

        int direction = transform.position.x - position.x > 0 ? 1 : -1;
        rb.AddForce(new Vector2(direction, 0.5f) * 5, ForceMode2D.Impulse);
        Invoke("OffDamaged", 1);
    }

    private void Die()
    {
        rb.velocity = Vector2.zero;
        isDie = true;
        anim.SetTrigger("Die");
        GameManager.Instance().GameOver();
        SoundManager.Instance.m_AudioSrc.clip = null;  //배경음 플레이 안함
    }

    public void AddScore(int value = 50)
    {
        if (score < 0)
            score = 0;

        score += value;
        InGameUI.instance.scoreText.text = score.ToString();
    }

    private void SetLayerCollisions(bool ignore)
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Monster"), ignore);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Trap"), ignore);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"), ignore);
    }

    private void OffDamaged()
    {
        gameObject.layer = 6;
        sprite.color = new Color(1, 1, 1, 1);
    }
}
