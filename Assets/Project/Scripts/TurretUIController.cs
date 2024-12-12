using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretUIController : MonoBehaviour
{
    [SerializeField] public TurretBehaviour turretBehaviour;
    [SerializeField] public TurretStatElement statPrefab;
    [SerializeField] private Vector3 statOffset;
    [SerializeField] private Vector3 statOffsetPosition;

    private List<TurretStatElement> spawnedStatList = new List<TurretStatElement>();

    

    private void Start()
    {
        foreach (ScriptableAttribute scriptableAttribute in turretBehaviour.AllAttributes)
            SpawnStatElement(scriptableAttribute);
    }

    private void SpawnStatElement(ScriptableAttribute attribute)
    {
        TurretStatElement instansiatedStatElement = GameObject.Instantiate(statPrefab);
        instansiatedStatElement.transform.SetParent(transform, false);
        spawnedStatList.Add(instansiatedStatElement);
        instansiatedStatElement.transform.localPosition += statOffset + (statOffsetPosition * spawnedStatList.Count);
        turretBehaviour.OnMouseoverToggle.AddListener(instansiatedStatElement.OnMouseoverToggle);

        instansiatedStatElement.SetAttribute(attribute);
    }
}
