using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour {
    public Slider slider;

    public void SetHealth(float health) {
        slider.value = health;
    }

    public void SetMaxHealth(float maxHealth) {
        slider.maxValue = maxHealth;
    }
}
