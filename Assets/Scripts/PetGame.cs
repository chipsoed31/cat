using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;

public class PetGame : MonoBehaviour
{
    public static PetGame Instance;
    public float hunger;
    public float energy;
    public float happiness;
    public int coins;
    public float decayRate = 0.1f;
    private bool isResting = false;
    private float timeSinceLastDecay = 0f;

    // UI Elements
    public Slider hungerSlider;
    public Slider energySlider;
    public Slider happinessSlider;
    public Button feedButton;
    public Button playButton;
    public Button sleepButton;
    public Button shopButton;
    public TMP_Text coinText;
    public TMP_Text coinShopText;
    public GameObject gameListPanel;
    public Button[] gameButtons; // Кнопки мини-игр

    // Shop and Inventory
    public GameObject inventoryPanel;
    public GameObject shopPanel;
    public GameObject productButtonPrefab; // Prefab кнопки продукта
    public Transform inventoryContent;
    public Transform shopContent;

    // Player currency
    

    // Pet Interaction
    public AudioSource petAudioSource;
    public AudioSource backgroundMusicSource;
    public AudioClip[] petSounds;
    public AudioClip eatSound;
    public AudioClip purchaseSound;
    public AudioClip ButtonSound;
    public AudioClip snoringSound;
    public AudioClip backgroundMusic;
    public Animator petAnimator;

    // Inventory and Shop
    private List<Product> inventory = new List<Product>();
    private List<Product> shopProducts = new List<Product>();

    // Settings UI
    public GameObject settingsPanel;  // Панель настроек
    public Slider musicVolumeSlider;  // Слайдер для музыки
    public float musicVolumeValue;
    public Slider masterVolumeSlider; // Слайдер для общей громкости
    public Button exitButton; // Кнопка выхода из игры
    public Button resetSaves;
    public GameObject SpeechBubble;
    public TMP_Text SpeechText;
    public GameObject SpeechEmoteBubble;
    public TMP_Text SpeechEmoteText;
    private float timer = 0;
    public bool IsSpeechBubble = false;
    public bool IsEmoteBubble = false;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        musicVolumeValue = PlayerPrefs.GetFloat("musicVolumeValue", 100);
        hunger = PlayerPrefs.GetFloat("Hunger", 100);
        energy = PlayerPrefs.GetFloat("Energy", 100);
        happiness = PlayerPrefs.GetFloat("Happiness", 100);
        coins = PlayerPrefs.GetInt("Coins", 50);
        LoadInventory(); // Загрузка инвентаря
    }
    void Start()
{
    resetSaves.onClick.AddListener(() => {
        Debug.Log("Кнопка сброса нажата!");
        ResetSaves();
    });
    
    // Настройка фоновой музыки
    if (backgroundMusic != null)
    {
        backgroundMusicSource.clip = backgroundMusic;
        backgroundMusicSource.loop = true;  // Зацикливаем музыку
        backgroundMusicSource.Play();  // Запускаем воспроизведение
    }

    shopProducts.Add(new Product("Салат", 25, 15));
    shopProducts.Add(new Product("Яблоко", 35, 10));
    shopProducts.Add(new Product("Мясо", 40, 15));
    shopProducts.Add(new Product("Пицца", 55, 30));
    shopProducts.Add(new Product("Рамен", 60, 35));
    shopProducts.Add(new Product("Суши", 65, 40));
    shopProducts.Add(new Product("Бургер", 70, 45));
    shopProducts.Add(new Product("Креветки", 120, 55));
    
    
    UpdateUI();

    // Устанавливаем начальные значения громкости
        musicVolumeSlider.value = musicVolumeValue;
        /* musicVolumeSlider.value = backgroundMusicSource.volume; */
        backgroundMusicSource.volume = musicVolumeSlider.value;
        masterVolumeSlider.value = AudioListener.volume;

        // Слушатели для слайдеров громкости
        musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
        masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);

        // Кнопка выхода
        exitButton.onClick.AddListener(ExitGame);
}

void UpdateUI()
{
    hungerSlider.value = hunger;
    energySlider.value = energy;
    happinessSlider.value = happiness;
    coinText.text = "Coins: " + coins;
}
void UpdateMusicVolume(float value)
    {
        backgroundMusicSource.volume = value;  // Обновляем громкость музыки
    }

    void UpdateMasterVolume(float value)
    {
        AudioListener.volume = value;  // Обновляем общую громкость
    }
    private void SaveInventory()
{
    string json = JsonUtility.ToJson(new ProductListWrapper { products = inventory });
    PlayerPrefs.SetString("Inventory", json);
    PlayerPrefs.Save();
}

public void LoadInventory()
{
    string json = PlayerPrefs.GetString("Inventory", string.Empty);
    if (!string.IsNullOrEmpty(json))
    {
        ProductListWrapper wrapper = JsonUtility.FromJson<ProductListWrapper>(json);
        inventory = wrapper.products ?? new List<Product>();
    }
}

