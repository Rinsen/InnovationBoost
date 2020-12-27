using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Rinsen.Outback;
using Rinsen.Outback.Abstractons;
using Rinsen.Outback.Models;

namespace Rinsen.OAuth.InMemoryStorage
{
    public class SecretStorage : ITokenSigningStorage, IWellKnownSigningStorage
    {
        private readonly EllipticCurveJsonWebKeyModel _ellipticCurveJsonWebKeyModel;
        private readonly JsonWebKey _key;
        private readonly ECDsaSecurityKey _ecdSaKey;

        public SecretStorage(RandomStringGenerator randomStringGenerator)
        {
            var secret = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            _ecdSaKey = new ECDsaSecurityKey(secret)
            {
                KeyId = randomStringGenerator.GetRandomString(20)
            };

            _key = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(_ecdSaKey);
            var publicKey = _ecdSaKey.ECDsa.ExportParameters(false);

            //_ellipticCurveJsonWebKeyModel = new EllipticCurveJsonWebKeyModel(_key.KeyId, Base64UrlEncoder.Encode(_key.X), Base64UrlEncoder.Encode(_key.Y), JsonWebKeyECTypes.P256, SecurityAlgorithms.EcdsaSha256);
            //_ellipticCurveJsonWebKeyModel = new EllipticCurveJsonWebKeyModel(_key.KeyId, _key.X, _key.Y, JsonWebKeyECTypes.P256, SecurityAlgorithms.EcdsaSha256);
            _ellipticCurveJsonWebKeyModel = new EllipticCurveJsonWebKeyModel(_ecdSaKey.KeyId, Base64UrlEncoder.Encode(publicKey.Q.X), Base64UrlEncoder.Encode(publicKey.Q.Y), JsonWebKeyECTypes.P256, SecurityAlgorithms.EcdsaSha256);
        }

        public Task<SecurityKey> GetSigningSecurityKey()
        {
            return Task.FromResult((SecurityKey)_key);
        }

        public Task<string> GetSigningAlgorithm()
        {
            return Task.FromResult(SecurityAlgorithms.EcdsaSha256);
        }

        public Task<EllipticCurveJsonWebKeyModelKeys> GetEllipticCurveJsonWebKeyModelKeys()
        {
            return Task.FromResult(new EllipticCurveJsonWebKeyModelKeys { Keys = new List<EllipticCurveJsonWebKeyModel> { _ellipticCurveJsonWebKeyModel } });
        }
    }
}
