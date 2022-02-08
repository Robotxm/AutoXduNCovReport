# AutoXduNCovReport

English | [简体中文](./README_zh-CN.md)

This is a tool that can report your COVID-19 information. You can combine it with GitHub Action so that the report process is fully automatic.

This tool supports submitting both daily information (a.k.a. '健康卡') and daily 3-time information (a.k.a. '晨午晚检'). For details, read the documents below.

## Requirements

- .NET 6 Runtime for running, or Visual Studio with .NET 6 SDK for developing (can be downloaded [here](https://dotnet.microsoft.com/download/dotnet)).

## Basic Usages

If you do not want to compile the source code, use `dotnet run` is OK. Or compile and replace `dotnet run --` with your executable file name, where `--username` (or `-u`) and `--password` (or `-p`) are required.

If you are submitting daily 3-time information, the option `--campus` (or `-c`) with value `N` for north campus or `S` for south campus is required.

`dotnet run -- [ncov|tcheck] [--username|-u] <your_student_id> [--password|-p] <your_password> [--campus|-c] <N|S> [--token|-k] <your_pushplus_token>`

If you want to receive a notification when submitting is failed (e.g. there is any exception while executing or submit unsuccessfully), you can specify your PushPlus token with option `--token` (or `-k`). This is optional so just ignore it if you do not want to be disturbed. For register PushPlus and other usages, refer to [PushPlus' Offical Website](https://www.pushplus.plus/)。

To view all commands and their options, use `dotnet run -- [ncov|tcheck] --help`.

### Examples

The option `--username` can also be `-u`, `--password` can also be `-p` and `--token` can also be `-k`.

#### Submit your daily information (a.k.a. '健康卡')

- Submit immediately

  `dotnet run -- ncov -u 1234567890 -p aabbccd`

- Submit immediately and receive notification via PushPlus

  `dotnet run -- ncov -u 1234567890 -p aabbccd -k 1a2a3d4f`

#### Submit your daily 3-time information (a.k.a. '晨午晚检')

**Notice that the option `--campus` (or `-c`) with value `N` or `S` is required when submit daily 3-time information.**

- Submit immediately

  `dotnet run -- tcheck -u 1234567890 -p aabbccd -c N`

- Submit immediately and receive notification via PushPlus

  `dotnet run -- tcheck -u 1234567890 -p aabbccd -c N -k 1a2a3d4f`

## Advanced Usages: Play with GitHub Action

### TL; DR

Fork this repository and follow the instructions in 'Create secrets' section below to create secrets and change the scheduled time if you like. Then enjoy~

### Full version

It is inconvenient to run the application manually every day. So let's play with GitHub Action to free ourselves.

Before we start, fork this repository. You do not want to store your username and password in my repository, don't you (also impossible)? After fork, let's rock.

#### Create secrets

First go to Settings -> Secrets, click 'New repository secret' to create secrets for your username and password. Take an example, let's use `USERNAME` for name with value `1234567890` and `PASSWORD` for password with value `aabbccd`. If you want to work with PushPlus, also create `SCKEY` with value `1a2a3d4f` (certainly this should be your own PushPlus token). Although the latest version has replaced Serverchan with PushPlus, here and in the GitHub Action configuration file in the repository I am still using `SCKEY` for compatibility.

You need create 2 more secrets. One is `FUNCTION` with value `tcheck` (or `ncov` if you want to submit daily information) and the other is `CAMPUS` with value `-c S` (or `-c N` if you live in north campus. **Specify empty value if you want to submit daily information instead of daily 3-time information**).

#### Create or edit workflow

Then go to Actions, you can see '.NET' in suggestions. Click 'Set up with this workflow', you can see a yaml file in the textbox. The `name` field can be changed to whatever you like. In section `on`, remove the `push` and `pull` sub-sections. Add following contents:

```yaml
workflow_dispatch:
schedule:
    - cron: '0 0,4,10 * * *'
```

`workflow_dispatch` allows us to run workflow manually. To do this, go to Actions, click 'Auto NCov Report' in the left panel and 'Run workflow' in the right. `schedule` allows the workflow to be run at a certain time.

Notice that GitHub uses time zone UTC, so you need substract 8 hours from the desired time if you want to use time zone UTC+8. `0 0,4,12 * * *` represents that the task will run at 8:00 UTC+8 (0:00 UTC), 12:00 UTC+8 (8:00 UTC) and 18:00 UTC+8 (10:00 UTC) everyday. You can also specify any other time you like.

Next, in section `steps`, replace `Build` and `Test` with following contents, here I take submitting immediately and receiving notification for example:

```yaml
- name: Run and send notification
  run: |
    cd AutoXduNCovReport
    dotnet run -- ${{ secrets.FUNCTION }} -u ${{ secrets.USERNAME }} -p ${{ secrets.PASSWORD }} ${{ secrets.CAMPUS}} -k ${{ secrets.SCKEY }}
```

Save and commit the workflow. Then you can try to run manually and check the result.

#### Switch between daily information and daily 3-time information

Normally, we are required to submit daily 3-time information on campus and submit daily information at home. Just change the value of secret `FUNCTION` following the instructions above and the change will be applied next time the workflow is triggered.

## Notice

This tool uses the information you submitted before when commit daily information, so if you want to change the geolocation, please submit on the website before the schedule is triggered.
