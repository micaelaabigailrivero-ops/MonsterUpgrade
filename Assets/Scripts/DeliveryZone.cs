using Unity.Netcode;
using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
       

        Debug.Log($"[BANCO] Choque detectado con: {other.name}");

 
        PlayerScore playerScore = other.GetComponent<PlayerScore>();
        if (playerScore == null)
        {
            playerScore = other.GetComponentInParent<PlayerScore>();
        }

        
        if (playerScore != null)
        {
            
            if (playerScore.IsOwner)
            {
                
                playerScore.SolicitarEntregarPuntos();
                Debug.Log("[BANCO] ¡ÉXITO! Solicitando entrega de puntos a través del puente de red seguro.");
            }
        }
        else
        {
            Debug.LogError($"[BANCO] Se detectó el choque con {other.name}, pero no tiene el script 'PlayerScore'.");
        }
    }
}