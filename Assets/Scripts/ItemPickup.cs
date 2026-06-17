using Unity.Netcode;
using UnityEngine;

public class ItemPickup : NetworkBehaviour
{
    [SerializeField] private int itemValue = 1;
    private bool isCollected = false;

    private void OnEnable()
    {
        isCollected = false;
    }

   
    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

       
        PlayerScore playerScore = other.GetComponentInParent<PlayerScore>();

        if (playerScore != null)
        {
           
            if (playerScore.IsOwner)
            {
                isCollected = true;

                
                playerScore.SolicitarAgregarPuntos(itemValue);

                
                if (IsServer)
                {
                    
                    GetComponent<NetworkObject>().Despawn(true);
                }
                else
                {
                    
                    DespawnItemServerRpc();

                   
                    OcultarItemLocal();
                }
            }
        }
    }

    
    [Rpc(SendTo.Server)]
    private void DespawnItemServerRpc()
    {
        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn(true); 
        }
    }

    
    private void OcultarItemLocal()
    {
        if (TryGetComponent<Collider>(out Collider col)) col.enabled = false;
        if (TryGetComponent<Renderer>(out Renderer ren)) ren.enabled = false;

        foreach (Transform hijo in transform)
        {
            hijo.gameObject.SetActive(false);
        }
    }
}