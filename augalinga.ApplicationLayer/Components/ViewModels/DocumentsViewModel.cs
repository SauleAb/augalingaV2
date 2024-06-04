using augalinga.Data.Access;
using augalinga.Data.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.ApplicationLayer.Components.ViewModels
{
    public class DocumentsViewModel
    {
        string _projectName;
        public DocumentsViewModel(string projectName)
        {
            _projectName = projectName;
            LoadDocuments(_projectName);
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

        public void AddDocumentToCollection(Document document)
        {
            Documents.Add(document);
            LoadDocuments(_projectName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadDocuments(string projectName)
        {
            using (var context = new DataContext())
            {
                var documents = context.Documents.Where(document => document.Project == projectName).ToList();

                Documents = new ObservableCollection<Document>(documents);
            }
        }

        public void RemoveDocument(string documentLink)
        {
            var documentToRemove = Documents.FirstOrDefault(p => p.Link == documentLink);
            //database
            using (var dbContext = new DataContext())
            {
                dbContext.Documents.Remove(documentToRemove);
                dbContext.SaveChanges();
            }
            //local
            Documents.Remove(documentToRemove);


            LoadDocuments(_projectName);
        }
    }
}
