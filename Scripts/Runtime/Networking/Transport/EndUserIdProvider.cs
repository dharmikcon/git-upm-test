using System;
using Convai.Scripts.Configuration;
using UnityEngine;

namespace Convai.Scripts.Networking.Transport
{
    /// <summary>
    /// Provides a mechanism to obtain or generate an end user ID for Convai services.
    /// Implementations may use device identifiers, user input, or other strategies.
    /// </summary>
    public interface IEndUserIdProvider
    {
        /// <summary>
        /// Gets an existing end user ID from the configuration, or creates a new one if none exists.
        /// </summary>
        /// <param name="configuration">The configuration data object containing the end user ID.</param>
        /// <returns>A normalized end user ID string.</returns>
        string GetOrCreateEndUserId(ConvaiConfigurationDataSO configuration);
    }
    /// <summary>
    /// Default implementation of <see cref="IEndUserIdProvider"/> that uses the device's unique identifier.
    /// If unavailable, generates a new GUID as the end user ID.
    /// </summary>
    public sealed class DeviceEndUserIdProvider : IEndUserIdProvider
    {
        public string GetOrCreateEndUserId(ConvaiConfigurationDataSO configuration)
        {
            string existing = Normalize(configuration?.EndUserId);
            if (!string.IsNullOrEmpty(existing))
            {
                return existing;
            }

            string candidate = SystemInfo.deviceUniqueIdentifier;

            if (string.IsNullOrWhiteSpace(candidate))
            {
                candidate = Guid.NewGuid().ToString();
            }

            string normalized = Normalize(candidate);

            if (configuration != null && !string.Equals(configuration.EndUserId, normalized, StringComparison.Ordinal))
            {
                configuration.EndUserId = normalized;
                configuration.Save();
            }

            return normalized;
        }

        private static string Normalize(string value) =>
            string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }
}

