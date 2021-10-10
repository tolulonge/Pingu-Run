using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackSpawner : MonoBehaviour
{
     public float chanceToSpawn = 1.2f;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {

       // OnDisable();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        if (Random.Range(0.0f, 1.0f) > chanceToSpawn)
        {
            return;
        }
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
