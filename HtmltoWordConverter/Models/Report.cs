using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;

namespace HtmltoWordConverter.Models
{
    public class Report
    {
        public Client client { get; set; }
        public Company company { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public List<DepartmentVM> DepartmentSection { get; set; }
        public List<Products> products { get; set; }
        public string language { get; set; }
        public Boolean mergeObservations { get; set; }

    }

    public class DepartmentVM
    {
        public int departmentId { get; set; }
        public string departmentName { get; set; }
        public string departmentDescription { get; set; }
        public int SubDepartmentId { get; set; }
        public string SubDepartmentName { get; set; }
        public string SubDepartmentDescription { get; set; }
        public List<ObservationVM> ObservationVM { get; set; }
        public Boolean mergeObservations { get; set; }
    }
    public class ObservationVM
    {
        public int ObservationId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string impact { get; set; }
        public string recommendation { get; set; }
        public string rootCause { get; set; }
        public string ObservationRisk { get; set; }
        public string managementResponse { get; set; }
        public string responsibility { get; set; }
        public DateTime targetDate { get; set; }
        public List<string> ObservationSamples { get; set; }
        public List<Reference> references { get; set; }

    }


    public class Client
    {
        public int clientId { get; set; }
        public string Name { get; set; }
    }

    public class Reference
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }
    public class Observation1
    {
        public int observationId { get; set; }
        public string Name { get; set; }
    }
    public class Company
    {
        public int companyId { get; set; }
        public string Name { get; set; }
    }
    public class Department
    {
        public int departmentId { get; set; }
        public string Name { get; set; }
    }
    public class SubDepartment
    {
        public int SubDepartmentId { get; set; }
        public int departmentId { get; set; }
        public string Name { get; set; }
    }

    public class Products
    {
        public int id { get; set; }
        public string name { get; set; }
        public int year { get; set; }
        public int population { get; set; }
        public int sample { get; set; }
    }

    //public class CascadingDropdownViewModel
    //{
    //    public int SelecteddepartmentId { get; set; }
    //    public int SelectedSubDepartmentId { get; set; }
    //    public IEnumerable<SelectListItem> Departments { get; set; }
    //}

    //public class CascadingDropdownsViewModel1
    //{
    //    public int SelectedCountryId { get; set; }
    //    public int SelectedCityId { get; set; }
    //    public IEnumerable<SelectListItem> Countries { get; set; }
    //}

    //public class DynamicFormElement
    //{
    //    public int SelectedCountryId { get; set; }
    //    public int SelectedCityId { get; set; }
    //}
}

