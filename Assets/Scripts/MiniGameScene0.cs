using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MiniGameScene0 : MonoBehaviour
{
    public TMP_Text timerText;  // Таймер
    public TMP_Text resultText;  // Результат
    public Button clickButton;  // Кнопка кликов
    public Button closeButton;

    private int clickCount = 0;
    private float gameTime = 10f;  // Время игры
    private bool isGameActive = true;

    void Start()
    {
        clickButton.onClick.AddListener(OnClickButton);
        closeButton.onClick.AddListener(EndGame);
          // Скрыть результат
        StartCoroutine(GameTimer());
    }
    void OnClickButton()
    {
        if (isGameActive)
        {
            clickCount++;
            resultText.text = "Твои клики: " + clickCount;
        }
    }

    IEnumerator GameTimer()
    {
        while (gameTime > 0)
        {
            gameTime -= Time.deltaTime;
            timerText.text = "Время: " + Mathf.Ceil(gameTime).ToString();
            yield return null;
        }

        EndGame();
    }

    void EndGame()
    {
        StopCoroutine(GameTimer());
        isGameActive = false;
        clickButton.interactable = false;

        // Показываем результат
        
        resultText.text = "Твои клики: " + clickCount;

        // Начисляем награду
        if (clickCount > 0)
        {
            clickCount /=2;
            resultText.text += $"\nТы получил {clickCount} монет!";
            clickCount += PlayerPrefs.GetInt("Coins", 50);
            PlayerPrefs.SetInt("Coins", clickCount);
        } 
        // Возвращаемся в основную сцену через 3 секунды
        Invoke("ReturnToMainScene", 3f);
    }

    void ReturnToMainScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
