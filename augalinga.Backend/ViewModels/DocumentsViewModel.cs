using augalinga.Data.Access;
using augalinga.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.Backend.ViewModels
{
    public class DocumentsViewModel
    {
        int _projectId;
        private readonly DataContext _dbContext;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public DocumentsViewModel(int projectId, DataContext dbContext)
        {
            _projectId = projectId;
            _dbContext = dbContext;
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
    }
}