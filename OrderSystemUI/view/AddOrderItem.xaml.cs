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
    /// Interaction logic for OrderNew.xaml
    /// </summary>
    public partial class AddOrderItem : Page
    {
        private OrderHeader _newOrder;
        private Orders _ordersRepo;
        private OrderItem _orderItem;
        public AddOrderItem(Orders orderRepo, OrderHeader order)
        {
            _ordersRepo = orderRepo;
            _newOrder = order;

            string connectionString =
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["OrderManagementDb"].ConnectionString;

            StockItems stockRepo = new StockItems(connectionString);

            InitializeComponent();
            _orderItem = new OrderItem();
            this.DataContext = _orderItem;

            stockSelector.ItemsSource = stockRepo.GetStockItems();
        }

        private void stock_SelectionChanged(object sender, RoutedEventArgs e)
        {
            StockItem selectedItem = (StockItem)stockSelector.SelectedItem;
            itemName.Text = selectedItem.Name;
            price.Text = selectedItem.Price.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string action = (sender as Button).Content.ToString();

            if (action == "Cancel")
            {
                var addOrderPage = new AddOrder(_ordersRepo, _newOrder);
                this.NavigationService.Navigate(addOrderPage);
            }
            else if (action == "Save")
            {

                if(itemName.Text.Length < 1)
                {
                    MessageBoxResult result = MessageBox.Show("Please select item!", "Notification");
                    return;
                }

                if (description.Text.Length == 0)
                {
                    MessageBoxResult result = MessageBox.Show("Please provide order description!", "Notification");
                    return;
                }

                if (_orderItem.Quantity == 0)
                {
                    MessageBoxResult result = MessageBox.Show("Please provide order quantity!", "Notification");
                    return;
                }

                _orderItem.Name = itemName.Text;
                _orderItem.Price = ((StockItem)stockSelector.SelectedItem).Price;
                _orderItem.StockItemId = ((StockItem)stockSelector.SelectedItem).Id;
                _orderItem.StockGap = ((StockItem)stockSelector.SelectedItem).InStock - _orderItem.Quantity;

                _newOrder.AddOrderItem(_orderItem);
                var addOrderPage = new AddOrder(_ordersRepo, _newOrder);
                this.NavigationService.Navigate(addOrderPage);
            }

            Console.WriteLine(action);
        }

    }
}
