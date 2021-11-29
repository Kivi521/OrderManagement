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
    /// Interaction logic for AddOrder.xaml
    /// </summary>
    public partial class AddOrder : Page
    {

        private Orders _ordersRepo;
        private StockItems _stockRepo;
        private OrderHeader _order;

        public AddOrder(Orders repo, OrderHeader newOrder=null)
        {

            _ordersRepo = repo;

            string connectionString =
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["OrderManagementDb"].ConnectionString;

            _stockRepo = new StockItems(connectionString);

            InitializeComponent();
            
            
            if (newOrder == null)
            {
                _order = new OrderHeader();
                _order.DateTime = DateTime.Now;
                _order.State = OrderStates.New;
                _order._orderItems = new List<OrderItem>();
            }
            else
            {
                _order = newOrder;
            }
            
            List<OrderHeader> orders = new List<OrderHeader>();
            orders.Add(_order);

            dgOrderItems.ItemsSource = _order._orderItems;
            dgOrder.ItemsSource = orders;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string action = (sender as Button).Content.ToString();

            if (action == "Cancel")
            {
                var indexPage = new OrdersIndex(_ordersRepo);
                this.NavigationService.Navigate(indexPage);
            }else if (action == "Add Item")
            {
                var addItemPage = new AddOrderItem(_ordersRepo, _order);
                this.NavigationService.Navigate(addItemPage);
            }
            else if (action == "Delete")
            {
                var selectedItem = (OrderItem)dgOrderItems.SelectedItems[0];
                
                _order._orderItems.Remove(selectedItem);

                dgOrderItems.Items.Refresh();
                dgOrder.Items.Refresh();
               
            }
            else if (action == "Submit")
            {

                if(_order.Count == 0)
                {
                    MessageBoxResult result = MessageBox.Show("Empty order!", "Notification");
                    return;
                }

                if (_order.IsAllInStock)
                {
                    _order.State = OrderStates.Complete;
                    _stockRepo.UpdateStockItemAmount(_order);
                }
                else
                {
                    _order.State = OrderStates.Pending;
                }

                int orderId = _ordersRepo.InsertOrderHeader(_order);

                foreach(OrderItem item in _order._orderItems)
                {
                    item.OrderHeaderId = orderId;
                    _ordersRepo.UpsertOrderItem(item);
                }

                var indexPage = new OrdersIndex(_ordersRepo);
                this.NavigationService.Navigate(indexPage);
            }

            Console.WriteLine(action);
        }

    }
}
