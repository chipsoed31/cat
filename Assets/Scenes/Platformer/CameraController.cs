using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Vector3 pos;
    
    private void Awake()
    {
        if(!player)
        {
            player = FindAnyObjectByType<Hero>().transform;
        }
    }
    private void Update()
    {
        pos = player.position;
        pos.z = -10f;
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime);
        
    }
}
