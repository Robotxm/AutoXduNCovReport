# AutoXduNCovReport

This is a tool that can report you COVID-19 information. You can combine it with GitHub Action so that the report process is fully automatic.

## Requirements

- .NET 5 Runtime for running, or Visual Studio with .NET 5 SDK for developing (download [here](https://dotnet.microsoft.com/download/dotnet/5.0))

## Basic Usages

If you do not want to compile, use `dotnet run` is OK.

`dotnet run -- [--username|-u] <your_student_id> [--password|-p] <your_password> [--sckey|-k] <your_serverchan_key>`

Or compile and replace `dotnet run --` with your executable file name, where `--username` (or `-u`) and `--password` (or `-p`) are required.

If you want to receive a notification when process is failed (e.g. there is any exception while executing or submit unsuccessfully), you can specify your Serverchan key with option `--sckey` (or `-k`). This is optional so just ignore it if you do not want to be disturbed.

### Examples

The option `--username` can also be `-u`, `--password` can also be `-p` and `--sckey` can also be `-k`.

- Submit immediately

  `dotnet run -- -u 1234567890 -p aabbccd`

- Submit immediately and receive notification via Serverchan

  `dotnet run -- -u 1234567890 -p aabbccd -k 1a2a3d4f`

## Advanced Usages

**I have created GitHub Action workflow. So just fork this repository and follow the instructions below to create secrets and change the scheduled time if you want, then enjoy~**

It is inconvenient to run the application manually every day. So let's play with GitHub Action to free ourselves.

Before we play, fork this repository. You do not want to store your username and password in my repository, don't you? After fork, let's start.

First go to Settings -> Secrets, click 'New repository secret' to create secrets that represent your username and password. Take an example, let's use `USERNAME` for name with value `1234567890` and `PASSWORD` for password with value `aabbccd`. If you want to work with Serverchan, also create `SCKEY` with value `1a2a3d4f` (certainly this should be your own Serverchan key).

Then go to Actions, we can see '.NET' in suggestions. Click 'Set up with this workflow'. We can see a yaml file in the textbox. The `name` field can be changed to whatever you like. In section `on`, remove the `push` and `pull` sub-sections. Add following contents:

```yaml
workflow_dispatch:
schedule:
    - cron: '0 0 * * *'
```

`workflow_dispatch` allows us to run workflow manually. To do this, go to Actions, click 'Auto NCov Report' in the left panel and 'Run workflow' in the right. `schedule` allows the workflow to be run at a certain time.

Notice that GitHub uses time zone UTC+0, so we need substract 8 hours from the desired time if we want to use time zone UTC+8. `0 0 * * *` represents that the task will run at 8:00 UTC+8 (0:00 UTC+0) everyday.

Next, in section `steps`, replace `Build` and `Test` with following contents, here I choose to submit immediately and receive notification for example:

```yaml
- name: Run and send notification
  run: |
    cd AutoXduNCovReport
    dotnet run -- -u ${{ secrets.USERNAME }} -p ${{ secrets.PASSWORD }} -k ${{ secrets.SCKEY }}
```

Save and commit the workflow. Then we can try to run manually and check the result.

## Notice

This tool uses the information you submitted before, so if you want to change the geolocation, please submit on the website before the schedule is triggered.
