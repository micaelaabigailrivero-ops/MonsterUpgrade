using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuUI : MonoBehaviour
{
    [Header("Paneles de la UI")]
    [SerializeField] private GameObject panelMenuConexion;

    [Header("Botones del Menú")]
    
    [SerializeField] private UnityEngine.UI.Button botonSerHost;
    [SerializeField] private UnityEngine.UI.Button AbrirSubMenuCliente;
    [SerializeField] private UnityEngine.UI.Button botonSalir;

    [Header("Botones de Partida")]
    [SerializeField] private UnityEngine.UI.Button botonDesconectar;

    private void Start()
    {
        
        botonSerHost.onClick.AddListener(IniciarHost);
        AbrirSubMenuCliente.onClick.AddListener(IniciarCliente);
        botonSalir.onClick.AddListener(SalirDelJuego);
        botonDesconectar.onClick.AddListener(Desconectarse);

        botonDesconectar.gameObject.SetActive(false);
        panelMenuConexion.SetActive(true);
    }

    private void IniciarHost()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            OcultarMenuPrincipal();
        }
    }

    private void IniciarCliente()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            OcultarMenuPrincipal();
        }
    }

    private void Desconectarse()
    {
        NetworkManager.Singleton.Shutdown();
        panelMenuConexion.SetActive(true);
        botonDesconectar.gameObject.SetActive(false);
    }

    private void OcultarMenuPrincipal()
    {
        panelMenuConexion.SetActive(false);
        botonDesconectar.gameObject.SetActive(true);
    }

    private void SalirDelJuego()
    {
        Application.Quit();
        Debug.Log("[Menú] Saliendo del juego...");
    }

    private void OnDestroy()
    {
        if (botonSerHost != null) botonSerHost.onClick.RemoveAllListeners();
        if (AbrirSubMenuCliente != null) AbrirSubMenuCliente.onClick.RemoveAllListeners();
        if (botonSalir != null) botonSalir.onClick.RemoveAllListeners();
        if (botonDesconectar != null) botonDesconectar.onClick.RemoveAllListeners();
    }
}