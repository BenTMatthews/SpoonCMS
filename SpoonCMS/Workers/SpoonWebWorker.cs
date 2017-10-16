using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.Workers
{
    public static class SpoonWebWorker
    {
        #region WebOps

        public static void BuildAdminPageDelegate(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("HelloWorld!");
            });
        }

        public static string BuildAdminPageString()
        {
            return "Hello World String";
        }

        #endregion
    }
}
