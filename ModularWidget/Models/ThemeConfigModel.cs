using ModularWidget.Common.Models;
using ModularWidget.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace ModularWidget.Models
{
    public class ThemeConfigModel : ModelBase, IThemeConfig
    {
        private string _name;
        private string _relativeUri;
        private bool _active;
        private IEnumerable<string> _keys;
        private IEnumerable<string> _colors;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string RelativeUri
        {
            get { return _relativeUri; }
            set
            {
                _relativeUri = value;
                OnPropertyChanged(nameof(RelativeUri));
            }
        }

        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                OnPropertyChanged(nameof(Active));
                OnPropertyChanged(nameof(TitleColor));
            }
        }

        public IEnumerable<string> Keys
        {
            get { return _keys; }
            set
            {
                _keys = value;
                OnPropertyChanged(nameof(Keys));
            }
        }

        public IEnumerable<string> Colors
        {
            get { return _colors; }
            set
            {
                _colors = value;
                OnPropertyChanged(nameof(Colors));
                OnPropertyChanged(nameof(PreviewTopColors));
            }
        }


        public IEnumerable<string> PreviewTopColors =>
            Colors.GroupBy(c => c).OrderByDescending(gr => gr.Count())
                .Take(4).Select(gr => gr.Key);

        public Brush TitleColor => _active ? Brushes.BlueViolet : Brushes.Black;

        public ThemeConfigModel(ThemeConfig config)
        {
            Name = config.Name;
            RelativeUri = config.RelativeUri;
            Active = config.Active;
            Keys = config.Keys;
            Colors = config.Colors;
        }
    }
}
