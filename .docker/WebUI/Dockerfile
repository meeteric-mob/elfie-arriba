FROM arriba-base as build

# # #  --- Functions Runtime Image --- # # #
FROM mcr.microsoft.com/dotnet/core/runtime

COPY --from=build /src/Arriba/Tools/bin/Debug/netcoreapp3.1 /Arriba/Tools/bin/app
ADD ./service-config/ /Arriba/Databases
WORKDIR /Arriba/Tools/bin/app

RUN ls -l
RUN find /Arriba
CMD ["dotnet", "Arriba.WorkItemCrawler.dll", "cseng", "-i"] 