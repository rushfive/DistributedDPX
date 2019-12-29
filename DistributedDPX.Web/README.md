#### Setting up Docker (for local dev)

*Make sure to add orchestration using docker to the project/solution too:*

```
in Visual Studio 2017 there is an additional Docker command for the F5 key action. 
This option lets you run or debug a multi-container application by running 
all the containers that are defined in the docker-compose.yml files at the 
solution level. The ability to debug multiple-container solutions means that 
you can set several breakpoints, each breakpoint in a different project (container), 
and while debugging from Visual Studio you will stop at breakpoints defined in 
different projects and running on different containers.
```

Need these 2 things:

**Cert Filename**

After setting up docker and running the project in VS, check `%APPDATA%\ASP.NET\Https` and a cert (.pfx) should exist.

**Cert Password**

Check the project's user secrets. I saw something like:

```
{
  "Kestrel:Certificates:Development:Password": "88bb630d-0168-42ec-84e8-768a799b0673"
}
```

In the docker compose file, add the env vars for the cert path and password. Example:

```
version: '3.4'

services:
  distributeddpx.web:
    image: ${DOCKER_REGISTRY-}distributeddpxweb
    environment:
      - ASPNETCORE_Kestrel__Certificates__Default__Password: 88bb630d-0168-42ec-84e8-768a799b0673
      - ASPNETCORE_Kestrel__Certificates__Default__Path: \https\DistributedDPX.Web.pfx
    build:
      context: .
      dockerfile: DistributedDPX.Web\Dockerfile
```

Resources:

https://github.com/dotnet/dotnet-docker/blob/master/samples/aspnetapp/aspnetcore-docker-https-development.md

https://github.com/dotnet/dotnet-docker/blob/master/samples/aspnetapp/aspnetcore-docker-https.md

---