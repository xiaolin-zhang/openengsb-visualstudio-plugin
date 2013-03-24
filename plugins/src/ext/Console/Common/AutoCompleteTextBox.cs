using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Console.Common
{
    public class AutoCompleteTextBox : TextBox
    {
        private Popup _autoCompletePopup = new Popup();
        private ListBox _autoCompleteDisplayer = new ListBox();

        public IEnumerable AutoCompleteSource { get; set; }

        public event EventHandler UpdateAutoCompleteSource;

        public AutoCompleteTextBox()
            : base()
        {
            _autoCompleteDisplayer.HorizontalAlignment = HorizontalAlignment.Stretch;
            _autoCompleteDisplayer.VerticalAlignment = VerticalAlignment.Stretch;

            _autoCompletePopup.Child = _autoCompleteDisplayer;
            _autoCompletePopup.Width = 150;
            _autoCompletePopup.StaysOpen = false;
            _autoCompletePopup.PopupAnimation = PopupAnimation.Scroll;
            _autoCompletePopup.Placement = PlacementMode.Bottom;
            _autoCompleteDisplayer.PreviewKeyDown += new KeyEventHandler(_autoCompleteDisplayer_PreviewKeyDown);
            _autoCompleteDisplayer.MouseDoubleClick += new MouseButtonEventHandler(_autoCompleteDisplayer_MouseDoubleClick);
        }

        void _autoCompleteDisplayer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            InsertText(_autoCompleteDisplayer.SelectedItem.ToString());
        }

        void _autoCompleteDisplayer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InsertText(_autoCompleteDisplayer.SelectedItem.ToString());
            }
            else if (e.Key == Key.Escape)
                _autoCompletePopup.IsOpen = false;
        }

        private void InsertText(string str)
        {
            SelectedText = str;
            _autoCompletePopup.IsOpen = false;
            Dispatcher.BeginInvoke(new Func<bool>(Focus), DispatcherPriority.Background);
        }

        private void RaiseUpdateAutoCompleteSource()
        {
            if (UpdateAutoCompleteSource != null)
                UpdateAutoCompleteSource(this, EventArgs.Empty);
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (Text == null)
                return;

            if (e.Key == Key.Space && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                RaiseUpdateAutoCompleteSource();
                _autoCompleteDisplayer.ItemsSource = AutoCompleteSource;

                if (_autoCompleteDisplayer.Items.Count > 0)
                {
                    _autoCompletePopup.PlacementRectangle = GetRectFromCharacterIndex(CaretIndex, true);
                    _autoCompletePopup.PlacementTarget = this;
                    _autoCompletePopup.IsOpen = true;
                    _autoCompleteDisplayer.Focus();
                    _autoCompleteDisplayer.SelectedIndex = 0;
                }
            }

            base.OnKeyDown(e);
        }
    }
}
