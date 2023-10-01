namespace Forge.Renderer.Utils;

public static class ModelUtils
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
}
