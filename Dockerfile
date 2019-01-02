FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY ./publish/Voltmeter.UI /app
ENTRYPOINT ["dotnet", "Voltmeter.UI.dll"]