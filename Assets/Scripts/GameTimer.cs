using TMPro;
using Unity.Netcode;

using UnityEngine;

public class GameTimer : NetworkBehaviour
{
    [Header("Configuracion del Tiempo")]
    [SerializeField] private float tiempoInicialSegundos = 60f;

   
    private NetworkVariable<float> tiempoRestante = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private TextMeshProUGUI textoTimerUI;
    private bool partidaActiva = false;

    [Header("UI de Fin de Juego")]
    private GameObject panelVictoria;
    private TextMeshProUGUI textoGanadorUI;

    private void Start()
    {
        if (IsServer && !IsSpawned)
        {
            GetComponent<NetworkObject>().Spawn();
        }
    }

    public override void OnNetworkSpawn()
    {
        BuscarUI();

       
        tiempoRestante.OnValueChanged += AlCambiarTiempo;

       
        if (IsServer)
        {
            if (!partidaActiva)
            {
                tiempoRestante.Value = tiempoInicialSegundos;
                partidaActiva = true;
            }
        }
        else
        {
            
            partidaActiva = true;
        }

        
        ActualizarTextoUI(tiempoRestante.Value);
    }

    public override void OnNetworkDespawn()
    {
        tiempoRestante.OnValueChanged -= AlCambiarTiempo;
    }

    private void AlCambiarTiempo(float valorAnterior, float valorNuevo)
    {
        ActualizarTextoUI(valorNuevo);
    }

    private void ActualizarTextoUI(float tiempo)
    {
        if (textoTimerUI != null)
        {
            
            int minutos = Mathf.FloorToInt(tiempo / 60);
            int segundos = Mathf.FloorToInt(tiempo % 60);
            textoTimerUI.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    private void BuscarUI()
    {
        if (textoTimerUI == null)
        {
            GameObject objTexto = GameObject.Find("TextoTimer");
            if (objTexto != null) textoTimerUI = objTexto.GetComponent<TextMeshProUGUI>();
        }

        if (panelVictoria == null)
        {
            GameObject canvasObj = GameObject.Find("Canvas");
            if (canvasObj != null)
            {
                Transform panelTransform = canvasObj.transform.Find("PanelVictoria");
                if (panelTransform != null)
                {
                    panelVictoria = panelTransform.gameObject;
                    Transform textoTransform = panelVictoria.transform.Find("TextoGanador");
                    if (textoTransform != null)
                    {
                        textoGanadorUI = textoTransform.GetComponent<TextMeshProUGUI>();
                    }
                }
            }
        }
    }

    private void Update()
    {
       
        if (!IsServer || !partidaActiva) return;

        if (tiempoRestante.Value > 0)
        {
            tiempoRestante.Value -= Time.deltaTime;
        }
        else
        {
            tiempoRestante.Value = 0;
            partidaActiva = false;
            CalcularGanadorServer();
        }
    }

    private void CalcularGanadorServer()
    {
        int mejorPuntaje = -1;
        ulong idGanador = 0;
        bool esEmpate = false;

        
        PlayerScore[] todosLosPuntajes = Object.FindObjectsByType<PlayerScore>(FindObjectsSortMode.None);

        if (todosLosPuntajes.Length == 0)
        {
            MostrarVictoriaRpc(0, 0, true);
            return;
        }

        foreach (PlayerScore scoreComponent in todosLosPuntajes)
        {
            int puntosDeEsteJugador = scoreComponent.puntosBanco.Value;
            ulong idDeEsteJugador = scoreComponent.OwnerClientId;

            Debug.Log($"[SERVER-BALANCE] Revisando Jugador {idDeEsteJugador + 1} con {puntosDeEsteJugador} pts.");

            if (puntosDeEsteJugador > mejorPuntaje)
            {
                mejorPuntaje = puntosDeEsteJugador;
                idGanador = idDeEsteJugador;
                esEmpate = false;
            }
            else if (puntosDeEsteJugador == mejorPuntaje)
            {
                esEmpate = true;
            }
        }

        if (todosLosPuntajes.Length > 1 && mejorPuntaje == 0)
        {
            esEmpate = true;
        }

        if (todosLosPuntajes.Length == 1)
        {
            esEmpate = false;
        }

        Debug.Log($"[SERVER-RESULTADO] Ganador definitivo enviado: Jugador {idGanador + 1} con {mejorPuntaje} pts. ¿Fue empate?: {esEmpate}");

        MostrarVictoriaRpc(idGanador, mejorPuntaje, esEmpate);
    }

    [Rpc(SendTo.Everyone)]
    private void MostrarVictoriaRpc(ulong idDelGanador, int puntosMaximos, bool huboEmpate)
    {
        BuscarUI();

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);

            if (textoTimerUI != null) textoTimerUI.text = "¡PARTIDA TERMINADA!";

            if (textoGanadorUI != null)
            {
                if (huboEmpate || puntosMaximos == 0)
                {
                    textoGanadorUI.text = $"¡Empate! Nadie logro superar al otro con {puntosMaximos} pts.";
                }
                else
                {
                    textoGanadorUI.text = $"¡Ganador: Jugador {idDelGanador} con {puntosMaximos} pts!";
                }
            }
        }
    }

    public void PresionarVolverAJugar()
    {
        if (IsServer)
        {
            ReiniciarPartida();
        }
    }

    private void ReiniciarPartida()
    {
        string nombreEscenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        NetworkManager.Singleton.SceneManager.LoadScene(nombreEscenaActual, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void PresionarSalir()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

#if UNITY_EDITOR
       
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipal"); 
#endif
    }
}