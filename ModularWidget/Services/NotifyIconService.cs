using Hardcodet.Wpf.TaskbarNotification;
using ModularWidget.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ModularWidget.Services
{
    public class NotifyIconService : INotifyIconService
    {
        private TaskbarIcon _taskbarIcon;
        private IEnumerable<NotifyIconElement> _rootElements;

        public event NotifyIconHandler RootElementsChanged;

        public void AddNotifyIconElement(NotifyIconElement element, NotifyIconElement parentElement = null)
        {
            AddNotifyIconElements(new[] { element }, parentElement);
        }

        public void AddNotifyIconElement(NotifyIconElement element, string parentElementName)
        {
            AddNotifyIconElements(new[] { element }, parentElementName);
        }

        public void AddNotifyIconElements(IEnumerable<NotifyIconElement> elements, NotifyIconElement parentElement = null)
        {
            if (parentElement != null)
            {
                parentElement.Children.AddRange(elements);
                return;
            }

            if (_rootElements is null)
                _rootElements = elements;
            else
                _rootElements = _rootElements.ToList().Union(elements);
            RootElementsChanged?.Invoke(_rootElements);
        }

        public void AddNotifyIconElements(IEnumerable<NotifyIconElement> elements, string parentElementName = null)
        {
            NotifyIconElement parentElement = null;
            if (!string.IsNullOrEmpty(parentElementName))
            {
                parentElement = _rootElements?.FirstOrDefault(e => e.FindByName(parentElementName) != null);
                if (parentElement is null)
                    throw new Exception($"Parent notify icon element with {parentElementName} name not found");
            }
            AddNotifyIconElements(elements, parentElement);
        }

        public NotifyIconElement FindElementByName(string elementName)
        {
            return _rootElements?.FirstOrDefault(e => e.FindByName(elementName) != null);
        }

        public void SetTaskbarIcon(TaskbarIcon taskbarIcon)
        {
            if (_taskbarIcon != null)
                throw new Exception("Taskbar icon has already been set before");
            _taskbarIcon = taskbarIcon;
        }
    }
}
