# AutoXduNCovReport

English | [简体中文](./README_zh-CN.md)

This is a tool that can report your COVID-19 information. You can combine it with GitHub Action so that the report process is fully automatic.

This tool supports submitting both daily information (a.k.a. '健康卡') and daily 3-time information (a.k.a. '晨午晚检'). For details, read the documents below.

## Requirements

- .NET 6 Runtime for running, or Visual Studio with .NET 6 SDK for developing (can be downloaded [here](https://dotnet.microsoft.com/download/dotnet)).

## Basic Usages

```
dotnet run -- [ncov|tcheck] [--username|-u] <your_student_id> [--password|-p] <your_password> [--campus|-c] <N|S> [--token|-k] <your_pushplus_token>
```

If you do not want to compile the source code, use `dotnet run` is OK. Or compile and replace `dotnet run --` with your executable file name, where `--username` (or `-u`) and `--password` (or `-p`) are required.

If you are submitting daily 3-time information, the option `--campus` (or `-c`) with value `N` for north campus or `S` for south campus is required.

If you want to receive a notification when submitting is failed (e.g. there is any exception while executing or submit unsuccessfully), you can specify your PushPlus token with option `--token` (or `-k`). This is optional so just ignore it if you do not want to be disturbed. For register PushPlus and other usages, refer to [PushPlus' Offical Website](https://www.pushplus.plus/)。

To view all commands and their options, use `dotnet run -- [ncov|tcheck] --help`.

### Examples

The option `--username` can also be `-u`, `--password` can also be `-p` and `--token` can also be `-k`.

#### Submit your daily information (a.k.a. '健康卡')

- Submit immediately

  `dotnet run -- ncov -u 1234567890 -p aabbccd`

- Submit immediately and receive notification via PushPlus if any exception is raised

  `dotnet run -- ncov -u 1234567890 -p aabbccd -k 1a2a3d4f`

#### Submit your daily 3-time information (a.k.a. '晨午晚检')

**Notice that the option `--campus` (or `-c`) with value `N` or `S` is required when submit daily 3-time information.**

- Submit immediately

  `dotnet run -- tcheck -u 1234567890 -p aabbccd -c N`

- Submit immediately and receive notification via PushPlus if any exception is raised

  `dotnet run -- tcheck -u 1234567890 -p aabbccd -c N -k 1a2a3d4f`

## Advanced Usages: Play with GitHub Action (Recommended)

### Before we start

Fork this repository before we start.

### Create secrets

First go to Settings -> Secrets -> Actions, click 'New repository secret' to create secrets following required secrets:

- `USERNAME`: The value is your student ID, such as `1234567890`
- `PASSWORD`: The value is your password for IDS, such as `aabbccd`
- `FUNCTION`: The value is either `tcheck` for submitting daily 3-time information or `ncov` for submitting daily information
- `CAMPUS`: For submitting daily 3-time information, the value is `-c S` if you live in south campus or  `-c N` if you live in north campus. For submitting daily information, the value is ` ` **(a space instead of an empty value, as GitHub does not allow the empty value)**

If you want to work with PushPlus, create one more secret:

- `SCKEY`: The value is your PushPlus token

So far the workflow can be executed. For more preferences, read the following.

### Switch between daily information and daily 3-time information

Normally, we are required to submit daily 3-time information on campus and submit daily information at home. Just change the value of secret `FUNCTION` following the instructions in 'Create secrets' section and the change will be applied next time the workflow is triggered.

### Run workflow manually (optional)

The workflow in this repository is configured to support running manually. To do this, go to Actions, click the second 'Auto NCov Report' in the left panel and 'Run workflow' in the right.

### Change scheduled time (optional)

Open `workflow.yml` in `.github/workflows` folder, you will see following content by default:

```yaml
name: Auto NCov Report

on:
  workflow_dispatch:
  schedule:
    - cron: '0 0,4,10 * * *'

jobs:
  build:

    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 5.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Run and send notification
      run: |
        cd AutoXduNCovReport
        dotnet run -- ${{ secrets.FUNCTION }} -u ${{ secrets.USERNAME }} -p "${{ secrets.PASSWORD }}" ${{ secrets.CAMPUS }} -k ${{ secrets.SCKEY }}
```

The line `- cron: '0 0,4,10 * * *'` specify the scheduled time. Notice that GitHub uses time zone UTC, so you need substract 8 hours from the desired time if you want to use time zone UTC+8. `0 0,4,12 * * *` represents that the task will run at 8:00 UTC+8 (0:00 UTC), 12:00 UTC+8 (8:00 UTC) and 18:00 UTC+8 (10:00 UTC) everyday.

If you want to specify another time different from the default value, you can modify the `cron` field (the content within quotes). It supports cron expressions.

After finishing all modification, Save and commit the workflow.

## Notice

This tool uses the information you submitted before when commit daily information, so if you want to change the geolocation, please submit on the website before the schedule is triggered.
