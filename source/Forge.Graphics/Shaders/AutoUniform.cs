using System.Linq.Expressions;
using System.Reflection;

namespace Forge.Graphics.Shaders;

internal static class AutoUniform
{
	private readonly static Dictionary<Type, Action<CompiledShader, object>> Cache = new();

	internal static void Bind(object obj, CompiledShader shader)
	{
		var type = obj.GetType();

		if (!Cache.TryGetValue(type, out var binder))
		{
			lock (Cache)
			{
				if (!Cache.TryGetValue(type, out binder))
				{
					binder = ConfigureBinder(type, shader);
					Cache[type] = binder;
				}
			}
		}

		binder(shader, obj);
	}

	private static Action<CompiledShader, object> ConfigureBinder(Type type, CompiledShader shader)
	{
		var prefix = GetUniformPrefix(type);
		var membersWithUniformAttributes = GetMembersWithUniformAttributes(type);
		var binderExpression = BuildUniformBindingExpression(type, prefix, membersWithUniformAttributes, shader);
		return binderExpression.Compile();
	}

	private static string GetUniformPrefix(Type type)
	{
		return type.GetCustomAttribute<UniformPrefixAttribute>()?.Prefix ?? string.Empty;
	}

	private static List<MemberInfo> GetMembersWithUniformAttributes(Type type) =>
		type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(m => m.GetCustomAttribute<UniformAttribute>() != null)
			.ToList();

	private static Expression<Action<CompiledShader, object>> BuildUniformBindingExpression(Type type, string prefix, IEnumerable<MemberInfo> members, CompiledShader shader)
	{
		var shaderParam = Expression.Parameter(typeof(CompiledShader), "shader");
		var objParam = Expression.Parameter(typeof(object), "obj");
		var castedObj = Expression.Convert(objParam, type);

		var expressions = new List<Expression>();

		foreach (var member in members)
		{
			string uniformName = prefix + member.GetCustomAttribute<UniformAttribute>()!.Name;

			if (shader.UniformExists(uniformName))
			{
				Expression getValueExpression = member switch
				{
					FieldInfo field => Expression.Field(castedObj, field),
					PropertyInfo prop => Expression.Property(castedObj, prop),
					_ => throw new InvalidOperationException($"Unsupported member type: {member.MemberType}")
				};

				var setValueExpression = BuildSetUniformExpression(shaderParam, getValueExpression, uniformName, member);

				expressions.Add(setValueExpression);
			}
		}

		return Expression.Lambda<Action<CompiledShader, object>>(
			Expression.Block(expressions),
			shaderParam,
			objParam
		);
	}

	private static MethodCallExpression BuildSetUniformExpression(ParameterExpression shaderParam, Expression getValueExpression, string uniformName, MemberInfo member)
	{
		var itemProperty = typeof(CompiledShader).GetProperty("Item")
						   ?? throw new InvalidOperationException("The indexer seems to be removed or renamed.");

		var setValueMethod = itemProperty.PropertyType.GetMethod(nameof(Uniform.SetValue), new[] { getValueExpression.Type })
							 ?? throw new InvalidOperationException($"No suitable SetValue method found for type: {getValueExpression.Type.Name}");

		var getItemExpression = Expression.Property(shaderParam, itemProperty, Expression.Constant(uniformName));

		return Expression.Call(getItemExpression, setValueMethod, getValueExpression);
	}
}
