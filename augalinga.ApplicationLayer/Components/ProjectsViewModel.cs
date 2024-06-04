using augalinga.Data.Access;
using augalinga.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.ApplicationLayer.Components
{
    public class ProjectsViewModel : INotifyPropertyChanged
    {
        public ProjectsViewModel()
        {
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

        public void AddProjectToCollection(Project project)
        {
            Projects.Add(project);
            LoadProjects();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadProjects()
        {
            var projects = new DataContext().Projects.ToList();
            Projects = new ObservableCollection<Project>(projects);
        }

        public void RemoveProject(string projectName)
        {
            //local
            var projectToRemove = Projects.FirstOrDefault(p => p.Name == projectName);
            Projects.Remove(projectToRemove);

            //database
            using (var dbContext = new DataContext())
            {
                dbContext.Projects.Remove(projectToRemove);
                dbContext.SaveChanges();
            }

            LoadProjects();
        }

    }
}
