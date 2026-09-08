namespace Beforeguard.Frostline.Core.Tests.CSharp;

using Beforeguard.Frostline.Core;
using Microsoft.FSharp.Core;
using Xunit;

public class FrostlineErrorTests
{
    [Fact]
    public void NotFound_Message_IncludesResource()
    {
        var error = FrostlineError.NewNotFound("wow/character/foo");

        Assert.Equal("Not found: wow/character/foo", error.Message);
    }

    [Fact]
    public void Unauthorized_Message_IsThePassedMessage()
    {
        var error = FrostlineError.NewUnauthorized("nope");

        Assert.Equal("nope", error.Message);
    }

    [Fact]
    public void GeneralError_Message_IsThePassedMessage()
    {
        var error = FrostlineError.NewGeneralError("boom", FSharpOption<System.Exception>.None);

        Assert.Equal("boom", error.Message);
    }

    [Fact]
    public void RateLimited_WithRetryAfter_IncludesSeconds()
    {
        var error = FrostlineError.NewRateLimited(FSharpOption<int>.Some(30));

        Assert.Equal("Rate limited, retry after 30s", error.Message);
    }

    [Fact]
    public void RateLimited_WithoutRetryAfter_IsGenericMessage()
    {
        var error = FrostlineError.NewRateLimited(FSharpOption<int>.None);

        Assert.Equal("Rate limited", error.Message);
    }
}
