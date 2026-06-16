using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour

{
    
    [Header("Configuración del Ítem")]
    [SerializeField] private GameObject itemPrefab; 

    [Header("Puntos de Spawn")]
    [SerializeField] private List<Transform> spawnPoints; 

    [Header("Tiempos")]
    [SerializeField] private float tiempoEntreSpawns = 5f; 
    [SerializeField] private int maxItemsEnMapa = 10; 

    private List<GameObject> itemsSpawneados = new List<GameObject>();

    
    public override void OnNetworkSpawn()
    {
       
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        
        StartCoroutine(RutinaSpawn());
    }

    private IEnumerator RutinaSpawn()
    {
   
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntreSpawns);

            
            itemsSpawneados.RemoveAll(item => item == null);

         
            if (itemsSpawneados.Count < maxItemsEnMapa && spawnPoints.Count > 0)
            {
                SpawnearItemAleatorio();
            }
        }
    }

    private void SpawnearItemAleatorio()
    {
        
        int indiceAleatorio = Random.Range(0, spawnPoints.Count);
        Transform puntoElegido = spawnPoints[indiceAleatorio];

        
        GameObject nuevoItem = Instantiate(itemPrefab, puntoElegido.position, puntoElegido.rotation);

       
        NetworkObject netObj = nuevoItem.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn(); 
            itemsSpawneados.Add(nuevoItem); 
        }
        else
        {
            Debug.LogError($"[Spawner] ¡El prefab {itemPrefab.name} NO tiene el componente NetworkObject!");
            Destroy(nuevoItem); 
        }
    }
}

