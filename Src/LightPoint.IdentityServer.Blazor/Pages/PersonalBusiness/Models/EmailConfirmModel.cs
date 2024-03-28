﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.PersonalBusiness.Models
{
    internal class EmailConfirmModel
    {
        public string? Email { get; set; }
        public string? ValidateCode { get; set; }
        public bool Confirmed { get; set; }
    }
}
