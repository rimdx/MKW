using System.Collections.ObjectModel;

namespace MKW.GUI.Wizard
{
    public abstract class WizardViewModel : ViewModelBase
    {
        public virtual string Header { get; }

        public ObservableCollection<WizardPage> Pages;

        private int currentPageIndex = 0;
        public int CurrentPageIndex
        {
            get => currentPageIndex;
            set
            {
                if (SetProperty(ref currentPageIndex, value))
                {
                    OnPropertyChanged(nameof(CurrentPage));
                    OnPropertyChanged(nameof(PageHeader));
                    OnPropertyChanged(nameof(CanGoBack));
                    OnPropertyChanged(nameof(CanGoNext));
                    OnPropertyChanged(nameof(CanFinish));
                }
            }
        }

        public WizardPage CurrentPage => Pages[CurrentPageIndex];

        public virtual bool CanGoBack => (CurrentPageIndex > 0);
        public virtual bool CanGoNext => (CurrentPageIndex < Pages.Count - 1);
        public virtual bool CanFinish => (CurrentPageIndex == Pages.Count - 1);

        public string PageHeader => CurrentPage.Header;

        public WizardViewModel(string header)
        {
            Pages = [];
            Header = header;
        }

        protected void AddPage(WizardPage page)
        {
            Pages.Add(page);

            OnPropertyChanged(nameof(CanFinish));
            OnPropertyChanged(nameof(CanGoNext));
            OnPropertyChanged(nameof(CanGoBack));
        }

        public virtual bool Next()
        {
            if (CanGoNext && CurrentPage.Next())
            {
                CurrentPageIndex++;
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool Back()
        {
            if (CanGoBack)
            {
                CurrentPageIndex--;
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool Finish()
        {
            return true;
        }

        public virtual bool Cancel()
        {
            return true;
        }
    }
}
