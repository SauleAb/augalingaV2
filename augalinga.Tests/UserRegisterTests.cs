using augalinga.Backend.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace augalinga.Tests
{
    public class UserRegisterTests
    {
        [Fact]
        public void FullName_ShouldRaisePropertyChanged_WhenValueIsSet()
        {
            // Arrange
            var viewModel = new UserRegisterViewModel();
            bool propertyChangedRaised = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(UserRegisterViewModel.FullName))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            viewModel.FullName = "John Doe";

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void Password_ShouldRaisePropertyChanged_WhenValueIsSet()
        {
            // Arrange
            var viewModel = new UserRegisterViewModel();
            bool propertyChangedRaised = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(UserRegisterViewModel.Password))
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
        public void Email_ShouldRaisePropertyChanged_WhenValueIsSet()
        {
            // Arrange
            var viewModel = new UserRegisterViewModel();
            bool propertyChangedRaised = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(UserRegisterViewModel.Email))
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
        public void Background_ShouldRaisePropertyChanged_WhenValueIsSet()
        {
            // Arrange
            var viewModel = new UserRegisterViewModel();
            bool propertyChangedRaised = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(UserRegisterViewModel.Background))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            viewModel.Background = "#000000";

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void Model_ShouldFailValidation_WhenFullNameIsMissing()
        {
            // Arrange
            var viewModel = new UserRegisterViewModel
            {
                Password = "password123",
                Email = "test@example.com",
                Background = "#FFFFFF"
            };

            var validationContext = new ValidationContext(viewModel);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(viewModel, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.ErrorMessage == "Full name is required.");
        }

        [Fact]
        public void Model_ShouldFailValidation_WhenEmailIsMissing()
        {
            // Arrange
            var viewModel = new UserRegisterViewModel
            {
                FullName = "John Doe",
                Password = "password123",
                Background = "#FFFFFF"
            };

            var validationContext = new ValidationContext(viewModel);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(viewModel, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.ErrorMessage == "Email is required.");
        }

        [Fact]
        public void Model_ShouldFailValidation_WhenEmailIsInvalid()
        {
            // Arrange
            var viewModel = new UserRegisterViewModel
            {
                FullName = "John Doe",
                Password = "password123",
                Email = "invalid-email",
                Background = "#FFFFFF"
            };

            var validationContext = new ValidationContext(viewModel);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(viewModel, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.ErrorMessage == "Invalid email address.");
        }

        [Fact]
        public void Model_ShouldFailValidation_WhenPasswordIsMissing()
        {
            // Arrange
            var viewModel = new UserRegisterViewModel
            {
                FullName = "John Doe",
                Email = "test@example.com",
                Background = "#FFFFFF"
            };

            var validationContext = new ValidationContext(viewModel);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(viewModel, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.ErrorMessage == "Password is required.");
        }

        [Fact]
        public void Model_ShouldPassValidation_WhenAllPropertiesAreValid()
        {
            // Arrange
            var viewModel = new UserRegisterViewModel
            {
                FullName = "John Doe",
                Password = "password123",
                Email = "test@example.com",
                Background = "#FFFFFF"
            };

            var validationContext = new ValidationContext(viewModel);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(viewModel, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }
    }
}
