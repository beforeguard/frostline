namespace Beforeguard.Frostline.Core.Tests.CSharp;

using Beforeguard.Frostline.Core;
using Xunit;

public class RegionExtensionsTests
{
    [Fact]
    public void US_MapsToUsHostname() => Assert.Equal("us.api.blizzard.com", Region.US.ToHostname());

    [Fact]
    public void EU_MapsToEuHostname() => Assert.Equal("eu.api.blizzard.com", Region.EU.ToHostname());

    [Fact]
    public void KR_MapsToKrHostname() => Assert.Equal("kr.api.blizzard.com", Region.KR.ToHostname());

    [Fact]
    public void TW_MapsToTwHostname() => Assert.Equal("tw.api.blizzard.com", Region.TW.ToHostname());

    [Fact]
    public void CN_MapsToChinaGatewayHostname() => Assert.Equal("gateway.battlenet.com.cn", Region.CN.ToHostname());
}
