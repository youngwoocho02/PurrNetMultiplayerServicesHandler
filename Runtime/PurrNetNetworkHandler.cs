using System.Threading.Tasks;
using PurrNet.Purrnity;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace PurrNet.MultiplayerServices
{
    /// <summary>
    /// An <see cref="INetworkHandler"/> for PurrNet using <see cref="PurrnityTransport"/>.
    /// Register via <c>SessionOptions.WithPurrHandler()</c> or <c>SessionOptions.WithNetworkHandler(new PurrNetNetworkHandler())</c>.
    /// </summary>
    public class PurrNetNetworkHandler : INetworkHandler
    {
        const string k_Tag = nameof(PurrNetNetworkHandler);

        PurrnityTransport m_Transport;
        NetworkConfiguration m_Configuration;

        /// <summary>
        /// Creates a new <see cref="PurrNetNetworkHandler"/>.
        /// Transport is resolved automatically from <see cref="NetworkManager.main"/> or the scene.
        /// </summary>
        public PurrNetNetworkHandler() { }

        /// <summary>
        /// Creates a new <see cref="PurrNetNetworkHandler"/> with the specified transport.
        /// </summary>
        /// <param name="transport">The <see cref="PurrnityTransport"/> instance to use.</param>
        public PurrNetNetworkHandler(PurrnityTransport transport)
        {
            m_Transport = transport;
        }

        PurrnityTransport ResolveTransport()
        {
            if (m_Transport != null)
                return m_Transport;

            var networkManager = NetworkManager.main;
            if (networkManager != null && networkManager.transport is PurrnityTransport purrnityTransport)
            {
                m_Transport = purrnityTransport;
                return m_Transport;
            }

            m_Transport = Object.FindFirstObjectByType<PurrnityTransport>();

            if (m_Transport == null)
            {
                throw new System.InvalidOperationException(
                    $"No {nameof(PurrnityTransport)} found. " +
                    "Either pass one explicitly, assign it to NetworkManager, or ensure one exists in the scene.");
            }

            return m_Transport;
        }

        /// <inheritdoc/>
        public Task StartAsync(NetworkConfiguration configuration)
        {
            var transport = ResolveTransport();
            Debug.Log($"[{k_Tag}] StartAsync - Role: {configuration.Role}, Type: {configuration.Type}");

            m_Configuration = configuration;

            switch (configuration.Type)
            {
                case NetworkType.Direct:
                    SetupDirect(transport, configuration);
                    break;
                case NetworkType.Relay:
                    SetupRelay(transport, configuration);
                    break;
                default:
                    return Task.FromException(
                        new System.NotSupportedException($"Network type '{configuration.Type}' is not supported by PurrNet."));
            }

            switch (configuration.Role)
            {
                case NetworkRole.Server:
                case NetworkRole.Host:
                    transport.StartServer();
                    Debug.Log($"[{k_Tag}] Server started.");
                    break;
                case NetworkRole.Client:
                    transport.StartClient();
                    Debug.Log($"[{k_Tag}] Client started.");
                    break;
            }

            // When binding to port 0, the OS chooses a random port.
            // Update the configuration with the actual port.
            if (configuration.Type == NetworkType.Direct &&
                configuration.Role != NetworkRole.Client &&
                configuration.DirectNetworkListenAddress.Port == 0)
            {
                var localEndpoint = transport.GetLocalEndpoint();
                configuration.UpdatePublishPort(localEndpoint.Port);
                Debug.Log($"[{k_Tag}] Port updated to {localEndpoint.Port}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync()
        {
            Debug.Log($"[{k_Tag}] StopAsync - Role: {m_Configuration?.Role}");

            if (m_Transport == null || m_Configuration == null)
            {
                return Task.CompletedTask;
            }

            if (m_Configuration.Role == NetworkRole.Client)
            {
                m_Transport.Disconnect();
                Debug.Log($"[{k_Tag}] Client disconnected.");
            }
            else
            {
                m_Transport.StopListening();
                Debug.Log($"[{k_Tag}] Server stopped.");
            }

            m_Configuration = null;
            return Task.CompletedTask;
        }

        static void SetupDirect(PurrnityTransport transport, NetworkConfiguration configuration)
        {
            Debug.Log($"[{k_Tag}] SetupDirect - Publish: {configuration.DirectNetworkPublishAddress}, Listen: {configuration.DirectNetworkListenAddress}");
            transport.SetConnectionData(
                configuration.DirectNetworkPublishAddress,
                configuration.DirectNetworkListenAddress);
        }

        static void SetupRelay(PurrnityTransport transport, NetworkConfiguration configuration)
        {
            if (configuration.Role == NetworkRole.Client)
            {
                transport.SetRelayServerData(configuration.RelayClientData);
                Debug.Log($"[{k_Tag}] SetupRelay (Client) - Endpoint: {configuration.RelayClientData.Endpoint}");
            }
            else
            {
                transport.SetRelayServerData(configuration.RelayServerData);
                Debug.Log($"[{k_Tag}] SetupRelay (Server) - Endpoint: {configuration.RelayServerData.Endpoint}");
            }
        }
    }
}
