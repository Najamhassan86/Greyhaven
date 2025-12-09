using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Highlight Images")]
    public Image girlHighlight;
    public Image boyHighlight;

    [Header("Next Scene")]
    public string nextSceneName = "Map_Hosp1";

    private string currentChoice = "Girl";   // default

    void Start()
    {
        ShowGirl();   // start with girl highlighted
    }

    // ---------- HOVER ----------
    public void OnHoverGirl()
    {
        ShowGirl();
    }

    public void OnHoverBoy()
    {
        ShowBoy();
    }

    // ---------- CLICK ----------
    public void OnClickGirl()
    {
        ShowGirl();
        ConfirmSelection();
    }

    public void OnClickBoy()
    {
        ShowBoy();
        ConfirmSelection();
    }

    // ---------- KEYBOARD ----------
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ShowGirl();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ShowBoy();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ConfirmSelection();
        }
    }

    // ---------- HELPERS ----------
    void ShowGirl()
    {
        currentChoice = "Girl";
        SetAlpha(girlHighlight, 1f);
        SetAlpha(boyHighlight, 0f);
    }

    void ShowBoy()
    {
        currentChoice = "Boy";
        SetAlpha(girlHighlight, 0f);
        SetAlpha(boyHighlight, 1f);
    }

    void SetAlpha(Image img, float a)
    {
        if (img == null) return;
        var c = img.color;
        c.a = a;
        img.color = c;
    }

    void ConfirmSelection()
    {
        // save for gameplay scene
        PlayerPrefs.SetString("SelectedCharacter", currentChoice);
        SceneManager.LoadScene(nextSceneName);
    }
}
