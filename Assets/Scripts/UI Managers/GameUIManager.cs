using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("UI Elemanlarý")]
    public GameObject gameOverPanel; 
    public TextMeshProUGUI resultText; 

    private void Awake()
    {
        Instance = this;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void ShowGameOver(string message, Color color)
    {
        Debug.Log("GameUIManager: Oyun Sonu Paneli Açýlýyor...");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            gameOverPanel.transform.SetAsLastSibling();

            if (resultText != null)
            {
                resultText.text = message;
                resultText.color = color;
            }
        }
        else
        {
            Debug.LogError("HATA: GameUIManager inspector'ýnda 'GameOverPanel' kutusu boþ!");
        }
    }


    public void AnaMenuyeDon()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}