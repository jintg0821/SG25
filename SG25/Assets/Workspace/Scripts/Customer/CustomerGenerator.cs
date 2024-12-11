using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomerGenerator : MonoBehaviour
{
    public GameObject[] customerPrefabs;
    public float spawnRateMin = 5f;
    public float spawnRateMax = 10f;
    public GameObject[] spawnPoints;
    public GameObject[] allCustomers;
    


    private float spawnRate;
    private float spawntime = 0f;

    void Start()
    {
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        spawnPoints = GameObject.FindGameObjectsWithTag("CustomerPoint");

    }
    void Update()
    {
        spawntime += Time.deltaTime;

        if (spawntime >= spawnRate)
        {
            spawntime = 0f;
            GameObject customer = customerPrefabs[Random.Range(0, customerPrefabs.Length)];
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
            Instantiate(customer, spawnPoint);
            spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        }

        if (GameManager.Instance.hours >= 22)
        {
            allCustomers = GameObject.FindGameObjectsWithTag("Customer");
            foreach (GameObject customer in allCustomers)
            {
                Destroy(customer);
            }    
        }
    }
}
