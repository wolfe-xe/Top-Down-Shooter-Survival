using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;
    public GameObject audioHolder;
    public GameObject graphicsHolder;

    public Slider[] volumeSliders;
    public Toggle[] resToggle;
    public int[] screenWidth;
    public Toggle fullscreenToggle;

    int activeScreenResIdx;

    private void Start()
    {
        activeScreenResIdx = PlayerPrefs.GetInt("ScreenResIndex");
        bool b = (PlayerPrefs.GetInt("fullscreen") ==1)?true:false;

        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

        for (int i = 0; i < resToggle.Length; i++)
        {
            resToggle[i].isOn = i == activeScreenResIdx;
        }

        //fullscreenToggle.isOn = b;
        //fullscreenToggle.isOn = b;
    }

    public void Play()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void MainMenu()
    {
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
        audioHolder.SetActive(false);
        graphicsHolder.SetActive(false);
    }
    public void OptionsMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
        audioHolder.SetActive(false);
        graphicsHolder.SetActive(false);
    }
    public void AuidoMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(false);
        audioHolder.SetActive(true);
        graphicsHolder.SetActive(false);
    }
    public void GraphicsMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(false);
        audioHolder.SetActive(false);
        graphicsHolder.SetActive(true);
    }
    public void Res(int i) 
    {
        if (resToggle[i].isOn)
        {
            activeScreenResIdx = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidth[i], (int)(screenWidth[i]/aspectRatio), false);
            PlayerPrefs.SetInt("ScreenResIndex", activeScreenResIdx);
            PlayerPrefs.Save();
        }
    }
    public void Back()
    {

    }
    public void MasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }
    public void SfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }
    public void MusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }
    public void SetFullscreen(bool b)
    {
        for (int i = 0; i < resToggle.Length; i++)
        {
            resToggle [i].interactable = !b;
        }

        if(!b)
        {
            Resolution[] allResolution = Screen.resolutions;
            Resolution maxResolution = allResolution[allResolution.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            Res(activeScreenResIdx);
        }

        PlayerPrefs.SetInt("fullscreen", ((b) ? 1 : 0));
        PlayerPrefs.Save();
    }

}
