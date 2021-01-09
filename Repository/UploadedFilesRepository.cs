using Invitee.Models;
using Invitee.Repository.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using Newtonsoft.Json;

namespace Invitee.Repository
{
    public class UploadedFilesRepository : IUploadedFilesRepository
    {
        private RepositoryContext repositoryContext;
        public UploadedFilesRepository(RepositoryContext repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }
        public IList<UploadedFileInfo> GetUploadedFileInfos()
        {
            var res = JsonConvert.DeserializeObject<List<UploadedFileInfo>>(System.IO.File.ReadAllText($"{AppContext.BaseDirectory}/Repository/UploadedFileInfo.json"));
            return res;
            //return  new List<UploadedFileInfo>
            //{
            //    new UploadedFileInfo
            //    {
            //        TableName = "MediaTemplate",
            //        ColumnName = "VideoFilePath",
            //        Path = HttpContext.Current.Server.MapPath("~/Videos/TemplateVideos")
            //    },
            //    new UploadedFileInfo
            //    {
            //        TableName = "Deliveries",
            //        ColumnName = "DeliveryFile",
            //        Path = HttpContext.Current.Server.MapPath("~/Videos/Delivery")
            //    }
            //};
        }

        public IList<string> GetListOfFileNames(string tableName, string columnName)
        {
            var set = this.repositoryContext.Set(tableName).Select(columnName) as IQueryable<string>;
            return set.ToList();
        }
    }
}