using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip _clip1;
    [SerializeField] private AudioClip _clip2;
    [SerializeField] private AudioClip _clip3;

    [SerializeField] private AudioSource _source;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Audio());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Audio()
    {
        while (true)
        {
            _source.clip = _clip1;
            _source.Play();

            yield return new WaitWhile(() => _source.isPlaying);

            _source.clip = _clip2;
            _source.Play();

            yield return new WaitWhile(() => _source.isPlaying);

            _source.clip = _clip3;
            _source.Play();

            yield return new WaitWhile(() => _source.isPlaying);
        }
    }
}
