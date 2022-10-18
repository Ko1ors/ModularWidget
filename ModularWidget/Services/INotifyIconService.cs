using Hardcodet.Wpf.TaskbarNotification;
using ModularWidget.Models;
using System.Collections.Generic;

namespace ModularWidget.Services
{
    public delegate void NotifyIconHandler(IEnumerable<NotifyIconElement> elements);

    public interface INotifyIconService
    {
        public event NotifyIconHandler RootElementsChanged;

        void SetTaskbarIcon(TaskbarIcon taskbarIcon);

        void AddNotifyIconElements(IEnumerable<NotifyIconElement> elements, NotifyIconElement parentElement = null);

        void AddNotifyIconElements(IEnumerable<NotifyIconElement> elements, string parentElementName);

        void AddNotifyIconElement(NotifyIconElement element, NotifyIconElement parentElement = null);

        void AddNotifyIconElement(NotifyIconElement element, string parentElementName);

        NotifyIconElement FindElementByName(string elementName);
    }
}
