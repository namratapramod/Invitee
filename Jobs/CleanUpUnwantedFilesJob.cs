using FluentScheduler;
using Invitee.Repository;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Jobs
{
    public class CleanUpUnwantedFilesJob : Registry
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly ILogger logger;
        private readonly HttpContext httpContext;

        public CleanUpUnwantedFilesJob(IRepositoryWrapper repoWrapper,HttpContext httpContext)
        {
            this.repositoryWrapper = repoWrapper;
            this.httpContext = httpContext;

            logger = new LoggerConfiguration()
                  .WriteTo.File(new JsonFormatter(),
                    AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + "/CleanUpUnwantedFilesJobLogs.txt", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 5242880, rollOnFileSizeLimit: true, shared: true)
                  .CreateLogger();

            Schedule(() =>
            {
                var upLoadedFileInfos = this.repositoryWrapper.UploadedFiles.GetUploadedFileInfos();
                List<string> filesToBeDeleted = new List<string>();
                List<string> filesTobeLogged = new List<string>();
                foreach (var item in upLoadedFileInfos)
                {
                    logger.Information("Now processing for {tableName} in path {path}", item.TableName, item.Path);
                    var dbFileNames = this.repositoryWrapper.UploadedFiles.GetListOfFileNames(item.TableName, item.ColumnName);
                    logger.Information("Got {dbcount} number of file names from {tableName}", dbFileNames.Count, item.TableName);
                    var localFiles = System.IO.Directory.Exists(System.IO.Path.Combine(httpContext.Server.MapPath(item.Path)))?System.IO.Directory.GetFiles(System.IO.Path.Combine(httpContext.Server.MapPath(item.Path)), $"*{System.IO.Path.GetExtension(dbFileNames[0])}"):new string[0];
                    logger.Information("Got {localcount} number of files from {path}", localFiles.Length, item.Path);

                    //Files which are not present in the table >> The files which has to be marked for deletion
                    var fileNotPresentInTable = localFiles.Where(x => !dbFileNames.Select(dx => System.IO.Path.GetFileName(dx)).Contains(System.IO.Path.GetFileName(x)));
                    logger.Information("Files which are not present in table {tableName} are {@fileList}", item.TableName, fileNotPresentInTable);

                    //Files which are not present in the system >> Files which has to be alerted and logged.
                    var fileNotAvailabeOnSystem = dbFileNames.Where(x => !localFiles.Select(dx => System.IO.Path.GetFileName(dx)).Contains(System.IO.Path.GetFileName(x)));
                    logger.Information("Files which are not present in path {path} are {@fileList}", item.Path, fileNotAvailabeOnSystem);

                    filesToBeDeleted.AddRange(fileNotPresentInTable);
                    filesTobeLogged.AddRange(fileNotAvailabeOnSystem);
                }
                logger.Information("Total number of files to be deleted are {delCount}", filesToBeDeleted.Count);
                logger.Information("Total number of files which has to be alerted are {alertCount}", filesTobeLogged.Count);

                foreach (var Deletefiles in filesToBeDeleted)
                {
                    logger.Information("Deleting the file {filesToBeDeleted}",filesToBeDeleted);
                    System.IO.File.Delete(Deletefiles);
                }

            }).ToRunNow().AndEvery(6).Hours();
        }
    }
}