using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private Transform CounterTopPoint;
    [SerializeField] private GameObject PlateVisualPrefab;
    [SerializeField] private PlatesCounter platesCounter;
    private List<GameObject> plateVisualGameObjectList;

    private void Awake()
    {
        plateVisualGameObjectList = new List<GameObject>();
    }

    private void Start()
    {
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemove += PlatesCounter_OnPlateRemove;
    }

    private void PlatesCounter_OnPlateSpawned(object sender, System.EventArgs e)
    {
        GameObject plateVisualGameObject = Instantiate(PlateVisualPrefab, CounterTopPoint);
        plateVisualGameObject.transform.localPosition = Vector3.zero; // Reset position relative to parent

        float plateOffsetY = 0.1f;
        plateVisualGameObject.transform.localPosition = new Vector3(0, plateVisualGameObjectList.Count * plateOffsetY, 0);

        plateVisualGameObjectList.Add(plateVisualGameObject);
    }

    private void PlatesCounter_OnPlateRemove(object sender, System.EventArgs e)
    {
        if (plateVisualGameObjectList.Count > 0)
        {
            GameObject plateVisualGameObject = plateVisualGameObjectList[plateVisualGameObjectList.Count - 1];
            plateVisualGameObjectList.RemoveAt(plateVisualGameObjectList.Count - 1);
            Destroy(plateVisualGameObject);
        }
    }
}
