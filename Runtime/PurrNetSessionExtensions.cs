using PurrNet.Purrnity;
using Unity.Services.Multiplayer;

namespace PurrNet.MultiplayerServices
{
    /// <summary>
    /// Extension methods for integrating PurrNet with Unity Multiplayer Services sessions.
    /// </summary>
    public static class PurrNetSessionExtensions
    {
        /// <summary>
        /// Configures the session to use PurrNet via <see cref="PurrnityTransport"/>.
        /// Combine with <see cref="SessionOptionsExtensions.WithDirectNetwork{T}(T, string, string, int)"/>
        /// or <see cref="SessionOptionsExtensions.WithRelayNetwork{T}(T, string)"/> to set up the network type.
        /// </summary>
        /// <param name="options">The session options to configure.</param>
        /// <param name="handler">
        /// The <see cref="PurrNetNetworkHandler"/> to use. If null, creates one with automatic transport resolution.
        /// </param>
        /// <typeparam name="T">The options type.</typeparam>
        /// <returns>The configured session options.</returns>
        public static T WithPurrHandler<T>(this T options, PurrNetNetworkHandler handler = null)
            where T : BaseSessionOptions
        {
            return options.WithNetworkHandler(handler ?? new PurrNetNetworkHandler());
        }

        /// <summary>
        /// Configures the session to use PurrNet with direct networking.
        /// Equivalent to <c>WithDirectNetwork(...).WithPurrHandler()</c>.
        /// </summary>
        /// <param name="options">The session options to configure.</param>
        /// <param name="listenIp">Listen for incoming connections at this address (<c>0.0.0.0</c> for all interfaces).</param>
        /// <param name="publishIp">Address that clients should use when connecting.</param>
        /// <param name="port">Port for both listening and client connections. 0 for auto-assign.</param>
        /// <param name="handler">
        /// The <see cref="PurrNetNetworkHandler"/> to use. If null, creates one with automatic transport resolution.
        /// </param>
        /// <typeparam name="T">The options type.</typeparam>
        /// <returns>The configured session options.</returns>
        public static T WithPurrDirect<T>(this T options,
            string listenIp = "127.0.0.1",
            string publishIp = "127.0.0.1",
            int port = 0,
            PurrNetNetworkHandler handler = null)
            where T : SessionOptions
        {
            return options.WithDirectNetwork(listenIp, publishIp, port).WithPurrHandler(handler);
        }

        /// <summary>
        /// Configures the session to use PurrNet with direct networking using <see cref="DirectNetworkOptions"/>.
        /// Equivalent to <c>WithDirectNetwork(networkOptions).WithPurrHandler()</c>.
        /// </summary>
        /// <param name="options">The session options to configure.</param>
        /// <param name="networkOptions">The direct network options.</param>
        /// <param name="handler">
        /// The <see cref="PurrNetNetworkHandler"/> to use. If null, creates one with automatic transport resolution.
        /// </param>
        /// <typeparam name="T">The options type.</typeparam>
        /// <returns>The configured session options.</returns>
        public static T WithPurrDirect<T>(this T options,
            DirectNetworkOptions networkOptions,
            PurrNetNetworkHandler handler = null)
            where T : SessionOptions
        {
            return options.WithDirectNetwork(networkOptions).WithPurrHandler(handler);
        }

        /// <summary>
        /// Configures the session to use PurrNet with Relay networking.
        /// Equivalent to <c>WithRelayNetwork(region).WithPurrHandler()</c>.
        /// </summary>
        /// <param name="options">The session options to configure.</param>
        /// <param name="region">
        /// Force a specific Relay region. If null, auto-selects the lowest latency region via QoS.
        /// </param>
        /// <param name="handler">
        /// The <see cref="PurrNetNetworkHandler"/> to use. If null, creates one with automatic transport resolution.
        /// </param>
        /// <typeparam name="T">The options type.</typeparam>
        /// <returns>The configured session options.</returns>
        public static T WithPurrRelay<T>(this T options,
            string region = null,
            PurrNetNetworkHandler handler = null)
            where T : SessionOptions
        {
            return options.WithRelayNetwork(region).WithPurrHandler(handler);
        }

        /// <summary>
        /// Configures the session to use PurrNet with Relay networking using <see cref="RelayNetworkOptions"/>.
        /// Equivalent to <c>WithRelayNetwork(relayOptions).WithPurrHandler()</c>.
        /// </summary>
        /// <param name="options">The session options to configure.</param>
        /// <param name="relayOptions">The Relay network options including protocol and region.</param>
        /// <param name="handler">
        /// The <see cref="PurrNetNetworkHandler"/> to use. If null, creates one with automatic transport resolution.
        /// </param>
        /// <typeparam name="T">The options type.</typeparam>
        /// <returns>The configured session options.</returns>
        public static T WithPurrRelay<T>(this T options,
            RelayNetworkOptions relayOptions,
            PurrNetNetworkHandler handler = null)
            where T : SessionOptions
        {
            return options.WithRelayNetwork(relayOptions).WithPurrHandler(handler);
        }
    }
}
