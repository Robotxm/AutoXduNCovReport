# AutoXduNCovReport

[English](./README.md) | 简体中文

这个工具可以帮你提交疫情信息。结合 GitHub Action 可以使这个过程完全自动化。

工具支持提交健康卡和晨午晚检信息。具体请继续阅读下面的文档。

## 前置要求

- 如果只想运行，那么需要 .NET 6 运行时。如果需要再次开发，需要 Visual Studio 和 .NET 6 SDK（可以[在这里下载](https://dotnet.microsoft.com/download/dotnet)）。

## 基本用法

如果不想编译源代码，可以直接使用 `dotnet run`。或者编译之后将 `dotnet run --` 替换为可执行文件的名称。`--username`（或 `-u`）和 `--password`（或 `-p`）是必需选项。

提交晨午晚检信息时，`--campus`（或 `-c`）是必需选项，值可以是 `N` 或 `S`，分别代表北校区和南校区。

`dotnet run -- [ncov|tcheck] [--username|-u] <your_student_id> [--password|-p] <your_password> [--campus|-c] <N|S> [--token|-k] <your_pushplus_token>`

如果想在提交失败时（例如执行过程中出现异常或者提交不成功）收到通知提醒，可以通过 `--token`（或 `-k`） 选项指定 PushPlus 的 Token。这个选项是可选的，不想被打扰的话就忽略吧。有关 PushPlus 的注册和其他用法，参见 [PushPlus 官方网站](https://www.pushplus.plus/)。

可以使用 `dotnet run -- [ncov|tcheck] --help` 查看所有的命令及其选项。

### 示例

`--username`、`--password` 和 `--token` 可以分别简写为 `-u`、`-p` 和 `-k`。

#### 提交健康卡信息

- 立即提交

  `dotnet run -- ncov -u 1234567890 -p aabbccd`

- 立即提交并接收 PushPlus 通知

  `dotnet run -- ncov -u 1234567890 -p aabbccd -k 1a2a3d4f`

#### 提交晨午晚检信息

**注意，在提交晨午晚检信息时，必须指定 `--campus`（或 `-c`）选项，值可以是 `N` 或 `S`。**

- 立即提交

  `dotnet run -- tcheck -u 1234567890 -p aabbccd -c N`

- 立即提交并接收 PushPlus 通知

  `dotnet run -- tcheck -u 1234567890 -p aabbccd -c N -k 1a2a3d4f`

## 高级用法：结合 GitHub Action

### 太长不看版

Fork 此 Repo，然后按照下面“创建 Secret”一节的说明创建好相应的 Secret。如果想的话还可以修改定时提交的时间。这样就够了。

### 完整说明

每天手动运行程序还是挺麻烦的，可以结合 GitHub Action 解放自己。

开始之前，先 Fork 此 Repo。肯定没人想把学号和密码存在我这里（当然这也不可能）。完成之后继续。

#### 创建 Secret

找到 Settings → Secrets，点击“New repository secret”来为学号和密码创建 Secret。例如，学号 Secret 的名称是 `USERNAME`，值是 `1234567890`；密码 Secret 的名称是 `PASSWORD`，值是 `aabbccd`。如果想结合 PushPlus 使用，再创建一个名称为 `SCKEY`，值为 `1a2a3d4f`（当然，换成自己 PushPlus 的 Token）。虽然最新版中已经用 PushPlus 换掉了 Serverchan，为了兼容，在示例和 GitHub Action 的配置文件中，仍使用 `SCKEY` 以向后兼容。

另外还需要两个 Secret。一个是 `FUNCTION`，值为 `tcheck`（如果想提交晨午晚检信息则为 `ncov`）。另一个是 `CAMPUS`，值为 `-c S`（如果住在北校区则为 `-c N`，提交晨午晚检信息时请设置为空值）。

#### 创建或编辑 Workflow

接下来转到 Actions，可以在建议中看到“.NET”。点击“Set up with this workflow”，在文本框中会出现一个 yaml 文件。`name` 字段可以随意修改。在 `on` 一节中，删除 `push` 和 `pull` 子项。添加下列内容：

```yaml
workflow_dispatch:
schedule:
    - cron: '0 0,4,10 * * *'
```

`workflow_dispatch` 允许手动运行 Workflow。如需手动运行，转到 Actions，点击左侧窗格中的“Auto NCov Report”，再点击右侧的“Run workflow”。`schedule` 允许 Workflow 在指定时间运行。

需要注意，GitHub 使用的时区是 UTC。这也就意味着，要使用 UTC+8 时区（北京时间），需要从目标时间中减去 8 小时。`0 0,4,12 * * *` 表示任务会在每天 8:00 UTC+8（0:00 UTC）、12:00 UTC+8（8:00 UTC）和 18:00 UTC+8（10:00 UTC）的时候运行。也可以指定其他时间。

然后，在 `steps` 一节中，将 `Build` 和 `Test` 替换为以下内容。这里立即提交并接收 PushPlus 通知为例：

```yaml
- name: Run and send notification
  run: |
    cd AutoXduNCovReport
    dotnet run -- ${{ secrets.FUNCTION }} -u ${{ secrets.USERNAME }} -p ${{ secrets.PASSWORD }} ${{ secrets.CAMPUS}} -k ${{ secrets.SCKEY }}
```

保存并提交 Workflow。然后可以尝试手动运行一次，并检查结果是否正确。

#### 切换晨午晚检和健康卡

在校期间需要填写晨午晚检，而假期在家时需要填写健康卡。按照上面的说明修改 `FUNCTION` 的值即可。改动在下次运行 Workflow 时生效。

## 注意

在填写健康卡时，会使用前一次填写的定位信息。因此如果需要修改定位，请在定时填写触发前提前在网页中手动填写。
