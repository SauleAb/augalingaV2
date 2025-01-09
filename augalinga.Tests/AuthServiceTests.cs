using augalinga.Backend.Services;
using augalinga.Data.Access;
using augalinga.Data.Entities;
using augalinga.Backend.ViewModels;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace augalinga.Tests
{
    public class AuthServiceTests
    {
        private DataContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new DataContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        private Mock<IEmailService> CreateEmailServiceMock()
        {
            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock
                .Setup(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            return emailServiceMock;
        }

        //[Fact]
        //public async Task Login_ShouldSucceed_WhenCredentialsAreCorrect()
        //{
        //    // Arrange
        //    var context = CreateDbContext();
        //    var emailServiceMock = CreateEmailServiceMock();

        //    var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        //    var user = new User { Email = "test@example.com", Password = hashedPassword, Token = Guid.NewGuid().ToString(), Color = "Test", FullName = "Test" };
        //    context.Users.Add(user);
        //    await context.SaveChangesAsync();

        //    var authService = new AuthService(emailServiceMock.Object, context);

        //    // Act
        //    var result = await authService.Login("test@example.com", "password123");

        //    // Assert
        //    Assert.True(result);
        //    Assert.True(authService.IsUserLoggedIn());
        //    Assert.Equal(user.Email, authService.GetCurrentUser()?.Email);
        //}

        [Fact]
        public async Task Login_ShouldFail_WhenCredentialsAreIncorrect()
        {
            // Arrange
            var context = CreateDbContext();
            var emailServiceMock = CreateEmailServiceMock();

            var authService = new AuthService(emailServiceMock.Object, context);

            // Act
            var result = await authService.Login("wrong@example.com", "wrongpassword");

            // Assert
            Assert.False(result);
            Assert.False(authService.IsUserLoggedIn());
        }

        //[Fact]
        //public async Task RegisterUser_ShouldSucceed_WhenEmailIsUnique()
        //{
        //    // Arrange
        //    var context = CreateDbContext();
        //    var emailServiceMock = CreateEmailServiceMock();

        //    var authService = new AuthService(emailServiceMock.Object, context);

        //    var registerViewModel = new UserRegisterViewModel
        //    {
        //        FullName = "New User",
        //        Email = "newuser@example.com",
        //        Password = "password123",
        //        Background = "#FFFFFF"
        //    };

        //    // Act
        //    var result = await authService.RegisterUser(registerViewModel);

        //    // Assert
        //    Assert.True(result);
        //    Assert.Single(context.Users);
        //    Assert.Equal(registerViewModel.Email, context.Users.FirstOrDefault()?.Email);
        //}

        [Fact]
        public async Task RegisterUser_ShouldFail_WhenEmailAlreadyExists()
        {
            // Arrange
            var context = CreateDbContext();
            var emailServiceMock = CreateEmailServiceMock();

            context.Users.Add(new User { Email = "existinguser@example.com", Color = "Test", FullName = "Test", Token = "Test", Password = "Test" });
            await context.SaveChangesAsync();

            var authService = new AuthService(emailServiceMock.Object, context);

            var registerViewModel = new UserRegisterViewModel
            {
                FullName = "Existing User",
                Email = "existinguser@example.com",
                Password = "password123",
                Background = "#FFFFFF"
            };

            // Act
            var result = await authService.RegisterUser(registerViewModel);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RequestPasswordReset_ShouldSendEmail_WhenUserExists()
        {
            // Arrange
            var context = CreateDbContext();
            var emailServiceMock = CreateEmailServiceMock();

            var user = new User { Email = "reset@example.com", Color = "Test", FullName = "Test", Token = "", Password = "Test" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var authService = new AuthService(emailServiceMock.Object, context);

            // Act
            var result = await authService.RequestPasswordResetAsync("reset@example.com");

            // Assert
            Assert.True(result);
            Assert.NotNull(user.PasswordResetToken);
            emailServiceMock.Verify(s => s.SendEmailAsync(
                "reset@example.com",
                It.IsAny<string>(),
                It.Is<string>(body => body.Contains(user.PasswordResetToken))), Times.Once);
        }

        [Fact]
        public async Task RequestPasswordReset_ShouldFail_WhenUserDoesNotExist()
        {
            // Arrange
            var context = CreateDbContext();
            var emailServiceMock = CreateEmailServiceMock();

            var authService = new AuthService(emailServiceMock.Object, context);

            // Act
            var result = await authService.RequestPasswordResetAsync("nonexistent@example.com");

            // Assert
            Assert.False(result);
            emailServiceMock.Verify(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ResetPassword_ShouldSucceed_WhenResetTokenIsValid()
        {
            // Arrange
            var context = CreateDbContext();
            var emailServiceMock = CreateEmailServiceMock();

            var user = new User { Email = "reset@example.com", PasswordResetToken = "1234", Color = "Test", FullName = "Test", Token = "", Password = "Test"};
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var authService = new AuthService(emailServiceMock.Object, context);

            // Act
            var result = await authService.ResetPasswordAsync("1234", "newpassword123");

            // Assert
            Assert.True(result);
            Assert.True(BCrypt.Net.BCrypt.Verify("newpassword123", user.Password));
            Assert.Null(user.PasswordResetToken); // Reset token should be cleared
        }

        //[Fact]
        //public async Task ResetPassword_ShouldFail_WhenResetTokenIsInvalid()
        //{
        //    // Arrange
        //    var context = CreateDbContext();
        //    var emailServiceMock = CreateEmailServiceMock();

        //    var user = new User { Email = "reset@example.com", PasswordResetToken = "1234", Color = "Test", FullName = "Test", Token = "", Password = "Test" };
        //    context.Users.Add(user);
        //    await context.SaveChangesAsync();

        //    var authService = new AuthService(emailServiceMock.Object, context);

        //    // Act
        //    var result = await authService.ResetPasswordAsync("wrongtoken", "newpassword123");

        //    // Assert
        //    Assert.False(result);
        //    Assert.False(BCrypt.Net.BCrypt.Verify("newpassword123", user.Password));
        //    Assert.Equal("1234", user.PasswordResetToken); // Token should remain unchanged
        //}
    }
}
