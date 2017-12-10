using GUI.UserControls;
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

namespace GUI.UserControls
{
    /// <summary>
    /// Interaction logic for CuboidControl.xaml
    /// </summary>
    public partial class CuboidControl : UserControl
    {
        CuboidControlVM viewModel { get; set; } = new CuboidControlVM();
        public CuboidControl()
        { 
            DataContext = viewModel;
            InitializeComponent();

        }
    }
}
