using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SpoonCMS.Classes;
using SpoonCMS.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

        public static string AdminPath { get; set; }

        #region PageGeneration

        public static void BuildAdminPageDelegate(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                switch (context.Request.Method.ToUpper())
                {
                    case "GET":
                        if (context.Request.Path.HasValue)
                        {
                            SpoonDataWorker data = new SpoonDataWorker();
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
                                        respConsSkinny.Data = null;
                                        respConsSkinny.Message = ex.Message;
                                        respConsSkinny.Success = false;
                                    }
                                    await context.Response.WriteAsync(JsonConvert.SerializeObject(respConsSkinny));
                                    break;

                                case "/GETCONTAINER":
                                    ServiceResponse<Container> respCon = new ServiceResponse<Container>();
                                    try
                                    {
                                        respCon = GetContainers(context);
                                    }
                                    catch (Exception ex)
                                    {
                                        respCon.Data = null;
                                        respCon.Message = ex.Message;
                                        respCon.Success = false;
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
                                    catch(Exception ex)
                                    {
                                        respSave.Data = null;
                                        respSave.Message = ex.Message;
                                        respSave.Success = false;
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
                                        respCreate.Data = "Failure";
                                        respCreate.Message = ex.Message;
                                        respCreate.Success = false;
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
                                        respEditName.Data = "Failure";
                                        respEditName.Message = ex.Message;
                                        respEditName.Success = false;
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
                                        respDelete.Data = "Failure";
                                        respDelete.Message = ex.Message;
                                        respDelete.Success = false;
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
            SpoonDataWorker data = new SpoonDataWorker();
            respConsSkinny.Data = data.GetAllContainers();
            respConsSkinny.Message = "Success";
            respConsSkinny.Success = true;

            return respConsSkinny;
        }

        private static ServiceResponse<Container> GetContainers(HttpContext context)
        {
            ServiceResponse<Container> respCon = new ServiceResponse<Container>();
            int id;
            Container retVal = null;
            if (int.TryParse(context.Request.Query["id"], out id))
            {
                SpoonDataWorker data = new SpoonDataWorker();
                retVal = data.GetContainer(id);

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

            if (int.TryParse(context.Request.Query["id"], out id))
            {
                string containerString;
                using (StreamReader reader = new StreamReader(context.Request.Body))
                {
                    containerString = reader.ReadToEnd();
                }

                Container postedCon = JsonConvert.DeserializeObject<Container>(containerString);

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

                SpoonDataWorker data = new SpoonDataWorker();
                data.UpdateContainer(postedCon);

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
                SpoonDataWorker dataCon = new SpoonDataWorker();
                dataCon.AddContainer(newCon);

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
                SpoonDataWorker dataCon = new SpoonDataWorker();
                dataCon.UpdateContainerName(conId, conName);

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
                SpoonDataWorker dataCon = new SpoonDataWorker();
                dataCon.DeleteContainer(conName);

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

        #endregion
    }
}
