#if TEST_FRAMEWORK

using NUnit.Framework;
using UnityEditor;

namespace DTT.PaginatedScrollView.Tests
{
    /// <summary>
    /// Class that contains tests for the <see cref="PaginatedScrollRect"/>.
    /// </summary>
    public class Test_PaginatedScrollRect
    {
        /// <summary>
        /// Tests the creating of a <see cref="PaginatedScrollRect"/>
        /// </summary>
        [Test]
        public void Test_CreateScrollRectFromMenuItem()
        {
            // Act.
            bool result = EditorApplication.ExecuteMenuItem("GameObject/UI/Paginated Scroll Rect");

            // Assert.
            Assert.IsTrue(result);
        }
    }
}

#endif
