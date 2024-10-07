using augalinga.Data.Access;
using augalinga.Data.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.Backend.ViewModels
{
    public class DraftsViewModel
    {
        int _projectId;
        public DraftsViewModel(int projectId)
        {
            _projectId = projectId;
            LoadDrafts(_projectId);
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

        public void AddDraftToCollection(Draft draft)
        {
            Drafts.Add(draft);
            LoadDrafts(_projectId);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadDrafts(int projectId)
        {
            using (var context = new DataContext())
            {
                var drafts = context.Drafts.Where(draft => draft.ProjectId == _projectId).ToList();

                Drafts = new ObservableCollection<Draft>(drafts);
            }
        }

        public void RemoveDraft(string draftLink)
        {
            //local
            var draftToRemove = Drafts.FirstOrDefault(p => p.Link == draftLink);
            Drafts.Remove(draftToRemove);

            //database
            using (var dbContext = new DataContext())
            {
                dbContext.Drafts.Remove(draftToRemove);
                dbContext.SaveChanges();
            }

            LoadDrafts(_projectId);
        }
    }
}