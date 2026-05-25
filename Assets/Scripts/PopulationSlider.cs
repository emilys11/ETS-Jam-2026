using UnityEngine;
using UnityEngine.UI;

public class PopulationSlider : MonoBehaviour
{
    [SerializeField] private Slider populationSlider;

    [Header("Population Settings")]
    [SerializeField] private int maxPopulation = (int) GameManager.Instance.loseThreshold;

    private void Start()
    {
        populationSlider.minValue = 0;
        populationSlider.maxValue = maxPopulation;
    }

    private void Update()
    {
        populationSlider.value = GameManager.Instance.GetDinosAlive();
    }
}