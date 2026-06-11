using ITSupportNative.BuildingBlocks;

namespace ITSupportNative.UnitTests;

public sealed class ProductInfoTests
{
    [Fact]
    public void NameUsesProductIdentity()
    {
        Assert.Equal("IT Support Native", ProductInfo.Name);
    }
}
