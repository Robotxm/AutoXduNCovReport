# AutoXduNCovReport

This is a tool that can report you COVID-19 information. You can combine it with GitHub Action so that the report process is fully automatic.

## Requirements

- .NET 5 Runtime for running, or Visual Studio with .NET 5 SDK for developing (download [here](https://dotnet.microsoft.com/download/dotnet/5.0))

## Basic Usages

If you do not want to compile, use `dotnet run` is OK.

`dotnet run -- [--username|-u] <your_student_id> [--password|-p] <your_password> [--sckey|-k] <your_serverchan_key>`

Or compile and replace `dotnet run --` with your executable file name, where `--username` (or `-u`) and `--password` (or `-p`) are required.

If you want to receive a notification when submit unsuccessfully, you can specify your Serverchan key with option `--sckey` (or `-k`). This is optional so just ignore it if you do not want to be disturbed.

### Examples

The option `--username` can also be `-u`, `--password` can also be `-p` and `--sckey` can also be `-k`.

- Submit immediately

  `dotnet run -- -u 1234567890 -p aabbccd`

- Submit immediately and receive notification via Serverchan

  `dotnet run -- -u 1234567890 -p aabbccd -k 1a2a3d4f`