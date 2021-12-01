using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderDataAccess;
using OrderDomain;


namespace OrderISystemTest
{
    [TestClass]
    public class DataAccessTest
    {
        private StockItems _SItems;
        private Orders _ordersRepo;

        [TestInitialize]
        public void Init()
        {
            string cnnStr = "Data Source=localhost;Initial Catalog= OrderManagementDbTestData; Integrated Security=true";
            _SItems = new StockItems(cnnStr);
            _ordersRepo = new Orders(cnnStr);
        }
       
        [TestMethod]
        public void TestGetStockItem()
        {
            StockItem item = _SItems.GetStockItem(1);
            Assert.AreEqual(item.Name,"Table");
        }

        [TestMethod]
        public void TestGetStockItems()
        {
            var items = _SItems.GetStockItems();
            int cnt = items.Count();
            Assert.AreEqual(9, cnt);

        }


        //[TestMethod]
        //public void TestGetOrderHeaders()
        //{

        //    int numOfRows = _ordersRepo.GetOrderHeaders().Count();

        //    Assert.AreEqual(1, numOfRows);

        //}

        //[TestMethod]
        //public void TestGetOrderHeader()
        //{

        //    OrderHeader order = _ordersRepo.GetOrderHeader(1);

        //    Assert.AreEqual(275, order.Total);
        //}


        [TestMethod]
        public void TestInsertOrderHeader()
        {

            OrderHeader newOrder = new OrderHeader();
            newOrder.DateTime = DateTime.Now;
            newOrder.State = OrderStates.New;

            int id = _ordersRepo.InsertOrderHeader(newOrder);

            Assert.AreEqual(15, id);

        }

        //[TestMethod]
        //public void TestInsertOrderItem()
        //{

        //    OrderItem newItem = new OrderItem();
        //    newItem.OrderHeaderId = 1;
        //    newItem.StockItemId = 1;
        //    newItem.Description = "i like this table";
        //    newItem.Price = 100;
        //    newItem.Quantity = 2;

        //    int numOfRows = _ordersRepo.UpsertOrderItem(newItem);

        //    Assert.AreEqual(numOfRows, 1);

        //}

        //[TestMethod]
        //public void TestUpdateOrderItem()
        //{
        //    OrderItem oldItem = _ordersRepo.GetItemsByHeaderId(1).First(p => p.StockItemId== 1);
        //    oldItem.Description = "i like this table very much!";
        //    int numOfRows = _ordersRepo.UpsertOrderItem(oldItem);
        //    Assert.AreEqual(1, numOfRows);
        //}

        //[TestMethod]
        //public void TestUpdateStockItemAmount()
        //{
        //    OrderHeader order = _ordersRepo.GetOrderHeader(1);            
        //    int numOfRows = _SItems.UpdateStockItemAmount(order);
        //    Assert.AreEqual(2, numOfRows);
        //}

        //[TestMethod]
        //public void TestUpdateOrderState()
        //{
        //    OrderHeader order = _ordersRepo.GetOrderHeader(1);
        //    int numOfRows = _ordersRepo.UpdateOrderState(order);
        //    Assert.AreEqual(1, numOfRows);
        //}

        //[TestMethod]
        //public void DeleteOrderHeaderAndOrderItems()
        //{
        //    OrderHeader order = _ordersRepo.GetOrderHeader(1);
        //    int numOfRows = _ordersRepo.DeleteOrderHeaderAndOrderItems(1);
        //    Assert.AreEqual(3, numOfRows);
        //}

    }
}
