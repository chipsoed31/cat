using UnityEngine;

public class Worm : Entity
{
    [SerializeField] private int lives = 3;
    private Animator anim;
    private bool IsDead = false;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D collision)

    {
        if(collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage();
            lives--;
            Debug.Log("Worm lives: " + lives);
        }
        if(lives < 1)
        {
            Die();
        }
    }
    public override void Die()
    {
        IsDead = true;
        anim.SetBool("IsDead", true);
        Destroy(gameObject, 1.5f);
    }

}
