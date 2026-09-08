namespace Beforeguard.Frostline.Core.Tests.CSharp;

using Beforeguard.Frostline.Core;
using Microsoft.FSharp.Core;
using Xunit;

public class ResultExtensionsTests
{
    [Fact]
    public void Match_OnOk_InvokesOkBranch()
    {
        var result = FSharpResult<int, FrostlineError>.NewOk(42);

        var output = result.Match(
            onOk: value => $"ok:{value}",
            onError: err => $"error:{err.Message}");

        Assert.Equal("ok:42", output);
    }

    [Fact]
    public void Match_OnError_InvokesErrorBranch()
    {
        var result = FSharpResult<int, FrostlineError>.NewError(FrostlineError.NewNotFound("x"));

        var output = result.Match(
            onOk: value => $"ok:{value}",
            onError: err => $"error:{err.Message}");

        Assert.Equal("error:Not found: x", output);
    }

    [Fact]
    public void TryGetValue_OnOk_ReturnsTrueAndValue()
    {
        var result = FSharpResult<int, FrostlineError>.NewOk(7);

        var success = result.TryGetValue(out var value);

        Assert.True(success);
        Assert.Equal(7, value);
    }

    [Fact]
    public void TryGetValue_OnError_ReturnsFalse()
    {
        var result = FSharpResult<int, FrostlineError>.NewError(FrostlineError.NewUnauthorized("nope"));

        var success = result.TryGetValue(out var value);

        Assert.False(success);
    }
}
