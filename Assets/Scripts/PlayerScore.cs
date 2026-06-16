using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerScore : NetworkBehaviour

{
    // Variables de red: Servidor escribe, TODOS leen
    public NetworkVariable<int> puntosEnMano = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> puntosBanco = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // UI Local (Solo para el dueño)
    private TextMeshProUGUI textoMiUI;

    // UI Global (Para la tabla de posiciones de todos)
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

    // ==========================================================
    // NUEVOS PUENTES DE RED: Para que el cliente pueda sumar puntos
    // ==========================================================

    // Llama a esto desde tus monedas/triggers cuando un jugador deba sumar puntos en mano
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

    // ==========================================================
    // RPCs PARA UNITY 6 (Ejecución forzada en el Servidor)
    // ==========================================================

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

    // --- MÉTODOS DEL SERVIDOR ---
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

    // --- RESPUESTAS A CAMBIOS DE RED ---

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