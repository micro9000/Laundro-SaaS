using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laundro.UnitTests.Core.Authentication;

[CollectionDefinition("AuthenticationTestsCollection")]
public class AuthenticationTestsCollection : ICollectionFixture<AuthenticationTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
