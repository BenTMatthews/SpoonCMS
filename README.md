# SpoonCMS
A lightweight .NET core CMS for page content - In Alpha (Just don't use it yet)

## Why would you use this?
Mostly because you like coding in .Net, but want some flexibility in your content. For the longest time in .Net web dev, if you wanted content management you had to choose between an extremely complex CMS systems that felt like a language/platform unto itself, or deploy updates to html or content files everytime you wanted any update with little exception. So I built a very simple system to manage content (actual content) that did these key things:
- Easy to integrate (2 lines of code for base implementation)
- Simple conceptually (You store HTML, you get HTML out)
- Let's me code in .Net without impediment

This is the core of what SpoonCMS does: very simple page content management. No routing, no complex auth systems, just managing the markup on your page.

## Getting started
Install the Nuget package: `Install-Package SpoonCMS -Version 0.2.0`
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
ViewData["Footer"] = container.GetItem("FooterContent").Value;
```

Now you can populate them on the view. Remember to use `@Html.Raw` since the html is stored encoded

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
We are managing page content here that is generated from the server, so a couple obvious methods to do it. The most common would be just to use the `ViewData` as shown above, but some have taken to keeping controllers skinny and using helper functions in the view, this works as well: 

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
        
        //Core focuses on claims for identity, you can specify the type and value for the claim to allow
        //If the collection is left empty, only the authenticated check will be done
        //In this example, any users that have the role "admins" or any users that have the name "John" will have access
        SpoonWebWorker.AuthClaims = new List<Claim>() { new Claim(ClaimTypes.Role, "admins"), new Claim(ClaimTypes.Name, "John") };
        SpoonWebWorker.AdminPath = "/admin";
        app.Map(SpoonWebWorker.AdminPath, SpoonWebWorker.BuildAdminPageDelegate);
        ...
    }
```

Some people have said they have very complex auth schemes and allowances they want to use, more than just allowing a certain group or subset of users. I originally said you are moving away from the simplicity of Spoon, but enough people asked for it and I gave in. You can create a controller and endpoint to generate the admin page and and handle the ajax calls:

```
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        ...
        //Note that when going to a controller instead of web delegate, you must remove the 
        //leading '/' from the path. Get it together and standardize this MS!
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
            // Me must specifcy in the second parameter that this is for a controller, so set it to true.
            return Content(SpoonWebWorker.GenerateResponseString(Request.HttpContext, true), "text/html");
        }

    }
}
```

Now we can access our admin page at /admin like before but with the controller instead of the web delegate. It will allow or disallow access based on the auth attribute or scheme you define like you would for any controller. 

Note that all auth actions you define will be for general web requests and api requests for the spoon admin.

#### But I need a LITTLE logic to my content

Again I was hesitant to add too much logic to gathering content since it would move away from simplicity and predictability, but I came around and added the concept of priority. I will also be adding a date range for valid content in a future date. All of this related to the function `Container.GetItem();` without specifying a `ContentItem` name.

This is to accomplish the scenario of quickly switching the priority of `ContentItem` in a container without having to update HTML. For instance, if I have a sale I want active between 1PM and 2PM on my site, but I don't want to copy and paste into the same `ContentItem`, I can have one called "NormalContent" and another called "SaleContent" and simply change their priority in the admin and save it down without having to edit HTML at 1PM and 2PM respectively.

This introduced the concept of using `Container` for dynamic content storage instead of representing an entity like a page. Another exmaple,  I could have a `Container` called "ProductPageContent" and include 3 `ContentItem` called "Normal", "GameDay", and "BigSale". You could have "Normal" as top priorty for everyday, change priority to "GameDay" if the local sportsball team is playing, and or make the top priorty `ContentItem` "BigSale" if you are trying to move more product that day. Your markup or code in your app would not have to change, simply call 

```
ViewData["BodyCopy"] = SpoonDataWorker.GetContainer("ProductPageContent").GetItem().Value;
```

and whichever you set as the high priority for that container in the admin will be what is returned.  


## Where is the data stored?

For the first iteration, all data will be stored in a [LiteDB](https://github.com/mbdavid/LiteDB) instance. It is a fantastic small document database that is light and portable. I subscribe to the idea that content storage should be kept away from app/user storage, and this reflects that. The default path will store it in Data\DB folder, but you can specify a different path on the Configure event like so:

```
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    ...
    SpoonDataWorker.connString = @"MyFolder\Storage\Spoon";
    ...
}
```

## The Admin

The approach for the admin is to be as simple as possible, while not being bland. The example project has a basic template. Once you load the admin with the URL you specified, you will see the list of containers you have created on the left:

![My image](Screens/load.JPG)

Once you select a container, the container's content items will show with a list at the bottom.

![My image](Screens/home.JPG)

The editor will show the content as an HTML WYSIWYG editor by default, but you can choose to work in the source. It is often recommended to code your markup in your IDE of choice, verify it, then paste it into the admin.

You can save content items individually, or save them all and their order within the conteiner using the "Save All & Order" button.

From this page you can also sort the items at the bottom if you plan to use the container items in an ordered manner. 


## FAQs
* "What if I user a big enterprise DB for my apps?"
    * Spoon will not interfere with that. The content calls, and Spoon as a whole, are design to not interfere with functionality.
* "Can't I please store it somewhere else?"
    * Adding other storage options is on the roadmap. This is not a high priority currently based on user feedback, but Mongo is next to be considered.
* "But I need features like creating routes, custom perms, on the fly templates..."
    * All valid things to want, but Spoon is not built for that. Other larger CMS products like Orchard or Umbraco are great products that are built for large scale applications. Thats not the goal of this project.
* "You know this isn't actually a whole lot of work or magic, right?
    * Yes. Just trying to (lightly) formalize work I, and many peers, have had to do a bunch of different times so we never have to again.
* "Okay there has to be SOME way to input content and access it dynamically with redeploying, right?"
    * You can fake it, and I have on some projects. If you make a view, call it Custom, that simply frames a data block, you can have this where you want some dynamic content.
```
@Html.Raw(ViewData["CustomData"])
```

and have this in a catchall controller that checks to see if the path matches an ID in the data store, and return the first content item if it does.

```
//removing leading slash
    string id = Request.Path.Value.Remove(0,1);
    if (!string.IsNullOrEmpty(id))
    {
        var container = SpoonDataWorker.GetContainer(id);

        if (container != null && container.Items.Count > 0)
        {
            ViewData["CustomData"] = container.GetItem().Value;
            return View("Custom");
        }
    }

    return View("NotFound");
```
You will also need to set a catchall route for this in startup, and make sure it's the last one entered.

```
 routes.MapRoute(
    name: "Custom",
    template:"{*AllValues}",
    defaults: new { controller = "Custom", action = "Custom" });
```

This means if you create a container called "Promotion" and try to access mysite.com/Promotion, you will have your view populated with the first content item in the promotion container.

## Thank You

This project is only possible thanks to others and their hard work:

* [LiteDB](https://github.com/mbdavid/LiteDB)
* [jquery](https://github.com/jquery/jquery)
* [handlebars.js](https://github.com/wycats/handlebars.js/)
* [CKEditor](https://github.com/ckeditor)
* [toastr](https://github.com/CodeSeven/toastr)
* [.NET core](https://github.com/dotnet/core)
* [Lauren Kidwell Designs](http://www.laurenkidwell.design/)
* [FontAwesome](https://github.com/FortAwesome/Font-Awesome)

## ToDo

- Redesign admin page (in progress, and badly needed)
- MongoDB DataClass
- SQLLite DataClass
- Active date range
- Enable/disable content Items
