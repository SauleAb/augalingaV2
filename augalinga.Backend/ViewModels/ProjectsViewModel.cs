using augalinga.Backend.Models;
using augalinga.Backend.Services;
using augalinga.Data.Access;
using augalinga.Data.Entities;
using augalinga.Data.Enums;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.Backend.ViewModels
{
    public class ProjectsViewModel : INotifyPropertyChanged
    {
        private readonly DataContext _dbContext;
        private readonly AzureBlobStorage _blobStorage;
        private readonly string _coverPhotoFolder = "cover photo";

        public ProjectsViewModel(DataContext dbContext)
        {
            _dbContext = dbContext;
            _blobStorage = new AzureBlobStorage();
            LoadProjects();
        }

        private ObservableCollection<Project> _projects;
        public ObservableCollection<Project> Projects
        {
            get => _projects;
            set
            {
                _projects = value;
                OnPropertyChanged(nameof(Projects));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadProjects()
        {
            var projects = _dbContext.Projects.ToList();
            Projects = new ObservableCollection<Project>(projects);
        }

        public async Task LoadProjectsAsync()
        {
            var projects = await _dbContext.Projects.ToListAsync();
            Projects = new ObservableCollection<Project>(projects);
        }

        public async Task AddProjectAsync(Project project, IBrowserFile selectedFile, INotificationService notificationService)
        {
            if (string.IsNullOrWhiteSpace(project.Name))
            {
                throw new ArgumentException("Project name cannot be empty.");
            }

            if (selectedFile != null)
            {
                await _blobStorage.UploadCoverPhoto(project.Name, selectedFile);
                project.ImageUrl = selectedFile.Name;
            }

            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync();

            Projects.Add(project);

            notificationService.CreateNotification(project.Name, null, NotificationType.ProjectAdded, null);
        }

        public string GetBlobUrl(string projectName, string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                return "/images/calendar_background.png";
            }
            return $"{_blobStorage.GetBlobBaseUrl()}/{projectName}/{_coverPhotoFolder}/{imageName}";
        }
    }
}
