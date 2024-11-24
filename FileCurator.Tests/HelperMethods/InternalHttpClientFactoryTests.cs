using FileCurator.HelperMethods;
using FileCurator.Tests.BaseClasses;
using Mecha.xUnit;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace FileCurator.Tests.HelperMethods
{
    public class InternalHttpClientFactoryTests : TestBaseClass<InternalHttpClientFactory>
    {
        public InternalHttpClientFactoryTests()
        {
            TestObject = new InternalHttpClientFactory();
        }

        [Property]
        public void ClientAlwaysTheSame(Credentials credentials)
        {
            var Result1 = TestObject.GetClient(credentials);
            var Result2 = TestObject.GetClient(credentials);
            Assert.Same(Result1, Result2);
        }

        [Property]
        public void ClientAlwaysTheSameWhenCopied([Required] Credentials credentials)
        {
            var Result1 = TestObject.GetClient(credentials);
            var Result2 = TestObject.GetClient(new Credentials { UseDefaultCredentials = credentials.UseDefaultCredentials, Domain = credentials.Domain, Password = credentials.Password, UserName = credentials.UserName });
            Assert.Same(Result1, Result2);
        }

        [Property]
        public void CredentialsReturnValid(Credentials credentials)
        {
            var Result = TestObject.GetClient(credentials);
            Assert.NotNull(Result);
        }
    }
}