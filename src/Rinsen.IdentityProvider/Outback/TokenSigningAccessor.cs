using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Rinsen.Outback;
using Rinsen.Outback.Abstractons;
using Rinsen.Outback.Models;

namespace Rinsen.IdentityProvider.Outback
{
    public class TokenSigningAccessor : ITokenSigningAccessor, IWellKnownSigningAccessor
    {
        public TokenSigningAccessor(RandomStringGenerator randomStringGenerator)
        {
            

        }

        public Task<EllipticCurveJsonWebKeyModelKeys> GetEllipticCurveJsonWebKeyModelKeys()
        {

            //var key = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(ecdSaKey);

            ////_ellipticCurveJsonWebKeyModel = new EllipticCurveJsonWebKeyModel(_key.KeyId, Base64UrlEncoder.Encode(_key.X), Base64UrlEncoder.Encode(_key.Y), JsonWebKeyECTypes.P256, SecurityAlgorithms.EcdsaSha256);
            ////_ellipticCurveJsonWebKeyModel = new EllipticCurveJsonWebKeyModel(_key.KeyId, _key.X, _key.Y, JsonWebKeyECTypes.P256, SecurityAlgorithms.EcdsaSha256);
            //var ellipticCurveJsonWebKeyModel = new EllipticCurveJsonWebKeyModel(ecdSaKey.KeyId, Base64UrlEncoder.Encode(publicKey.Q.X), Base64UrlEncoder.Encode(publicKey.Q.Y), JsonWebKeyECTypes.P256, SecurityAlgorithms.EcdsaSha256);
            throw new NotImplementedException();
            //var key = new JsonWebKey();
            //new EllipticCurveJsonWebKeyModel(_ecdSaKey.KeyId, Base64UrlEncoder.Encode(publicKey.Q.X), Base64UrlEncoder.Encode(publicKey.Q.Y), JsonWebKeyECTypes.P256, SecurityAlgorithms.EcdsaSha256);
        }

        public Task<SecurityKeyWithAlgorithm> GetSigningSecurityKey()
        {
            //var secret = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            //var parameters = secret.ExportParameters(true);

            ////ECCurve.CreateFromOid(new Oid { FriendlyName = "nistP256", Value = "1.2.840.10045.3.1.7" })
            //var para = new ECParameters
            //{
            //    Curve = ECCurve.NamedCurves.nistP256,
            //    D = parameters.D,
            //    Q = new ECPoint
            //    {
            //        X = parameters.Q.X,
            //        Y = parameters.Q.Y
            //    }
            //};

            //var secret2 = ECDsa.Create();
            //secret2.ImportParameters(parameters);
            throw new NotImplementedException();
        }
    }
}
