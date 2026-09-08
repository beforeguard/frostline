namespace Beforeguard.Frostline.Core.Tests.CSharp;

using Beforeguard.Frostline.Core;
using Xunit;

public class OptionalLoggerConstructorTests
{
    [Fact]
    public void TokenManager_CanBeConstructed_WithoutLogger()
    {
        var config = ClientConfig.Create("id", "secret", Region.US);

        using var tokenManager = new TokenManager(config);

        Assert.NotNull(tokenManager);
    }

    [Fact]
    public void BattleNetHttpClient_CanBeConstructed_WithoutLogger()
    {
        var config = ClientConfig.Create("id", "secret", Region.US);

        using var httpClient = new BattleNetHttpClient(config);

        Assert.Equal(Region.US, httpClient.Region);
    }
}
