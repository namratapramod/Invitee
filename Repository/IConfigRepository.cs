using Invitee.Entity;
using Invitee.Repository.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public interface IConfigRepository : IRepository<Config>
    {
        ReferralSettings AddReferralSettings(ReferralSettings referralSettings);
        ReferralSettings GetLatestReferralSetting();
        IEnumerable<ReferralSettings> GetReferralSettingsList();
    }
}