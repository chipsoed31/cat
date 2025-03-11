using UnityEngine;

public class FlappyCat : MonoBehaviour
{
    [SerializeField] private int jumpForce = 6;
    [SerializeField] private float rotationSpeed = 10f;
    

    private Rigidbody2D rb;
    private Animator anim;
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip hitSound;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetInteger("state", 3);
    }
    
    private void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            Jump();
            audioSource.clip = jumpSound;
            audioSource.Play();
        }
    }
    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, rb.linearVelocity.y * rotationSpeed);
    }
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") && GameController.Instance.IsGameOver == false)
        {audioSource.clip = hitSound;
        audioSource.Play();
        GameController.Instance.GameOver();}
        
    }
}
