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
