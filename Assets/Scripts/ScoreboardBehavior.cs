using System;
using UnityEngine;

public class ScoreboardBehavior : MonoBehaviour {
    private int _rabbitScore;
    private int _cowScore;
    private int _pigScore;
    private int _chickenScore;

    private void Start() {
        _rabbitScore = 0;
        _cowScore = 0;
        _pigScore = 0;
        _chickenScore = 0;
    }

    public void AddPoint(Animal animal) {
        switch (animal) {
            case Animal.Rabbit:
                _rabbitScore++;
                break;
            case Animal.Cow:
                _cowScore++;
                break;
            case Animal.Pig:
                _pigScore++;
                break;
            case Animal.Chicken:
                _chickenScore++;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(animal), animal, null);
        }
    }
}
