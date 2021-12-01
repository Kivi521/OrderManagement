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
    /// Interaction logic for OrdersIndex.xaml
    /// </summary>
    public partial class OrdersIndex : Page
    {

        public Orders _ordersRepo;
        public OrdersIndex(Orders odRp)
        {
            _ordersRepo = odRp;
            InitializeComponent();
            dgOrders.ItemsSource = _ordersRepo.GetOrderHeaders();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string action = (sender as Button).Content.ToString();

            if(action == "Details")
            {
                var order = (OrderHeader)dgOrders.SelectedItems[0];

                var detailPage = new OrderDetailsView(order, _ordersRepo);
                this.NavigationService.Navigate(detailPage);
            }else if (action == "New Order")
            {
                var addOrderPage = new AddOrder(_ordersRepo);
                this.NavigationService.Navigate(addOrderPage);
            }

                Console.WriteLine(action);
        }
    }
}
