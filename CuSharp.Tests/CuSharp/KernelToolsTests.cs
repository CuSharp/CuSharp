using System;
using CuSharp.Kernel;
using CuSharp.Tests.TestHelper;
using Xunit;

namespace CuSharp.Tests.CuSharp
{
    [Collection("Sequential")]
    [Trait(TestCategories.TestCategory, TestCategories.Unit)]
    public class KernelToolsTests
    {
        [Fact]
        public void TestSyncThreads()
        {
            Assert.Throws<NotSupportedException>(() => KernelTools.SyncThreads());

            var exception = Record.Exception(() =>
            {
                KernelTools.CallSyncThreadsAction = () => Console.Write("Syncing now!");
                KernelTools.SyncThreads();
            });
            Assert.Null(exception);
            
        }

        [Fact]
        public void TestBlockIndex()
        {
            Assert.Throws<NotSupportedException>(() => KernelTools.BlockIndex);

            KernelTools.GetBlockIndexAction = () => (42, 42, 42);
            Assert.True((42, 42, 42) == KernelTools.BlockIndex);
        }

        [Fact]
        public void TestThreadIndex()
        {
            Assert.Throws<NotSupportedException>(() => KernelTools.ThreadIndex);

            KernelTools.GetThreadIndexAction = () => (1337, 1337, 1337);
            Assert.True((1337, 1337, 1337) == KernelTools.ThreadIndex);
        }

        [Fact]
        public void TestBlockDimensions()
        {
            Assert.Throws<NotSupportedException>(() => KernelTools.BlockDimension);

            KernelTools.GetBlockDimensionAction = () => (112, 112, 112);
            Assert.True((112, 112, 112) == KernelTools.BlockDimension);
        }
    }
}