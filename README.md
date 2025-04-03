# Developer notes
Each supplier has their own API endpoints that we access.
For those using swagger we can use the swagger file they provide to generate a client file in c# that we can call from our application

To generate the client from a swagger file:

Install Nswag
```
dotnet tool install -g NSwag.ConsoleCore
```
Run NSwag with the swagger file you have from each supplier
```
nswag openapi2csclient /input:filename.json /output:filename.cs
```
If a supplier updates their endpoints we might have to download a new swagger file and generate a new client for our program.
