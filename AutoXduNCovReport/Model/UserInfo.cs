using Refit;

namespace AutoXduNCovReport.Model
{
    record UserInfo([property:AliasAs("username")] string Username, [property: AliasAs("password")] string Password);
}