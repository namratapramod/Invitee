using Invitee.Infrastructure.PaymentInfra;
using Invitee.Repository;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Invitee.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly ILogger logger;
        private readonly IPaymentService paymentService;
        public DefaultController(IRepositoryWrapper repositoryWrapper, ILogger logger, IPaymentService paymentService)
        {
            this.repositoryWrapper = repositoryWrapper;
            this.logger = logger.ForContext(this.GetType());
            this.paymentService = paymentService;
        }

        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdvancedForm()
        {
            return View();
        }

        public ActionResult SimpleTables()
        {
            return View();
        }

        public ActionResult SimpleForm()
        {
            return View();
        }

        public void TestSome()
        {
            var upLoadedFileInfos = this.repositoryWrapper.UploadedFiles.GetUploadedFileInfos();
            List<string> filesToBeDeleted = new List<string>();
            List<string> filesTobeLogged = new List<string>();
            foreach (var item in upLoadedFileInfos)
            {
                var dbFileNames = this.repositoryWrapper.UploadedFiles.GetListOfFileNames(item.TableName, item.ColumnName);
                var localFiles = System.IO.Directory.GetFiles(System.IO.Path.Combine(Server.MapPath(item.Path)), $"*{System.IO.Path.GetExtension(dbFileNames[0])}");

                //Files which are not present in the table >> The files which has to be marked for deletion
                var fileNotPresentInTable = localFiles.Where(x => !dbFileNames.Select(dx=>System.IO.Path.GetFileName(dx)).Contains(System.IO.Path.GetFileName(x)));

                //Files which are not present in the system >> Files which has to be alerted and logged.
                var fileNotAvailabeOnSystem = dbFileNames.Where(x => !localFiles.Select(dx => System.IO.Path.GetFileName(dx)).Contains(System.IO.Path.GetFileName(x)));

                filesToBeDeleted.AddRange(fileNotPresentInTable);
                filesTobeLogged.AddRange(fileNotAvailabeOnSystem);
            }
            foreach (var Deletefiles in filesToBeDeleted)
            {
                logger.Information("Deleting the file" + filesToBeDeleted);
                System.IO.File.Delete(Deletefiles);
            }
        }

    }
} 