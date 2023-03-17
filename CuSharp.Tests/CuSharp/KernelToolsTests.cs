using System;
using Xunit;

namespace CuSharp.Tests
{
    public class KernelToolsTests
    {
        [Fact]
        public void TestSyncThreadsThrows()
        {
            Assert.Throws<NotSupportedException>(() => KernelTools.SyncThreads());
        }

        [Fact]
        public void TestBlockIndexThrows()
        {
            Assert.Throws<NotSupportedException>(() => KernelTools.BlockIndex);
        }

        [Fact]
        public void TestThreadIndexThrows()
        {
            Assert.Throws<NotSupportedException>(() => KernelTools.ThreadIndex);
        }

        [Fact]
        public void TestBlockDimensionsThrows()
        {
            Assert.Throws<NotSupportedException>(() => KernelTools.BlockDimensions);
        }

        [Fact]
        public void TestSyncThreadsOverwriteDoesNotThrow()
        {
            KernelTools.CallSyncThreadsAction = () => Console.Write("Syncing now!");
            KernelTools.SyncThreads();
        }

        [Fact]
        public void TestBlockIndexOverwrite()
        {
            KernelTools.GetBlockIndexAction = () => (42, 42, 42);
            Assert.True((42,42,42) == KernelTools.BlockIndex);
        }

        [Fact]
        public void TestThreadIndexOverwrite()
        {
            KernelTools.GetThreadIndexAction = () => (1337, 1337, 1337);
            Assert.True((1337,1337,1337) == KernelTools.ThreadIndex);
        }

        [Fact]
        public void TestBlockDimensionsOverwrite()
        {
            KernelTools.GetBlockDimensionsAction = () => (112, 112,112);
            Assert.True((112,112,112) == KernelTools.BlockDimensions);
        }
    }
}