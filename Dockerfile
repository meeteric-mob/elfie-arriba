FROM mcr.microsoft.com/dotnet/core/sdk

ADD . /src

RUN dotnet build Arriba.Core.sln
RUN dotnet test