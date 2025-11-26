using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RaidMonitor.Application.Queries;
using RaidMonitor.Application.Services;
using RaidMonitor.Configuration.Options;
using RaidMonitor.Core.Common;
using RaidMonitor.Tests.Common;

namespace RaidMonitor.Application.Tests.Queries;

[TestFixture]
public class GetRaidStatusHandlerTests : TestBase
{
    private const string QueryName = "Get RAID Status";

    private Mock<ILogger<GetRaidStatusHandler>> _loggerMock;

    private Mock<IOptions<RaidIssueOptions>> _optionsMock;

    private Mock<IFileService> _fileServiceMock;

    private GetRaidStatusHandler _queryHandler;

    private CancellationToken _cancellationToken;

    private string[] _testFileContent;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<GetRaidStatusHandler>>(MockBehavior.Strict);

        _optionsMock = new Mock<IOptions<RaidIssueOptions>>(MockBehavior.Strict);

        _fileServiceMock = new Mock<IFileService>(MockBehavior.Strict);

        _queryHandler = new GetRaidStatusHandler(_loggerMock.Object, _optionsMock.Object, _fileServiceMock.Object);

        _cancellationToken = CancellationToken.None;

        _testFileContent =
        [
            "Personalities : [raid1] [linear] [multipath] [raid0] [raid6] [raid5] [raid4] [raid10]",
            "md123: active raid1 sda[1] sdb[2]",
            "12345678 blocks [2/2] [UU]",
            "",
            "md457: active raid0 sdc[1] sdd[2] sde[3]",
            "12345678 blocks [2/2] [UUU]",
            "",
            "unused devices: <none>"
        ];
    }

    [Test]
    public async Task ExecuteAsync_WhenNoBadRaidArraysFound_ShouldReturnEmptyList()
    {
        // Arrange
        Setup_Options(2);

        Setup_FileService_ReadProcMdstatAsync();

        // Act
        var result = await Act();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            AssertSuccess(result);

            Assert.That(result.Value, Is.Empty);
        }
    }

    [Test]
    public async Task ExecuteAsync_WhenBadArraysFound_ShouldReturnList()
    {
        // Arrange
        _testFileContent = [
            "Personalities : [raid1] [linear] [multipath] [raid0] [raid6] [raid5] [raid4] [raid10]",
            "md123: active raid1 sda[1] sdb[2]",
            "12345678 blocks [2/2] [UU]",
            "",
            "md457: active raid0 sdc[1] sdd[2] sde[3]",
            "12345678 blocks [2/2] [UU_]",
            "",
            "",
            "md678: active raid0 sdc[1] sdd[2](F) sde[3]",
            "12345678 blocks [2/2] [U_U]",
            "",
            "unused devices: <none>"
        ];

        var badArrays = new List<string> {
            $"md457: active raid0 sdc[1] sdd[2] sde[3]{Environment.NewLine}12345678 blocks [2/2] [UU_]",
            $"md678: active raid0 sdc[1] sdd[2](F) sde[3]{Environment.NewLine}12345678 blocks [2/2] [U_U]"
        };

        Setup_Options(3);

        Setup_FileService_ReadProcMdstatAsync();

        _loggerMock.Setup(LogLevel.Information, $"Query '{QueryName}' found '2' bad arrays: {Environment.NewLine}{string.Join(Environment.NewLine, badArrays)}");

        // Act
        var result = await Act();

        // Assert
        AssertSuccess(result);

        Assert.That(result.Value, Is.EquivalentTo(badArrays));
    }

    private Task<Result<List<string>>> Act() => _queryHandler.ExecuteAsync(_cancellationToken);

    private void Setup_FileService_ReadProcMdstatAsync()
    {
        _fileServiceMock
            .Setup(x => x.ReadProcMdstatAsync(
                It.Is<CancellationToken>(y => y == CancellationToken.None)))
            .ReturnsAsync(_testFileContent)
            .Verifiable(Times.Once);
    }

    private void Setup_Options(int times = 1)
    {
        _optionsMock
            .Setup(m => m.Value)
            .Returns(new RaidIssueOptions()
            {
                Keywords = ["(F)", "_"]
            })
            .Verifiable(Times.Exactly(times));
    }
}
