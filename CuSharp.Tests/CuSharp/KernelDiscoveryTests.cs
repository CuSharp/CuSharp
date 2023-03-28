using System.Linq;
using CuSharp.Kernel;
using Xunit;

namespace CuSharp.Tests.CuSharp
{
    [Collection("Sequential")]
    public class KernelDiscoveryTests
    {
        private readonly KernelDiscovery _discovery = new();

        public KernelDiscoveryTests()
        {
            _discovery.ScanAll();
        }

        [Fact]
        public void TestDiscoverAllAnnotated()
        {
            var methods = _discovery.GetAllMethods();
            Assert.Equal(2, methods.Count());
        }
        
        [Fact]
        public void TestNotDiscoverPublicInstanceMethod()
        {
            var classType = typeof(KernelDiscoveryTests);
            Assert.False(_discovery.IsMarked($"{classType.FullName}.PublicInstance"));
        }

        [Fact]
        public void TestNotDiscoverPrivateInstanceMethod()
        {
            var classType = typeof(KernelDiscoveryTests);
            Assert.False(_discovery.IsMarked($"{classType.FullName}.PrivateInstance"));
        }

        [Fact]
        public void TestDiscoverPublicStaticMethod()
        {
            var classType = typeof(KernelDiscoveryTests);
            Assert.True(_discovery.IsMarked($"{classType.FullName}.PublicStatic"));
        }

        [Fact]
        public void TestDiscoverPrivateStaticMethod()
        {
            var classType = typeof(KernelDiscoveryTests);
            Assert.True(_discovery.IsMarked($"{classType.FullName}.PrivateStatic"));
        }

        [Fact]
        public void TestDiscoverGetMethod()
        {
            var classType = typeof(KernelDiscoveryTests);
            var method = _discovery.GetMethod($"{classType.FullName}.PrivateStatic");
            Assert.NotNull(method);
        }

        [Fact]
        public void TestDiscoveryDoesNotFindWrongName()
        {
            var isMethod = _discovery.IsMarked("IAmNotAMethod");
            Assert.False(isMethod);
        }
        
        [Kernel]
        // ReSharper disable once UnusedMember.Global : Test requires this method
        // Non-static methods will not be discovered
        public void PublicInstance()
        { }

        [Kernel]
        // ReSharper disable once UnusedMember.Local : Test requires this method
        // Non-static methods will not be discovered
        private void PrivateInstance()
        { }

        [Kernel]
        // ReSharper disable once UnusedMember.Global : Test requires this method
        public static void PublicStatic()
        { }

        [Kernel]
        // ReSharper disable once UnusedMember.Local : Test requires this method
        private static void PrivateStatic()
        { }
    }
}