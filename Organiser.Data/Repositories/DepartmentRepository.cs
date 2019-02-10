using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Organiser.Data.Context;
using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public class DepartmentRepository : Repository<AppDbContext, Department>
    {
        private AppDbContext _context;

        public DepartmentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public List<Department> GetLocations(List<int> locationIds)
        {
            DataTable dboList = new DataTable();
            dboList.Columns.Add(new DataColumn("ID", typeof(int)));
            foreach (var id in locationIds)
            {
                dboList.Rows.Add(id);
            }

            SqlParameter param1 = new SqlParameter("@List", dboList);
            _context.Database.ExecuteSqlCommand("spGetLocationNamesByIds @List", param1);

            https://stackoverflow.com/questions/14735477/get-return-value-from-stored-procedure

            return new List<Department>();
        }
    }
}
