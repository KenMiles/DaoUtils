using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestsDaoUtilsCore.testHelpers
{
    class CheckArrays
    {
        public static void CheckSameValues<T>(string message, IEnumerable<T> actual, params T[] expected)
        {
            CollectionAssert.AreEquivalent(expected, actual.ToArray(), message);
        }

        public static void CheckSameValues<T>(string message, IEnumerable<T> actual, IEnumerable<T> expected)
        {
            CheckSameValues(message, actual, expected.ToArray());
        }

        public static void CheckSameValues<T>(IEnumerable<T> actual, params T[] expected)
        {
            CollectionAssert.AreEquivalent(expected, actual.ToArray());
        }

        public static void CheckEqual<T>(string message, IEnumerable<T> actual, params string[] expected)
        {
            var actualLines = string.Join("\n", actual.Select(a => $"{a}"));
            var expectedLines = string.Join("\n", expected);
            Assert.AreEqual(expectedLines, actualLines, message);
        }

        public static void CheckEqual<T>(string message, IEnumerable<T> actual, IEnumerable<string> expected)
        {
            CheckEqual(message, actual, expected.ToArray());
        }
    }
}
