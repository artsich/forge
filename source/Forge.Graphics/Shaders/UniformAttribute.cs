namespace Forge.Graphics.Shaders;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class UniformAttribute : Attribute
{
	public UniformAttribute(string name)
	{
		Name = name;
	}

	public string Name { get; }
}
