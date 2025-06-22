using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] tracks;                   
    public AudioSource audioSource;            
    public AudioMixerGroup musicMixerGroup;    

    public float fadeDuration = 2f;             
    private int currentTrackIndex = 0;

    private void Start()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.outputAudioMixerGroup = musicMixerGroup;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        if (tracks.Length > 0)
            StartCoroutine(PlayMusicLoop());
    }

    private IEnumerator PlayMusicLoop()
    {
        while (true)
        {
            AudioClip currentTrack = tracks[currentTrackIndex];
            audioSource.clip = currentTrack;
            audioSource.volume = 0f;
            audioSource.Play();

            yield return StartCoroutine(FadeIn());

    
            yield return new WaitForSeconds(currentTrack.length - fadeDuration);

           
            yield return StartCoroutine(FadeOut());

            currentTrackIndex = (currentTrackIndex + 1) % tracks.Length;
        }
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        audioSource.volume = 1f;
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();
    }
}
