using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameManagerBehavior : MonoBehaviour {
    public List<Tuple<GameObject, AnimalTypes>> AllAnimals { get; private set; }
    public Dictionary<AnimalTypes, AnimalAttributes> AnimalAttributesDict { get; set; }
    public const float SwordDamage = 5.0f;
    public const float AxeDamage = 7.5f;

    public CinemachineFreeLook cinemachineFreeLook;
    public GameObject pauseCanvas;

    private void Awake() {
        AllAnimals = new List<Tuple<GameObject, AnimalTypes>>();
        AnimalAttributesDict = new Dictionary<AnimalTypes, AnimalAttributes>();
        AnimalAttributesDict[AnimalTypes.Rabbit] = new AnimalAttributes(5.0f, 50.0f);
        AnimalAttributesDict[AnimalTypes.Cow] = new AnimalAttributes(4.0f, 100.0f);
        AnimalAttributesDict[AnimalTypes.Pig] = new AnimalAttributes(4.5f, 75.0f);
        // TODO: Add Chicken Here!
    }

    private void Update() {
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
}
