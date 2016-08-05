﻿namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Configuration.AdvanceExtensibility;
    using Features;
    using Routing;
    using Settings;
    using Transport;

    /// <summary>
    /// Exposes settings related to routing.
    /// </summary>
    public class RoutingSettings : ExposeSettings
    {
        internal RoutingSettings(SettingsHolder settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Adds a static unicast route.
        /// </summary>
        /// <param name="messageType">Message type.</param>
        /// <param name="destination">Destination endpoint.</param>
        public void RouteToEndpoint(Type messageType, string destination)
        {
            ThrowOnAddress(destination);

            Settings.GetOrCreate<ConfiguredUnicastRoutes>().Add(knownMessageTypes => new List<RouteTableEntry>
            {
                new RouteTableEntry(messageType, UnicastRoute.CreateFromEndpointName(destination),  RoutePriority.SpecificType)
            });
        }

        /// <summary>
        /// Adds a static unicast route for all types contained in the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly whose messages should be routed.</param>
        /// <param name="destination">Destination endpoint.</param>
        public void RouteToEndpoint(Assembly assembly, string destination)
        {
            ThrowOnAddress(destination);

            Settings.GetOrCreate<ConfiguredUnicastRoutes>().Add(knownMessageTypes => knownMessageTypes.Where(t => t.Assembly == assembly).Select(t => new RouteTableEntry(t, UnicastRoute.CreateFromEndpointName(destination), RoutePriority.SpecificAssembly)));
        }

        /// <summary>
        /// Adds a static unicast route for all types contained in the specified assembly within the given namespace.
        /// </summary>
        /// <param name="assembly">The assembly whose messages should be routed.</param>
        /// <param name="namespace">The namespace of the messages which should be routed.</param>
        /// <param name="destination">Destination endpoint.</param>
        public void RouteToEndpoint(Assembly assembly, string @namespace, string destination)
        {
            ThrowOnAddress(destination);

            // empty namespace is null, not string.empty
            @namespace = @namespace == string.Empty ? null : @namespace;

            Settings.GetOrCreate<ConfiguredUnicastRoutes>().Add(knownMessageTypes => knownMessageTypes.Where(t => t.Assembly == assembly && t.Namespace == @namespace).Select(t => new RouteTableEntry(t, UnicastRoute.CreateFromEndpointName(destination), RoutePriority.SpecificAssembly)));
        }

        static void ThrowOnAddress(string destination)
        {
            if (destination.Contains("@"))
            {
                throw new ArgumentException($"A logical endpoint name should not contain '@', but received '{destination}'. To specify an endpoint's address, use the instance mapping file for the MSMQ transport, or refer to the routing documentation.");
            }
        }
    }

    /// <summary>
    /// Exposes settings related to routing.
    /// </summary>
    public class RoutingSettings<T> : RoutingSettings
        where T : TransportDefinition
    {
        internal RoutingSettings(SettingsHolder settings)
            : base(settings)
        {
        }
    }
}