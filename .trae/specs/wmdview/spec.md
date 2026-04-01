# WMDView - Windows Markdown 查看器 - 产品需求文档

## Overview
- **Summary**: 一个轻量、快速、流畅的 Windows 原生 Markdown 预览器，使用 WPF/WinUI 3 + Markdig + 原生富文本渲染技术栈，无需 WebView。
- **Purpose**: 解决传统 Markdown 预览器体积大、启动慢、内存占用高、渲染模糊等问题，提供极致的阅读体验。
- **Target Users**: 需要频繁查看 Markdown 文档的开发者、写作者、学生等 Windows 用户。

## Goals
- 实现单窗口 Markdown 实时预览功能
- 体积控制在 2-5MB，启动秒开
- 内存占用极低（几 MB ~ 20MB）
- 支持完整的 GitHub Flavored Markdown 语法
- 自动跟随系统亮/暗主题
- 实时监听文件修改并自动刷新

## Non-Goals (Out of Scope)
- Markdown 编辑功能（仅预览）
- 多标签页支持
- 云端同步功能
- 导出为 PDF/Word 等格式
- 插件系统

## Background & Context
- 传统 Markdown 预览器多使用 WebView/Electron 技术，体积 80MB-150MB，启动慢，内存占用高
- Windows 原生控件提供更好的性能、清晰度和系统集成
- Markdig 是 .NET 生态最强大的 Markdown 解析库，支持完整的 GFM 语法
- WPF 提供成熟的 FlowDocument 富文本渲染能力

## Functional Requirements
- **FR-1**: 打开并解析 Markdown 文件
- **FR-2**: 将 Markdown 渲染为原生富文本
- **FR-3**: 支持完整的 GFM 语法（标题、粗体、斜体、列表、引用、代码块、表格、图片、链接、分割线、任务列表）
- **FR-4**: 实时监听文件修改并自动刷新预览
- **FR-5**: 自动跟随系统亮/暗主题
- **FR-6**: 窗口标题显示当前文件路径

## Non-Functional Requirements
- **NFR-1**: 启动时间 < 1 秒
- **NFR-2**: 安装包体积 2-5MB
- **NFR-3**: 内存占用 < 20MB（常规文档）
- **NFR-4**: 文件修改刷新延迟 < 100ms
- **NFR-5**: 支持 Windows ClearType 字体渲染
- **NFR-6**: 完美适配高分屏（DPI 缩放）
- **NFR-7**: 滚动丝滑无延迟

## Constraints
- **Technical**: 必须使用 .NET (WPF 或 WinUI 3) + Markdig + 原生富文本渲染，禁止使用 WebView
- **Business**: 无特定预算或 timeline 约束
- **Dependencies**: Markdig 库

## Assumptions
- 用户使用 Windows 操作系统
- .NET 运行时已安装（或自包含）
- Markdown 文件编码为 UTF-8

## Acceptance Criteria

### AC-1: 打开并渲染 Markdown 文件
- **Given**: 用户有一个有效的 Markdown 文件
- **When**: 用户通过应用打开该文件
- **Then**: 文件内容被正确解析并渲染为可读的富文本格式
- **Verification**: `human-judgment`

### AC-2: 支持完整 GFM 语法
- **Given**: 一个包含所有 GFM 语法元素的测试 Markdown 文件
- **When**: 应用打开该文件
- **Then**: 所有语法元素（标题、粗体、斜体、列表、引用、代码块、表格、图片、链接、分割线、任务列表）都被正确渲染
- **Verification**: `human-judgment`

### AC-3: 实时监听文件修改
- **Given**: 一个已打开的 Markdown 文件
- **When**: 用户在外部编辑器中修改并保存该文件
- **Then**: 应用自动检测到修改并在 100ms 内刷新预览
- **Verification**: `programmatic`

### AC-4: 启动性能
- **Given**: 应用已关闭
- **When**: 用户双击启动应用
- **Then**: 应用在 1 秒内完全启动并显示窗口
- **Verification**: `programmatic`

### AC-5: 系统主题跟随
- **Given**: 应用正在运行
- **When**: 用户切换 Windows 系统主题（亮模式 ↔ 暗模式）
- **Then**: 应用界面自动跟随系统主题变化
- **Verification**: `human-judgment`

### AC-6: 高分屏适配
- **Given**: 用户使用高分屏（如 4K）并设置了 DPI 缩放
- **When**: 应用打开并显示 Markdown 文档
- **Then**: 所有文本和 UI 元素清晰锐利，无模糊
- **Verification**: `human-judgment`

### AC-7: 滚动体验
- **Given**: 一个长 Markdown 文档已打开
- **When**: 用户滚动浏览文档
- **Then**: 滚动过程丝滑流畅，无卡顿或延迟
- **Verification**: `human-judgment`

### AC-8: 窗口标题显示
- **Given**: 一个 Markdown 文件已打开
- **When**: 查看应用窗口标题栏
- **Then**: 标题栏显示当前打开文件的完整路径
- **Verification**: `human-judgment`

## Open Questions
- [ ] 选择 WPF 还是 WinUI 3？
- [ ] 是否支持命令行参数打开文件？
- [ ] 是否需要托盘图标？
