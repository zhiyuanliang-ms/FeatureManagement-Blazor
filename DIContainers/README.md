The objectives of this project are: 
1. To demonstrate how to use Microsoft.FeatureManagement with other dependency injection frameworks.
2. To explain the reason why people might prefer to use the open-source dependency injection framework, such as AutoFac, instead of `Microsoft.Extensions.DependencyInjection` (MEDI).

There are some popular dependency injection frameworks including AutoFac, Unity, Ninject and Castle.Windsor. ([ref](https://learn.microsoft.com/en-us/dotnet/architecture/porting-existing-aspnet-apps/dependency-injection-differences))
The project provides demos to demonstrate how to register Feature Management services for these frameworks.

`Microsoft.Extensions.DependencyInjection` is the dependency injection framework for .NET provided by Microsoft, which designed to meet base-level functional needs. While MEDI offers a limited set of features, developers seeking enhanced capabilities have plenty of reasons to opt for alternative dependency injection frameworks that offer a more powerful feature set.

Using AutoFac for comparison, I will provide two examples to illustrate the shortcomings of MEDI.

