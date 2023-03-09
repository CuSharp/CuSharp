using System.Linq;
using Xunit;

namespace CuSharp.Tests
{
    public class KernelDiscoveryTests
    {
        private KernelDiscovery discovery = new KernelDiscovery();

        public KernelDiscoveryTests()
        {
            discovery.ScanAll();
        }

        [Fact]
        public void TestDiscoverAllAnnotated()
        {
            var methods = discovery.GetAllMethods();
            Assert.Equal(4, methods.Count());
        }
        
        [Fact]
        public void TestDiscoverPublicInstanceMethod()
        {
            Assert.True(discovery.IsMarked("CuSharp.Tests.KernelDiscoveryTests.PublicInstance"));
        }

        [Fact]
        public void TestDiscoverPrivateInstanceMethod()
        {
            Assert.True(discovery.IsMarked("CuSharp.Tests.KernelDiscoveryTests.PrivateInstance"));
        }

        [Fact]
        public void TestDiscoverPublicStaticMethod()
        {
            Assert.True(discovery.IsMarked("CuSharp.Tests.KernelDiscoveryTests.PublicStatic"));
        }

        [Fact]
        public void TestDiscoverPrivateStaticMethod()
        {
            Assert.True(discovery.IsMarked("CuSharp.Tests.KernelDiscoveryTests.PrivateStatic"));
        }

        [Fact]
        public void TestDiscoverGetMethod()
        {
            var method = discovery.GetMethod("CuSharp.Tests.KernelDiscoveryTests.PrivateStatic");
            Assert.NotNull(method);
        }

        [Fact]
        public void TestDiscoveryDoesNotFindWrongName()
        {
            var isMethod = discovery.IsMarked("IAmNotAMethod");
            Assert.False(isMethod);
        }
        
        [Kernel]
        public void PublicInstance()
        { }

        [Kernel]
        private void PrivateInstance()
        { }

        [Kernel]
        public static void PublicStatic()
        { }

        [Kernel]
        private static void PrivateStatic()
        { }
    }
}