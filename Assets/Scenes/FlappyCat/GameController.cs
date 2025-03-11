using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    [SerializeField] private int earnedCoins;
    [SerializeField] private GameObject GameOverPanel;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI coinTextGameOver;
    public Button restart;
    public Button Exit;
    private int highScore;
    public AudioSource audioSource;
    public AudioClip scoreSound;
    public AudioClip buttonSound;
    private float timeSinceLastDecay = 0f;
    private float timer = 0f;
    public TMP_Text hungerortiredText;
    public float hunger;
    public float energy;
    public float happiness;
    public float decayRate = 0.1f;
    public GameObject CatIsTiredOrHungryPanel;
    private int salary = 0;
    public bool IsGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        hunger = PetGame.Instance.hunger;
        energy = PetGame.Instance.energy;
        happiness = PetGame.Instance.happiness;
    }

    private void Start()
    {
        // Загрузка рекорда из PlayerPrefs
        highScore = PlayerPrefs.GetInt("BestScore", 0);
        UpdateHighScoreText();
        
    }
    void Update()
    {
        timeSinceLastDecay += Time.deltaTime; // Увеличиваем счётчик времени
        timer += Time.deltaTime;

        if (timeSinceLastDecay >= 1f) // Каждую секунду вызываем DecayStats
        {
            DecayStats();
            timeSinceLastDecay = 0f; // Сбрасываем счётчик
        }
        if (timer >= 10f) 
        {
            HungerTired();
            timer = 0f; // Сбрасываем счётчик
        }
    }
    public void DecayStats()
    {
    hunger -= decayRate;
    hunger = Mathf.Clamp(hunger, 0, 100);

    energy -= decayRate;
    energy = Mathf.Clamp(energy, 0, 100);
    happiness -= decayRate;
    happiness = Mathf.Clamp(happiness, 0, 100);

    // Сохраняем изменения в Singleton
    PlayerPrefs.SetFloat("Hunger", hunger);
    PlayerPrefs.SetFloat("Energy", energy);
    PlayerPrefs.SetFloat("Happiness", happiness);
    PlayerPrefs.Save();
}
public void HungerTired()
    {
        if(hunger < 20 && energy < 20)
        {
            CatIsTiredOrHungryPanel.SetActive(true);
            Debug.Log("Кот проголодался и устал!");
            hungerortiredText.text = "Кот проголодался и устал!";
            PlayerPrefs.Save();
            Invoke(nameof(EndGame), 4f);
        }
        else if(hunger < 20)
        {
            CatIsTiredOrHungryPanel.SetActive(true);
            Debug.Log("Кот проголодался и не хочет больше играть");
            hungerortiredText.text = "Кот проголодался и не хочет больше играть!";
            PlayerPrefs.Save();
            Invoke(nameof(EndGame), 4f);
        }
        else if(energy < 20)
        {
            CatIsTiredOrHungryPanel.SetActive(true);
            Debug.Log("Кот устал и не хочет больше играть");
            hungerortiredText.text += "Кот устал и не хочет больше играть!";
            PlayerPrefs.Save();
            Invoke(nameof(EndGame), 4f);
        }
    }

    public void GameOver()
    {
        IsGameOver = true;
        UpdateScore();
        UpdateHighScoreText();
        GameOverPanel.SetActive(true);
        earnedCoins *=2;
        coinTextGameOver.text = "Заработано монет: " + earnedCoins;
        if(earnedCoins > 0)
        {
            salary = earnedCoins + PlayerPrefs.GetInt("Coins", 50);
            PlayerPrefs.SetInt("Coins", salary);
        }
        
        // Обновляем и сохраняем рекорд, если нужно
        
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CoinCollected()
    {
        if(!IsGameOver)
        {
            earnedCoins += 1;
            coinText.text = "Счет: " + earnedCoins;
        }
        
    }

    public void EndGame()
    {
        
        PlayerPrefs.SetFloat("Hunger", PetGame.Instance.hunger);
        PlayerPrefs.SetFloat("Energy", PetGame.Instance.energy);
        PlayerPrefs.SetFloat("Happiness", PetGame.Instance.happiness);
        if(salary == 0)
        {
            salary = earnedCoins + PlayerPrefs.GetInt("Coins", 50);
            PlayerPrefs.SetInt("Coins", salary);
            UpdateScore();
            UpdateHighScoreText();
        }
        PlayerPrefs.Save(); // ВАЖНО
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }

    private void UpdateScore()
    {
        if (earnedCoins > highScore)
        {
            highScore = earnedCoins;
            PlayerPrefs.SetInt("BestScore", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreText(); // Обновляем текст на UI
        }
    }

    private void UpdateHighScoreText()
    {
        highScoreText.text = "Рекорд: " + highScore;
    }

    public void PlayScoreSound()
    {
        audioSource.clip = scoreSound;
        audioSource.Play();
    }
    public void ButtonSound()
    {
        audioSource.clip = buttonSound;
        audioSource.Play();
    }
}
