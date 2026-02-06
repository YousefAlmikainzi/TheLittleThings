using UnityEngine;
using System.Collections;

public class BossVoiceLooper : MonoBehaviour
{
    [SerializeField] AudioClip clipA;
    [SerializeField] AudioClip clipB;
    [SerializeField] float interval = 5f;

    bool useA = true;
    Coroutine loop;

    void OnEnable()
    {
        loop = StartCoroutine(PlayLoop());
    }

    void OnDisable()
    {
        if (loop != null)
            StopCoroutine(loop);
    }

    IEnumerator PlayLoop()
    {
        while (true)
        {
            AudioClip clip = useA ? clipA : clipB;
            useA = !useA;

            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, transform.position);

            yield return new WaitForSeconds(interval);
        }
    }
}
