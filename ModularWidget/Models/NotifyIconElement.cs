using ModularWidget.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ModularWidget.Models
{
    public class NotifyIconElement
    {
        public string Name { get; set; }

        public string Header { get; set; }

        public Action<object> Action { get; set; }

        public RelayCommand ActionCommand => Action is not null ? new RelayCommand(Action) : null;

        public ObservableCollection<NotifyIconElement> Children { get; set; }

        public NotifyIconElement FindByName(string name)
        {
            if (string.Equals(Name, name))
                return this;
            if (Children is null || Children.Count == 0)
                return null;
            return Children.FirstOrDefault(e => e.FindByName(name) != null);
        }
    }
}
