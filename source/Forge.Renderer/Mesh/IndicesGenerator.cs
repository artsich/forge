namespace Forge.Renderer.Utils;

public static class IndicesGenerator
{
    public static uint[] GenerateQuadIndices(int count)
    {
        var quadIndices = new uint[count];

        var offset = 0u;
        for (uint i = 0; i < count; i += 6)
        {
            quadIndices[i + 0] = offset + 0;
            quadIndices[i + 1] = offset + 1;
            quadIndices[i + 2] = offset + 2;

            quadIndices[i + 3] = offset + 2;
            quadIndices[i + 4] = offset + 3;
            quadIndices[i + 5] = offset + 0;

            offset += 4;
        }

        return quadIndices;
    }

	public static uint[] GenerateCircleIndices(int count, int segmentCount)
	{
		var indices = new uint[count * segmentCount * 3];

		for (int i = 0; i < count; i++)
		{
			var offset = (uint)(i * (segmentCount + 1));
            var newFirstCircleIndex = i * segmentCount * 3;

			for (int j = 0; j < segmentCount; j++)
			{
				indices[newFirstCircleIndex + j * 3 + 0] = offset;
				indices[newFirstCircleIndex + j * 3 + 1] = (uint)(offset + j + 1);
                indices[newFirstCircleIndex + j * 3 + 2] = (j == segmentCount - 1) ? offset + 1 : (uint)(offset + j + 2);
			}
		}

		return indices;
	}
}
