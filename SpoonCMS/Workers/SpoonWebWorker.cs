using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SpoonCMS.Classes;
using SpoonCMS.DataClasses;
using SpoonCMS.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static SpoonCMS.DataClasses.Enums;

namespace SpoonCMS.Workers
{
    public static class SpoonWebWorker
    {
        private static string _htmlPath = "SpoonCMS.Files.Admin.html";
        private static string _jsPath = "SpoonCMS.Files.Script.js";
        private static string _cssPath = "SpoonCMS.Files.Styles.css";
        private static string _containerListTemplatePath = "SpoonCMS.Files.Templates.ContainerList.hbs";
        private static string _containerDetailsTemplatePath = "SpoonCMS.Files.Templates.ContainerDetails.hbs";
        private static string _iconPath = "SpoonCMS.Files.Icon.jpg";

        private static string _jsMarker = "[!CustomScriptBlock!]";
        private static string _cssMarker = "[!CustomStyleBlock!]";
        private static string _jsPathMarker = "[!CustomPathMarker!]";

        public static string AdminPath { get; set; } = @"/admin";
        public static bool RequireAuth { get; set; } = false;
        public static List<Claim> AuthClaims { get; set; } = new List<Claim>();

        public static ISpoonData SpoonData;

        #region DataGeneration

        public static ISpoonData GenerateDataWorker(SpoonDBType dbType, string connString)
        {
            ISpoonData dataWorker;

            switch (dbType)
            {
                case SpoonDBType.LiteDB:
                    dataWorker = new LiteDBData(connString);
                    break;

                case SpoonDBType.PostGres:
                    dataWorker = new PostGresData(connString);
                    break;

                default:
                    dataWorker = new LiteDBData();
                    break;
            }

            return dataWorker;
        }

        #endregion

        #region PageGeneration

        public static void BuildAdminPageDelegate(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                string resp = GenerateResponseString(context);
                await context.Response.WriteAsync(resp);
            });
        }

        public static string GenerateResponseString(HttpContext context, bool fromController = false)
        {
            string response;
            string path;
            string method = context.Request.Method;

            //Add a prefix "/" and remove the admin path from the context path. This is different when using staight delagate. PLease standardize this MS!
            if (context.Request.Path.HasValue)
            {
                path = (fromController ? context.Request.Path.Value.Replace("/" + AdminPath, "") : context.Request.Path.Value);
            }
            else
            {
                path = null;
            }
            
            if (IsAuthorizedAsync(context))
            {
                switch (method.ToUpper())
                {
                    case "GET":
                        if (!String.IsNullOrEmpty(path))
                        {
                            switch (path.ToUpper())
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
                                    response = JsonConvert.SerializeObject(respConsSkinny);
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

                                    response = JsonConvert.SerializeObject(respCon);
                                    break;

                                default:
                                    response = context.Request.Path.Value + ": No action found";
                                    break;
                            }
                        }
                        else
                        {
                            response = BuildAdminPageString();
                        }
                        break;

                    case "POST":
                        if (!String.IsNullOrEmpty(path))
                        {
                            switch (path.ToUpper())
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

                                    response = JsonConvert.SerializeObject(respSave);
                                    break;

                                case "/SAVECONTENTITEM":
                                    ServiceResponse<string> respContentSave = new ServiceResponse<string>();
                                    try
                                    {
                                        respContentSave = SaveContentItem(context);
                                    }
                                    catch (Exception ex)
                                    {
                                        respContentSave.SetToError(ex);
                                    }

                                    response = JsonConvert.SerializeObject(respContentSave);
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

                                    response = JsonConvert.SerializeObject(respCreate);
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

                                    response = JsonConvert.SerializeObject(respEditName);
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

                                    response = JsonConvert.SerializeObject(respDelete);
                                    break;

                                default:
                                    response = context.Request.Path.Value + ": No action found";
                                    break;
                            }
                        }
                        else
                        {
                            response = BuildAdminPageString();
                        }
                        break;
                    default:
                        response = BuildAdminPageString();
                        break;
                }
            }
            else
            {
                response = context.Request.Path.Value + " Access Denied";
            }

            return response;
        }

        private static string BuildAdminPageString()
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

            using (Stream stream = assembly.GetManifestResourceStream(_iconPath))
            {
                byte[] filebytes = new byte[stream.Length];
                stream.Read(filebytes, 0, Convert.ToInt32(stream.Length));
                string base64 = Convert.ToBase64String(filebytes, Base64FormattingOptions.None);               
                html = html.Replace("[!IconBlock!]", base64);                
            }

            retval = html.Replace(_jsMarker, js).Replace(_cssMarker, css) + hbTemplates;

            return retval;
        }

        #endregion

        #region PageOperations

        private static ServiceResponse<List<ContainerSkinny>> GetContainers()
        {
            ServiceResponse<List<ContainerSkinny>> respConsSkinny = new ServiceResponse<List<ContainerSkinny>>();
            respConsSkinny.Data = SpoonData.GetAllContainers();
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
                retVal = SpoonData.GetContainer(id);

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

                SpoonData.UpdateContainer(postedCon);

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

        private static ServiceResponse<string> SaveContentItem(HttpContext context)
        {
            ServiceResponse<string> respSave = new ServiceResponse<string>();
            int id;
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

            if (int.TryParse(context.Request.Query["id"], out id))
            {
                string contentString;
                using (StreamReader reader = new StreamReader(context.Request.Body))
                {
                    contentString = reader.ReadToEnd();
                }

                ContentItem postedContent = JsonConvert.DeserializeObject<ContentItem>(contentString, settings);
                               
                Container con = SpoonData.GetContainer(id);

                string itemKey = con.Items.FirstOrDefault(x => x.Value.Id == postedContent.Id).Key; //See if exists
                if(!string.IsNullOrEmpty(itemKey))
                {
                    con.RemoveItem(itemKey); // just remove the esisting item we are replacing.
                }

                con.AddItem(postedContent);

                SpoonData.UpdateContainer(con);

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
                SpoonData.AddContainer(newCon);

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
                SpoonData.UpdateContainerName(conId, conName);

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

            if (!String.IsNullOrEmpty(conName))
            {
                SpoonData.DeleteContainer(conName);

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

        private static bool IsAuthorizedAsync(HttpContext context)
        {                   
            bool retval = false;
            if (RequireAuth)
            {
                try
                {
                    AuthenticateResult auth = context.AuthenticateAsync().Result;

                    if (auth.Succeeded)
                    {
                        var userClaims = auth.Principal.Claims.Select(claim => new { claim.Type, claim.Value }).ToArray();
                        if (AuthClaims.Count != 0)
                        {
                            foreach (Claim claim in AuthClaims)
                            {
                                if (userClaims.Any(x => x.Type == claim.Type && x.Value == claim.Value))
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
                }
                catch(AggregateException aggEx)
                {
                    return false;
                }
                catch(Exception ex)
                {
                    return false;
                }
            }
            else
            {
                retval = true;
            }

            return retval;
        }

        #endregion
    }
}
