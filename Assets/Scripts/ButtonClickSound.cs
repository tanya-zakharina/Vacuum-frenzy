using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ButtonClickSound : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioMixer audioMixer;
    public AudioMixerGroup sfxGroup;

    private AudioSource audioSource;
    private HashSet<Button> subscribedButtons = new HashSet<Button>();
    private HashSet<Slider> subscribedSliders = new HashSet<Slider>();
    public string sfxParameterName = "SFXVolume";

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        if (audioMixer != null)
        {
            var groups = audioMixer.FindMatchingGroups("SFX");
            if (groups.Length > 0)
                audioSource.outputAudioMixerGroup = groups[0];
        }

        if (sfxGroup != null)
            audioSource.outputAudioMixerGroup = sfxGroup;

        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        float dbVolume = Mathf.Log10(Mathf.Clamp(sfxVolume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(sfxParameterName, dbVolume);

        SubscribeAllUIElements();
    }

    private void OnEnable()
    {
        SubscribeAllUIElements();
    }

    private void Update()
    {
        SubscribeNewUIElements();
    }

    private void SubscribeAllUIElements()
    {
        foreach (var button in FindObjectsOfType<Button>(true))
        {
            TrySubscribeButton(button);
        }

        foreach (var slider in FindObjectsOfType<Slider>(true))
        {
            TrySubscribeSlider(slider);
        }
    }

    private void SubscribeNewUIElements()
    {
        foreach (var button in FindObjectsOfType<Button>(true))
        {
            if (!subscribedButtons.Contains(button))
                TrySubscribeButton(button);
        }

        foreach (var slider in FindObjectsOfType<Slider>(true))
        {
            if (!subscribedSliders.Contains(slider))
                TrySubscribeSlider(slider);
        }
    }

    private void TrySubscribeButton(Button button)
    {
        button.onClick.AddListener(PlayClickSound);
        subscribedButtons.Add(button);
    }

    private void TrySubscribeSlider(Slider slider)
    {
        var trigger = slider.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = slider.gameObject.AddComponent<EventTrigger>();
        }

        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        entry.callback.AddListener(_ => PlayClickSound());

        trigger.triggers.Add(entry);
        subscribedSliders.Add(slider);
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
