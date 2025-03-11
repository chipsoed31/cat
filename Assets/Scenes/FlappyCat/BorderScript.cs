using UnityEngine;

public class BorderScript : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && GameController.Instance.IsGameOver == false)
        {
            GameController.Instance.CoinCollected();
            GameController.Instance.PlayScoreSound();
        }
    }
}
