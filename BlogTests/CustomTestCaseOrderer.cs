using System.Collections.Generic;
using Utilities.Extensions;
using Xunit.Sdk;
using System.Collections.Concurrent;
using Xunit.Abstractions;
using System.Linq;
using System.Reflection;

namespace BlogTests
{
    /// <summary>
    /// Custom xUnit test case orderer that uses the OrderAttribute
    /// </summary>
    public class CustomTestCaseOrderer : ITestCaseOrderer
    {
        public const string TypeName = "BlogTests.CustomTestCaseOrderer";
        public const string AssembyName = "BlogTests";

        public static readonly ConcurrentDictionary<string, ConcurrentQueue<string>> QueuedTests 
            = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            return testCases.OrderBy(getOrder);
        }

        static int getOrder<TTestCase>(TTestCase testCase)
            where TTestCase : ITestCase
        {
            // Enqueue the test name.
            QueuedTests
                .GetOrAdd(
                    testCase.TestMethod.TestClass.Class.Name,
                    key => new ConcurrentQueue<string>())
                .Enqueue(testCase.TestMethod.Method.Name);

            // Order the test based on the attribute.
            var attr = testCase.TestMethod.Method
                .ToRuntimeMethod()
                .GetCustomAttribute<OrderAttribute>();
            return attr?.I ?? 0;
        }
    }
}
