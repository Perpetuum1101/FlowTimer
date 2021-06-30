using FlowTimer.UI.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FlowTimer.UI.Components
{
    public partial class ItemControl : UserControl
    {
        public ItemControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (ItemViewModel)DataContext;
            if (vm != null && vm.Focus)
            {
                TaskName.Focus();
                TaskName.SelectAll();
                vm.Focus = false;
            }
        }
    }
}
