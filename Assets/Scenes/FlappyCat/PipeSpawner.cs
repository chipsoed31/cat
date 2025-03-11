using Unity.VisualScripting;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] private float maxtime = 7f;
    [SerializeField] private float heightRange = 0.45f;
    [SerializeField] private GameObject _pipe;
    private float timer = 0f;
    private void Start()
    {
        SpawnPipe();
    }
    private void Update()
    {
        if(timer > maxtime)
        {
            SpawnPipe();
            timer = 0;
        }
        timer += Time.deltaTime;
    }
    private void SpawnPipe()
    {
        Vector3 spawnPos = transform.position + new Vector3(0, Random.Range(-heightRange, heightRange));
        GameObject pipe = Instantiate(_pipe, spawnPos, Quaternion.identity);

        Destroy(pipe, 10f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameOver();
        }
    }
    private void GameOver(){}
}
