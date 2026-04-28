using CMS.IdentityService.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CMS.Tests.Unit.Domain;

[TestFixture]
[Category("UC-USR-001")]
public class UserDomainTests
{
    [Test]
    public void Create_WithValidData_SetsAllPropertiesCorrectly()
    {
        var user = User.Create("john@example.com", "hashedpwd", "John", "Doe");

        user.Email.Should().Be("john@example.com");
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.IsActive.Should().BeTrue();
        user.Id.Should().NotBeEmpty();
    }

    [Test]
    public void Create_NewUser_IsActiveByDefault()
    {
        var user = User.Create("test@example.com", "hash", "Test", "User");

        user.IsActive.Should().BeTrue();
    }

    [Test]
    public void Deactivate_ActiveUser_SetsIsActiveFalse()
    {
        var user = User.Create("test@example.com", "hash", "Test", "User");

        user.Deactivate();

        user.IsActive.Should().BeFalse();
    }

    [Test]
    public void Activate_DeactivatedUser_SetsIsActiveTrue()
    {
        var user = User.Create("test@example.com", "hash", "Test", "User");
        user.Deactivate();

        user.Activate();

        user.IsActive.Should().BeTrue();
    }

    [Test]
    public void UpdateRoles_WithRoles_AddsRolesToCollection()
    {
        var user = User.Create("test@example.com", "hash", "Test", "User");

        user.UpdateRoles(new[] { "OpsManager", "Dispatcher" });

        user.Roles.Should().HaveCount(2);
        user.Roles.Select(r => r.RoleName).Should().Contain("OpsManager");
        user.Roles.Select(r => r.RoleName).Should().Contain("Dispatcher");
    }

    [Test]
    public void UpdateRoles_CalledTwice_ReplacesExistingRoles()
    {
        var user = User.Create("test@example.com", "hash", "Test", "User");
        user.UpdateRoles(new[] { "OpsManager" });

        user.UpdateRoles(new[] { "Driver" });

        user.Roles.Should().HaveCount(1);
        user.Roles.First().RoleName.Should().Be("Driver");
    }

    [Test]
    public void UpdateProfile_WithNewNames_UpdatesFields()
    {
        var user = User.Create("test@example.com", "hash", "Old", "Name");

        user.UpdateProfile("New", "Updated");

        user.FirstName.Should().Be("New");
        user.LastName.Should().Be("Updated");
    }

    [Test]
    public void UpdateProfile_WithNullFirstName_KeepsExistingFirstName()
    {
        var user = User.Create("test@example.com", "hash", "Original", "Name");

        user.UpdateProfile(null, "NewLastName");

        user.FirstName.Should().Be("Original");
        user.LastName.Should().Be("NewLastName");
    }

    [Test]
    public void SetPasswordResetToken_SetsTokenAndExpiry()
    {
        var user = User.Create("test@example.com", "hash", "Test", "User");
        var expiry = DateTime.UtcNow.AddHours(1);

        user.SetPasswordResetToken("reset-token-123", expiry);

        user.PasswordResetToken.Should().Be("reset-token-123");
        user.PasswordResetTokenExpiry.Should().Be(expiry);
    }

    [Test]
    public void IsPasswordResetTokenValid_WithValidToken_ReturnsTrue()
    {
        var user = User.Create("test@example.com", "hash", "Test", "User");
        user.SetPasswordResetToken("valid-token", DateTime.UtcNow.AddHours(1));

        var result = user.IsPasswordResetTokenValid("valid-token");

        result.Should().BeTrue();
    }

    [Test]
    public void IsPasswordResetTokenValid_WithExpiredToken_ReturnsFalse()
    {
        var user = User.Create("test@example.com", "hash", "Test", "User");
        user.SetPasswordResetToken("expired-token", DateTime.UtcNow.AddHours(-1));

        var result = user.IsPasswordResetTokenValid("expired-token");

        result.Should().BeFalse();
    }

    [Test]
    public void IsPasswordResetTokenValid_WithWrongToken_ReturnsFalse()
    {
        var user = User.Create("test@example.com", "hash", "Test", "User");
        user.SetPasswordResetToken("correct-token", DateTime.UtcNow.AddHours(1));

        var result = user.IsPasswordResetTokenValid("wrong-token");

        result.Should().BeFalse();
    }

    [Test]
    public void UpdatePassword_ClearsResetToken()
    {
        var user = User.Create("test@example.com", "hash", "Test", "User");
        user.SetPasswordResetToken("token", DateTime.UtcNow.AddHours(1));

        user.UpdatePassword("new-hash");

        user.PasswordHash.Should().Be("new-hash");
        user.PasswordResetToken.Should().BeNull();
        user.PasswordResetTokenExpiry.Should().BeNull();
    }
}
