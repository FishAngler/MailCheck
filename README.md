# MailCheck
A C# implementation of the famous <a href="https://github.com/mailcheck/mailcheck">javascript mailcheck plugin</a>

A C# library that suggests a right domain when your users misspell it in an email address.

Installation
------------

#### NuGet ####

```
PM> Install-Package FishAngler.MailCheck
```

Usage
-----

#### Default Usage ####

```
var mailCheck = new MailCheck();
var suggestion = mailCheck.Suggest("test@hotmal.con");
if (suggestion != null)
{
    Debug.WriteLine($"Suggested email: {suggestion.Full}");
}
```

#### Custom Usage ####
```
string[] domains = { "custom-domain.com", "hotmail.com", "gmail.com", "yahoo.com" };
string[] secondLevelDomains = { "custom-domain", "hotmail", "gmail", "yahoo" };
string[] topLevelDomains = { "com" };

var mailCheck = new MailCheck(domains, secondLevelDomains, topLevelDomains);
var suggestion = mailCheck.Suggest("test@hotmal.con");
if (suggestion != null)
{
    Debug.WriteLine($"Suggested email: {suggestion.Full}");
}

suggestion = mailCheck.Suggest("test@custom-doman.con");
if (suggestion != null)
{
    Debug.WriteLine($"Suggested email: {suggestion.Full}");
}
```
