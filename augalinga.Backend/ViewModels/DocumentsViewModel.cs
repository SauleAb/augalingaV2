using augalinga.Backend.Models;
using augalinga.Backend.Services;
using augalinga.Data.Access;
using augalinga.Data.Entities;
using augalinga.Data.Enums;
using Azure;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.Backend.ViewModels
{
    public class DocumentsViewModel
    {
        int _projectId;
        private readonly DataContext _dbContext;
        string containerName = "augalinga";
        string folder = "documents";
        private AzureBlobStorage blobStorage;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public DocumentsViewModel(int projectId, DataContext dbContext)
        {
            _projectId = projectId;
            _dbContext = dbContext;
            blobStorage = new AzureBlobStorage();
            LoadDocuments(_projectId);
        }

        private ObservableCollection<Document> _documents;
        public ObservableCollection<Document> Documents
        {
            get => _documents;
            set
            {
                _documents = value;
                OnPropertyChanged(nameof(Documents));
            }
        }

        public void LoadDocuments(int projectId)
        {
                var documents = _dbContext.Documents.Where(document => document.ProjectId == _projectId).ToList();

                Documents = new ObservableCollection<Document>(documents);
        }

        public void RemoveDocument(string documentLink)
        {
            var documentToRemove = Documents.FirstOrDefault(p => p.Link == documentLink);
            if (documentToRemove != null)
            {
                _dbContext.Documents.Remove(documentToRemove);
                _dbContext.SaveChanges();

                Documents.Remove(documentToRemove);
            }

            LoadDocuments(_projectId);
        }

        public async Task UploadFilesAsync(IEnumerable<IBrowserFile> selectedFiles, Project project, INotificationService notificationService)
        {
            try
            {
                foreach (var file in selectedFiles)
                {
                    await blobStorage.UploadFile(project.Name, folder, file);

                    string fileUrl = await blobStorage.GetBlobUrlAsync(project.Name, folder, file.Name);

                    var newDocument = new Document
                    {
                        ProjectId = _projectId,
                        Link = fileUrl,
                        Name = file.Name
                    };
                    await _dbContext.Documents.AddAsync(newDocument);
                    await _dbContext.SaveChangesAsync();
                    Documents.Add(newDocument);
                    notificationService.CreateNotification(file.Name, project.Name, NotificationType.DocumentAdded, null);
                    LoadDocuments(_projectId);
                }

            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Azure Storage Error: {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex}");
            }
        }
        public async Task DeleteDocument(Document document, string projectName)
        {
            _dbContext.Documents.Remove(document);
            await _dbContext.SaveChangesAsync();

            string blobName = $"{projectName}/{folder}/{document.Name}";
            await blobStorage.DeleteBlobAsync(blobName);

            LoadDocuments(_projectId);

        }
    }
}