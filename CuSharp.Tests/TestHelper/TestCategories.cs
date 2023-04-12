namespace CuSharp.Tests.TestHelper
{
    public class TestCategories
    {
        /// <summary>
        /// Name of the trait in XUnit
        /// </summary>
        public const string TestCategory = "TestCategory";

        /// <summary>
        /// All unit tests which run independently from a compile configuration
        /// </summary>
        public const string Unit = "Unit";

        /// <summary>
        /// All unit tests which only run in debug mode
        /// </summary>
        public const string UnitDebugOnly = "UnitDebugOnly";

        /// <summary>
        /// All unit tests which only run in release mode
        /// </summary>
        public const string UnitReleaseOnly = "UnitReleaseOnly";

        /// <summary>
        /// All tests which requires a Nvidia GPU or some other systems
        /// </summary>
        public const string Integration = "Integration";
    }
}
