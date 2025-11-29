using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RaidMonitor.Application.Abstractions.Data;
using RaidMonitor.Application.Abstractions.Data.Repositories;
using RaidMonitor.Application.Abstractions.Services;
using RaidMonitor.Application.Commands;
using RaidMonitor.Configuration.Options;
using RaidMonitor.Core.Common;
using RaidMonitor.Core.Models;
using RaidMonitor.Tests.Common;

namespace RaidMonitor.Application.Tests.Commands;

[TestFixture]
public class SendWarningEmailHandlerTests : TestBase
{
    private const string CommandName = "Send Warning Email";

    private Mock<ILogger<SendWarningEmailHandler>> _loggerMock;

    private Mock<IOptions<EmailOptions>> _optionsMock;

    private Mock<IEmailService> _emailServiceMock;

    private Mock<IUserRepository> _userRepositoryMock;

    private Mock<IEventRepository> _eventRepositoryMock;

    private Mock<IApplicationContext> _contextMock;

    private SendWarningEmailHandler _commandHandler;

    private List<string> _requestBadArrays;

    private CancellationToken _cancellationToken;

    private DateTime _timeBeforeTest;

    private int _testActualChanges;

    private string _testBackupEmail;

    private string _testFrom;

    private string _testMessage;

    private List<string> _testRecipients;

    private string _testSubject;

    private List<User> _testUsers;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<SendWarningEmailHandler>>(MockBehavior.Strict);

        _optionsMock = new Mock<IOptions<EmailOptions>>(MockBehavior.Strict);

        _emailServiceMock = new Mock<IEmailService>(MockBehavior.Strict);

        _userRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);

        _eventRepositoryMock = new Mock<IEventRepository>(MockBehavior.Strict);

        _contextMock = new Mock<IApplicationContext>(MockBehavior.Strict);

        _commandHandler = new SendWarningEmailHandler(_loggerMock.Object,
                                                      _optionsMock.Object,
                                                      _emailServiceMock.Object,
                                                      _userRepositoryMock.Object,
                                                      _eventRepositoryMock.Object,
                                                      _contextMock.Object);

        _requestBadArrays =
        [
            "Test bad array 1",
            "Test bad array 2"
        ];

        _cancellationToken = CancellationToken.None;

        _timeBeforeTest = DateTime.UtcNow;

        _testActualChanges = 1;

        _testBackupEmail = "test_backup@email.com";

        _testFrom = "test_from@email.com";

        _testMessage = "Issues found in 2 arrays:<br><pre><code>Test bad array 1</code></pre><br><pre><code>Test bad array 2</code></pre>";

        _testRecipients =
        [
            "test1@email.com",
            "test2@email.com"
        ];

        _testSubject = "Test Subject";

        _testUsers =
        [
            new User
            {
                Id = 123,
                Email = _testRecipients[0]
            },
            new User
            {
                Id = 456,
                Email = _testRecipients[1]
            }
        ];
    }

    [Test]
    public async Task ExecuteAsync_WhenNoUsersFound_ShouldSendToBackupEmail()
    {
        // Arrange
        _testRecipients = ["test_backup@email.com"];

        _testUsers = [];

        Setup_Logger_Starting();

        Setup_Options(3);

        Setup_UserRepository_GetUsersAsync();

        _loggerMock.Setup(LogLevel.Warning, $"Command '{CommandName}' found no users to contact; using backup email '{_testBackupEmail}' instead.");

        Setup_EmailService_SendEmailAsync();

        _eventRepositoryMock
            .Setup(x => x.Add(
                It.Is<Event>(y => _timeBeforeTest <= y.LoggedAt && y.LoggedAt <= DateTime.UtcNow
                             && y.Message == _testMessage
                             && y.UsersNotifiedIds.Count == 0)))
            .Verifiable(Times.Once);

        Setup_Context_SaveChangesAsync();

        Setup_Logger_Completed();

        // Act
        var result = await Act();

        // Assert
        AssertSuccess(result);
    }

    [Test]
    public async Task ExecuteAsync_WhenUsersFound_ShouldSendToBackupEmail()
    {
        // Arrange
        Setup_Logger_Starting();

        Setup_Options(2);

        Setup_UserRepository_GetUsersAsync();

        Setup_EmailService_SendEmailAsync();

        Setup_EventRepository_Add();

        Setup_Context_SaveChangesAsync();

        Setup_Logger_Completed();

        // Act
        var result = await Act();

        // Assert
        AssertSuccess(result);
    }

    private Task<Result> Act() => _commandHandler.ExecuteAsync(_requestBadArrays, _cancellationToken);

    private void Setup_Context_SaveChangesAsync()
    {
        _contextMock
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.FromResult(_testActualChanges))
            .Verifiable(Times.Once);
    }

    private void Setup_EmailService_SendEmailAsync()
    {
        foreach (var recipient in _testRecipients)
        {
            _emailServiceMock
                .Setup(x => x.SendEmailAsync(
                    recipient,
                    _testSubject,
                    _testMessage
                ))
                .Returns(Task.CompletedTask)
                .Verifiable(Times.Once);
        }
    }

    private void Setup_EventRepository_Add()
    {
        _eventRepositoryMock
            .Setup(x => x.Add(
                It.Is<Event>(y => _timeBeforeTest <= y.LoggedAt && y.LoggedAt <= DateTime.UtcNow
                                  && y.Message == _testMessage
                                  && y.UsersNotifiedIds.SequenceEqual(_testUsers.Select(z => z.Id)))))
            .Verifiable(Times.Once);
    }

    private void Setup_Logger_Completed()
    {
        _loggerMock.Setup(LogLevel.Information, $"Command '{CommandName}' completed, sending an email to: {string.Join(", ", _testRecipients)}.");
    }

    private void Setup_Logger_Starting()
    {
        _loggerMock.Setup(LogLevel.Information, $"Command '{CommandName}' started.");
    }

    private void Setup_Options(int times = 1)
    {
        _optionsMock
            .Setup(m => m.Value)
            .Returns(new EmailOptions
            {
                BackupEmail = _testBackupEmail,
                From = _testFrom,
                Subject = _testSubject
            })
            .Verifiable(Times.Exactly(times));
    }

    private void Setup_UserRepository_GetUsersAsync()
    {
        _userRepositoryMock
            .Setup(x => x.GetAllAsync(
                It.Is<CancellationToken>(y => y == _cancellationToken)))
            .ReturnsAsync(_testUsers)
            .Verifiable(Times.Once);
    }
}
