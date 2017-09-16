using JetBrains.Annotations;
using Synergy.Contracts;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Web;

namespace Synergy.Lerman.Models
{
    public static class UniqueId
    {
        private static int numberOfGeneratedIdentifiers;

        [NotNull, Pure]
        public static string New(string prefix = "")
        {
            string base64Guid = Convert.ToBase64String(Guid.NewGuid()
                                                           .ToByteArray());
            return prefix + HttpUtility.UrlEncode(base64Guid)
                                       .FailIfNull("id")
                                       .Replace("%", "");

            //string id = UniqueId.GenerateNewIdentifier();
            //return id;
        }

        [NotNull, Pure]
        private static string GenerateNewIdentifier()
        {
            int currentCounterValue = Interlocked.Increment(ref UniqueId.numberOfGeneratedIdentifiers);
            DateTime now = DateTime.UtcNow;
            int seed = new CryptoRandom().Next(0, 9999999);
            int uniquifier = (seed + currentCounterValue) % 100000;
            return $"{now:yyyyMMddHHmmssfff}{uniquifier:D5}";
        }
    }

    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class CryptoRandom : Random, ICryptoRandom
    {
        private readonly RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();

        public override int Next()
        {
            var uint32Buffer = new byte[4];
            this.cryptoProvider.GetBytes(uint32Buffer);
            return BitConverter.ToInt32(uint32Buffer, 0) & 0x7FFFFFFF;
        }

        public override int Next(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            return this.Next(0, maxValue);
        }

        public override int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue));
            if (minValue == maxValue)
                return minValue;

            return minValue + (int)((maxValue - minValue) * this.NextDouble());
        }

        public override double NextDouble()
        {
            var uint32Buffer = new byte[4];
            this.cryptoProvider.GetBytes(uint32Buffer);
            uint rand = BitConverter.ToUInt32(uint32Buffer, 0);
            return rand / (1.0 + uint.MaxValue);
        }

        public static int NextIndex(int maxValue)
        {
            return new CryptoRandom().Next(maxValue);
        }
    }

    public interface ICryptoRandom
    {
        int Next(int minValue, int maxValue);
    }
}