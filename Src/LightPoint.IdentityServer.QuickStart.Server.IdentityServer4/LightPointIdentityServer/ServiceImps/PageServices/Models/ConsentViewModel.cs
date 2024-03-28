﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Device.Models;
using System.Collections.Generic;

namespace LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.PageServices.Models
{
    public class ConsentViewModel : ConsentInputModel
    {
        public string? ClientName { get; set; }
        public string? ClientUrl { get; set; }
        public string? ClientLogoUrl { get; set; }
        public bool AllowRememberConsent { get; set; }

        public IEnumerable<ScopeViewModel>? IdentityScopes { get; set; }
        public IEnumerable<ScopeViewModel>? ApiScopes { get; set; }
    }
}
