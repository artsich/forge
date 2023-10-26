namespace Forge.Renderer.Font;

public readonly struct KerningKey
{
	private readonly int a;
	private readonly int b;

	public KerningKey(char a, char b)
		: this((int)a, (int)b)
	{
	}

	public KerningKey(int a, int b)
	{
		this.a = a;
		this.b = b;
	}

	public override bool Equals(object? obj)
	{
		if (obj is KerningKey other)
		{
			return GetKey() == other.GetKey();
		}
		return false;
	}

	public override int GetHashCode()
	{
		return GetKey().GetHashCode();
	}

	private int GetKey() => (a << 16) | b;

	public static bool operator ==(KerningKey left, KerningKey right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(KerningKey left, KerningKey right)
	{
		return !(left == right);
	}
}
