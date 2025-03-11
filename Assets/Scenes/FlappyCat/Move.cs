using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private int speed = 1;
    private float timer = 0f;

    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
    private void FixedUpdate()
    {
        if(timer>10)
        {
            speed++;
            timer = 0;
        }
        timer += Time.deltaTime;
    }
}
