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
using OrderSystemUI.view;
using OrderDataAccess;

namespace OrderSystemUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
     
        public MainWindow()
        {
            InitializeComponent();
            string connectionString =
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["OrderManagementDb"].ConnectionString;

            Orders ordersRepo = new Orders(connectionString);
            
            frame.Navigate(new OrdersIndex(ordersRepo));
        }
    }
}
