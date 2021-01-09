﻿using AutoMapper;
using Invitee.AutoMapperProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.App_Start
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize((cfg) =>
                {
                    cfg.AddProfile<EntityToViewModelMapping>();
                    cfg.AddProfile<ViewModelToEntityMapping>();
                });
        }
    }
}