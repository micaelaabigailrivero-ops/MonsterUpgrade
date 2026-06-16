using Unity.Netcode;
using UnityEngine;

public class ItemPickup : NetworkBehaviour
{
    [SerializeField] private int itemValue = 1;
    private bool isCollected = false;

  
    public void RecolectarItem(PlayerScore playerScore)
    {
       
        Debug.Log($"[ITEM] ¡Alguien me tocó! ¿Ya fui recolectado antes?: {isCollected}");

        if (isCollected) return;
        isCollected = true;

        playerScore.SolicitarAgregarPuntos(itemValue);

        if (IsServer)
        {
            Debug.Log("[ITEM] Soy el Servidor y voy a destruirme de la red ahora mismo.");
            GetComponent<NetworkObject>().Despawn(true);
        }
        else
        {
            Debug.Log("[ITEM] Soy un Cliente, ocultando gráficos.");
            gameObject.SetActive(false);
        }
    }
}
