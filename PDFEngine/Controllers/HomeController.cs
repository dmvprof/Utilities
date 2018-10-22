using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDFEngine.Models;
using Kendo.Mvc.UI.Fluent;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Objects;
using System.IO;
using ReportLibrary;

namespace PDFEngine.Controllers
{



    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

//        [HttpPost]
        public ActionResult ReportRun( string shift  )
        {
                if (ModelState.IsValid)
                {

                    
                    var entity = new RunMaster
                    {
                        RunDate = DateTime.Now,
                        Description = "Salesman Report",
                        Type = 1,
                        UserName = "",
                    };
                    this.DataStore.RunMasters.Add(entity);
                    this.DataStore.SaveChanges();  //get back the new Identity value of the PrRunDates row to use to update with link later

                var RegionList = this.DataStore.Customers.Where(d => d.Region == shift);
//                var RegionList = from d in this.DataStore.Customers select d;

//                RegionList = this.DataStore.Customers.Where(s => s.Region == shift);

                foreach (var C in RegionList)
                { 
                    //   START  REPORT
                    Telerik.Reporting.Report report = new ReportLibrary.Reports.SalesPersonReport();

                        Telerik.Reporting.TypeReportSource typeReportSource =
                             new Telerik.Reporting.TypeReportSource();
                        report.ReportParameters[0].Value = shift;
                        typeReportSource.TypeName = report.GetType().AssemblyQualifiedName;
                        
                        //typeReportSource.Parameters[0].Value = shift;

                        string path = System.IO.Path.GetTempPath();
                        byte[] fileData = CreateReport(report);

                        var reportentity = new RunDetail
                        {
                            ReportDescription = "Register",
                            ReportFilter = 1,
                            RunId = entity.Id,
                            ReportBLOB = fileData
                        };
                        this.DataStore.RunDetails.Add(reportentity);

                }
                this.DataStore.SaveChanges();

                return Json(new { RunID = entity.Id });
                        
                }

                return Json(new { RunID = "Model Invalid-Contact Support" });
        }

    
    
        private static byte[] CreateReport(Telerik.Reporting.Report report)
        {

            Telerik.Reporting.Processing.ReportProcessor reportProcessor =
                new Telerik.Reporting.Processing.ReportProcessor();

            System.Collections.Hashtable deviceInfo =
                new System.Collections.Hashtable();

            try
            {

                Telerik.Reporting.Processing.RenderingResult result =
                reportProcessor.RenderReport("PDF", report, null);


                string fileName = result.DocumentName + "." + result.Extension;
                string path = System.IO.Path.GetTempPath();
                string filePath = System.IO.Path.Combine(path, fileName);

                using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
                    fs.Close();
                }

                FileStream fStream = System.IO.File.OpenRead(filePath);
                byte[] contents = new byte[fStream.Length];
                fStream.Read(contents, 0, (int)fStream.Length);
                fStream.Close();

                return contents;
            }
            catch (Exception e)
            {
                    byte[] x = new byte[0];
                    return x;
            }

        }


        public FileResult GetPdf(int reportid = 0, int runid = 1)
        {
            RunDetail myPRReport = this.DataStore.RunDetails.Find(reportid);
            FileStream myfileStream = new FileStream(Guid.NewGuid().ToString() + "doc.pdf", FileMode.Create);

            for (int i = 0; i < myPRReport.ReportBLOB.Length; i++)
            {
                myfileStream.WriteByte(myPRReport.ReportBLOB[i]);
            }

            myfileStream.Seek(0, SeekOrigin.Begin);

            return File(myfileStream, "application/pdf");
        }


        public ActionResult Report_List_Read([DataSourceRequest]DataSourceRequest request, int myRunID = 0)
        {
            DataSourceResult result = this.DataStore.vwReportLists.Where(p => p.RunId == myRunID).ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    
    
    }
}
