using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider slider;
    public float sliderValue;
    public Image imagenMute;

    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat("volume", 1f);
        AudioListener.volume = slider.value;
        IsVolumeActive();
    }

    public void ChangeSlider(float value)
    {
        slider.value = value;
        PlayerPrefs.SetFloat("volume", sliderValue);
        AudioListener.volume = slider.value;
        IsVolumeActive();
    }

    public void IsVolumeActive()
    {
        if (slider.value == 0)
        {
            imagenMute.enabled = true;
        }
        else
        {
            imagenMute.enabled = false;
        }
    }


}
