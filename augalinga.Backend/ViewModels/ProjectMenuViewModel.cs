using augalinga.Backend.Models;
using augalinga.Backend.Services;
using augalinga.Data.Access;
using augalinga.Data.Entities;
using augalinga.Data.Enums;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;

namespace augalinga.Backend.ViewModels
{
    public class ProjectMenuViewModel : INotifyPropertyChanged
    {
        private readonly DataContext _dbContext;
        private readonly AzureBlobStorage _blobStorage;

        public ProjectMenuViewModel(DataContext dbContext)
        {
            _dbContext = dbContext;
            _blobStorage = new AzureBlobStorage();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Project _project;
        public Project Project
        {
            get => _project;
            set
            {
                _project = value;
                OnPropertyChanged(nameof(Project));
            }
        }

        public async Task LoadProjectAsync(int projectId)
        {
            Project = await _dbContext.Projects.FindAsync(projectId);
            if (Project == null)
            {
                throw new InvalidOperationException("Project not found.");
            }
        }

        public string GetBlobUrl(string projectName, string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                return "/images/default_photo.png";
            }
            return $"{_blobStorage.GetBlobBaseUrl()}/{projectName}/cover photo/{imageName}";
        }

        public async Task SaveProjectChangesAsync(string updatedProjectName, IBrowserFile selectedPhoto, INotificationService notificationService)
        {
            if (Project.Name != updatedProjectName)
            {
                if (!string.IsNullOrEmpty(Project.ImageUrl))
                {
                    string previousPhotoKey = $"{Project.Name}/cover photo/{Path.GetFileName(Project.ImageUrl)}";
                    string newPhotoKey = $"{updatedProjectName}/cover photo/{Path.GetFileName(Project.ImageUrl)}";
                    await _blobStorage.RenameBlobAsync(previousPhotoKey, newPhotoKey);
                }

                Project.Name = updatedProjectName;
            }

            if (selectedPhoto != null)
            {
                if (!string.IsNullOrEmpty(Project.ImageUrl))
                {
                    string previousPhotoKey = $"{Project.Name}/cover photo/{Path.GetFileName(Project.ImageUrl)}";
                    await _blobStorage.DeleteBlobAsync(previousPhotoKey);
                }

                await _blobStorage.UploadCoverPhoto(Project.Name, selectedPhoto);
                Project.ImageUrl = selectedPhoto.Name;
            }

            _dbContext.Projects.Update(Project);
            await _dbContext.SaveChangesAsync();
            notificationService.CreateNotification(Project.Name, null, NotificationType.ProjectModified, null);
        }

        public async Task DeleteProjectAsync(INotificationService notificationService)
        {
            if (!string.IsNullOrEmpty(Project.ImageUrl))
            {
                string photoKey = $"{Project.Name}/cover photo/{Path.GetFileName(Project.ImageUrl)}";
                await _blobStorage.DeleteBlobAsync(photoKey);
            }

            _dbContext.Projects.Remove(Project);
            await _dbContext.SaveChangesAsync();
            notificationService.CreateNotification(Project.Name, null, NotificationType.ProjectDeleted, null);
        }
    }
}
