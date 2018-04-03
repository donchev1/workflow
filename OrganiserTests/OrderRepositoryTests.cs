using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Organiser.Models;

namespace OrganiserTests
{
    [TestClass]
    public class OrderRepositoryTests
    {
        private readonly AppDbContext _appDbContext;

        [TestMethod]
        public void GetAllActiveOrdersForLocation()
        {
            var orderRepo = IOrderRepository;
            
        }
    }
}
