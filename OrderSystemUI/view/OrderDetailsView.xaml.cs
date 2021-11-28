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
using OrderDataAccess;
using OrderDomain;


namespace OrderSystemUI.view
{
    /// <summary>
    /// Interaction logic for OrderDetailsView.xaml
    /// </summary>
    public partial class OrderDetailsView : Page
    {
        Orders _ordersRepo;
        public OrderDetailsView(OrderHeader order, Orders ordersRepo)
        {
            _ordersRepo = ordersRepo;
            InitializeComponent();
            dgOrderItems.ItemsSource = order._orderItems;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string action = (sender as Button).Content.ToString();

            if (action == "Back")
            {
                var indexPage = new OrdersIndex(_ordersRepo);
                this.NavigationService.Navigate(indexPage);
            }

            Console.WriteLine(action);
        }
    }
}
