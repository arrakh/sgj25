using UnityEngine;
using Assets.SimpleLocalization.Scripts;
public class MainMenuScreen : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeLanguage(string language)
    {
        LocalizationManager.Language = language;
    }
}
