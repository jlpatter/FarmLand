using UnityEngine;
using UnityEngine.UI;

namespace CharacterBehavior.PlayerBehavior {
    public class PlayerHealthBar : MonoBehaviour {
        public Slider slider;

        public void SetHealth(float health) {
            slider.value = health;
        }

        public void SetMaxHealth(float maxHealth) {
            slider.maxValue = maxHealth;
        }
    }
}
