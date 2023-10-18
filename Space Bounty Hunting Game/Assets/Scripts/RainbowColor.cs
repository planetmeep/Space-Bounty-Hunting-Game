using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowColor : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    [SerializeField] private float _hueShiftSpeed = 0.2f;
    [SerializeField, Range(0, 1)] private float _saturation = 1f;
    [SerializeField, Range(0, 1)] private float _value = 1f;

    private void Update()
    {
        float amountToShift = _hueShiftSpeed * Time.deltaTime;
        Color newColor = ShiftHueBy(spriteRenderer.color, amountToShift);
        spriteRenderer.color = newColor;
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
        return Color.HSVToRGB(hue, sat, val);
    }
}
