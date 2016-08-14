using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaoUtils.Standard;
using DaoUtilsCore.core;

namespace DaoUtils.core
{
    abstract internal class AbstractParametersReadHelper<T> : IReadHelper<T> where T : IReadValue
    {
        private readonly T[] _parameters;
        private readonly Dictionary<string, T> _paramReaders;
        protected AbstractParametersReadHelper(IEnumerable<IDaoParameterInternal> parameters)
        {
            var parms = parameters.Select(p => new {param = ParamReader(p), name = p.Name}).ToArray();
            _parameters = parms.Select(p => p.param).ToArray();
            _paramReaders = parms.ToDictionary(p => p.name, p => p.param, StringComparer.InvariantCultureIgnoreCase);
        }

        protected abstract T ParamReader(IDaoParameterInternal parameter);

        public T Named(string name)
        {
            var key = name?.ToLower() ?? "<NULL>";
            if (!_paramReaders.ContainsKey(key)) throw new DaoUtilsException($"Unknown Parameter '{name}'");
            return _paramReaders[key];
        }

        public T this[string name] => Named(name);

        T IReadHelper<T>.this[int columIndex] => _parameters[columIndex];
    }
}
