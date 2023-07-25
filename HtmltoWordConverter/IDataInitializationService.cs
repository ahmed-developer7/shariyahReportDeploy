using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using HtmltoWordConverter.Models;
using System.Collections.Generic;

namespace HtmltoWordConverter
{
    public interface IDataInitializationService
    {
        string companyName { get; set; }
        List<Company> GetCompanies();
        List<Client> getClients();
        List<Department> getDepartments();
        List<Observation1> getObservations();
        List<Products> GetProducts();
        List<SubDepartment> GetSubDepartments(int id);

        bool isFormEditable();
        void changeFormTypeEditAndPreview(bool isEditable);
        Report GetShariyahReport();
        void SetShariyahReport(Report report);

        //void InitializeData();
    }
}
