using System.Collections.ObjectModel;

namespace MKW.GUI
{
    public abstract class TransformedObservableCollection<TViewModel, TItem> : ObservableCollection<TViewModel>
    {
        protected abstract TViewModel CreateViewModel(TItem item);
        protected abstract void UpdateViewModel(TViewModel viewModel, TItem item);
        protected abstract object GetViewModelKey(TViewModel viewModel);
        protected abstract object GetItemKey(TItem item);

        public void SetItems(IEnumerable<TItem> newItems)
        {
            Dictionary<object, TItem> itemsDictionary = [];
            foreach (TItem item in newItems)
            {
                itemsDictionary.Add(GetItemKey(item), item);
            }

            using (BlockReentrancy())
            {
                for (int i = 0; i < Count;)
                {
                    TViewModel viewModel = this[i];
                    object key = GetViewModelKey(viewModel);
                    if (itemsDictionary.TryGetValue(key, out TItem item))
                    {
                        UpdateViewModel(viewModel, item);
                        itemsDictionary.Remove(key);
                        i++;
                    }
                    else
                    {
                        RemoveItem(i);
                    }
                }

                foreach (TItem item in itemsDictionary.Values)
                {
                    InsertItem(Count, CreateViewModel(item));
                }
            }
        }
    }
}
