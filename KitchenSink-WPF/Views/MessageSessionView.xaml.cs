using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KitchenSink
{
    /// <summary>
    /// Interaction logic for MessageSessionView.xaml
    /// </summary>
    public partial class MessageSessionView : UserControl
    {
        private MessageSessionViewModel viewModel;
        public MessageSessionView()
        {
            InitializeComponent();
            viewModel = this.DataContext as MessageSessionViewModel;
        }

        private void mentionList_DropDownOpened(object sender, EventArgs e)
        {
            viewModel?.FetchMemberships();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textbox = sender as TextBox;
            if (e.Key == Key.Enter &&
                !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) &&
                !(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                viewModel?.SendMessage(null);
                e.Handled = true;
            }
        }
    }
}
