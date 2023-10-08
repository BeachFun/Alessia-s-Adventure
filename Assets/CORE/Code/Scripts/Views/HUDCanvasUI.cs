using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class HUDCanvasUI : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsForInitialization;

    private void Awake()
    {
        objectsForInitialization.ToList().ForEach(e => e.SetActive(true));
    }

    private void Start()
    {
        objectsForInitialization.ToList().ForEach(e => e.SetActive(false));
    }
}
