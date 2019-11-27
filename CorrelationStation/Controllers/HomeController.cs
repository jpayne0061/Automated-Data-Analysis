using CorrelationStation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using CorrelationStation.Services;

namespace CorrelationStation.Controllers
{
    public class HomeController : Controller
    {
        ParsingService _parsingService;
        ConfigurationService _configurationService;
        SummaryService _summaryService;
        DeleteRecordService _deleteRecordService;

        public HomeController()
        {
            _parsingService = new ParsingService();
            _configurationService = new ConfigurationService();
            _summaryService = new SummaryService();
            _deleteRecordService = new DeleteRecordService();
        }

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
            if (vm.File == null)
            {
                ModelState.AddModelError(string.Empty, "Please select a file first");
                TempData["ViewData"] = ViewData;
                TempData["ModelState"] = ModelState;
                return RedirectToAction("Upload");
            }

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
                SelectTypeVM selectTypeVM = _configurationService.MakeSelectType(vm, path);
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
            _configurationService.MakeDropDownAndFirstFive(vm);

            if (vm.ColumnTypes.Values.Where(x => x != "").Count() < 2)
            {
                ModelState.AddModelError(string.Empty, "*You must select at least two variables");
                TempData["ViewData"] = ViewData;
                TempData["ModelState"] = ModelState;
                ViewData = (ViewDataDictionary)TempData["ViewData"];
                return View("SelectTypes", vm);
            }

            Dictionary<string, List<string>> invalidColumns = _parsingService.CheckForInvalidColumns(vm.ColumnTypes, vm.FirstFiveRows);

            if (invalidColumns.Count > 0)
            {
                ViewBag.Error = "<p class='viewbag-error'>*Invalid data found* <br/>Did you mark a categorical variabal as numeral? </p>";
                TempData["ViewData"] = ViewData;
                TempData["ModelState"] = ModelState;
                ViewData = (ViewDataDictionary)TempData["ViewData"];
                return View("SelectTypes", vm);
            }


            Dictionary<string, List<string>> dictFile = _parsingService.CsvToDictionary(vm.Path);

            StatSummaryVM summaryVM = _summaryService.GetSummaryVM(dictFile, vm);
            summaryVM.CreatedOn = DateTime.Now;

            string userId = User.Identity.GetUserId();

            _summaryService.SaveStatSummary(summaryVM, userId);

            ViewBag.Saved = "true";

            return View("Report", summaryVM);
        }

        public ActionResult GetReport(int id)
        {
            string userId = User.Identity.GetUserId();

            bool reportSaved;
            if (userId != null)
            {
                reportSaved = _summaryService.CheckIfReportSaved(userId, id);
                if (reportSaved)
                    ViewBag.Saved = "true";
                else
                    ViewBag.Saved = "false";
            }
            else
            {
                ViewBag.Saved = "true";
            }


            StatSummaryVM ss = _summaryService.GetSummaryById(id);
            return View("Report", ss);
        }

        [Authorize]
        public ActionResult StatSummaries()
        {
            string userId = User.Identity.GetUserId();

            List<StatSummaryVM> summaries = _summaryService.GetUserSummaries(userId);

            return View("AllStatSummaries", summaries);
        }

        public ActionResult AllStatSummaries()
        {

            List<StatSummaryVM> summaries = _summaryService.GetAllSummaries();

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

            _deleteRecordService.DeleteRecords();
            return RedirectToAction("Index");
        }
    }
}