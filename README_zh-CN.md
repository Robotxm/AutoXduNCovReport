# AutoXduNCovReport

[English](./README.md) | 简体中文

这个工具可以帮你提交疫情信息。结合 GitHub Action 可以使这个过程完全自动化。

工具支持提交健康卡和晨午晚检信息。具体请继续阅读下面的文档。

## 前置要求

- 如果只想运行，那么需要 .NET 6 运行时。如果需要再次开发，需要 Visual Studio 和 .NET 6 SDK（可以[在这里下载](https://dotnet.microsoft.com/download/dotnet)）。

## 基本用法

```
dotnet run -- [ncov|tcheck] [--username|-u] <your_student_id> [--password|-p] <your_password> [--campus|-c] <N|S> [--token|-k] <your_pushplus_token>
```

如果不想编译源代码，可以直接使用 `dotnet run`。或者编译之后将 `dotnet run --` 替换为可执行文件的名称。`--username`（或 `-u`）和 `--password`（或 `-p`）是必需选项。

提交晨午晚检信息时，`--campus`（或 `-c`）是必需选项，值可以是 `N` 或 `S`，分别代表北校区和南校区。

如果想在提交失败时（例如执行过程中出现异常或者提交不成功）收到通知提醒，可以通过 `--token`（或 `-k`） 选项指定 PushPlus 的 Token。这个选项是可选的，不想被打扰的话就忽略吧。有关 PushPlus 的注册和其他用法，参见 [PushPlus 官方网站](https://www.pushplus.plus/)。

可以使用 `dotnet run -- [ncov|tcheck] --help` 查看所有的命令及其选项。

### 示例

`--username`、`--password` 和 `--token` 可以分别简写为 `-u`、`-p` 和 `-k`。

#### 提交健康卡信息

- 立即提交

  `dotnet run -- ncov -u 1234567890 -p aabbccd`

- 立即提交并在出错时接收 PushPlus 通知

  `dotnet run -- ncov -u 1234567890 -p aabbccd -k 1a2a3d4f`

#### 提交晨午晚检信息

**注意，在提交晨午晚检信息时，必须指定 `--campus`（或 `-c`）选项，值可以是 `N` 或 `S`。**

- 立即提交

  `dotnet run -- tcheck -u 1234567890 -p aabbccd -c N`

- 立即提交并在出错时接收 PushPlus 通知

  `dotnet run -- tcheck -u 1234567890 -p aabbccd -c N -k 1a2a3d4f`

## 高级用法：结合 GitHub Action（推荐）

### 开始之前

首先 Fork 此 Repo。

### 创建 Secret

首先转到 Settings → Secrets → Actions，点击“New repository secret”，创建以下必需 Secret：

- `USERNAME`: 值为学号，例如 `1234567890`
- `PASSWORD`: 值为统一身份认证系统的密码，例如 `aabbccd`
- `FUNCTION`: 提交晨午晚检信息时，值为 `tcheck`；提交健康卡信息时，值为 `ncov`
- `CAMPUS`: 提交晨午晚检信息时，如果位于南校区，则值为 `-c S`；如果位于北校区，则值为 `-c N`。提交疫情通信息时，值为 ` `**（注意是一个空格而不是空值，因为 GitHub 不允许空值 Secret）**

如果想结合 PushPlus 使用，再创建以下 Secret：

- `SCKEY`: 值为你申请的 PushPlus 的 Token

到目前为止，Workflow 已经可以执行了。下面是一些其他的自定义选项供参考。

### 切换晨午晚检和健康卡

在校期间需要填写晨午晚检，而假期在家时需要填写健康卡。按照“创建 Secret”一节中的说明修改 `FUNCTION` 的值即可。改动在下次运行 Workflow 时生效。

### 手动运行 Workflow（可选）

此 Repo 的 Workflow 支持手动运行。如有需要，转到 Actions，点击左侧窗格中的第二个“Auto NCov Report”，再点击右侧的“Run workflow”即可。

### 修改计划时间（可选）

打开 `.github/workflows` 文件夹中的 `workflow.yml` 文件，默认情况下会看到以下内容：

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

`- cron: '0 0,4,10 * * *'` 这一行指定了计划时间。需要注意，GitHub 使用的时区是 UTC。这也就意味着，要使用 UTC+8 时区（北京时间），需要从目标时间中减去 8 小时。`0 0,4,12 * * *` 表示任务会在每天 8:00 UTC+8（0:00 UTC）、12:00 UTC+8（8:00 UTC）和 18:00 UTC+8（10:00 UTC）的时候运行。

如果想要指定其他时间，可以修改 `cron` 字段（引号里面的内容）。支持 cron 表达式。

修改完成之后，保存并提交 Workflow。

## 注意

在填写健康卡时，会使用前一次填写的定位信息。因此如果需要修改定位，请在定时填写触发前提前在网页中手动填写。
