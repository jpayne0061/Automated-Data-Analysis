using CorrelationStation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meta.Numerics.Statistics;
using CsvHelper;
using Microsoft.AspNet.Identity;

namespace CorrelationStation.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Report(List<ChiStats> chis)
        {
           

            return View(chis);
        }

        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Upload()
        {
            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
            }
            //if (TempData["ModelState"] != null)
            //    TempData["ModelState"] = ModelState;

            UploadVM file = new UploadVM();

            return View(file);
        }

        [HttpPost]
        public ActionResult ChooseAnalysis(UploadVM vm)
        {


            return View();
        }


        [HttpPost]
        public ActionResult SelectTypes(UploadVM vm)
        {

            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
            }

            if (vm.File.ContentLength > 11000000)
            {
                ModelState.AddModelError(string.Empty, "*There is a 10MB size limit at this time");
                TempData["ViewData"] = ViewData;
                TempData["ModelState"] = ModelState;
                return RedirectToAction("Upload");
            }
                    
           

            if (vm.File.ContentLength > 0 && ModelState.IsValid)
            {
                var guid = Guid.NewGuid();
                var path = Path.Combine(Server.MapPath("~/Content/Files"), guid.ToString());
                SelectTypeVM selectTypeVM = Methods.MakeSelectType(vm, path);
                selectTypeVM.FileName = vm.File.FileName;
                return View(selectTypeVM);

            }
            else
            {
                return RedirectToAction("Upload");
            }


}

        
        public ActionResult ProcessCSV(SelectTypeVM vm)
        {
            Methods.MakeDropDownAndFirstFive(vm);

            if (vm.ColumnTypes.Values.Where(x => x != "").Count() < 2 )
            {
                ModelState.AddModelError(string.Empty, "*You must select at least two variables");
                TempData["ViewData"] = ViewData;
                TempData["ModelState"] = ModelState;
                ViewData = (ViewDataDictionary)TempData["ViewData"];
                return View("SelectTypes", vm);
            }

            Dictionary<string, List<string>> invalidColumns = Methods.CheckForInvalidColumns(vm.ColumnTypes, vm.FirstFiveRows);

            if (invalidColumns.Count > 0)
            {
                //string errorMessage = Methods.GetExceptionMessage(invalidColumns);
                //ViewBag.Error = Methods.GetExceptionMessage(invalidColumns);
                ViewBag.Error = "<p class='viewbag-error'>*Invalid data found* <br/>Did you mark a categorical variabal as numeral? </p>";
                //ModelState.AddModelError(string.Empty, errorMessage);
                TempData["ViewData"] = ViewData;
                TempData["ModelState"] = ModelState;
                ViewData = (ViewDataDictionary)TempData["ViewData"];
                return View("SelectTypes", vm);
            }
            //check variable types




            Dictionary<string, List<string>> dictFile = Methods.CsvToDictionary(vm.Path);

            StatSummaryVM summaryVM = Methods.GetSummaryVM(dictFile, vm);
            summaryVM.CreatedOn = DateTime.Now;

            string userId = User.Identity.GetUserId();

            Methods.SaveStatSummary(summaryVM, userId);

            ViewBag.Saved = "true";

            return View("Report", summaryVM);
        }

        public ActionResult GetReport(int id)
        {
            string userId = User.Identity.GetUserId();

            bool reportSaved;
            if (userId != null)
            {
                reportSaved = Methods.CheckIfReportSaved(userId, id);
                if (reportSaved)
                    ViewBag.Saved = "true";
                else
                    ViewBag.Saved = "false";
            }
            else
            {
                ViewBag.Saved = "true";
            }


            StatSummaryVM ss =  Methods.GetSummaryById(id);
            return View("Report", ss);
        }

        [Authorize]
        public ActionResult StatSummaries()
        {
            string userId = User.Identity.GetUserId();

            List<StatSummaryVM> summaries = Methods.GetUserSummaries(userId);

            return View("AllStatSummaries", summaries);
        }

        public ActionResult AllStatSummaries()
        {

            List<StatSummaryVM> summaries = Methods.GetAllSummaries();

            return View(summaries);
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult DeleteRecords()
        {
            string userId = User.Identity.GetUserId();

            if (userId != "3146a6f3-818c-4623-b251-f1eb28acb43c")
            {
                return RedirectToAction("Index");
            }

            Methods methods = new Methods();
            methods.DeleteRecords();
            return RedirectToAction("Index");
        }
        //public ActionResult SaveSummary(StatSummaryVM ss)
        //{
        //    Methods.SaveStatSummary(ss);

        //    return RedirectToAction("StatSummaries");
        //}

    }
}