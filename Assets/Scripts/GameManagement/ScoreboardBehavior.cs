using System;
using System.Collections.Generic;
using System.Linq;
using CharacterBehavior;
using UnityEngine;
using UnityEngine.UI;

namespace GameManagement {
    public class ScoreboardBehavior : MonoBehaviour {
        private int _rabbitScore;
        private int _cowScore;
        private int _pigScore;
        private int _chickenScore;
        private Text _rabbitScoreText;
        private Text _cowScoreText;
        private Text _pigScoreText;
        private Text _chickenScoreText;

        private void Start() {
            _rabbitScore = 0;
            _cowScore = 0;
            _pigScore = 0;
            _chickenScore = 0;
            _rabbitScoreText = GameObject.Find("RabbitScoreText").GetComponent<Text>();
            _cowScoreText = GameObject.Find("CowScoreText").GetComponent<Text>();
            _pigScoreText = GameObject.Find("PigScoreText").GetComponent<Text>();
            _chickenScoreText = GameObject.Find("ChickenScoreText").GetComponent<Text>();
        }

        public void AddPoint(AnimalTypes animalType) {
            switch (animalType) {
                case AnimalTypes.Rabbit:
                    _rabbitScore++;
                    _rabbitScoreText.text = "Rabbit Score: " + _rabbitScore;
                    break;
                case AnimalTypes.Cow:
                    _cowScore++;
                    _cowScoreText.text = "Cow Score: " + _cowScore;
                    break;
                case AnimalTypes.Pig:
                    _pigScore++;
                    _pigScoreText.text = "Pig Score: " + _pigScore;
                    break;
                case AnimalTypes.Chicken:
                    _chickenScore++;
                    _chickenScoreText.text = "Chicken Score: " + _chickenScore;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animalType), animalType, null);
            }
        }

        public void RemovePoint(AnimalTypes animal) {
            switch (animal) {
                case AnimalTypes.Rabbit:
                    _rabbitScore--;
                    _rabbitScoreText.text = "Rabbit Score: " + _rabbitScore;
                    break;
                case AnimalTypes.Cow:
                    _cowScore--;
                    _cowScoreText.text = "Cow Score: " + _cowScore;
                    break;
                case AnimalTypes.Pig:
                    _pigScore--;
                    _pigScoreText.text = "Pig Score: " + _pigScore;
                    break;
                case AnimalTypes.Chicken:
                    _chickenScore--;
                    _chickenScoreText.text = "Chicken Score: " + _chickenScore;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animal), animal, null);
            }
        }

        public AnimalTypes GetWinner() {
            var scoreList =
                new List<Tuple<int, AnimalTypes>> {
                    new Tuple<int, AnimalTypes>(_cowScore, AnimalTypes.Cow),
                    new Tuple<int, AnimalTypes>(_rabbitScore, AnimalTypes.Rabbit),
                    new Tuple<int, AnimalTypes>(_pigScore, AnimalTypes.Pig),
                    new Tuple<int, AnimalTypes>(_chickenScore, AnimalTypes.Chicken)
                };
        
            return scoreList.Max().Item2;
        }
    }
}
