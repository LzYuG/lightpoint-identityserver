using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LightPoint.IdentityServer.Blazor.Pages.RazorPages.RazorPageBase
{
    public abstract class LightPointPageModel : PageModel
    {
        public LightPointPageModel(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public string? AntiForgeryToken = "";
        private readonly IAntiforgery _antiforgery;

        #region Helper Funcs
        public IActionResult LPRedirect(string url)
        {
            return Redirect(url);
        }

        public IActionResult LPRedirect(string url, object model)
        {
            var query = "?";
            if (model != null)
            {
                var props = model.GetType().GetProperties();

                foreach (var prop in props)
                {
                    var value = prop.GetValue(model) as string;
                    if (value != null)
                    {
                        query += prop.Name + "=" + HttpUtility.UrlEncode(value) + "&";

                    }
                }
            }
            if (query == "?")
            {
                query = "";
            }
            else if (query.Last() == '&')
            {
                query = query[..^1];
            }

            url += query;
            return Redirect(url);
        }
        #endregion

    }
}
