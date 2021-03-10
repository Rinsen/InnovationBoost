using System;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rinsen.IdentityProvider.Outback.Entities;
using Rinsen.Outback.Abstractons;
using Rinsen.Outback.Models;

namespace Rinsen.IdentityProvider.Outback
{
    public class TokenSigningAccessor : ITokenSigningAccessor, IWellKnownSigningAccessor
    {
        private readonly OutbackDbContext _outbackDbContext;

        public TokenSigningAccessor(OutbackDbContext outbackDbContext)
        {
            _outbackDbContext = outbackDbContext;
        }

        public async Task<EllipticCurveJsonWebKeyModelKeys> GetEllipticCurveJsonWebKeyModelKeys()
        {
            var secrets = await _outbackDbContext.Secrets.ToListAsync();

            var result = new EllipticCurveJsonWebKeyModelKeys();
            foreach (var secret in secrets)
            {
                switch (secret.PublicKeyCryptographyType)
                {
                    case PublicKeyCryptographyType.EC_NistP256:
                        var parameters = JsonSerializer.Deserialize<CryptographyParameters>(secret.CryptographyData);

                        result.Keys.Add(new EllipticCurveJsonWebKeyModel(parameters.KeyId, parameters.EncodedX, parameters.EncodedY, JsonWebKeyECTypes.P256, SecurityAlgorithms.EcdsaSha256));
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public async Task<SecurityKeyWithAlgorithm> GetSigningSecurityKey()
        {
            var secret = await _outbackDbContext.Secrets.SingleOrDefaultAsync(m => m.ActiveSigningKey == true);

            switch (secret.PublicKeyCryptographyType)
            {
                case PublicKeyCryptographyType.EC_NistP256:
                    var storedParameters = JsonSerializer.Deserialize<CryptographyParameters>(secret.CryptographyData);
                    var ecParameters = new ECParameters
                    {
                        Curve = ECCurve.NamedCurves.nistP256,
                        D = Base64UrlEncoder.DecodeBytes(storedParameters.EncodedD),
                        Q = new ECPoint
                        {
                            X = Base64UrlEncoder.DecodeBytes(storedParameters.EncodedX),
                            Y = Base64UrlEncoder.DecodeBytes(storedParameters.EncodedY)
                        }
                    };

                    var ecdsa = ECDsa.Create();
                    ecdsa.ImportParameters(ecParameters);

                    var ecdSaKey = new ECDsaSecurityKey(ecdsa)
                    {
                        KeyId = storedParameters.KeyId
                    };

                    return new SecurityKeyWithAlgorithm(JsonWebKeyConverter.ConvertFromECDsaSecurityKey(ecdSaKey), SecurityAlgorithms.EcdsaSha256);
                default:
                    throw new Exception($"{secret.PublicKeyCryptographyType} is not supported");
            }
        }
    }

    public class CryptographyParameters
    {
        public string KeyId { get; set; }

        public string EncodedD { get; set; }

        public string EncodedX { get; set; }

        public string EncodedY { get; set; }

    }
}
