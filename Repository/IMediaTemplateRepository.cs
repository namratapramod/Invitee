using Invitee.Entity;
using Invitee.Repository.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public interface IMediaTemplateRepository : IRepository<MediaTemplate>
    {
        void UpdateSlideTexts(int mediaTemplateId, string[] slideTexts);
        new void Update(MediaTemplate mediaTemplate);
        void LikeUnlike(int id, int userId);
    }
}