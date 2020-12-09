using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public Canvas mainMenu;
    public Canvas settingsMenu;
    public Canvas graphics;
    public Canvas sounds;
    public Canvas misc;

    //Resolution
    public bool fullscreen;
    public TextMeshProUGUI resolutionText;
    private int _newResolution;
    private Resolution[] _resolutionArray;
    private Resolution _actualResolution;

    //Quality+
    public TextMeshProUGUI qualityText;
    private string[] _qualityNames;
    private int _actualQuality;
    private int _newQuality;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        QualityStart();
        ResolutionStart();
    }

    #region MainMenu
    public void OnNewGameButton()
    {
        //TODO:Transicion
        gameObject.SetActive(false);
        SceneLoadManager.instance.LoadSceneAsync("Player", LoadSceneMode.Additive);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        EventManager.DispatchEvent(GameEvent.START_GAME);
    }

    public void OnSettingsMenuButton()
    {
        mainMenu.enabled = false;
        settingsMenu.enabled = true;
    }

    public void OnBackMenuButton()
    {
        mainMenu.enabled = true;
        settingsMenu.enabled = false;
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
    #endregion

    #region Volume
    public void OnSoundVolumeChange(float vol)
    {
        Debug.Log("Change Sound Volumen" + vol);
        //SoundsManager.ChangeSoundVolume(vol);
    }

    public void OnMasterVolumeChange(float vol)
    {
        Debug.Log("Change Master Volumen" + vol);
        //SoundsManager.ChangeMasterVolume(vol);
    }

    public void OnMusicVolumeChange(float vol)
    {
        Debug.Log("Change Music Volumen" + vol);
        //SoundsManager.ChangeMusicVolume(vol);
    }

    public void OnAmbientVolumeChange(float vol)
    {
        Debug.Log("Change Ambient Volumen" + vol);
        //SoundsManager.ChangeAmbientVolume(vol);
    }
    #endregion

    #region Misc
    public void OnLanguageChange(int option)
    {
        switch (option)
        {
            case 0:
                //LangageManager.Change("English")
                Debug.Log("Ingles!");
                break;
            case 1:
                //LangageManager.Change("Spanish")
                Debug.Log("Español!");
                break;
            default:
                break;
        }
    }
    #endregion

    #region Resolution
    //UI
    private void ResolutionStart()
    {
        _resolutionArray = Screen.resolutions;
        _actualResolution = Screen.currentResolution;
        resolutionText.text = _actualResolution.width.ToString() + " - " + _actualResolution.height.ToString();
        for (int i = 0; i < _resolutionArray.Length; i++)
        {
            if(_actualResolution.width == _resolutionArray[i].width && _actualResolution.height == _resolutionArray[i].height)
            {
                _newResolution = i;
                break;
            }
        }
        Resolutions();
    }

    //Resolution
    public void NextResolution()
    {
        _newResolution++;
        Resolutions();
    }

    public void BackResolution()
    {
        _newResolution--;
        Resolutions();
    }

    public void FullScreen() {
        fullscreen = !fullscreen;
    }

    private void Resolutions()
    {
        _newResolution = Mathf.Clamp(_newResolution, 0, _resolutionArray.Length - 1);
        resolutionText.text = _resolutionArray[_newResolution].ToString();
    }

    #endregion

    #region Quality
    private void QualityStart()
    {
        _qualityNames  = QualitySettings.names;
        _newQuality = QualitySettings.GetQualityLevel();
        Quality();
    }

    public void NextQuality()
    {
        _newQuality++;
        Quality();
    }

    public void BackQuality()
    {
        _newQuality--;
        Quality();
    }

    private void Quality()
    {
        _newQuality = Mathf.Clamp(_newQuality, 0, _qualityNames.Length - 1);
        qualityText.text = _qualityNames[_newQuality];
    }
    #endregion
    public void Apply()
    {
        Screen.SetResolution(_resolutionArray[_newResolution].width, _resolutionArray[_newResolution].height, fullscreen);
        QualitySettings.SetQualityLevel(_newQuality);
    }
    #region Tabs
    public void OnSettingsSoundButton()
    {
        sounds.enabled = true;
        graphics.enabled = false;
        misc.enabled = false;
    }

    public void OnSettingsGraphicsButton()
    {
        sounds.enabled = false;
        graphics.enabled = true;
        misc.enabled = false;
    }

    public void OnSettingsMiscButton()
    {
        sounds.enabled = false;
        graphics.enabled = false;
        misc.enabled = true;
    }
    #endregion
}
