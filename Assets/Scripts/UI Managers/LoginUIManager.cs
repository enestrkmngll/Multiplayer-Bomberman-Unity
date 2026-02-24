using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginUIManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject loginPanelObjesi;
    public GameObject mainMenuObjesi;

    [Header("Script Referansý (YENÝ)")]
    public ThemeUIManager themeManagerScript;

    [Header("Inputlar")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI feedbackText;

    private UserRepository userRepo;

    void Start()
    {
        userRepo = new UserRepository();
        if (GameManager.Instance != null && GameManager.Instance.currentUser != null && !string.IsNullOrEmpty(GameManager.Instance.currentUser.Username))
        {
            Debug.Log("Zaten giriþ yapýlmýþ kullanýcý var: " + GameManager.Instance.currentUser.Username);

            loginPanelObjesi.SetActive(false);
            mainMenuObjesi.SetActive(true);

            if (themeManagerScript != null)
            {
                themeManagerScript.SetUserInfo(GameManager.Instance.currentUser);
            }
            return; 
        }
        loginPanelObjesi.SetActive(true);
        mainMenuObjesi.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }

    public void GirisYapButonu()
    {
        string u = usernameInput.text.Trim();
        string p = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p)) return;

        User user = userRepo.Login(u, p);

        if (user != null)
        {
            Debug.Log("Giriþ Baþarýlý: " + user.Username);

            if (GameManager.Instance != null)
                GameManager.Instance.currentUser = user;

            loginPanelObjesi.SetActive(false);
            mainMenuObjesi.SetActive(true);

            if (themeManagerScript != null)
            {
                themeManagerScript.SetUserInfo(user);
                Debug.Log("Veri ThemeUIManager'a gönderildi.");
            }
            else
            {
                Debug.LogError("HATA: Inspector'da 'Theme Manager Script' kutusu BOÞ! GameManager objesini buraya sürükle.");
            }
            // -----------------------------
        }
        else
        {
            UpdateFeedback("Hatalý giriþ.", Color.red);
        }
    }

    public void KayitOlButonu()
    {
        string u = usernameInput.text;
        string p = passwordInput.text;
        bool basarili = userRepo.Register(u, p);
        if (basarili) UpdateFeedback("Kayýt Baþarýlý", Color.green);
        else UpdateFeedback("Hata", Color.red);
    }

    void UpdateFeedback(string msg, Color color)
    {
        if (feedbackText != null) { feedbackText.text = msg; feedbackText.color = color; }
    }
}