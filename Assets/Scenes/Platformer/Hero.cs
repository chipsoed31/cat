using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Entity
{
    [SerializeField] private float speed = 3f; // скорость движения
    [SerializeField] private int lives = 5; // скорость движения
    [SerializeField] private float jumpForce = 6f; // сила прыжка
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private GameObject attackHitboxPrefab;
    private bool isGrounded = false;


    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    public AudioSource audioSource;
    public static Hero Instance {get; set;}

    private States State
    {
        get {return (States)anim.GetInteger("state");}
        set {anim.SetInteger("state", (int)value);}
    }
    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponentInChildren<AudioSource>();
        
        
    }
    
    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        if (isGrounded) State = States.idle;
        if (Input.GetButton("Horizontal"))
            Run();
        if (isGrounded && Input.GetButtonDown("Jump"))
            Jump();
        if (Input.GetButtonDown("Fire1"))
            Attack();
    }

    private void Run()
{
    if (isGrounded) State = States.run;

    float move = Input.GetAxis("Horizontal");
    rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
    sprite.flipX = move < 0.0f;

    if (!audioSource.isPlaying)
        {
            audioSource.clip = walkSound;
            audioSource.Play();
        }
}

    private void Jump()
{
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    audioSource.PlayOneShot(jumpSound);
}
     private void Attack()
    {
        State = States.attack;
        anim.SetTrigger("attack");
        audioSource.PlayOneShot(attackSound);

        // Создаем хитбокс для атаки
        Vector3 attackPosition = transform.position + new Vector3(sprite.flipX ? -0.5f : 0.5f, 0, 0);
        Instantiate(attackHitboxPrefab, attackPosition, Quaternion.identity);
    }


    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrounded = collider.Length > 1;
        if (!isGrounded) State = States.jump;
    }
     public override void GetDamage()
    {
        lives -= 1;
        audioSource.PlayOneShot(damageSound);
        Debug.Log("Жизней у игрока: " + lives);
        if (lives < 1) Die();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            audioSource.PlayOneShot(coinSound);
            Destroy(collision.gameObject);
            Debug.Log("Монета собрана!");
        }
    }
    public enum States
    {
        idle,
        run,
        jump,
        attack
    }
}