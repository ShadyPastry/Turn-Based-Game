using UnityEngine;

public static class ColorExtensions {
  public static Color Randomize(this Color color, float minusVariance, float plusVariance) {
    float r = Mathf.Clamp01(Random.Range(color.r * minusVariance, color.r * plusVariance));
    float g = Mathf.Clamp01(Random.Range(color.g * minusVariance, color.g * plusVariance));
    float b = Mathf.Clamp01(Random.Range(color.b * minusVariance, color.b * plusVariance));
    return new Color(r, g, b);
  }
}
