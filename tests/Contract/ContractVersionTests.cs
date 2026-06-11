using ITSupportNative.Contracts;

namespace ITSupportNative.ContractTests;

public sealed class ContractVersionTests
{
    [Fact]
    public void CurrentStartsAtVersionOne()
    {
        Assert.Equal(1, ContractVersion.Current);
    }
}
