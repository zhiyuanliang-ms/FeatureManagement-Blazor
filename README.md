# FeatureManagement-Blazor
Investigating how FeatureManagement can be enhanced to support Blazor

Currently, FeatureManagement.AspNetCore provides FeatureGateAttribute, FeatureTagHelper, feature-flag-based MVC filter and middleware.

Blazor is a framework for building interactive client-side web UI based on the Razor component.

There is also Blazor Server option. Blazor Server provides support for hosting Razor components on the server in an ASP.NET Core app. UI updates are handled over a SignalR WebSocket connection.

Blazor apps run outside of the ASP.NET Core pipeline. There will only be one HTTP request happened in the beginning. After that, all interactions are through WebSocket.

FeatureGateAttribute is built from ASP.NET ActionFilter which is incompatible with Blazor.
MVC filter and middleware are also incompatible with Blazor, since they are based on the ASP.NET pipeline.

HttpContextAccessor/HttpContext should be avoided in the Razor components of Blazor server apps. A critical aspect of server-side Blazor security is that the information attached to a given circuit might become updated at some point after the Blazor circuit is established but the IHttpContextAccessor isn't updated. [Ref](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/server/threat-mitigation?view=aspnetcore-7.0#avoid-ihttpcontextaccessorhttpcontext-in-razor-components)


The recommended approach for passing request state to the Blazor app is through root component(App.razor) parameters during the app's initial rendering(_Host.cshtml). Alternatively, the app can copy the data into a scoped service in the root component's initialization lifecycle event for use across the app.

This will make the HttpContextAccessor pattern for TargetingFilter fail. 
Besides, the FeatureManager and feature filters are always registered as a singleton. Scoped services can not be consumed by them.

Tag Helpers aren't supported in Razor components. [Ref](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/?view=aspnetcore-3.1#tag-helpers-arent-supported-in-components)

FeatureTagHelper can only be used in _Host.cshtml

