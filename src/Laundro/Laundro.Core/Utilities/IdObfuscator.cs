using Sqids;

namespace Laundro.Core.Utilities;

public interface IIdObfuscator
{
    int Decode(string encodedId);
    string Encode(int id);
}

public class IdObfuscator : IIdObfuscator
{
    private static readonly SqidsEncoder<int> _IdsEncoder = new SqidsEncoder<int>(
        new SqidsOptions { MinLength = 10 });

    public string Encode(int id)
    {
        return _IdsEncoder.Encode(id);
    }

    public int Decode(string encodedId)
    {
        var decoded = _IdsEncoder.Decode(encodedId);
        return decoded[0];
    }
}
