using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ConexionIPDirecta : MonoBehaviour
{
    [Header("Paneles del Menú")]
    [SerializeField] private GameObject panelMenuPrincipal; 
    [SerializeField] private GameObject subMenuCliente;      

    [Header("UI de Conexión")]
    [SerializeField] private TMP_InputField inputIPCliente;   

    private UnityTransport transport;

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        }

       
        if (subMenuCliente != null)
        {
            subMenuCliente.SetActive(false);
        }
    }

  
    public void AbrirSubMenuCliente()
    {
        if (subMenuCliente != null)
        {
            subMenuCliente.SetActive(true);
        }

       
        if (panelMenuPrincipal != null)
        {
            panelMenuPrincipal.SetActive(false);
        }
    }


    public void VolverAlMenuPrincipal()
    {
        if (subMenuCliente != null) subMenuCliente.SetActive(false);
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(true);
    }


    public void BotonSerHost()
    {
        if (NetworkManager.Singleton == null || transport == null) return;

      
        OcultarTodosLosMenus();

        transport.SetConnectionData("0.0.0.0", 7777);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("EscenaJuego", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

 
    public void BotonConectarCliente()
    {
        if (NetworkManager.Singleton == null || transport == null) return;

        string ipEscrita = inputIPCliente.text.Trim();

        if (string.IsNullOrEmpty(ipEscrita))
        {
            ipEscrita = "127.0.0.1";
        }

       
        OcultarTodosLosMenus();

        transport.SetConnectionData(ipEscrita, 7777);
        NetworkManager.Singleton.StartClient();
    }

    
    private void OcultarTodosLosMenus()
    {
        if (subMenuCliente != null) subMenuCliente.SetActive(false);
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(false);
    }
}
