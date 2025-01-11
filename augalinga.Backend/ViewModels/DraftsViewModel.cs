using augalinga.Backend.Models;
using augalinga.Backend.Services;
using augalinga.Data.Access;
using augalinga.Data.Entities;
using augalinga.Data.Enums;
using Azure;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.Backend.ViewModels
{
    public class DraftsViewModel
    {
        private readonly int _projectId;
        private readonly DataContext _dbContext;
        private readonly AzureBlobStorage _blobStorage;
        private readonly string _folder = "drafts";

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DraftsViewModel(int projectId, DataContext dbContext)
        {
            _projectId = projectId;
            _dbContext = dbContext;
            _blobStorage = new AzureBlobStorage();
            LoadDrafts();
        }

        private ObservableCollection<Draft> _drafts;
        public ObservableCollection<Draft> Drafts
        {
            get => _drafts;
            set
            {
                _drafts = value;
                OnPropertyChanged(nameof(Drafts));
            }
        }

        public void LoadDrafts()
        {
            var drafts = _dbContext.Drafts.Where(draft => draft.ProjectId == _projectId).ToList();
            Drafts = new ObservableCollection<Draft>(drafts);
        }

        public async Task UploadDraftsAsync(IEnumerable<IBrowserFile> selectedFiles, Project project, INotificationService notificationService)
        {
            try
            {
                foreach (var file in selectedFiles)
                {
                    await _blobStorage.UploadFile(project.Name, _folder, file);
                    string fileUrl = await _blobStorage.GetBlobUrlAsync(project.Name, _folder, file.Name);

                    var newDraft = new Draft
                    {
                        ProjectId = _projectId,
                        Link = fileUrl,
                        Name = file.Name
                    };

                    await _dbContext.Drafts.AddAsync(newDraft);
                    await _dbContext.SaveChangesAsync();

                    Drafts.Add(newDraft);

                    notificationService.CreateNotification(file.Name, project.Name, NotificationType.DraftAdded, null);

                    LoadDrafts();
                }
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Azure Storage Error: {ex}");
                throw new Exception("Failed to upload drafts to Azure Blob Storage.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex}");
                throw;
            }
        }

        public async Task DeleteDraftAsync(Draft draft, Project project, INotificationService notificationService)
        {
            try
            {
                string blobName = $"{project.Name}/{_folder}/{draft.Name}";
                await _blobStorage.DeleteBlobAsync(blobName);

                _dbContext.Drafts.Remove(draft);
                await _dbContext.SaveChangesAsync();

                Drafts.Remove(draft);

                notificationService.CreateNotification(draft.Name, project.Name, NotificationType.DraftDeleted, null);

                LoadDrafts();
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Azure Storage Error: {ex}");
                throw new Exception("Failed to delete draft from Azure Blob Storage.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex}");
                throw;
            }
        }
    }
}
