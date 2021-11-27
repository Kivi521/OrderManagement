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

namespace OrderSystemUI.view
{
    /// <summary>
    /// Interaction logic for OrderNew.xaml
    /// </summary>
    public partial class OrderNew : Page
    {
        public StockItems _stRepo;
        
        public OrderNew(StockItems sItems)
        {
            
            _stRepo = sItems;
            InitializeComponent();
            sItem.ItemsSource = _stRepo.GetStockItems();
        }

    }
}
