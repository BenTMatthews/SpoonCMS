using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SpoonCMS.Classes;
using SpoonCMS.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace SpoonCMS.Workers
{
    public static class SpoonWebWorker
    {
        private static string _htmlPath = "SpoonCMS.Files.Admin.html";
        private static string _jsPath = "SpoonCMS.Files.Script.js";
        private static string _cssPath = "SpoonCMS.Files.Styles.css";
        private static string _containerListTemplatePath = "SpoonCMS.Files.Templates.ContainerList.hbs";
        private static string _containerDetailsTemplatePath = "SpoonCMS.Files.Templates.ContainerDetails.hbs";

        private static string _jsMarker = "[!CustomScriptBlock!]";
        private static string _cssMarker = "[!CustomStyleBlock!]";
        private static string _jsPathMarker = "[!CustomPathMarker!]";

        public static string AdminPath { get; set; } = @"/admin";
        public static bool RequireAuth { get; set; } = false;
        public static List<string> AuthRoles { get; set; } = new List<string>();

        #region PageGeneration

        public static void BuildAdminPageDelegate(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                if (IsAuthorized(context))
                {
                    switch (context.Request.Method.ToUpper())
                    {
                        case "GET":
                            if (context.Request.Path.HasValue)
                            {
                                switch (context.Request.Path.Value.ToUpper())
                                {
                                    case "/GETCONTAINERS":
                                        ServiceResponse<List<ContainerSkinny>> respConsSkinny = new ServiceResponse<List<ContainerSkinny>>();
                                        try
                                        {
                                            respConsSkinny = GetContainers();
                                        }
                                        catch (Exception ex)
                                        {
                                            respConsSkinny.SetToError(ex);
                                        }
                                        await context.Response.WriteAsync(JsonConvert.SerializeObject(respConsSkinny));
                                        break;

                                    case "/GETCONTAINER":
                                        ServiceResponse<Container> respCon = new ServiceResponse<Container>();
                                        try
                                        {
                                            respCon = GetContainer(context);
                                        }
                                        catch (Exception ex)
                                        {
                                            respCon.SetToError(ex);
                                        }

                                        await context.Response.WriteAsync(JsonConvert.SerializeObject(respCon));
                                        break;

                                    default:
                                        await context.Response.WriteAsync(context.Request.Path.Value + ": No action found");
                                        break;
                                }
                            }
                            else
                            {
                                await context.Response.WriteAsync(BuildAdminPageString());
                            }
                            break;

                        case "POST":
                            if (context.Request.Path.HasValue)
                            {
                                switch (context.Request.Path.Value.ToUpper())
                                {
                                    case "/SAVECONTAINER":
                                        ServiceResponse<string> respSave = new ServiceResponse<string>();
                                        try
                                        {
                                            respSave = SaveContainer(context);
                                        }
                                        catch (Exception ex)
                                        {
                                            respSave.SetToError(ex);
                                        }

                                        await context.Response.WriteAsync(JsonConvert.SerializeObject(respSave));
                                        break;

                                    case "/CREATECONTAINER":
                                        ServiceResponse<string> respCreate = new ServiceResponse<string>();
                                        try
                                        {
                                            respCreate = CreateContainer(context);
                                        }
                                        catch (Exception ex)
                                        {
                                            respCreate.SetToError(ex);
                                        }

                                        await context.Response.WriteAsync(JsonConvert.SerializeObject(respCreate));
                                        break;

                                    case "/EDITCONTAINERNAME":
                                        ServiceResponse<string> respEditName = new ServiceResponse<string>();
                                        try
                                        {
                                            respEditName = EditContainerName(context);
                                        }
                                        catch (Exception ex)
                                        {
                                            respEditName.SetToError(ex);
                                        }

                                        await context.Response.WriteAsync(JsonConvert.SerializeObject(respEditName));
                                        break;

                                    case "/DELETECONTAINER":
                                        ServiceResponse<string> respDelete = new ServiceResponse<string>();
                                        try
                                        {
                                            respDelete = DeleteContainer(context);
                                        }
                                        catch (Exception ex)
                                        {
                                            respDelete.SetToError(ex);
                                        }

                                        await context.Response.WriteAsync(JsonConvert.SerializeObject(respDelete));
                                        break;

                                    default:
                                        await context.Response.WriteAsync(context.Request.Path.Value + ": No action found");
                                        break;
                                }
                            }
                            else
                            {
                                await context.Response.WriteAsync(BuildAdminPageString());
                            }
                            break;
                        default:
                            await context.Response.WriteAsync(BuildAdminPageString());
                            break;
                    }
                }
                else
                {
                    await context.Response.WriteAsync(context.Request.Path.Value + ": Access Denied");
                }

            });
        }

        public static string BuildAdminPageString()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string html;
            string js;
            string css;
            string hbTemplates;
            string retval;


            //Need to eventually make these async
            using (Stream stream = assembly.GetManifestResourceStream(_htmlPath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
            }

            using (Stream stream = assembly.GetManifestResourceStream(_jsPath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    js = reader.ReadToEnd();
                    js = js.Replace(_jsPathMarker, AdminPath);
                }
            }

            using (Stream stream = assembly.GetManifestResourceStream(_cssPath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    css = reader.ReadToEnd();
                }
            }

            using (Stream stream = assembly.GetManifestResourceStream(_containerListTemplatePath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    hbTemplates = reader.ReadToEnd();
                }
            }

            using (Stream stream = assembly.GetManifestResourceStream(_containerDetailsTemplatePath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    hbTemplates += reader.ReadToEnd();
                }
            }

            retval = html.Replace(_jsMarker, js).Replace(_cssMarker, css) + hbTemplates;

            return retval;
        }

        #endregion

        #region PageOperations

        private static ServiceResponse<List<ContainerSkinny>> GetContainers()
        {
            ServiceResponse<List<ContainerSkinny>> respConsSkinny = new ServiceResponse<List<ContainerSkinny>>();
            respConsSkinny.Data = SpoonDataWorker.GetAllContainers();
            respConsSkinny.Message = "Success";
            respConsSkinny.Success = true;

            return respConsSkinny;
        }

        private static ServiceResponse<Container> GetContainer(HttpContext context)
        {
            ServiceResponse<Container> respCon = new ServiceResponse<Container>();
            int id;
            Container retVal = null;
            if (int.TryParse(context.Request.Query["id"], out id))
            {
                retVal = SpoonDataWorker.GetContainer(id);

                respCon.Data = retVal;
                respCon.Message = "Success";
                respCon.Success = true;
            }
            else
            {
                respCon.Data = null;
                respCon.Message = "Id not valid";
                respCon.Success = false;
            }

            return respCon;
        }

        private static ServiceResponse<string> SaveContainer(HttpContext context)
        {
            ServiceResponse<string> respSave = new ServiceResponse<string>();
            int id;
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

            if (int.TryParse(context.Request.Query["id"], out id))
            {
                string containerString;
                using (StreamReader reader = new StreamReader(context.Request.Body))
                {
                    containerString = reader.ReadToEnd();
                }

                Container postedCon = JsonConvert.DeserializeObject<Container>(containerString, settings);

                //Do a quick check to make sure any newly generated GUIDs are unique and there was no funny business on the client side
                List<Guid> guids = new List<Guid>();
                foreach (KeyValuePair<string, ContentItem> item in postedCon.Items)
                {
                    if (!guids.Contains(item.Value.Id))
                    {
                        guids.Add(item.Value.Id);
                    }
                    else
                    {
                        item.Value.Id = Guid.NewGuid();
                        guids.Add(item.Value.Id);
                    }
                }

                SpoonDataWorker.UpdateContainer(postedCon);

                respSave.Data = "Success";
                respSave.Message = "Success";
                respSave.Success = true;
            }
            else
            {
                respSave.Data = "Failure";
                respSave.Message = "Id not valid";
                respSave.Success = false;
            }

            return respSave;
        }

        private static ServiceResponse<string> CreateContainer(HttpContext context)
        {
            ServiceResponse<string> respCreate = new ServiceResponse<string>();
            string conName = context.Request.Query["name"];
            if (!String.IsNullOrEmpty(conName))
            {
                Container newCon = new Container(conName);
                SpoonDataWorker.AddContainer(newCon);

                respCreate.Data = "Success";
                respCreate.Message = "Success";
                respCreate.Success = true;
            }
            else
            {
                respCreate.Data = "Failure";
                respCreate.Message = "Name not valid";
                respCreate.Success = false;
            }

            return respCreate;
        }

        private static ServiceResponse<string> EditContainerName(HttpContext context)
        {
            ServiceResponse<string> respEditName = new ServiceResponse<string>();
            string conName = context.Request.Query["name"];
            int conId;

            if (int.TryParse(context.Request.Query["id"], out conId) && !String.IsNullOrEmpty(conName))
            {
                SpoonDataWorker.UpdateContainerName(conId, conName);

                respEditName.Data = "Success";
                respEditName.Message = "Success";
                respEditName.Success = true;
            }
            else
            {
                respEditName.Data = "Failure";
                respEditName.Message = "Name or Id not valid";
                respEditName.Success = false;
            }

            return respEditName;
        }

        private static ServiceResponse<string> DeleteContainer(HttpContext context)
        {
            ServiceResponse<string> respDelete = new ServiceResponse<string>();
            string conName = context.Request.Query["name"];
            int conId;

            if (int.TryParse(context.Request.Query["id"], out conId) && !String.IsNullOrEmpty(conName))
            {
                SpoonDataWorker.DeleteContainer(conName);

                respDelete.Data = "Success";
                respDelete.Message = "Success";
                respDelete.Success = true;
            }
            else
            {
                respDelete.Data = "Failure";
                respDelete.Message = "Name or Id not valid";
                respDelete.Success = false;
            }

            return respDelete;
        }

        private static bool IsAuthorized(HttpContext context)
        {
            var ret = context.User.HasClaim(c => c.Type == ClaimTypes.Role);
            var claims = context.User.Claims.Select(claim => new { claim.Type, claim.Value }).ToArray();
            bool retval = false;
            if(RequireAuth && (context.User.Identities.Any(i => i.IsAuthenticated)))
            {                
                if (AuthRoles.Count != 0)
                {
                    foreach(string role in AuthRoles)
                    {
                        if(context.User.IsInRole(role))
                        {
                            retval = true;
                            break;
                        }
                    }
                }
                else
                {
                    retval = true;
                }
            }

            return retval;
        }

        #endregion
    }
}
