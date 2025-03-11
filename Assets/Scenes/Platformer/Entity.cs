using UnityEngine;

public class Entity : MonoBehaviour
{
    public virtual void GetDamage()
    {
        Debug.Log("Получен урон");
    }

    public virtual void Die()
    {
        Destroy(this.gameObject);
    }
}
