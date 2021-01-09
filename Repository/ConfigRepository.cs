using Invitee.Entity;
using Invitee.Repository.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public class ConfigRepository : RepositoryBase<Config>, IConfigRepository
    {
        private RepositoryContext repositoryContext;
        public ConfigRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }

        public ReferralSettings AddReferralSettings(ReferralSettings referralSettings)
        {
            return this.repositoryContext.ReferralSettings.Add(referralSettings);
        }

        public ReferralSettings GetLatestReferralSetting()
        {
            return this.repositoryContext.ReferralSettings.OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public IEnumerable<ReferralSettings> GetReferralSettingsList()
        {
            return this.repositoryContext.ReferralSettings;
        }
    }
}