using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class Menu : MonoBehaviour
{
    //Menu panel
    private GameObject _menuPanel;
    //Options panel
    private GameObject _optionsPanel;

    //MusicVolumeSlider
    private Slider _musicSlider;
    //AmbienceVolumeSlider
    private Slider _ambienceSlider;
    //EffectsVolumeSlider
    private Slider _effectsSlider;

    //volume changed events from audio manager
    public static event AudioManager.MusicVolumeChangedEvent MusicVolumeChangedEvent;
    public static event AudioManager.AmbienceVolumeChangedEvent AmbienceVolumeChangedEvent;
    public static event AudioManager.EffectsVolumeChangedEvent EffectsVolumeChangedEvent;

    void Start()
    {
        //find the gameObject with "MenuPanel" name and set it to the _menuPanel
        //this way both mainMenu and ingameMenu can be handled with the same script
        _menuPanel = GameObject.Find("MenuPanel");
        //find the gameObject with "OptionsPanel" name and set it to the _optionsPanel
        _optionsPanel = GameObject.Find("OptionsPanel");

        //find the gameObjects named Start, Options and Exit buttons and add event listeners to them
        GameObject.Find("StartButton").GetComponent<Button>().onClick.AddListener(PlayGame);
        GameObject.Find("OptionsButton").GetComponent<Button>().onClick.AddListener(ToggleOptions);
        GameObject.Find("ExitButton").GetComponent<Button>().onClick.AddListener(QuitGame);

        //add eventlisteners to the sliders and invoke Audio Manager events on value change
        _musicSlider = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();
        _ambienceSlider = GameObject.Find("AmbienceVolumeSlider").GetComponent<Slider>();
        _effectsSlider = GameObject.Find("EffectsVolumeSlider").GetComponent<Slider>();

        _musicSlider.onValueChanged.AddListener(delegate { MusicVolumeChangedEvent?.Invoke(_musicSlider.value); });
        _ambienceSlider.onValueChanged.AddListener(delegate { AmbienceVolumeChangedEvent?.Invoke(_ambienceSlider.value); });
        _effectsSlider.onValueChanged.AddListener(delegate { EffectsVolumeChangedEvent?.Invoke(_effectsSlider.value); });


        //add an onclick event to the back button in the options panel
        GameObject.Find("OptionsBackButton").GetComponent<Button>().onClick.AddListener(ToggleOptions);

        ToggleOptions();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("LoadoutMenu");
    }

    //toggle the options panel
    public void ToggleOptions()
    {
        Debug.Log("Before toggle " + _optionsPanel.activeSelf);
        _optionsPanel.SetActive(!_optionsPanel.activeSelf);

        Debug.Log("After toggle: " + _optionsPanel.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
