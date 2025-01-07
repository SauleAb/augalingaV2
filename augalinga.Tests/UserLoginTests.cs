using augalinga.Backend.ViewModels;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace augalinga.Tests
{
    public class UserLoginTests
    {
        [Fact]
        public void Email_ShouldRaisePropertyChanged_WhenValueIsSet()
        {
            // Arrange
            var viewModel = new UserLoginViewModel();
            bool propertyChangedRaised = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(UserLoginViewModel.Email))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            viewModel.Email = "test@example.com";

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void Password_ShouldRaisePropertyChanged_WhenValueIsSet()
        {
            // Arrange
            var viewModel = new UserLoginViewModel();
            bool propertyChangedRaised = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(UserLoginViewModel.Password))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            viewModel.Password = "password123";

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void Email_ShouldFailValidation_WhenNotProvided()
        {
            // Arrange
            var viewModel = new UserLoginViewModel
            {
                Email = null, // Invalid value
                Password = "password123"
            };

            var validationContext = new ValidationContext(viewModel) { MemberName = nameof(UserLoginViewModel.Email) };
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(viewModel.Email, validationContext, validationResults);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.ErrorMessage == "Email is required.");
        }

        [Fact]
        public void Password_ShouldFailValidation_WhenNotProvided()
        {
            // Arrange
            var viewModel = new UserLoginViewModel
            {
                Email = "test@example.com",
                Password = null // Invalid value
            };

            var validationContext = new ValidationContext(viewModel) { MemberName = nameof(UserLoginViewModel.Password) };
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(viewModel.Password, validationContext, validationResults);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.ErrorMessage == "Password is required.");
        }

        [Fact]
        public void Model_ShouldPassValidation_WhenAllPropertiesAreValid()
        {
            // Arrange
            var viewModel = new UserLoginViewModel
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var validationContext = new ValidationContext(viewModel);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(viewModel, validationContext, validationResults, validateAllProperties: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }
    }
}
