using HtmltoWordConverter.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using Aspose.Words;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.CodeAnalysis.CSharp;
using DocumentFormat.OpenXml.Math;
using Xceed.Words.NET;
using Microsoft.Extensions.Primitives;

namespace HtmltoWordConverter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IWebHostEnvironment _hostingEnvironment;
        private IDataInitializationService _dataInitializationService;
        private List<Company> companies = new List<Company>();
        private List<Client> clients = new List<Client>();
        private List<Department> departments = new List<Department>();
        private List<Observation1> observations = new List<Observation1>();
        private List<Products> products = new List<Products>();
        private List<SubDepartment> subDepartment = new List<SubDepartment>();
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment, IDataInitializationService dataInitializationService)
        {
            _hostingEnvironment = environment;
            _logger = logger;
            _dataInitializationService = dataInitializationService;
            companies = _dataInitializationService.GetCompanies();
            clients = _dataInitializationService.getClients();
            departments = _dataInitializationService.getDepartments();
            observations = _dataInitializationService.getObservations();
            products = _dataInitializationService.GetProducts();
        }

        public IActionResult Index()
        {
            ////string htmlFilePath = "D:\\Saad\\fatima\\HtmltoWordConverterProject\\HtmltoWordConverter\\wwwroot\\EmailTemplate.html";
            //string htmlFilePath = "D:\\Saad\\fatima\\HtmltoWordConverterProject\\HtmltoWordConverter\\wwwroot\\Audit-Report-Tempalte-converio.html";
            //string wordFilePath = "D:\\Saad\\fatima\\HtmltoWordConverterProject\\HtmltoWordConverter\\wwwroot\\EmailTemplate2.docx";

            //// Load the HTML file
            //Aspose.Words.Document doc = new Aspose.Words.Document(htmlFilePath);

            //// Save the document as Word file
            //doc.Save(wordFilePath, SaveFormat.Docx);
            //return View();
            //return RedirectToAction("Create");
            return RedirectToAction("Privacy");
        }


        public IActionResult Create()
        {
            var report = new Report();

            ViewBag.companies = companies.Select(c => new SelectListItem { Value = c.companyId.ToString(), Text = c.Name }).ToList();
            ViewBag.clients = clients.Select(c => new SelectListItem { Value = c.clientId.ToString(), Text = c.Name }).ToList();
            ViewBag.departments = departments.Select(d => new SelectListItem { Value = d.departmentId.ToString(), Text = d.Name }).ToList();
            ViewBag.Observation = observations.Select(d => new SelectListItem { Value = d.observationId.ToString(), Text = d.Name }).ToList();
            ViewBag.products = products.Select(d => new SelectListItem { Value = d.id.ToString(), Text = d.name }).ToList();
            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Report report, List<int> MergedObservations)
        {
            //if (ModelState.IsValid)
            //{

            var company = this.companies.FirstOrDefault(c => c.companyId == report.company.companyId);
            if (company != null)
            {
                report.company.Name = company.Name;
            }
            var client = this.clients.FirstOrDefault(c => c.clientId == report.client.clientId);
            if (client != null)
            {
                report.client.Name = client.Name;
            }
            for (int i = 0; i < report.DepartmentSection.Count; i++)
            {
                var dept = report.DepartmentSection[i];
                var department = this.departments.FirstOrDefault(c => c.departmentId == dept.departmentId);
                if (department != null)
                {
                    report.DepartmentSection[i].departmentName = department.Name;
                    if (dept.SubDepartmentId != 0)
                    {
                        var subDept = _dataInitializationService.GetSubDepartments(dept.departmentId);
                        var subDepartment = subDept.FirstOrDefault(c => c.departmentId == dept.departmentId);
                        if (subDepartment != null)
                        {
                            report.DepartmentSection[i].SubDepartmentName = subDepartment.Name;
                        }
                    }
                    for (int j = 0; j < report.DepartmentSection[i].ObservationVM.Count; j++)
                    {
                        var observ = report.DepartmentSection[i].ObservationVM[j];
                        var observation = this.observations.FirstOrDefault(c => c.observationId == observ.ObservationId);
                        if (observation != null)
                        {
                            report.DepartmentSection[i].ObservationVM[j].title = observation.Name;
                        }
                    }

                }
            }

            
            for (int i = 0; i < report.products.Count; i++)
            {
                var prod = report.products[i];
                var product = this.products.FirstOrDefault(c => c.id == prod.id);
                if (product != null)
                {
                    report.products[i].name = product.name;
                }
            }

            var FilePath = Path.Combine(_hostingEnvironment.WebRootPath, "EmailTemplate.html");
            StreamReader str = new StreamReader(FilePath);
            string ReportText = str.ReadToEnd();
            str.Close();

            ReportText = ReportText.Replace("CompanyName", report.company.Name);
            ReportText = ReportText.Replace("[RequestDescription]", "test req");
            ReportText = ReportText.Replace("[link]", "test req");
            ReportText = ReportText.Replace("[imageLogo]", "test req");
            //ViewBag.htmltext = ReportText;
            //ViewBag.path = FilePath;

            _dataInitializationService.companyName = report.company.Name + "_template.html";
            string newFilePath = Path.Combine(_hostingEnvironment.WebRootPath, _dataInitializationService.companyName);

            using (StreamWriter writer = new StreamWriter(newFilePath))
            {
                writer.Write(ReportText);
            }

            return RedirectToAction("Privacy");
            //}
            //return View(Report);
        }
        //public IActionResult GenerateWordDocument()
        //{
        //    string _fileCSS = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "style.css");
        //    string _strCSS = System.IO.File.ReadAllText(_fileCSS);

        //    string _baseURL = "http://localhost:1385/"; // Update with your actual base URL
        //    string _filename = $"{Guid.NewGuid()}.docx";

        //    string htmlRaw = @"..."; // Your HTML content here

        //    // Combine the CSS and HTML content into a complete HTML document
        //    StringBuilder strHTML = new StringBuilder();
        //    strHTML.Append("<html " +
        //        " xmlns:o='urn:schemas-microsoft-com:office:office'" +
        //        " xmlns:w='urn:schemas-microsoft-com:office:word'" +
        //        " xmlns='http://www.w3.org/TR/REC-html40'>" +
        //        "<head><title>Invoice Sample</title>");

        //    strHTML.Append("<xml><w:WordDocument>" +
        //        " <w:View>Print</w:View>" +
        //        " <w:Zoom>100</w:Zoom>" +
        //        " <w:DoNotOptimizeForBrowser/>" +
        //        " </w:WordDocument>" +
        //        " </xml>");

        //    strHTML.Append("<style>" + _strCSS + "</style></head>");
        //    strHTML.Append("<body><div class='page-settings'>" + htmlRaw + "</div></body></html>");

        //    // Save the HTML content to a temporary file
        //    string tempFilePath = Path.GetTempFileName();
        //    File.WriteAllText(tempFilePath, strHTML.ToString());

        //    // Generate the Word document using DocX
        //    using (var doc = DocX.Create(_filename))
        //    {
        //        // Load the HTML content as a string
        //        var htmlContent = File.ReadAllText(tempFilePath);

        //        // Parse the HTML content to add it to the Word document
        //        var html = new DocX.HtmlImporter(doc);
        //        doc.InsertDocument(html.Import(htmlContent).InsertAtBookmark("", string.Empty), true);
        //        doc.Save();
        //    }

        //    // Delete the temporary HTML file
        //    System.IO.File.Delete(tempFilePath);

        //    // Return the Word document as a file attachment
        //    return PhysicalFile(_filename, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", _filename);
        //}
        public IActionResult ConvertToWord()
        {
            return SaveDOCX(_dataInitializationService.companyName, null, false, 0, 0, 0, 0);

            //string _baseURL = "http://localhost:1385/";
            //string _filename = System.Guid.NewGuid().ToString() + ".doc";
            //string htmlRaw = @"<table class='tbl'><thead><tr><th class='style0' colspan='2'> <img src='" + _baseURL + "img/logo.png' style='width: 180px;' /></th><th class='style1' colspan='4'><p style='font-size: 24px; padding-bottom: 2px; padding-top: 2px; font-weight: bold; margin-bottom: 1px;'>INVOICE</p> ID-2021-0024<br> Issue Date:21/09/2021<br> Delivery Date: 22/09/2021<br> Due Date:30/09/2021<br> <br><p style='font-size: 24px; padding-bottom: 2px; padding-top: 2px; font-weight: bold; margin-bottom: 1px;'>CLIENT DETAILS</p> Client 1<br> GST Number:XXXXXXXXXX</th></tr></thead><tbody><tr><td class='headstyle0' colspan='5' style='padding-top: 60px;'></td></tr><tr><td class='style3a'>ITEM</td><td class='style3a'>DESCRIPTION</td><td class='style3a'>QUANTITY</td><td class='style3a'>UNIT PRICE</td><td class='style3a'>TOTAL</td></tr><tr><td class='style3'>Item-1</td><td class='style3'>Description -1</td><td class='style3'>2 Pkt</td><td class='style3'>90.00</td><td class='style3b'>180.00</td></tr><tr><td class='style3'>Item-2</td><td class='style3'>Description-2</td><td class='style3'>5 Pkt</td><td class='style3'>35.00</td><td class='style3b'>175.00</td></tr><tr><td class='style3'>Item-3</td><td class='style3'>Description-3</td><td class='style3'>5 Kg</td><td class='style3'>50.00</td><td class='style3b'>250.00</td></tr><tr><td class='style3'>Item-4</td><td class='style3'>Description-4</td><td class='style3'>5 Kg</td><td class='style3'>150.00</td><td class='style3b'>750.00</td></tr><tr><td class='style3'>Item-5</td><td class='style3'>Description-5</td><td class='style3'>5 Kg</td><td class='style3'>100.00</td><td class='style3b'>500.00</td></tr><tr><td class='style0' colspan='2' rowspan='3'></td><td class='style3' colspan='2'>Total</td><td class='style3b'>1855.00</td></tr><tr><td class='style3' colspan='2'>GST@18%</td><td class='style3b'>333.90</td></tr><tr><td class='style3' colspan='2'>Net Payable Amount</td><td class='style3b'>2188.90</td></tr><tr><td class='style0' colspan='5' style='padding-top: 100px;'></td></tr><tr><td class='style0' colspan='5' style='background-color: aliceblue; border-radius: 2px;'><i>Note:Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</i></td></tr><tr><td class='style1' colspan='5' style='padding-top: 150px;'> Thank You<br> <b>CodeSample</b></td></tr></tbody></table>";

            //StringBuilder strHTML = new StringBuilder("");
            //strHTML.Append("<html " +
            //    " xmlns:o='urn:schemas-microsoft-com:office:office'" +
            //    " xmlns:w='urn:schemas-microsoft-com:office:word'" +
            //    " xmlns='http://www.w3.org/TR/REC-html40'>" +
            //    "<head><title>Invoice Sample</title>");

            //strHTML.Append("<xml><w:WordDocument>" +
            //    " <w:View>Print</w:View>" +
            //    " <w:Zoom>100</w:Zoom>" +
            //    " <w:DoNotOptimizeForBrowser/>" +
            //    " </w:WordDocument>" +
            //    " </xml>");

            //strHTML.Append("</head>");
            //strHTML.Append("<body><div class='page-settings'>" + htmlRaw + "</div></body></html>");


            ////using (StreamReader Reader = new StreamReader("D:\\Saad\\fatima\\HtmltoWordConverterProject\\HtmltoWordConverter\\wwwroot\\EmailTemplate.html"))
            ////{
            ////    StringBuilder Sb = new StringBuilder();
            ////    Sb.Append(Reader.ReadToEnd());



            ////}
            return View();
        }


        private static FileContentResult SaveDOCX(string fileName, string BodyText, bool isLandScape, double rMargin, double lMargin, double bMargin, double tMargin)
        {
            var documentPath = "D:\\Saad\\fatima\\HtmltoWordConverterProject\\HtmltoWordConverter\\wwwroot\\Doc4.docx";
            WordprocessingDocument document = WordprocessingDocument.Open(documentPath, true);
            MainDocumentPart mainDocumenPart = document.MainDocumentPart;

            //Place the HTML String into a MemoryStream Object
            using (StreamReader Reader = new StreamReader("D:\\Saad\\fatima\\HtmltoWordConverterProject\\HtmltoWordConverter\\wwwroot\\"+ fileName))
            {
                StringBuilder Sb = new StringBuilder();
                Sb.Append(Reader.ReadToEnd());
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Sb.ToString()));

                //Assign an HTML Section for the String Text
                string htmlSectionID = "MySection_" + Guid.NewGuid().ToString().Replace("-", "_");

                // Create alternative format import part.
                AlternativeFormatImportPart formatImportPart = mainDocumenPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html, htmlSectionID);

                // Feed HTML data into format import part (chunk).
                formatImportPart.FeedData(ms);
                AltChunk altChunk = new AltChunk();
                altChunk.Id = htmlSectionID;

                //Clear out the Document Body and Insert just the HTML string.  (This prevents an empty First Line)
                mainDocumenPart.Document.Body.RemoveAllChildren();
                mainDocumenPart.Document.Body.Append(altChunk);

                /*
                 Set the Page Orientation and Margins Based on Page Size
                 inch equiv = 1440 (1 inch margin)
                 */
                double width = 8.5 * 1440;
                double height = 11 * 1440;

                SectionProperties sectionProps = new SectionProperties();
                PageSize pageSize;
                if (isLandScape)
                    pageSize = new PageSize() { Width = (UInt32Value)height, Height = (UInt32Value)width, Orient = PageOrientationValues.Landscape };
                else
                    pageSize = new PageSize() { Width = (UInt32Value)width, Height = (UInt32Value)height, Orient = PageOrientationValues.Portrait };

                rMargin = rMargin * 1440;
                lMargin = lMargin * 1440;
                bMargin = bMargin * 1440;
                tMargin = tMargin * 1440;

                PageMargin pageMargin = new PageMargin() { Top = (Int32)tMargin, Right = (UInt32Value)rMargin, Bottom = (Int32)bMargin, Left = (UInt32Value)lMargin, Header = (UInt32Value)360U, Footer = (UInt32Value)360U, Gutter = (UInt32Value)0U };

                sectionProps.Append(pageSize);
                sectionProps.Append(pageMargin);
                mainDocumenPart.Document.Body.Append(sectionProps);

                //Saving/Disposing of the created word Document
                document.MainDocumentPart.Document.Save();
                document.Dispose();
            }
            // Return the Word document as a file attachment for download
            var fileBytes = System.IO.File.ReadAllBytes(documentPath);
            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            FileContentResult fileContentResult = new FileContentResult(fileBytes, contentType)
            {
                FileDownloadName = "YourFileName.docx"
            };

            // Delete the generated Word document after it has been returned as a download
            //System.IO.File.Delete(documentPath);

            // Return the file as a download
            return fileContentResult;
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            // SaveDOCX("company", null, false, 0, 0, 0,0);
          
        }
        public IActionResult Privacy()
        {
            Report report = new Report
            {
                client = new Client
                {
                    clientId = 1,
                    Name = "ABC Company"
                },
                company = new Company
                {
                    companyId = 1,
                    Name = "XYZ Corporation"
                },
                startDate = DateTime.Now.AddDays(-7),
                endDate = DateTime.Now,
                DepartmentSection = new List<DepartmentVM>
                {
                    new DepartmentVM
                    {
                        departmentId = 1,
                        departmentName = "Department A",
                        departmentDescription = "The audit process included the review of the approved product represented by Ijarah documents in addition to the contracts executed by the\r\nCompany with the relevant parties. A sample was selected from the transactions which represent 100% of total number of transactions to\r\nensure that they are being carried out in accordance with the Shari’a guidelines approved by the SSB. During the assignment, we found the\r\nfollowing observations",
                        SubDepartmentId = 1,
                        SubDepartmentName = "Subdepartment A1",
                        SubDepartmentDescription = "The audit process included the review of the approved product represented by Ijarah documents in addition to the contracts executed by the\r\nCompany with the relevant parties. A sample was selected from the transactions which represent 100% of total number of transactions to\r\nensure that they are being carried out in accordance with the Shari’a guidelines approved by the SSB. During the assignment, we found the\r\nfollowing observations",
                        ObservationVM = new List<ObservationVM>
                        {
                            new ObservationVM
                            {
                                ObservationId = 1,
                                title = "Observation 1",
                                description = "This observation is regarding a safety issue that was identified during the inspection. It involves a potential hazard in the manufacturing process that could result in accidents. It is crucial to address this issue promptly to ensure the well-being of the workers.",
                                impact = "High to implement immediate corrective actions to eliminate the identified safety hazard. This may involve modifying equipment",
                                recommendation = "It is recommended to implement immediate corrective actions to eliminate the identified safety hazard. This may involve modifying equipment, improving safety protocols, and providing proper training to the employees.",
                                rootCause = "The root cause of this safety issue can be attributed to inadequate maintenance of equipment and a lack of proper safety training for the workers.",
                                ObservationRisk = "Medium",
                                managementResponse = "Noted. The company will use the stamp mechanism in the same cases.",
                                responsibility = "MSRD",
                                targetDate = DateTime.Now,
                                ObservationSamples = new List<string> { "011-I", "011-I", "011-I", "011-I", "011-I", "011-I", "011-I", "011-I" },
                                references = new List<Reference>
                                {
                                    new Reference
                                    {
                                        id = 1,
                                        title = "Reference 1",
                                        description = "For additional information on safety regulations and best practices, please refer to the Occupational Safety and Health Administration (OSHA) guidelines."
                                    },
                                    new Reference
                                    {
                                        id = 2,
                                        title = "Reference 2",
                                        description = "The National Institute for Occupational Safety and Health (NIOSH) publication on hazard control measures can also provide valuable insights."
                                    }
                                }
                            },
                            new ObservationVM
                            {
                                ObservationId = 2,
                                title = "Observation 2",
                                description = "This observation pertains to a minor quality issue detected during the quality control process. Although it does not pose an immediate threat, it can affect the overall product quality and customer satisfaction if not addressed appropriately.",
                                impact = "Low to implement immediate corrective actions to eliminate the identified safety hazard. This may involve modifying equipment",
                                recommendation = "To mitigate the impact of this quality issue, it is recommended to closely monitor the affected production line and conduct regular inspections. Any deviations from the quality standards should be promptly identified and rectified.",
                                rootCause = "The root cause of this quality issue is attributed to a malfunctioning component in the production equipment, which leads to occasional variations in product specifications.",
                                ObservationRisk = "Low",
                                 managementResponse = "Noted. The company will use the stamp mechanism in the same cases.",
                                responsibility = "MSRD",
                                targetDate = DateTime.Now,
                                ObservationSamples = new List<string> { "011-I", "011-I", "011-I", "011-I", "011-I", "011-I" },
                                references = new List<Reference>
                                {
                                    new Reference
                                    {
                                        id = 3,
                                        title = "Reference 3",
                                        description = "To address similar quality issues, you can refer to the International Organization for Standardization (ISO) quality management standards, specifically ISO 9001."
                                    }
                                }
                            }
                        },
                    },
                    new DepartmentVM
                    {
                        departmentId = 1,
                        departmentName = "Department B",
                        departmentDescription = "The audit process included the review of the approved product represented by Ijarah documents in addition to the contracts executed by the\r\nCompany with the relevant parties. A sample was selected from the transactions which represent 100% of total number of transactions to\r\nensure that they are being carried out in accordance with the Shari’a guidelines approved by the SSB. During the assignment, we found the\r\nfollowing observations",
                        SubDepartmentId = 2,
                        SubDepartmentName = "Subdepartment B2",
                        SubDepartmentDescription = "The audit process included the review of the approved product represented by Ijarah documents in addition to the contracts executed by the\r\nCompany with the relevant parties. A sample was selected from the transactions which represent 100% of total number of transactions to\r\nensure that they are being carried out in accordance with the Shari’a guidelines approved by the SSB. During the assignment, we found the\r\nfollowing observations",
                        ObservationVM = new List<ObservationVM>
                        {
                            new ObservationVM
                            {
                                ObservationId = 1,
                                title = "Observation 1",
                                description = "This observation is regarding a safety issue that was identified during the inspection. It involves a potential hazard in the manufacturing process that could result in accidents. It is crucial to address this issue promptly to ensure the well-being of the workers.",
                                impact = "High to implement immediate corrective actions to eliminate the identified safety hazard. This may involve modifying equipment",
                                recommendation = "It is recommended to implement immediate corrective actions to eliminate the identified safety hazard. This may involve modifying equipment, improving safety protocols, and providing proper training to the employees.",
                                rootCause = "The root cause of this safety issue can be attributed to inadequate maintenance of equipment and a lack of proper safety training for the workers.",
                                ObservationRisk = "Medium",
                                managementResponse = "Noted. The company will use the stamp mechanism in the same cases.",
                                responsibility = "MSRD",
                                targetDate = DateTime.Now,
                                ObservationSamples = new List<string> { "011-I", "011-I", "011-I", "011-I", "011-I", "011-I", "011-I", "011-I" },
                                references = new List<Reference>
                                {
                                    new Reference
                                    {
                                        id = 1,
                                        title = "Reference 1",
                                        description = "For additional information on safety regulations and best practices, please refer to the Occupational Safety and Health Administration (OSHA) guidelines."
                                    },
                                    new Reference
                                    {
                                        id = 2,
                                        title = "Reference 2",
                                        description = "The National Institute for Occupational Safety and Health (NIOSH) publication on hazard control measures can also provide valuable insights."
                                    }
                                }
                            },
                            new ObservationVM
                            {
                                ObservationId = 2,
                                title = "Observation 2",
                                description = "This observation pertains to a minor quality issue detected during the quality control process. Although it does not pose an immediate threat, it can affect the overall product quality and customer satisfaction if not addressed appropriately.",
                                impact = "Low to implement immediate corrective actions to eliminate the identified safety hazard. This may involve modifying equipment",
                                recommendation = "To mitigate the impact of this quality issue, it is recommended to closely monitor the affected production line and conduct regular inspections. Any deviations from the quality standards should be promptly identified and rectified.",
                                rootCause = "The root cause of this quality issue is attributed to a malfunctioning component in the production equipment, which leads to occasional variations in product specifications.",
                                ObservationRisk = "Low",
                                 managementResponse = "Noted. The company will use the stamp mechanism in the same cases.",
                                responsibility = "MSRD",
                                targetDate = DateTime.Now,
                                ObservationSamples = new List<string> { "011-I", "011-I", "011-I", "011-I", "011-I", "011-I" },
                                references = new List<Reference>
                                {
                                    new Reference
                                    {
                                        id = 3,
                                        title = "Reference 3",
                                        description = "To address similar quality issues, you can refer to the International Organization for Standardization (ISO) quality management standards, specifically ISO 9001."
                                    }
                                }
                            }
                        },
                    },
                    new DepartmentVM
                    {
                        departmentId = 2,
                        departmentName = "Department C",
                        departmentDescription = "The audit process included the review of the approved product represented by Ijarah documents in addition to the contracts executed by the\r\nCompany with the relevant parties. A sample was selected from the transactions which represent 100% of total number of transactions to\r\nensure that they are being carried out in accordance with the Shari’a guidelines approved by the SSB. During the assignment, we found the\r\nfollowing observations",
                        SubDepartmentId = 3,
                        SubDepartmentName = "Subdepartment C1",
                        SubDepartmentDescription = "The audit process included the review of the approved product represented by Ijarah documents in addition to the contracts executed by the\r\nCompany with the relevant parties. A sample was selected from the transactions which represent 100% of total number of transactions to\r\nensure that they are being carried out in accordance with the Shari’a guidelines approved by the SSB. During the assignment, we found the\r\nfollowing observations",
                        ObservationVM = new List<ObservationVM>
                        {
                            new ObservationVM
                            {
                                ObservationId = 1,
                                title = "Observation 1",
                                description = "This observation is regarding a safety issue that was identified during the inspection. It involves a potential hazard in the manufacturing process that could result in accidents. It is crucial to address this issue promptly to ensure the well-being of the workers.",
                                impact = "High to implement immediate corrective actions to eliminate the identified safety hazard. This may involve modifying equipment",
                                recommendation = "It is recommended to implement immediate corrective actions to eliminate the identified safety hazard. This may involve modifying equipment, improving safety protocols, and providing proper training to the employees.",
                                rootCause = "The root cause of this safety issue can be attributed to inadequate maintenance of equipment and a lack of proper safety training for the workers.",
                                ObservationRisk = "Medium",
                                managementResponse = "Noted. The company will use the stamp mechanism in the same cases.",
                                responsibility = "MSRD",
                                targetDate = DateTime.Now,
                                ObservationSamples = new List<string> { "011-I", "011-I", "011-I", "011-I", "011-I", "011-I", "011-I", "011-I" },
                                references = new List<Reference>
                                {
                                    new Reference
                                    {
                                        id = 1,
                                        title = "Reference 1",
                                        description = "For additional information on safety regulations and best practices, please refer to the Occupational Safety and Health Administration (OSHA) guidelines."
                                    },
                                    new Reference
                                    {
                                        id = 2,
                                        title = "Reference 2",
                                        description = "The National Institute for Occupational Safety and Health (NIOSH) publication on hazard control measures can also provide valuable insights."
                                    }
                                }
                            },
                            new ObservationVM
                            {
                                ObservationId = 2,
                                title = "Observation 2",
                                description = "This observation pertains to a minor quality issue detected during the quality control process. Although it does not pose an immediate threat, it can affect the overall product quality and customer satisfaction if not addressed appropriately.",
                                impact = "Low to implement immediate corrective actions to eliminate the identified safety hazard. This may involve modifying equipment",
                                recommendation = "To mitigate the impact of this quality issue, it is recommended to closely monitor the affected production line and conduct regular inspections. Any deviations from the quality standards should be promptly identified and rectified.",
                                rootCause = "The root cause of this quality issue is attributed to a malfunctioning component in the production equipment, which leads to occasional variations in product specifications.",
                                ObservationRisk = "Low",
                                 managementResponse = "Noted. The company will use the stamp mechanism in the same cases.",
                                responsibility = "MSRD",
                                targetDate = DateTime.Now,
                                ObservationSamples = new List<string> { "011-I", "011-I", "011-I", "011-I", "011-I", "011-I" },
                                references = new List<Reference>
                                {
                                    new Reference
                                    {
                                        id = 3,
                                        title = "Reference 3",
                                        description = "To address similar quality issues, you can refer to the International Organization for Standardization (ISO) quality management standards, specifically ISO 9001."
                                    }
                                }
                            }
                        },
                    }
                },
                products = new List<Products>
                {
                    new Products
                    {
                        id = 1,
                        name = "Product A",
                        year = 2021,
                        population = 1000,
                        sample = 100
                    },
                    new Products
                    {
                        id = 2,
                        name = "Product B",
                        year = 2022,
                        population = 2000,
                        sample = 200
                    }
                },
                    language = "English",
                    //mergeObservations = true
                };

            _dataInitializationService.SetShariyahReport(report);
            _dataInitializationService.changeFormTypeEditAndPreview(false);
            var FilePath = Path.Combine(_hostingEnvironment.WebRootPath, "EmailTemplate.html");
            StreamReader str = new StreamReader(FilePath);
            string ReportText = str.ReadToEnd();
            str.Close();
            
            var PRODUCT_TABLE = productTable(report);


            ReportText = ReportText.Replace("COMPANY_NAME", report.company.Name);
            ReportText = ReportText.Replace("CLIENT_NAME", report.client.Name);
            ReportText = ReportText.Replace("START_DATE", report.startDate.ToShortDateString());
            ReportText = ReportText.Replace("END_DATE", report.endDate.ToShortDateString());
            
            ReportText = ReportText.Replace("PRODUCT_TABLE", PRODUCT_TABLE);

            var OBSERVATION_TABLES = ObservationTables(report);
            var REFERENCES = ReferanceTable(report);

            ReportText = ReportText.Replace("OBSERVATION_TABLES", OBSERVATION_TABLES);
            ReportText = ReportText.Replace("REFERENCES", REFERENCES);
            //ViewBag.htmltext = ReportText;
            //ViewBag.path = FilePath;


            _dataInitializationService.companyName = report.company.Name + "_template.html";
            string newFilePath = Path.Combine(_hostingEnvironment.WebRootPath, _dataInitializationService.companyName);

            if (System.IO.File.Exists(newFilePath))
            {
                System.IO.File.Delete(newFilePath);
            }

            using (StreamWriter writer = new StreamWriter(newFilePath))
            {
                writer.Write(ReportText);
            }


            ViewBag.companyName = _dataInitializationService.companyName;
            return View();
        }

        public IActionResult Edit()
        {
            _dataInitializationService.changeFormTypeEditAndPreview(true);
            Report report = _dataInitializationService.GetShariyahReport();

            var FilePath = Path.Combine(_hostingEnvironment.WebRootPath, "EmailTemplate.html");
            StreamReader str = new StreamReader(FilePath);
            string ReportText = str.ReadToEnd();
            str.Close();

            var PRODUCT_TABLE = productTable(report);

            ReportText = ReportText.Replace("COMPANY_NAME", report.company.Name);
            ReportText = ReportText.Replace("CLIENT_NAME", report.client.Name);
            ReportText = ReportText.Replace("START_DATE", report.startDate.ToShortDateString());
            ReportText = ReportText.Replace("END_DATE", report.endDate.ToShortDateString());

            ReportText = ReportText.Replace("PRODUCT_TABLE", PRODUCT_TABLE);

            var OBSERVATION_TABLES = ObservationTables(report);
            var REFERENCES = ReferanceTable(report);

            ReportText = ReportText.Replace("OBSERVATION_TABLES", OBSERVATION_TABLES);
            ReportText = ReportText.Replace("REFERENCES", REFERENCES);
            //ViewBag.htmltext = ReportText;
            //ViewBag.path = FilePath;


            _dataInitializationService.companyName = report.company.Name + "_template.html";
            string newFilePath = Path.Combine(_hostingEnvironment.WebRootPath, _dataInitializationService.companyName);

            if (System.IO.File.Exists(newFilePath))
            {
                System.IO.File.Delete(newFilePath);
            }

            using (StreamWriter writer = new StreamWriter(newFilePath))
            {
                writer.Write(ReportText);
            }

            ViewBag.companyName = _dataInitializationService.companyName;
            return View();
        }

        private string ObservationTables(Report report)
        {
            //var obs = report.ObservationVM[0];
            StringBuilder dynamicRowBuilder = new StringBuilder();
            foreach (var (dept, index) in report.DepartmentSection.Select((dept, index) => (dept, index)))
                //foreach (var dept in report.DepartmentSection)
            {
                dynamicRowBuilder.Append($"" +
                       $"<p class=\"s18\" style=\"padding-top: 4pt;padding-left: 14pt;text-indent: 0pt;text-align: justify;\">{dept.departmentName}</p>\r\n    " +
                       $"<p style=\"padding-top: 8pt;padding-left: 14pt;text-indent: 0pt;line-height: 119%;text-align: justify;\">{dept.departmentDescription}:</p>\r\n    " +
                       $"<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n    " +
                       $"");
                foreach (var (observationVM, indexObs) in dept.ObservationVM.Select((observationVM, index) => (observationVM, index)))
                {

                    //Observation, Risk Rating, Impact, Recommendation
                    if (_dataInitializationService.isFormEditable())
                    {
                        dynamicRowBuilder.Append($"     <p class=\"s10\" style=\"padding-top: 4pt;padding-left: 33pt;text-indent: -19pt;text-align: left;\">\r\n                    {observationVM.title}</p>\r\n    <p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n    <p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n    " +
                            $"<table style=\"border-collapse:collapse;margin-left:14pt\" cellspacing=\"0\">\r\n        <tr style=\"height:18pt\">\r\n            <td style=\"width:271pt;border-top-style:solid;border-top-width:1pt;border-top-color:#4B4B4B\" rowspan=\"2\"\r\n                bgcolor=\"#F2F2F2\">\r\n                <p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n                <p class=\"s22\" style=\"padding-left: 5pt;text-indent: 0pt;text-align: left;\">Observation</p>\r\n            </td>\r\n            <td style=\"width:355pt;border-top-style:solid;border-top-width:1pt;border-top-color:#4B4B4B\" colspan=\"2\"\r\n                bgcolor=\"#365E90\">\r\n                <p class=\"s23\" style=\"padding-top: 3pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">Risk\r\n                    Rating: Moderate</p>\r\n            </td>\r\n        </tr>\r\n        " +
                            $"<tr style=\"height:18pt\">\r\n            <td style=\"padding-bottom: 8px;width: 250.328px;position: absolute;\" colspan=\"2\" bgcolor=\"#CCCCCC\">\r\n                <p class=\"s22\" style=\"padding-top: 3pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">Impact\r\n                    </p>\r\n            </td>\r\n            <td style=\"width:355pt\" colspan=\"2\" bgcolor=\"#CCCCCC\">\r\n                <p class=\"s22\" style=\"padding-top: 3pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">\r\n                    Recommendation</p>\r\n            </td>\r\n        </tr>\r\n        " +
                            $"<tr style=\"height:100pt\">\r\n            <td style=\"width:271pt\">\r\n                <textarea class=\"s22\" style=\"padding-top: 5pt;padding-left: 5pt;padding-right: 4pt;text-indent: 0pt;line-height: 114%;text-align: justify;\" rows=\"5\" cols=\"30\">{observationVM.description}</textarea>\r\n            </td>\r\n            <td style=\"width:186pt\" bgcolor=\"#F2F2F2\">\r\n                <textarea class=\"s22\" style=\"padding-top: 5pt;padding-left: 5pt;padding-right: 4pt;text-indent: 0pt;line-height: 114%;text-align: justify;\" rows=\"5\" cols=\"30\">{observationVM.impact}</textarea>\r\n            </td>\r\n            <td style=\"width:169pt\">\r\n                <textarea class=\"s22\" style=\"padding-top: 5pt;padding-left: 5pt;padding-right: 4pt;text-indent: 0pt;line-height: 114%;text-align: justify;\" rows=\"5\" cols=\"30\">{observationVM.recommendation}</textarea>\r\n            </td>\r\n        </tr>\r\n        " +
                            $"</table>");
                    } else
                    {
                        dynamicRowBuilder.Append($"     " +
                            $"    <p class=\"s19\" style=\"padding-left: 14pt;text-indent: 0pt;text-align: left;\"> {index + 1}.{indexObs + 1}. {observationVM.title}</p>\r\n" +
                            //$"<p class=\"s10\" style=\"padding-top: 4pt;padding-left: 33pt;text-indent: -19pt;text-align: left;\">\r\n                    {observationVM.title}</p>\r\n    " +
                            $"<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n    " +
                            $"<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n    " +
                            $"<table style=\"border-collapse:collapse;margin-left:14pt\" cellspacing=\"0\">\r\n        <tr style=\"height:18pt\">\r\n            <td style=\"width:271pt;border-top-style:solid;border-top-width:1pt;border-top-color:#4B4B4B\" rowspan=\"2\"\r\n                bgcolor=\"#F2F2F2\">\r\n                <p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n                <p class=\"s22\" style=\"padding-left: 5pt;text-indent: 0pt;text-align: left;\">Observation</p>\r\n            </td>\r\n            <td style=\"width:355pt;border-top-style:solid;border-top-width:1pt;border-top-color:#4B4B4B\" colspan=\"2\"\r\n                bgcolor=\"#365E90\">\r\n                <p class=\"s23\" style=\"padding-top: 3pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">Risk\r\n                    Rating: Moderate</p>\r\n            </td>\r\n        </tr>\r\n        " +
                            $"<tr style=\"height:18pt\">\r\n            <td style=\"padding-bottom: 8px;width: 250.328px;position: absolute;\" colspan=\"2\" bgcolor=\"#CCCCCC\">\r\n                <p class=\"s22\" style=\"padding-top: 3pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">Impact\r\n                    </p>\r\n            </td>\r\n            <td style=\"width:355pt\" colspan=\"2\" bgcolor=\"#CCCCCC\">\r\n                <p class=\"s22\" style=\"padding-top: 3pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">\r\n                    Recommendation</p>\r\n            </td>\r\n        </tr>\r\n        " +
                            $"<tr style=\"height:100pt\">\r\n            <td style=\"width:271pt\">\r\n                <p class=\"s22\"\r\n                    style=\"padding-top: 5pt;padding-left: 5pt;padding-right: 4pt;text-indent: 0pt;line-height: 114%;text-align: justify;\">\r\n                    {observationVM.description}</p>\r\n            </td>\r\n            <td style=\"width:186pt\" bgcolor=\"#F2F2F2\">\r\n                <p class=\"s22\"\r\n                    style=\"padding-top: 5pt;padding-left: 5pt;padding-right: 4pt;text-indent: 0pt;line-height: 114%;text-align: justify;\">\r\n                    {observationVM.impact}</p>\r\n            </td>\r\n            <td style=\"width:169pt\">\r\n                <p class=\"s22\"\r\n                    style=\"padding-top: 5pt;padding-left: 5pt;padding-right: 4pt;text-indent: 0pt;line-height: 114%;text-align: justify;\">\r\n                    {observationVM.recommendation}</p>\r\n            </td>\r\n        </tr>\r\n        " +
                            $"</table>");
                    }
                    
                    ////$"<tr style=\"height:18pt\">\r\n            <td style=\"width:626pt\" colspan=\"3\" bgcolor=\"#CCCCCC\">\r\n                <p class=\"s20\" style=\"padding-top: 3pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">Examples of\r\n                    transactions in which the observation occurred:</p>\r\n            </td>\r\n        </tr>\r\n    " +

                    //ObservationSamples
                    if (observationVM.ObservationSamples.Count > 0)
                    {
                        dynamicRowBuilder.Append($"<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n                " +
                            $"<table style=\"border-collapse:collapse;margin-left:5.96pt\" cellspacing=\"0\">\r\n                    " +
                            $"<tr style=\"height:16pt\">\r\n                        <td style=\"width:629pt\" colspan=\"8\" bgcolor=\"#CCCCCC\">\r\n                            <p class=\"s20\"\r\n                                style=\"padding-top: 2pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">Examples of\r\n                                transactions in which the observation occurred:\r\n                            </p>\r\n                        </td>\r\n                    </tr>" +
                            $"<tr style=\"height:16pt\">");
                        foreach (var sample in observationVM.ObservationSamples)
                        {
                            dynamicRowBuilder.Append($" <td\r\n                            style=\"width:79pt;border-right-style:solid;border-right-width:1pt;border-right-color:#666666\">\r\n                            " +
                                $"<p class=\"s20\"\r\n                                style=\"padding-top: 2pt;padding-left: 28pt;padding-right: 28pt;text-indent: 0pt;text-align: center;\">\r\n                                " +
                                $"{sample}</p>\r\n                        " +
                                $"</td>");
                        }
                        dynamicRowBuilder.Append($"</tr>\r\n                " +
                            $"</table>");
                    }

                    //references
                    if (observationVM.references.Count > 0)
                    {
                        dynamicRowBuilder.Append($"<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n    " +
                            $"<table style=\"border-collapse:collapse;margin-left:14pt\" cellspacing=\"0\">\r\n        " +
                            $"<tr style=\"height:18pt\">\r\n            " +
                            $"<td style=\"width:271pt\" bgcolor=\"#CCCCCC\">\r\n                <p class=\"s22\" style=\"padding-top: 3pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">Reference\r\n                </p>\r\n            </td>\r\n            " +
                            $"<td style=\"width:355pt\" colspan=\"3\" bgcolor=\"#CCCCCC\">\r\n                <p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n            </td>\r\n        " +
                            $"</tr>\r\n");
                        foreach (var reference in observationVM.references)
                        {
                            dynamicRowBuilder.Append($"<tr style=\"height:24pt\">" +
                                $"<td style=\"width:700pt\">\r\n                " +
                                $"<p class=\"s24\" style=\"padding-top: 5pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">{reference.description}</p>\r\n            </td>\r\n            " +
                                $"</tr>");

                        }
                        dynamicRowBuilder.Append($"</table>");
                    }

                    //Root Cause, Management Response, Responsibility, Target Date
                    dynamicRowBuilder.Append($"<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n    " +
                            $"<table style=\"border-collapse:collapse;margin-left:14pt\" cellspacing=\"0\">\r\n        " +
                            $"<tr style=\"height:18pt\">\r\n            " +
                            $"<td style=\"width:271pt\" bgcolor=\"#CCCCCC\">\r\n                <p class=\"s22\" style=\"padding-top: 3pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">Root Cause\r\n                </p>\r\n            </td>\r\n            <td style=\"width:355pt\" colspan=\"3\" bgcolor=\"#CCCCCC\">\r\n                <p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n            </td>\r\n        " +
                            $"</tr>\r\n        " +

                            $"<tr style=\"height:24pt\">\r\n            " +
                            $"<td style=\"width:700pt\">\r\n                <p class=\"s24\" style=\"padding-top: 5pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">{observationVM.rootCause}</p>\r\n            </td>\r\n            <td style=\"width:68pt\">\r\n                <p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n            </td>\r\n            <td style=\"width:118pt\">\r\n                <p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n            </td>\r\n            <td style=\"width:169pt\">\r\n                <p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n            </td>\r\n        " +
                            $"</tr>\r\n        " +

                            $"<tr style=\"height:18pt\">\r\n            " +
                            $"<td style=\"width:271pt\" bgcolor=\"#CCCCCC\">\r\n                <p class=\"s22\" style=\"padding-top: 3pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">Management\r\n                    Response</p>\r\n            </td>\r\n            <td style=\"width:68pt\" bgcolor=\"#CCCCCC\">\r\n                <p class=\"s22\" style=\"padding-top: 3pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">\r\n                    Responsibility</p>\r\n            </td>\r\n            <td style=\"width:118pt\" bgcolor=\"#CCCCCC\">\r\n                <p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n            </td>\r\n            <td style=\"width:169pt\" bgcolor=\"#CCCCCC\">\r\n                <p class=\"s22\" style=\"padding-top: 3pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">Target Date\r\n                </p>\r\n            </td>\r\n        " +
                            $"</tr>\r\n        " +

                            $"<tr style=\"height:23pt\">\r\n            " +
                            $"<td style=\"width:271pt\">\r\n                <p class=\"s22\" style=\"padding-top: 5pt;padding-left: 5pt;text-indent: 0pt;text-align: left;\">{observationVM.managementResponse}</p>\r\n            </td>\r\n            <td style=\"width:68pt\" bgcolor=\"#F2F2F2\">\r\n                <p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n            </td>\r\n            " +
                            $"<td style=\"width:118pt\" bgcolor=\"#F2F2F2\">\r\n                <p class=\"s22\" style=\"padding-top: 5pt;padding-left: 14pt;text-indent: 0pt;text-align: left;\">{observationVM.responsibility}</p>\r\n            </td>\r\n            " +
                            $"<td style=\"width:169pt\">\r\n                <p class=\"s22\"\r\n                    style=\"padding-top: 5pt;padding-left: 61pt;padding-right: 61pt;text-indent: 0pt;text-align: center;\">\r\n                    {observationVM.targetDate}</p>\r\n            </td>\r\n        " +
                            $"</tr>\r\n    " +
                            $"</table>");
                }
            }
            string dynamicRows = dynamicRowBuilder.ToString();

            return dynamicRows;
        }

        private string productTable(Report report)
        {
            string tableHtml = @"
                <table style='border-collapse:collapse' cellspacing='0'>
                        <tr style=""height:12pt"">
                                    <td style=""width:345pt;border-bottom-style:solid;border-bottom-width:1pt"" colspan=""6""
                                        bgcolor=""#999999"">
                                        <p class=""s12""
                                            style=""padding-left: 99pt;padding-right: 99pt;text-indent: 0pt;line-height: 11pt;text-align: center;"">
                                            Exhibit 1: Products and Samples</p>
                                    </td>
                                </tr>
                                <tr style=""height:14pt"">
                                    <td style=""width:84pt;border-top-style:solid;border-top-width:1pt"" bgcolor=""#4B4B4B"">
                                        <p class=""s12"" style=""padding-left: 24pt;text-indent: 0pt;text-align: left;"">Product</p>
                                    </td>
                                    <td style=""width:37pt;border-top-style:solid;border-top-width:1pt"" bgcolor=""#931F38"">
                                        <p class=""s12"" style=""padding-left: 8pt;text-indent: 0pt;text-align: left;"">Year</p>
                                    </td>
                                    <td style=""width:59pt;border-top-style:solid;border-top-width:1pt;border-right-style:solid;border-right-width:1pt""
                                        bgcolor=""#999999"">
                                        <p class=""s12""
                                            style=""padding-left: 4pt;padding-right: 4pt;text-indent: 0pt;text-align: center;"">population
                                        </p>
                                    </td>
                                    <td style=""width:41pt;border-top-style:solid;border-top-width:1pt;border-left-style:solid;border-left-width:1pt;border-right-style:solid;border-right-width:1pt""
                                        bgcolor=""#931F38"">
                                        <p class=""s12""
                                            style=""padding-left: 3pt;padding-right: 4pt;text-indent: 0pt;text-align: center;"">sample</p>
                                    </td>
                                    <td style=""width:60pt;border-top-style:solid;border-top-width:1pt;border-left-style:solid;border-left-width:1pt""
                                        bgcolor=""#999999"">
                                        <p class=""s12""
                                            style=""padding-left: 4pt;padding-right: 4pt;text-indent: 0pt;text-align: center;"">Percentage
                                        </p>
                                    </td>
                                </tr>

                    <tr style='height:37pt'>
                        <td style='width:84pt;border-left-style:solid;border-left-width:1pt;border-right-style:solid;border-right-width:1pt'>
                            <p class='s13' style='padding-top: 7pt;padding-left: 23pt;padding-right: 14pt;text-indent: -8pt;line-height: 113%;text-align: left;'>
                                Ijarah muntahia Bittamleek</p>
                        </td>
                        <td style='width:37pt;border-left-style:solid;border-left-width:1pt;border-right-style:solid;border-right-width:1pt'>
                            <p style='text-indent: 0pt;text-align: left;'><br /></p>
                            <p class='s13' style='padding-left: 9pt;text-indent: 0pt;text-align: left;'>2022</p>
                        </td>
                        <td style='width:59pt;border-left-style:solid;border-left-width:1pt;border-right-style:solid;border-right-width:1pt'>
                            <p style='text-indent: 0pt;text-align: left;'><br /></p>
                            <p class='s13' style='padding-left: 23pt;padding-right: 23pt;text-indent: 0pt;text-align: center;'>19</p>
                        </td>
                        <td style='width:41pt;border-left-style:solid;border-left-width:1pt;border-right-style:solid;border-right-width:1pt'>
                            <p style='text-indent: 0pt;text-align: left;'><br /></p>
                            <p class='s13' style='padding-left: 3pt;padding-right: 4pt;text-indent: 0pt;text-align: center;'>19</p>
                        </td>
                        <td style='width:60pt;border-left-style:solid;border-left-width:1pt'>
                            <p style='text-indent: 0pt;text-align: left;'><br /></p>
                            <p class='s13' style='padding-left: 3pt;padding-right: 4pt;text-indent: 0pt;text-align: center;'>100%</p>
                        </td>
                    </tr>
                </table>
                ";

            StringBuilder dynamicRowBuilder = new StringBuilder();

            foreach (var product in report.products)
            {
                dynamicRowBuilder.Append("<tr style='height:37pt'>\r\n    ");
                dynamicRowBuilder.Append($" <td style='width:84pt;border-left-style:solid;border-left-width:1pt;border-right-style:solid;border-right-width:1pt'>\r\n        <p class='s13' style='padding-top: 7pt;padding-left: 23pt;padding-right: 14pt;text-indent: -8pt;line-height: 113%;text-align: left;'>\r\n            {product.name}</p>\r\n    </td>");
                dynamicRowBuilder.Append($" <td style='width:37pt;border-left-style:solid;border-left-width:1pt;border-right-style:solid;border-right-width:1pt'>\r\n        <p style='text-indent: 0pt;text-align: left;'><br /></p>\r\n        <p class='s13' style='padding-left: 9pt;text-indent: 0pt;text-align: left;'>{product.year}</p>\r\n    </td>");
                dynamicRowBuilder.Append($"<td style='width:59pt;border-left-style:solid;border-left-width:1pt;border-right-style:solid;border-right-width:1pt'>\r\n        <p style='text-indent: 0pt;text-align: left;'><br /></p>\r\n        <p class='s13' style='padding-left: 23pt;padding-right: 23pt;text-indent: 0pt;text-align: center;'>{product.population}</p>\r\n    </td>");
                dynamicRowBuilder.Append($"<td style='width:41pt;border-left-style:solid;border-left-width:1pt;border-right-style:solid;border-right-width:1pt'>\r\n        <p style='text-indent: 0pt;text-align: left;'><br /></p>\r\n        <p class='s13' style='padding-left: 3pt;padding-right: 4pt;text-indent: 0pt;text-align: center;'>{product.sample}</p>\r\n    </td>");
                dynamicRowBuilder.Append($" <td style='width:60pt;border-left-style:solid;border-left-width:1pt'>\r\n        <p style='text-indent: 0pt;text-align: left;'><br /></p>\r\n        <p class='s13' style='padding-left: 3pt;padding-right: 4pt;text-indent: 0pt;text-align: center;'>{(product.sample / (double)product.population) * 100}%</p>\r\n    </td>");
                dynamicRowBuilder.Append("</tr>");
            }

            string dynamicRows = dynamicRowBuilder.ToString();

            // Insert the dynamic rows into the table HTML
            tableHtml = tableHtml.Replace("</table>", dynamicRows + "</table>");

            // Use the updated table HTML as needed
            return tableHtml;

        }

        private string ReferanceTable(Report report)
        {
          
            StringBuilder dynamicRowBuilder = new StringBuilder();
            dynamicRowBuilder.Append($"<ul id=\"l11\">");
            foreach (var dept in report.DepartmentSection)
            {
                foreach (var observationVM in dept.ObservationVM)
                {
                    
                    if (observationVM.references.Count > 0)
                    {   
                        foreach (var reference in observationVM.references)
                        {
                            dynamicRowBuilder.Append($" <li data-list-text=\"&gt;\">\r\n                    " +
                                $"<h1 style=\"padding-top: 12pt;padding-left: 47pt;text-indent: -16pt;text-align: left;\">{reference.title}:</h1>\r\n                    " +
                                $"<p class=\"s27\" style=\"padding-top: 2pt;padding-left: 47pt;text-indent: 0pt;text-align: left;\">{reference.description}</p>\r\n                    " +
                                $"<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>\r\n                " +
                                $"</li>");
                        }
                    }
                }
            }
            dynamicRowBuilder.Append($"</ul>");
            string dynamicRows = dynamicRowBuilder.ToString();
            return dynamicRows;

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult GetSubDepartments(int id)
        {
            var subDepartments = new List<SubDepartment>();

            // Retrieve subdepartments based on the selected departmentId from your data source
            // Here, I'm using some sample data
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

            // Create a list of SelectListItem objects for the subdepartments
            var subDepartmentList = subDepartments.Select(subDepartment => new SelectListItem
            {
                Value = subDepartment.SubDepartmentId.ToString(),
                Text = subDepartment.Name
            });

            return Json(subDepartmentList);
        }

    }
}
