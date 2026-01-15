using Convai.Scripts.Configuration;
using NUnit.Framework;
using UnityEngine;

namespace Convai.Tests.PlayMode
{
    public class ConvaiConfigurationDataTests
    {
        [Test]
        public void Copy_PreservesEndUserId()
        {
            ConvaiConfigurationDataSO original = ScriptableObject.CreateInstance<ConvaiConfigurationDataSO>();
            original.APIKey = "test-api-key";
            original.PlayerName = "Player";
            original.EndUserId = "end-user-123";

            ConvaiConfigurationDataSO copy = ConvaiConfigurationDataSO.Copy(original);

            Assert.AreEqual(original.APIKey, copy.APIKey);
            Assert.AreEqual(original.PlayerName, copy.PlayerName);
            Assert.AreEqual(original.EndUserId, copy.EndUserId);
        }

        [Test]
        public void Load_CopiesEndUserId()
        {
            ConvaiConfigurationDataSO source = ScriptableObject.CreateInstance<ConvaiConfigurationDataSO>();
            source.APIKey = "test-api-key";
            source.PlayerName = "Player";
            source.EndUserId = "end-user-abc";

            ConvaiConfigurationDataSO target = ScriptableObject.CreateInstance<ConvaiConfigurationDataSO>();
            target.Load(source);

            Assert.AreEqual(source.APIKey, target.APIKey);
            Assert.AreEqual(source.PlayerName, target.PlayerName);
            Assert.AreEqual(source.EndUserId, target.EndUserId);
        }
    }
}

