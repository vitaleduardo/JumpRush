using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;

    public void SetHealth(int current, int max)
    {
        float value = (float)current / max;
        fillImage.fillAmount = value;
    }
}