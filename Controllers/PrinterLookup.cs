using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Management;

namespace WebPrinter.Controllers
{
    public class PrinterLookupController : Controller
    {
        public class PrinterStatus
        {
            public string Name { get; set; }
            public string Status { get; set; }
        }

        private enum PrinterStatusCode
        {
            Other = 1,
            Ready = 3
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetPrinterStatusList()
        {
            List<PrinterStatus> printerStatusList = new List<PrinterStatus>();

            using (var printerQuery = new ManagementObjectSearcher("SELECT * FROM Win32_Printer"))
            {
                foreach (var printer in printerQuery.Get())
                {
                    var name = printer.GetPropertyValue("Name") as string;
                    var statusCode = (ushort)printer.GetPropertyValue("PrinterStatus");
                    var status = GetPrinterStatusString(statusCode);

                    printerStatusList.Add(new PrinterStatus
                    {
                        Name = name,
                        Status = status
                    });
                }
            }

            return Json(printerStatusList);
        }

        private static string GetPrinterStatusString(uint statusCode)
        {
            string statusString;

            switch ((PrinterStatusCode)statusCode)
            {
                case PrinterStatusCode.Other:
                    statusString = "Paused or Out of Paper";
                    break;
                case PrinterStatusCode.Ready:
                    statusString = "Ready";
                    break;
                default:
                    statusString = "Error";
                    break;
            }

            return statusString;
        }
    }
}
