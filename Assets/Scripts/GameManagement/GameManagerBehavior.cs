using System;
using System.Collections.Generic;
using CharacterBehavior;
using CharacterBehavior.AnimalBehavior;
using Cinemachine;
using TMPro;
using UnityEngine;

namespace GameManagement {
    public class GameManagerBehavior : MonoBehaviour {
        public List<Tuple<GameObject, AnimalTypes>> AllAnimals { get; private set; }
        public Dictionary<AnimalTypes, AnimalAttributes> AnimalAttributesDict { get; private set; }
        public const float SwordDamage = 5.0f;
        public const float AxeDamage = 7.5f;

        public TMP_Text timerText;
        public TMP_Text winnerText;
        public ScoreboardBehavior scoreboardBehavior;
        public CinemachineFreeLook cinemachineFreeLook;
        public GameObject pauseCanvas;

        private float _timer;
        private bool _hasDisplayedWinner;

        private void Awake() {
            _timer = 301.0f;  // TODO: Maybe make this an option at the beginning?
            _hasDisplayedWinner = false;
            AllAnimals = new List<Tuple<GameObject, AnimalTypes>>();
            AnimalAttributesDict = new Dictionary<AnimalTypes, AnimalAttributes>();
            AnimalAttributesDict[AnimalTypes.Rabbit] = new AnimalAttributes(5.0f, 50.0f);
            AnimalAttributesDict[AnimalTypes.Cow] = new AnimalAttributes(4.0f, 100.0f);
            AnimalAttributesDict[AnimalTypes.Pig] = new AnimalAttributes(4.5f, 75.0f);
            // TODO: Add Chicken Here!
        }

        private void Update() {
            _timer -= Time.deltaTime;
            if (_timer < 0.0f) {
                DisplayWinner();
            }
            else {
                DisplayTime(_timer);
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (pauseCanvas.activeSelf) {
                    pauseCanvas.SetActive(false);
                    Cursor.visible = false;
                    cinemachineFreeLook.m_YAxis.m_InputAxisName = "Mouse Y";
                    cinemachineFreeLook.m_XAxis.m_InputAxisName = "Mouse X";
                }
                else {
                    pauseCanvas.SetActive(true);
                    Cursor.visible = true;
                    cinemachineFreeLook.m_YAxis.m_InputAxisName = "";
                    cinemachineFreeLook.m_XAxis.m_InputAxisName = "";
                }
            }
        }

        private void DisplayWinner() {
            if (!_hasDisplayedWinner) {
                timerText.gameObject.SetActive(false);
                winnerText.text = scoreboardBehavior.GetWinner() + "s Win!";
                winnerText.gameObject.SetActive(true);

                _hasDisplayedWinner = true;
            }
        }
    
        private void DisplayTime(float timeToDisplay)
        {
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);  
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);

            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
