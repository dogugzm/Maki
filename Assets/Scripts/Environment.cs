using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Environment : MonoBehaviour
{

    [SerializeField] EnvironmentData environmentData;
    public static Action OnPlayerCentered {  get; private set; }

    public List<Transform> instantiatedParts = new();

    private void OnEnable()
    {
        OnPlayerCentered += InstantiateEnvironmentPart;
    }

    private void OnDisable()
    {
        OnPlayerCentered -= InstantiateEnvironmentPart;
    }

    private void Start()
    {
        foreach (Transform part in transform)
        {
            instantiatedParts.Add(part);
        }
    }

    private void InstantiateEnvironmentPart()
     {
        GameObject instantiated = Instantiate(environmentData.EnvironmentPart, instantiatedParts[^1].position + new Vector3(100, 0, 0), Quaternion.identity);
        instantiated.transform.SetParent(transform);
        instantiatedParts.Add(instantiated.transform);
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        float movement = environmentData.Speed * Time.deltaTime;

        transform.Translate(Vector3.left * movement);

    }
}
