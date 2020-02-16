using System;

namespace PerformanceTest
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    class ParameterAttribute : Attribute
    {
        public ParameterAttribute(string parameterName, string parameterDescription)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            ParameterDescription = parameterDescription;
        }
        public ParameterAttribute(string parameterName) :this(parameterName, null)
        {

        }
        public string ParameterName { get; }
        public string ParameterDescription { get; }
    }
}
