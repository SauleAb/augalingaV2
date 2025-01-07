using augalinga.Data.Access;
using augalinga.Data.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.Backend.ViewModels
{
    public class PhotosViewModel
    {
        int _projectId;
        string _category;
        private readonly DataContext _dbContext;
        public PhotosViewModel(int projectId, string category, DataContext dbContext)
        {
            _projectId = projectId;
            _category = category;
            _dbContext = dbContext;
            LoadPhotos(_projectId, _category);
        }

        public PhotosViewModel(int projectId, DataContext dbContext)
        {
            _projectId = projectId;
            _dbContext = dbContext;
            LoadAllPhotos(_projectId);
        }

        private ObservableCollection<Photo> _photos;
        public ObservableCollection<Photo> Photos
        {
            get => _photos;
            set
            {
                _photos = value;
                OnPropertyChanged(nameof(Photos));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadPhotos(int projectId, string category)
        {
                var photos = _dbContext.Photos
                    .Where(photo => photo.ProjectId == projectId && photo.Category == category)
                    .ToList();

                Photos = new ObservableCollection<Photo>(photos);
        }

        private void LoadAllPhotos(int projectId)
        {
                var photos = _dbContext.Photos
                    .Where(photo => photo.ProjectId == projectId)
                    .ToList();

                Photos = new ObservableCollection<Photo>(photos);
            
        }
    }
}