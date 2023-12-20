using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RainbowColor : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Tilemap tilemap;
    [SerializeField] private float _hueShiftSpeed = 0.2f;
    [SerializeField, Range(0, 1)] private float _saturation = 1f;
    [SerializeField, Range(0, 1)] private float _value = 1f;
    [SerializeField, Range(0, 1)] private float _opacity = 1f;

    private void Start()
    {
        TryGetComponent<SpriteRenderer>(out spriteRenderer);
        TryGetComponent<Tilemap>(out tilemap);
    }
    private void Update()
    {
        float amountToShift = _hueShiftSpeed * Time.deltaTime;
        if (spriteRenderer != null)
        {
            Color newColor = ShiftHueBy(spriteRenderer.color, amountToShift);
            spriteRenderer.color = newColor;
        } else if (tilemap != null) 
        {
            Color newColor = ShiftHueBy(tilemap.color, amountToShift);
            tilemap.color = newColor;
        }
    }

    private Color ShiftHueBy(Color color, float amount)
    {
        // convert from RGB to HSV
        Color.RGBToHSV(color, out float hue, out float sat, out float val);

        // shift hue by amount
        hue += amount;
        sat = _saturation;
        val = _value;

        // convert back to RGB and return the color
        Color targetColor = Color.HSVToRGB(hue, sat, val);
        targetColor.a = _opacity;
        return targetColor;
    }
}
