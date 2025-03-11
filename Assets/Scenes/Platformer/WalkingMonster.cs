using UnityEngine;

public class WalkingMonster : Entity
{
    private float speed = 1f;
    private int lives = 3;
    private Vector3 dir;
    private SpriteRenderer sprite;
    private Animator anim;
    private Rigidbody2D rb;
    private bool isDead = false;

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        dir = transform.right;
    }
    private void Move()
{
    // Проверка на столкновение с чем-то (с использованием метода для проверки перед движением)
     Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.1f + transform.right * dir.x * 0.7f, 0.1f);
        if (colliders.Length > 0) dir *= -1;

        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, Time.deltaTime);
        sprite.flipX = dir.x > 0.0f;
        Debug.Log("Changing direction, new dir: " + dir);
}


     private void UpdateAnimation()
    {
        // Если монстр мертв, включаем анимацию смерти
        if (isDead)
        {
            anim.SetBool("IsDead", true);
            anim.SetBool("IsWalking", false);
            return;
        }

        // Анимация ходьбы
        anim.SetBool("IsWalking", dir != Vector3.zero);
    }
    private void Update()
    {
        Move();
        UpdateAnimation();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage();
            lives--;
        }
        if(lives<1)
        {
            Die();
        }
    }
    public override void Die()
    {
        isDead = true;
        anim.SetBool("IsDead", true);

        // Удаляем объект после завершения анимации
        Destroy(gameObject, 1.5f); // Подождем 1.5 секунды для завершения анимации
    }
}
