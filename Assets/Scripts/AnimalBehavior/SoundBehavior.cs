using UnityEngine;

namespace AnimalBehavior {
    public class SoundBehavior : MonoBehaviour {

        private AudioSource _audioSource;
        private float _timer;

        private const float MinTime = 10.0f;
        private const float MaxTime = 30.0f;

        private void Start() {
            _audioSource = GetComponent<AudioSource>();
            _timer = Random.Range(MinTime, MaxTime);
        }

        private void Update() {
            _timer -= Time.deltaTime;
            if (_timer < 0.0f) {
                _audioSource.Play();
                _timer = Random.Range(MinTime, MaxTime);
            }
        }
    }
}
