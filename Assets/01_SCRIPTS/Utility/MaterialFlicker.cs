using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HDRColor : MonoBehaviour
{
    Volume postProcVolume;
    Bloom bloom;
    float originalValue;
    [SerializeField] float upper = 1.1f;
    [SerializeField] float lower = 0.8f;
    [SerializeField] float rate = 0.1f;

    float timer;
    float prevValue;
    float targetValue;

    private void Awake()
    {
        postProcVolume = GetComponent<Volume>();
    }

    private void Start()
    {
        if (postProcVolume.profile.TryGet(out Bloom b))
            bloom = b;
        originalValue = bloom.intensity.value;
        prevValue = originalValue;
        targetValue = NewValue();
    }

    private void Update()
    {
        if (timer >= rate)
        {
            timer = 0;
            prevValue = targetValue;
            targetValue = NewValue();
            bloom.intensity.value = prevValue;
            return;
        }

        bloom.intensity.value = Mathf.Lerp(prevValue, targetValue, timer / rate);

        timer += Time.deltaTime;
    }

    float NewValue()
    {
        float multiple = Random.Range(upper, lower);
        return originalValue * multiple;
    }
}