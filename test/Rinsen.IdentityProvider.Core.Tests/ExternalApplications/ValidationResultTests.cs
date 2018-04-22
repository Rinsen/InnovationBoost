using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Rinsen.IdentityProvider.ExternalApplications
{
    public class ValidationResultTests
    {
        [Fact]
        public void WhenCreateSucessValidationResult_GetValidStateWithToken()
        {
            var token = "MyToken";
            var validationResult = ValidationResult.Success(token);

            Assert.Equal(token, validationResult.Token);
            Assert.True(validationResult.Succeeded);
            Assert.False(validationResult.Failed);
        }

        [Fact]
        public void WhenCreateFailedValidationResult_GetFailedStateWithNullToken()
        {
            var validationResult = ValidationResult.Failure();

            Assert.Null(validationResult.Token);
            Assert.False(validationResult.Succeeded);
            Assert.True(validationResult.Failed);
        }
    }
}
