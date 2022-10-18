using ModularWidget.Common.ViewModels;
using ModularWidget.Models;
using ModularWidget.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ModularWidget.ViewModels
{
    public class NotifyIconViewModel : ViewModelBase
    {
        private readonly INotifyIconService _notifyIconService;

        public ObservableCollection<NotifyIconElement> Elements { get; set; }

        public NotifyIconViewModel(INotifyIconService notifyIconService)
        {
            _notifyIconService = notifyIconService;
            _notifyIconService.RootElementsChanged += NotifyIconServiceRootElementsChanged;
            Elements = new ObservableCollection<NotifyIconElement>();
        }

        private void NotifyIconServiceRootElementsChanged(IEnumerable<NotifyIconElement> elements)
        {
            Elements.Clear();
            Elements.AddRange(elements);
        }
    }
}
