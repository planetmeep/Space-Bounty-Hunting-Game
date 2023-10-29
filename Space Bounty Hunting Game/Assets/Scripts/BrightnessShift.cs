using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BrightnessShift : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Tilemap tilemap;
    [SerializeField] private float brightnessShiftTime;
    [SerializeField] private float minBrightness;
    [SerializeField] private float maxBrightness;
    private float currentBrightness;
    private float hue;
    private float saturation;

    private void Start()
    {
        TryGetComponent<SpriteRenderer>(out spriteRenderer);
        TryGetComponent<Tilemap>(out tilemap);
        if (spriteRenderer != null)
        {
            Color.RGBToHSV(spriteRenderer.color, out hue, out saturation, out _);
        }
        else if (tilemap != null)
        {
            Color.RGBToHSV(tilemap.color, out hue, out saturation, out _);
        }
        currentBrightness = minBrightness;
    }

    private void Update()
    {
        currentBrightness = Mathf.Lerp(minBrightness, maxBrightness, Mathf.PingPong(Time.time / brightnessShiftTime, 1));
        if (spriteRenderer != null)
        {

            spriteRenderer.color = tilemap.color = Color.HSVToRGB(hue, saturation, currentBrightness);
        }
        else if (tilemap != null)
        {
            tilemap.color = Color.HSVToRGB(hue, saturation, currentBrightness);
        }
    }
}
