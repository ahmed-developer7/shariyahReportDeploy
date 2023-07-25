using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using HtmltoWordConverter.Models;
using System.Collections.Generic;
using System.Linq;

namespace HtmltoWordConverter
{
    public class DataInitializationService : IDataInitializationService
    {
        public string companyName { get; set; }
        private bool isEditable { get; set; } = false;
        private Report shariyahReport;

        //private readonly YourDbContext _dbContext;

        //public DataInitializationService(YourDbContext dbContext)
        //{
        //    _dbContext = dbContext;
        //}

        //public void InitializeData()
        //{
        //    // Initialize data for entities
        //    getCompanies();
        //    getClients();
        //    getDepartments();
        //    getObservations();
        //    // Add more initialization methods as needed
        //}

        public List<Company> GetCompanies()
        {
            var companyList = new List<Company>
        {
            new Company { companyId = 1, Name = "Alpha Tech" },
            new Company { companyId = 2, Name = "Beta Tech" },
            new Company { companyId = 3, Name = "Gamma Tech" },
            new Company { companyId = 4, Name = "Shariyah Tech" }
        };
            return companyList;
            //// Add companies to the context and save changes
            //_dbContext.Companies.AddRange(companyList);
            //_dbContext.SaveChanges();
        }

        public List<Client> getClients()
        {
            var clientList = new List<Client>();
            var client1 = new Client()
            {
                clientId = 1,
                Name = "Ahmed Ali"
            };
            clientList.Add(client1);

            var client2 = new Client()
            {
                clientId = 2,
                Name = "Ragnar Lorth"
            };
            clientList.Add(client2);

            var client3 = new Client()
            {
                clientId = 3,
                Name = "Rollo Wessey"
            };
            clientList.Add(client3);

            var client4 = new Client()
            {
                clientId = 4,
                Name = "Arjun Kumar"
            };
            clientList.Add(client4);

            return clientList;
            // Initialize clients
        }

        public List<Department> getDepartments()
        {
            var departmentList = new List<Department>
            {
                new Department { departmentId = 1, Name = "Electrical" },
                new Department { departmentId = 2, Name = "Software" },
                new Department { departmentId = 3, Name = "Mechanical" }
            };
            return departmentList;
            // Initialize departments
        }

        public List<Observation1> getObservations()
        {
            var oblist = new List<Observation1>
            {
                new Observation1 { observationId = 1, Name = "observation 1" },
                new Observation1 { observationId = 2, Name = "observation 2" },
            };
            return oblist;

            // Initialize observations
        }

        public List<Products> GetProducts()
        {
            var productList = new List<Products>();
            for (int i = 1; i <= 5; i++)
            {
                var product = new Products
                {
                    id = i,
                    name = "Product " + i
                };

                productList.Add(product);
            }
            return productList;
        }
        public List<SubDepartment> GetSubDepartments(int id)
        {
            var subDepartments = new List<SubDepartment>();
            if (id == 1)
            {
                subDepartments.Add(new SubDepartment { departmentId = 1, Name = "sub1", SubDepartmentId = 1 });
                subDepartments.Add(new SubDepartment { departmentId = 1, Name = "sub2", SubDepartmentId = 2 });
            }
            else if (id == 2)
            {
                subDepartments.Add(new SubDepartment { departmentId = 2, Name = "sub3", SubDepartmentId = 3 });
                subDepartments.Add(new SubDepartment { departmentId = 2, Name = "sub4", SubDepartmentId = 4 });
            }

            return subDepartments;
        }

        public bool isFormEditable()
        {
            return this.isEditable;
        }
        public void changeFormTypeEditAndPreview(bool isEditable)
        {   
            this.isEditable = isEditable;
        }

        public Report GetShariyahReport()
        {
            return shariyahReport;
        }

        public void SetShariyahReport(Report report)
        {
            shariyahReport = report;
        }
    }

}
