using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MiniGame2048 : MonoBehaviour
{

    public TMP_Text coinText;
    public TMP_Text[] tileTexts;
    private int[,] grid = new int[4, 4];
    private int coinsEarned = 0;
    public AudioSource audioSource; // Источник аудио
    public AudioClip track1; // Первый трек
    public AudioClip track2; // Второй трек
    public Button playTrack1Button; // Кнопка для первого трека
    public Button playTrack2Button; // Кнопка для второго трека
    public Slider volumeSlider; // Слайдер громкости
    public Button muteButton; // Кнопка для выключения музыки
    private float timeSinceLastDecay = 0f;
    private float timer = 0f;
    public TMP_Text hungerortiredText;
    public float hunger;
    public float energy;
    public float happiness;
    public float decayRate = 0.1f;
    public GameObject CatIsTiredOrHungryPanel;
    public TMP_Text salaryText;

    
    void Awake()
    {
        hunger = PetGame.Instance.hunger;
        energy = PetGame.Instance.energy;
        happiness = PetGame.Instance.happiness;
    }
    void Start()
    {
        InitializeGrid();
        SpawnNumber();
        SpawnNumber();
        UpdateUI();
        playTrack1Button.onClick.AddListener(PlayTrack1);
        playTrack2Button.onClick.AddListener(PlayTrack2);
        volumeSlider.onValueChanged.AddListener(SetVolume);
        muteButton.onClick.AddListener(MuteMusic);
        volumeSlider.value = audioSource.volume;
    }
    void PlayTrack1()
    {
        audioSource.clip = track1;
        audioSource.Play();
    }
    void PlayTrack2()
    {
        audioSource.clip = track2;
        audioSource.Play();
    }
    // Настроить громкость
    void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
    // Выключить музыку
    void MuteMusic()
    {
        audioSource.Pause(); // Остановить музыку
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) MakeMove(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.S)) MakeMove(Vector2Int.right);
        if (Input.GetKeyDown(KeyCode.A)) MakeMove(Vector2Int.down);
        if (Input.GetKeyDown(KeyCode.D)) MakeMove(Vector2Int.up);

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

    void MakeMove(Vector2Int direction)
    {
        bool moved = SlideAndMerge(direction);
        if (moved)
        {
            coinsEarned += 1;
            SpawnNumber();
            UpdateUI();

        if (IsGameOver())
        {
            EndGame();  // Завершаем игру, если нет доступных ходов
        }

        }
    }
    bool IsGameOver()
{
    // Проверяем, есть ли пустые клетки
    for (int x = 0; x < 4; x++)
    {
        for (int y = 0; y < 4; y++)
        {
            if (grid[x, y] == 0)
                return false; // Если есть пустая клетка, игра не закончена
        }
    }

    // Проверяем, есть ли возможные слияния
    for (int x = 0; x < 4; x++)
    {
        for (int y = 0; y < 4; y++)
        {
            if (x < 3 && grid[x, y] == grid[x + 1, y]) return false; // Слияние по вертикали
            if (y < 3 && grid[x, y] == grid[x, y + 1]) return false; // Слияние по горизонтали
        }
    }

    return true; // Если нет пустых клеток и нет возможных слияний, игра закончена
}

    void InitializeGrid()
    {
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                grid[x, y] = 0;
    }

    void SpawnNumber()
    {
        var emptyCells = new List<Vector2Int>();
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                if (grid[x, y] == 0)
                    emptyCells.Add(new Vector2Int(x, y));

        if (emptyCells.Count > 0)
        {
            var cell = emptyCells[Random.Range(0, emptyCells.Count)];
            grid[cell.x, cell.y] = Random.value < 0.9f ? 2 : 4;
        }
    }

    bool SlideAndMerge(Vector2Int direction)
    {
        bool moved = false;

        for (int i = 0; i < 4; i++)
        {
            int[] line = new int[4];
            for (int j = 0; j < 4; j++)
            {
                int x = direction.x == 0 ? i : direction.x > 0 ? 3 - j : j;
                int y = direction.y == 0 ? i : direction.y > 0 ? 3 - j : j;
                line[j] = grid[x, y];
            }

            int[] mergedLine = MergeLine(line);

            for (int j = 0; j < 4; j++)
            {
                int x = direction.x == 0 ? i : direction.x > 0 ? 3 - j : j;
                int y = direction.y == 0 ? i : direction.y > 0 ? 3 - j : j;
                if (grid[x, y] != mergedLine[j]) moved = true;
                grid[x, y] = mergedLine[j];
            }
        }

        return moved;
    }

    int[] MergeLine(int[] line)
    {
        List<int> merged = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            if (line[i] == 0) continue;
            if (merged.Count > 0 && merged[merged.Count - 1] == line[i])
            {
                merged[merged.Count - 1] *= 2;
                
                merged.Add(0);
            }
            else
            {
                merged.Add(line[i]);
            }
        }

        while (merged.Count < 4) merged.Add(0);
        return merged.ToArray();
    }

    void UpdateUI()
    {
        coinText.text = "Ходов: " + coinsEarned;

        // Обновление отображения сетки
        int index = 0;  // Индекс для массива tileTexts
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                tileTexts[index].text = grid[x, y] != 0 ? grid[x, y].ToString() : "";
                index++;
            }
        }
    }

    public void EndGame()
    {   
        PlayerPrefs.SetFloat("Hunger", PetGame.Instance.hunger);
        PlayerPrefs.SetFloat("Energy", PetGame.Instance.energy);
        PlayerPrefs.SetFloat("Happiness", PetGame.Instance.happiness);
        if(coinsEarned > 0)
        {
            coinsEarned /=2;
            salaryText.text = "Вы заработали: " + coinsEarned + " монет!";
            coinsEarned +=PlayerPrefs.GetInt("Coins", 50);
            PlayerPrefs.SetInt("Coins", coinsEarned);
        }
        PlayerPrefs.Save(); // ВАЖНО
        Invoke("SceneLoad", 3f);
    }
    public void SceneLoad()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void HungerTired()
    {
        if(hunger < 10 && energy < 10)
        {
            CatIsTiredOrHungryPanel.SetActive(true);
            Debug.Log("Кот проголодался и устал!");
            hungerortiredText.text = "Кот проголодался и устал!";
            PlayerPrefs.Save();
            Invoke(nameof(EndGame), 4f);
        }
        else if(hunger < 10)
        {
            CatIsTiredOrHungryPanel.SetActive(true);
            Debug.Log("Кот проголодался и не хочет больше играть");
            hungerortiredText.text = "Кот проголодался и не хочет больше играть!";
            PlayerPrefs.Save();
            Invoke(nameof(EndGame), 4f);
        }
        else if(energy < 10)
        {
            CatIsTiredOrHungryPanel.SetActive(true);
            Debug.Log("Кот устал и не хочет больше играть");
            hungerortiredText.text = "Кот устал и не хочет больше играть!";
            PlayerPrefs.Save();
            Invoke(nameof(EndGame), 4f);
        }
    }
}