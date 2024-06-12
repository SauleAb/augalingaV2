using augalinga.Data.Access;
using augalinga.Data.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.ApplicationLayer.Components.ViewModels
{
    public class PhotosViewModel
    {
        string _projectName;
        string _category;
        public PhotosViewModel(string projectName, string category)
        {
            _projectName = projectName;
            _category = category;
            LoadPhotos(_projectName, _category);
        }

        public PhotosViewModel(string projectName)
        {
            _projectName = projectName;
            LoadAllPhotos(_projectName);
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

        public void AddPhotoToCollection(Photo photo)
        {
            Photos.Add(photo);
            LoadPhotos(_projectName, _category);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadPhotos(string projectName, string category)
        {
            using (var context = new DataContext())
            {
                var photos = context.Photos
                    .Where(photo => photo.Project == projectName && photo.Category == category)
                    .ToList();

                Photos = new ObservableCollection<Photo>(photos);
            }
        }

        private void LoadAllPhotos(string projectName)
        {
            using (var context = new DataContext())
            {
                var photos = context.Photos
                    .Where(photo => photo.Project == projectName)
                    .ToList();

                Photos = new ObservableCollection<Photo>(photos);
            }
        }

        public void RemovePhoto(string photoLink)
        {
            //local
            var photoToRemove = Photos.FirstOrDefault(p => p.Link == photoLink);
            Photos.Remove(photoToRemove);

            //database
            using (var dbContext = new DataContext())
            {
                dbContext.Photos.Remove(photoToRemove);
                dbContext.SaveChanges();
            }

            LoadPhotos(_projectName, _category);
        }
    }
}
