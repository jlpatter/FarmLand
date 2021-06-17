using System;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterBehavior.AnimalBehavior {
    public class AIHealthBar : MonoBehaviour {
        public Slider slider;

        public void SetHealth(float health) {
            slider.value = health;
            if (Math.Abs(health - slider.maxValue) < 0.01f) {
                gameObject.SetActive(false);
            }
            else {
                gameObject.SetActive(true);
            }
        }

        public void SetMaxHealth(float maxHealth) {
            slider.maxValue = maxHealth;
        }
    }
}