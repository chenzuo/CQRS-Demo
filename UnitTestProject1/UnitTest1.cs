using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private readonly FakeBus _bus;
        private readonly ReadModelFacade _readmodel;

        public UnitTest1()
        {
            var bus = new FakeBus();
            var storage = new EventStore(bus);
            var rep = new Repository<InventoryItem>(storage);
            var commands = new InventoryCommandHandlers(rep);

            bus.RegisterHandler<CheckInItemsToInventory>(commands.Handle);
            bus.RegisterHandler<CreateInventoryItem>(commands.Handle);
            bus.RegisterHandler<DeactivateInventoryItem>(commands.Handle);
            bus.RegisterHandler<RemoveItemsFromInventory>(commands.Handle);
            bus.RegisterHandler<RenameInventoryItem>(commands.Handle);

            var detail = new InventoryItemDetailView();
            bus.RegisterHandler<InventoryItemCreated>(detail.Handle);
            bus.RegisterHandler<InventoryItemDeactivated>(detail.Handle);
            bus.RegisterHandler<InventoryItemRenamed>(detail.Handle);
            bus.RegisterHandler<ItemsCheckedInToInventory>(detail.Handle);
            bus.RegisterHandler<ItemsRemovedFromInventory>(detail.Handle);

            var list = new InventoryListView();
            bus.RegisterHandler<InventoryItemCreated>(list.Handle);
            bus.RegisterHandler<InventoryItemRenamed>(list.Handle);
            bus.RegisterHandler<InventoryItemDeactivated>(list.Handle);

            _bus = bus;
            _readmodel = new ReadModelFacade();
        }

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            Console.WriteLine("AssemblyInitialize()");
            //MessageBox.Show("Assembly Init");
        }

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Console.WriteLine("ClassInitialize()");
            // MessageBox.Show("ClassInit");
        }

        [TestInitialize()]
        public void Initialize()
        {
            Console.WriteLine("TestInitialize()");
            //MessageBox.Show("TestMethodInit");
        }

        [TestCleanup()]
        public void Cleanup()
        {
            Console.WriteLine("Cleanup()");
            //MessageBox.Show("TestMethodCleanup");
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            Console.WriteLine("ClassCleanup()");
            //MessageBox.Show("ClassCleanup");
        }

        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("AssemblyCleanup()");
            //MessageBox.Show("AssemblyCleanup");
        }

        [TestMethod, STAThread]
        public void TestMethod1()
        {
            AsyncContext.Run(() =>
            {
                // Act
                _bus.Send(new CreateInventoryItem(Guid.NewGuid(), "hello world"));
                IEnumerable<InventoryItemListDto> enumerable = _readmodel.GetInventoryItems();
                List<InventoryItemListDto> asList = enumerable.ToList();
                Console.WriteLine(asList.Count);
            });

            // Assert
            Console.WriteLine("TestMethod1()");
        }

        [TestMethod]
        public async Task TestMethod2()
        {
            await Task.CompletedTask;
        }

        private Task<Task> DoWorkAsync()
        {
            //create a task completion source
            //the type of the result value must be the same
            //as the type in the returning Task
            TaskCompletionSource<Task> tcs = new TaskCompletionSource<Task>();
            Task.Run(() =>
            {
                _bus.Send(new CreateInventoryItem(Guid.NewGuid(), "hello world"));
                tcs.SetResult(Task.CompletedTask);
            });
            //return the Task
            return tcs.Task;
        }


        //[TestMethod]
        //public void TestMethod3()
        //{
        //    GeneralThreadAffineContext.Run(() =>
        //    {
        //        viewModel = new MyViewModel();
        //        viewModel.Run();
        //    });
        //    //Assert something here
        //}
    }

}
