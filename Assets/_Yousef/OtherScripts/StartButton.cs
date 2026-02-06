using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonNextScene : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioSource audioSource;
    [SerializeField] private float delay = 1f;

    private bool hasBeenPressed = false;

    public void GoToNextScene()
    {
        if (hasBeenPressed) return;
        hasBeenPressed = true;

        if (clickSound != null)
        {
            if (audioSource == null)
            {
                GameObject temp = new GameObject("TempAudio");
                temp.transform.position = transform.position;
                audioSource = temp.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }

            audioSource.clip = clickSound;
            audioSource.spatialBlend = 0f;
            audioSource.Play();
        }

        StartCoroutine(LoadSceneAfterDelay(delay));
    }

    IEnumerator LoadSceneAfterDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
