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
using System.ComponentModel;
using FontAwesome.WPF;

namespace Doroshenko05
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {
        private ImageAwesome _loader;
        private ManagerProcessView _processesListView;

        public MainWindow()
        {
            InitializeComponent();
            ShowProcessesListView();
        }

        private void ShowProcessesListView()
        {
            MainGrid.Children.Clear();
            if (_processesListView == null)
                _processesListView = new ManagerProcessView(ShowLoader);
            MainGrid.Children.Add(_processesListView);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _processesListView?.Close();
            SystemProcessThreadsManipulation.Close();
            base.OnClosing(e);
        }

        private void ShowLoader(bool isShow)
        {
            LoaderHelper.OnRequestLoader(MainGrid, ref _loader, isShow);
        }
    }
}
