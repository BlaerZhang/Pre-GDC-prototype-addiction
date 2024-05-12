using System.Collections;
using UnityEngine;

namespace _Scripts.Manager
{
    // TODO: adjustable volume
    public class AudioManager : MonoBehaviour
    {
        public void PlaySound(AudioClip audioClip)
        {
            if (audioClip == null) return;
            GameObject tempAudio = new GameObject("Temp Audio");
            AudioSource tempAudioSource = tempAudio.AddComponent<AudioSource>();

            tempAudioSource.playOnAwake = false;
            tempAudioSource.clip = audioClip;
            tempAudioSource.Play();
            StartCoroutine(DestroyTempAudio(tempAudio));
        }

        public GameObject PlayLoopSound(AudioClip audioClip)
        {
            if (audioClip == null) return null;
            GameObject tempAudio = new GameObject("Temp Audio");
            AudioSource tempAudioSource = tempAudio.AddComponent<AudioSource>();
            tempAudioSource.playOnAwake = false;
            tempAudioSource.loop = true;
            tempAudioSource.clip = audioClip;
            tempAudioSource.Play();

            return tempAudio;
        }

        public void StopLoopSound(GameObject tempAudio)
        {
            Destroy(tempAudio);
        }

        private IEnumerator DestroyTempAudio(GameObject tempAudio)
        {
            float clipLength = tempAudio.GetComponent<AudioSource>().clip.length;
            yield return new WaitForSeconds(clipLength + 1f);
            Destroy(tempAudio);
        }
    }
}