using Maestro.FactoryProviders;
using Maestro.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Maestro.TypeFactoryResolvers
{
	internal class ConcreteClosedClassFactoryProviderResolver : IFactoryProviderResolver
	{
		public bool TryGet(Type type, string name, Context context, out IFactoryProvider factoryProvider)
		{
			factoryProvider = null;

			if (type.IsArray) return false;
			if (Reflector.IsPrimitive(type)) return false;
			if (!type.IsConcreteClosedClass()) return false;
			if (type.IsConcreteClassClosing(typeof(Func<>))) return false;
			if (type.IsConcreteClassClosing(typeof(Lazy<>))) return false;

			var candidates = new List<Candidate>();

			var ctors = type.GetConstructors();
			for (var i = 0; i < ctors.Length; i++)
			{
				var ctor = ctors[i];
				candidates.Add(new Candidate(ctor));
			}

			candidates.Sort(CandidateComparer.Instance);

			for (var ci = 0; ci < candidates.Count; ci++)
			{
				var candidate = candidates[ci];
				var parameters = candidate.Parameters;
				var match = true;

				for (var pi = 0; pi < parameters.Length; pi++)
				{
					var parameter = parameters[pi];
					if (context.CanGetService(parameter.ParameterType, name)) continue;
					match = false;
					break;
				}

				if (!match) continue;

				factoryProvider = new TypeFactoryProvider(type, name) { Constructor = candidate.Ctor };
				return true;
			}

			return false;
		}

		private class Candidate
		{
			public Candidate(ConstructorInfo ctor)
			{
				Ctor = ctor;
				Parameters = ctor.GetParameters();
			}

			public ConstructorInfo Ctor { get; }
			public ParameterInfo[] Parameters { get; }
		}

		private class CandidateComparer : IComparer<Candidate>
		{
			public static readonly IComparer<Candidate> Instance = new CandidateComparer();

			public int Compare(Candidate x, Candidate y)
			{
				return y.Parameters.Length.CompareTo(x.Parameters.Length);
			}
		}
	}
}