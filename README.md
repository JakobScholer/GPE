# Developer notes
ApiSettings.json should contain some combination of IDs, Keys and Secrets for each supplier.
The fields are empty when downloading this project from Github and should remain this way for security reasons.
Add the required information for each supplier when setting it up locally.

## Digikey
They provide a swagger file that can be used to generate a client file in c# that we can call from our application
To generate the client from a swagger file:
Install Nswag
```
dotnet tool install -g NSwag.ConsoleCore
```
Run NSwag with the swagger file you have from each supplier
```
nswag openapi2csclient /input:filename.json /output:filename.cs
```
## Farnell
All information should be imbedded in the request url.
Api key is required for all requests.
Pricing information requires a CustomerID and Secret Key. TODO get this

