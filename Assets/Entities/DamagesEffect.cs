using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] Image _damageImage;
    [SerializeField] AudioClip _damageSound;
    [SerializeField] float _fadeDuration = 2f;
    Color _originalColor;
    [SerializeField] AudioSource _audioSource;
    Coroutine fadeCoroutine;

    void Start()
    {
        _originalColor = _damageImage.color;
        _damageImage.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0);
    }

    void Update()
    {
        if (_damageImage.color.a > 0)
        {
            float newAlpha = _damageImage.color.a - _fadeDuration * Time.deltaTime;
            _damageImage.color = new Color(_damageImage.color.r, _damageImage.color.g, _damageImage.color.b, newAlpha);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowDamageEffect();
            //AudioSource.PlayClipAtPoint(_damageSound, other.transform.position);
            //PlayDamageSound();
        }
    }

    private void PlayDamageSound() 
    {
        if (_damageSound != null) 
        {
            Debug.Log("Damage Sound correctly assigned and initialized.");
            _audioSource.PlayOneShot(_damageSound); 
        } 
    }
    public void ShowDamageEffect()
    {
        if (_damageImage != null) 
        { 
            if (fadeCoroutine != null) 
            { 
                StopCoroutine(fadeCoroutine); 
            } 
            _damageImage.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0.15f);
            fadeCoroutine = StartCoroutine(FadeOutImage()); 
        }
    }
    IEnumerator FadeOutImage() 
    { 
        float elapsedTime = 0f;
        Color startColor = _damageImage.color;
        Color endColor = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0);
        while (elapsedTime < _fadeDuration) 
        {
            _damageImage.color = Color.Lerp(startColor, endColor, elapsedTime / _fadeDuration); elapsedTime += Time.deltaTime; 
            yield return null;
        } 
        _damageImage.color = endColor; 
    }
}