// Класс для обертки списка продуктов (для сериализации/десериализации JSON)
[System.Serializable]
private class ProductListWrapper
{
    public List<Product> products;
}

    // Функция выхода из игры
    public void ExitGame()
    {
        PlayerPrefs.Save();
        // Выйти из игры
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Для режима редактора
        #else
        Application.Quit(); // Для сборки игры
        #endif
    }

    public void OpenSettings()
    {   
        ButtonClickSound();
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        ButtonClickSound();
        settingsPanel.SetActive(false);
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
    if(timer >15f)
    {
        HungryTiredOrBored();
        timer = 0;
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
    PlayerPrefs.SetInt("Coins", coins);
    PlayerPrefs.SetFloat("musicVolumeValue", backgroundMusicSource.volume);
    PlayerPrefs.Save();

    UpdateUI();
}
void ButtonClickSound()
{
    petAudioSource.clip = ButtonSound;
    petAudioSource.Play();
}
    // Взаимодействие с питомцем
    public void FeedPet()
    {
        inventoryPanel.SetActive(true);
        ButtonClickSound();
        PopulateInventory();
    }

    public void RestPet()
    {
        ButtonClickSound();
        if (isResting)
        {
            StopCoroutine(RestoreEnergy());
            petAnimator.ResetTrigger("Sleep");
            petAudioSource.loop = false;
            petAudioSource.Stop(); // Останавливаем храп, если питомец проснулся
            isResting = false;
            SpeechBubble.SetActive(false);
        }
        else if (energy > 89 && !isResting)
        {
            SpeechText.text = "Я не хочу спать!";
            SpeechBubble.SetActive(true);
            Invoke("SpeechBubbleRemove", 3f);
        }
        else
        {
            SpeechText.text = "Z z z......";
            SpeechBubble.SetActive(true);
            petAnimator.SetTrigger("Sleep");
            petAudioSource.clip = snoringSound;
            petAudioSource.loop = true;  // Сделать храп зацикленным
            petAudioSource.Play();  // Начать проигрывание храпа
            StartCoroutine(RestoreEnergy());
            isResting = true;
        }
    }

    IEnumerator RestoreEnergy()
    {
        while (energy < 100f)
        {
            energy += 5f;
            energy = Mathf.Clamp(energy, 0, 100);
            UpdateUI();
            yield return new WaitForSeconds(1f);

            if (energy > 99f || !isResting)
            {
                SpeechBubble.SetActive(false);
                petAnimator.ResetTrigger("Sleep");
                isResting = false;
                petAudioSource.loop = false;
                petAudioSource.Stop();
                break;
            }
        }
    }

    public void PlayWithPet()
    {
        ButtonClickSound();
        if (energy >= 19f)
        {
            gameListPanel.SetActive(true);
            for (int i = 0; i < gameButtons.Length; i++)
            {
                int index = i;
                gameButtons[i].onClick.AddListener(() => StartMiniGame(index));
            }
        }
        else
        {
            gameListPanel.SetActive(false);
            SpeechText.text = "Я устал...";
            SpeechBubble.SetActive(true);
            Invoke("SpeechBubbleRemove", 3f);
        }
        UpdateUI();
    }

    void StartMiniGame(int gameIndex)
    {
        
        OpenMinigame("MiniGameScene" + gameIndex);
    }

    public void CloseGameList()
    {
        ButtonClickSound();
        gameListPanel.SetActive(false);
    }

    public void InteractWithPet()
    {
        int soundIndex = Random.Range(0, petSounds.Length);
        petAudioSource.clip = petSounds[soundIndex];
        petAudioSource.Play();
        int animaIndex = Random.Range(0, 2);
        if(animaIndex==1)
        {
            petAnimator.SetTrigger("Washing");
            StartCoroutine(WaitAndRestoreAnimation(6f));
        }
        else {petAnimator.SetTrigger("Stretch");
            StartCoroutine(WaitAndRestoreAnimation(2.7f));}
        energy -= 1f;
        energy = Mathf.Clamp(energy, 0, 100);
    }
    IEnumerator WaitAndRestoreAnimation(float duration)
{
    // Ждем указанное время
    yield return new WaitForSeconds(duration);

    // После завершения анимации переключаемся на стандартную анимацию
    petAnimator.ResetTrigger("Washing");
    petAnimator.ResetTrigger("Stretch"); // Используй название триггера для стандартной анимации
}

    public void OpenMinigame(string minigameScene)
    {
        ButtonClickSound();
        happiness += 10f;
        happiness = Mathf.Clamp(happiness, 0, 100);

        energy -= 8f;
        energy = Mathf.Clamp(energy, 0, 100);
            
        hunger -= 5f;
        hunger = Mathf.Clamp(hunger, 0, 100);
        PlayerPrefs.Save();
        // Сохраняем текущее состояние
        PlayerPrefs.SetFloat("Hunger", hunger);
        PlayerPrefs.SetFloat("Energy", energy);
        PlayerPrefs.SetFloat("Happiness", happiness);
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();
        backgroundMusicSource.Pause();
        SceneManager.LoadScene(minigameScene);
    }

    // Shop functions
    public void OpenShop()
    {
        ButtonClickSound();
        shopPanel.SetActive(true);
        PopulateShop();
        
    }

    public void CloseShop()
    {
        ButtonClickSound();
        shopPanel.SetActive(false);
    }

    private void PopulateShop()
    {
        coinShopText.text = "Coins: " + coins;
        foreach (Transform child in shopContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Product product in shopProducts)
        {
            GameObject button = Instantiate(productButtonPrefab, shopContent);
            button.GetComponentInChildren<TMP_Text>().text = $"{product.name} - {product.price} монет, Сытость: {product.satiety}";
            Product currentProduct = product;
            button.GetComponent<Button>().onClick.AddListener(() => BuyProduct(currentProduct));
        }
    }

    private void BuyProduct(Product product)
    {
        if (coins >= product.price)
        {
            coins -= product.price;
            
            Product existingProduct = inventory.Find(p => p.name == product.name);
            if (existingProduct != null)
            {
                existingProduct.amount++;
            }
            else
            {
                inventory.Add(new Product(product.name, product.price, product.satiety, 1));
            }
            petAudioSource.clip = purchaseSound;
            petAudioSource.Play();
            SaveInventory();
            UpdateUI();
            PopulateShop();
        }
    }
    
    // Inventory functions
    private void PopulateInventory()
    {
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Product product in inventory)
        {
            GameObject button = Instantiate(productButtonPrefab, inventoryContent);
            button.GetComponentInChildren<TMP_Text>().text = $"{product.name} (x{product.amount}) - Сытость: {product.satiety}";
            Product currentProduct = product;
            button.GetComponent<Button>().onClick.AddListener(() => UseProduct(currentProduct));
        }
    }

    private void UseProduct(Product product)
    {
        if (product.amount > 0)
        {
            if(hunger < 91)
            {
                hunger += product.satiety;
                hunger = Mathf.Clamp(hunger, 0, 100);
                product.amount--;

                if (product.amount == 0)
                {
                    inventory.Remove(product);
                }
                petAudioSource.clip = eatSound;
                petAudioSource.Play();
                SaveInventory();
                UpdateUI();
                PopulateInventory();
            }
            else
            {
                inventoryPanel.SetActive(false);
                SpeechText.text = "Я не хочу есть!";
                SpeechBubble.SetActive(true);
                Invoke("SpeechBubbleRemove", 3f);
            }
        }
    }


    // Close Inventory
    public void CloseInventory()
    {
        ButtonClickSound();
        inventoryPanel.SetActive(false);
    }
    public void ResetSaves()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        UpdateUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Перезапуск сцены
    }
    public void SpeechBubbleRemove()
    {
        SpeechBubble.SetActive(false);
        SpeechEmoteBubble.SetActive(false);
        IsEmoteBubble = false;
        IsSpeechBubble = false;
    }
    public void HungryTiredOrBored()
    {
        
        
        if(hunger < 15 && !isResting && IsEmoteBubble == false)
        {
            IsSpeechBubble = true;
            shopPanel.SetActive(false);
            inventoryPanel.SetActive(false);
            settingsPanel.SetActive(false);
            gameListPanel.SetActive(false);
            SpeechText.text = "Я хочу есть!";
            SpeechBubble.SetActive(true);
            Invoke("SpeechBubbleRemove", 3f);
        }
        if(energy < 15 && !isResting && IsEmoteBubble == false)
        {
            IsSpeechBubble = true;
            shopPanel.SetActive(false);
            inventoryPanel.SetActive(false);
            settingsPanel.SetActive(false);
            gameListPanel.SetActive(false);
            SpeechText.text = "Я хочу спать!";
            SpeechBubble.SetActive(true);
            Invoke("SpeechBubbleRemove", 3f);
        }
        if(happiness < 15 && !isResting && IsEmoteBubble == false)
        {
            IsSpeechBubble = true;
            shopPanel.SetActive(false);
            inventoryPanel.SetActive(false);
            settingsPanel.SetActive(false);
            gameListPanel.SetActive(false);
            SpeechText.text = "Мне скучно...";
            SpeechBubble.SetActive(true);
            Invoke("SpeechBubbleRemove", 3f);
        }

        if(hunger>92 || energy >92 && !isResting && IsSpeechBubble == false)
        {
            IsEmoteBubble = true;
            SpeechEmoteText.text = ":)";
            SpeechEmoteBubble.SetActive(true);
            Invoke("SpeechBubbleRemove", 3f);
        }
        if(happiness>92 && !isResting && IsSpeechBubble == false)
        {
            IsEmoteBubble = true;
            SpeechEmoteText.text = ":D";
            SpeechEmoteBubble.SetActive(true);
            Invoke("SpeechBubbleRemove", 3f);
        }
    }
    
}
    

// Product class for inventory and shop
[System.Serializable]
public class Product
{
    public string name;
    public int price;
    public int satiety;
    public int amount;

    public Product(string name, int price, int satiety, int amount = 1)
    {
        this.name = name;
        this.price = price;
        this.satiety = satiety;
        this.amount = amount;
    }
    
}
