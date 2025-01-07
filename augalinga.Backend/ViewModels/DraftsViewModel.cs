using augalinga.Data.Access;
using augalinga.Data.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.Backend.ViewModels
{
    public class DraftsViewModel
    {
        int _projectId;
        private readonly DataContext _dbContext;
        public DraftsViewModel(int projectId, DataContext dbContext)
        {
            _projectId = projectId;
            _dbContext = dbContext;
            LoadDrafts(_projectId);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private void LoadDrafts(int projectId)
        {
            var drafts = _dbContext.Drafts.Where(draft => draft.ProjectId == _projectId).ToList();

            Drafts = new ObservableCollection<Draft>(drafts);
        }
    }
}