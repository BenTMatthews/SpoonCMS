# SpoonCMS
A lightweight CMS for page content - In Alpha (Just don't use it yet)

## Why would you use this?
Mostly because you like coding in .Net, but want some flexibility in your content. For the longest time in .Net web dev, if you wanted content management you had to choose between an extremely complex CMS systems that felt like a language/platform unto itself, or deploy updates to html or content files everytime you wanted any update with little exception. So I built a very simple system to manage content (actual content) that did these key things:
- Easy to integrate (2 lines of code for base implementation)
- Simple conceptually (You store HTML, you get HTML out)
- Let's me code in .Net without impediment

This is the core of what SpoonCMS does: very simple page content management. No routing, no complex auth systems, just managing the markup on your page.

## Getting started
Install the Nuget package: `Install-Package SpoonCMS -Version 0.1.0`
Setup your routes to the admin page
```
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        ...
        SpoonWebWorker.AdminPath = "/admin";
        app.Map(SpoonWebWorker.AdminPath, SpoonWebWorker.BuildAdminPageDelegate);
        ...
    }
```

And you now have it installed and can access the admin page at the path you specificy.

## Key Concepts

There are really only 2 classes you will be dealing with (remember keeping it simple?)

The `ContentItem` class is the basis for your content. For the most part, this will store the name name of your Content (For instance: "HeaderContent") and the value of it which would usually be HTML (`<div>This is the Header Content</div>`). ContentItems will be stored in a `Container`, which at it's heart is just a collection of `ContentItem`.

After you have stored content into a container, you would access it like so:
```
SpoonDataWorker.GetContainer("ContainerName").GetItem("ContentItemName").Value;
```
Html will be returned and you can use it as you like.

Containers are mostly built to reflect the entity for which you use them, but there is flexibility there to organize as you like. For instance I would have a "HomePage" container, with content items for each peice of that entity that I would want to be editable. So to generate my content for my page to render as I like, I would populate it as such:
```
Container container = SpoonDataWorker.GetContainer("HomePage");

ViewData["HeaderContent"] = container.GetItem("HeaderContent").Value;
ViewData["BodyCotentBlock"] = container.GetItem("BodyContentBlock").Value;
ViewData["RightRail"] = container.GetItem("RightRailContent").Value;
ViewData["LeftNav"] = container.GetItem("LeftNavLinks").Value;
ViewData["Footer"] = container.GetItem("FotterContent").Value;
```

The one could populate them on the view. Remember to use `@Html.Raw` since the html is stored encoded

```
<body>
    <div id="header-block">@Html.Raw(ViewData["HeaderContent"])</div>
    <div id="body-copy">@Html.Raw(ViewData["BodyCotentBlock"])</div>
    <div id="right-rail">@Html.Raw(ViewData["RightRail"])</div>
    <div id="left-nav">@Html.Raw(ViewData["LeftNav"])</div>
    <div id="footer">@Html.Raw(ViewData["Footer"])</div>
</body>
```
## Some Examples and alternative cases

#### Gathering content in the view
We are managing page content here that is generated from the server, so a couple obvious methods to do it. The most common would be just to use the `ViewData` as show above, but some have taken to keeping my controllers skinny and using helper functions in the view, thsi works as well: 

```
@functions
{
    SpoonCMS.Classes.Container container = SpoonCMS.Workers.SpoonDataWorker.GetContainer("HomePage");
    public string GetHtmlFromSpoon(string itemName)
    {
        return System.Net.WebUtility.HtmlDecode(container.GetItem(itemName).Value);
    }
}
<body>
    <div id="header-block">@Html.Raw(@GetHtmlFromSpoon("HeaderContent"))</div>
    <div id="body-copy">@Html.Raw(@GetHtmlFromSpoon("BodyCotentBlock"))</div>
    <div id="right-rail">@Html.Raw(@GetHtmlFromSpoon("RightRail"))</div>
    <div id="left-nav">@Html.Raw(@GetHtmlFromSpoon("LeftNav"))</div>
    <div id="footer">@Html.Raw(@GetHtmlFromSpoon("Footer"))</div>
</body>
```

#### Securing the admin
Obviously you do not want the admin page to be open to everyone, so there are some security options.

```
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        ...
        //This requires the request content to the admin to be "Authenticated". If this is left to false
        //No security checks will be done.
        SpoonWebWorker.RequireAuth = true;
        
        //Core focuses on claims for identity, you can specify the role and value for the claim to allow
        //If this is left empty, only the authenticated check will be done
        SpoonCMS.Workers.SpoonWebWorker.AuthClaims = new List<Claim>() { new Claim(ClaimTypes.Role, "admins"), new Claim(ClaimTypes.Name, "John") };
        SpoonWebWorker.AdminPath = "/admin";
        app.Map(SpoonWebWorker.AdminPath, SpoonWebWorker.BuildAdminPageDelegate);
        ...
    }
```

Some people have said they have very complex auth schemes and allowances they want to use, more than just allowing a certain group or subset of users. I originally said you are moving away from the simplicity of Spoon, but enough people ask for it. You can create a controller and endpoint to generate the admin page and it's functions:

```
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        ...
        //Note that when going to a controller instead of web delegate, you must remove the 
        //leading '/' from the path
        SpoonWebWorker.AdminPath = "admin";
        app.UseMvc(routes =>
        {
            routes.MapRoute(
                name: "SpoonAdmin",
                template: SpoonWebWorker.AdminPath + "/{*AllValues}",
                defaults: new { controller = "Spoon", action = "Admin" });

            routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
        });
        ...
    }
   
```

The routes, actions, and controllers are customizable like any other. And then we can generate a controller like so:
```
using Microsoft.AspNetCore.Mvc;
using SpoonCMS.Workers;


namespace ExampleCore.Controllers
{
    public class SpoonController : Controller
    {
        [MyFancyAuthorizationAttribute]
        public IActionResult Admin()
        {
            return Content(SpoonWebWorker.GenerateResponseString(Request.HttpContext, true), "text/html");
        }

    }
}
```

Now we can access our admin page at /admin like before and it will allow or disallow access based on the auth attribute or scheme you define. 

Note that all auth actions you definie will be for general web requests and api requests for the spoon admin.

#### But I need a LITTLE logic to my content

Again I was hesitant to add too much logic to gathering content since it would move away from simplicity and predictability, but I came around and added the concept of priority. I will also be adding a date range for valid content in a future date. All of this related to the function of `Container.GetItem();`

This is to accomplish the scenario of quickly switching the priority of `ContentItem` in a container without having to update HTML. For instance, if I have a sale I want active between 1PM and 2PM on my site, but I don't want to copy and paste into the same `ContentItem`, I can have one names "NormalContent" and another called "SaleContent" and simply change their priority in the admin and save it down without having to edit html.

This introduced the concept of using `Container` for dynamic content storage instead of representing an entity like a page. For instance,  I could have a `Container` called "ProductPageContent" and include 3 `ContentItem` called "Normal", "GameDay", and "BigSale". You could have "Normal" as top priorty for everyday stuff, change priority to "GameDay" if the local sportsball team is playing, and or make the top priorty `ContentItem` "BigSale" if you are trying to move more product that day. Your markup or code would note have to change, simply call 

```
ViewData["BodyCopy"] = SpoonDataWorker.GetContainer("ProductPageContent").GetItem().Value;
```

and whichever you set as the high priority in the admin will be what is returned.  
