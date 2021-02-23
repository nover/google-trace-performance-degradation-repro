# Google Trace performance degradation reproduction

This is a small dotnet 5.0 WebApi with Google Trace, created with the purpose of hunting down a performance degradation issue uncovered in a production application.

It relates to the Google Trace `fallback predicate`, issue is tracked here on GitHub:
https://github.com/googleapis/google-cloud-dotnet/issues/5966

## How to use

1. Dump a google service account with `trace writer` permissions in the root of the project with name `google-creds.json`
2. Update `appsettings.Development.json` to point to your `google project id`
3. Ensure that enable google trace is set to true in `appsettings.Development.json`
4. In `startup.cs` line 41, comment in our out the custom trace predicate
5. Ensure that you have the open source `k6` load tool installed
6. Start the application: `ASPNETCORE_ENVIRONMENT=Development dotnet run -c Release --project GoogleTracePerformanceRepro/GoogleTracePerformanceRepro.csproj`
7. Start the load script in a new console: `k6 run --vus 50 --duration 240s ./load_script.js`


In the k6 output take note of the `http_reqs`.
