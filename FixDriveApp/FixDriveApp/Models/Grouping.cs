using System.Collections.ObjectModel;

namespace FixDriveApp.Models
{
	public class Grouping<K, T> : ObservableCollection<T>
    {
        public K Name { get; private set; }
        public Grouping(K name, IEnumerable<T> items) : base(items)
        {
            Name = name;
        }
    }
}

