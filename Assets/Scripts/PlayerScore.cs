using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerScore : NetworkBehaviour

{
  
    public NetworkVariable<int> puntosEnMano = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> puntosBanco = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

   
    private TextMeshProUGUI textoMiUI;

    
    private TextMeshProUGUI textoLeaderboard;

    public override void OnNetworkSpawn()
    {
        puntosEnMano.OnValueChanged += AlCambiarPuntosMano;
        puntosBanco.OnValueChanged += AlCambiarPuntosBanco;

        ConfigurarUI();
    }

    public override void OnNetworkDespawn()
    {
        puntosEnMano.OnValueChanged -= AlCambiarPuntosMano;
        puntosBanco.OnValueChanged -= AlCambiarPuntosBanco;
    }

    private void ConfigurarUI()
    {
        // 1. UI Propia
        if (IsOwner && textoMiUI == null)
        {
            GameObject objTextoLocal = GameObject.Find("TextoPuntaje");
            if (objTextoLocal != null)
            {
                textoMiUI = objTextoLocal.GetComponent<TextMeshProUGUI>();
            }
        }
        if (IsOwner && textoMiUI != null)
        {
            textoMiUI.text = $"En Mano: {puntosEnMano.Value}";
        }

        // 2. Tabla Global
        if (textoLeaderboard == null)
        {
            GameObject objTextoGlobal = GameObject.Find($"Texto_Jugador_{OwnerClientId}");
            if (objTextoGlobal != null)
            {
                textoLeaderboard = objTextoGlobal.GetComponent<TextMeshProUGUI>();
            }
        }
        if (textoLeaderboard != null)
        {
            textoLeaderboard.text = $"Jugador {OwnerClientId + 1}: {puntosBanco.Value} pts";
        }
    }

    
    
    

    
    public void SolicitarAgregarPuntos(int cantidad)
    {
        if (IsServer)
        {
            AddScore(cantidad);
        }
        else if (IsClient)
        {
            AddScoreServerRpc(cantidad);
        }
    }

    // Llama a esto desde tu zona de entrega/banco para guardar los puntos
    public void SolicitarEntregarPuntos()
    {
        if (IsServer)
        {
            EntregarPuntosServer();
        }
        else if (IsClient)
        {
            EntregarPuntosServerRpc();
        }
    }

   

    [Rpc(SendTo.Server)]
    private void AddScoreServerRpc(int amount)
    {
        AddScore(amount);
    }

    [Rpc(SendTo.Server)]
    private void EntregarPuntosServerRpc()
    {
        EntregarPuntosServer();
    }

    
    public void AddScore(int amount)
    {
        if (!IsServer) return;
        puntosEnMano.Value += amount;
    }

    public int EntregarPuntosServer()
    {
        if (!IsServer) return 0;
        if (puntosEnMano.Value == 0) return 0;

        int cantidadAEntregar = puntosEnMano.Value;
        puntosBanco.Value += cantidadAEntregar;
        puntosEnMano.Value = 0;

        return cantidadAEntregar;
    }

    

    private void AlCambiarPuntosMano(int viejo, int nuevo)
    {
        if (IsOwner)
        {
            if (textoMiUI == null) ConfigurarUI();
            if (textoMiUI != null) textoMiUI.text = $"En Mano: {nuevo}";
        }
    }

    private void AlCambiarPuntosBanco(int viejo, int nuevo)
    {
        if (textoLeaderboard == null) ConfigurarUI();
        if (textoLeaderboard != null)
        {
            textoLeaderboard.text = $"Jugador {OwnerClientId + 1}: {nuevo} pts";
        }
    }
}