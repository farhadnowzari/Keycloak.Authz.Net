# Keycloak.Authz.Net

This library is created to bring the Keycloak's fine-grained authorization to C#.

If you want to learn more about how to configure keycloak as an authorization server, check [this video](https://www.youtube.com/watch?v=E5g50CVRGv8) or if you are not a visual learner, check the [official keycloak documentation](https://www.keycloak.org/docs/latest/authorization_services/index.html).

## Installation

This library is available in nuget and can be installed with the following command:
```sh
dotnet add package Keycloak.Authz.Net
```

**Note**: This is a dotnet 8 specific library.

## Why should you use this?

Well if you already know how the legacy `[Authorize]` attribute works, or how `.RequireAuthorization()` extension method for endpoints works, then you will have no problem to switch to something much more complex, but simplified enough so you can use it exactly the same as before.

For example the attributes and extension methods are changed like this:

* [Authorize] -> [Authz]
* .RequireAuthorization() -> .RequireAuthz()

Now if you are interested, check out the [Documentation](https://github.com/farhadnowzari/Keycloak.Authz.Net/wiki)

## Contributing
Anyone from the community is so welcomed to just jump in, fix things or add new features. I am maintaining this project just by myself, but will appreciate some help 😊

Just simply make a PR and we will check it out together.

## Resources
Check the [Documentation] for more info about how to set it up.

For an example project, check [example](https://github.com/farhadnowzari/Keycloak.Authz.Net/tree/main/example)

For more information on Keycloak fine-grained authorization check:
* [My video on it](https://www.youtube.com/watch?v=E5g50CVRGv8)
* Keycloak [official documentations](https://www.keycloak.org/docs/latest/authorization_services/index.html).