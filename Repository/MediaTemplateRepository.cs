using Invitee.Entity;
using Invitee.Repository.Infra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public class MediaTemplateRepository : RepositoryBase<MediaTemplate>, IMediaTemplateRepository
    {
        private RepositoryContext repositoryContext;
        public MediaTemplateRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }

        public void LikeUnlike(int id, int userId)
        {
            var existing = this.repositoryContext.MediaTemplateLikes.Where(x => x.MediaTemplateId == id && x.UserId == userId);
            if (existing.Any())
                this.repositoryContext.Entry(existing.Single()).State = System.Data.Entity.EntityState.Deleted;
            else
                this.repositoryContext.MediaTemplateLikes.Add(new MediaTemplateLike { MediaTemplateId = id, UserId = userId });
        }

        public void UpdateSlideTexts(int mediaTemplateId, string[] slideTexts)
        {
            var mediaTemplate = this.repositoryContext.MediaTemplates.Find(mediaTemplateId);
            this.repositoryContext.SlideTexts.RemoveRange(mediaTemplate.SlideTexts);
            foreach (var item in slideTexts)
            {
                if(!string.IsNullOrEmpty(item))
                    mediaTemplate.SlideTexts.Add(new SlideText { Text = item });
            }
            this.repositoryContext.SaveChanges();
        }

        void IMediaTemplateRepository.Update(MediaTemplate mediaTemplate)
        {
            var dbMediaTemplate = this.repositoryContext.MediaTemplates.Find(mediaTemplate.Id);
            this.repositoryContext.Entry(dbMediaTemplate).CurrentValues.SetValues(mediaTemplate);

            foreach (var item in dbMediaTemplate.Costings.ToList())
            {
                var exitingCost = mediaTemplate.Costings.Where(x => x.Id == item.Id);
                if (!exitingCost.Any())
                {
                    dbMediaTemplate.Costings.Remove(item);
                }
            }
            
            foreach (var item in mediaTemplate.Costings) 
            {
                var existingChild = dbMediaTemplate.Costings
               .Where(c => c.Id == item.Id)
               .SingleOrDefault();

                if (existingChild != null)
                    // Update child
                    repositoryContext.Entry(existingChild).State = System.Data.Entity.EntityState.Unchanged;
                else
                {
                    if(repositoryContext.Entry(item).State == System.Data.Entity.EntityState.Detached)
                    {
                        repositoryContext.Costings.Attach(item);
                        dbMediaTemplate.Costings.Add(item);
                    }
                    
                }
            }
            this.repositoryContext.SaveChanges();
        }
    }
}