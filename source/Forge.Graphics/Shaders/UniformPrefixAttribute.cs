namespace Forge.Graphics.Shaders;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class UniformPrefixAttribute : Attribute
{
	public UniformPrefixAttribute(string prefix)
	{
		Prefix = prefix;
	}

	public string Prefix { get; }
}
