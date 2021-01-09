using Invitee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public interface IUploadedFilesRepository
    {
        IList<UploadedFileInfo> GetUploadedFileInfos();
        IList<string> GetListOfFileNames(string tableName, string columnName);
    }
}