using UnityEngine;

public class Obstacle : Entity
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Hero.Instance != null && collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage();
        }
    }

    
}
